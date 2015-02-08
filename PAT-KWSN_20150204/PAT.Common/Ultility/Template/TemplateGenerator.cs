/**
 * To Generate the specified module solution
 * @author Ma Junwei, 30/12/2010 
 * */

using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TextTemplating;

namespace PAT.Common.Ultility.Template
{
    public class TemplateGenerator
    {
        public static void GenerateFileAsDefault(ITextTemplatingEngineHost host, string templateFilePath, string outputFileName)
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            Stream myStream = myAssembly.GetManifestResourceStream(templateFilePath);

            if (myStream != null)
            {
                StreamReader reader = new StreamReader(myStream);
                string text = reader.ReadToEnd();

                Engine engine = new Engine();

                // use the engine to transfor the template
                string output = engine.ProcessTemplate(text, host);


                File.WriteAllText(outputFileName, output, Encoding.UTF8);
            }

            //throw new Exception("Wrong template file path");
        }
    }
}

