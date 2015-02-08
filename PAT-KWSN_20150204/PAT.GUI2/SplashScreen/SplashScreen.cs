using System;

namespace PAT.GUI.SplashScreen
{
    /// <summary>
    /// Define the types of message
    /// including "Success/Warning/Error"
    /// </summary>
    public enum TypeOfMessage
    {
        Success,
        Warning,
        Error,
    }

    /// <summary>
    /// @author Ma Junwei
    /// Used to control the splash screen
    /// </summary>
    public static class SplashScreen
    {
        static SplashScreenForm _sf = null;

        /// <summary>
        /// Displays the splashscreen
        /// </summary>
        public static void ShowSplashScreen()
        {
            try
            {
                if (_sf == null)
                {
                    _sf = new SplashScreenForm();
                    _sf.ShowSplashScreen();
                }
            }
            catch (Exception)
            {                
            }            
        }

        /// <summary>
        /// Closes the SplashScreen
        /// </summary>
        public static void CloseSplashScreen()
        {
            try
            {
                if (_sf != null)
                {
                    _sf.CloseSplashScreen();
                    _sf = null;
                }
            }
            catch (Exception)
            {
                
            }
        }

        /// <summary>
        /// Update text in default green color of success message
        /// </summary>
        /// <param name="text">Message</param>
        public static void UpdateStatusText(string text)
        {
            try
            {
                if (_sf != null)
                    _sf.UdpateStatusText(text);
            }
            catch (Exception)
            {
                
            }
        }

        /// <summary>
        /// Update text with message color defined as green/yellow/red/ for success/warning/failure
        /// </summary>
        /// <param name="text">Message</param>
        /// <param name="tom">Type of Message</param>
        public static void UpdateStatusTextWithStatus(string text, TypeOfMessage tom)
        {
            try
            {
                if (_sf != null)
                    _sf.UdpateStatusTextWithStatus(text, tom);
            }
            catch (Exception)
            {                
            }            
        }
    }
}
