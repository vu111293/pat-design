using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;
using PAT.Common.Classes.ModuleInterface;


namespace PAT.Common.GUI
{
    public partial class ExceptionDialog: Form
    {
        private SpecificationBase spec;

        public ExceptionDialog(Exception ex, string APPLICATION_NAME, SpecificationBase specification)
        {
            InitializeComponent();

            spec = specification;

            this.Text = APPLICATION_NAME + " " + this.Text;

            ActionBox.Text = Resources.You_have_following_options_ + Environment.NewLine;
            ActionBox.Text += Resources._1__Click_the_Email_button_to_report_the_error_to_us_ + Environment.NewLine;
            ActionBox.Text += Resources._2__Click_the_Continue_button_to_ingore_the_error_ + Environment.NewLine;
            ActionBox.Text += Resources._3__Click_the_Stop_button_to_exit_the_application_ + Environment.NewLine;
            ActionBox.Text += Resources.At_same_time__you_can_submit_your_model_to_us_by_using_model_submit_button_in_the_PAT_editor_ + Environment.NewLine;

            DetailBox.Text = ExceptionToString(ex);

            string trace = "";
            if (ex.Data.Contains("trace"))
            {
                trace = Environment.NewLine + "Trace leads to exception:" + Environment.NewLine + ex.Data["trace"].ToString();
            }
            ErrorBox.Text = ErrorBox.Text + APPLICATION_NAME + ". " + Environment.NewLine + ex.Message + trace;

        }

        private void Button_Email_Click(object sender, EventArgs e)
        {
            if (SendEmail())
            {
                MessageBox.Show(Resources.Email_is_sent_successfully_, Common.Ultility.Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(Resources.Email_can_not_be_sent__please_manual_email_the_exception_to_PAT_team_, Common.Ultility.Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        /// <summary>
        /// translate exception object to string, with additional system info
        /// </summary>
        /// <param name="objException"></param>
        private static string ExceptionToString(Exception objException)
        {
            StringBuilder objStringBuilder = new StringBuilder();
            if (objException.InnerException != null)
            {
                //-- sometimes the original exception is wrapped in a more relevant outer exception
                //-- the detail exception is the "inner" exception
                //-- see http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dnbda/html/exceptdotnet.asp
                objStringBuilder.Append("(Inner Exception)");
                objStringBuilder.Append(Environment.NewLine);
                objStringBuilder.Append(ExceptionToString(objException.InnerException));
                objStringBuilder.Append(Environment.NewLine);
                objStringBuilder.Append("(Outer Exception)");
                objStringBuilder.Append(Environment.NewLine);
            }
            //-- get general system and app information
            objStringBuilder.Append(SysInfoToString());

            //'-- get exception-specific information
            objStringBuilder.Append("Exception Source:      ");

            try
            {
                objStringBuilder.Append(objException.Source);
            }
            catch (Exception e)
            {
                objStringBuilder.Append(e.Message);
            }

            objStringBuilder.Append(Environment.NewLine);

            objStringBuilder.Append("Exception Type:        ");

            try
            {
                objStringBuilder.Append(objException.GetType().FullName);
            }
            catch (Exception e)
            {
                objStringBuilder.Append(e.Message);
            }


            objStringBuilder.Append(Environment.NewLine);

            objStringBuilder.Append("Exception Message:     ");

            try
            {
                objStringBuilder.Append(objException.Message);
            }
            catch (Exception e)
            {
                objStringBuilder.Append(e.Message);
            }

            objStringBuilder.Append(Environment.NewLine);

            objStringBuilder.Append("Exception Target Site: ");

            try
            {
                objStringBuilder.Append(objException.TargetSite.Name);
            }
            catch (Exception e)
            {
                objStringBuilder.Append(e.Message);
            }

            objStringBuilder.Append(Environment.NewLine);


            try
            {
                objStringBuilder.Append(EnhancedStackTrace(objException));
            }
            catch (Exception e)
            {
                objStringBuilder.Append(e.Message);
            }

            objStringBuilder.Append(Environment.NewLine);
            return objStringBuilder.ToString();
        }

        //        '--
        //'-- gather some system information that is helpful to diagnosing
        //'-- exception
        //'--
        public static string SysInfoToString()
        {
            StringBuilder objStringBuilder = new StringBuilder();


            objStringBuilder.Append("Date and Time:         ");
            objStringBuilder.Append(DateTime.Now);
            objStringBuilder.Append(Environment.NewLine);

            objStringBuilder.Append("Machine Name:          ");
            try
            {
                objStringBuilder.Append(Environment.MachineName);
            }
            catch (Exception e)
            {
                objStringBuilder.Append(e.Message);
            }
            objStringBuilder.Append(Environment.NewLine);

            objStringBuilder.Append("IP Address:            ");
            objStringBuilder.Append(GetCurrentIP());
            objStringBuilder.Append(Environment.NewLine);

            objStringBuilder.Append("Current User:          ");
            objStringBuilder.Append(UserIdentity());
            objStringBuilder.Append(Environment.NewLine);
            objStringBuilder.Append(Environment.NewLine);

            objStringBuilder.Append("Application Domain:    ");
            try
            {
                objStringBuilder.Append(AppDomain.CurrentDomain.FriendlyName);
            }
            catch (Exception e)
            {
                objStringBuilder.Append(e.Message);
            }


            objStringBuilder.Append(Environment.NewLine);
            objStringBuilder.Append("Assembly Codebase:     ");
            try
            {
                objStringBuilder.Append(ParentAssembly.CodeBase);
            }
            catch (Exception e)
            {
                objStringBuilder.Append(e.Message);
            }
            objStringBuilder.Append(Environment.NewLine);

            objStringBuilder.Append("Assembly Full Name:    ");
            try
            {
                objStringBuilder.Append(ParentAssembly.FullName);
            }
            catch (Exception e)
            {
                objStringBuilder.Append(e.Message);
            }
            objStringBuilder.Append(Environment.NewLine);

            objStringBuilder.Append("Assembly Version:      ");
            try
            {
                objStringBuilder.Append(ParentAssembly.GetName().Version.ToString());
            }
            catch (Exception e)
            {
                objStringBuilder.Append(e.Message);
            }
            objStringBuilder.Append(Environment.NewLine);

            //objStringBuilder.Append("Assembly Build Date:   ");
            //try
            //{
            //    objStringBuilder.Append(AssemblyBuildDate(ParentAssembly).ToString());
            //  }
            //catch (Exception e)
            //{
            //    objStringBuilder.Append(e.Message);
            //}
            objStringBuilder.Append(Environment.NewLine);
            objStringBuilder.Append(Environment.NewLine);

            //If blnIncludeStackTrace Then
            //    objStringBuilder.Append(EnhancedStackTrace());
            //End If

            return objStringBuilder.ToString();
        }

        private static Assembly _objParentAssembly;

        private static Assembly ParentAssembly
        {
            get
            {
                if (_objParentAssembly == null)
                {
                    if (Assembly.GetEntryAssembly() == null)
                    {
                        _objParentAssembly = Assembly.GetCallingAssembly();
                    }
                    else
                    {
                        _objParentAssembly = Assembly.GetEntryAssembly();
                    }
                }
                return _objParentAssembly;
            }
        }

        //        '--
        //'-- get IP address of this machine
        //'-- not an ideal method for a number of reasons (guess why!)
        //'-- but the alternatives are very ugly
        //'--
        private static string GetCurrentIP()
        {
            try
            {
                return Dns.GetHostEntry(Dns.GetHostName()).AddressList[0].ToString();
            }
            catch
            {
                return "127.0.0.1";
            }
        }

        //        '--
        //'-- exception-safe WindowsIdentity.GetCurrent retrieval returns "domain\username"
        //'-- per MS, this sometimes randomly fails with "Access Denied" particularly on NT4
        //'--
        private static string CurrentWindowsIdentity()
        {
            try
            {
                return WindowsIdentity.GetCurrent().Name;
            }
            catch
            {
                return "";
            }
        }

        //'--
        //'-- exception-safe "domain\username" retrieval from Valuation
        //'--
        private static string CurrentEnvironmentIdentity()
        {
            try
            {
                return Environment.UserDomainName + "\\" + Environment.UserName;
            }
            catch
            {
                return "";
            }
        }


        //'--
        //'-- retrieve identity with fallback on error to safer method
        //'--
        private static string UserIdentity()
        {
            string strTemp = CurrentWindowsIdentity();
            if (strTemp == "")
            {
                strTemp = CurrentEnvironmentIdentity();
            }

            return strTemp;
        }

        //        '--
        //'-- enhanced stack trace generator
        //'--
        private static string EnhancedStackTrace(Exception objException)
        {
            StackTrace objStackTrace = new StackTrace(objException, true);
            StringBuilder sb = new StringBuilder();

            sb.Append(Environment.NewLine);
            sb.Append("---- Stack Trace ----");
            sb.Append(Environment.NewLine);

            for (int i = 0; i < objStackTrace.FrameCount; i++)
            {
                StackFrame sf = objStackTrace.GetFrame(i);
                MemberInfo mi = sf.GetMethod();

                sb.Append(StackFrameToString(sf));
            }

            sb.Append(Environment.NewLine);

            return sb.ToString();
        }


        //'--
        //'-- turns a single stack frame object into an informative string
        //'--
        private static string StackFrameToString(StackFrame sf)
        {
            StringBuilder sb = new StringBuilder();

            int intParam;
            MemberInfo mi = sf.GetMethod();


            //'-- build method name
            sb.Append("   ");
            sb.Append(mi.DeclaringType.Namespace);
            sb.Append(".");
            sb.Append(mi.DeclaringType.Name);
            sb.Append(".");
            sb.Append(mi.Name);

            //'-- build method params
            ParameterInfo[] objParameters = sf.GetMethod().GetParameters();
            sb.Append("(");
            intParam = 0;
            foreach (ParameterInfo objParameter in objParameters)
            {
                intParam += 1;
                if (intParam > 1)
                {
                    sb.Append(", ");
                }

                sb.Append(objParameter.Name);
                sb.Append(" As ");
                sb.Append(objParameter.ParameterType.Name);
            }


            sb.Append(")");
            sb.Append(Environment.NewLine);

            //'-- if source code is available, append location info
            sb.Append("       ");
            if (sf.GetFileName() == null || sf.GetFileName().Length == 0)
            {
                sb.Append(Path.GetFileName(ParentAssembly.CodeBase));
                //'-- native code offset is always available
                sb.Append(": N ");
                sb.Append(String.Format("{0:#00000}", sf.GetNativeOffset()));
            }
            else
            {
                sb.Append(Path.GetFileName(sf.GetFileName()));
                sb.Append(": line ");
                sb.Append(String.Format("{0:#0000}", sf.GetFileLineNumber()));
                sb.Append(", col ");
                sb.Append(String.Format("{0:#00}", sf.GetFileColumnNumber()));
                //'-- if IL is available, append IL location info
                if (sf.GetILOffset() != StackFrame.OFFSET_UNKNOWN)
                {
                    sb.Append(", IL ");
                    sb.Append(String.Format("{0:#0000}", sf.GetILOffset()));
                }
            }

            sb.Append(Environment.NewLine);


            return sb.ToString();
        }


        private void Button_Continue_Click(object sender, EventArgs e)
        {
            ForceExit = true;
            this.Close();
        }

        public static bool ForceExit = false;
        private void Button_Stop_Click(object sender, EventArgs e)
        {
           
            try
            {
                ForceExit = true;
                Application.Exit();
            }
            catch (Exception)
            {
            }
            
        }

        private void ExceptionDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private bool SendEmail()
        {
            Cursor = Cursors.WaitCursor;
            this.Button_Continue.Enabled = false;
            this.Button_Email.Enabled = false;
            this.Button_Stop.Enabled = false;

            string specText = "";
            if(spec != null)
            {
                try
                {
                    specText = Environment.NewLine + Environment.NewLine + "Model:" + Environment.NewLine + spec.InputModelText;
                }
                catch(Exception) 
                {
                    
                }                
            }

            bool result = Common.Ultility.Ultility.SendEmail("PAT Exception Report", Environment.NewLine + Environment.NewLine + "Reporter:" + TextBox_Email.Text + Environment.NewLine + Environment.NewLine + "Exception: " + ErrorBox.Text + Environment.NewLine + Environment.NewLine + "How it happend:" + Environment.NewLine + UserInfoBox.Text + Environment.NewLine + Environment.NewLine + DetailBox.Text + specText, Common.Ultility.Ultility.PAT_EMAIL);

            this.Button_Continue.Enabled = true;
            this.Button_Email.Enabled = true;
            this.Button_Stop.Enabled = true;
            Cursor = Cursors.Default;

            return result;
        }

     
        ////        '--
        ////'-- returns build datetime of assembly
        ////'-- assumes default assembly value in AssemblyInfo:
        ////'-- <Assembly: AssemblyVersion("1.0.*")> 
        ////'--
        ////'-- filesystem create time is used, if revision and build were overridden by user
        ////'--
        //private static DateTime AssemblyBuildDate(Assembly objAssembly)
        //{
        //    DateTime dtBuild;
        //    Version objVersion = objAssembly.GetName().Version;

        //    dtBuild = CType("01/01/2000", DateTime).AddDays(objVersion.Build)._
        //    AddSeconds(objVersion.Revision*2)
        //    If
        //    TimeZone.IsDaylightSavingTime(DateTime.Now, TimeZone.CurrentTimeZone.GetDaylightChanges(DateTime.Now.Year))
        //    Then
        //        dtBuild = dtBuild.AddHours(1)
        //    End If
        //    If
        //    dtBuild > DateTime.Now
        //    Or
        //    objVersion.Build < 730
        //    Or
        //    objVersion.Revision = 0
        //    Then
        //        dtBuild = AssemblyFileTime(objAssembly)
        //    End If
        //    return dtBuild;
        //}
    }
}