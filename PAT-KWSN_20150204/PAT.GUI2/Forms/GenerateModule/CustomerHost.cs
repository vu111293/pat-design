/**
 * Customer Tool Generator Host
 * to generate csharp class file(.cs file)
 * from template file(.tt file)
 * @author Ma Junwei, 3/1/2011
 */
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TextTemplating;

namespace PAT.GUI.Forms.GenerateModule
{
    [Serializable]
    class CustomerHost : ITextTemplatingEngineHost
    {
        #region Parameters that need to be resolved
        /// <summary>
        /// define the parameters that might be resolved in the text template
        /// there is a better way to do this in .NET 4.0, using Session or CallContext
        /// but that is not supported by vs 2008 and .NET 3.5
        /// </summary>
        [Serializable]
        internal class Parameter
        {
            /// <summary>
            /// Properties of the parameter
            /// </summary>
            public string ModuleCode { get; set; }

            public string ModuleName { get; set; }

            public string Namespace { get; set; }

            public string AssertionsNamespace { get; set; }

            public string LtsNamespace { get; set; }

            public string UltilityNamespace { get; set; }

            public string DllDirection { get; set; }

            public string EmbeddedResource { get; set; }

            public string Classes { get; set; }

            public string ClassName { get; set; }
        }
        #endregion

        #region ITextTemplatingEngineHost Members

        // this should be initualized here
        public Parameter Params = new Parameter();

        /// <summary>
        /// The path and file name of the text template that is being processed
        /// </summary>
        internal string TemplateFileValue;
        public string TemplateFile
        {
            get { return TemplateFileValue; }
        }

        /// <summary>
        /// The errors occur when the engine processes a template
        /// the engine passes the errors to the host when it is processing
        /// and the host can decide how to display them
        /// </summary>
        private CompilerErrorCollection _errorsValue;
        public CompilerErrorCollection Errors
        {
            get { return _errorsValue; }
        }
        /// <summary>
        /// This is the extension of the generated output file
        /// The current class provide ".cs" as the default value.
        /// And this can be modified by the Set method
        /// </summary>
        private string _fileExtensionValue = ".cs";
        public string FileExtension
        {
            get { return _fileExtensionValue; }
        }

        /// <summary>
        /// This is the encoding of the generated output file
        /// The current class provide "UTF-8" as the default value.
        /// And this can be modified by the Set Method
        /// </summary>
        private Encoding _fileEncodingValue = Encoding.UTF8;
        public Encoding FileEncoding
        {
            get { return _fileEncodingValue; }
        }

        /// <summary>
        /// The engine calls this method based on the optional include directive
        /// if the user has specified it in the text template.
        /// This method can be called 0,1,or more times
        /// ------------------------------------------------------------------------------------------------
        /// The included text is returned in the context parameter.
        /// If the host searches the registry for the location of include files,
        /// or if the host searches multiple locations by default, the host can
        /// return the final path of the include file in the location parameter.
        /// </summary>
        /// <param name="requestFileName"></param>
        /// <param name="content"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public bool LoadIncludeText(string requestFileName, out string content, out string location)
        {
            content = String.Empty;
            location = String.Empty;

            // if the argument is the fully qualified path of an existing file,
            // then we are done.
            if (File.Exists(requestFileName))
            {
                content = File.ReadAllText(requestFileName);
                return true;
            }
            // if the argument is not a fully qualified path
            // then we find the template in the default direction
            if (requestFileName != null)
            {
                content = File.ReadAllText(Path.Combine(@"Template",requestFileName));
            }
            // if the file does not exist in the path, we failed
            // and this can be customized if neccesary.
            return false; 
        }

        /// <summary>
        /// The engine calls this method to resolve assembly references used in
        /// the generated transformation class project and for the optional assembly
        /// directive if the user has specified it in the text template.
        /// This method can be called 0,1, or more times
        /// </summary>
        /// <param name="assemblyReference"></param>
        /// <returns></returns>
        public string ResolveAssemblyReference(string assemblyReference)
        {
            // if the argument is the fully qualified path of an existing file,
            // then we are done.
            if (File.Exists(assemblyReference))
            {
                return assemblyReference;
            }
            // Maybe the assembly is in the same folder as the text template that
            // called the directive.
            // the TemplateFile can not be null
            // ReSharper disable AssignNullToNotNullAttribute
            string candidate = Path.Combine(Path.GetDirectoryName(TemplateFile), assemblyReference);
            // ReSharper restore AssignNullToNotNullAttribute
            if (File.Exists(candidate))
            {
                return candidate;
            }
            // if come here, we can use another customized method to resolve it
            return String.Empty;
        }

        /// <summary>
        /// The engine calls this method based on the directives the user has
        /// specified in the text template
        /// This method can be called 0,1, or more times
        /// </summary>
        /// <param name="processorName"></param>
        /// <returns></returns>
        public Type ResolveDirectiveProcessor(string processorName)
        {
            // we do nothing here
            // if the customer tool wants to process any directive,
            // you can add some code here
            if (string.Compare(processorName,"XYZ",StringComparison.OrdinalIgnoreCase) == 0)
            {
                
            }
            throw new Exception("Directive processor Not Found");
        }

        /// <summary>
        /// A directive processor can call this method if a file name does not
        /// have a path.
        /// The host can attempt to provide path information by searching
        /// specific paths for the file and returning the file and path if found.
        /// This method can be called 0,1 or more times
        /// </summary>
        /// <param name="path"></param>
        /// <exception cref="ArgumentNullException">file name cannot be null</exception>
        /// <returns></returns>
        public string ResolvePath(string path)
        {
            if(path == null)
            {
                throw new ArgumentNullException("Path cannot" + " be null");
            }
            if(File.Exists(path))
            {
                return path;
            }
            // The template cannot be null here
            // Maybe the file is in the same folder as the text template that called the directive.
            // ReSharper disable AssignNullToNotNullAttribute
            string candidate = Path.Combine(Path.GetDirectoryName(TemplateFile), path);
            // ReSharper restore AssignNullToNotNullAttribute
            if(File.Exists(candidate))
            {
                return candidate;
            }
            // You can look for more places if neccesary
            return path;
        }

        /// <summary>
        /// If a call to a directive in a text template does not provide a value
        /// for a required parameter, the directive processor can try to get it
        /// from the host by calling this method.
        /// This method can be called 0, 1, or more times.
        /// </summary>
        /// <param name="directiveId"></param>
        /// <param name="processorName"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public string ResolveParameterValue(string directiveId, string processorName, string parameterName)
        {
            if(directiveId == null)
            {
                throw new ArgumentNullException("directiveId" + " cannot be null");
            }
            if(processorName == null)
            {
                throw new ArgumentNullException("processorName" + " cannot be null");
            }
            if(parameterName == null)
            {
                throw new ArgumentNullException("parameterName" + " cannot be null");
            }
            // if the parameter name is not null,then we try to resolve it in the host
            // if no specified value is suitable for the parameter,then we return null
            string returnValue = string.Empty;
            switch (parameterName)
            {
                case "ModuleName":
                    returnValue = Params.ModuleName;
                    break;
                case "ModuleCode":
                    returnValue = Params.ModuleCode;
                    break;
                case "Namespace":
                    returnValue = Params.Namespace;
                    break;
                case "AssertionsNamespace":
                    returnValue = Params.AssertionsNamespace;
                    break;
                case "LTSNamespace":
                    returnValue = Params.LtsNamespace;
                    break;
                case "UltilityNamespace":
                    returnValue = Params.UltilityNamespace;
                    break;
                case "DllDirection":
                    returnValue = Params.DllDirection;
                    break;
                case "EmbeddedResource":
                    returnValue = Params.EmbeddedResource;
                    break;
                case "Classes":
                    returnValue = Params.Classes;
                    break;
                case "ClassName":
                    returnValue = Params.ClassName;
                    break;
                default:
                    break;
            }
            return returnValue;
        }

        public AppDomain ProvideTemplatingAppDomain(string content)
        {
            return AppDomain.CreateDomain(content);
        }

        public void LogErrors(CompilerErrorCollection errors)
        {
            _errorsValue = errors;
        }

        /// <summary>
        /// The engine calls this method to change the extension
        ///  of the generated csharp output file based on the optional
        ///  output directive if the user specifies it in the text template
        /// </summary>
        /// <param name="extension"></param>
        public void SetFileExtension(string extension)
        {
            // if the extension hasn't "." in front of it
            // add one
            if (!extension.Contains("."))
            {
                extension = "." + extension;
            }
            _fileExtensionValue = extension;
        }

        /// <summary>
        /// reset the file extension to the default value
        /// </summary>
        public void ResetFileExtension()
        {
            _fileExtensionValue = ".cs";
        }

        /// <summary>
        /// change the encoding of the generated text output file
        /// </summary>
        /// <param name="encoding"></param>
        /// <param name="fromOutputDirective"></param>
        public void SetOutputEncoding(Encoding encoding, bool fromOutputDirective)
        {
            _fileEncodingValue = encoding;
        }

        /// <summary>
        /// Called by the engine to enquire about
        /// the processing options you require.
        /// If you recognize that option, return an appropriate value
        /// Otherwise, pass back NULL
        /// </summary>
        /// <param name="optionName"></param>
        /// <returns></returns>
        public object GetHostOption(string optionName)
        {
            object returnObj;
            switch (optionName)
            {
                case "CacheAssemblies":
                    returnObj = true;
                    break;
                default:
                    returnObj = null;
                    break;
            }
            return returnObj;
        }

        /// <summary>
        /// The host can provide standard assembly references.
        /// The engine will use these references when compiling and 
        /// excuting the generated transformation class.
        /// </summary>
        public IList<string> StandardAssemblyReferences
        {
            get
            {
                return new[]
                           {
                               typeof (Uri).Assembly.Location
                           };
            }
        }

        /// <summary>
        /// The host can provide standard imports or using statements.
        /// The engine will add these statements to generated
        /// transformation class
        /// </summary>
        public IList<string> StandardImports
        {
            get
            {
                return new[]
                           {
                               "System"
                           };
            }
        }
        #endregion
    }
}
