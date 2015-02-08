using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.ModuleInterface;
using PAT.Common.GUI;

namespace PAT.Common.Ultility
{
    public sealed class Ultility
    {

        public const string APPLICATION_NAME = "PAT 3";
        public const string APPLICATION_FULL_NAME = "Process Analysis Toolkit";

        public const string PAT_EMAIL = "pat@comp.nus.edu.sg";
        public static string PUBLISH_URL = "http://www.comp.nus.edu.sg/~pat/publish35/PAT 3.application";

        public static string PUBLISH_URL_COM = "http://www.comp.nus.edu.sg/~pat/publish3com/PAT 3.application";

        public static bool LoadMathDLL = true;

        public static Dictionary<string, MethodInfo> CSharpMethods = new Dictionary<string, MethodInfo>();
        public static Dictionary<string, Type> CSharpDataType = new Dictionary<string, Type>();

        public static string APPLICATION_PATH = System.AppDomain.CurrentDomain.BaseDirectory;
        public static string ExampleFolderPath = Path.Combine(APPLICATION_PATH, "Examples"); //Application.StartupPath + "\\Lib";
        public static string LibFolderPath = Path.Combine(APPLICATION_PATH, "Lib"); //Application.StartupPath + "\\Lib";
        public static string ModuleFolderPath = Path.Combine(APPLICATION_PATH, "Modules"); //Application.StartupPath + "\\Lib";

        public static bool SEND_EMAIL_USE_SSL = true;

        public static bool IsWindowsOS
        {
            get
            {
                return (Environment.OSVersion.Platform == PlatformID.Win32NT || Environment.OSVersion.Platform == PlatformID.Win32Windows);
            }
        }

        public static bool IsUnixOS
        {
            get
            {
                //return true;
                int p = (int)Environment.OSVersion.Platform;

                //mac shows 4, but PlatformID.MacOSX is 6 actually.
                if ((p == 4) || (p == 6) || (p == 128) || Environment.OSVersion.Platform == PlatformID.MacOSX)
                {
                    //Console.WriteLine("Running on Unix");
                    return true;
                }
                else
                {
                    //Console.WriteLine("NOT running on Unix");
                    return false;
                }    
                
                //return (Environment.OSVersion.Platform == PlatformID.MacOSX);
            }
        }

        public static string AssemblyVersion(Assembly assembly)
        {
            string s = assembly.GetName().Version.ToString();
            string[] sarray = s.Split('.');
            return sarray[0] + "." + sarray[1] + "." + sarray[2] + " (Build " + sarray[3] + ")";
        }


        //module information
        public static List<string> ModuleFolderNames;
        public static List<string> ModuleNames;
        public static Dictionary<string, ModuleFacadeBase> ModuleDictionary;

        public static ModuleFacadeBase LoadModule(string moduleName)
        {
            try
            {

                string facadeClass = "PAT." + moduleName + ".ModuleFacade";

                string file = Path.Combine(Path.Combine(Common.Ultility.Ultility.ModuleFolderPath, moduleName), "PAT.Module." + moduleName + ".dll");
                Assembly assembly1 = Assembly.LoadFrom(file);

                ModuleFacadeBase CurrentModule = (ModuleFacadeBase)assembly1.CreateInstance(
                                                                        facadeClass,
                                                                        true,
                                                                        BindingFlags.CreateInstance,
                                                                        null, null,
                    //new object[] {moduleNode.Attributes["name"].InnerText}
                                                                        null, null);


               
                return CurrentModule;

            }
            catch
            {
            }
            return null;
        }

        public static void ClearDataLibary()
        {
            CSharpMethods.Clear();
            CSharpDataType.Clear();
            //MacroDefinition.Clear();
        }

        public static Icon GetModuleIcon(string key)
        {
            return Icon.FromHandle((((Bitmap)GetModuleImage(key)).GetHicon())); 
        }

        public static Image GetModuleImage(string key)
        {
            if (ModuleDictionary.ContainsKey(key))
            {
                return ModuleDictionary[key].ModuleIcon;
            }
            else
            {
                return Images.ImageList.Images["Error"];
            }
        }


        public static string UserDocumentFolderPath
        {
            get
            {
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "PAT User Setting");
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception)
                {
                }

                return path;
            }
        }

        //public static void LoadSupportingLibrary()
        //{
        //    CSharpMethods = new Dictionary<string, MethodInfo>();

        //    LoadMathLib();
            
        //    if (Directory.Exists(LibFolderPath))
        //    {
        //        string[] libFiles = Directory.GetFiles(LibFolderPath);
                
        //        foreach (string dll in libFiles)
        //        {
        //            if(dll.ToLower().EndsWith("dll"))
        //            {
        //                LoadDLLLibrary(dll);
        //            }
        //        }
        //    }
        //}

        public static void LoadDLLLibrary(string dllName)
        {
            try
            {
                //string oldFile = Path.Combine(Application.StartupPath, dllName);

                //AppDomainSetup ads = new AppDomainSetup();
                //ads.ApplicationName = "PATDomain" + counter;
                //ads.ShadowCopyFiles = "true";
                //ads.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory + @"Lib"; 
                //ads.PrivateBinPath = @"Lib";
                //ads.PrivateBinPathProbe = "true";



                //Directory.SetCurrentDirectory(LibFolderPath);

                // Set up the Evidence
                //Evidence evidence = new Evidence(AppDomain.CurrentDomain.Evidence);
                //evidence.Clear();

             
                //AppDomain newDomain = AppDomain.CreateDomain("PATDomain" + counter, null, ads); //, new Evidence(), LibFolderPath, LibFolderPath, true);
                //newDomain.AssemblyResolve += new ResolveEventHandler(newDomain_AssemblyResolve);
                //newDomain.ReflectionOnlyAssemblyResolve += new ResolveEventHandler(newDomain_ReflectionOnlyAssemblyResolve);

                byte[] rawAssembly = LoadFile(dllName);
                Assembly systemAssembly = Assembly.Load(rawAssembly); // newDomain.Load(rawAssembly);


                //cant call the entry method if the assembly is null
                if (systemAssembly != null)
                {
                    //Use reflection to get a reference to the Math class

                    Module[] modules = systemAssembly.GetModules(false);
                    Type[] types = modules[0].GetTypes();

                    //loop through each class that was defined and look for the first occurrance of the Math class
                    foreach (Type type in types)
                    {
                        if (type.Namespace == "PAT.Lib")
                        {
                            // get all of the members of the math class and map them to the same member
                            // name in uppercase
                            MethodInfo[] mis = type.GetMethods();
                            foreach (MethodInfo mi in mis)
                            {
                                string name = mi.Name + mi.GetParameters().Length;
                                if (mi.IsStatic && !CSharpMethods.ContainsKey(name))
                                {
                                    if (mi.ReturnType.Name == "Void" || mi.ReturnType.Name == "Int32" || mi.ReturnType.Name == "Int16" || mi.ReturnType.Name == "Byte" || mi.ReturnType.Name == "Boolean" || mi.ReturnType.Name == "Int32[]" || mi.ReturnType.Name == "UserDefinedDataType")
                                    {
                                        CSharpMethods.Add(name, mi);
                                    }
                                }
                            }

                            if(type.IsSubclassOf(typeof(ExpressionValue)))
                            {
                                if (!CSharpDataType.ContainsKey(type.Name))
                                {
                                    CSharpDataType.Add(type.Name, type);
                                }
                            }
                        }                        
                    }
                }
                //systemAssembly = null;
                //System.AppDomain.Unload(newDomain);
                //newDomain = null;
                //ads = null;

            }
            catch (Exception ex)
            {
                //throw ex;
                MessageBox.Show("Error happened in Loading " + dllName + "! Please make sure you put the it under Lib Folder of PAT.\r\nIf you are using Vista, please disable the UAC or install PAT other than C:\\Programe Files.", APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        public static void LoadAddProjectLibrary(string dllName)
        {
            try
            {
                byte[] rawAssembly = LoadFile(dllName);
                Assembly systemAssembly = Assembly.Load(rawAssembly); // newDomain.Load(rawAssembly);


                //cant call the entry method if the assembly is null
                if (systemAssembly != null)
                {
                    //Use reflection to get a reference to the Math class

                    Module[] modules = systemAssembly.GetModules(false);
                    Type[] types = modules[0].GetTypes();

                    //loop through each class that was defined and look for the first occurrance of the Math class
                    foreach (Type type in types)
                    {
                        //if (type.Namespace == "PAT.Lib")
                        {
                            // get all of the members of the math class and map them to the same member
                            // name in uppercase
                            MethodInfo[] mis = type.GetMethods();
                            foreach (MethodInfo mi in mis)
                            {
                                string name = mi.Name + mi.GetParameters().Length;
                                if (mi.IsStatic && !CSharpMethods.ContainsKey(name))
                                {
                                    if (mi.ReturnType.Name == "Void" || mi.ReturnType.Name == "Int32" || mi.ReturnType.Name == "Int16" || mi.ReturnType.Name == "Byte" || mi.ReturnType.Name == "Boolean" || mi.ReturnType.Name == "Int32[]" || mi.ReturnType.Name == "UserDefinedDataType")
                                    {
                                        CSharpMethods.Add(name, mi);
                                    }
                                }
                            }

                            if (type.IsSubclassOf(typeof(ExpressionValue)))
                            {
                                if (!CSharpDataType.ContainsKey(type.Name))
                                {
                                    CSharpDataType.Add(type.Name, type);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //throw ex;
                MessageBox.Show("Error happened in Loading " + dllName + "! " + ex.Message + "\r\n" + ex.StackTrace, APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        public static Expression InitializeUserDefinedDataType(string type)
        {
            if(CSharpDataType.ContainsKey(type))
            {
                try
                {
                    return Activator.CreateInstance(CSharpDataType[type]) as ExpressionValue;    
                }
                catch (Exception)
                {
                    
                }
                
            }
            
            return null;
        }

        public static bool IsUserDefinedDataTypeDefined(string type)
        {
            return CSharpDataType.ContainsKey(type);
        }

        public static void LoadMathLib()
        {
            try
            {
                // get a reflected assembly of the System assembly
                Assembly systemAssembly = Assembly.GetAssembly(typeof(System.Math));

                //cant call the entry method if the assembly is null
                if (systemAssembly != null)
                {
                    //Use reflection to get a reference to the Math class

                    Module[] modules = systemAssembly.GetModules(false);
                    Type[] types = modules[0].GetTypes();

                    //loop through each class that was defined and look for the first occurrance of the Math class
                    foreach (Type type in types)
                    {
                        if (type.Name == "Math")
                        {
                            // get all of the members of the math class and map them to the same member
                            // name in uppercase
                            MethodInfo[] mis = type.GetMethods();
                            foreach (MethodInfo mi in mis)
                            {
                                string name = mi.Name + mi.GetParameters().Length;
                                if (mi.IsStatic && !CSharpMethods.ContainsKey(name))
                                {
                                    if (mi.ReturnType.Name == "Int32" || mi.ReturnType.Name == "Double")
                                    {
                                        CSharpMethods.Add(name, mi);
                                    }
                                }
                            }
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                //throw ex;
                MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace, APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        public static byte[] LoadFile(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Open);            
            byte[] buffer = new byte[(int)fs.Length];
            fs.Read(buffer, 0, buffer.Length);
            fs.Close();
            
            return buffer;
        }

        public static ImageConstantLibrary Images = new ImageConstantLibrary();

        public static Image GetImage(string key)
        {
            if (Images.ImageList.Images.IndexOfKey(key) != -1)
            {
                return Images.ImageList.Images[key];
            }
            else
            {
                return Images.ImageList.Images["Error"];
            }
        }



        public static Icon GetIcon(string key)
        {
            return Icon.FromHandle(((Bitmap) Ultility.GetImage(key)).GetHicon());
        }

        //Exception Logging for the application
        public static void LogException(Exception ex, SpecificationBase spec)
        {
            ExceptionDialog log = new ExceptionDialog(ex, APPLICATION_NAME, spec);
            log.ShowDialog();
            if (Common.Ultility.Ultility.IsWindowsOS)
            {
                FlashWindowEx(log);
            }
        }

        //Exception Logging for the application
        public static void LogRuntimeException(RuntimeException ex)
        {
            RuntimeExceptionDialog log = new RuntimeExceptionDialog(ex, APPLICATION_NAME);
            log.ShowDialog();
            if (Common.Ultility.Ultility.IsWindowsOS)
            {
                FlashWindowEx(log);
            }
        }

        
  

        public static string GenerateLaTexString(Expression exp)
        {
            switch (exp.ExpressionType)
            {
                case ExpressionType.Variable:
                    return exp.ToString();
                case ExpressionType.Constant:
                    if (exp is BoolConstant)
                    {
                        return ((BoolConstant)exp).Value.ToString();
                    }
                    else
                    {
                        return exp.ToString();
                    }
                case ExpressionType.Record:
                    {
                        Record record = exp as Record;

                        StringBuilder sb = new StringBuilder("[");

                        for (int i = 0; i < record.Associations.Length - 1; i++)
                        {
                            Expression con = record.Associations[i];
                            sb.Append(GenerateLaTexString(con) + ", ");
                        }
                        sb.Append(GenerateLaTexString(record.Associations[record.Associations.Length - 1]));

                        sb.Append("]");
                        return sb.ToString();
                    }
                case ExpressionType.PrimitiveApplication:
                    // First evaluate the first argument, then the second, and
                    // then evaluate using evalPrimAppl.
                    {


                        PrimitiveApplication newexp = exp as PrimitiveApplication;



                        if (newexp.Argument2 == null)
                        {
                            if (newexp.Operator == "~")
                            {
                                return "-" + GenerateLaTexString(newexp.Argument1);
                            }
                            else
                            {
                                return newexp.Operator + GenerateLaTexString(newexp.Argument1);
                            }
                        }
                        else
                        {
                            if (newexp.Operator == ".")
                            {
                                return GenerateLaTexString(newexp.Argument1) + "[" + GenerateLaTexString(newexp.Argument2) + "]";
                            }
                            else if (newexp.Operator == "mod")
                            {
                                return "(" + GenerateLaTexString(newexp.Argument1) + " \\% " + GenerateLaTexString(newexp.Argument2) + ")";
                            }
                            else
                            {
                                string op = "";
                                switch (newexp.Operator)
                                {
                                    case "&&":
                                        op = @"\land";
                                        break;
                                    case "||":
                                        op = @"\lor";
                                        break;
                                    case "==":
                                        op = @"==";
                                        break;
                                    case "!=":
                                        op = @"\neq";
                                        break;
                                    case ">=":
                                        op = @"\geq";
                                        break;
                                    case "<=":
                                        op = @"\leq";
                                        break;
                                    case "\\":
                                        op = @"\backslash";
                                        break;
                                    default:
                                        op = newexp.Operator;
                                        break;
                                }

                                return "(" + GenerateLaTexString(newexp.Argument1) + " " + op + " " + GenerateLaTexString(newexp.Argument2) + ")";
                            }
                        }
                    }
                case ExpressionType.Assignment:
                    {
                        Assignment assign = exp as Assignment;
                        return assign.LeftHandSide + " = " + GenerateLaTexString(assign.RightHandSide) + ";";
                    }
                case ExpressionType.PropertyAssignment:
                    {
                        PropertyAssignment pa = (PropertyAssignment)exp;
                        return GenerateLaTexString(pa.RecordExpression) + "[" + GenerateLaTexString(pa.PropertyExpression) + "]=" + GenerateLaTexString(pa.RightHandExpression) + ";";
                    }
                case ExpressionType.If:
                    // Conditionals are evaluated by evaluating the then-part or 
                    // else-part depending of the result of evaluating the condition.
                    {
                        If ifC = (If)exp;
                        if (ifC.ElsePart != null)
                        {
                            return " if (" + GenerateLaTexString(ifC.Condition) + ") \\{" + GenerateLaTexString(ifC.ThenPart) + "\\} else \\{" + GenerateLaTexString(ifC.ElsePart) + "\\}";
                        }
                        else
                        {
                            return " if (" + GenerateLaTexString(ifC.Condition) + ") \\{" + GenerateLaTexString(ifC.ThenPart) + "\\}";
                        }
                    }
                case ExpressionType.Sequence:

                    return GenerateLaTexString(((Sequence)exp).FirstPart) + ";" + GenerateLaTexString(((Sequence)exp).SecondPart);

                case ExpressionType.While:

                    return "while (" + GenerateLaTexString(((While)exp).Test) + ") \\{" + GenerateLaTexString(((While)exp).Body) + "\\}";
                case ExpressionType.StaticMethodCall:
                    StaticMethodCall call = exp as StaticMethodCall;
                    StringBuilder strbui = new StringBuilder("call(" + call.MethodName + ",");
                    for (int i = 0; i < call.Arguments.Length; i++)
                    {
                        if (i == call.Arguments.Length - 1)
                        {
                            strbui.Append(call.Arguments[i].ToString());
                        }
                        else
                        {
                            strbui.Append(call.Arguments[i] + ",~");
                        }
                    }
                    strbui.Append(")");
                    return strbui.ToString();
            }
            
            return "";
        }



        public static string GenerateLaTexStringFromModel(string CSPmodel, bool showLineNumber)
        {
            //process the model line by line
            String[] strings = Regex.Split(CSPmodel, "\r\n");

            //save the translated string to be returned
            StringBuilder translatedStr = new StringBuilder();

            translatedStr.Append("\\begin{syntax}" + "\r\n");

            //get translation rule
            Dictionary<string, string> Properties = GetProperties();

            int lineNumber = 0;

            foreach (String content in strings)
            {
                string translatedContent = content;

                if (translatedContent.Length != 0)
                {

                    if (showLineNumber)
                    {
                        //add line number
                        translatedContent = (++lineNumber) + ". " + translatedContent;
                    }
                    
                    foreach (KeyValuePair<string, string> keyValuePair in Properties)
                    {

                        Regex RE = new Regex(keyValuePair.Key, RegexOptions.None);
                        translatedContent = RE.Replace(translatedContent, keyValuePair.Value);
                    }

                    // if the operatiors [] and <> in the assertions, replace them by "\always" and "\eventually"
                    if (translatedContent.Contains("\\models"))
                    {
                        Regex RE = new Regex(@"\\extchoice", RegexOptions.None);
                        translatedContent = RE.Replace(translatedContent, "\\always");
                        RE = new Regex(@"\\intchoice", RegexOptions.None);
                        translatedContent = RE.Replace(translatedContent, "\\eventually");
                    }


                    // if the translating operator is followe by a word character, a blank need inserted in between.
                    List<String> values = new List<string>();

                    foreach (KeyValuePair<string, string> keyValuePair in Properties)
                    {
                        values.Add(keyValuePair.Value);
                    }

                    values.Add("\\always");
                    values.Add("\\eventually");

                    foreach (string value in values)
                    {
                        if (Regex.IsMatch(value.Substring(value.Length - 1, 1), @"\w"))
                        {
                            Regex RE = new Regex("\\" + value + @"\B", RegexOptions.None);
                            translatedContent = RE.Replace(translatedContent, value + "~");
                        }

                    }

                }

                translatedContent = translatedContent + "~\\\\" + "\r\n";
                translatedStr.Append(translatedContent);

            }

            translatedStr.Append("\\end{syntax}");
            return translatedStr.ToString();

        }

        private static Dictionary<string, string> GetProperties()
        {
            Dictionary<string, string> Properties = new Dictionary<string, string>();
            string[] kvp;
            string[] records = new string[] { @"\\=\hide", @"_=\_", @"{=\{", @"}=\}", "@=$@$", @"%=\%", @"#=\#", @"&=\&", @"->=\fun", @"\[\]=\extchoice", @"<>=\intchoice", @"\|\|\|=\interleave", @"\|\|=\parallel", @"\|==\models", @"refines\s*?<FD>=\sqsupseteq_{FD}", @"refines\s*?<F>=\sqsupseteq_F", @"refines=\sqsupseteq_T", @"\btau\b=\tau", @"\t=~~~~", @" =~" };
            Regex RE = new Regex("=", RegexOptions.RightToLeft);
            Match theMatch;
            foreach (string record in records)
            {
                theMatch = RE.Match(record);
                kvp = new string[2];
                kvp[0] = record.Substring(0, theMatch.Index);
                kvp[1] = record.Substring(theMatch.Index + 1);
                Properties.Add(kvp[0], kvp[1]);
            }
            return Properties;
        }

        public static bool SendEmail(string pSubject, string pBody, string to)
        {
            string pGmailEmail = "PatMaillist@gmail.com";
            string pGmailPassword = "maillist";
            string pGmailSMTP = "smtp.gmail.com";
            int port = 587;
            string pTo = to;

            SmtpClient client = null;

            if (!SEND_EMAIL_USE_SSL)
            {
                pGmailEmail = "pat@patroot.com";
                pGmailPassword = "Pat123";
                pGmailSMTP = "mail.patroot.com"; // "mail.patroot.com"; //smtp.patroot.com
                port = 25;
                client = new SmtpClient(pGmailSMTP);

                // Add credentials if the SMTP server requires them.
                //client.EnableSsl = true;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(pGmailEmail, pGmailPassword);
                // CredentialCache.DefaultNetworkCredentials;CredentialCache.DefaultNetworkCredentials;// new NetworkCredential(pGmailEmail, pGmailPassword); // CredentialCache.DefaultNetworkCredentials;
            }
            else //if windows, use ssl to send, which is a safer way to avoid unblocking by networks.
            {
                client = new SmtpClient(pGmailSMTP, port);

                // Add credentials if the SMTP server requires them.
                client.EnableSsl = true;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(pGmailEmail, pGmailPassword);
            }
            

            try
            {
                //System.Web.Mail.MailMessage myMail = new System.Web.Mail.MailMessage();
                //myMail.Fields.Add
                //    ("http://schemas.microsoft.com/cdo/configuration/smtpserver",
                //     "smtp.gmail.com");
                //myMail.Fields.Add
                //    ("http://schemas.microsoft.com/cdo/configuration/smtpserverport",
                //     "465");
                //myMail.Fields.Add
                //    ("http://schemas.microsoft.com/cdo/configuration/sendusing",
                //     "2");
                ////sendusing: cdoSendUsingPort, value 2, for sending the message using 
                ////the network.

                ////smtpauthenticate: Specifies the mechanism used when authenticating 
                ////to an SMTP 
                ////service over the network. Possible values are:
                ////- cdoAnonymous, value 0. Do not authenticate.
                ////- cdoBasic, value 1. Use basic clear-text authentication. 
                ////When using this option you have to provide the user name and password 
                ////through the sendusername and sendpassword fields.
                ////- cdoNTLM, value 2. The current process security context is used to 
                //// authenticate with the service.
                //myMail.Fields.Add
                //    ("http://schemas.microsoft.com/cdo/configuration/smtpauthenticate", "1");
                ////Use 0 for anonymous
                //myMail.Fields.Add
                //    ("http://schemas.microsoft.com/cdo/configuration/sendusername",
                //     pGmailEmail);
                //myMail.Fields.Add
                //    ("http://schemas.microsoft.com/cdo/configuration/sendpassword",
                //     pGmailPassword);
                //myMail.Fields.Add
                //    ("http://schemas.microsoft.com/cdo/configuration/smtpusessl",
                //     "true");
                //myMail.From = pGmailEmail;
                //myMail.To = pTo;
                //myMail.Subject = pSubject;
                //myMail.BodyFormat = pFormat;
                //myMail.Body = pBody;
                ////if (pAttachmentPath.Trim() != "")
                ////{
                ////    MailAttachment MyAttachment =
                ////            new MailAttachment(pAttachmentPath);
                ////    myMail.Attachments.Add(MyAttachment);
                ////    myMail.Priority = System.Web.Mail.MailPriority.High;
                ////}

                //System.Web.Mail.SmtpMail.SmtpServer = "smtp.gmail.com:465";
                //System.Web.Mail.SmtpMail.Send(myMail);
                //return true;


                System.Net.Mail.MailMessage myMailNew = new MailMessage(pGmailEmail, pTo, pSubject, pBody);

                               
              
                
                


                //try
                //{
                client.Send(myMailNew);
                //}
                //catch (Exception ex)
                //{
                //    Console.WriteLine("Exception caught in CreateMessageWithAttachment(): {0}",
                //                      ex.ToString());
                //}

                //myMailNew.Fields.Add
                //   ("http://schemas.microsoft.com/cdo/configuration/smtpserver",
                //    "smtp.gmail.com");
                //myMail.Fields.Add
                //    ("http://schemas.microsoft.com/cdo/configuration/smtpserverport",
                //     "465");
                //myMail.Fields.Add
                //    ("http://schemas.microsoft.com/cdo/configuration/sendusing",
                //     "2");
                ////sendusing: cdoSendUsingPort, value 2, for sending the message using 
                ////the network.

                ////smtpauthenticate: Specifies the mechanism used when authenticating 
                ////to an SMTP 
                ////service over the network. Possible values are:
                ////- cdoAnonymous, value 0. Do not authenticate.
                ////- cdoBasic, value 1. Use basic clear-text authentication. 
                ////When using this option you have to provide the user name and password 
                ////through the sendusername and sendpassword fields.
                ////- cdoNTLM, value 2. The current process security context is used to 
                //// authenticate with the service.
                //myMail.Fields.Add
                //    ("http://schemas.microsoft.com/cdo/configuration/smtpauthenticate", "1");
                ////Use 0 for anonymous
                //myMail.Fields.Add
                //    ("http://schemas.microsoft.com/cdo/configuration/sendusername",
                //     pGmailEmail);
                //myMail.Fields.Add
                //    ("http://schemas.microsoft.com/cdo/configuration/sendpassword",
                //     pGmailPassword);
                //myMail.Fields.Add
                //    ("http://schemas.microsoft.com/cdo/configuration/smtpusessl",
                //     "true");
                //myMail.From = pGmailEmail;
                //myMail.To = pTo;
                //myMail.Subject = pSubject;
                //myMail.BodyFormat = pFormat;
                //myMail.Body = pBody;
                ////if (pAttachmentPath.Trim() != "")
                ////{
                ////    MailAttachment MyAttachment =
                ////            new MailAttachment(pAttachmentPath);
                ////    myMail.Attachments.Add(MyAttachment);
                ////    myMail.Priority = System.Web.Mail.MailPriority.High;
                ////}

                //System.Web.Mail.SmtpMail.SmtpServer = "smtp.gmail.com:465";
                //System.Web.Mail.SmtpMail.Send(myMail);
                return true;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error happened in sending email:" + ex.Message + "\r\n" + "Try to switch on/off the \"Sending email using SSL\" option in the Tools -> Options");                
            }

            return false;
        }

        public static bool IsAValidName(string word)
        {
            return !string.IsNullOrEmpty(word) && (char.IsLetter(word[0]) || word[0] == '_');
        }



        //public static bool IsLTSModule(string moduleName)
        //{
        //    if (moduleName == "Labeled Transition Systems" || moduleName == "Timed Automata")
        //    {
        //        return true;
        //    }

        //    return false;
        //}


        #region resource management

        public static ResourceManager resmgr = new ResourceManager("PAT.Common.Resources", Assembly.GetExecutingAssembly());
        public static void SetCulture(CultureInfo culture)
        {
            Resources.Culture = culture;
        }

        public static Image GetResourceImage(string key)
        {
            object obj = resmgr.GetObject(key);
            if (obj != null)
            {
                return obj as Bitmap;
            }
            return Images.ImageList.Images["Error"];
            //if (Images.ImageList.Images.IndexOfKey(key) != -1)
            //{
            //    return Images.ImageList.Images[key];
            //}
            //else
            //{
            //    return Images.ImageList.Images["Error"];
            //}
        }

        //public static string GetRecourseString(string key)
        //{
        //    return resmgr.GetString(key, Thread.CurrentThread.CurrentUICulture);
        //}

        #endregion

        public enum LicenseType
        {
            Valid,
            Evaluation,
            Invalid
            
        }

        public static LicenseType IsValidLicenseAvailable = LicenseType.Valid;
        public const int LicenseBoundedStateNumber = 10000;

#if DEBUG
        public const bool MULTI_CORE = true;
#else
        public const bool MULTI_CORE = false;
#endif

#if ADD_IN
        public const bool ADD_IN = true;
#else
        public const bool ADD_IN = false;
#endif


        #region "Flash Toolbar"

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool FlashWindowEx(ref FLASHWINFO pwfi);

        [StructLayout(LayoutKind.Sequential)]
        public struct FLASHWINFO
        {
            public UInt32 cbSize;
            public IntPtr hwnd;
            public UInt32 dwFlags;
            public UInt32 uCount;
            public UInt32 dwTimeout;
        }

        //Stop flashing. The system restores the window to its original state. 
        public const UInt32 FLASHW_STOP = 0;
        //Flash the window caption. 
        public const UInt32 FLASHW_CAPTION = 1;
        //Flash the taskbar button. 
        public const UInt32 FLASHW_TRAY = 2;
        //Flash both the window caption and taskbar button.
        //This is equivalent to setting the FLASHW_CAPTION | FLASHW_TRAY flags. 
        public const UInt32 FLASHW_ALL = 3;
        //Flash continuously, until the FLASHW_STOP flag is set. 
        public const UInt32 FLASHW_TIMER = 4;
        //Flash continuously until the window comes to the foreground. 
        public const UInt32 FLASHW_TIMERNOFG = 12; 

        /// Minor adjust to the code above
        /// <summary>
        /// Flashes a window until the window comes to the foreground
        /// Receives the form that will flash
        /// </summary>
        /// <param name="hWnd">The handle to the window to flash</param>
        /// <returns>whether or not the window needed flashing</returns>
        public static bool FlashWindowEx(Form frm)
        {
            if (Common.Ultility.Ultility.IsWindowsOS)
            {
                IntPtr hWnd = frm.Handle;
                FLASHWINFO fInfo = new FLASHWINFO();

                fInfo.cbSize = Convert.ToUInt32(Marshal.SizeOf(fInfo));
                fInfo.hwnd = hWnd;
                fInfo.dwFlags = FLASHW_ALL | FLASHW_TIMERNOFG;
                fInfo.uCount = UInt32.MaxValue;
                fInfo.dwTimeout = 0;

                return FlashWindowEx(ref fInfo);
            }

            return false;
        }
        
        #endregion

    }
}