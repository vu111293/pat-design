using System;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Win32;
using PAT.Common.Ultility;


namespace PAT.GUI.Register
{
    public partial class RegisterationForm : Form
    {
        public RegisterationForm()
        {
            InitializeComponent();
        }

        private void RegisterationForm_Load(object sender, EventArgs e)
        {
            //TextBox_HardwareCode.Text = License.Status.GetHardwareID(true, true, false, false);
            TextBox_HardwareCode.Text = License.Status.HardwareID;

            if (IsValidLicenseAvailable() == Ultility.LicenseType.Valid)
            {
                Label_Register.Text = "Your copy of PAT is registered!";
                this.Button_Purchase.Enabled = false;

                if (License.Status.Evaluation_Lock_Enabled)
                {
                    Label_ValidTime.Text = string.Format("{0} days of {1} days",
                                                         RegisterationForm.CurrentEvaluationDays(),
                                                         RegisterationForm.TotalEvaluationDays());
                }
                else if (License.Status.Expiration_Date_Lock_Enable)
                {
                    Label_ValidTime.Text = License.Status.Expiration_Date.ToShortDateString();
                }
            }

            try
            {
                for (int i = 0; i < License.Status.KeyValueList.Count; i++)
                {
                    string key = License.Status.KeyValueList.GetKey(i).ToString();
                    string value = License.Status.KeyValueList.GetByIndex(i).ToString();

                    if (key == "User")
                    {
                        Label_RegisteredUser.Text = value;
                    }
                    //MessageBox.Show(key + " " + value);
                }
            }
            catch (Exception)
            {
                Label_RegisteredUser.Text = "";
            }
        }

        //To Liuyang: To know whether valid license available, can use the IsValidLicenseAvailable(or License.Status.Licensed) at the chunk listed below.
        //Check if a valid license file is available

        /*** Check if a valid license file is available. ***/
        public static Ultility.LicenseType IsValidLicenseAvailable()
        {
            //Common.Ultility.Ultility.IsValidLicenseAvailable = License.Status.Licensed;
            if (License.Status.Hardware_Lock_Enabled)
            {
                 if (License.Status.HardwareID != License.Status.License_HardwareID)
                 {
                     Common.Ultility.Ultility.IsValidLicenseAvailable = Ultility.LicenseType.Invalid;
                     return Ultility.LicenseType.Invalid; 
                 }
            }

            if (License.Status.Evaluation_Lock_Enabled)
            {
                if (License.Status.Evaluation_Type == License.EvaluationType.Trial_Days)
                {
                    int time = License.Status.Evaluation_Time;
                    int time_current = License.Status.Evaluation_Time_Current;

                    if (time - time_current > 0 && License.Status.Licensed)
                    {
                        Common.Ultility.Ultility.IsValidLicenseAvailable = Ultility.LicenseType.Evaluation;
                        return Ultility.LicenseType.Evaluation;
                    }
                }
            }
            else if (License.Status.Expiration_Date_Lock_Enable)
            {
                System.DateTime expiration_date = License.Status.Expiration_Date;

                if (expiration_date.CompareTo(DateTime.Now) > 0 && License.Status.Licensed)
                {
                    Common.Ultility.Ultility.IsValidLicenseAvailable = Ultility.LicenseType.Valid;
                    return Ultility.LicenseType.Valid;
                }
            }
            else if (License.Status.Licensed)
            {
                Common.Ultility.Ultility.IsValidLicenseAvailable = Ultility.LicenseType.Valid;
                return Ultility.LicenseType.Valid;
            }

            Common.Ultility.Ultility.IsValidLicenseAvailable = Ultility.LicenseType.Invalid;
            return Ultility.LicenseType.Invalid;
        }

        public static int TotalEvaluationDays()
        {
            if (License.Status.Evaluation_Lock_Enabled && License.Status.Evaluation_Type == License.EvaluationType.Trial_Days)
            {
                return License.Status.Evaluation_Time;
            }

            return 0;
        }

        public static int CurrentEvaluationDays()
        {
            if (License.Status.Evaluation_Lock_Enabled && License.Status.Evaluation_Type == License.EvaluationType.Trial_Days)
            {
                return License.Status.Evaluation_Time_Current;
            }
            return 0;
        }

        public static bool ShowEvaluationMessage()
        {
            
            if(MessageBox.Show("You are running an evaluation version of PAT. Do you want to purchase the full version?", "Registration", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
            {
                RegisterationForm form = new RegisterationForm();
                form.ShowDialog();
                return true;
            }
            return false;
        }

        public static bool ShowExpireMessage()
        {
            if (MessageBox.Show("Your version of PAT is expired. Simulation and verification functions in PAT are disabled. Do you want to purchase the full version?", "Registration", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
            {
                RegisterationForm form = new RegisterationForm();
                form.ShowDialog();
                return true;
            }
            return false;
        }

        private void Button_Purchase_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("mailto:pat_info@zipc.com");
            }
            catch (Exception ex)
            {
                Ultility.LogException(ex, null);
            }
            
           
            //string url="http://windows.patroot.com/MemberCenter/LicenseControl.aspx?hid=" + License.Status.HardwareID;
            
            //try
            //{
            //    string defaultBrowserPath = GetDefaultBrowserPath();

            //    // launch default browser
            //    Process.Start(defaultBrowserPath, url);
            //}
            //catch (Exception exp)
            //{
            //    MessageBox.Show(exp.Message);
            //}

        }

        private static string GetDefaultBrowserPath()
        {

            string key = @"htmlfile\shell\open\command";

            RegistryKey registryKey =

            Registry.ClassesRoot.OpenSubKey(key, false);

            // get default browser path

            return ((string)registryKey.GetValue(null, null)).Split('"')[1];

        }
    }
    




//Read additonal license information from a license license

//    /*** Read additonal license information from a license license ***/
//    public void ReadAdditonalLicenseInformation()
//    {
//        /* Check first if a valid license file is found */
//        if (License.Status.Licensed)
//        {
//            /* Read additional license information */
//            for (int i = 0; i < License.Status.KeyValueList.Count; i++)
//            {
//                string key = License.Status.KeyValueList.GetKey(i).ToString();
//                string value = License.Status.KeyValueList.GetByIndex(i).ToString();
//            }
//        }
//    }



//Check the license status of Evaluation Lock

//    /*** Check the license status of Evaluation Lock ***/
//    public void CheckEvaluationLock()
//    {
//        bool lock_enabled = License.Status.Evaluation_Lock_Enabled;
//        EvaluationType ev_type = License.Status.Evaluation_Type;
//        int time = License.Status.Evaluation_Time;
//        int time_current = License.Status.Evaluation_Time_Current;
//    }



//Check the license status of Expiration Date Lock

//    /*** Check the license status of Expiration Date Lock ***/
//    public void CheckExpirationDateLock()
//    {
//        bool lock_enabled = License.Status.Expiration_Date_Lock_Enable;
//        System.DateTime expiration_date = License.Status.Expiration_Date;
//    }



//Check the license status of Number Of Uses Lock

//    /*** Check the license status of Number Of Uses Lock ***/
//    public void CheckNumberOfUsesLock()
//    {
//        bool lock_enabled = License.Status.Number_Of_Uses_Lock_Enable;
//        int max_uses = License.Status.Number_Of_Uses;
//        int current_uses = License.Status.Number_Of_Uses_Current;
//    }



//Check the license status of Number Of Instances Lock

//    /*** Check the license status of Number Of Instances Lock ***/
//    public void CheckNumberOfInstancesLock()
//    {
//        bool lock_enabled = License.Status.Number_Of_Instances_Lock_Enable;
//        int max_instances = License.Status.Number_Of_Instances;
//    }



//Check the license status of Hardware Lock

//    /*** Check the license status of Hardware Lock ***/
//    public void CheckHardwareLock()
//    {
//        bool lock_enabled = License.Status.Hardware_Lock_Enabled;

//        if (lock_enabled)
//        {
//            /* Get Hardware ID which is stored inside the license file */
//            string lic_hardware_id  = License.Status.License_HardwareID;
//        }
//    }



//Get Hardware ID of the current machine

//    /*** Get Hardware ID of the current machine ***/
//    public string GetHardwareID()
//    {
//        return License.Status.HardwareID;
//    }



//Compare current Hardware ID with Hardware ID stored in License File

//    /*** Compare current Hardware ID with Hardware ID stored in License File ***/
//    public bool CompareHardwareID()
//    {
//        if (License.Status.HardwareID == License.Status.License_HardwareID)
//            return true;
//        else
//            return false;
//    }



//Invalidate the license

//    /*** Invalidate the license. Please note, your protected software does not accept a license file anymore! ***/
//    public void InvalidateLicense()
//    {
//        string confirmation_code = License.Status.InvalidateLicense();
//    }



//Check if a confirmation code is valid

//    /*** Check if a confirmation code is valid ***/
//    public bool CheckConfirmationCode(string confirmation_code)
//    {
//        return License.Status.CheckConfirmationCode(License.Status.HardwareID,
//        confirmation_code);
//    }



//Reactivate the license

//    /*** Reactivate an invalidated license. ***/
//    public bool ReactivateLicense(string reactivation_code)
//    {
//        return License.Status.ReactivateLicense(reactivation_code);
//    }



//Manually load a license using a filename

//    /*** Load the license. ***/
//    public void LoadLicense(string filename)
//    {
//        License.Status.LoadLicense(filename);
//    }



//Manually load a license using byte[]

//    /*** Load the license. ***/
//    public void LoadLicense(byte[] license)
//    {
//        License.Status.LoadLicense(license);
//    }



//Get loaded license (if available) as byte[]

//    /*** Get the license. ***/
//    public byte[] GetLicense()
//    {
//        return License.Status.License;
//    }

  		 
}
