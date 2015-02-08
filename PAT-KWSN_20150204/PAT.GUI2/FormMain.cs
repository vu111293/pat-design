using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using Fireball.Docking;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.SharpDevelop.DefaultEditor.Commands;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using PAT.Common;
using PAT.Common.Classes.ModuleInterface;
using PAT.Common.GUI;
using PAT.Common.Ultility;
using PAT.GUI.CLTSDrawing;
using PAT.GUI.Docking;
using PAT.GUI.EditingFunction.GoTo;
using PAT.GUI.ERADrawing;
using PAT.GUI.Forms;
using PAT.GUI.LTSDrawing;
using PAT.GUI.PDDLDrawing;
using PAT.GUI.PNDrawing;
using PAT.GUI.Properties;
using PAT.GUI.Register;
using PAT.GUI.SNDrawing;
using PAT.GUI.UMLDrawing;
using SearchAndReplace;
using CommentRegion = PAT.GUI.Docking.CommentRegion;
using KeyEventHandler = System.Windows.Forms.KeyEventHandler;
using PAT.GUI.TADrawing;

using PAT.GUI.ModuleGUI.KWSNDrawing;

namespace PAT.GUI
{
    public partial class FormMain : Form
    {
        private const string TAG = "FormMain";

        public delegate void DelegateGotoLine(string url, int lineNo);
        public DockContainer DockContainer = null;
        public event EventHandler<NewCodeEditorEventArgs> NewCodeEditor;

        //list of language files
        public static List<SyntaxMode> Languages = null;
        private string OpenFilter;
        public OutputDockingWindow Output_Window = null;
        public ErrorListWindow ErrorListWindow = null;

        public ModelExplorerWindow ModelExplorerWindow = null;
        public FindUsagesWindow FindUsagesWindow = null;

        public FormMain()
        {
            InitializeComponent();

            DockContainer = new DockContainer();
            DockContainer.MainForm = this;
            DockContainer.Dock = DockStyle.Fill;

            this.toolStripContainer1.ContentPanel.Controls.Add(this.DockContainer);

            DockContainer.ShowDocumentIcon = true;

            DockContainer.DocumentStyle = DocumentStyles.DockingWindow;

            DockContainer.ActiveDocumentChanged += new EventHandler(_DockContainer_ActiveDocumentChanged);

            DockContainer.ActiveContentChanged += new EventHandler(_DockContainer_ActiveContentChanged);

            DockContainer.ContextMenu = new ContextMenu();

            ///=========================Integration with SharpDevelop Start==================================

            Assembly exe = this.GetType().Assembly;

            ResourceService.RegisterNeutralStrings(new ResourceManager("PAT.GUI.Properties.StringResources", exe));
            ResourceService.RegisterNeutralImages(new ResourceManager("PAT.GUI.Properties.BitmapResources", exe));


            WorkbenchSingleton.DockContainer = DockContainer;
            WorkbenchSingleton.InitializeWorkbench();

            CoreStartup startup = new CoreStartup(Common.Ultility.Ultility.APPLICATION_NAME);
            startup.ConfigDirectory = Common.Ultility.Ultility.APPLICATION_PATH;// Application.StartupPath;
            startup.DataDirectory = Common.Ultility.Ultility.APPLICATION_PATH;// Application.StartupPath;

            startup.StartCoreServices();

            startup.RunInitialization();
            ///=========================Integration with SharpDevelop End==================================

            // change the message of the splash screen for loading modules
            SplashScreen.SplashScreen.UpdateStatusText("Loading modules..");

            //load modules
            LoadModules();

            // after load the modules, close the splash screen
            SplashScreen.SplashScreen.UpdateStatusText("Modules Loaded");

            ToolStripMenuItem[] tools = GetNewFileItems();

            ToolbarButton_New.DropDownItems.AddRange(tools);
            ToolStripItem wizardItem = ToolbarButton_New.DropDownItems[0];
            ToolbarButton_New.DropDownItems.RemoveAt(0);
            ToolbarButton_New.DropDownItems.Add(new ToolStripSeparator());
            ToolbarButton_New.DropDownItems.Add(wizardItem);

            tools = GetNewFileItems();

            MenuButton_New.DropDownItems.AddRange(tools);
            wizardItem = MenuButton_New.DropDownItems[0];
            MenuButton_New.DropDownItems.RemoveAt(0);
            MenuButton_New.DropDownItems.Add(new ToolStripSeparator());
            MenuButton_New.DropDownItems.Add(wizardItem);

            this.LoadRecents();

            this.FormClosing += new FormClosingEventHandler(FormMain_FormClosing);

            ((ToolStripProfessionalRenderer)StandardToolBar.Renderer).ColorTable.UseSystemColors = true;

            Output_Window = new OutputDockingWindow();
            Output_Window.Icon = Common.Ultility.Ultility.GetIcon("Output");
            Output_Window.Disposed += new EventHandler(output_Window_Disposed);

            ErrorListWindow = new ErrorListWindow();
            ErrorListWindow.Icon = Common.Ultility.Ultility.GetIcon("ErrorList");
            ErrorListWindow.Disposed += new EventHandler(ErrorListWindow_Disposed);
            ErrorListWindow.ListView.DoubleClick += new EventHandler(ErrorListView_DoubleClick);

            ModelExplorerWindow = new ModelExplorerWindow();
            ModelExplorerWindow.Icon = Common.Ultility.Ultility.GetIcon("ModelExplorer");
            ModelExplorerWindow.Disposed += new EventHandler(ModelExplorerWindowWindow_Disposed);
            ModelExplorerWindow.TreeView_Model.NodeMouseDoubleClick += new TreeNodeMouseClickEventHandler(TreeView_Model_NodeMouseDoubleClick);
            ModelExplorerWindow.Button_Refresh.Click += new EventHandler(Button_SpecParse_Click);

            FindUsagesWindow = new FindUsagesWindow();
            FindUsagesWindow.Icon = Common.Ultility.Ultility.GetIcon("Find Usages");
            FindUsagesWindow.Disposed += new EventHandler(FindUsagesWindow_Disposed);
            FindUsagesWindow.ListView.DoubleClick += new EventHandler(FindUsagesWindow_DoubleClick);
            FindUsagesWindow.Button_Refresh.Click += new EventHandler(Button_Refresh_Click);

            //check the correct language manu
            foreach (ToolStripMenuItem item in this.MenuButton_Languages.DropDownItems)
            {
                if (item.Tag.ToString() == GUIUltility.GUI_LANGUAGE)
                {
                    item.Checked = true;
                    break;
                }
            }

            SetFormMainCaption();

            this.toolStripContainer1.TopToolStripPanel.Controls.Clear();
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.FunctionalToolBar);
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.StandardToolBar);
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.MenuStrip);

            StandardToolBar.Location = new Point(3, 24);
            FunctionalToolBar.Location = new Point(3, 49);
            FunctionalToolBar.Size = new Size(409, 25);
            MenuStrip.Dock = DockStyle.Top;
            MenuStrip.Location = new Point(0, 0);

            if (!Common.Ultility.Ultility.IsWindowsOS)
                MenuButton_ModuleGenerator.Visible = false;

#if ACADEMIC
            MenuButton_Register.Visible = false;
            toolStripSeparator12.Visible = false;
            MenuButton_Japanese.Visible = false;
#else
            if (RegisterationForm.IsValidLicenseAvailable() != Ultility.LicenseType.Valid)
            {
                MenuButton_ModuleGenerator.Visible = false;
                MenuButton_CSharpLibary.Visible = false;
            }
#endif

            CheckForItemState();

            enableSpecificationOutputToolStripMenuItem.Checked = PAT.Common.Classes.Ultility.Ultility.ENABLE_PARSING_OUTPUT;
        }


        public void StartCheckUpdate()
        {
            if (GUIUltility.CHECK_UPDATE_AT_START && Common.Ultility.Ultility.IsWindowsOS)
            {
                UpdateCheckingWorker = new UpdateCheckingWorker();
                UpdateCheckingWorker.Initialize(this);

                UpdateCheckingWorker.ReturnResult += new UpdateCheckingWorker.ReturnResultEvent(GraphBuilder_ReturnResult);
                UpdateCheckingWorker.Start();
            }
        }

        private UpdateCheckingWorker UpdateCheckingWorker;

        private void GraphBuilder_ReturnResult()
        {
            try
            {
                this.stNewVersion.Visible = UpdateCheckingWorker.HasUpdate;
            }
            catch (Exception) { }
        }

        public void RepositionToolbar()
        {
            StandardToolBar.Location = new Point(3, 24);
            FunctionalToolBar.Location = new Point(3, 49);
            FunctionalToolBar.Size = new Size(409, 25);
            MenuStrip.Dock = DockStyle.Top;
            MenuStrip.Location = new Point(0, 0);
        }

        private void SetFormMainCaption()
        {
            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString().Substring(0, 5);

#if DEBUG
            this.Text = this.Text + " " + version + Resources._running_on_ + Environment.OSVersion.Platform + ") (Debug Model)";
#else
            this.Text = this.Text + " " + version + Resources._running_on_ + Environment.OSVersion.Platform + ")";
#endif

#if ACADEMIC
#else
            if (RegisterationForm.IsValidLicenseAvailable() == Ultility.LicenseType.Valid)
            {
                this.Text = this.Text + " - " + Resources.Registered_Version;
            }
            else if (RegisterationForm.IsValidLicenseAvailable() == Ultility.LicenseType.Evaluation)
            {
                this.Text = this.Text + string.Format(" - Evaluation Version ({0} days of {1} days)", RegisterationForm.CurrentEvaluationDays(), RegisterationForm.TotalEvaluationDays());
            }
            else if (RegisterationForm.IsValidLicenseAvailable() == Ultility.LicenseType.Invalid)
            {
                this.Text = this.Text + " - " + Resources.Version_Expired;
            }

            this.Text = this.Text.Replace("PAT", "PATPro");

#endif
            MenuButton_WhatsNewInPAT.Text = MenuButton_WhatsNewInPAT.Text + " " + version;
        }

        private void FormMain_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Right)
                {
                    saveToolStripMenuItem.Enabled = ToolbarButton_Save.Enabled;
                    openContainingFolderToolStripMenuItem.Enabled = !string.IsNullOrEmpty(this.CurrentEditorTabItem.FileName); //this.CurrentEditor.FileName != null;
                    this.ContextMenuStrip.Show(sender as Control, e.X, e.Y);
                }
            }
            catch (Exception ex)
            {
                Ultility.LogException(ex, null);
            }
        }

        #region Editor Methods

        private List<string> AcdamicModules = new List<string>() { "CSP", "PCSP", "RTS", "PRTS", "LTS" };

        private void LoadModules()
        {
            //initial the language list
            Languages = new List<SyntaxMode>();

            //load the open filter as well.
            OpenFilter = "";

            Common.Ultility.Ultility.ModuleFolderNames = new List<string>();
            Common.Ultility.Ultility.ModuleNames = new List<string>();
            Common.Ultility.Ultility.ModuleDictionary = new Dictionary<string, ModuleFacadeBase>();

            string[] modules = Directory.GetDirectories(Common.Ultility.Ultility.ModuleFolderPath);
            List<string> modulePathes = new List<string>(modules);
            modulePathes.Sort();

            foreach (string module in modulePathes)
            {
                try
                {
                    string moduleFolderName = Path.GetFileNameWithoutExtension(module);

                    if (this.LoadModule(moduleFolderName))
                    {
                        SyntaxMode syntax = HighlightingManager.Manager.AddHighlightingStrategy(Path.Combine(module, "Syntax.xshd"));

                        if (syntax != null)
                        {
                            Languages.Add(syntax);
                            if (GUIUltility.DEFAULT_MODELING_LANGUAGE == syntax.Name)
                                OpenFilter = syntax.Name + " (*" + syntax.ExtensionString + ")|*" + syntax.ExtensionString + "|" + OpenFilter;
                            else
                                OpenFilter += syntax.Name + " (*" + syntax.ExtensionString + ")|*" + syntax.ExtensionString + "|";

                            Common.Ultility.Ultility.ModuleFolderNames.Add(moduleFolderName);
                            Common.Ultility.Ultility.ModuleNames.Add(syntax.Name);

                            if (!Common.Ultility.Ultility.ModuleDictionary.ContainsKey(syntax.Name))
                                Common.Ultility.Ultility.ModuleDictionary.Add(syntax.Name, CurrentModule);

                            if (!Common.Ultility.Ultility.Images.ImageList.Images.ContainsKey(syntax.Name))
                                Common.Ultility.Ultility.Images.ImageList.Images.Add(syntax.Name, CurrentModule.ModuleIcon);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(Resources.Error_happned_in_loading_module_ + module + ":\r\n" + ex.Message,
                        Resources.Loading_error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            OpenFilter += "All File (*.*)|*.*";
        }

        private void _DockContainer_ActiveContentChanged(object sender, EventArgs e)
        {
            this.CheckForItemState();
        }

        private void _DockContainer_ActiveDocumentChanged(object sender, EventArgs e)
        {
            this.CheckInsMode();

            this.CheckForItemState();

            if (this.CurrentEditorTabItem != null)
                SetAllFileNameLabel(CurrentEditorTabItem.FileName);

            Caret_Change(sender, e);

            this.StatusLabel_Status.Text = "Ready";
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                foreach (IDockableWindow doc in this.DockContainer.Documents)
                {
                    if (doc is EditorTabItem)
                    {
                        EditorTabItem item = doc as EditorTabItem;
                        if (item.TabText.EndsWith("*"))
                        {
                            MemoryBox box = new MemoryBox();
                            MemoryBoxResult rt = box.ShowCloseMemoryDialog(item.TabText.TrimEnd('*'));

                            if (rt == MemoryBoxResult.Yes)
                            {
                                if (string.IsNullOrEmpty(item.FileName) || !this.CurrentEditorTabItem.HaveFileName)
                                {
                                    SaveFileDialog svd = new SaveFileDialog();
                                    svd.Filter = item.FileExtension;
                                    if (svd.ShowDialog() == DialogResult.OK)
                                    {
                                        item.Save(svd.FileName);
                                        AddRecent(item.FileName);
                                        SetAllFileNameLabel(svd.FileName);

                                        item.TabText = item.TabText.TrimEnd('*');
                                    }
                                }
                                else
                                {
                                    item.Save(null);
                                    item.TabText = item.TabText.TrimEnd('*');
                                    SetAllFileNameLabel(item.FileName);
                                }
                            }
                            else if (rt == MemoryBoxResult.Cancel)
                            {
                                e.Cancel = true;
                                return;
                            }
                            else if (rt == MemoryBoxResult.NoToAll)
                            {
                                break;
                            }
                        }
                    }
                }

                if (!e.Cancel)
                {
                    this.SaveRecentFiles();
                    GUIUltility.SaveSettingValue();
                }
            }
            catch (IOException) { }
            catch (Exception ex)
            {
                Ultility.LogException(ex, null);
            }
        }

        private void Caret_Change(object sender, EventArgs e)
        {
            if (CurrentEditor != null)
            {
                stColumLabel.Text = "Col: " + this.CurrentEditor.ActiveTextAreaControl.Caret.Column;
                stLineLabel.Text = "Ln: " + (this.CurrentEditor.ActiveTextAreaControl.Caret.Line + 1);
            }
            else
            {
                stColumLabel.Text = "";
                stLineLabel.Text = "";
            }
        }

        private void ToolbarButton_New_Click(object sender, EventArgs e)
        {
            if (GUIUltility.DEFAULT_MODELING_LANGUAGE == "")
            {
                this.ToolbarButton_NewModelWizard.PerformClick();
            }
            else
            {
                foreach (ToolStripItem item in ToolbarButton_New.DropDownItems)
                {
                    if (item.Text == GUIUltility.DEFAULT_MODELING_LANGUAGE)
                    {
                        item.PerformClick();
                        break;
                    }
                }
            }
        }

        private void ToolbarButton_NewModelWizard_Click(object sender, EventArgs e)
        {
            try
            {
                NewWizardForm newWizard = new NewWizardForm();
                if (newWizard.ShowDialog() == DialogResult.OK)
                {
                    ShowModel(newWizard.TemplateModel, newWizard.SelectedModule);

                    if (newWizard.SetDefaultLanguage)
                    {
                        GUIUltility.DEFAULT_MODELING_LANGUAGE = newWizard.SelectedModule;
                        GUIUltility.SaveSettingValue();
                    }
                }
            }
            catch (Exception ex)
            {
                Common.Ultility.Ultility.LogException(ex, null);
            }
        }

        private void mnuOpen_Click(object sender, EventArgs e)
        {
            this.Open();
        }

        private void mnuSave_Click(object sender, EventArgs e)
        {
            this.Save();
        }

        private void mnuSaveAs_Click(object sender, EventArgs e)
        {
            this.SaveAs();
        }

        private void mnuExit_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Exit();
        }

        private void LoadRecents()
        {
            try
            {
                string file = Path.Combine(Common.Ultility.Ultility.APPLICATION_PATH, "recents.xml");
                if (!File.Exists(file))
                {
                    file = Path.Combine(Common.Ultility.Ultility.UserDocumentFolderPath, "recents.xml");
                    if (!File.Exists(file))
                        return;
                }

                XmlTextReader xreader = new XmlTextReader(file);
                int index = 1;

                while (xreader.Read())
                {
                    if (xreader.IsStartElement("file"))
                    {
                        string path = xreader.ReadString();
                        ToolStripMenuItem item = new ToolStripMenuItem(index.ToString() + ": " + Path.GetFileName(path));
                        item.Tag = path;
                        item.Click += new EventHandler(item_Click);
                        MenuButton_RecentFiles.DropDownItems.Add(item);
                        index++;
                    }
                }

                xreader.Close();
            }
            catch (Exception) { }
        }

        private void AddRecent(string filename)
        {
            for (int i = 0; i < MenuButton_RecentFiles.DropDownItems.Count; i++)
            {
                ToolStripMenuItem current = (ToolStripMenuItem)MenuButton_RecentFiles.DropDownItems[i];
                if (current.Tag != null && current.Tag.ToString() == filename)
                {
                    MenuButton_RecentFiles.DropDownItems.Remove(current);
                    break;
                }
            }

            if (MenuButton_RecentFiles.DropDownItems.Count > 7)
            {
                for (int i = MenuButton_RecentFiles.DropDownItems.Count - 1; i > 7; i--)
                    MenuButton_RecentFiles.DropDownItems.RemoveAt(i);
            }

            ToolStripMenuItem item = new ToolStripMenuItem("1: " + Path.GetFileName(filename));
            item.Tag = filename;
            item.Click += new EventHandler(item_Click);

            MenuButton_RecentFiles.DropDownItems.Insert(0, item);
            for (int i = 1; i < MenuButton_RecentFiles.DropDownItems.Count; i++)
            {
                ToolStripMenuItem current = (ToolStripMenuItem)MenuButton_RecentFiles.DropDownItems[i];
                current.Text = (i + 1).ToString() + ": " + Path.GetFileName(current.Tag.ToString());
            }
        }

        private void SaveRecentFiles()
        {
            try
            {
                string file = Path.Combine(Common.Ultility.Ultility.APPLICATION_PATH, "recents.xml");
                if (!SaveRecentFile(file))
                {
                    file = Path.Combine(Common.Ultility.Ultility.UserDocumentFolderPath, "recents.xml");
                    SaveRecentFile(file);
                }
            }
            catch (Exception) { }
        }

        private bool SaveRecentFile(string file)
        {
            try
            {
                XmlTextWriter xwr = new XmlTextWriter(file, Encoding.UTF8);
                xwr.WriteStartElement("files");

                for (int i = 0; i < MenuButton_RecentFiles.DropDownItems.Count; i++)
                {
                    ToolStripMenuItem current = (ToolStripMenuItem)MenuButton_RecentFiles.DropDownItems[i];
                    xwr.WriteStartElement("file");
                    xwr.WriteString(current.Tag.ToString());
                    xwr.WriteEndElement();
                }

                xwr.WriteEndElement();
                xwr.Flush();
                xwr.Close();

                return true;
            }
            catch (Exception) { }

            return false;
        }

        private void item_Click(object sender, EventArgs e)
        {
            try
            {
                ToolStripMenuItem item = (ToolStripMenuItem)sender;
                this.OpenFile(item.Tag.ToString(), true);
            }
            catch (Exception ex)
            {
                Ultility.LogException(ex, null);
            }
        }

        public void Save()
        {
            try
            {
                //if the document is saved already.
                if (!this.CurrentEditorTabItem.TabText.EndsWith("*"))
                {
                    this.StatusLabel_Status.Text = "Model Saved";
                    return;
                }

                if (string.IsNullOrEmpty(this.CurrentEditorTabItem.FileName) || !this.CurrentEditorTabItem.HaveFileName)
                {
                    this.SaveAs();
                }
                else
                {
                    string filePath = this.CurrentEditorTabItem.FileName;
                    bool isReadOnly = ((File.GetAttributes(filePath) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly);
                    if (isReadOnly)
                    {
                        if (MessageBox.Show(Resources.This_file_is_read_only__Do_you_want_to_overwrite_it_, Resources.Saving_error, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
                            File.SetAttributes(filePath, File.GetAttributes(filePath) & ~(FileAttributes.ReadOnly));
                        else
                            return;
                    }

                    this.CurrentEditorTabItem.Save(null);

                    if (CurrentEditorTabItem != null && CurrentEditorTabItem.TabText.EndsWith("*"))
                        CurrentEditorTabItem.TabText = CurrentEditorTabItem.TabText.TrimEnd('*');

                    ToolbarButton_Save.Enabled = false;
                    MenuButton_Save.Enabled = ToolbarButton_Save.Enabled;
                }
                SetAllFileNameLabel(this.CurrentEditorTabItem.FileName);
                this.StatusLabel_Status.Text = "Model Saved";
            }
            catch (FileNotFoundException)
            {
                this.SaveAs();
            }
            catch (System.UnauthorizedAccessException)
            {
                this.SaveAs();
            }
            catch (IOException)
            {
                this.SaveAs();
            }
            catch (Exception ex)
            {
                Ultility.LogException(ex, null);
            }
        }

        public void Open()
        {
            try
            {
                OpenFileDialog opf = new OpenFileDialog();
                opf.Filter = OpenFilter;
                if (opf.ShowDialog() == DialogResult.OK)
                    this.OpenFile(opf.FileName, true);
            }
            catch (Exception ex)
            {
                Ultility.LogException(ex, null);
            }
        }

        public EditorTabItem CurrentEditorTabItem
        {
            get
            {
                if (Common.Ultility.Ultility.IsUnixOS)
                    return CurrentActiveTab;

                return this.DockContainer.ActiveDocument as EditorTabItem;
            }
        }

        public ToolStripPanel TopToolStripPanel
        {
            get             {                return toolStripContainer1.TopToolStripPanel;            }
        }

        public void OpenFileLastFile()
        {
            try
            {
                if (MenuButton_RecentFiles.DropDownItems.Count > 0)
                {
                    ToolStripMenuItem item = (ToolStripMenuItem)MenuButton_RecentFiles.DropDownItems[0];
                    this.OpenFile(item.Tag.ToString(), false);
                }
            }
            catch (Exception) { }

            if (DockContainer.Documents.Length == 0)
                this.ToolbarButton_New.PerformButtonClick();
        }

        public EditorTabItem OpenFile(string filename, bool ShowMessageBox)
        {
            int N = DockContainer.Documents.Length;
            for (int i = 0; i < N; i++)
            {
                EditorTabItem item = DockContainer.Documents[i] as EditorTabItem;
                if (item != null)
                {
                    if (item.FileName == filename || (!Common.Ultility.Ultility.IsWindowsOS && item.FileName.EndsWith(filename)))
                    {
                        item.Activate();
                        CurrentActiveTab = item;
                        SetAllFileNameLabel(filename);
                        return item;
                    }
                }
            }

            if (File.Exists(filename))
            {
                //try
                {
                    SyntaxMode ModuleSyntax = null;
                    foreach (SyntaxMode syntax in Languages)
                    {
                        foreach (string extension in syntax.Extensions)
                        {
                            if (filename.ToLower().EndsWith(extension))
                            {
                                ModuleSyntax = syntax;
                                break;
                            }
                        }

                        if (ModuleSyntax != null)
                            break;
                    }

                    if (ModuleSyntax == null)
                    {
                        if (ShowMessageBox)
                            MessageBox.Show(Resources.Error_happened_in_opening_ + filename + ".\r\n" + Resources.File_format_is_not_supported_by_PAT_, Common.Ultility.Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);

                        return null;
                    }

                    //try
                    {
                        OpenFilter = "";
                        foreach (SyntaxMode syntax in Languages)
                        {
                            if (ModuleSyntax.Name == syntax.Name)
                                OpenFilter = syntax.Name + " (*" + syntax.ExtensionString + ")|*" + syntax.ExtensionString + "|" + OpenFilter;
                            else
                                OpenFilter += syntax.Name + " (*" + syntax.ExtensionString + ")|*" + syntax.ExtensionString + "|";
                        }

                        OpenFilter += "All File (*.*)|*.*";
                    }
                    //catch (Exception) { }

                    EditorTabItem tabItem = this.AddDocument(ModuleSyntax.Name);
                    tabItem.Open(filename);
                    AddRecent(filename);
                    SetAllFileNameLabel(filename);
                    if (tabItem.TabText.EndsWith("*"))
                        tabItem.TabText = tabItem.TabText.TrimEnd('*');
                    this.StatusLabel_Status.Text = "Ready";

                    return tabItem;
                }
                /*catch (Exception ex)
                {
                    if (ShowMessageBox)
                    {
                        MessageBox.Show(Resources.Open_Error_ + ex.Message + "\r\n sfgasdfgszdfgsdfgsdfgdjkkjhjjhvasrjfgasd" + Resources.Please_make_sure_that_the_format_is_correct_,
                            Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    Log.d(TAG, ex.Message);
                }*/
            }
            else
            {
                if (ShowMessageBox)
                {
                    MessageBox.Show(Resources.Open_Error__the_selected_file_is_not_found_,
                        Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            return null;
        }

        public static EditorTabItem CurrentActiveTab;

        public EditorTabItem AddDocument(string moduleName)
        {
            bool registerMouseEvent = DockContainer.Panes.Count == 0;
            EditorTabItem tabItem;

            if (moduleName == "Labeled Transition Systems" || moduleName == "Light Labeled Transition Systems")
            {
                tabItem = new LTSTabItem(moduleName);
            }
            else if (moduleName == "Compositional Labeled Transition Systems")
            {
                tabItem = new CLTSTabItem(moduleName);
            }
            else if (moduleName == "Timed Automata")
            {
                tabItem = new TATabItem(moduleName);
            }
            else if (moduleName == "Event Recording Automata")
            {
                tabItem = new ERATabItem(moduleName);
            }
            else if (moduleName == "NESC Model")
            {
                tabItem = new SNTabItem(moduleName);
            }
            else if (moduleName == "BPEL Model")
            {
                tabItem = new BPELTabItem(moduleName);
            }
            else if (moduleName == "UML Model")
            {
                tabItem = new UMLTabItem(moduleName);
            }
            else if (moduleName == "PDDL Model")
            {
                tabItem = new PDDLTabItem(moduleName);
            }
            // add module Petri Nets Model here
            else if (moduleName == "PN Model")
            {
                tabItem = new PNTabItem(moduleName);
            }
            else if (moduleName == "PN2 Model")
            {
                tabItem = new PNTabItem(moduleName);
            }
            else if (moduleName == "KWSN Model")
            {
                tabItem = new WSNTabItem(moduleName, "KWSN", this);
            }
            else
            {
                tabItem = new EditorTabItem(moduleName);
            }

            tabItem.Tag = "";
            tabItem.Show(DockContainer, DockState.Document);
            tabItem.CodeEditor.LineViewerStyle = (MenuButton_HignlightCurrentLine.Checked) ? LineViewerStyle.FullRow : LineViewerStyle.None;
            tabItem.FormClosing += new FormClosingEventHandler(tabItem_FormClosing);
            if (NewCodeEditor != null)
                NewCodeEditor(this, new NewCodeEditorEventArgs(ref tabItem));

            tabItem.CodeEditor.TextChanged += new EventHandler(editorControl_TextChanged);
            tabItem.CodeEditor.KeyUp += new KeyEventHandler(editorControl_KeyUp);
            tabItem.CodeEditor.ActiveTextAreaControl.TextArea.DragDrop += new DragEventHandler(CodeEditor_DragDrop);
            tabItem.TabActivited += new EditorTabItem.TabActivitedHandler(tabItem_Activated);

            tabItem.CodeEditor.ActiveTextAreaControl.Caret.CaretModeChanged += new EventHandler(CaretModeChanged);
            tabItem.CodeEditor.ActiveTextAreaControl.Enter += new EventHandler(Caret_Change);
            tabItem.CodeEditor.ActiveTextAreaControl.Caret.PositionChanged += Caret_Change;
            tabItem.IsDirtyChanged += new EventHandler(editorControl_TextChanged);
            tabItem.FindUsages += new EditorTabItem.FindUsagesHandler(tabItem_FindUsages);
            tabItem.GoToDeclarition += new EditorTabItem.GoToDeclaritionHandler(OpenException);

            MenuButton_Window.DropDownItems.Add(tabItem.WindowMenuItem);
            tabItem.WindowMenuItem.Click += new EventHandler(WindowMenuItem_Click);
            tabItem.Activate();

            CurrentActiveTab = tabItem;
            if (registerMouseEvent)
                DockContainer.Panes[0].TabStripControl.MouseDown += new MouseEventHandler(FormMain_MouseDown);

            return tabItem;
        }

        void CodeEditor_DragDrop(object sender, DragEventArgs e)
        {
            // Can only drop files, so check
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                return;

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files)
                this.OpenFile(file, true);
        }

        void CaretModeChanged(object sender, EventArgs e)
        {
            this.CheckInsMode();
        }

        private void tabItem_Activated(EditorTabItem tab)
        {
            CurrentActiveTab = tab;
            foreach (var item in MenuButton_Window.DropDownItems)
            {
                if (item is ToolStripMenuItem)
                    (item as ToolStripMenuItem).Checked = false;
            }

            tab.WindowMenuItem.Checked = true;
            LoadModule(tab.ModuleName);

            if (ModelExplorerWindow != null)
            {
                if (ModelExplorerWindow.Specification != tab.Specification)
                    ModelExplorerWindow.DisplayTree(tab.Specification);
            }
        }

        private void WindowMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menu = sender as ToolStripMenuItem;
            int N = DockContainer.Documents.Length;
            for (int i = 0; i < N; i++)
            {
                EditorTabItem item = DockContainer.Documents[i] as EditorTabItem;
                if (item != null && item.TabText == menu.Text)
                {
                    item.Activate();
                    CurrentActiveTab = item;
                    return;
                }
            }
        }

        private void tabItem_FormClosing(object sender, FormClosingEventArgs e)
        {
            EditorTabItem currentDoc = sender as EditorTabItem;
            if (currentDoc != null)
            {
                if (currentDoc.TabText.EndsWith("*"))
                {
                    DialogResult rt = MessageBox.Show(string.Format(Resources.Document__0__unsaved__Do_you_want_to_save_it_before_close_, currentDoc.TabText.TrimEnd('*')),
                        Ultility.APPLICATION_NAME, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (rt == DialogResult.Yes)
                        this.Save();
                    else if (rt == DialogResult.Cancel)
                        e.Cancel = true;
                }
            }

            MenuButton_Window.DropDownItems.Remove(currentDoc.WindowMenuItem);
            if (DockContainer.Documents.Length > 1)
            {
                this.MenuButton_NavigateBack.PerformClick();
            }
            else if (DockContainer.Documents.Length == 1)
            {
                while (this.FunctionalToolBar.Items.Count > 6)
                    this.FunctionalToolBar.Items.RemoveAt(this.FunctionalToolBar.Items.Count - 1);

                CurrentActiveTab = null;
                if (ModelExplorerWindow != null)
                    ModelExplorerWindow.DisplayTree(null);
            }

            CheckForItemState();
        }

        public void SaveAs()
        {
            try
            {
                SaveFileDialog svd = new SaveFileDialog();
                if (CurrentEditorTabItem != null)
                {
                    svd.Filter = CurrentEditorTabItem.FileExtension;
                    if (svd.ShowDialog() == DialogResult.OK)
                    {
                        CurrentEditorTabItem.Save(svd.FileName);
                        AddRecent(CurrentEditorTabItem.FileName);
                        SetAllFileNameLabel(svd.FileName);
                        //CurrentEditorTabItem.SetSyntaxLanguageFromFile(svd.FileName);
                        CurrentEditorTabItem.TabText = Path.GetFileName(svd.FileName);
                        ToolbarButton_Save.Enabled = false;
                        MenuButton_Save.Enabled = ToolbarButton_Save.Enabled;

                        this.StatusLabel_Status.Text = "Model Saved";
                    }
                }

            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                Ultility.LogException(ex, null);
            }
        }

        private void mnuUndo_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.Output_Window.TextBox.Focused)
                    this.Output_Window.TextBox.Undo();
                else
                    this.CurrentEditorTabItem.Undo();
            }
            catch (Exception ex)
            {
                Ultility.LogException(ex, null);
            }
        }

        private void mnuRedo_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.Output_Window.TextBox.Focused)
                    this.Output_Window.TextBox.Redo();
                else
                    this.CurrentEditorTabItem.Redo();
            }
            catch (Exception ex)
            {
                Ultility.LogException(ex, null);
            }
        }

        private void mnuCut_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.Output_Window.TextBox.Focused)
                    this.Output_Window.TextBox.Cut();
                else
                    this.CurrentEditorTabItem.Cut();
            }
            catch (Exception ex)
            {
                Ultility.LogException(ex, null);
            }
        }

        private void MenuButton_Delete_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.Output_Window.TextBox.Focused)
                    this.Output_Window.TextBox.Copy();
                else
                    this.CurrentEditorTabItem.Delete();
            }
            catch (Exception ex)
            {
                Ultility.LogException(ex, null);
            }
        }

        private void mnuCopy_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.Output_Window.TextBox.Focused)
                    this.Output_Window.TextBox.Copy();
                else
                    this.CurrentEditorTabItem.Copy();
            }
            catch (Exception ex)
            {
                Ultility.LogException(ex, null);
            }
        }

        private void mnuPaste_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.Output_Window.TextBox.Focused)
                    this.Output_Window.TextBox.Paste();
                else
                    this.CurrentEditorTabItem.Paste();
            }
            catch (Exception ex)
            {
                Ultility.LogException(ex, null);
            }
        }

        private void mnuSelectAll_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.Output_Window.TextBox.Focused)
                    this.Output_Window.TextBox.SelectAll();
                else
                    this.CurrentEditorTabItem.SelectAll();
            }
            catch (Exception ex)
            {
                Ultility.LogException(ex, null);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            this.Save();
        }

        #region "Tab Context Manual Clicks Events"

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Save();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentEditorTabItem != null)
                CurrentEditorTabItem.Close();
        }

        private void closeAllButThisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int counter = 0;
            while (DockContainer.Documents.Length > 1 || counter != 1)
            {
                EditorTabItem item = DockContainer.Documents[counter] as EditorTabItem;
                if (item == this.DockContainer.ActiveDocument)
                    counter++;
                else
                    item.Close();
            }
        }

        private void closeAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (EditorTabItem item in this.DockContainer.Documents)
                item.Close();
        }

        private void MenuButton_CloseCurrent_Click(object sender, EventArgs e)
        {
            if (this.CurrentEditorTabItem != null)
                this.CurrentEditorTabItem.Close();
        }

        private void openContainingFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stFileLabel_DoubleClick(sender, e);
        }

        #endregion

        private void btnOpen_Click(object sender, EventArgs e)
        {
            this.Open();
        }

        private void btnSaveAs_Click(object sender, EventArgs e)
        {
            this.SaveAs();
        }

        private void ToolbarButton_SubmitModel_Click(object sender, EventArgs e)
        {
            try
            {
                if (CurrentEditorTabItem != null && CurrentEditorTabItem.Text.Trim() != "")
                {
                    SubmitToAuthorForm form = new SubmitToAuthorForm(CurrentEditorTabItem.Text.Trim());
                    form.ShowDialog();
                }
                else
                {
                    MessageBox.Show(Resources.Please_load_a_model_first_, Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                Common.Ultility.Ultility.LogException(ex, null);
            }
        }

        private void CheckForItemState()
        {
            if (this.CurrentEditorTabItem == null)
            {
                MenuButton_Undo.Enabled = false;
                MenuButton_Redo.Enabled = false;

                MenuButton_Delete.Enabled = false;
                MenuButton_Paste.Enabled = false;
                MenuButton_Cut.Enabled = false;
                MenuButton_Copy.Enabled = false;

                MenuButton_SelectAll.Enabled = false;

                ToolbarButton_Cut.Enabled = false;
                ToolbarButton_Copy.Enabled = false;
                ToolbarButton_Paste.Enabled = false;

                ToolbarButton_Redo.Enabled = false;
                ToolbarButton_Undo.Enabled = false;
                ToolbarButton_Save.Enabled = false;
                ToolbarButton_SaveAs.Enabled = false;
                ToolbarButton_SubmitModel.Enabled = false;
                MenuButton_SubmitModel.Enabled = false;

                ToolbarButton_Print.Enabled = false;
                MenuButton_Print.Enabled = false;

                ToolbarButton_LaTexGenerate.Enabled = false;
                this.MenuButton_GenerateLatex.Enabled = false;

                MenuButton_Save.Enabled = false;
                MenuButton_SaveAs.Enabled = false;

                ToolbarButton_Find.Enabled = false;
                ToolbarButton_FindNext.Enabled = false;
                ToolbarButton_Replace.Enabled = false;

                MenuButton_Find.Enabled = false;
                MenuButton_FindNext.Enabled = false;
                MenuButton_Replace.Enabled = false;

                this.MenuButton_GoTo.Enabled = false;
                this.MenuButton_AddComment.Enabled = false;
                this.MenuButton_RemoveComment.Enabled = false;

                ToolbarButton_AddComment.Enabled = false;
                ToolbarButton_RemoveComment.Enabled = false;

                MenuButton_LineNumber.Enabled = false;
                MenuButton_HignlightCurrentLine.Enabled = false;

                FunctionalToolBar.Enabled = false;
                MenuButton_CheckGrammer.Enabled = false;
                MenuButton_Simulation.Enabled = false;
                MenuButton_Verification.Enabled = false;

                MenuButton_NavigateBack.Enabled = false;
                MenuButton_NavigateForward.Enabled = false;
                MenuButton_Split.Enabled = false;
                MenuButton_CloseCurrent.Enabled = false;
                MenuButton_CloseAllDocument.Enabled = false;

                MenuButton_ToggleOutline.Enabled = false;
                MenuButton_ToggleAllOutlines.Enabled = false;
                MenuButton_ToggleBookmark.Enabled = false;
                MenuButton_PreviousBookmark.Enabled = false;
                MenuButton_NextBookmark.Enabled = false;
                MenuButton_ClearAllBookmarks.Enabled = false;

                ToolbarButton_ZoomOut.Enabled = false;
                ToolbarButton_Zoom100.Enabled = false;
                ToolbarButton_ZoomIn.Enabled = false;

                return;
            }

            MenuButton_Undo.Enabled = this.CurrentEditorTabItem.CanUndo;
            MenuButton_Redo.Enabled = this.CurrentEditorTabItem.CanRedo;

            ToolbarButton_Save.Enabled = this.CurrentEditorTabItem.IsDirty;
            ToolbarButton_SaveAs.Enabled = true;

            ToolbarButton_SubmitModel.Enabled = true;
            MenuButton_SubmitModel.Enabled = true;

            ToolbarButton_Print.Enabled = this.CurrentEditorTabItem.CanPrint;
            MenuButton_Print.Enabled = this.CurrentEditorTabItem.CanPrint;

            ToolbarButton_LaTexGenerate.Enabled = this.CurrentEditorTabItem.CanPrint;
            MenuButton_GenerateLatex.Enabled = this.CurrentEditorTabItem.CanPrint;

            MenuButton_Save.Enabled = ToolbarButton_Save.Enabled;
            MenuButton_SaveAs.Enabled = true;

            ToolbarButton_Find.Enabled = this.CurrentEditorTabItem.CanFind;
            ToolbarButton_FindNext.Enabled = this.CurrentEditorTabItem.CanFind;
            ToolbarButton_Replace.Enabled = this.CurrentEditorTabItem.CanFind;

            MenuButton_Delete.Enabled = true;
            MenuButton_Paste.Enabled = this.CurrentEditorTabItem.CanPaste;
            MenuButton_Cut.Enabled = this.CurrentEditorTabItem.CanCut;
            MenuButton_Copy.Enabled = this.CurrentEditorTabItem.CanCopy;

            MenuButton_SelectAll.Enabled = this.CurrentEditorTabItem.CanSelectAll;

            ToolbarButton_Cut.Enabled = MenuButton_Cut.Enabled;
            ToolbarButton_Copy.Enabled = MenuButton_Copy.Enabled;
            ToolbarButton_Paste.Enabled = MenuButton_Paste.Enabled;

            ToolbarButton_Redo.Enabled = MenuButton_Redo.Enabled;
            ToolbarButton_Undo.Enabled = MenuButton_Undo.Enabled;

            MenuButton_Find.Enabled = ToolbarButton_Find.Enabled;
            MenuButton_FindNext.Enabled = ToolbarButton_Find.Enabled;
            MenuButton_Replace.Enabled = ToolbarButton_Find.Enabled;

            this.MenuButton_GoTo.Enabled = CurrentEditorTabItem.CodeEditor.Visible;

            this.MenuButton_AddComment.Enabled = CurrentEditorTabItem.CodeEditor.Visible;
            this.MenuButton_RemoveComment.Enabled = CurrentEditorTabItem.CodeEditor.Visible;

            this.ToolbarButton_AddComment.Enabled = CurrentEditorTabItem.CodeEditor.Visible;
            this.ToolbarButton_RemoveComment.Enabled = CurrentEditorTabItem.CodeEditor.Visible;


            MenuButton_LineNumber.Enabled = CurrentEditorTabItem.CodeEditor.Visible;
            MenuButton_HignlightCurrentLine.Enabled = CurrentEditorTabItem.CodeEditor.Visible;
            MenuButton_LineNumber.Checked = this.CurrentEditor.ShowLineNumbers;
            MenuButton_HignlightCurrentLine.Checked = this.CurrentEditor.LineViewerStyle == LineViewerStyle.FullRow;

            FunctionalToolBar.Enabled = true;
            MenuButton_CheckGrammer.Enabled = true;
            MenuButton_Simulation.Enabled = true;
            MenuButton_Verification.Enabled = true;

            MenuButton_NavigateBack.Enabled = true;
            MenuButton_NavigateForward.Enabled = true;
            MenuButton_Split.Enabled = true;
            MenuButton_CloseCurrent.Enabled = true;
            MenuButton_CloseAllDocument.Enabled = true;

            MenuButton_ToggleOutline.Enabled = CurrentEditorTabItem.CodeEditor.Visible;
            MenuButton_ToggleAllOutlines.Enabled = CurrentEditorTabItem.CodeEditor.Visible;
            MenuButton_ToggleBookmark.Enabled = CurrentEditorTabItem.CodeEditor.Visible;
            MenuButton_PreviousBookmark.Enabled = CurrentEditorTabItem.CodeEditor.Visible;
            MenuButton_NextBookmark.Enabled = CurrentEditorTabItem.CodeEditor.Visible;
            MenuButton_ClearAllBookmarks.Enabled = CurrentEditorTabItem.CodeEditor.Visible;

            ToolbarButton_ZoomOut.Enabled = true;
            ToolbarButton_Zoom100.Enabled = true;
            ToolbarButton_ZoomIn.Enabled = true;
        }

        private void SetAllFileNameLabel(string name)
        {
            stFileLabel.Text = Path.GetFileName(name);
            stFileLabel.ToolTipText = Resources.Double_click_to_open_the_folder_of_ + name;
        }

        private void mnuEdit_DropDownOpening(object sender, EventArgs e)
        {
            this.CheckForItemState();
        }

        private void btnCut_Click(object sender, EventArgs e)
        {
            try
            {
                this.CurrentEditorTabItem.Cut();
            }
            catch (Exception ex)
            {
                Ultility.LogException(ex, null);
            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            try
            {
                this.CurrentEditorTabItem.Copy();
            }
            catch (Exception ex)
            {
                Ultility.LogException(ex, null);
            }
        }

        private void btnPaste_Click(object sender, EventArgs e)
        {
            try
            {
                this.CurrentEditorTabItem.Paste();
            }
            catch (Exception ex)
            {
                Ultility.LogException(ex, null);
            }
        }

        private void btnUndo_Click(object sender, EventArgs e)
        {
            try
            {
                this.CurrentEditorTabItem.Undo();
            }
            catch (Exception ex)
            {
                Ultility.LogException(ex, null);
            }
        }

        private void btnRedo_Click(object sender, EventArgs e)
        {
            try
            {
                this.CurrentEditorTabItem.Redo();
            }
            catch (Exception ex)
            {
                Ultility.LogException(ex, null);
            }
        }

        private void editorControl_TextChanged(object sender, EventArgs e)
        {
            this.CheckForItemState();
        }

        private ToolStripMenuItem[] GetNewFileItems()
        {
            int i = 0;
            ArrayList list = new ArrayList();
            foreach (SyntaxMode mode in Languages)
            {
                ToolStripMenuItem tabItem = new ToolStripMenuItem(mode.Name);
                tabItem.Click += new EventHandler(NewMenuItems_Click);
                tabItem.Image = Ultility.GetModuleImage(mode.Name);
                list.Add(tabItem);

                i++;
            }

            ToolStripMenuItem[] items = new ToolStripMenuItem[list.Count];
            list.CopyTo(items);

            return items;
        }

        //store the current loaded module, if the next module is same as the current one, then don't need to reload it.
        private ModuleFacadeBase CurrentModule;

        private bool LoadModule(string moduleName)
        {
            try
            {
                if (Common.Ultility.Ultility.ModuleDictionary.ContainsKey(moduleName))
                {
                    if (CurrentModule == null || moduleName != CurrentModule.ModuleName)
                    {
                        CurrentModule = Common.Ultility.Ultility.ModuleDictionary[moduleName];
                        this.FunctionalToolBar.SuspendLayout();
                        while (this.FunctionalToolBar.Items.Count > 6)
                            this.FunctionalToolBar.Items.RemoveAt(this.FunctionalToolBar.Items.Count - 1);

                        CurrentModule.ToolbarInitialize(this.FunctionalToolBar, Toolbar_Button_Click);
                        this.FunctionalToolBar.ResumeLayout();
                    }
                }
                else
                {
                    string facadeClass = "PAT." + moduleName + ".ModuleFacade";
                    string file = Path.Combine(Path.Combine(Common.Ultility.Ultility.ModuleFolderPath, moduleName), "PAT.Module." + moduleName + ".dll");

                    Assembly assembly = Assembly.LoadFrom(file);
                    CurrentModule = (ModuleFacadeBase)assembly.CreateInstance(
                                                           facadeClass,
                                                           true,
                                                           BindingFlags.CreateInstance,
                                                           null, null,
                                                           null, null);

                    if (CurrentModule.GetType().Namespace != "PAT." + moduleName)
                    {
                        CurrentModule = null;
                        return false;
                    }

                    CurrentModule.ShowModel += new ShowModelHandler(ShowModel);
                    CurrentModule.ExampleMenualToolbarInitialize(this.MenuButton_Examples);
                    CurrentModule.ReadConfiguration();

                    this.FunctionalToolBar.SuspendLayout();

                    while (this.FunctionalToolBar.Items.Count > 6)
                        this.FunctionalToolBar.Items.RemoveAt(this.FunctionalToolBar.Items.Count - 1);

                    CurrentModule.ToolbarInitialize(this.FunctionalToolBar, Toolbar_Button_Click);
                    this.FunctionalToolBar.ResumeLayout();
                }

                return true;
            }
            catch { }

            return false;
        }

        private void NewMenuItems_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            EditorTabItem tabItem = AddDocument(item.Text);
            tabItem.TabText = tabItem.TabText.TrimEnd('*');
        }

        private void stFileLabel_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (CurrentEditorTabItem != null)
                    Process.Start(Path.GetDirectoryName(CurrentEditorTabItem.FileName));
            }
            catch { }
        }

        private void stFileLabel_MouseEnter(object sender, EventArgs e)
        {
            StatusBar.Cursor = Cursors.Hand;
        }

        private void stFileLabel_MouseLeave(object sender, EventArgs e)
        {
            StatusBar.Cursor = Cursors.Default;
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                Find Find = new Find();
                Find.Run();
            }
            catch (Exception ex)
            {
                Ultility.LogException(ex, null);
            }
        }

        private void btnFindNext_Click(object sender, EventArgs e)
        {
            try
            {
                FindNext FindNext = new FindNext();
                FindNext.Run();
            }
            catch (Exception ex)
            {
                Ultility.LogException(ex, null);
            }
        }

        private void btnReplace_Click(object sender, EventArgs e)
        {
            try
            {
                Replace Replace = new Replace();
                Replace.Run();
            }
            catch (Exception ex)
            {
                Ultility.LogException(ex, null);
            }
        }

        private void CheckInsMode()
        {
            if (this.CurrentEditor == null)
                return;

            if (CurrentEditor.ActiveTextAreaControl.Caret.CaretMode == CaretMode.OverwriteMode)
                stKeyMod.Text = "OVR";
            else
                stKeyMod.Text = "INS";
        }

        private void editorControl_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Insert)
                this.CheckInsMode();

            Caret_Change(sender, e);
        }

        private void mnuPrint_Click(object sender, EventArgs e)
        {
            this.Print();
        }

        private void Print()
        {
            try
            {
                if (CurrentEditorTabItem != null)
                {
                    PrintDialog pdlg = new PrintDialog();
                    pdlg.Document = this.CurrentEditorTabItem.PrintDocument;
                    pdlg.AllowSomePages = true;
                    if (pdlg.ShowDialog(this) == DialogResult.OK)
                        pdlg.Document.Print();
                }
            }
            catch (Exception ex)
            {
                Ultility.LogException(ex, null);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            this.Print();
        }

        /// <summary>
        /// The current editor showed on FireEdit
        /// </summary>
        public SharpDevelopTextAreaControl CurrentEditor
        {
            get
            {
                if (Common.Ultility.Ultility.IsUnixOS)
                {
                    if (CurrentActiveTab != null)
                        return CurrentActiveTab.CodeEditor;

                    return null;
                }

                if (this.DockContainer.ActiveDocument == null || !(DockContainer.ActiveDocument is EditorTabItem))
                    return null;

                return ((EditorTabItem)this.DockContainer.ActiveDocument).CodeEditor;
            }
        }

        #endregion

        #region Enabled Disable Button
        private void DisableAllControls()
        {
            this.Cursor = Cursors.WaitCursor;
            this.FunctionalToolBar.Enabled = false;
            MenuStrip.Enabled = false;
        }

        private void EnableAllControls()
        {
            this.FunctionalToolBar.Enabled = true;
            MenuStrip.Enabled = true;
            this.Cursor = Cursors.Default;
        }
        #endregion

        private void MenuButton_Import_Click(object sender, EventArgs e)
        {
            OpenFileDialog opf = new OpenFileDialog();
            opf.Filter = "UML Model (*.xmi)|*.xmi|UML Model (*.uml)|*.uml|All File (*.*)|*.*";
            opf.InitialDirectory = Common.Ultility.Ultility.ExampleFolderPath;
            if (opf.ShowDialog() == DialogResult.OK)
            {
                StreamReader swr = new StreamReader(opf.FileName, System.Text.Encoding.UTF8);
                string PromelaModel = swr.ReadToEnd();
                swr.Close();
                EditorTabItem tabItem = AddDocument("CSP Model");

                if (PromelaModel.Trim() == "")
                {
                    MessageBox.Show(Resources.The_Promela_model_is_empty_, Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DisableAllControls();
                SpecificationBase spec = null;
                try
                {
                    //clear the error list
                    if (!ErrorListWindow.IsDisposed)
                        ErrorListWindow.Clear();

                    string moduleName = tabItem.ModuleName;
                    if (LoadModule(moduleName))
                    {
                        string modelType = PAT.Common.Classes.Ultility.Ultility.PROMELA;
                        Stopwatch t = new Stopwatch();
                        t.Start();
                        if (opf.FileName.EndsWith("xmi"))
                        {
                            modelType = PAT.Common.Classes.Ultility.Ultility.UML;
                            spec = CurrentModule.ParseSpecification(opf.FileName, modelType, tabItem.FileName);
                        }
                        else
                        {
                            spec = CurrentModule.ParseSpecification(tabItem.Text, modelType, tabItem.FileName);
                        }

                        t.Stop();

                        if (spec != null)
                        {
                            if (modelType == PAT.Common.Classes.Ultility.Ultility.PROMELA)
                            {
                                this.StatusLabel_Status.Text = Resources.Promela_Model_Translated;
                            }
                            else
                            {
                                this.StatusLabel_Status.Text = Resources.UML_Model_Translated;
                                if (enableSpecificationOutputToolStripMenuItem.Checked)
                                    tabItem.Text = spec.GetSpecification();
                            }
                            tabItem.Specification = spec;

                            if (spec.Errors.Count > 0)
                            {
                                string key = "";
                                foreach (KeyValuePair<string, ParsingException> pair in spec.Errors)
                                {
                                    key = pair.Key;
                                    break;
                                }
                                ParsingException parsingException = spec.Errors[key];
                                spec.Errors.Remove(key);
                                throw parsingException;
                            }

                            {
                                this.StatusLabel_Status.Text = Resources.Grammar_Checked;
                                MenuButton_OutputPanel.Checked = true;
                                if (enableSpecificationOutputToolStripMenuItem.Checked)
                                {
                                    Output_Window.TextBox.Text =
                                        string.Format(Resources.Specification_is_parsed_in__0_s, t.Elapsed.TotalSeconds) +
                                        "\r\n" + spec.GetSpecification() + "\r\n" + Output_Window.TextBox.Text;
                                }
                                else
                                {
                                    Output_Window.TextBox.Text =
                                        string.Format(Resources.Specification_is_parsed_in__0_s, t.Elapsed.TotalSeconds) +
                                        "\r\n" + "\r\n" + Output_Window.TextBox.Text;
                                }

                                Output_Window.Show(DockContainer);

                                if (spec.Warnings.Count > 0)
                                {
                                    this.MenuButton_ErrorList.Checked = true;
                                    ErrorListWindow.AddWarnings(spec.Warnings);

                                }
                            }
                            EnableAllControls();
                        }
                        else
                        {
                            EnableAllControls();
                        }
                    }
                }
                catch (ParsingException ex)
                {
                    EnableAllControls();
                    if (spec != null)
                    {
                        ErrorListWindow.AddWarnings(spec.Warnings);
                        ErrorListWindow.AddErrors(spec.Errors);
                        MenuButton_ErrorList.Checked = true;
                    }


                    //if (ex.Line > 0 && ex.Text != null)
                    {
                        tabItem.HandleParsingException(ex);
                        ErrorListWindow.InsertError(ex);
                    }

                    this.MenuButton_ErrorList.Checked = true;

                    if (ex.Line > 0)
                    {
                        MessageBox.Show(Resources.Parsing_error_at_line_ + ex.Line + Resources._column_ + ex.CharPositionInLine + ": " + ex.Text + "\n" + ex.Message, Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        MenuButton_OutputPanel.Checked = true;
                        this.Output_Window.TextBox.Text = Resources.Parsing_error_at_line_ + ex.Line + Resources._column_ + ex.CharPositionInLine + ": " + ex.Text + "\n" + ex.Message + "\r\n\r\n" + this.Output_Window.TextBox.Text; //"\n" + ex.StackTrace +     
                    }
                    else
                    {
                        MessageBox.Show(Resources.Parsing_error__ + ex.Message, Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        MenuButton_OutputPanel.Checked = true;
                        this.Output_Window.TextBox.Text = Resources.Parsing_error__ + ex.Message + "\r\n\r\n" + this.Output_Window.TextBox.Text;
                    }
                }
                catch (Exception ex)
                {
                    EnableAllControls();
                    MessageBox.Show(Resources.Parsing_error__ + ex.Message, Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    MenuButton_OutputPanel.Checked = true;
                    this.Output_Window.TextBox.Text = Resources.Parsing_error__ + ex.Message + "\n" + ex.StackTrace + "\r\n\r\n" + this.Output_Window.TextBox.Text;
                }
            }
        }

        private void Button_SpecParse_Click(object sender, EventArgs e)
        {
            ParseSpecification(true);
        }

        private void MenuButton_Formatting_Click(object sender, EventArgs e)
        {
            if (CurrentEditorTabItem != null && CurrentEditorTabItem.Text.Trim() != "")
            {
                string starting = "sb.AppendLine(\"";
                string ending = "\");";

                string result = starting + CurrentEditorTabItem.Text.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r\n", ending + "\r\n" + starting);
                if (result.EndsWith(starting))
                    result = result.Substring(0, result.Length - starting.Length).TrimEnd('\r', '\n');

                if (!result.EndsWith(ending))
                    result = result + ending;

                MenuButton_OutputPanel.Checked = true;
                Output_Window.TextBox.Text = result + Output_Window.TextBox.Text;
            }
        }

        private void MenuButton_LineCounter_Click(object sender, EventArgs e)
        {
            try
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    if (Directory.Exists(dialog.SelectedPath))
                    {
                        int line = GetLineCounter(dialog.SelectedPath);
                        this.Output_Window.AppendOutput("Total Number of Lines: " + line + "\r\n");
                        MenuButton_OutputPanel.Checked = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Ultility.LogException(ex, null);
            }
        }

        private int GetLineCounter(string path)
        {
            string[] files = Directory.GetFiles(path);
            int LOC = 0;
            foreach (string fileName in files)
            {
                string file = fileName.ToLower();
                if (file.EndsWith(".c") || file.EndsWith(".cpp") || file.EndsWith(".h") || file.EndsWith(".cs") ||
                    file.EndsWith(".java"))
                {
                    string line;
                    int loc = 0;
                    TextReader reader = new StreamReader(fileName);
                    while ((line = reader.ReadLine()) != null)
                        loc++;

                    reader.Close();
                    Output_Window.AppendOutput(fileName + " " + loc + "\r\n");
                    LOC += loc;
                }
            }

            files = Directory.GetDirectories(path);
            foreach (string dirName in files)
                LOC += GetLineCounter(dirName);

            return LOC;
        }

        private SpecificationBase ParseSpecification(bool showVerbolMsg)
        {
            if (CurrentEditorTabItem == null || this.CurrentEditorTabItem.Text.Trim() == "")
            {
                if (showVerbolMsg)
                    MessageBox.Show(Resources.Please_input_a_model_first_, Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);

                return null;
            }

            DisableAllControls();
            SpecificationBase spec = null;
            try
            {
                //clear the error list
                if (!ErrorListWindow.IsDisposed)
                    ErrorListWindow.Clear();

                string moduleName = CurrentEditorTabItem.ModuleName;
                if (LoadModule(moduleName))
                {
                    string option = GetOption();
                    Stopwatch t = new Stopwatch();
                    t.Start();
                    spec = CurrentModule.ParseSpecification(this.CurrentEditorTabItem.Text, option, CurrentEditorTabItem.FileName);
                    t.Stop();

                    if (spec != null)
                    {
                        CurrentEditorTabItem.Specification = spec;
                        if (spec.Errors.Count > 0)
                        {
                            string key = "";
                            foreach (KeyValuePair<string, ParsingException> pair in spec.Errors)
                            {
                                key = pair.Key;
                                break;
                            }

                            ParsingException parsingException = spec.Errors[key];
                            spec.Errors.Remove(key);
                            throw parsingException;
                        }

                        if (showVerbolMsg)
                        {
                            this.StatusLabel_Status.Text = Resources.Grammar_Checked;
                            MenuButton_OutputPanel.Checked = true;
                            if (enableSpecificationOutputToolStripMenuItem.Checked)
                            {
                                Output_Window.TextBox.Text =
                                    string.Format(Resources.Specification_is_parsed_in__0_s, t.Elapsed.TotalSeconds) +
                                    "\r\n" + spec.GetSpecification() + "\r\n" + Output_Window.TextBox.Text;
                            }
                            else
                            {
                                Output_Window.TextBox.Text =
                                    string.Format(Resources.Specification_is_parsed_in__0_s, t.Elapsed.TotalSeconds) +
                                    "\r\n" + "\r\n" + Output_Window.TextBox.Text;
                            }

                            Output_Window.Show(DockContainer);

                            if (spec.Warnings.Count > 0)
                            {
                                this.MenuButton_ErrorList.Checked = true;
                                ErrorListWindow.AddWarnings(spec.Warnings);

                                ShowErrorMessage();
                            }
                        }

                        //Open the translation result .csp file 
                        if (CurrentModule.ModuleName == "MDL Model")
                        {
                            ShowModel(spec.InputModelText, "CSP Model");
                            CurrentModule.ParseSpecification(spec.InputModelText, option, CurrentEditorTabItem.FileName);
                        }

                        if (ModelExplorerWindow != null)
                        {
                            ModelExplorerWindow.DisplayTree(spec);
                            if (ModelExplorerWindow.VisibleState == DockState.DockRightAutoHide)
                            {
                                DockContainer.ActiveAutoHideContent = ModelExplorerWindow;
                            }
                        }

                        EnableAllControls();

                        return spec;
                    }
                    else
                    {
                        EnableAllControls();

                        return null;
                    }
                }
            }
            catch (ParsingException ex)
            {
                EnableAllControls();
                if (showVerbolMsg)
                {
                    if (spec != null)
                    {
                        ErrorListWindow.AddWarnings(spec.Warnings);
                        ErrorListWindow.AddErrors(spec.Errors);
                    }

                    CurrentEditorTabItem.HandleParsingException(ex);
                    ErrorListWindow.InsertError(ex);
                    MenuButton_ErrorList.Checked = true;

                    if (ex.Line > 0)
                    {
                        MessageBox.Show(Resources.Parsing_error_at_line_ + ex.Line + Resources._column_ + ex.CharPositionInLine + ": " + ex.Text + "\nFile: " + ex.DisplayFileName + (string.IsNullOrEmpty(ex.NodeName) ? "" : ", Node: " + ex.NodeName) + "\n" + ex.Message, Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        MenuButton_OutputPanel.Checked = true;
                        this.Output_Window.TextBox.Text = Resources.Parsing_error_at_line_ + ex.Line + Resources._column_ + ex.CharPositionInLine + ": " + ex.Text + "\nFile: " + ex.DisplayFileName + (string.IsNullOrEmpty(ex.NodeName) ? "" : ", Node: " + ex.NodeName) + "\n" + ex.Message + "\r\n\r\n" + this.Output_Window.TextBox.Text; //"\n" + ex.StackTrace +     
                    }
                    else
                    {
                        MessageBox.Show(Resources.Parsing_error__ + ex.Message, Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        MenuButton_OutputPanel.Checked = true;
                        this.Output_Window.TextBox.Text = Resources.Parsing_error__ + ex.Message + "\r\n\r\n" + this.Output_Window.TextBox.Text; //"\n" + ex.StackTrace +     
                    }
                    ShowErrorMessage();
                }

            }
            catch (Exception ex)
            {
                EnableAllControls();
                if (showVerbolMsg)
                {
#if (DEBUG)
                    MessageBox.Show(Resources.Parsing_error__ + ex.Message, Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    MenuButton_OutputPanel.Checked = true;
                    this.Output_Window.TextBox.Text = Resources.Parsing_error__ + ex.Message + "\n" + ex.StackTrace + "\r\n\r\n" + this.Output_Window.TextBox.Text;
#else
                    MessageBox.Show("Unknow Parsing Error!", Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    MenuButton_OutputPanel.Checked = true;
                    this.Output_Window.TextBox.Text = "Unknow Parsing Error!\r\n\r\n" + this.Output_Window.TextBox.Text;
#endif
                }
            }

            return null;
        }

        private void ShowErrorMessage()
        {
            ErrorListWindow.Show(DockContainer);

            if (!Common.Ultility.Ultility.IsWindowsOS)
            {
                foreach (ListViewItem item in ErrorListWindow.ListView.Items)
                {
                    //only show warning
                    if (item.ImageIndex == 0)
                    {
                        MessageBox.Show(item.SubItems[2].Text, Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }

        private string GetOption()
        {
            string option = "";
            option += MenuButton_EnableSimplificationOfTheFormula.Checked ? "" : "l";
            option += MenuButton_EnableOntheflyAutomataSimplification.Checked ? "" : "o";
            option += MenuButton_EnableAPosterioriAutomataSimplification.Checked ? "" : "p";
            option += MenuButton_EnableStronglyConnectedComponentsSimplification.Checked ? "" : "c";
            option += MenuButton_EnableTrickingInAcceptingConditions.Checked ? "" : "a";
            return option;
        }

        private void Button_ModelChecking_Click(object sender, EventArgs e)
        {
            try
            {
#if !ACADEMIC
                if (RegisterationForm.IsValidLicenseAvailable() == Ultility.LicenseType.Invalid)
                {
                    RegisterationForm.ShowExpireMessage();           
                    return;
                }
                else if (RegisterationForm.IsValidLicenseAvailable() == Ultility.LicenseType.Evaluation)
                {
                    RegisterationForm.ShowEvaluationMessage();
                }
#endif
                if (ParseSpecification(true) != null)
                {
                    if (GUIUltility.AUTO_SAVE)
                        Save();

                    CurrentModule.ShowModelCheckingWindow(this.CurrentEditorTabItem.TabText.TrimEnd('*'));
                }
            }
            catch (Exception ex)
            {
                Ultility.LogException(ex, null);
            }
        }

        private void Button_Simulation_Click(object sender, EventArgs e)
        {
            try
            {
#if !ACADEMIC
                if (RegisterationForm.IsValidLicenseAvailable() == Ultility.LicenseType.Invalid)
                {
                    RegisterationForm.ShowExpireMessage();
                    return;
                }
                else if (RegisterationForm.IsValidLicenseAvailable() == Ultility.LicenseType.Evaluation)
                {
                    RegisterationForm.ShowEvaluationMessage();
                }
#endif
                //if the parsing is successful
                if (ParseSpecification(true) != null)
                {
                    if (GUIUltility.AUTO_SAVE)
                        Save();

                    CurrentModule.ShowSimulationWindow(this.CurrentEditorTabItem.TabText.TrimEnd('*'));
                }
            }
            catch (Exception ex)
            {
                Ultility.LogException(ex, null);
            }
        }

        private void MenuButton_VerificationBatch_Click(object sender, EventArgs e)
        {
            try
            {
                ModelCheckingBatchForm form = new ModelCheckingBatchForm();
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                Ultility.LogException(ex, null);
            }
        }

        private void btnLaTex_Click(object sender, EventArgs e)
        {
            try
            {
                LaTexPrintingDialog dialog = new LaTexPrintingDialog();
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string latex = "";
                    if (dialog.DirectGeneration)
                    {
                        latex += Common.Ultility.Ultility.GenerateLaTexStringFromModel(this.CurrentEditorTabItem.Text, dialog.DisplayLineNumber);
                    }
                    else
                    {
                        //if the parsing is successful
                        if (ParseSpecification(false) != null)
                        {
                            if (GUIUltility.AUTO_SAVE)
                                Save();

                            latex += CurrentModule.PrintLaTexString();
                        }
                        else
                            return;
                    }

                    DataStoreWindow form = new DataStoreWindow();
                    form.Text = "LaTex ";
                    form.Size = new Size(600, 600);
                    form.StartPosition = FormStartPosition.CenterScreen;
                    form.TextBox.Text = latex;
                    form.TextBox.ReadOnly = false;
                    form.Icon = Icon.FromHandle(((Bitmap)this.ToolbarButton_LaTexGenerate.Image).GetHicon());
                    form.Show();
                }
            }
            catch (Exception ex)
            {
                Ultility.LogException(ex, null);
            }
        }

        private void Toolbar_Button_Click(object sender, EventArgs e)
        {
            try
            {
                //if the parsing is successful
                if (ParseSpecification(true) != null)
                {
                    if (GUIUltility.AUTO_SAVE)
                        Save();

                    CurrentModule.PerformButtonClick(this.CurrentEditorTabItem.TabText.TrimEnd('*'), (sender as ToolStripButton).Text);
                }
            }
            catch (Exception ex)
            {
                Ultility.LogException(ex, null);
            }
        }

        private void ShowModel(string inputModel, string module)
        {
            this.DisableAllControls();
            try
            {
                EditorTabItem tabItem = AddDocument(module);
                tabItem.Text = inputModel;
                tabItem.TabText = tabItem.TabText.TrimEnd('*');

                this.LoadModule(module);
            }
            catch (Exception ex)
            {
                Ultility.LogException(ex, null);
            }
            this.EnableAllControls();
        }

        private void showResultPanelToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            try
            {
                if (MenuButton_OutputPanel.Checked)
                {
                    if (Output_Window.IsDisposed)
                    {
                        Output_Window = new OutputDockingWindow();
                        Output_Window.Icon = Ultility.GetIcon("Output");
                        Output_Window.Disposed += new EventHandler(output_Window_Disposed);
                        ChangeFormCulture.ChangeForm(Output_Window, Thread.CurrentThread.CurrentUICulture.Name);
                    }
                    Output_Window.Show(DockContainer);
                }
                else
                    Output_Window.Hide();
            }
            catch (Exception ex)
            {
                Ultility.LogException(ex, null);
            }
        }

        private void showErrorListToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            try
            {
                if (MenuButton_ErrorList.Checked)
                {
                    if (ErrorListWindow.IsDisposed)
                    {
                        ErrorListWindow = new ErrorListWindow();
                        ErrorListWindow.Icon = Ultility.GetIcon("ErrorList");
                        ErrorListWindow.Disposed += new EventHandler(ErrorListWindow_Disposed);
                        ErrorListWindow.ListView.DoubleClick += new EventHandler(ErrorListView_DoubleClick);
                        ChangeFormCulture.ChangeForm(ErrorListWindow, Thread.CurrentThread.CurrentUICulture.Name);
                    }
                    ErrorListWindow.Show(DockContainer);
                }
                else
                {
                    try
                    {
                        ErrorListWindow.Hide();
                    }
                    catch (Exception) { }
                }
            }
            catch (Exception ex)
            {
                Ultility.LogException(ex, null);
            }
        }

        private void MenuButton_ModelExplorer_CheckStateChanged(object sender, EventArgs e)
        {
            try
            {
                if (MenuButton_ModelExplorer.Checked)
                {
                    if (ModelExplorerWindow.IsDisposed)
                    {
                        ModelExplorerWindow = new ModelExplorerWindow();
                        ModelExplorerWindow.Icon = Ultility.GetIcon("ModelExplorer");
                        ModelExplorerWindow.Disposed += new EventHandler(ErrorListWindow_Disposed);
                        ModelExplorerWindow.TreeView_Model.NodeMouseDoubleClick += TreeView_Model_NodeMouseDoubleClick;
                        ModelExplorerWindow.Button_Refresh.Click += new EventHandler(Button_SpecParse_Click);
                        ChangeFormCulture.ChangeForm(ModelExplorerWindow, Thread.CurrentThread.CurrentUICulture.Name);
                    }
                    ModelExplorerWindow.Show(DockContainer);
                }
                else
                {
                    try
                    {
                        ModelExplorerWindow.Hide();
                    }
                    catch (Exception) { }
                }
            }
            catch (Exception ex)
            {
                Ultility.LogException(ex, null);
            }
        }

        private void MenuButton_FindUsages_CheckStateChanged(object sender, EventArgs e)
        {
            try
            {
                if (MenuButton_FindUsages.Checked)
                {
                    if (FindUsagesWindow.IsDisposed)
                    {
                        FindUsagesWindow = new FindUsagesWindow();
                        FindUsagesWindow.Icon = Common.Ultility.Ultility.GetIcon("Find Usages");
                        FindUsagesWindow.Disposed += new EventHandler(FindUsagesWindow_Disposed);
                        FindUsagesWindow.ListView.DoubleClick += new EventHandler(FindUsagesWindow_DoubleClick);
                        FindUsagesWindow.Button_Refresh.Click += new EventHandler(Button_Refresh_Click);

                        ChangeFormCulture.ChangeForm(FindUsagesWindow, Thread.CurrentThread.CurrentUICulture.Name);
                    }

                    FindUsagesWindow.Show(DockContainer);
                }
                else
                {
                    try
                    {
                        FindUsagesWindow.Hide();
                    }
                    catch (Exception) { }
                }
            }
            catch (Exception ex)
            {
                Ultility.LogException(ex, null);
            }
        }

        private void ErrorListView_DoubleClick(object sender, EventArgs e)
        {
            if (ErrorListWindow.ListView.SelectedItems.Count > 0)
            {
                if (ErrorListWindow.ListView.SelectedItems[0].Tag != null && ErrorListWindow.ListView.SelectedItems[0].Tag is ParsingException)
                {
                    ParsingException ex = ErrorListWindow.ListView.SelectedItems[0].Tag as ParsingException;
                    OpenException(ex);
                }
            }
        }

        private void OpenException(ParsingException ex)
        {
            try
            {
                if (string.IsNullOrEmpty(ex.FileName))
                {
                    if (CurrentEditorTabItem != null)
                        this.CurrentEditorTabItem.HandleParsingException(ex);
                }
                else
                {
                    EditorTabItem item1 = OpenFile(ex.FileName, false);
                    if (item1 != null)
                        item1.HandleParsingException(ex);
                }
            }
            catch (Exception) { }
        }

        private void FindUsagesWindow_DoubleClick(object sender, EventArgs e)
        {
            if (FindUsagesWindow.ListView.SelectedItems.Count > 0)
            {
                if (FindUsagesWindow.ListView.SelectedItems[0].Tag != null && FindUsagesWindow.ListView.SelectedItems[0].Tag is ParsingException)
                {
                    ParsingException ex = FindUsagesWindow.ListView.SelectedItems[0].Tag as ParsingException;
                    OpenException(ex);
                }
            }
        }

        private void TreeView_Model_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Tag != null && e.Node.Tag is ParsingException)
            {
                ParsingException ex = e.Node.Tag as ParsingException;
                OpenException(ex);
            }
        }

        private void output_Window_Disposed(object sender, EventArgs e)
        {
            MenuButton_OutputPanel.Checked = false;

            if ((DockContainer.ActiveDocument as Control) != null)
            {
                (DockContainer.ActiveDocument as Control).Hide();
                (DockContainer.ActiveDocument as Control).Show();
            }
        }

        private void ErrorListWindow_Disposed(object sender, EventArgs e)
        {
            MenuButton_ErrorList.Checked = false;
            if ((DockContainer.ActiveDocument as Control) != null)
            {
                (DockContainer.ActiveDocument as Control).Hide();
                (DockContainer.ActiveDocument as Control).Show();
            }
        }

        private void ModelExplorerWindowWindow_Disposed(object sender, EventArgs e)
        {
            MenuButton_ModelExplorer.Checked = false;
            if ((DockContainer.ActiveDocument as Control) != null)
            {
                (DockContainer.ActiveDocument as Control).Hide();
                (DockContainer.ActiveDocument as Control).Show();
            }
        }

        private void FindUsagesWindow_Disposed(object sender, EventArgs e)
        {
            MenuButton_FindUsages.Checked = false;
            if ((DockContainer.ActiveDocument as Control) != null)
            {
                (DockContainer.ActiveDocument as Control).Hide();
                (DockContainer.ActiveDocument as Control).Show();
            }
        }

        private void tabItem_FindUsages(string name, List<ParsingException> usages, SpecificationBase spec)
        {
            MenuButton_FindUsages.Checked = true;
            try
            {
                int N;
                foreach (ParsingException parsingException in usages)
                {
                    N = DockContainer.Documents.Length;
                    for (int i = 0; i < N; i++)
                    {
                        EditorTabItem item = DockContainer.Documents[i] as EditorTabItem;
                        if (item != null)
                        {
                            if (item.FileName == parsingException.FileName || (!Common.Ultility.Ultility.IsWindowsOS && item.FileName.EndsWith(parsingException.FileName)))
                            {
                                parsingException.Source = item.GetLineText(parsingException.Line - 1);
                                break;
                            }
                        }
                    }
                }

                ParsingException declarationToken = spec.DeclaritionTable[name].DeclarationToken;
                N = DockContainer.Documents.Length;
                for (int i = 0; i < N; i++)
                {
                    EditorTabItem item = DockContainer.Documents[i] as EditorTabItem;
                    if (item != null)
                    {
                        if (item.FileName == declarationToken.FileName || (!Common.Ultility.Ultility.IsWindowsOS && item.FileName.EndsWith(declarationToken.FileName)))
                        {
                            declarationToken.Source = item.GetLineText(declarationToken.Line - 1);
                            break;
                        }
                    }
                }
            }
            catch (Exception) { }

            FindUsagesWindow.FillData(name, usages, spec);
            if (FindUsagesWindow.VisibleState == DockState.DockBottomAutoHide)
                this.DockContainer.ActiveAutoHideContent = FindUsagesWindow;
        }

        private void Button_Refresh_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(FindUsagesWindow.Term))
            {
                SpecificationBase spec = this.ParseSpecification(false);
                if (spec != null)
                {
                    List<ParsingException> exp = spec.FindUsages(FindUsagesWindow.Term);
                    tabItem_FindUsages(FindUsagesWindow.Term, exp, spec);
                }
            }
        }

        #region Toolbar Buttons Functions

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Common.Ultility.Ultility.IsWindowsOS)
                    Process.Start("http://www.comp.nus.edu.sg/~pat/OnlineHelp");
                else
                {
                    string path = Path.Combine(Path.Combine(Common.Ultility.Ultility.APPLICATION_PATH, "Docs"), "Help.chm");
                    if (File.Exists(path))
                        Process.Start(path);
                }
            }
            catch (Exception ex)
            {
                Ultility.LogException(ex, null);
            }
        }

        private void WebsiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("http://www.patroot.com/");
            }
            catch (Exception ex)
            {
                Ultility.LogException(ex, null);
            }
        }

        private void toolStripMenuItem_Support_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("mailto:pat@comp.nus.edu.sg");
            }
            catch (Exception ex)
            {
                Ultility.LogException(ex, null);
            }
        }

        private void mnuAbout_Click(object sender, EventArgs e)
        {
            try
            {
                AboutBox b = new AboutBox();
                b.ShowDialog();
            }
            catch (Exception ex)
            {
                Ultility.LogException(ex, null);
            }
        }

        private void checkForUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                GUIUltility.CheckAndUpdatePAT(true);
            }
            catch (Exception ex)
            {
                Ultility.LogException(ex, null);
            }
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                OptionForm oForm = new OptionForm();
                oForm.ShowDialog();
            }
            catch (Exception ex)
            {
                Ultility.LogException(ex, null);
            }
        }

        private void mathLibaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CSharpLibraryForm oForm = new CSharpLibraryForm();
                oForm.ShowDialog();
            }
            catch (Exception ex)
            {
                Ultility.LogException(ex, null);
            }
        }

        private void MenuButton_LTL2BA_Click(object sender, EventArgs e)
        {
            try
            {
                LTL2AutoamataConverter oForm = new LTL2AutoamataConverter("", GetOption());
                oForm.Show();
            }
            catch (Exception ex)
            {
                Ultility.LogException(ex, null);
            }
        }

        private void alwaysOnTopToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            try
            {
                if (MenuButton_AlwaysOnTop.Checked)
                    this.TopMost = true;
                else
                    this.TopMost = false;
            }
            catch (Exception ex)
            {
                Ultility.LogException(ex, null);
            }
        }

        private void statusBarToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            this.StatusBar.Visible = MenuButton_StatusBar.Checked;
        }

        private void standardToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            this.StandardToolBar.Visible = MenuButton_Standard.Checked;
        }

        private void lineNumberToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            this.CurrentEditor.ShowLineNumbers = MenuButton_LineNumber.Checked;
        }

        private void functionalToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            FunctionalToolBar.Visible = MenuButton_Specification.Checked;
        }

        private void hignlightCurrentLineToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            if (CurrentEditor != null)
                CurrentEditor.LineViewerStyle = (MenuButton_HignlightCurrentLine.Checked) ? LineViewerStyle.FullRow : LineViewerStyle.None;
        }

        private void gotoLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                GotoLineForm GotoLineNumber = new GotoLineForm(CurrentEditor);
                GotoLineNumber.ShowDialog();
            }
            catch (Exception ex)
            {
                Ultility.LogException(ex, null);
            }
        }

        private void CommentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CommentRegion CommentRegion = new CommentRegion();
                CommentRegion.Run();
            }
            catch (Exception ex)
            {
                Ultility.LogException(ex, null);
            }
        }

        private void UnCommentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CommentRegion CommentRegion = new CommentRegion();
                CommentRegion.Run();
            }
            catch (Exception ex)
            {
                Ultility.LogException(ex, null);
            }
        }
        #endregion

        private void whatsNewInPAT22ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("http://www.comp.nus.edu.sg/~pat/#[[Version History]]");
            }
            catch (Exception ex)
            {
                MessageBox.Show(Resources.Exception_happened__ + ex.Message + "\r\n" + ex.StackTrace,
                                Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void navigtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int N = DockContainer.Documents.Length;
            for (int i = 0; i < N; i++)
            {
                EditorTabItem item = DockContainer.Documents[i] as EditorTabItem;
                if (item != null && item == CurrentEditorTabItem)
                {
                    if (i == 0)
                        item = DockContainer.Documents[N - 1] as EditorTabItem;
                    else
                        item = DockContainer.Documents[i - 1] as EditorTabItem;

                    item.Activate();
                    CurrentActiveTab = item;
                    return;
                }
            }
            CurrentActiveTab = null;
        }

        private void navigateToNextTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int N = DockContainer.Documents.Length;
            for (int i = 0; i < N; i++)
            {
                EditorTabItem item = DockContainer.Documents[i] as EditorTabItem;
                if (item != null && item == CurrentEditorTabItem)
                {
                    item = DockContainer.Documents[(i + 1) % N] as EditorTabItem;
                    item.Activate();
                    CurrentActiveTab = item;
                    return;
                }
            }

            CurrentActiveTab = null;
        }

        private void FormMain_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void FormMain_DragDrop(object sender, DragEventArgs e)
        {
            // Can only drop files, so check
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                return;

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files)
                this.OpenFile(file, true);
        }

        private void FormMain_MouseUp(object sender, MouseEventArgs e)
        {
            this.DoDragDrop(sender, DragDropEffects.Link);
        }

        private void LanguageChangedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                this.Hide();

                GUIUltility.GUI_LANGUAGE = (sender as ToolStripMenuItem).Tag.ToString();
                ChangeFormCulture.ChangeAllForms(GUIUltility.GUI_LANGUAGE);

                //check the correct language manu
                foreach (ToolStripMenuItem item in this.MenuButton_Languages.DropDownItems)
                    item.Checked = false;

                (sender as ToolStripMenuItem).CheckState = CheckState.Checked;
                GUIUltility.SaveSettingValue();
                SetFormMainCaption();

                this.Show();
            }
            catch (Exception ex)
            {
                Ultility.LogException(ex, null);
            }
        }

        private void MenuButton_ToggleBookmark_Click(object sender, EventArgs e)
        {
            try
            {
                ToggleBookmark ToggleBookmark = new ToggleBookmark();
                ToggleBookmark.Run();
            }
            catch (Exception ex)
            {
                Ultility.LogException(ex, null);
            }
        }

        private void MenuButton_PreviousBookmark_Click(object sender, EventArgs e)
        {
            try
            {
                PrevBookmark PrevBookmark = new PrevBookmark();
                PrevBookmark.Run();
            }
            catch (Exception ex)
            {
                Ultility.LogException(ex, null);
            }
        }

        private void MenuButton_NextBookmark_Click(object sender, EventArgs e)
        {
            try
            {
                NextBookmark NextBookmark = new NextBookmark();
                NextBookmark.Run();
            }
            catch (Exception ex)
            {
                Ultility.LogException(ex, null);
            }
        }

        private void MenuButton_ClearAllBookmarks_Click(object sender, EventArgs e)
        {
            try
            {
                ClearBookmarks ClearBookmarks = new ClearBookmarks();
                ClearBookmarks.Run();
            }
            catch (Exception ex)
            {
                Ultility.LogException(ex, null);
            }
        }

        private void MenuButton_ToggleOutline_Click(object sender, EventArgs e)
        {
            try
            {
                ToggleFolding ToggleFolding = new ToggleFolding();
                ToggleFolding.Run();
            }
            catch (Exception ex)
            {
                Ultility.LogException(ex, null);
            }
        }

        private void MenuButton_ToggleAllOutlines_Click(object sender, EventArgs e)
        {
            try
            {
                ToggleAllFoldings ToggleAllFoldings = new ToggleAllFoldings();
                ToggleAllFoldings.Run();
            }
            catch (Exception ex)
            {
                Ultility.LogException(ex, null);
            }
        }

        private void SplittoolStripMenuItem_Click(object sender, EventArgs e)
        {
            SplitWindow();
        }

        private void MenuButton_Split_Click(object sender, EventArgs e)
        {
            SplitWindow();
        }

        private void SplitWindow()
        {
            try
            {
                if (this.CurrentEditorTabItem != null)
                    CurrentEditorTabItem.SplitWindow();
            }
            catch (Exception ex)
            {
                Ultility.LogException(ex, null);
            }
        }

        private void ToolbarButton_ZoomIn_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.CurrentEditorTabItem != null)
                    CurrentEditorTabItem.ZoomIn();
            }
            catch (Exception ex)
            {
                Ultility.LogException(ex, null);
            }
        }

        private void ToolbarButton_Zoom100_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.CurrentEditorTabItem != null)
                    CurrentEditorTabItem.Zoom100();
            }
            catch (Exception ex)
            {
                Ultility.LogException(ex, null);
            }
        }

        private void ToolbarButton_ZoomOut_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.CurrentEditorTabItem != null)
                    CurrentEditorTabItem.ZoomOut();
            }
            catch (Exception ex)
            {
                Ultility.LogException(ex, null);
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolbarButton_New.PerformButtonClick();
        }

        private void MenuButton_Register_Click(object sender, EventArgs e)
        {
            RegisterationForm form = new RegisterationForm();
            form.ShowDialog();
        }

        private void MenuButton_ModuleGenerator_Click(object sender, EventArgs e)
        {
            ModuleGeneratorForm form = new ModuleGeneratorForm();
            form.ShowDialog();
        }

        private void stNewVersion_Click(object sender, EventArgs e)
        {
            try
            {
                GUIUltility.UpdatePAT(true);
            }
            catch (Exception) { }
        }

        private void enableSpecificationOutputToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            PAT.Common.Classes.Ultility.Ultility.ENABLE_PARSING_OUTPUT =
                enableSpecificationOutputToolStripMenuItem.Checked;
            GUIUltility.SaveSettingValue();
        }
    }
}