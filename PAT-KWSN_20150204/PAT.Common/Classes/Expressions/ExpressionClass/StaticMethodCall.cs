using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using PAT.Common.Classes.CUDDLib;
using PAT.Common.Classes.SemanticModels.LTS.BDD;
using PAT.Common.Classes.Ultility;
using PAT.Common.Ultility;
namespace PAT.Common.Classes.Expressions.ExpressionClass
{
    public sealed class StaticMethodCall : Expression
    {
        public string MethodName;
        public Expression[] Arguments;

        public StaticMethodCall(String method, Expression[] args)
        {
            ExpressionType = ExpressionType.StaticMethodCall;
            MethodName = method;
            Arguments = args;
            StringBuilder sb = new StringBuilder("C@" + MethodName + "(");
            foreach (Expression exp in Arguments)
            {
                HasVar = HasVar || exp.HasVar;
                sb.Append(exp.ExpressionID + ",");
            }
            expressionID = sb.ToString();
        }

        public override String ToString()
        {
            if (Arguments.Length > 0)
            {
                StringBuilder sb = new StringBuilder("call(" + MethodName + ",");
                for (int i = 0; i < Arguments.Length; i++)
                {
                    if (i == Arguments.Length - 1)
                    {
                        sb.Append(Arguments[i].ToString());
                    }
                    else
                    {
                        sb.Append(Arguments[i].ToString() + ", ");
                    }
                }
                sb.Append(")");
                return sb.ToString();
            }
            else
            {
                return "call(" + MethodName + ")";
            }
        }
        
        //public override String GetID()
        //{
        //    StringBuilder sb = new StringBuilder("C@" + MethodName + "(");
        //    for (int i = 0; i < Arguments.Length; i++)
        //    {
        //        if (i == Arguments.Length - 1)
        //        {
        //            sb.Append(Arguments[i].GetID());
        //        }
        //        else
        //        {
        //            sb.Append(Arguments[i].GetID() + ",");
        //        }
        //    }
        //    sb.Append(")");
        //    return sb.ToString();
        //}

        public override List<string> GetVars()
        {
            List<string> vars = new List<string>(16);
            foreach (Expression exp in Arguments)
            {
                Ultility.Ultility.AddList(vars, exp.GetVars());
            }
            return vars;
        }

        public override Expression ClearConstant(Dictionary<string, Expression> constMapping)
        {
            Expression[] newArgs = new Expression[Arguments.Length];

            for (int i = 0; i < Arguments.Length; i++)
            {
                newArgs[i] = Arguments[i].ClearConstant(constMapping);
            }

            return new StaticMethodCall(MethodName, newArgs);
        }

        public override bool HasExternalLibraryCall()
        {
            //BDD can support call to channel properties
            return (MethodName != Constants.cfull && MethodName != Constants.cempty && MethodName != Constants.ccount
                    && MethodName != Constants.csize && MethodName != Constants.cpeek);
        }

        /// 
        /// <summary>
        /// Use when the boolen constant is used as expression: a = true
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override ExpressionBDDEncoding TranslateIntExpToBDD(Model model)
        {
            string channelName = Arguments[0].ToString();
            string countChannelVariable = Model.GetCountVarChannel(channelName);
            string topChannelVariable = Model.GetTopVarChannel(channelName);

            switch (MethodName)
            {
                case Constants.cfull:
                    Expression isFull = Expression.EQ(new Variable(countChannelVariable), new IntConstant(model.mapChannelToSize[channelName]));
                    ExpressionBDDEncoding isFullTemp = isFull.TranslateBoolExpToBDD(model);
                    isFullTemp.ExpressionDDs = new List<CUDDNode>(isFullTemp.GuardDDs);
                    isFullTemp.GuardDDs = new List<CUDDNode>() { CUDD.Constant(1) };
                    return isFullTemp;
                case Constants.cempty:
                    Expression isEmpty = Expression.EQ(new Variable(countChannelVariable), new IntConstant(0));
                    ExpressionBDDEncoding isEmptyTemp = isEmpty.TranslateBoolExpToBDD(model);
                    isEmptyTemp.ExpressionDDs = new List<CUDDNode>(isEmptyTemp.GuardDDs);
                    isEmptyTemp.GuardDDs = new List<CUDDNode>() { CUDD.Constant(1) };
                    return isEmptyTemp;
                case Constants.ccount:
                    return new Variable(countChannelVariable).TranslateIntExpToBDD(model);
                case Constants.csize:
                    return new IntConstant(model.mapChannelToSize[channelName]).TranslateIntExpToBDD(model);
                case Constants.cpeek:
                    //(top_a - count_a) % L
                    Expression popedElementPosition = new PrimitiveApplication(PrimitiveApplication.MOD,
                                                Expression.MINUS(new Variable(topChannelVariable), new Variable(countChannelVariable)),
                                                new IntConstant(model.mapChannelToSize[channelName]));

                    //a[top_a - count_a % L][i]
                    return new PrimitiveApplication(PrimitiveApplication.ARRAY, new Variable(channelName), Expression.TIMES(
                                                                                                                                        popedElementPosition,
                                                                                                                                        new IntConstant(Model.MAX_MESSAGE_LENGTH))
                                                                                                                                    ).TranslateIntExpToBDD(model);
            }

            throw new Exception("Static call can not be encoded in BDD");

        }

        /// 
        /// <summary>
        /// Use when the boolen constant is used as expression: a = true
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override ExpressionBDDEncoding TranslateBoolExpToBDD(Model model)
        {
            string channelName = Arguments[0].ToString();
            string countChannelVariable = Model.GetCountVarChannel(channelName);

            switch (MethodName)
            {
                case Constants.cfull:
                    Expression isFull = Expression.EQ(new Variable(countChannelVariable), new IntConstant(model.mapChannelToSize[channelName]));
                    return isFull.TranslateBoolExpToBDD(model);
                case Constants.cempty:
                    Expression isEmpty = Expression.EQ(new Variable(countChannelVariable), new IntConstant(0));
                    return isEmpty.TranslateBoolExpToBDD(model);
            }

            throw new Exception("Static call can not be encoded in BDD");

        }

        #if DEBUG
        public override void GenerateMSIL(ILGenerator ilGenerator, TypeBuilder typeBuilder)
        {
            FieldInfo fb;
            Type type;
            
            //TODO Get size of this channel
            int size = 0;

            switch (MethodName)
            {
                case Constants.cfull:
                    fb = typeBuilder.GetField(Arguments[0].ExpressionID);
                    ilGenerator.Emit(OpCodes.Ldsfld, fb);
                    type = fb.GetType();
                    ilGenerator.Emit(OpCodes.Callvirt, type.GetMethod("get_Count"));
                    new IntConstant(size).GenerateMSIL(ilGenerator, typeBuilder);
                    ilGenerator.Emit(OpCodes.Ceq);
                    break;
                case Constants.cempty:
                    fb = typeBuilder.GetField(Arguments[0].ExpressionID);
                    ilGenerator.Emit(OpCodes.Ldsfld, fb);
                    type = fb.GetType();
                    ilGenerator.Emit(OpCodes.Callvirt, type.GetMethod("get_Count"));
                    ilGenerator.Emit(OpCodes.Ldc_I4_0);
                    ilGenerator.Emit(OpCodes.Ceq);
                    break;
                case Constants.ccount:
                    fb = typeBuilder.GetField(Arguments[0].ExpressionID);
                    ilGenerator.Emit(OpCodes.Ldsfld, fb);
                    type = fb.GetType();
                    ilGenerator.Emit(OpCodes.Callvirt, type.GetMethod("get_Count"));              
                    break;
                case Constants.cpeek:
                    fb = typeBuilder.GetField(Arguments[0].ExpressionID);
                    ilGenerator.Emit(OpCodes.Ldsfld, fb);
                    type = fb.GetType();
                    ilGenerator.Emit(OpCodes.Callvirt, type.GetMethod("Peek"));     
                    break;
                case Constants.csize:
                    new IntConstant(size).GenerateMSIL(ilGenerator, typeBuilder);
                    break;
                default:
                    string key = MethodName + Arguments.Length;

                    MethodInfo methodInfo = Common.Ultility.Ultility.CSharpMethods[key];

                    ilGenerator.Emit(OpCodes.Call, methodInfo);
                    break;
            }
        }
#endif
    }
}