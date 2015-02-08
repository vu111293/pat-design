using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Antlr.Runtime;
using Microsoft.Msagl.Drawing;
using PAT.Common.Classes.BA;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.SemanticModels.LTS.Assertion;

namespace PAT.Common.Classes.ModuleInterface
{
    public abstract class SpecificationBase
    {
        /// <summary>
        /// if there is a c# code in the model, global environment needs to be cloned for if/case/guard.
        /// because the c# code can update the enviornment in the bool expression 
        /// </summary>
        public string SpecificationName;       

        public string FilePath;
        public HashSet<string> IncludeFiles;

        public string InputModelText;
        public static bool HasAtomicEvent;
        public static bool HasCSharpCode;

        public static List<string> SyncrhonousChannelNames;
        public static bool HasSyncrhonousChannel;

        public static bool IsParsing;
        public static bool CheckVariableDeclare = true;

        public Dictionary<string, AssertionBase> AssertionDatabase = new Dictionary<string, AssertionBase>(8);

        public abstract ConfigurationBase SimulationInitialization(string startingProcess);

        public virtual Bitmap MapConfigurationToImage(ConfigurationBase config, int imageSize)
        {
            return null;
        }

        public Graph GenerateBAGraph(string assert)
        {
            if (AssertionDatabase.ContainsKey(assert))
            {
                AssertionLTL ass = AssertionDatabase[assert] as AssertionLTL;
                if (ass != null)
                {
                    Graph g = LTL2BA.AutomatonToDot(ass.BA);
                    g.UserData = "!(" + ass.LTLString + ")";
                    return g;
                }
            }

            return null;
        }
        
        
        protected SpecificationBase(string spec, string filePath)
        {
            InputModelText = spec;

            //get the name of the specification
            if(spec.StartsWith(@"//@@"))
            {
                int index = spec.IndexOf("@@", 4);
                if(index != -1)
                {
                    SpecificationName = spec.Substring(4, index - 4);    
                }
            }
            
            FilePath = filePath;
            Errors.Clear();
            Warnings.Clear();

            IncludeFiles = new HashSet<string>();

            ParsingException.FileOffset.Clear();
            ParsingException.FileOffset.Add(ParsingException.CountLinesInFile(spec), new string[] {FilePath});
        }



        /// <summary>
        /// Generate the compete spec from the include files
        /// </summary>
        /// <returns></returns>
        protected string GetCompleteSpecFromIncludeFiles()
        {
            //add the original model
            string completeSpec = InputModelText;

            ParsingException.FileOffset.Clear();
            ParsingException.FileOffset.Add(ParsingException.CountLinesInFile(completeSpec), new string[] {FilePath});

            //add the included models one by one
            foreach (string file in IncludeFiles)
            {
                try
                {
                    //cannot add the same file twice.
                    if (file != FilePath)
                    {
                        completeSpec += "\r\n" + System.IO.File.ReadAllText(file, Encoding.UTF8);
                        ParsingException.FileOffset.Add(ParsingException.CountLinesInFile(completeSpec), new string[] {file});    
                    }                    
                }
                catch (Exception exception)
                {
                    throw new ParsingException("Error happend in loading \"" + file + "\":" + exception.Message, -1, -1, "");
                }
            }

            //clear the included files after the inclusion
            IncludeFiles.Clear();
            return completeSpec;
        }

        public Dictionary<string, Expression> DeclarationDatabase = new Dictionary<string, Expression>(8);
        public Dictionary<string, KeyValuePair<List<string>, Expression>> MacroDefinition = new Dictionary<string, KeyValuePair<List<string>, Expression>>();
        public Dictionary<string, Expression> GlobalConstantDatabase;

   
        public abstract string GetSpecification();
        public abstract List<string> GetProcessNames();
        public abstract void LockSpecificationData();
        public virtual void UnLockSpecificationData()
        {
            
        }

        public SortedDictionary<string, Declaration> DeclaritionTable = new SortedDictionary<string, Declaration>();
        
        public virtual ParsingException GoToDeclarition(string term)
        {
            if(DeclaritionTable.ContainsKey(term))
            {
                return DeclaritionTable[term].DeclarationToken;
            }

            return null;
        }

        public Dictionary<string, List<ParsingException>> UsageTable = new Dictionary<string, List<ParsingException>>(16);
        public virtual List<ParsingException> FindUsages(string term)
        {
            if (UsageTable.ContainsKey(term))
            {
                UsageTable[term].Sort(new ParsingExceptionComparer());
                return UsageTable[term];
            }

            return new List<ParsingException>(0);
        }

        public virtual List<ParsingException> RenameFindUsages(string term)
        {
            if (UsageTable.ContainsKey(term))
            {
                List<ParsingException> returnList = new List<ParsingException>(UsageTable[term]);
                    
                if (DeclaritionTable.ContainsKey(term))
                {
                    returnList.Add(DeclaritionTable[term].DeclarationToken);                    
                }

                returnList.Sort(new ParsingExceptionComparer());     
                return returnList;
            }
            return new List<ParsingException>(0);
        }


        #region Methods For Auto Completion

        public abstract List<string> GetAllProcessNames();
        public abstract List<string> GetGlobalVarNames();
        public virtual List<string> GetChannelNames() { return new List<string>(); }
        public abstract string[] GetParameterNames(string process);

        #endregion

        public static bool IsSimulation = false;
        public void LockSharedData(bool isSimulation)
        {
            IsSimulation = isSimulation;
            Ultility.Ultility.LockSharedData(this);
        }

        public void UnLockSharedData()
        {
            Ultility.Ultility.UnLockSharedData(this);
        }
        public bool GrabSharedDataLock()
        {
            return Ultility.Ultility.GrabSharedDataLock();
        }

        #region "Warning and Error"

        public Dictionary<string, ParsingException> Warnings = new Dictionary<string, ParsingException>();
        public Dictionary<string, ParsingException> Errors = new Dictionary<string, ParsingException>();

        public void AddNewWarning(string msg, IToken token)
        {
            string key = msg + token.Line + token.CharPositionInLine;
            if (!Warnings.ContainsKey(key))
            {
                Warnings.Add(key, new ParsingException(msg, token));
            }
        }

        public void AddNewError(string msg, IToken token)
        {
            string key = msg + token.Line + token.CharPositionInLine;
            if (!Errors.ContainsKey(key))
            {
                Errors.Add(key, new ParsingException(msg, token));
            }
        }

        #endregion
    }
}