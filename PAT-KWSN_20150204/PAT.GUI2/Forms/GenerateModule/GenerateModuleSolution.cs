/**
 * To Generate the specified module solution
 * @author Ma Junwei, 30/12/2010 
 * */
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;
using Microsoft.VisualStudio.TextTemplating;

namespace PAT.GUI.Forms.GenerateModule
{
    class GenerateModuleSolution
    {
        #region Fields
        //------------------------------------------
        // the solution direction
        private  string solutionHomeDirection;
        private  string solutionPropertiesDirection;
        private  string solutionAssertionDirection;
        private  string solutionUltilityDirection;
        private  string solutionLtsDirection;
        private  string solutionLtsSampleDirection;
        private  string solutionClasses;

        private string solutionMainDirection;
        private string solutionMainPropertiesDirection;

        private string solutionRootDirection;

        //------------------------------------------
        // the solution params
        private  string sourceModuleIconPath;
        private  string destModuleIconPath;
        private  string solutionModuleName;
        private  string solutionModuleCode;
        private  string ModuleCode;
        private  string assertionsNamespace;
        private string semanticModel;
        private  string ltsNamespace;
        private  string ultilityNamespace;
        
        private  List<string> customSyntax;

        private  bool isBdd;
        private  bool assertionDeadlock;
        private  bool assertionLTL;
        private  bool assertionReachability;
        private  bool assertionRefinement;
        private  bool assertionDeterminism;
        private  bool assertionDivergence;
        //------------------------------------------
        // where are the templates
        private  string templateHomeDirection;
        private  string templatePropertiesDirection;
        private  string templateAssertionsDirection;
        private  string templateLTSDirection;
        private  string templateUltilityDirection;

        private string templateMainDirection;
        private string templateRootDirection;
        private string templateMainPropertiesDirection;
        #endregion

        /// <summary>
        /// Generate CSharp solution here
        /// </summary>
        /// <param name="option"></param>
        public void GenerateSolution(GenerateOption option)
        {
            // generate the solution tree
            solutionHomeDirection = Path.Combine(option.OutputFolder, "PAT.Module." + option.ModuleCode);
            solutionPropertiesDirection = Path.Combine(solutionHomeDirection,"Properties");
            solutionAssertionDirection = Path.Combine(solutionHomeDirection, "Assertions");
            solutionLtsDirection = Path.Combine(solutionHomeDirection, "LTS");
            solutionLtsSampleDirection = Path.Combine(solutionLtsDirection, "Sample");
            solutionUltilityDirection = Path.Combine(solutionHomeDirection, "Ultility");

            solutionMainDirection = Path.Combine(option.OutputFolder, "PAT.Main");
            solutionMainPropertiesDirection = Path.Combine(solutionMainDirection, "Properties");

            solutionRootDirection = option.OutputFolder;

            //--------------------------------------------------------------
            // resolve the parameters here, this params will be used by ptt
            // file to generate the solution
            solutionModuleCode = "PAT." + option.ModuleCode;
            ModuleCode = option.ModuleCode;
            solutionModuleName = option.ModuleName;
            assertionsNamespace = solutionModuleCode + ".Assertions";
            ltsNamespace = solutionModuleCode + ".LTS";
            ultilityNamespace = solutionModuleCode + ".Ultility";

            // resolve the options the user selected

            // the custom syntax
            customSyntax = option.CustomSyntax;

            semanticModel = option.Semantics;

            // whether generate BDD encoding methods
            isBdd = option.IsBdd;

            // the assertion option selected
            // ------------------------------------------------------------------------
            assertionDeadlock = option.Assertion.AssertionDeadlock;
            assertionLTL = option.Assertion.AssertionLTL;
            assertionRefinement = option.Assertion.AssertionRefinement;
            assertionReachability = option.Assertion.AssertionReachability;
            assertionDeterminism = option.Assertion.AssertionDeterminism;
            assertionDivergence = option.Assertion.AssertionDivergence;

            // the picture location
            // -----------------------------------------------------------------------
            if (option.ModuleIconLocation != null && !option.ModuleIconLocation.Equals(""))
            {
                sourceModuleIconPath = option.ModuleIconLocation;
                // ReSharper disable AssignNullToNotNullAttribute
                destModuleIconPath = Path.Combine(solutionHomeDirection, Path.GetFileName(option.ModuleIconLocation));
                // ReSharper restore AssignNullToNotNullAttribute
            }

            //--------------------------------------------------------------
            // please assign the template path in the next code line
            templateHomeDirection = Path.Combine(Application.StartupPath, @"Docs\Template\Module");

            templateAssertionsDirection = Path.Combine(templateHomeDirection, "Assertions");
            templateLTSDirection = Path.Combine(templateHomeDirection, "LTS");

            templateUltilityDirection = Path.Combine(templateHomeDirection, "Ultility");
            templatePropertiesDirection = Path.Combine(templateHomeDirection, "Properties");

            templateMainDirection = Path.Combine(Application.StartupPath, @"Docs\Template\Main");
            templateMainPropertiesDirection = Path.Combine(templateMainDirection, "Properties");
            
            templateRootDirection = Path.Combine(Application.StartupPath, @"Docs\Template");
            
            // determine which fold to use
            switch (semanticModel)
            {
                case "Labeled Transition System (LTS)":
                    templateAssertionsDirection += "LTS";
                    templateLTSDirection += "LTS";
                    break;

                case "Timed Transition System (TTS)":
                    // use the default templates now!
                    templateAssertionsDirection += "LTS"; // TODO use the default now
                    templateLTSDirection += "LTS";
                    break;

                case "Markov Decision Processes (MDP)":
                    templateAssertionsDirection += "MDP";
                    templateLTSDirection += "MDP";
                    // can also generate special classes here
                    break;
            }

            //--------------------------------------------------------------
            // Create the folder first
            GenerateFolders();
            // After create the folder, we start to create the files
            GenerateFiles();

        }

        /// <summary>
        /// Generate the Folders First
        /// </summary>
        private void GenerateFolders()
        {
            // Create the Base Folder
            Directory.CreateDirectory(solutionHomeDirection);
            // Create the Sub Folder
            Directory.CreateDirectory(solutionPropertiesDirection);
            Directory.CreateDirectory(solutionAssertionDirection);
            Directory.CreateDirectory(solutionLtsDirection);
            Directory.CreateDirectory(solutionLtsSampleDirection);
            Directory.CreateDirectory(solutionUltilityDirection);

            // Create the Base Folder
            Directory.CreateDirectory(solutionMainDirection);
            Directory.CreateDirectory(solutionMainPropertiesDirection);
        }

        /// <summary>
        /// Generate the Files in the solution
        /// </summary>
        private void GenerateFiles()
        {
            // if specifed, then copy the icon to the solution path
            if( null != sourceModuleIconPath  && !sourceModuleIconPath.Equals(""))
            {
                if(File.Exists(destModuleIconPath))
                {
                    File.Delete(destModuleIconPath);
                }
                File.Copy(sourceModuleIconPath, destModuleIconPath);
            }
         
            //-------------------------------------------------------------------------------------------------------
            // empty the class that has generated
            solutionClasses = string.Empty;
            //-------------------------------------------------------------------------------------------------------
            // generate the custom syntax first
            var customThread = new Thread(GenerateCustomThread);
            //-------------------------------------------------------------------------------------------------------
            // generate the base classes and contents
            var baseThread = new Thread(GenerateBaseThread);       
            //------------------------------------------------------------------------------------------------------
            // generate the classes in the assertion folder
            var assertionThread = new Thread(GenerateAssertionThread);     
            //------------------------------------------------------------------------------------------------------
            // generate the classes in the lts folder
            var ltsThread = new Thread(GenerateLTSThread);           
            //------------------------------------------------------------------------------------------------------
            // generate the classes in the ultility folder
            var ultilityThread = new Thread(GenerateUltilityThread);

            var mainThread = new Thread(GenerateMainThread);  
            
            // start all the threads here
            //------------------------------------------------------------------------------------------------------
            customThread.Start();
            baseThread.Start();
            assertionThread.Start();
            ltsThread.Start();
            ultilityThread.Start();
            mainThread.Start();
            //------------------------------------------------------------------------------------------------------
            // generate the csharp solution file at last
            // this must be excuted after all the threads end.
            var csprojThread = new Thread(GenerateCSPROJThread);
            customThread.Join();
            baseThread.Join();
            assertionThread.Join();
            ltsThread.Join();
            ultilityThread.Join();
            mainThread.Join();
            csprojThread.Start();
        }
        #region Generate File From Text Template

        private const int RetryBound = 3;

        /// <summary>
        /// Generate a default extension(".cs") file without renaming
        /// </summary>
        /// <param name="host"></param>
        /// <param name="engine"></param>
        /// <param name="fileName"></param>
        /// <param name="aliasName"></param>
        /// <param name="extension"></param>
        private void GenerateFileAsDefault(CustomerHost host, Engine engine, string fileName, string aliasName, string extension, int retryCount)
        {
            // if the extension is not null, then specify the output file extension
            // else use the default value(.cs) as the output file extension
            host.TemplateFileValue = fileName;
            if(extension != null)
            {
                host.SetFileExtension(extension);
            }


            string inputFileStreamName = fileName.Replace(templateRootDirection, "").TrimStart('\\').Replace('\\', '.');


            // if the aliasName is not null, then specify the output file as the alias name
            // else use the input file name(exclude the extension) as the fileName
            string outputFileName = aliasName ?? Path.GetFileNameWithoutExtension(fileName);

            // how to determine the output file's direction?
            // if the text template exists in the Assertion folder,then put the
            // output file to the Assertion folder, and if it exists in LTS folder,
            // put the output file to LTS folder, and so on
            // if the input file direction is null, then we define it ""
            string inputFileDirection = Path.GetDirectoryName(fileName)??string.Empty;

            string outputFileDirection;
            if (inputFileDirection.EndsWith("Main"))
            {
                outputFileDirection = solutionMainDirection;
            } 
            else if (inputFileDirection.Contains("Main\\Properties"))
            {
                outputFileDirection = solutionMainPropertiesDirection;
            }
            else if (inputFileDirection.Contains("Properties"))
            {
                outputFileDirection = solutionPropertiesDirection;
            }
            else if (fileName.Contains("Assertions"))
            {
                // the assertions direction
                outputFileDirection = solutionAssertionDirection;
            }
            else if (fileName.Contains("Sample"))
            {
                // the LTS direction
                outputFileDirection = solutionLtsSampleDirection;
            }
            else if (fileName.Contains("LTS"))
            {
                // the LTS direction
                outputFileDirection = solutionLtsDirection;
            }
            else if (fileName.Contains("Ultility"))
            {
                // the ultility direction
                outputFileDirection = solutionUltilityDirection;
            }
            else if (fileName.EndsWith("PAT3 Source.ptt"))
            {
                // the ultility direction
                outputFileDirection = solutionRootDirection;
            }
            else
            {
                // if none of above, then put the output file to the home direction
                outputFileDirection = solutionHomeDirection;
            }

            outputFileName = Path.Combine(outputFileDirection, outputFileName) + host.FileExtension;

            Assembly myAssembly = Assembly.GetExecutingAssembly();
            Stream myStream = myAssembly.GetManifestResourceStream("PAT.GUI.Docs.Template." + inputFileStreamName);
            
            if (extension == "resources")
            {
                byte[] buf = new byte[myStream.Length];  //declare arraysize    
                myStream.Read(buf, 0, buf.Length); // read from stream to byte array

                File.WriteAllBytes(outputFileName, buf);                
            }
            else
            {
                StreamReader reader = new StreamReader(myStream);
                string text = reader.ReadToEnd(); 

                // use the engine to transfor the template
                string output = engine.ProcessTemplate(text, host);

                // write the generated file into the output file specified before
                // use the encoding style set in host
                File.WriteAllText(outputFileName, output, host.FileEncoding);
    
            }
            
            // after the generate, add the classes in the local classes field which will be used to
            // generate the .csproj file
            if (!inputFileDirection.Contains("\\Main") && !fileName.EndsWith("PAT3 Source.ptt") && retryCount == 1)
            {
                solutionClasses += outputFileName.Replace(solutionHomeDirection, "").TrimStart('\\') + "|";
            }

            // reset the host extension property
            host.ResetFileExtension();

            if (host.Errors.Count > 0)
            {
                if (retryCount <= RetryBound)
                {
                    GenerateFileAsDefault(host, engine, fileName, aliasName, extension, retryCount + 1);
                }
                else
                {
                    foreach (var error in host.Errors)
                    {
                        MessageBox.Show(
                            string.Format("File[{0}] is Generated unsuccessfully for: {1}", outputFileName, error),
                            Common.Ultility.Ultility.APPLICATION_NAME, MessageBoxButtons.OK);
                    }
                }
            }
        }
        #endregion

        #region multithreads to generate the file
        private void GenerateCustomThread()
        {
            var host = new CustomerHost();
            var engine = new Engine();
            host.Params.ModuleName = solutionModuleName;
            host.Params.Namespace = solutionModuleCode;
            host.Params.AssertionsNamespace = assertionsNamespace;
            host.Params.LtsNamespace = ltsNamespace;
            host.Params.UltilityNamespace = ultilityNamespace;
            host.Params.ModuleCode = ModuleCode;

            // specify the parameter values in the template
            // this can use session if .Net 4.0 is supported!
            host.Params.Namespace = solutionModuleCode;
            if (customSyntax != null && customSyntax.Count != 0)
            {
                foreach (var syntax in customSyntax)
                {
                    host.Params.ClassName = syntax;
                    GenerateFileAsDefault(host, engine, Path.Combine(templateLTSDirection, "CustomSyntax.ptt"), syntax, null, 1);
                }
            }
        }

        private void GenerateBaseThread()
        {
            var host = new CustomerHost();
            var engine = new Engine();
            // specify the parameter values in the template
            // this can use session if .Net 4.0 is supported!
            host.Params.ModuleName = solutionModuleName;
            host.Params.Namespace = solutionModuleCode;
            host.Params.AssertionsNamespace = assertionsNamespace;
            host.Params.LtsNamespace = ltsNamespace;
            host.Params.UltilityNamespace = ultilityNamespace;
            host.Params.ModuleCode = ModuleCode;
            host.Params.EmbeddedResource = Path.GetFileName(destModuleIconPath);

            GenerateFileAsDefault(host, engine, Path.Combine(templateHomeDirection, "Syntax.ptt"), null, ".xshd", 1);
            GenerateFileAsDefault(host, engine, Path.Combine(templatePropertiesDirection, "AssemblyInfo.ptt"), null, null, 1);
            GenerateFileAsDefault(host, engine, Path.Combine(templateHomeDirection, "ModuleFacade.ptt"), null, null, 1);
        }

        private void GenerateAssertionThread()
        {
            //-------------------------------------------------------------------------------------------------------
            // set the host and engine properties
            var host = new CustomerHost();
            var engine = new Engine();
            // specify the parameter values in the template
            // this can use session if .Net 4.0 is supported!
            host.Params.ModuleName = solutionModuleName;
            host.Params.ModuleName = solutionModuleName;
            host.Params.Namespace = solutionModuleCode;
            host.Params.AssertionsNamespace = assertionsNamespace;
            host.Params.LtsNamespace = ltsNamespace;
            host.Params.UltilityNamespace = ultilityNamespace;
            host.Params.ModuleCode = ModuleCode;

            GenerateFileAsDefault(host, engine, Path.Combine(templateAssertionsDirection, "DataStore.ptt"), null, null, 1);

            GenerateFileAsDefault(host, engine, Path.Combine(templateAssertionsDirection, "Assertion.ptt"), null, null, 1);

            if(assertionDeadlock)
            {
                GenerateFileAsDefault(host, engine, Path.Combine(templateAssertionsDirection, "AssertionDeadLock.ptt"), ModuleCode + "AssertionDeadLock", null, 1);
            }

            if(assertionDeterminism)
            {
                GenerateFileAsDefault(host, engine, Path.Combine(templateAssertionsDirection, "AssertionDeterminism.ptt"), ModuleCode + "AssertionDeterminism", null, 1);
            }

            if(assertionDivergence)
            {
                GenerateFileAsDefault(host, engine, Path.Combine(templateAssertionsDirection, "AssertionDivergence.ptt"), ModuleCode + "AssertionDivergence", null, 1);
            }

            if(assertionLTL)
            {
                GenerateFileAsDefault(host, engine, Path.Combine(templateAssertionsDirection, "AssertionLTL.ptt"), ModuleCode + "AssertionLTL", null, 1);
            }
            
            if (assertionReachability)
            {
                GenerateFileAsDefault(host, engine, Path.Combine(templateAssertionsDirection, "AssertionReachability.ptt"), ModuleCode + "AssertionReachability", null, 1);
                GenerateFileAsDefault(host, engine, Path.Combine(templateAssertionsDirection, "AssertionReachabilityWith.ptt"), ModuleCode + "AssertionReachabilityWith", null, 1);  
            }

            if(assertionRefinement)
            {
                GenerateFileAsDefault(host, engine, Path.Combine(templateAssertionsDirection, "AssertionRefinement.ptt"), ModuleCode + "AssertionRefinement", null, 1);
                GenerateFileAsDefault(host, engine, Path.Combine(templateAssertionsDirection, "AssertionRefinementF.ptt"), ModuleCode + "AssertionRefinementF", null, 1);
                GenerateFileAsDefault(host, engine, Path.Combine(templateAssertionsDirection, "AssertionRefinementFD.ptt"), ModuleCode + "AssertionRefinementFD", null, 1);            
            }

        }

        private void GenerateLTSThread()
        {
            //-------------------------------------------------------------------------------------------------------
            // set the host and engine properties
            var host = new CustomerHost();
            var engine = new Engine();
            // specify the parameter values in the template
            // this can use session if .Net 4.0 is supported!
            host.Params.ModuleName = solutionModuleName;
            host.Params.Namespace = solutionModuleCode;
            host.Params.AssertionsNamespace = assertionsNamespace;
            host.Params.LtsNamespace = ltsNamespace;
            host.Params.UltilityNamespace = ultilityNamespace;
            host.Params.ModuleCode = ModuleCode;
 
//            var directory = new DirectoryInfo(templateLTSDirection);
//            var files = new List<FileInfo>();
//            files.AddRange(directory.GetFiles());
//
//            foreach (var file in files)
//            {
//                GenerateFileAsDefault(host,engine,file.Name,null,null);
//            }
            if(templateLTSDirection.Contains("MDP"))
            {
                GenerateFileAsDefault(host, engine, Path.Combine(templateLTSDirection, "PCaseProcess.ptt"), null, null, 1);
            }

            string templateLTSDirectionSample = Path.Combine(templateLTSDirection, "Sample");

            GenerateFileAsDefault(host, engine, Path.Combine(templateLTSDirectionSample, "Assertion.ptt"), null, null, 1);
            GenerateFileAsDefault(host, engine, Path.Combine(templateLTSDirectionSample, "AtomicProcess.ptt"), null, null, 1);
            GenerateFileAsDefault(host, engine, Path.Combine(templateLTSDirectionSample, "CaseProcess.ptt"), null, null, 1);
            GenerateFileAsDefault(host, engine, Path.Combine(templateLTSDirectionSample, "ChannelInput.ptt"), null, null, 1);
            GenerateFileAsDefault(host, engine, Path.Combine(templateLTSDirectionSample, "ChannelInputGuarded.ptt"), null, null, 1);
            GenerateFileAsDefault(host, engine, Path.Combine(templateLTSDirectionSample, "ChannelOutput.ptt"), null, null, 1);
            GenerateFileAsDefault(host, engine, Path.Combine(templateLTSDirectionSample, "ConditionalChoice.ptt"), null, null, 1);
            GenerateFileAsDefault(host, engine, Path.Combine(templateLTSDirectionSample, "ConditionalChoiceAtomic.ptt"), null, null, 1);
            GenerateFileAsDefault(host, engine, Path.Combine(templateLTSDirectionSample, "ConditionalChoiceBlocking.ptt"), null, null, 1);
            GenerateFileAsDefault(host, engine, Path.Combine(templateLTSDirectionSample, "DataOperationPrefix.ptt"), null, null, 1);
            GenerateFileAsDefault(host, engine, Path.Combine(templateLTSDirectionSample, "EventPrefix.ptt"), null, null, 1);
            GenerateFileAsDefault(host, engine, Path.Combine(templateLTSDirectionSample, "GuardProcess.ptt"), null, null, 1);
            GenerateFileAsDefault(host, engine, Path.Combine(templateLTSDirectionSample, "Hiding.ptt"), null, null, 1);
            GenerateFileAsDefault(host, engine, Path.Combine(templateLTSDirectionSample, "IndexChoice.ptt"), null, null, 1);
            GenerateFileAsDefault(host, engine, Path.Combine(templateLTSDirectionSample, "IndexedProcess.ptt"), null, null, 1);
            GenerateFileAsDefault(host, engine, Path.Combine(templateLTSDirectionSample, "IndexExternalChoice.ptt"), null, null, 1);
            GenerateFileAsDefault(host, engine, Path.Combine(templateLTSDirectionSample, "IndexInterleave.ptt"), null, null, 1);
            GenerateFileAsDefault(host, engine, Path.Combine(templateLTSDirectionSample, "IndexInterleaveAbstract.ptt"), null, null, 1);
            GenerateFileAsDefault(host, engine, Path.Combine(templateLTSDirectionSample, "IndexInternalChoice.ptt"), null, null, 1);
            GenerateFileAsDefault(host, engine, Path.Combine(templateLTSDirectionSample, "IndexParallel.ptt"), null, null, 1);
            GenerateFileAsDefault(host, engine, Path.Combine(templateLTSDirectionSample, "Interrupt.ptt"), null, null, 1);
            GenerateFileAsDefault(host, engine, Path.Combine(templateLTSDirectionSample, "Sequence.ptt"), null, null, 1);
            GenerateFileAsDefault(host, engine, Path.Combine(templateLTSDirectionSample, "Skip.ptt"), null, null, 1);
            GenerateFileAsDefault(host, engine, Path.Combine(templateLTSDirectionSample, "Stop.ptt"), null, null, 1);


            GenerateFileAsDefault(host, engine, Path.Combine(templateLTSDirection, "Process.ptt"), null, null, 1);
            GenerateFileAsDefault(host, engine, Path.Combine(templateLTSDirection, "Definition.ptt"), null, null, 1);
            GenerateFileAsDefault(host, engine, Path.Combine(templateLTSDirection, "DefinitionRef.ptt"), null, null, 1);
            GenerateFileAsDefault(host, engine, Path.Combine(templateLTSDirection, "Configuration.ptt"), null, null, 1);
            GenerateFileAsDefault(host, engine, Path.Combine(templateLTSDirection, "ConfigurationWithChannelData.ptt"), null, null, 1);
            GenerateFileAsDefault(host, engine, Path.Combine(templateLTSDirection, "Specification.ptt"), null, null, 1);
        }

        private void GenerateUltilityThread()
        {
            //-------------------------------------------------------------------------------------------------------
            // set the host and engine properties
            var host = new CustomerHost();
            var engine = new Engine();
            // specify the parameter values in the template
            // this can use session if .Net 4.0 is supported!
            host.Params.ModuleName = solutionModuleName;
            host.Params.Namespace = solutionModuleCode;
            host.Params.AssertionsNamespace = assertionsNamespace;
            host.Params.LtsNamespace = ltsNamespace;
            host.Params.UltilityNamespace = ultilityNamespace;
            host.Params.ModuleCode = ModuleCode;

            GenerateFileAsDefault(host, engine, Path.Combine(templateUltilityDirection, "Ultility.ptt"), null, null, 1);
        }

        private void GenerateCSPROJThread()
        {
            var host = new CustomerHost();
            var engine = new Engine();
            // specify the parameter values in the template
            // this can use session if .Net 4.0 is supported!
            host.Params.ModuleName = solutionModuleName;
            host.Params.Namespace = solutionModuleCode;
            host.Params.DllDirection = Application.StartupPath;
            host.Params.Classes = solutionClasses;
            host.Params.ModuleCode = ModuleCode;
            host.Params.EmbeddedResource = Path.GetFileName(destModuleIconPath);

            GenerateFileAsDefault(host, engine, Path.Combine(templateHomeDirection, "Project.ptt"), "PAT.Module." + ModuleCode, ".csproj", 1);
        }


        private void GenerateMainThread()
        {
            //-------------------------------------------------------------------------------------------------------
            // set the host and engine properties
            var host = new CustomerHost();
            var engine = new Engine();
            // specify the parameter values in the template
            // this can use session if .Net 4.0 is supported!
            host.Params.ModuleName = solutionModuleName;
            host.Params.Namespace = solutionModuleCode;
            host.Params.ModuleCode = ModuleCode;
            host.Params.DllDirection = Application.StartupPath;

            GenerateFileAsDefault(host, engine, Path.Combine(templateMainDirection, "PAT.Main.ptt"), null, "csproj", 1);
            GenerateFileAsDefault(host, engine, Path.Combine(templateMainDirection, "OutputDockingWindow.ptt"), null, null, 1);
            GenerateFileAsDefault(host, engine, Path.Combine(templateMainDirection, "FormMain.ptt"), null, null, 1);
            GenerateFileAsDefault(host, engine, Path.Combine(templateMainDirection, "FormMain.resx.ptt"), "FormMain", "resx", 1);
            GenerateFileAsDefault(host, engine, Path.Combine(templateMainDirection, "FormMain.Designer.ptt"), null, null, 1);
            GenerateFileAsDefault(host, engine, Path.Combine(templateMainDirection, "ErrorListWindow.ptt"), null, null, 1);
            GenerateFileAsDefault(host, engine, Path.Combine(templateMainDirection, "ErrorListWindow.resx.ptt"), "ErrorListWindow", "resx", 1);
            GenerateFileAsDefault(host, engine, Path.Combine(templateMainDirection, "EditorTabItem.ptt"), null, null, 1);
            GenerateFileAsDefault(host, engine, Path.Combine(templateMainDirection, "Program.ptt"), null, null, 1);
            
            GenerateFileAsDefault(host, engine, Path.Combine(templateMainPropertiesDirection, "BitmapResources.ptt"), null, "resources", 1);
            GenerateFileAsDefault(host, engine, Path.Combine(templateMainPropertiesDirection, "StringResources.ptt"), null, "resources", 1);
            GenerateFileAsDefault(host, engine, Path.Combine(templateMainPropertiesDirection, "Resources.Designer.ptt"), null, null, 1);
            GenerateFileAsDefault(host, engine, Path.Combine(templateMainPropertiesDirection, "Resources.ptt"), null, "resx", 1);

            GenerateFileAsDefault(host, engine, Path.Combine(templateRootDirection, "PAT3 Source.ptt"), null, "sln", 1);
       
        }
        #endregion
    }
}

