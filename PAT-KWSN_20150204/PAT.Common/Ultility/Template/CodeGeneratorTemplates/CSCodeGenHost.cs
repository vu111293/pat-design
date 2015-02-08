/**
 * PAT.CSP.CodeGenerator.CS
 *
 * It generate C# program from CSP# model
 * The CSP features are provided in PAT.Runtime.dll
 *
 * School of Computing, National University of Singapore
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TextTemplating;
using System.IO;
using System.CodeDom.Compiler;

namespace PAT.CSP.CodeGenerator.CS
{
    [Serializable]
    public class CSCodeGenHost : ITextTemplatingEngineHost
    {
        private string templateFile;// the path of template file
        private string fileExtension;
        private Encoding fileEncoding = Encoding.UTF8;

        private string projectName;
        private string classNames;
        private string extlibNames;


        // the standard returns from examples
		public IList<string> StandardAssemblyReferences
		{
			get { return new[] { typeof(Uri).Assembly.Location }; }
		}

		public IList<string> StandardImports
		{
			get { return new[] { "System" }; }
		}

		public string TemplateFile { get { return templateFile; } }

		public Encoding FileEncoding { get { return fileEncoding; } }

        // methods start
        public CSCodeGenHost(string projName)
        {
            projectName = projName;
        }

        public void SetTemplateFilePath(string tfPath)
        {
            templateFile = tfPath;
            return;
        }

        public void SetClassesString(string clsNames)
        {
            classNames = clsNames;
            return;
        }

        public void SetExtlibString(string nspNames)
        {
            extlibNames = nspNames;
            return;
        }


        // return null as we do not need it currently
		public object GetHostOption(string optionName)
		{
            return null;
		}

        // currently we don't process included text
		public bool LoadIncludeText(string requestFileName, out string content, out string location)
		{
			//throw new NotImplementedException();
            content = System.String.Empty;
            location = System.String.Empty;
            //SUtil.LogMes("shall not call CSCodeGenHost::LoadIncludeText");
            return false;
		}

        
        // logged the errors' text to our logging utility
		public void LogErrors(System.CodeDom.Compiler.CompilerErrorCollection errors)
		{
            //SUtil.LogErr(errors.ToString());
			foreach (System.CodeDom.Compiler.CompilerError e in errors) {
				//SUtil.LogErr(e.ToString());
			}
            return;
		}

        // currently create new domain each time using the template
		public AppDomain ProvideTemplatingAppDomain(string content)
		{
            return AppDomain.CreateDomain("Generation App Domain");
		}

        // resolve only if it's already resolved, else return empty
		public string ResolveAssemblyReference(string assemblyReference)
		{
            if (File.Exists(assemblyReference)) {
                return assemblyReference;
            }
            return String.Empty;
		}

        // throw exception if the directive processor not found
		public Type ResolveDirectiveProcessor(string processorName)
		{
            throw new Exception("Directive processor Not Found");
		}

        // return corresponding parameter's name
		public string ResolveParameterValue(string directiveId, string processorName, string parameterName)
		{
            if (directiveId == null) {
                throw new ArgumentNullException("directiveId" + " cannot be null");
            }
            if (processorName == null) {
                throw new ArgumentNullException("processorName" + " cannot be null");
            }
            if (parameterName == null) {
                throw new ArgumentNullException("parameterName" + " cannot be null");
            }
            string returnValue = string.Empty;
            switch (parameterName)
            {
                case "ProjectName":
                    returnValue = projectName;
                    break;
                case "classNames":
                    returnValue = classNames;
                    break;
				case "ExtlibNames":
                    returnValue = extlibNames;
					break;
                default:
                    break;
            }
            return returnValue;
		}

        // try to get the path fo find the file
		public string ResolvePath(string path)
		{
            if (path == null) {
                throw new ArgumentNullException("Path cannot" + " be null");
            }
            if (File.Exists(path)) {
                return path;
            }
            string candidate = Path.Combine(Path.GetDirectoryName(TemplateFile), path);
            if (File.Exists(candidate)) {
                return candidate;
            }
            return path;
		}

		public void SetFileExtension(string extension)
		{
            fileExtension = extension;
		}

		public void SetOutputEncoding(Encoding encoding, bool fromOutputDirective)
		{
            fileEncoding = encoding;
		}
    }
}
