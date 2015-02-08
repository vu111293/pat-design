using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Globalization;
using System.Reflection;
using Microsoft.Office.Interop.Excel;

/**
 * Format the output to excel file 
 * @author Ma Junwei
 */

namespace PAT.GUI.Forms.GenerateModule
{
    public class Format
    {
        // the titles of the columns
        private String[] _TITLES = new String[] { "ModelName", "Assertion", "#Process", "Property", "Result", "#Clocks", "#States(K)", "Time(s)", "Time_zeno(s)", "Probability(Min)", "Probability(Max)" };

        // to restore the path of input file in the disk
        private String _path;

        // to restore the style of the file,whether it is a zeno file
        private bool _isZeno;

        // to restore the output file contents
        private List<string> _models = new List<string>();
        private List<String> _assertions = new List<String>();
        private List<String> _processes = new List<String>();
        private List<String> _properties = new List<String>();
        private List<String> _results = new List<String>();
        private List<Int32> _clocks = new List<Int32>();
        private List<Double> _states = new List<Double>();
        private List<String> _time = new List<String>();
        private List<String> _timeZeno = new List<String>();
        private List<Double> _pmax = new List<Double>();
        private List<Double> _pmin = new List<Double>();

        // Constructor
        public Format(String path)
        {
            _path = path;
        }

        /**
         * read the content of the file by line
         */
        public void ReadFromDisk()
        {
            if ( string.IsNullOrEmpty(_path))
            {
                throw new Exception("Please select the file to generate first!");
            }

            // whether the file is a zeno file
            _isZeno = _path.Contains("zeno") ? true : false;

            try
            {
                // start to read from the disk
                StreamReader reader = new StreamReader(_path);

                // locate the current line the reader
                String line;

                // make sure that there wouldn't be BLANK LINES in the file
                while ((line = reader.ReadLine()) != null)
                {
                    // 1. get the model name and the #process
                    // if a line contents ".rts" or ".ta" or ".pcsp" or ".csp" or "prts", parse it to modelName
                    if ((line.Contains(".rts") || line.Contains(".ta") || line.Contains(".pcsp") || line.Contains(".csp") || line.Contains(".prts")))
                    {
                        //get the modelName,exclude the path
                        String modelName = GetFileName(line);

                        // the number of process is just after the modelName,such as csmacd-5.rts,then we get 5 as the #process
                        // '-' and '_' are permitted here for marking #process,other chars are illegal 
                        if (modelName.Contains('-'))
                        {
                            _models.Add(modelName.Split('-')[0].Trim());
                            _processes.Add(modelName.Split('-')[1].Trim());
                        }
                        else if (modelName.Contains('_'))
                        {
                            _models.Add(modelName.Split('_')[0].Trim());
                            _processes.Add(modelName.Split('_')[1].Trim());
                        }
                        else
                        {
                            _models.Add(modelName);
                            _processes.Add("No.");
                        }
                        continue;
                    }

                    // 2. get the assertions
                    // if a line contents "Assertions:", parse it to assertion name
                    if (line.Contains("Assertion:"))
                    {
                        // before adding the assertion,  excute the Repair function
                        Repair(false);
                        // parse the contents after "Assertion:" to assertion name
                        _assertions.Add(line.Split(':')[1].Trim());
                        continue;
                    }

                    // 3. get the result and the properties
                    // if the line contains the contents as following,then parse the line as result
                    if (line.Contains("VALID")||line.Contains("NOT valid")||line.Contains("is Valid with"))
                    {
                        //  get the property:1.deadlockfree 2.reaches 3.LTL
                        // if the line contains the contents as following,then parse the line as the property
                        if (line.Contains("deadlockfree"))
                        {
                            _properties.Add("DeadLock Freeness");
                        }
                        else if (line.Contains("reaches"))
                        {
                            _properties.Add("reachability");
                        }
                        else if (line.Contains("|="))
                        {
                            _properties.Add("LTL");
                        }
                        else if (line.Contains("refines"))
                        {
                            _properties.Add("refinement");
                        }

                        //  get the result
                        if (line.Contains("is VALID"))
                        {
                            _results.Add("YES");
                            _pmax.Add(1);
                            _pmin.Add(1);
                        }
                        else if (line.Contains("NOT valid"))
                        {
                            _results.Add("NO");
                            _pmax.Add(0);
                            _pmin.Add(0);
                        }
                        //for Probability checking
                        //if an assertion is probabilistic, then parse to probability
                        else
                        {
                            _results.Add("Probability");
                            //add pmin
                           _pmin.Add(Double.Parse(getPmin(line)));
                            //add pmax
                            _pmax.Add(Double.Parse(getPmax(line)));
                        }

                        continue;
                    }

                    
                    // 4. get the clocks
                    // if "Clocks" contains,then parse to clocks
                    if (line.Contains("Clocks"))
                    {
                        _clocks.Add(Int32.Parse(line.Split(':')[1].Trim()));
                        continue;
                    }

                    // 5. get the states
                    // if "Visited States:" contains,then parse to states
                    if (line.Contains("Visited States:"))
                    {
                        double statesNum = Double.Parse(line.Split(':')[1].Trim()) / 1000;
                        // keep all numbers after the decimal
                        _states.Add(statesNum);
                        continue;
                    }

                    // 6. get the  time
                    // if "Time used" contains,then parse to time
                    // iff the name of file contains "zeno",then parse to time_zeno,else parse to time
                    if (line.Contains("Time Used"))
                    {
                        // replace the last letter of time 's' to number 0
                        // keep 2 numbers after the decimal
                        string time = line.Split(':')[1].TrimEnd(new [] {' ', 's'});
                        if (!_isZeno)
                        {
                            _time.Add(time);
                            _timeZeno.Add("0");
                        }
                        else
                        {
                            _timeZeno.Add(time);
                            _time.Add("0");
                        }
                        continue;
                    }
                }

                // repair after all lines are read
                Repair(true);
            }
            catch (Exception e)
            {
                throw new Exception("Exception Occur when reading the text file,reason:"+e);
            }
        }
         
        // write the contents into excel file
        // the file name is just like the input file name exclude the type
        public void WriteToExcel()
        {

            //set the save path of excel file
            String fileName =  GetFilePath(_path) + GetFileName(_path)+".xlsx";

            //set the excel property
            try
            {
                Application _excel = new ApplicationClass();
                _excel.UserControl = true;

                // the language version of the excel program in Windows,must set the culture
                System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US"); 
                Workbook _workBook = _excel.Workbooks.Add(Type.Missing);
                Worksheet _workSheet = (Worksheet)_workBook.ActiveSheet;

                // range for title
                Range titleRange = _workSheet.get_Range("A1", "K1");

                //set the titles in the excel
                titleRange.Value2 = _TITLES;
                titleRange.Columns.AutoFit();
                titleRange.Font.Bold = true;
                titleRange.Font.Color = 5;
                titleRange.Interior.ColorIndex = 41;
                titleRange.Borders.LineStyle = 1;

                // range for the elements
                Range elementsRange = _workSheet.get_Range("A2", "K" +(_assertions.Count+1));
                elementsRange.Interior.ColorIndex = 37;
                elementsRange.Borders.LineStyle = 1;

                for(int j = 1;j<=_assertions.Count;j++)
                {
                    _workSheet.Cells[j + 1, 1] = _models[j - 1];
                    _workSheet.Cells[j + 1, 2] = _assertions[j - 1];
                    _workSheet.Cells[j + 1, 3] = _processes[j - 1];
                    _workSheet.Cells[j + 1, 4] = _properties[j - 1];
                    _workSheet.Cells[j + 1, 5] = _results[j - 1];
                    _workSheet.Cells[j + 1, 6] = _clocks[j - 1];
                    _workSheet.Cells[j + 1, 7] = _states[j - 1];
                    _workSheet.Cells[j + 1, 8] = _time[j - 1];
                    _workSheet.Cells[j + 1, 9] = _timeZeno[j - 1];
                    _workSheet.Cells[j + 1, 10] = _pmin[j - 1];
                    _workSheet.Cells[j + 1, 11] = _pmax[j - 1];
                }

                object missing = Missing.Value;
                // ban the save and override alerts
                _excel.DisplayAlerts = false;
                _excel.AlertBeforeOverwriting = false;
                _workSheet.SaveAs(fileName, missing, missing, missing, missing, missing, missing, missing, missing, missing);
                _excel.Workbooks.Close();
                _excel.Quit();
                System.GC.Collect();
            }catch(Exception e)
            {
                throw new Exception("Error occur when generate the excel file, reason: " + e);
            }
        }

        // parse the file path or the line content and return the modelName
        private String GetFileName(String line)
        {
            int start = line.LastIndexOf("\\");
            int end = line.LastIndexOf(".");
            return line.Substring(start + 1, end - start - 1);
        }

        // parse the file path(include the file name),and return the file path(exclude the file name)
        private String GetFilePath(String line)
        {
            int end = line.LastIndexOf("\\");
            return line.Substring(0,end+1);
        }

        //parse Pmax to the excel 
        private string getPmax(String line)
        {
            if(line.Contains("is Valid with Probability"))//property with prob
            {
                int start = line.LastIndexOf(" ");
                int end = line.LastIndexOf("]");
                return line.Substring(start + 1, end - start - 1).Trim();
            }
            
            if(line.Contains("is Valid with Max"))//property with pmax

            {
                int start = line.LastIndexOf(" ");
                int end = line.LastIndexOf(";");
                return line.Substring(start + 1, end - start - 1).Trim();
            }

            return "-1";
        }

        //parse Pmin to the excel
        private string getPmin(String line)
        {
            if (line.Contains("is Valid with Probability"))//property with prob
            {
                int start = line.LastIndexOf("[");
                int end = line.LastIndexOf(",");
                return line.Substring(start + 1, end - start - 1).Trim();
            }

            if (line.Contains("is Valid with Min"))//property with pmin
            {
                int start = line.LastIndexOf(" ");
                int end = line.LastIndexOf(";");
                return line.Substring(start + 1, end - start - 1).Trim();
            }

            return "-1";
        }

        

        // Repair the input record when some of the elements are lost
        // use the Assertion Name as the base
        private void Repair(Boolean isLastRepair)
        {
            if (!isLastRepair)
            {
                // repair the modelName and Processes iff the reader doesn't come to the end 
                // when a model contents muti assertions, and if the additional assertion is added,then add the latest model name and the process
                if (_assertions.Count != _models.Count - 1)
                {
                    //if the models list is empty,that means their is no modelName parsing in the .txt file, we parse the fileName as the modelName instead of the line content
                    if (_models.Count == 0)
                    {
                        String fileName = GetFileName(_path);
                        if (fileName.Contains('-'))
                        {
                            _models.Add(fileName.Split('-')[0]);
                            _processes.Add(fileName.Split('-')[1]);
                        }
                        else if (fileName.Contains('_'))
                        {
                            _models.Add(fileName.Split('_')[0]);
                            _processes.Add(fileName.Split('_')[1]);
                        }
                        else
                        {
                            _models.Add(fileName);
                            _processes.Add("No.");
                        }

                    }
                    else
                    {
                        _models.Add(_models[_models.Count - 1]);
                        _processes.Add(_processes[_processes.Count - 1]);
                    }
                }
            }

            // repair the properties and results
            // when lost the properties and result,then add "N/A" as default
            // property and result are parsing from one line, so their counts must be equal
            if(_assertions.Count != _properties.Count && _assertions.Count!= _results.Count){
                _properties.Add("No.");
                _results.Add("No.");
            }

            // repair the clocks
            // add 0 as default
            if (_assertions.Count != _clocks.Count)
            {
                _clocks.Add(0);
            }

            // repair the states
            // add 0 as default
            if(_assertions.Count != _states.Count)
            {
                _states.Add(0);
            }
            // repair the time and time_zeno
            // add 0 as default
            // time and timeZeno are parsing from one line,so their counts must be equal
            if (_assertions.Count != _time.Count && _assertions.Count != _timeZeno.Count) 
            {
                _time.Add("0");
                _timeZeno.Add("0");
            }
        }
    }
}