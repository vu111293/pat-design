using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using PAT.Common.Classes.CUDDLib;
using PAT.Common.Classes.SemanticModels.LTS.BDD;


namespace PAT.Common.Classes.Expressions.ExpressionClass
{
    /// <summary>
    /// ExpressionBDDEncoding to suport array expression.
    /// For example: a[i] then the GuardDDs will contains condition of value of i, i = 0, i = 1,...,
    /// ExpressionDDs will contain the corresponding expression of a[i], a[0], a[1], a[2]...
    /// After finishing encoding a boolean expresion which does not consist of any ADD, then only need to use GuarDDS
    /// Then just get the GuardDDs[0]
    /// </summary>
    public class ExpressionBDDEncoding
    {
        /// <summary>
        /// BDD
        /// When ExpressionBDDEncoding is a result of an complete expression, GuardDDs only contains 1 elements
        /// </summary>
        public List<CUDDNode> GuardDDs;

        /// <summary>
        /// ADD
        /// When ExpressionBDDEncoding is a result of an incomplete expression, ExpressionDDs is not empty
        /// </summary>
        public List<CUDDNode> ExpressionDDs;

       
        public ExpressionBDDEncoding()
        {
            this.GuardDDs = new List<CUDDNode>();
            this.ExpressionDDs = new List<CUDDNode>();
        }

        /// <summary>
        /// Some expression we only care with guard, not expression  like boolean expression.
        /// In such cases, expression is empty. So size should get from guards
        /// </summary>
        public int Count()
        {
            return this.GuardDDs.Count;
        }

        public void DeRef()
        {
            CUDD.Deref(this.GuardDDs);
            CUDD.Deref(this.ExpressionDDs);
        }

        public void Ref()
        {
            CUDD.Ref(this.GuardDDs);
            CUDD.Ref(this.ExpressionDDs);
        }

        /// <summary>
        /// [ REFS: 'none', DEREFS: 'dd if failed to add' ]
        /// </summary>
        public void AddNodeToGuard(CUDDNode dd)
        {
            if (!dd.Equals(CUDD.ZERO))
            {
                this.GuardDDs.Add(dd);
            }
            else
            {
                CUDD.Deref(dd);
            }
        }

        /// <summary>
        /// [ REFS: 'none', DEREFS: 'dd if failed to add' ]
        /// </summary>
        public void AddNodeToGuard(List<CUDDNode> dds)
        {
            foreach (CUDDNode dd in dds)
            {
                this.AddNodeToGuard(dd);
            }
        }
    }

    public enum ExpressionType : byte
    {
        Constant,
        Variable,
        Record,
        PrimitiveApplication,
        Assignment,
        PropertyAssignment,
        If,
        Sequence,
        While,
        Let,
        StaticMethodCall,
        ClassMethodCall,
        NewObjectCreation,
        ClassMethodCallInstance,
        ClassProperty,
        ClassPropertyAssignment
        //Fun,
        //RecFun,
        //Application

    }

    [Serializable]
    public abstract class Expression
    {
        public ExpressionType ExpressionType;
        internal string expressionID;
        public virtual string ExpressionID
        {
            get
            {
                return expressionID;
            }
        }

        public virtual List<string> GetVars()
        {
            return new List<string>(0);
        }

        public bool HasVar;

        public virtual Expression ClearConstant(Dictionary<string, Expression> constMapping)
        {
            return this;
        }

        /// <summary>
        /// Encode a boolean expression
        /// [ REFS: 'result', DEREFS: '' ]
        /// </summary>
        public virtual ExpressionBDDEncoding TranslateBoolExpToBDD(Model model)
        {
            return new ExpressionBDDEncoding();
        }

        /// <summary>
        /// Encode an arithmetic expression
        /// [ REFS: 'result', DEREFS: '' ]
        /// </summary>
        public virtual ExpressionBDDEncoding TranslateIntExpToBDD(Model model)
        {
            return new ExpressionBDDEncoding();
        }

        /// <summary>
        /// For Update, If While, Sequence. Based on the current variable values in resultBefore, return the variable values after the statement is executed
        /// [ REFS: 'result', DEREFS: 'resultBefore' ]
        /// </summary>
        public virtual ExpressionBDDEncoding TranslateStatementToBDD(ExpressionBDDEncoding resultBefore, Model model)
        {
            return new ExpressionBDDEncoding();
        }

        public virtual bool HasExternalLibraryCall () 
        {
            return false;
        }

        /// <summary>
        /// Return guard1 and guard2
        /// </summary>
        /// <param name="guard1"></param>
        /// <param name="guard2"></param>
        /// <returns></returns>
        public static Expression CombineGuard(Expression guard1, Expression guard2)
        {
            if (guard1 != null && guard2 != null)
            {
                return Expression.AND(guard1, guard2);
            }
            else if (guard1 == null)
            {
                return guard2;
            }
            else
            {
                return guard1;
            }
        }

        /// <summary>
        /// Return block1;block2
        /// </summary>
        /// <param name="block1"></param>
        /// <param name="block2"></param>
        /// <returns></returns>
        public static Expression CombineProgramBlock(Expression block1, Expression block2)
        {
            if (block1 != null && block2 != null)
            {
                return new Sequence(block1, block2);
            }
            else if (block1 == null)
            {
                return block2;
            }
            else
            {
                return block1;
            }
        }

        /// <summary>
        /// And of more than 1 expression
        /// </summary>
        /// <param name="exps"></param>
        /// <returns></returns>
        public static Expression AND(params Expression[] exps)
        {
            Expression result = exps[0];
            for(int i = 1; i < exps.Length; i++)
            {
                result = new PrimitiveApplication(PrimitiveApplication.AND, result, exps[i]);
            }
            return result;
        }

        /// <summary>
        /// Or of not empty expressions
        /// </summary>
        /// <param name="exps"></param>
        /// <returns></returns>
        public static Expression OR(params Expression[] exps)
        {
            Expression result = exps[0];
            for (int i = 1; i < exps.Length; i++)
            {
                result = new PrimitiveApplication(PrimitiveApplication.OR, result, exps[i]);
            }
            return result;
        }

        /// <summary>
        /// Return negation of the boolean expression exp1
        /// </summary>
        /// <param name="exp1"></param>
        /// <returns></returns>
        public static Expression NOT(Expression exp1)
        {
            return new PrimitiveApplication(PrimitiveApplication.NOT, exp1);
        }

        /// <summary>
        /// Return sum of more than 1 expression
        /// </summary>
        /// <param name="exps"></param>
        /// <returns></returns>
        public static Expression PLUS(params Expression[] exps)
        {
            Expression result = exps[0];
            for (int i = 1; i < exps.Length; i++)
            {
                result = new PrimitiveApplication(PrimitiveApplication.PLUS, result, exps[i]);
            }
            return result;
        }

        /// <summary>
        /// Return exp1 minus exp2
        /// </summary>
        /// <param name="exp1"></param>
        /// <param name="exp2"></param>
        /// <returns></returns>
        public static Expression MINUS(Expression exp1, Expression exp2)
        {
            return new PrimitiveApplication(PrimitiveApplication.MINUS, exp1, exp2);
        }

        /// <summary>
        /// Return product of more than 1 expression
        /// </summary>
        /// <param name="exps"></param>
        /// <returns></returns>
        public static Expression TIMES(params Expression[] exps)
        {
            Expression result = exps[0];
            for (int i = 1; i < exps.Length; i++)
            {
                result = new PrimitiveApplication(PrimitiveApplication.TIMES, result, exps[i]);
            }
            return result;
        }

        /// <summary>
        /// Divide
        /// </summary>
        /// <param name="exp1"></param>
        /// <param name="exp2"></param>
        /// <returns></returns>
        public static Expression DIVIDE(Expression exp1, Expression exp2)
        {
            return new PrimitiveApplication(PrimitiveApplication.DIVIDE, exp1, exp2);
        }

        /// <summary>
        /// Mod
        /// </summary>
        /// <param name="exp1"></param>
        /// <param name="exp2"></param>
        /// <returns></returns>
        public static Expression MOD(Expression exp1, Expression exp2)
        {
            return new PrimitiveApplication(PrimitiveApplication.MOD, exp1, exp2);
        }

        /// <summary>
        /// Negative
        /// </summary>
        /// <param name="exp1"></param>
        /// <param name="exp2"></param>
        /// <returns></returns>
        public static Expression NEG(Expression exp1, Expression exp2)
        {
            return new PrimitiveApplication(PrimitiveApplication.NEGATIVE, exp1, exp2);
        }

        /// <summary>
        /// Equal
        /// </summary>
        /// <param name="exp1"></param>
        /// <param name="exp2"></param>
        /// <returns></returns>
        public static Expression EQ(Expression exp1, Expression exp2)
        {
            return new PrimitiveApplication(PrimitiveApplication.EQUAL, exp1, exp2);
        }

        /// <summary>
        /// Not equal
        /// </summary>
        /// <param name="exp1"></param>
        /// <param name="exp2"></param>
        /// <returns></returns>
        public static Expression NE(Expression exp1, Expression exp2)
        {
            return new PrimitiveApplication(PrimitiveApplication.NOT_EQUAL, exp1, exp2);
        }

        /// <summary>
        /// Greater
        /// </summary>
        /// <param name="exp1"></param>
        /// <param name="exp2"></param>
        /// <returns></returns>
        public static Expression GT(Expression exp1, Expression exp2)
        {
            return new PrimitiveApplication(PrimitiveApplication.GREATER, exp1, exp2);
        }

        /// <summary>
        /// Greater or equal
        /// </summary>
        /// <param name="exp1"></param>
        /// <param name="exp2"></param>
        /// <returns></returns>
        public static Expression GE(Expression exp1, Expression exp2)
        {
            return new PrimitiveApplication(PrimitiveApplication.GREATER_EQUAL, exp1, exp2);
        }

        /// <summary>
        /// Less than
        /// </summary>
        /// <param name="exp1"></param>
        /// <param name="exp2"></param>
        /// <returns></returns>
        public static Expression LT(Expression exp1, Expression exp2)
        {
            return new PrimitiveApplication(PrimitiveApplication.LESS, exp1, exp2);
        }

        /// <summary>
        /// Less or equal
        /// </summary>
        /// <param name="exp1"></param>
        /// <param name="exp2"></param>
        /// <returns></returns>
        public static Expression LE(Expression exp1, Expression exp2)
        {
            return new PrimitiveApplication(PrimitiveApplication.LESS_EQUAL, exp1, exp2);
        }
        
#if DEBUG
        public virtual void GenerateMSIL(ILGenerator ilGenerator, TypeBuilder typeBuilder)
        {
            return;
        }
#endif 
    }

    [Serializable]
    public abstract class ExpressionValue : Expression
    {
        //public abstract string GetID();

        public virtual ExpressionValue GetClone()
        {
            try
            {
                //return this;
                if (!this.GetType().IsSerializable)
                {
                    throw new ArgumentException(
                        string.Format(
                            "External C# type {0} must be serializable or overrite GetClone method manually!",
                            this.GetType().ToString()));
                }
                
                //using (MemoryStream stream = new MemoryStream())
                //{
                //    BinaryFormatter formatter = new BinaryFormatter();
                //    formatter.Serialize(stream, this);
                //    stream.Position = 0;
                //    return (ExpressionValue)formatter.Deserialize(stream);
                //} 

                BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream ms = new MemoryStream();
                // To prevent errors serializing between version number differences (e.g. Version 1 serializes, and Version 2 deserializes)   
                formatter.Serialize(ms, this);
                ms.Position = 0;
                formatter.Binder = new AllowAllAssemblyVersionsDeserializationBinder();
                // Allow the exceptions to bubble up    // System.ArgumentNullException    // System.Runtime.Serialization.SerializationException    // System.Security.SecurityException   
                ExpressionValue mro = (ExpressionValue) formatter.Deserialize(ms);
                ms.Close();
                return mro;

            }
            catch (Exception ex)
            {
                throw new RuntimeException(string.Format("Runtime exception when cloning the object {0}. Please check the serialization implementation of the class.", this.GetType().ToString()));
            }
        }
    }


    /// <summary>
    /// http://spazzarama.com/2009/06/25/binary-deserialize-unable-to-find-assembly/
    /// </summary>
    internal sealed class AllowAllAssemblyVersionsDeserializationBinder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            foreach (Type type in PAT.Common.Ultility.Ultility.CSharpDataType.Values)
            {
                if(type.FullName == typeName)
                {
                    return type;
                }
            }

            Type typeToDeserialize = null;
            String currentAssembly = Assembly.GetExecutingAssembly().FullName;
            // In this case we are always using the current assembly       
            assemblyName = currentAssembly; // Get the type using the typeName and assemblyName
            typeToDeserialize = Type.GetType(String.Format("{0}, {1}", typeName, assemblyName));
            return typeToDeserialize;
        }
    }
}
