using System;
using System.Collections.Generic;
using System.Diagnostics;
using PAT.Common.Classes.CUDDLib;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.SemanticModels.Prob.Encoding;
using PAT.Common.Classes.SemanticModels.Prob.ExpressionClass;
using PAT.Common.Classes.SemanticModels.Prob.SystemStructure;
using PAT.Common.Classes.SemanticModels.Prob.Verification;

namespace PAT.Common.Classes.SemanticModels.Prob
{
    public class Test
    {
        private static Modules Example1()
        {
            List<VarDeclaration> globalVars = new List<VarDeclaration>();

            List<Command> commands = new List<Command>();
            List<VarDeclaration> localVars = new List<VarDeclaration>();
            Module module = new Module("DTMC", commands, localVars);

            string state = "state";
            localVars.Add(new VarDeclaration(state, 0, 5, 0));

            List<Update> updates = new List<Update>();
            updates.Add(new Update(new List<Assignment>() { new Assignment(state, new IntConstant(1)) }, 0.1));
            updates.Add(new Update(new List<Assignment>() { new Assignment(state, new IntConstant(2)) }, 0.9));
            commands.Add(new Command(Expression.EQ(new Variable(state), new IntConstant(0)), updates));

            updates = new List<Update>();
            updates.Add(new Update(new List<Assignment>() { new Assignment(state, new IntConstant(0)) }, 0.4));
            updates.Add(new Update(new List<Assignment>() { new Assignment(state, new IntConstant(3)) }, 0.6));
            commands.Add(new Command(Expression.EQ(new Variable(state), new IntConstant(1)), updates));

            updates = new List<Update>();
            updates.Add(new Update(new List<Assignment>() { new Assignment(state, new IntConstant(2)) }, 0.1));
            updates.Add(new Update(new List<Assignment>() { new Assignment(state, new IntConstant(3)) }, 0.1));
            updates.Add(new Update(new List<Assignment>() { new Assignment(state, new IntConstant(4)) }, 0.5));
            updates.Add(new Update(new List<Assignment>() { new Assignment(state, new IntConstant(5)) }, 0.3));
            commands.Add(new Command(Expression.EQ(new Variable(state), new IntConstant(2)), updates));

            updates = new List<Update>();
            updates.Add(new Update(new List<Assignment>() { new Assignment(state, new IntConstant(3)) }, 1));
            commands.Add(new Command(Expression.EQ(new Variable(state), new IntConstant(3)), updates));

            updates = new List<Update>();
            updates.Add(new Update(new List<Assignment>() { new Assignment(state, new IntConstant(4)) }, 1));
            commands.Add(new Command(Expression.EQ(new Variable(state), new IntConstant(4)), updates));


            updates = new List<Update>();
            updates.Add(new Update(new List<Assignment>() { new Assignment(state, new IntConstant(4)) }, 0.7));
            updates.Add(new Update(new List<Assignment>() { new Assignment(state, new IntConstant(5)) }, 0.3));
            commands.Add(new Command(Expression.EQ(new Variable(state), new IntConstant(5)), updates));


            SystemDef systemDef = new SingleModuleSystem("DTMC");

            List<RewardItem> rewardItems = new List<RewardItem>();
            rewardItems.Add(new RewardItem(null, new BoolConstant(true), new DoubleConstant(0.7)));
            rewardItems.Add(new RewardItem(string.Empty, new BoolConstant(true), new DoubleConstant(0.5)));
            RewardStruct rewardStruct = new RewardStruct("steps", rewardItems);

            Modules result = new Modules(ModelType.DTMC, globalVars, new List<Module>() { module }, new List<RewardStruct>() { rewardStruct }, systemDef, null);

            return result;
        }

        public static void Test1()
        {
            Modules modules = Example1();

            ModulesToBDD modulesToBdd = new ModulesToBDD(modules);
            ProbModel probModel = (ProbModel)modulesToBdd.Encode();


            ProbModelChecker probMC = new ProbModelChecker(probModel);

            CUDDNode phi1 = probMC.EncodeExpression(Expression.NE(new Variable("state"), new IntConstant(1)));
            CUDDNode phi2 = probMC.EncodeExpression(Expression.EQ(new Variable("state"), new IntConstant(4)));

            probMC.GetResult(probMC.ComputeUntil(phi1, phi2), probMC.start);
            probMC.GetResult(probMC.ComputeReachReward(phi2, 0), probMC.start);

            probMC.Close();
        }

        private static Modules Example2()
        {
            List<VarDeclaration> globalVars = new List<VarDeclaration>();

            List<Command> commands = new List<Command>();
            List<VarDeclaration> localVars = new List<VarDeclaration>();
            Module module = new Module("MDP", commands, localVars);

            string state = "state";
            localVars.Add(new VarDeclaration(state, 0, 3, 0));

            List<Update> updates = new List<Update>();
            updates.Add(new Update(new List<Assignment>() { new Assignment(state, new IntConstant(0)) }, 0.25));
            updates.Add(new Update(new List<Assignment>() { new Assignment(state, new IntConstant(2)) }, 0.5));
            updates.Add(new Update(new List<Assignment>() { new Assignment(state, new IntConstant(3)) }, 0.25));
            commands.Add(new Command(Expression.EQ(new Variable(state), new IntConstant(0)), updates));

            updates = new List<Update>();
            updates.Add(new Update(new List<Assignment>() { new Assignment(state, new IntConstant(1)) }, 1));
            commands.Add(new Command(Expression.EQ(new Variable(state), new IntConstant(0)), updates));

            updates = new List<Update>();
            updates.Add(new Update(new List<Assignment>() { new Assignment(state, new IntConstant(0)) }, 0.1));
            updates.Add(new Update(new List<Assignment>() { new Assignment(state, new IntConstant(1)) }, 0.5));
            updates.Add(new Update(new List<Assignment>() { new Assignment(state, new IntConstant(2)) }, 0.4));
            commands.Add(new Command(Expression.EQ(new Variable(state), new IntConstant(1)), updates));

            updates = new List<Update>();
            updates.Add(new Update(new List<Assignment>() { new Assignment(state, new IntConstant(2)) }, 1));
            commands.Add(new Command(Expression.EQ(new Variable(state), new IntConstant(2)), updates));

            updates = new List<Update>();
            updates.Add(new Update(new List<Assignment>() { new Assignment(state, new IntConstant(3)) }, 1));
            commands.Add(new Command(Expression.EQ(new Variable(state), new IntConstant(3)), updates));

            updates = new List<Update>();
            updates.Add(new Update(new List<Assignment>() { new Assignment(state, new IntConstant(2)) }, 1));
            commands.Add(new Command(Expression.EQ(new Variable(state), new IntConstant(3)), updates));

            SystemDef systemDef = new SingleModuleSystem("MDP");

            List<RewardItem> rewardItems = new List<RewardItem>();
            rewardItems.Add(new RewardItem(null, new BoolConstant(true), new IntConstant(1)));
            rewardItems.Add(new RewardItem(string.Empty, new BoolConstant(true), new DoubleConstant(0.5)));
            RewardStruct rewardStruct = new RewardStruct("steps", rewardItems);

            Modules result = new Modules(ModelType.MDP, globalVars, new List<Module>() { module }, new List<RewardStruct>() { rewardStruct }, systemDef, null);

            return result;
        }

        public static void Test2()
        {
            Modules modules = Example2();

            ModulesToBDD modulesToBdd = new ModulesToBDD(modules);
            NonDetModel nondetModel = (NonDetModel)modulesToBdd.Encode();


            NondetModelChecker nondetMC = new NondetModelChecker(nondetModel);

            CUDDNode phi1 = nondetMC.EncodeExpression(Expression.EQ(new Variable("state"), new IntConstant(2)));

            nondetMC.GetResult(nondetMC.ComputeFuture(phi1, true), nondetMC.start);
            nondetMC.GetResult(nondetMC.ComputeFuture(phi1, false), nondetMC.start);
            nondetMC.GetResult(nondetMC.ComputeReachReward(phi1, 0, true), nondetMC.start);
            nondetMC.GetResult(nondetMC.ComputeReachReward(phi1, 0, false), nondetMC.start);
            nondetMC.GetResult(nondetMC.ComputeInstReward(0, 10, true), nondetMC.start);
            nondetMC.GetResult(nondetMC.ComputeInstReward(0, 10, false), nondetMC.start);

            nondetMC.Close();
        }

        private static Modules Example3()
        {
            List<VarDeclaration> globalVars = new List<VarDeclaration>();

            List<Command> commands = new List<Command>();
            List<VarDeclaration> localVars = new List<VarDeclaration>();
            Module dieModule = new Module("die", commands, localVars);

            string s = "s";
            string d = "d";
            localVars.Add(new VarDeclaration(s, 0, 7, 0));
            localVars.Add(new VarDeclaration(d, 0, 6, 0));

            List<Update> updates = new List<Update>();
            updates.Add(new Update(new List<Assignment>() { new Assignment(s, new IntConstant(1)) }, 0.5));
            updates.Add(new Update(new List<Assignment>() { new Assignment(s, new IntConstant(2)) }, 0.5));
            commands.Add(new Command(Expression.EQ(new Variable(s), new IntConstant(0)), updates));

            updates = new List<Update>();
            updates.Add(new Update(new List<Assignment>() { new Assignment(s, new IntConstant(3)) }, 0.5));
            updates.Add(new Update(new List<Assignment>() { new Assignment(s, new IntConstant(4)) }, 0.5));
            commands.Add(new Command(Expression.EQ(new Variable(s), new IntConstant(1)), updates));

            updates = new List<Update>();
            updates.Add(new Update(new List<Assignment>() { new Assignment(s, new IntConstant(5)) }, 0.5));
            updates.Add(new Update(new List<Assignment>() { new Assignment(s, new IntConstant(6)) }, 0.5));
            commands.Add(new Command(Expression.EQ(new Variable(s), new IntConstant(2)), updates));

            updates = new List<Update>();
            updates.Add(new Update(new List<Assignment>() { new Assignment(s, new IntConstant(1)) }, 0.5));
            updates.Add(new Update(new List<Assignment>() { new Assignment(s, new IntConstant(7)), new Assignment(d, new IntConstant(1)) }, 0.5));
            commands.Add(new Command(Expression.EQ(new Variable(s), new IntConstant(3)), updates));

            updates = new List<Update>();
            updates.Add(new Update(new List<Assignment>() { new Assignment(s, new IntConstant(7)), new Assignment(d, new IntConstant(2)) }, 0.5));
            updates.Add(new Update(new List<Assignment>() { new Assignment(s, new IntConstant(7)), new Assignment(d, new IntConstant(3)) }, 0.5));
            commands.Add(new Command(Expression.EQ(new Variable(s), new IntConstant(4)), updates));

            updates = new List<Update>();
            updates.Add(new Update(new List<Assignment>() { new Assignment(s, new IntConstant(7)), new Assignment(d, new IntConstant(4)) }, 0.5));
            updates.Add(new Update(new List<Assignment>() { new Assignment(s, new IntConstant(7)), new Assignment(d, new IntConstant(5)) }, 0.5));
            commands.Add(new Command(Expression.EQ(new Variable(s), new IntConstant(5)), updates));


            updates = new List<Update>();
            updates.Add(new Update(new List<Assignment>() { new Assignment(s, new IntConstant(2)) }, 0.5));
            updates.Add(new Update(new List<Assignment>() { new Assignment(s, new IntConstant(7)), new Assignment(d, new IntConstant(6)) }, 0.5));
            commands.Add(new Command(Expression.EQ(new Variable(s), new IntConstant(6)), updates));

            updates = new List<Update>();
            updates.Add(new Update(new List<Assignment>() { new Assignment(s, new IntConstant(7)) }, 1));
            commands.Add(new Command(Expression.EQ(new Variable(s), new IntConstant(7)), updates));

            SystemDef systemDef = new SingleModuleSystem("die");

            List<RewardItem> rewardItems = new List<RewardItem>();
            rewardItems.Add(new RewardItem(null, Expression.LT(new Variable(s), new IntConstant(7)), new IntConstant(1)));
            rewardItems.Add(new RewardItem(string.Empty, Expression.EQ(new Variable(s), new IntConstant(3)), new DoubleConstant(0.5)));
            RewardStruct rewardStruct = new RewardStruct("coin_flips", rewardItems);

            Modules result = new Modules(ModelType.DTMC, globalVars, new List<Module>() { dieModule }, new List<RewardStruct>() { rewardStruct }, systemDef, null);

            return result;
        }

        public static void Test3()
        {
            Modules modules = Example3();

            ModulesToBDD modulesToBdd = new ModulesToBDD(modules);
            ProbModel probModel = (ProbModel) modulesToBdd.Encode();

            ProbModelChecker probModelChecker = new ProbModelChecker(probModel);

            CUDDNode phi = probModelChecker.EncodeExpression(Expression.AND(Expression.EQ(new Variable("s"), new IntConstant(7)), Expression.EQ(new Variable("d"), new IntConstant(6))));
            probModelChecker.GetResult(probModelChecker.ComputeFuture(phi), probModelChecker.start);

            CUDDNode phi1 = probModelChecker.EncodeExpression(Expression.EQ(new Variable("s"), new IntConstant(7)));
            probModelChecker.GetResult(probModelChecker.ComputeReachReward(phi1, 0), probModelChecker.start);

            probModelChecker.Close();
        }

        private static Modules Example4()
        {
            List<VarDeclaration> globalVars = new List<VarDeclaration>();

            List<Module> allModules = new List<Module>();
            string x1 = "x1";
            string x2 = "x2";
            string x3 = "x3";
            string x4 = "x4";
            string x5 = "x5";
            string x6 = "x6";
            string x7 = "x7";

            List<Command> commands = new List<Command>();
            List<VarDeclaration> localVars = new List<VarDeclaration>();
            Module process1 = new Module("process1", commands, localVars);

            localVars.Add(new VarDeclaration(x1, 0, 1));

            List<Update> updates = new List<Update>();
            updates.Add(new Update(new List<Assignment>() { new Assignment(x1, new IntConstant(0)) }, 0.5));
            updates.Add(new Update(new List<Assignment>() { new Assignment(x1, new IntConstant(1)) }, 0.5));
            commands.Add(new Command("step", Expression.EQ(new Variable(x1), new Variable(x7)), updates));

            updates = new List<Update>();
            updates.Add(new Update(new List<Assignment>() { new Assignment(x1, new Variable(x7)) }, 1));
            commands.Add(new Command("step", Expression.NE(new Variable(x1), new Variable(x7)), updates));

            allModules.Add(process1);

            Module process2 = process1.Rename("process2", new Dictionary<string, string>() { { x1, x2 }, { x7, x1 } });
            allModules.Add(process2);

            Module process3 = process1.Rename("process3", new Dictionary<string, string>() { { x1, x3 }, { x7, x2 } });
            allModules.Add(process3);

            Module process4 = process1.Rename("process4", new Dictionary<string, string>() { { x1, x4 }, { x7, x3 } });
            allModules.Add(process4);

            Module process5 = process1.Rename("process5", new Dictionary<string, string>() { { x1, x5 }, { x7, x4 } });
            allModules.Add(process5);

            Module process6 = process1.Rename("process6", new Dictionary<string, string>() { { x1, x6 }, { x7, x5 } });
            allModules.Add(process6);

            Module process7 = process1.Rename("process7", new Dictionary<string, string>() { { x1, x7 }, { x7, x6 } });
            allModules.Add(process7);

            SystemDef systemDef1 = new SingleModuleSystem("process1");
            SystemDef systemDef2 = new SingleModuleSystem("process2");
            SystemDef systemDef3 = new SingleModuleSystem("process3");
            SystemDef systemDef4 = new SingleModuleSystem("process4");
            SystemDef systemDef5 = new SingleModuleSystem("process5");
            SystemDef systemDef6 = new SingleModuleSystem("process6");
            SystemDef systemDef7 = new SingleModuleSystem("process7");

            SystemDef systemDef = new FullParallelSystem(new List<SystemDef>() { systemDef1, systemDef2, systemDef3, systemDef4, systemDef5, systemDef6, systemDef7 });

            List<RewardItem> rewardItems = new List<RewardItem>();
            rewardItems.Add(new RewardItem(null, new BoolConstant(true), new IntConstant(1)));
            RewardStruct rewardStruct = new RewardStruct("steps", rewardItems);

            Modules result = new Modules(ModelType.DTMC, globalVars, allModules, new List<RewardStruct>() { rewardStruct }, systemDef, new BoolConstant(true));

            return result;
        }

        public static void Test4()
        {
            string x1 = "x1";
            string x2 = "x2";
            string x3 = "x3";
            string x4 = "x4";
            string x5 = "x5";
            string x6 = "x6";
            string x7 = "x7";

            Modules modules = Example4();

            ModulesToBDD modulesToBdd = new ModulesToBDD(modules);
            ProbModel probModel = (ProbModel) modulesToBdd.Encode();

            ProbModelChecker probModelChecker = new ProbModelChecker(probModel);

            CUDDNode phi1 = probModelChecker.EncodeExpression(Expression.EQ(new Variable(x1), new Variable(x2)));
            CUDDNode phi2 = probModelChecker.EncodeExpression(Expression.EQ(new Variable(x2), new Variable(x3)));
            CUDDNode phi3 = probModelChecker.EncodeExpression(Expression.EQ(new Variable(x3), new Variable(x4)));
            CUDDNode phi4 = probModelChecker.EncodeExpression(Expression.EQ(new Variable(x4), new Variable(x5)));
            CUDDNode phi5 = probModelChecker.EncodeExpression(Expression.EQ(new Variable(x5), new Variable(x6)));
            CUDDNode phi6 = probModelChecker.EncodeExpression(Expression.EQ(new Variable(x6), new Variable(x7)));
            CUDDNode phi7 = probModelChecker.EncodeExpression(Expression.EQ(new Variable(x7), new Variable(x1)));

            CUDDNode numToken = CUDD.Function.Plus(phi1,
                                                   CUDD.Function.Plus(phi2,
                                                                      CUDD.Function.Plus(phi3, CUDD.Function.Plus(phi4, CUDD.Function.Plus(phi5, CUDD.Function.Plus(phi6, phi7))))));
            CUDD.Ref(numToken);
            CUDDNode stable = CUDD.Function.Equal(numToken, CUDD.Constant(1));

            probModelChecker.GetResult(probModelChecker.ComputeBoundedFuture(stable, 10), probModelChecker.start);
            probModelChecker.GetResult(probModelChecker.ComputeFuture(stable), probModelChecker.start);
            probModelChecker.GetResult(probModelChecker.ComputeReachReward(stable, 0), probModelChecker.start);

            probModelChecker.Close();
        }

        private static Modules Example5(int N, int K)
        {
            int range = 2*(K + 1)*N;
            int counterInit = (K + 1)*N;
            int left = N;
            int right = 2*(K + 1)*N - N;

            string counter = "counter";
            List<VarDeclaration> globalVars = new List<VarDeclaration>();
            globalVars.Add(new VarDeclaration(counter, 0, range, counterInit));

            List<Module> allModules = new List<Module>();
            string pc1 = "pc1";
            string coin1 = "coin1";

            string pc2 = "pc2";
            string coin2 = "coin2";

            List<Command> commands = new List<Command>();
            List<VarDeclaration> localVars = new List<VarDeclaration>();
            Module process1 = new Module("process1", commands, localVars);

            localVars.Add(new VarDeclaration(pc1, 0, 3, 0));
            localVars.Add(new VarDeclaration(coin1, 0, 1, 0));

            // flip coin
            List<Update> updates = new List<Update>();
            updates.Add(new Update(new List<Assignment>() { new Assignment(coin1, new IntConstant(0)), new Assignment(pc1, new IntConstant(1))}, 0.5));
            updates.Add(new Update(new List<Assignment>() { new Assignment(coin1, new IntConstant(1)), new Assignment(pc1, new IntConstant(1)) }, 0.5));
            commands.Add(new Command(Expression.EQ(new Variable(pc1), new IntConstant(0)), updates));

            // write tails -1  (reset coin to add regularity)
            updates = new List<Update>();
            updates.Add(
                new Update(
                    new List<Assignment>()
                        {
                            new Assignment(counter, Expression.MINUS(new Variable(counter), new IntConstant(1))),
                            new Assignment(pc1, new IntConstant(2)),
                            new Assignment(coin1, new IntConstant(0))
                        }, 1));
            commands.Add(
                new Command(
                    Expression.AND(Expression.EQ(new Variable(pc1), new IntConstant(1)), Expression.EQ(new Variable(coin1), new IntConstant(0)),
                                   Expression.GT(new Variable(counter), new IntConstant(0))), updates));

            // write heads +1 (reset coin to add regularity)
            updates = new List<Update>();
            updates.Add(
                new Update(
                    new List<Assignment>()
                        {
                            new Assignment(counter, Expression.PLUS(new Variable(counter), new IntConstant(1))),
                            new Assignment(pc1, new IntConstant(2)),
                            new Assignment(coin1, new IntConstant(0))
                        }, 1));
            commands.Add(
                new Command(
                    Expression.AND(Expression.EQ(new Variable(pc1), new IntConstant(1)), Expression.EQ(new Variable(coin1), new IntConstant(1)),
                                   Expression.LT(new Variable(counter), new IntConstant(range))), updates));

            // check
            // decide tails
            updates = new List<Update>();
            updates.Add(
                new Update(
                    new List<Assignment>()
                        {
                            new Assignment(pc1, new IntConstant(3)),
                            new Assignment(coin1, new IntConstant(0))
                        }, 1));
            commands.Add(
                new Command(
                    Expression.AND(Expression.EQ(new Variable(pc1), new IntConstant(2)),
                                   Expression.LE(new Variable(counter), new IntConstant(left))), updates));

            // decide heads
            updates = new List<Update>();
            updates.Add(
                new Update(
                    new List<Assignment>()
                        {
                            new Assignment(pc1, new IntConstant(3)),
                            new Assignment(coin1, new IntConstant(1))
                        }, 1));
            commands.Add(
                new Command(
                    Expression.AND(Expression.EQ(new Variable(pc1), new IntConstant(2)),
                                   Expression.GE(new Variable(counter), new IntConstant(right))), updates));

            // flip again
            updates = new List<Update>();
            updates.Add(
                new Update(
                    new List<Assignment>()
                        {
                            new Assignment(pc1, new IntConstant(0))
                        }, 1));
            commands.Add(
                new Command(
                    Expression.AND(Expression.EQ(new Variable(pc1), new IntConstant(2)), Expression.GT(new Variable(counter), new IntConstant(left)),
                                   Expression.LT(new Variable(counter), new IntConstant(right))), updates));

            // loop (all loop together when done)
            updates = new List<Update>();
            updates.Add(
                new Update(
                    new List<Assignment>()
                        {
                            new Assignment(pc1, new IntConstant(3))
                        }, 1));
            commands.Add(new Command("done", Expression.EQ(new Variable(pc1), new IntConstant(3)), updates));

            allModules.Add(process1);

            Module process2 = process1.Rename("process2", new Dictionary<string, string>() { { pc1, pc2 }, { coin1, coin2 } });
            allModules.Add(process2);

           
            SystemDef systemDef1 = new SingleModuleSystem("process1");
            SystemDef systemDef2 = new SingleModuleSystem("process2");
           

            SystemDef systemDef = new FullParallelSystem(new List<SystemDef>() { systemDef1, systemDef2 });

            List<RewardItem> rewardItems = new List<RewardItem>();
            rewardItems.Add(new RewardItem(null, new BoolConstant(true), new IntConstant(1)));
            RewardStruct rewardStruct = new RewardStruct("steps", rewardItems);

            Modules result = new Modules(ModelType.MDP, globalVars, allModules, new List<RewardStruct>() { rewardStruct }, systemDef, null);

            return result;
        }

        public static void Test5()
        {
            string pc1 = "pc1";
            string coin1 = "coin1";

            string pc2 = "pc2";
            string coin2 = "coin2";

            Modules modules = Example5(2, 5);

            ModulesToBDD modulesToBdd = new ModulesToBDD(modules);
            NonDetModel nonDetModel = (NonDetModel)modulesToBdd.Encode();

            NondetModelChecker nondetMC = new NondetModelChecker(nonDetModel);

            CUDDNode finished = nondetMC.EncodeExpression(Expression.AND(Expression.EQ(new Variable(pc1), new IntConstant(3)), Expression.EQ(new Variable(pc2), new IntConstant(3))));

            CUDDNode allCoinsEqual0 = nondetMC.EncodeExpression(Expression.AND(Expression.EQ(new Variable(coin1), new IntConstant(0)), Expression.EQ(new Variable(coin2), new IntConstant(0))));

            CUDDNode allCoinsEqual1 = nondetMC.EncodeExpression(Expression.AND(Expression.EQ(new Variable(coin1), new IntConstant(1)), Expression.EQ(new Variable(coin2), new IntConstant(1))));

            CUDDNode agree = nondetMC.EncodeExpression(Expression.AND(Expression.EQ(new Variable(coin1), new Variable(coin2))));

            nondetMC.GetResult(nondetMC.ComputeFuture(finished, true), nondetMC.start);

            CUDD.Ref(finished, allCoinsEqual0);
            nondetMC.GetResult(nondetMC.ComputeFuture(CUDD.Function.And(finished, allCoinsEqual0), true), nondetMC.start);

            CUDD.Ref(finished, allCoinsEqual1);
            nondetMC.GetResult(nondetMC.ComputeFuture(CUDD.Function.And(finished, allCoinsEqual1), true), nondetMC.start);

            CUDD.Ref(finished, agree);
            nondetMC.GetResult(nondetMC.ComputeFuture(CUDD.Function.And(finished, CUDD.Function.Not(agree)), false), nondetMC.start);

            nondetMC.GetResult(nondetMC.ComputeReachReward(finished, 0, true), nondetMC.start);

            nondetMC.GetResult(nondetMC.ComputeReachReward(finished, 0, false), nondetMC.start);

            nondetMC.Close();
        }

        
    }
}
