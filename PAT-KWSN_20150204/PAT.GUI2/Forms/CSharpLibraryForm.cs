using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Gui;
using Microsoft.CSharp;
using PAT.Common.Ultility;
using PAT.GUI.Docking;
using PAT.GUI.Properties;


namespace PAT.GUI.Forms
{
    public partial class CSharpLibraryForm : Form
    {
        private const string MathClassFile = "PAT.Lib.Example.cs";
        private string FileName = null;

        EditorTabItem TextBox_Program = new EditorTabItem("C#");

        public CSharpLibraryForm() : this (Path.Combine(Ultility.LibFolderPath, MathClassFile))
        {
            //InitializeComponent();

            //TextBox_Program.HideGoToDeclarition();

            //this.TextBox_Code.Visible = false;
            //TextBox_Program.Dock = DockStyle.Fill;
            //this.toolStripContainer1.ContentPanel.Controls.Add(TextBox_Program.CodeEditor);


            //FileName = Path.Combine(Ultility.LibFolderPath, MathClassFile);
            //if (File.Exists(FileName))
            //{
            //    StreamReader tr = new StreamReader(FileName);
            //    this.TextBox_Program.Text = tr.ReadToEnd();
            //    tr.Close();
            //    this.StatusLabel_FileName.Text = FileName;
            //}
            //else
            //{
            //    FileName = null; 
            //}

            //MenuStrip menuStrip1 = new MenuStrip();
            //ToolStripMenuItem button = new ToolStripMenuItem();
            //button.Size = new Size(0, 0);
            //button.ShortcutKeys = (Keys)Shortcut.CtrlS;
            //button.Click += new EventHandler(this.Button_Save_Click);
            //menuStrip1.Items.AddRange(new ToolStripItem[] { button });
            //button = new ToolStripMenuItem();
            //button.Size = new Size(0, 0);
            //button.ShortcutKeys = (Keys)Shortcut.CtrlO;
            //button.Click += new EventHandler(this.Button_Load_Click);
            //menuStrip1.Items.AddRange(new ToolStripItem[] { button });
            //button = new ToolStripMenuItem();
            //button.Size = new Size(0, 0);
            //button.ShortcutKeys = (Keys)Shortcut.CtrlB;
            //button.Click += new EventHandler(this.Button_build_Click);
            //menuStrip1.Items.AddRange(new ToolStripItem[] { button });
            //menuStrip1.Visible = false;
            //Controls.Add(menuStrip1);
            ////MainMenuStrip = menuStrip1;

            //Button_BuildMode.ComboBox.SelectedIndex = 0;

            //TextBox_Program.CodeEditor.TextChanged += new EventHandler(CodeEditor_TextChanged);

        }

        public CSharpLibraryForm(string FileName)
        {
            InitializeComponent();

            TextBox_Program.HideGoToDeclarition();

            this.TextBox_Code.Visible = false;
            TextBox_Program.Dock = DockStyle.Fill;
            this.toolStripContainer1.ContentPanel.Controls.Add(TextBox_Program.CodeEditor);


            //FileName = Path.Combine(Ultility.LibFolderPath, MathClassFile);
            if (File.Exists(FileName))
            {
                StreamReader tr = new StreamReader(FileName);
                this.TextBox_Program.Text = tr.ReadToEnd();
                tr.Close();
                this.StatusLabel_FileName.Text = FileName;
            }
            else
            {
                FileName = null;
            }

            MenuStrip menuStrip1 = new MenuStrip();
            ToolStripMenuItem button = new ToolStripMenuItem();
            button.Size = new Size(0, 0);
            button.ShortcutKeys = (Keys)Shortcut.CtrlS;
            button.Click += new EventHandler(this.Button_Save_Click);
            menuStrip1.Items.AddRange(new ToolStripItem[] { button });
            button = new ToolStripMenuItem();
            button.Size = new Size(0, 0);
            button.ShortcutKeys = (Keys)Shortcut.CtrlO;
            button.Click += new EventHandler(this.Button_Load_Click);
            menuStrip1.Items.AddRange(new ToolStripItem[] { button });
            button = new ToolStripMenuItem();
            button.Size = new Size(0, 0);
            button.ShortcutKeys = (Keys)Shortcut.CtrlB;
            button.Click += new EventHandler(this.Button_build_Click);
            menuStrip1.Items.AddRange(new ToolStripItem[] { button });
            menuStrip1.Visible = false;
            Controls.Add(menuStrip1);
            //MainMenuStrip = menuStrip1;

            Button_BuildMode.ComboBox.SelectedIndex = 0;

            TextBox_Program.CodeEditor.TextChanged += new EventHandler(CodeEditor_TextChanged);

        }

        void CodeEditor_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(StatusLabel_FileName.Text) && !StatusLabel_FileName.Text.EndsWith("*"))
            {
                StatusLabel_FileName.Text += "*"; 
            }
        }

        private void Button_New_Click(object sender, EventArgs e)
        {
            FileName = null;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Text;");

            sb.AppendLine("//the namespace must be PAT.Lib, the class and method names can be arbitrary");
            sb.AppendLine("namespace PAT.Lib");
            sb.AppendLine("{");
            sb.AppendLine("    /// <summary>");
            sb.AppendLine("    /// The math library that can be used in your model.");
            sb.AppendLine("    /// all methods should be declared as public static.");
            sb.AppendLine("    /// ");
            sb.AppendLine("    /// The parameters must be of type \"int\", or \"int array\"");
            sb.AppendLine("    /// The number of parameters can be 0 or many");
            sb.AppendLine("    /// ");
            sb.AppendLine("    /// The return type can be bool, int or int[] only.");
            sb.AppendLine("    /// ");
            sb.AppendLine("    /// The method name will be used directly in your model.");
            sb.AppendLine("    /// e.g. call(max, 10, 2), call(dominate, 3, 2), call(amax, [1,3,5]),");
            sb.AppendLine("    /// ");
            sb.AppendLine("    /// Note: method names are case sensetive");
            sb.AppendLine("    /// </summary>");
            sb.AppendLine("    public class NewLib");
            sb.AppendLine("    {");
            sb.AppendLine("	       public static bool dummy(int[] values)");
            sb.AppendLine("        {");
            sb.AppendLine("		        return true;");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            this.TextBox_Program.Text = sb.ToString();
            this.StatusLabel_FileName.Text = "";
            this.StatusLabel_Status.Text = "";

            this.StatusLabel_Status.Text = "Ready";

        }

        private void Button_Datatype_Click(object sender, EventArgs e)
        {
            FileName = null;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Text;");
            sb.AppendLine("using PAT.Common.Classes.Expressions.ExpressionClass;");
            sb.AppendLine();
            sb.AppendLine("//the namespace must be PAT.Lib, the class and method names can be arbitrary");
            sb.AppendLine("namespace PAT.Lib");
            sb.AppendLine("{");
            sb.AppendLine("    /// <summary>");
            sb.AppendLine("    /// The math library that can be used in your model.");
            sb.AppendLine("    /// all methods should be declared as public static.");
            sb.AppendLine("    /// ");
            sb.AppendLine("    /// The parameters must be of type \"int\", or \"int array\"");
            sb.AppendLine("    /// The number of parameters can be 0 or many");
            sb.AppendLine("    /// ");
            sb.AppendLine("    /// The return type can be bool, int or int[] only.");
            sb.AppendLine("    /// ");
            sb.AppendLine("    /// The method name will be used directly in your model.");
            sb.AppendLine("    /// e.g. call(max, 10, 2), call(dominate, 3, 2), call(amax, [1,3,5]),");
            sb.AppendLine("    /// ");
            sb.AppendLine("    /// Note: method names are case sensetive");
            sb.AppendLine("    /// </summary>");
            sb.AppendLine("    public class NewDataType : ExpressionValue");
            sb.AppendLine("    {");
            sb.AppendLine("");
            sb.AppendLine("        /// <summary>");
            sb.AppendLine("        /// Please implement this method to provide the string representation of the datatype");
            sb.AppendLine("        /// </summary>");
            sb.AppendLine("        /// <returns></returns>");
            sb.AppendLine("        public override string ToString()");
            sb.AppendLine("        {");
            sb.AppendLine("            return \"\";");
            sb.AppendLine("        }");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("        /// <summary>");
            sb.AppendLine("        /// Please implement this method to return a deep clone of the current object");
            sb.AppendLine("        /// </summary>");
            sb.AppendLine("        /// <returns></returns>");
            sb.AppendLine("        public override ExpressionValue GetClone()");
            sb.AppendLine("        {");
            sb.AppendLine("            return this;");
            sb.AppendLine("        }");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("        /// <summary>");
            sb.AppendLine("        /// Please implement this method to provide the compact string representation of the datatype");
            sb.AppendLine("        /// </summary>");
            sb.AppendLine("        /// <returns></returns>");
            sb.AppendLine("        public override string ExpressionID");
            sb.AppendLine("        {");
            sb.AppendLine("            get {return \"\"; }");
            sb.AppendLine("        }");
            sb.AppendLine("");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            this.TextBox_Program.Text = sb.ToString();
            this.StatusLabel_FileName.Text = "";
            this.StatusLabel_Status.Text = "Ready";

        }


        private void Button_Load_Click(object sender, EventArgs e)
        {
            OpenFileDialog opf = new OpenFileDialog();
            opf.Filter = "Visual C# Files (*.cs)|*.cs|All Files (*.*)|*.*";                       
            opf.InitialDirectory = GetOpenFolderDirectory();
            
            
            if (opf.ShowDialog() == DialogResult.OK)
            {
                FileName = opf.FileName;
                if (File.Exists(FileName))
                {
                    StreamReader tr = new StreamReader(FileName);
                    this.TextBox_Program.Text = tr.ReadToEnd();
                    tr.Close();
                    this.StatusLabel_FileName.Text = FileName;
                    this.StatusLabel_Status.Text = "Ready";

                }
                else
                {
                    FileName = null;
                }
            }
        }

        private void Button_Save_Click(object sender, EventArgs e)
        {
            try
            {

                if (FileName == null)
                {
                    SaveFileDialog svd = new SaveFileDialog();

                    svd.InitialDirectory = GetOpenFolderDirectory();
                    
                    svd.DefaultExt = ".cs";
                    svd.Filter = "Visual C# Files (*.cs)|*.cs|All Files (*.*)|*.*";

                    if (svd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        FileName = svd.FileName;
                    }
                    else
                    {
                        return;
                    }
                }

                StreamWriter sw = new StreamWriter(FileName);
                sw.Write(this.TextBox_Program.Text);
                sw.Close();
                this.StatusLabel_FileName.Text = FileName;

                this.StatusLabel_Status.Text = "File Saved";

                //MessageBox.Show(Resources.C__library_code_is_saved_successfully_, Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Information);


            }
            catch (Exception ex)
            {
                MessageBox.Show(Resources.Exception_happened_during_saving__ + ex.Message + "\r\n" + ex.StackTrace, Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private static string GetOpenFolderDirectory()
        {
            try
            {
                if (WorkbenchSingleton.DockContainer.ActiveDocument != null &&
                    WorkbenchSingleton.DockContainer.ActiveDocument is EditorTabItem)
                {
                    if ((WorkbenchSingleton.DockContainer.ActiveDocument as EditorTabItem).Text.IndexOf("#import") != -1)
                    {
                        if ((WorkbenchSingleton.DockContainer.ActiveDocument as EditorTabItem).Text.IndexOf("#import \"PAT.Lib.") == -1)
                        {
                            string fileName =
                                (WorkbenchSingleton.DockContainer.ActiveDocument as EditorTabItem).FileName;
                            if (File.Exists(fileName))
                            {
                                return Path.GetDirectoryName(fileName);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                
            }

            return Ultility.LibFolderPath;
        }

        private void Button_build_Click(object sender, EventArgs e)
        {
            try
            {
                if (FileName != null)
                {
                    StreamWriter sw = new StreamWriter(FileName);
                    sw.Write(this.TextBox_Program.Text);
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(Resources.Exception_happened_during_saving__ + ex.Message + "\r\n" + ex.StackTrace, Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            SaveFileDialog svd = new SaveFileDialog();            
            svd.InitialDirectory = GetOpenFolderDirectory();

            svd.DefaultExt = ".dll";
            svd.Filter = "PAT Library Files (*.dll)|*.dll";
            svd.FileName = Path.GetFileNameWithoutExtension(this.StatusLabel_FileName.Text);

            if (svd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string dllName = svd.FileName;

                    //delete the dll if exist
                    if (File.Exists(svd.FileName))
                    {
                        File.Delete(svd.FileName);
                    }

                    if (Button_BuildMode.ComboBox.Text == "Contracts")
                    {
                        dllName = dllName + ".temp";
                    }

                    CompilerParameters parameters;
                    CompilerResults results;

                    parameters = new CompilerParameters();
                    parameters.OutputAssembly = dllName;
                    parameters.GenerateExecutable = false;
                    parameters.GenerateInMemory = false;

                    
                    if (Button_BuildMode.ComboBox.Text == "Release")
                    {
                        parameters.IncludeDebugInformation = false;
                    }
                    else
                    {
                        parameters.IncludeDebugInformation = true;
                    }

                    parameters.TreatWarningsAsErrors = false;
                    parameters.WarningLevel = 4;
                    



                    // Optimize output, and make a windows executable (no console)
                    parameters.CompilerOptions = "/optimize /target:library";

                    // Put any references you need here - even your own dll's, if you want to use them
                    List<string> refs = new List<string>()
                    {
                        "System.dll",
                        "System.Core.dll",
                        "System.Drawing.dll",
                        "System.Windows.Forms.dll",
                        Path.Combine(Common.Ultility.Ultility.APPLICATION_PATH, "PAT.Common.dll"),
                        //Application.StartupPath + "\\PAT.Module.CSP.dll"
                    };

                    //add additional DLL files need for the compilation
                    refs.AddRange(Directory.GetFiles(Path.Combine(Common.Ultility.Ultility.APPLICATION_PATH, "ExtDLL"), "*.dll"));

                    if (Button_BuildMode.ComboBox.Text == "Contracts")
                    {
                        refs.Add(Path.Combine(Ultility.LibFolderPath, "Microsoft.Contracts.dll"));
                    }

                    // Add the assemblies that we will reference to our compiler parameters
                    parameters.ReferencedAssemblies.AddRange(refs.ToArray());

                    Dictionary<string, string> provOptions = new Dictionary<string, string>();

                    provOptions.Add("CompilerVersion", "v3.5");

                    // Get a code provider for C#
                    CSharpCodeProvider provider = new CSharpCodeProvider(provOptions);


                    // Read the entire contents of the file
                    string sourceCode = this.TextBox_Program.Text;


                    //Add the contracts line 
                    if (Button_BuildMode.ComboBox.Text == "Contracts")
                    {
                        string contractDefine = "#define CONTRACTS_FULL";
                        if (!sourceCode.Contains(contractDefine))
                        {
                            sourceCode = contractDefine + " \r\n" + sourceCode;
                        }

                    }

                    string assemblyAttributes = string.Empty;
                    //if (sourceExe != null)
                    //    assemblyAttributes = CopyAssemblyInformation(sourceExe);

                    // Add assembly attributes to the source code
                    sourceCode = sourceCode.Replace("[[AssemblyAttributes]]", assemblyAttributes);

                    // Compile our code
                    results = provider.CompileAssemblyFromSource(parameters, new string[] { sourceCode });

                    if (results.Errors.Count > 0)
                    {
                        foreach (CompilerError error in results.Errors)
                        {
                            MessageBox.Show(Resources.An_error_occurred_while_compiling_the_C__code__ +
                                            String.Format(
                                                "Line {0}, Col {1}: Error {2} - {3}",
                                                error.Line,
                                                error.Column,
                                                error.ErrorNumber,
                                                error.ErrorText), Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else
                    {
                        if (Button_BuildMode.ComboBox.Text == "Contracts") // && results.Errors.Count == 0
                        {
                            try
                            {
                                string ccrewrite = "\"" + Path.Combine(Ultility.LibFolderPath, "ccrewrite.exe") + "\"";
                                Process process = new Process();
                                process.StartInfo = new ProcessStartInfo(ccrewrite, "");
                                process.StartInfo.WorkingDirectory = Common.Ultility.Ultility.APPLICATION_PATH;// Application.StartupPath;

                                process.StartInfo.Arguments = "\"" + dllName + "\" -output \"" + svd.FileName + "\"";
                                process.Start();

                                while (!process.HasExited)
                                {
                                    System.Threading.Thread.Sleep(500);
                                }

                                MessageBox.Show(Resources.C__library_is_built_successfully__You_can_use_them_now_in_your_models_, Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Information);    
                            }
                            catch (Exception ex1)
                            {
                                if (File.Exists(dllName))
                                {
                                    File.Delete(dllName);
                                }
                                MessageBox.Show(Resources.Exception_happened_during_the_Contracts_compilation__ + ex1.Message + "\r\n" + ex1.StackTrace, Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show(Resources.C__library_is_built_successfully__You_can_use_them_now_in_your_models_, Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Information);    
                        }                        
                    }
                    

                    //remove the temp file if any
                    try
                    {
                        if (dllName.EndsWith(".temp") && File.Exists(dllName))
                        {
                            File.Delete(dllName);
                        }
                    }
                    catch (Exception)
                    {
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(Resources.Exception_happened_during_the_compilation__ + ex.Message + "\r\n" + ex.StackTrace, Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        private void StatusLabel_FileName_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.StatusLabel_FileName.Text))
            {
                Process.Start(Path.GetDirectoryName(StatusLabel_FileName.Text));
            }
        }

        private void CSharpLibraryForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (StatusLabel_FileName.Text.EndsWith("*"))
            {
                MemoryBox box = new MemoryBox();
                MemoryBoxResult rt = box.ShowCloseMemoryDialog(StatusLabel_FileName.Text.TrimEnd('*'), true);


                if (rt == MemoryBoxResult.Yes)
                {
                    Button_Save_Click(sender, e);
                }
                else if (rt == MemoryBoxResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
                else if (rt == MemoryBoxResult.NoToAll)
                {
                    return;
                }
            }
        }
    }
}