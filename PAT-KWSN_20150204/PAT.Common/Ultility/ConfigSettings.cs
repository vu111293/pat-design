using System;
using System.Data;
using System.IO;

namespace PAT.Common.Ultility
{
    public class ConfigSettings
    {
        public static ConfigSettings Settings;
        /// 
        /// This DataSet is used as a memory data structure to hold config
        /// section/key/value pairs Inside this DataSet, a DataTable for each
        /// section is created
        /// 
        private DataSet m_dsOptions;
        // This is the filename for the DataSet XML serialization
        private string m_szConfigFileName;

        /// 
        /// This property is read-only, because it is set through Initialize or
        /// Store methods
        /// 
        public string szConfigFileName
        {
            get
            {
                return m_szConfigFileName;
            }
        }

        /// 
        /// This method has to be invoked before using any other method of
        /// ConfigOpt class ConfigFile parameter is the name of the config file
        /// to be read. If the config file does not exist, and the user calls
        /// "SetOptions", the config file will be created when "Store" is
        /// called.
        /// 
        /// 
        public void Initialize(string szConfigFile)
        {
            this.m_szConfigFileName = szConfigFile;
            this.m_dsOptions = new DataSet("ConfigOpt");
            if (File.Exists(szConfigFile))
            {
                // If the specified config file exists, it is read to populate
                // the DataSet
                this.m_dsOptions.ReadXml(szConfigFile);
            }
        }

        /// 
        /// This method serializes the memory data structure holding the config
        /// parameters The filename used is the one defined calling Initialize
        /// method
        /// 
        public bool Store()
        {
            try
            {
                this.Store(this.m_szConfigFileName);
                return true;
            }
            catch (Exception)
            {

            }

            return false;
        }

        /// 
        /// Same as Store() method, but with the ability to serialize on 
        /// different filename
        /// 
        /// 
        public void Store(string szConfigFile)
        {
            this.m_szConfigFileName = szConfigFile;
            this.m_dsOptions.WriteXml(szConfigFile);
        }

        /// 
        /// Read a configuration Value, given its name and section name
        /// (szOptionName). If the Key is not defined, the default value is
        /// returned, and the entry is added to the data set.
        /// 
        /// 
        /// 
        /// 
        /// string
        public string GetOption(string szSectionName, string szOptionName,
                                string szDefault)
        {
            bool bAddOption = true;
            string szReturn = szDefault;
            if (this.m_dsOptions.Tables[szSectionName] != null)
            {
                DataView dv = m_dsOptions.Tables[szSectionName].DefaultView;
                dv.RowFilter = "OptionName='" + szOptionName + "'";
                if (dv.Count > 0)
                {
                    szReturn = dv[0]["OptionValue"].ToString();
                    bAddOption = false;
                }
            }

            if (bAddOption == true)
            {
                this.SetOption(szSectionName, szOptionName, szDefault);
            }

            return szReturn;
        }

        /// 
        /// Overload for getting integer values
        /// 
        /// 
        /// 
        /// 
        /// int
        public int GetOption(string szSectionName, string szOptionName,
                             int iDefault)
        {
            return int.Parse(this.GetOption(szSectionName, szOptionName,
                                            iDefault.ToString()));
        }

        /// 
        /// Overload for getting double values
        /// 
        /// 
        /// 
        /// 
        /// 
        public double GetOption(string szSectionName, string szOptionName,
                                double dDefault)
        {
            return double.Parse(this.GetOption(szSectionName, szOptionName,
                                               dDefault.ToString()));
        }

        /// 
        /// Write in the memory data structure a Key/Value pair for a
        /// configuration setting. If the Key already exists, the Value is
        /// simply updated, else the Key/Value pair is added. Warning: to update
        /// the written Key/Value pair on the config file, you need to call
        /// Store
        /// 
        /// 
        /// 
        /// 
        public void SetOption(string szSectionName, string szOptionName,
                              string szOptionValue)
        {
            if (this.m_dsOptions.Tables[szSectionName] == null)
            {
                DataTable dt = new DataTable(szSectionName);
                dt.Columns.Add("OptionName", Type.GetType("System.String"));
                dt.Columns.Add("OptionValue", Type.GetType("System.String"));
                // dt.Columns.Add("OptionType", 
                this.m_dsOptions.Tables.Add(dt);
            }

            DataView dv = this.m_dsOptions.Tables[szSectionName].DefaultView;
            dv.RowFilter = "OptionName='" + szOptionName + "'";
            if (dv.Count > 0)
            {
                dv[0]["OptionValue"] = szOptionValue;
            }
            else
            {
                DataRow dr = m_dsOptions.Tables[szSectionName].NewRow();
                dr["OptionName"] = szOptionName;
                dr["OptionValue"] = szOptionValue;
                this.m_dsOptions.Tables[szSectionName].Rows.Add(dr);
            }
        }

        /// 
        /// Overload of SetOption that takes an integer option value
        /// 
        /// 
        /// 
        /// 
        public void SetOption(string szSectionName, string szOptionName,
                              int iOptionValue)
        {
            this.SetOption(szSectionName, szOptionName, iOptionValue.
                                                            ToString());
        }

        /// 
        /// Overload of SetOption that takes a double option value
        /// 
        /// 
        /// 
        /// 
        public void SetOption(string szSectionName, string szOptionName,
                              double dOptionValue)
        {
            this.SetOption(szSectionName, szOptionName, dOptionValue.
                                                            ToString());
        }
    }
}