using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Fireball.Docking;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using PAT.Common;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.GUI.Drawing;
using PAT.Common.GUI.PDDLModule;
using PAT.GUI.EditingFunction.CodeCompletion;
using Tools.Diagrams;
using PAT.GUI.Docking;
using CanvasItemData = PAT.Common.GUI.Drawing.LTSCanvas.CanvasItemData;
using PAT.Common.Ultility;

namespace PAT.GUI.PDDLDrawing
{
    public class PDDLTabItem : EditorTabItem
    {
        private IContainer components;
        private SplitContainer splitContainer1;
        private ToolStripContainer toolStripContainer1;
        private ToolStrip toolStrip1;
        private ToolStripSeparator toolStripSeparator3;
        private ImageList imageList2;
       
        private ContextMenuStrip contextMenuStrip2;
        private ToolStripMenuItem openWindowsExplorerToolStripMenuItem;
        private ToolStripMenuItem openModelInWindowsExplorerToolStripMenuItem;
        private ToolStripMenuItem changeFunctionNameToolStripMenuItem;
        private ToolStripMenuItem deleteFunctionToolStripMenuItem;

        private ToolStripMenuItem addFunctionToolStripMenuItem;
        private ToolStripMenuItem importDomainFileToolStripMenuItem;

        private TreeView TreeView_Structure;
        private TreeNode ProblemsNode;
        private TreeNode DomainNode;

        public PDDLTabItem(string moduleName)
        {
            InitializeComponent();
            AddEventHandlerForButtons();

            toolStripContainer1.Visible = false;

            textEditorControl = new SharpDevelopTextAreaControl();
            textEditorControl.Dock = DockStyle.Fill;
            textEditorControl.ContextMenuStrip = EditorContextMenuStrip;
            textEditorControl.BorderStyle = BorderStyle.Fixed3D;
            textEditorControl.Visible = true;

            splitContainer1.Panel2.Controls.Add(textEditorControl);

            TabText = "Document " + counter;
            counter++;

            textEditorControl.FileNameChanged += _EditorControl_FileNameChanged;
            textEditorControl.TextChanged += textEditorControl_TextChanged;
            textEditorControl.Tag = this;


            Padding = new Padding(2, 2, 2, 2);
            DockableAreas = DockAreas.Document;

            secondaryViewContentCollection = new SecondaryViewContentCollection(this);
            InitFiles();

            file = FileService.CreateUntitledOpenedFile(TabText, new byte[] { });
            file.CurrentView = this;
            textEditorControl.FileName = file.FileName;
            files.Clear();
            files.Add(file);

            SetSyntaxLanguage(moduleName);

            textEditorControl.Document.FoldingManager.FoldingStrategy = new FoldingStrategy();

            // Highlight the matching bracket or not...
            textEditorControl.ShowMatchingBracket = true;
            textEditorControl.BracketMatchingStyle = BracketMatchingStyle.Before;


            HostCallbackImplementation.Register(this);
            CodeCompletionKeyHandler.Attach(this, textEditorControl);
            ToolTipProvider.Attach(this, textEditorControl);

            pcRegistry = new ProjectContentRegistry(); // Default .NET 2.0 registry

            // Persistence lets SharpDevelop.Dom create a cache file on disk so that
            // future starts are faster.
            // It also caches XML documentation files in an on-disk hash table, thus
            // reducing memory usage.
            pcRegistry.ActivatePersistence(Path.Combine(Path.GetTempPath(), "CSharpCodeCompletion"));

            myProjectContent = new DefaultProjectContent();
            myProjectContent.Language = LanguageProperties.CSharp;

            TreeView_Structure.HideSelection = false;
            splitContainer1.SplitterDistance = 100;
            TreeView_Structure.ExpandAll();
            ProblemsNode.ExpandAll();
        }

        private void InitTextEditor()
        {
            textEditorControl.Visible = true;
            if (DomainNode.Tag != null)
            {
                textEditorControl.Text = (DomainNode.Tag as PDDLFile).Content;
                TextEditorChanged = false;
            }
            else
            {
                textEditorControl.Text = "";
            }

            TreeView_Structure.SelectedNode = TreeView_Structure.Nodes[0];
            
            SetDirty();
        }

        private void AddEventHandlerForButtons()
        {
            foreach (ToolStripItem button in toolStrip1.Items)
            {
                if (button is ToolStripButton)
                {
                    ((ToolStripButton)button).CheckStateChanged += button_CheckStateChanged;
                }
            }
        }

        private void contextMenuStrip2_Opening(object sender, CancelEventArgs e)
        {
            openWindowsExplorerToolStripMenuItem.Enabled = false;
            openWindowsExplorerToolStripMenuItem.Visible = false;

            importDomainFileToolStripMenuItem.Enabled = false;
            importDomainFileToolStripMenuItem.Visible = false;

            openModelInWindowsExplorerToolStripMenuItem.Enabled = false;
            openModelInWindowsExplorerToolStripMenuItem.Visible = false;

            changeFunctionNameToolStripMenuItem.Enabled = false;
            changeFunctionNameToolStripMenuItem.Visible = false;

            deleteFunctionToolStripMenuItem.Enabled = false;
            deleteFunctionToolStripMenuItem.Visible = false;

            addFunctionToolStripMenuItem.Enabled = false;
            addFunctionToolStripMenuItem.Visible = false;

            if (TreeView_Structure.SelectedNode != null)
            {
                var selectedNode = TreeView_Structure.SelectedNode;
                if(selectedNode.Parent != null
                    && selectedNode.Parent.Name == Parsing.PROBLEMS_NODE_NAME)
                {//selected node is a function node
                    openWindowsExplorerToolStripMenuItem.Enabled = true;
                    openWindowsExplorerToolStripMenuItem.Visible = true;

                    deleteFunctionToolStripMenuItem.Enabled = true;
                    deleteFunctionToolStripMenuItem.Visible = true;
                }
                else if (selectedNode.Name == Parsing.DOMAIN_NODE_NAME)
                {//the domain node is selected
                    openWindowsExplorerToolStripMenuItem.Enabled = true;
                    openWindowsExplorerToolStripMenuItem.Visible = true;

                    importDomainFileToolStripMenuItem.Enabled = true;
                    importDomainFileToolStripMenuItem.Visible = true;
                }
                else if (selectedNode.Name == Parsing.PROBLEMS_NODE_NAME)
                {
                    addFunctionToolStripMenuItem.Enabled = true;
                    addFunctionToolStripMenuItem.Visible = true;
                    openModelInWindowsExplorerToolStripMenuItem.Enabled = true;
                    openModelInWindowsExplorerToolStripMenuItem.Visible = true;
                }
            }
        }

        //openModelInWindowsExplorerToolStripMenuItem_Click
        private void openModelInWindowsExplorerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string appPath = fileName;

            if(!(string.IsNullOrEmpty(appPath)) && Path.IsPathRooted(appPath))
            {
                if (File.Exists(appPath))
                {
                    appPath = Path.GetDirectoryName(appPath);
                    Process.Start(@appPath);
                }else
                {
                    MessageBox.Show("File + " + appPath + "doesn't exist!", "Error!",
                     MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }else
            {
                MessageBox.Show("Please save the model file first!", "Error!",
                     MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

           
        }

        private void importDomainFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog opf = new OpenFileDialog();
            opf.Multiselect = false;
            opf.Filter = "PDDL File (*.pddl)|*.pddl|All File (*.*)|*.*";
            opf.Title = "Choose the PDDL file to be imported as DOMAIN";

            if (Path.IsPathRooted(FileName))
                opf.InitialDirectory = FileName;
            else
            {
                opf.InitialDirectory = Directory.GetCurrentDirectory();
            }

            if (opf.ShowDialog() == DialogResult.OK)
            {
                string pathOfNewPDDL = opf.FileName;
                string fName = Path.GetFileNameWithoutExtension(pathOfNewPDDL);

                StreamReader sReader = null;
                string pddlContent = "";
                try
                {
                    FileStream fileStream = new FileStream(pathOfNewPDDL, FileMode.Open, FileAccess.Read);
                    sReader = new StreamReader(fileStream);
                    pddlContent = sReader.ReadToEnd();
                }
                catch (DirectoryNotFoundException DNF)
                {
                    pddlContent = "";
                }
                catch (FileNotFoundException FNF)
                {
                    pddlContent = "";
                }
                finally
                {
                    if (sReader != null)
                    {
                        sReader.Close();
                    }
                }
                PDDLFile newFile = new PDDLFile(fName, opf.FileName, pddlContent);
                DomainNode.Tag = newFile;
                textEditorControl.Text = pddlContent;
            }
        }

        private void openWindowsExplorerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var pddlFile = TreeView_Structure.SelectedNode.Tag as PDDLFile;

            if(pddlFile == null)
                return;

            var appPath = pddlFile.FilePath;

            if (!(string.IsNullOrEmpty(appPath)) && Path.IsPathRooted(appPath))
            {
                if (File.Exists(appPath))
                {
                    appPath = Path.GetDirectoryName(appPath);
                    Process.Start(@appPath);
                }
                else
                {
                    MessageBox.Show("File + " + appPath + "doesn't exist!", "Error!",
                     MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            else
            {
                MessageBox.Show("Please save the model file first!", "Error!",
                     MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void AddFunctionToolStripMenuItemClick(object sender, EventArgs e)
        {
            OpenFileDialog opf = new OpenFileDialog();
            opf.Multiselect = false;
            opf.Filter = "PDDL File (*.pddl)|*.pddl|All File (*.*)|*.*";
            opf.Title = "Choose the PDDL file to be added";

            if (Path.IsPathRooted(FileName))
                opf.InitialDirectory = FileName;
            else
            {
                opf.InitialDirectory = Directory.GetCurrentDirectory();
            }

            if (opf.ShowDialog() == DialogResult.OK)
            {
                string pathOfNewPDDL = opf.FileName;
                string fName = Path.GetFileNameWithoutExtension(pathOfNewPDDL);

                TreeNode newNode = ProblemsNode.Nodes.Add(fName);
                newNode.Name = fName;
                newNode.Text = fName;

                TreeView_Structure.SelectedNode = newNode;

                StreamReader sReader = null;
                string pddlContent = "";
                  try
                    {
                        FileStream fileStream = new FileStream(pathOfNewPDDL, FileMode.Open, FileAccess.Read);
                        sReader = new StreamReader(fileStream);
                        pddlContent = sReader.ReadToEnd();
                    }
                    catch (DirectoryNotFoundException DNF)
                    {
                        pddlContent = "";
                    }
                    catch (FileNotFoundException FNF)
                    {
                        pddlContent = "";
                    }
                    finally
                    {
                        if (sReader != null)
                        {
                            sReader.Close();
                        }
                    }
                PDDLFile newFunction = new PDDLFile(fName, opf.FileName, pddlContent);
                newNode.Tag = newFunction;
            }
        }

        private void DeleteFunctionToolStripMenuItemClick(object sender, EventArgs e)
        {
            var dName = TreeView_Structure.SelectedNode.Text;
            
            if (MessageBox.Show("Do you want to delete the problem " + dName + "?",
                                    "Yes", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                TreeView_Structure.SelectedNode.Remove();
                SetDirty();
            }

        }

      

        private bool IsDiagramNameDuplicated()
        {
            List<string> names = new List<string>();
            foreach (TreeNode dNode in this.ProblemsNode.Nodes)
            {
                if (names.Contains(dNode.Text))
                {
                    MessageBox.Show("No duplicate diagram names are allowed", "Error", MessageBoxButtons.OK,
                                                                                            MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    return false;
                }
                else
                {
                    names.Add(dNode.Text);
                }
            }
            return true;
        }

        private TreeNode LastSelectedNode;
        private bool TextEditorChanged;

        private void SavePddlContent()
        {
            var pddlFile = LastSelectedNode.Tag as PDDLFile;
            //diagram.XmiContent = textEditorControl.Text;
            pddlFile.Content = textEditorControl.Text;

            if (File.Exists(pddlFile.FilePath))
            {
                try
                {
                    File.WriteAllText(@pddlFile.FilePath, textEditorControl.Text);
                }
                catch (DirectoryNotFoundException)
                {

                }
                catch (FileNotFoundException)
                {

                }
            }
        }

        //todo
        protected new void textEditorControl_TextChanged(object sender, EventArgs e)
        {
            textEditorControl.Document.FoldingManager.UpdateFoldings(null, null);
            TextEditorChanged = true;

            SetDirty();
        }

        #region InitializeComponent
        private void InitializeComponent()
        {
            components = new Container();
            System.ComponentModel.ComponentResourceManager resources = new ComponentResourceManager(typeof(PDDLTabItem));
            ProblemsNode = new TreeNode(Parsing.PROBLEMS_NODE_NAME);
            DomainNode = new TreeNode(Parsing.DOMAIN_NODE_NAME);
            //TreeNode treeNode8 = new TreeNode(Parsing.PDDL_MODEL_NODE_NAME, 
              //      new[] {DomainNode, FunctionsNode});

            DomainNode = new TreeNode(Parsing.DOMAIN_NODE_NAME,
                    new[] { ProblemsNode });

            splitContainer1 = new SplitContainer();
            TreeView_Structure = new TreeView();
            contextMenuStrip2 = new ContextMenuStrip(components);
            openWindowsExplorerToolStripMenuItem = new ToolStripMenuItem();
            importDomainFileToolStripMenuItem = new ToolStripMenuItem();
            openModelInWindowsExplorerToolStripMenuItem = new ToolStripMenuItem();
            addFunctionToolStripMenuItem = new ToolStripMenuItem();
            changeFunctionNameToolStripMenuItem = new ToolStripMenuItem();
            deleteFunctionToolStripMenuItem = new ToolStripMenuItem();

            imageList2 = new ImageList(components);
            toolStripContainer1 = new ToolStripContainer();
            toolStrip1 = new ToolStrip();
            toolStripSeparator3 = new ToolStripSeparator();


            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            contextMenuStrip2.SuspendLayout();
            toolStripContainer1.TopToolStripPanel.SuspendLayout();
            toolStripContainer1.SuspendLayout();
            toolStrip1.SuspendLayout();
            SuspendLayout();
          
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(TreeView_Structure);
            splitContainer1.Panel1MinSize = 120;
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(toolStripContainer1);
            splitContainer1.Panel2MinSize = 100;
            splitContainer1.Size = new Size(617, 399);
            splitContainer1.SplitterDistance = 178;
            splitContainer1.TabIndex = 3;
            // 
            // TreeView_Structure
            // 
            TreeView_Structure.ContextMenuStrip = contextMenuStrip2;
            TreeView_Structure.Dock = DockStyle.Fill;
            TreeView_Structure.HideSelection = false;
            TreeView_Structure.Location = new Point(0, 0);
            TreeView_Structure.Name = "TreeView_Structure";

            DomainNode.Name = Parsing.DOMAIN_NODE_NAME;
            DomainNode.StateImageIndex = 0;
            DomainNode.Text = Parsing.DOMAIN_NODE_NAME;
            DomainNode.Tag = new PDDLFile("", "", "");
            ProblemsNode.Name = Parsing.PROBLEMS_NODE_NAME;
            ProblemsNode.StateImageIndex = 1;
            ProblemsNode.Text = Parsing.PROBLEMS_NODE_NAME;
            DomainNode.Name = Parsing.DOMAIN_NODE_NAME;
            DomainNode.Text = Parsing.DOMAIN_NODE_NAME;
            TreeView_Structure.Nodes.AddRange(new[] { DomainNode });
            TreeView_Structure.Size = new Size(178, 399);
            TreeView_Structure.StateImageList = imageList2;
            TreeView_Structure.TabIndex = 0;
            TreeView_Structure.NodeMouseClick += TreeView_Structure_NodeMouseClick;
            TreeView_Structure.NodeMouseDoubleClick += TreeView_Structure_NodeMouseDoubleClick;
            // 
            // contextMenuStrip2
            // 
            contextMenuStrip2.Items.Add(openWindowsExplorerToolStripMenuItem);
            contextMenuStrip2.Items.Add(importDomainFileToolStripMenuItem);
            contextMenuStrip2.Items.Add(openModelInWindowsExplorerToolStripMenuItem);
            contextMenuStrip2.Items.Add(addFunctionToolStripMenuItem);
            contextMenuStrip2.Items.Add(changeFunctionNameToolStripMenuItem);
            contextMenuStrip2.Items.Add(deleteFunctionToolStripMenuItem);

            contextMenuStrip2.Name = "contextMenuStrip2";
            contextMenuStrip2.Size = new Size(50, 15);
            contextMenuStrip2.Opening += contextMenuStrip2_Opening;
            // 
            // openWindowsExplorerToolStripMenuItem
            // 
            openWindowsExplorerToolStripMenuItem.Name = "openWindowsExplorerToolStripMenuItem";
            openWindowsExplorerToolStripMenuItem.Size = new Size(50, 15);
            openWindowsExplorerToolStripMenuItem.Text = "View File in Windows Explorer";
            openWindowsExplorerToolStripMenuItem.Click += openWindowsExplorerToolStripMenuItem_Click;

            //
            // importDomainFileToolStripMenuItem
            //
            importDomainFileToolStripMenuItem.Name = "importDomainFileToolStripMenuItem";
            importDomainFileToolStripMenuItem.Size = new Size(50, 15);
            importDomainFileToolStripMenuItem.Text = "Import a PDDL domain file";
            importDomainFileToolStripMenuItem.Click += importDomainFileToolStripMenuItem_Click;

            //
            // changeFunctionNameToolStripMenuItem
            //
            changeFunctionNameToolStripMenuItem.Name = "changeDiagramNameToolStripMenuItem";
            changeFunctionNameToolStripMenuItem.Size = new Size(50, 15);
            changeFunctionNameToolStripMenuItem.Text = "Change Name";
            //changeFunctionNameToolStripMenuItem.Click += changeDiagramNameToolStripMenuItem_Click;
            
            //
            // deleteFunctionToolStripMenuItem
            //
            deleteFunctionToolStripMenuItem.Name = "deleteFunctionToolStripMenuItem";
            deleteFunctionToolStripMenuItem.Size = new Size(50, 15);
            deleteFunctionToolStripMenuItem.Text = "Delete this Problem";
            deleteFunctionToolStripMenuItem.Click += DeleteFunctionToolStripMenuItemClick;

            //
            // addFunctionToolStripMenuItem
            //
            addFunctionToolStripMenuItem.Name = "addFunctionToolStripMenuItem";
            addFunctionToolStripMenuItem.Size = new Size(50, 15);
            addFunctionToolStripMenuItem.Text = "Add a PDDL Problem";
            addFunctionToolStripMenuItem.Click += AddFunctionToolStripMenuItemClick;

            //
            //openModelInWindowsExplorerToolStripMenuItem
            //
            openModelInWindowsExplorerToolStripMenuItem.Name = "openModelInWindowsExplorerToolStripMenuItem";
            openModelInWindowsExplorerToolStripMenuItem.Size = new Size(50, 15);
            openModelInWindowsExplorerToolStripMenuItem.Text = "View Model in Windows Explorer";
            openModelInWindowsExplorerToolStripMenuItem.Click += openModelInWindowsExplorerToolStripMenuItem_Click;
            // 
            // imageList2
            // 
            imageList2.ImageStream = ((ImageListStreamer)(resources.GetObject("imageList2.ImageStream")));
            imageList2.TransparentColor = Color.Transparent;
            imageList2.Images.SetKeyName(0, "declare.png");
            imageList2.Images.SetKeyName(1, "channel.png");
            imageList2.Images.SetKeyName(2, "templates.png");
            imageList2.Images.SetKeyName(3, "questionMark.png");
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            toolStripContainer1.ContentPanel.Size = new Size(435, 374);
            toolStripContainer1.Dock = DockStyle.Fill;
            toolStripContainer1.Location = new Point(0, 0);
            toolStripContainer1.Name = "toolStripContainer1";
            toolStripContainer1.Size = new Size(435, 399);
            toolStripContainer1.TabIndex = 3;
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            toolStripContainer1.TopToolStripPanel.Controls.Add(toolStrip1);
            // 
            // toolStrip1
            // 
            toolStrip1.Dock = DockStyle.None;
            toolStrip1.Items.AddRange(new ToolStripItem[] {
            toolStripSeparator3,
            });
            toolStrip1.Location = new Point(3, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(133, 25);
            toolStrip1.TabIndex = 1;
       
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(6, 25);
    
            // 
            // PDDLTabItem
            // 
            ClientSize = new Size(617, 399);
            Controls.Add(splitContainer1);
            Name = "PDDLTabItem";
            //contextMenuStrip1.ResumeLayout(false);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.ResumeLayout(false);
            contextMenuStrip2.ResumeLayout(false);
            toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            toolStripContainer1.TopToolStripPanel.PerformLayout();
            toolStripContainer1.ResumeLayout(false);
            toolStripContainer1.PerformLayout();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);

            //InitializeCanvasMethod();
            //InitializePOROverheadsMethod();

        }
        //private void InitializePOROverheadsMethod()
        //{
        //    string file = Path.Combine(Path.Combine(Common.Ultility.Ultility.ModuleFolderPath, "NESC"), "PAT.Module.NESC.dll");
        //    Assembly assembly = Assembly.LoadFrom(file);
        //    Module[] modules = assembly.GetModules(false);
        //    Type type = null;
        //    Type[] types = modules[0].GetTypes();
        //    if (types != null && types.Length > 0)
        //    {
        //        for (int i = 0; i < types.Length; i++)
        //        {
        //            if (types[i].Name == "Specification")
        //            {
        //                type = types[i];
        //                break;
        //            }
        //        }
        //    }

        //    if (type != null)
        //    {
        //        POROverhead_Method = type.GetMethod("GetStaticAnalysisOverhead");
        //    }

        //}


        //private void InitializeCanvasMethod()
        //{
        //    string file = Path.Combine(Path.Combine(Common.Ultility.Ultility.ModuleFolderPath, "NESC"), "PAT.Module.NESC.dll");
        //    Assembly assembly = Assembly.LoadFrom(file);
        //    Module[] modules = assembly.GetModules(false);
        //    Type type = null;
        //    Type[] types = modules[0].GetTypes();
        //    if(types != null && types.Length > 0)
        //    {
        //        for(int i = 0; i < types.Length; i++)
        //        {
        //            if(types[i].Name == "Specification")
        //            {
        //                type = types[i];
        //                break;
        //            }
        //        }
        //    }
            
        //    if(type != null)
        //    {
        //        GenerateCanvas_Method = type.GetMethod("GenerateSymbolicLTSForSensor");
        //    }

        //}

        //private LTSCanvas GetCanvasOfSensor(string sensorID)
        //{
        //    if(GenerateCanvas_Method != null)
        //    {
        //        var para = new object[3];
        //        para[0] = sensorID;
        //        para[1] = this.Text;
        //        para[2] = this.FileName;
        //        var resultv = GenerateCanvas_Method.Invoke(null, para) as LTSCanvas;
        //        return resultv;
        //    }
        //    return null;
        //}

        void TreeView_Structure_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            {
                if (e.Button == MouseButtons.Right && e.Clicks == 1)
                {
                    TreeView_Structure.SelectedNode = e.Node;

                    if (e.Node.Text != Parsing.DOMAIN_NODE_NAME)
                        contextMenuStrip2.Show(e.Location);
                }
                else
                {

                    if (LastSelectedNode != null && LastSelectedNode.Parent != null
                    && LastSelectedNode.Parent.Text == Parsing.PROBLEMS_NODE_NAME)
                    {//The last selected node is a diagram node
                        if (!(e.Node.Parent != null
                            && e.Node.Parent.Text == LastSelectedNode.Parent.Text
                            && e.Node.Text == LastSelectedNode.Text
                            && e.Node.Name == LastSelectedNode.Name))
                        {//the node clicked is a state diagram node different from the current one
                            if (TextEditorChanged)
                            {
                                TextEditorChanged = false;
                                if (MessageBox.Show("Do you want to save the changes of " + LastSelectedNode.Name + "?",
                                    "Yes", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                {
                                    SavePddlContent();
                                }

                            }

                        }
                    }
                    else if (LastSelectedNode != null
                       && LastSelectedNode.Text == Parsing.DOMAIN_NODE_NAME
                       && e.Node.Text != Parsing.DOMAIN_NODE_NAME)
                    {
                        if (TextEditorChanged)
                        {
                            TextEditorChanged = false;
                            if (MessageBox.Show("Do you want to save the changes of Assertions?",
                                "Yes", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                LastSelectedNode.Tag = textEditorControl.Text;
                                SavePddlContent();
                            }

                        }
                    }

                    LastSelectedNode = e.Node;

                    if (e.Node.Parent != null && e.Node.Parent.Text == Parsing.PROBLEMS_NODE_NAME)
                    {//current node is a 
                        toolStripContainer1.Visible = false;
                        textEditorControl.Visible = false;
                        textEditorControl.TextChanged -= textEditorControl_TextChanged;
                        textEditorControl.Text = (e.Node.Tag as PDDLFile).Content;
                        textEditorControl.TextChanged += textEditorControl_TextChanged;

                        textEditorControl.Visible = true;
                    }
                    else
                    {
                        if (e.Node.Text == Parsing.DOMAIN_NODE_NAME)
                        {
                            toolStripContainer1.Visible = false;
                            textEditorControl.Visible = false;
                            textEditorControl.TextChanged -= textEditorControl_TextChanged;
                            if (DomainNode.Tag != null)
                            {
                                textEditorControl.Text = (DomainNode.Tag as PDDLFile).Content;
                            }
                            else
                            {
                                textEditorControl.Text = "";
                            }
                            textEditorControl.TextChanged += textEditorControl_TextChanged;

                            textEditorControl.Visible = true;
                        }
                    }

                    RaiseIsDirtyChanged();
                }
            }
        }

        private void button_CheckStateChanged(object sender, EventArgs e)
        {
            foreach (ToolStripItem button in toolStrip1.Items)
            {
                ToolStripButton buttonTemp = button as ToolStripButton;
                if (buttonTemp != null && buttonTemp != sender)
                {
                    buttonTemp.Checked = false;
                }
            }
        }

        #endregion
        
        public static bool HighLightControlUsingRed;

        private void TreeView_Structure_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeView_Structure_NodeMouseClick(sender, e);
        }

        public override bool CanUndo
        {
            get
            {
                if (textEditorControl.Visible)
                {
                    return CodeEditor.EnableUndo;
                }
                return false;
            }
        }

        public override bool CanRedo
        {
            get
            {
                if (textEditorControl.Visible)
                {
                    return CodeEditor.EnableRedo;
                }
                return false;

            }
        }

        public override bool CanCut
        {
            get
            {
                if (textEditorControl.Visible)
                {
                    return EnableCut;
                }
                return false;

            }
        }

        public override bool CanCopy
        {
            get
            {
                if (textEditorControl.Visible)
                {
                    return EnableCopy;
                }
                return false;

            }
        }
        public override bool CanPaste
        {
            get
            {
                if (textEditorControl.Visible)
                {
                    return EnablePaste;// textEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.EnablePaste;
                }
                return false;

            }
        }

        public override bool CanSelectAll
        {
            get
            {
                if (textEditorControl.Visible)
                {
                    return CodeEditor.CanSelect;
                }
                return false;

            }
        }

        public override bool CanFind
        {
            get
            {
                if (textEditorControl.Visible)
                {
                    return true;
                }
                return false;

            }
        }

        public override bool CanPrint
        {
            get
            {
                if (textEditorControl.Visible)
                {
                    return true;
                }
                return false;
            }
        }

        public override void Undo()
        {
            if (textEditorControl.Visible)
            {
                CodeEditor.Undo();
            }
        }

        public override void Redo()
        {
            if (textEditorControl.Visible)
            {
                CodeEditor.Redo();
            }
        }

        public override void Cut()
        {

            //CodeEditor.Cut();
            textEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.Cut(null, null);

        }

        public override void Copy()
        {

            //CodeEditor.Copy();
            textEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.Copy(null, null);
        }

        public override void Paste()
        {
            textEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.Paste(null, null);
        }

        public override void SelectAll()
        {

            textEditorControl.ActiveTextAreaControl.TextArea.ClipboardHandler.SelectAll(null, null);

        }

     public override void SplitWindow()
        {
            if (textEditorControl.Visible)
            {
                textEditorControl.Split();
            }
        }

        /// <summary>
        /// To initialize an PDDLTabItem instance from a .pddlx file (in xml format)
        /// </summary>
        /// <param name="text">the text string of the source .pddlx file</param>
        public override void SetText(string text)
        {
            //generate an SNModel instance from the .uml source code
            PDDLModel pddlModel = PDDLModel.LoadPDDLModelFromXML(text);
            
            if (pddlModel.Domain != null)
            {
                DomainNode.Tag = pddlModel.Domain;
            }
            else { DomainNode.Tag = null; }
            

            ProblemsNode.Nodes.Clear();

            foreach (var problem in pddlModel.Problems)
            {
                TreeNode node = ProblemsNode.Nodes.Add(problem.Key);
                node.Tag = problem.Value;
                node.Name = problem.Key;
                node.Text = problem.Key;
            }
            textEditorControl.Visible = true;
            InitTextEditor();
        }

        public override string GetText()
        {
            XmlDocument doc = GetDoc();
            return doc.InnerXml;
        }
        public override string FileName
        {
            get
            {
                return fileName;
            }
        }

        private string fileName;

        /// <summary>
        /// Save the current PDDLTabItem instance as a .pddlx file
        /// </summary>
        /// <param name="filename">file name</param>
        public override void Save(string filename)
        {
            var CurrentNode = TreeView_Structure.SelectedNode;
            if(CurrentNode.Parent != null &&
                CurrentNode.Parent.Text == Parsing.PROBLEMS_NODE_NAME)
            {//current node is a diagram
                if(TextEditorChanged)
                {//save the diagram
                    TextEditorChanged = false;
                    var problem = CurrentNode.Tag as PDDLFile;
                    problem.Content = textEditorControl.Text;
                    SaveFile(problem.FilePath, problem.Content, problem);
                }
            }
            else if (CurrentNode.Text == Parsing.DOMAIN_NODE_NAME)
            {//save the assertion
                if (TextEditorChanged)
                {
                    TextEditorChanged = false;
                    var domain = CurrentNode.Tag as PDDLFile;
                    domain.Content = textEditorControl.Text;
                    SaveFile(domain.FilePath, domain.Content, domain);
                }
            }

            if (!string.IsNullOrEmpty(filename) && filename != fileName)
            {
                fileName = filename;
                string pddlName = Path.GetFileNameWithoutExtension(filename);
                TreeView_Structure.Nodes[0].Name = pddlName;
            }

            XmlDocument doc = GetDoc();
            doc.Save(fileName);

            HaveFileName = true;
        }

        private static void SaveFile(string path, string content, PDDLFile fileObj)
        {
            string pathToSave = path;

            if (string.IsNullOrEmpty(path) || !Path.IsPathRooted(path))
            {
                var getNewPath = MessageBox.Show("File \"" + path + "\" doesn't exist! Do you want to continue to select a path?", 
                    "Yes", MessageBoxButtons.YesNo) == DialogResult.Yes;

                if (getNewPath)
                {
                    SaveFileDialog svd = new SaveFileDialog();

                    svd.Filter = "PDDL File (*.pddl)|*.pddl|All File (*.*)|*.*";

                    if (svd.ShowDialog() == DialogResult.OK)
                    {
                        pathToSave = svd.FileName;
                        fileObj.FilePath = pathToSave;
                        fileObj.Name = Path.GetFileNameWithoutExtension(pathToSave);
                    }

                }
                else
                {
                    MessageBox.Show("The file is not saved!",
                                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            try
            {
                File.WriteAllText(@pathToSave, content);
            }
            catch (DirectoryNotFoundException)
            {

            }
            catch (FileNotFoundException)
            {

            }

            //MessageBox.Show("The file is saved successfully!",
            //                                   "Save File Message", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        //Generate XML from the current PDDLTabItem
        private XmlDocument GetDoc()
        {
            PDDLFile domain = null;

            if (DomainNode.Tag != null)
                domain = DomainNode.Tag as PDDLFile;

            Dictionary<string, PDDLFile> problems = new Dictionary<string, PDDLFile>();
            foreach (TreeNode node in ProblemsNode.Nodes)
            {
                problems.Add(node.Name, node.Tag as PDDLFile);
            }

            PDDLModel pddlModel = new PDDLModel("PDDLModel", problems, domain);

            return pddlModel.GenerateXML();
        }

        public override void Open(string filename)
        {
            ToolTipText = filename;
            TabText = Path.GetFileName(filename);
            HaveFileName = true;
            fileName = filename;

            StreamReader streamReader = new StreamReader(filename);
            string text = streamReader.ReadToEnd();
            streamReader.Close();

            SetText(text);
        }

        public override void HandleParsingException(ParsingException ex)
        {
            try
            {
                if (ex is GraphParsingException)
                {
                    string pname = (ex as GraphParsingException).ProcessName;
                    foreach (TreeNode node in ProblemsNode.Nodes)
                    {
                        if (node.Text == pname)
                        {
                            TreeView_Structure.SelectedNode = node;
                            TreeView_Structure_NodeMouseDoubleClick(node,
                                new TreeNodeMouseClickEventArgs(node, MouseButtons.Left,
                                                                                                    2, 0, 0));
                            break;
                        }
                    }
                }
                else
                {

                    TreeView_Structure.SelectedNode = TreeView_Structure.Nodes[0];
                    TreeView_Structure_NodeMouseDoubleClick(TreeView_Structure.Nodes[0],
                                                            new TreeNodeMouseClickEventArgs(null, MouseButtons.Left,
                                                                                            2, 0, 0));


                    if (ex.Line >= 1 && ex.CharPositionInLine >= 0 && ex.Text != null)
                    {
                        textEditorControl.ActiveTextAreaControl.JumpTo(ex.Line - 1);
                        SelectionManager selectionManager =
                            textEditorControl.ActiveTextAreaControl.TextArea.SelectionManager;
                        selectionManager.ClearSelection();
                        selectionManager.SetSelection(new TextLocation(ex.CharPositionInLine, ex.Line - 1),
                                                      new TextLocation(ex.CharPositionInLine + ex.Text.Length,
                                                                       ex.Line - 1));
                        textEditorControl.Refresh();
                    }
                }
            }
            catch
            {

            }
        }

    }
}
