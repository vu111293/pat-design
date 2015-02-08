using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Antlr.Runtime;
using PAT.Common.Classes.ModuleInterface;
using PAT.Common.GUI;
using PAT.Common.GUI.ModelChecker;

namespace PAT.Common
{
    public delegate void Toolbar_Button_Click_Delegate(object sender, EventArgs e);
    public delegate void ShowModelHandler(string inputModel, string module);

    public abstract class ModuleFacadeBase
    {
        public ShowModelHandler ShowModel;
        protected SpecificationBase Specification;

        public virtual SpecificationBase ParseSpecification(string text, string options, string filePath)
        {
            if (Common.Classes.Ultility.Ultility.GrabSharedDataLock())
            {
                Specification = InstanciateSpecification(text, options, filePath);
                return Specification;
            }
            else
            {
                MessageBox.Show(Resources.Please_stop_verification_or_simulation_before_parsing_the_model_, Common.Ultility.Ultility.APPLICATION_NAME, MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);
                return null;
            }
        }

        

        public virtual void ShowSimulationWindow(string tabName)
        {
            if (Specification != null)
            {
                if (Specification.GetProcessNames().Count > 0)
                {
                    SimulationForm SimulationForm = InstanciateSimulationForm();
                    try
                    {
                        if (SimulationForm != null)
                        {
                            SimulationForm.SetSpec(tabName, Specification);
                            SimulationForm.Show();
                        }
                        else
                        {
                            MessageBox.Show(Resources.ModuleFacadeBase_ShowSimulationWindow_Simulation_is_not_supported_in_this_module_, Common.Ultility.Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                    }
                    catch (Exception ex)
                    {
                        SimulationForm.Close();
                    }
                }
                else
                {
                    MessageBox.Show(Resources.Please_input_at_least_one_runnable_process__process_with_no_parameters__, Common.Ultility.Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        public virtual void ShowModelCheckingWindow(string tabName)
        {
            if (Specification != null)
            {
                if (Specification.AssertionDatabase.Count > 0)
                {
                    if (ModelCheckingForm.ModelCheckingFormInstance != null)
                    {
                        if (
                            MessageBox.Show(Resources.Only_one_model_checking_window_can_be_open_at_a_time__Do_you_want_to_close_the_current_one_to_open_a_new_one_,
                                Common.Ultility.Ultility.APPLICATION_NAME, MessageBoxButtons.YesNo,
                                MessageBoxIcon.Warning) == DialogResult.Yes)
                        {
                            ModelCheckingForm.ModelCheckingFormInstance.Close();
                        }
                        else
                        {
                            return;
                        }
                    }

                    if (ModelCheckingForm.ModelCheckingFormInstance == null)
                    {
                        ModelCheckingForm form = InstanciateModelCheckingForm(tabName);
                        form.Show();
                    }
                }
                else
                {
                    MessageBox.Show(Resources.Please_input_at_least_one_assertion_, Common.Ultility.Ultility.APPLICATION_NAME,MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        protected abstract SpecificationBase InstanciateSpecification(string text, string options, string filePath);

        protected virtual SimulationForm InstanciateSimulationForm()
        {
            return new SimulationForm();
        }

        protected virtual ModelCheckingForm InstanciateModelCheckingForm(string tabName)
        {
            return new ModelCheckingForm(tabName, Specification);
        }

        public virtual Image ModuleIcon
        {
            get
            {
                return Common.Ultility.Ultility.GetImage(ModuleName);
            }
        }

        public abstract string ModuleName
        {
            get;
        }

        public virtual IToken FindDeclarationToken(string keyword)
        {
            return null;
        }

        public virtual string PrintLaTexString()
        {
            return "";
        }

        public virtual void ToolbarInitialize(ToolStrip toolStrip, Toolbar_Button_Click_Delegate clickMethod)
        {

        }

        public virtual void PerformButtonClick(string tabName, string button)
        {
            
        }

        public virtual void ExampleMenualToolbarInitialize(ToolStripMenuItem ExampleMenu)
        {
            
        }

        public abstract List<string> GetTemplateTypes();
        public abstract SortedList<string, string> GetTemplateNames(string type);
        public abstract string GetTemplateModel(string templateName);


        //configuration related interface
        public virtual void ReadConfiguration()
        {
        }

        public virtual void WriteConfiguration()
        {
        }

        public virtual OptionPanelInterface GetOptionPanel()
        {
            return null;
        }

    }
}
