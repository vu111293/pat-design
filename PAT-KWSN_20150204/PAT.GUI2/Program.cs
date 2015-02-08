using System;
using System.Threading;
using System.Windows.Forms;

namespace PAT.GUI
{
    
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] files)
        {
            Application.SetCompatibleTextRenderingDefault(false);
            Application.EnableVisualStyles();
            Application.DoEvents(); 

            //if (Common.Ultility.Ultility.IsWindowsOS)
            //{
            //    // start the splash thread at the beginning
            //    var splashThread = new Thread(SplashScreen.SplashScreen.ShowSplashScreen) {IsBackground = true};
            //    splashThread.Start();
            //}

            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);

            try
            {
                GUIUltility.ReadSettingValue();
            }
            catch (Exception) { }

            FormMain mainForm; 
            if (Common.Ultility.Ultility.IsWindowsOS)
            {
                //// tell the user what we do here
                //SplashScreen.SplashScreen.UpdateStatusText("Update checked");

                mainForm = new FormMain();

                //// after load the main form, close the splashscreen
                //SplashScreen.SplashScreen.CloseSplashScreen();
            }
            else
            {
                mainForm = new FormMain();
            }

            try
            {
                if (files.Length > 0)
                    mainForm.OpenFile(files[0], false);
                else
                    mainForm.OpenFileLastFile();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Invalid File on arguments:" + ex.Message + ex.StackTrace);
            }

            mainForm.StartCheckUpdate();

            Application.Run(mainForm);
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            MessageBox.Show("Unhandled Exception: " + e.Exception.Message + "\r\n" + e.Exception.StackTrace);
        }
    }
}