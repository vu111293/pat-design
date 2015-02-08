using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using PAT.Common;
using PAT.Common.Classes.ModuleInterface;
using PAT.PN.LTS;
using PAT.Common.Classes.LTS;
using PAT.Common.GUI;
using System.Windows.Forms;
using PAT.PN.Utility.Example;

namespace PAT.PN
{
    public sealed class ModuleFacade : ModuleFacadeBase
    {
        protected override SpecificationBase InstanciateSpecification(string text, string options, string filePath)
        {
            return new Specification(text, options, filePath);
        }

        public override string ModuleName
        {
            get
            {
                return "PN Model";
            }
        }

        public override Image ModuleIcon
        {
            get
            {
                try
                {
                    Assembly myAssembly = Assembly.GetExecutingAssembly();
                    Stream myStream = myAssembly.GetManifestResourceStream("PAT.PN.pn.ico");
                    Bitmap image = new Bitmap(myStream);
                    return image;
                }
                catch (Exception)
                {
                    return Common.Ultility.Ultility.GetModuleImage("Error");
                }
            }
        }

        public override List<string> GetTemplateTypes()
        {
            throw new NotImplementedException();
        }

        public override SortedList<string, string> GetTemplateNames(string type)
        {
            throw new NotImplementedException();
        }

        public override string GetTemplateModel(string templateName)
        {
            throw new NotImplementedException();
        }

        public override void ExampleMenualToolbarInitialize(ToolStripMenuItem ExampleMenu)
        {
            ToolStripMenuItem PNExamples = new ToolStripMenuItem("PN Examples") { Image = ModuleIcon };
            PNExamples.Image = this.ModuleIcon;

            ExampleMenu.DropDownItems.AddRange(new ToolStripItem[] { PNExamples });
            //TODO
            Add2DiningPhilsExample(PNExamples);
            AddH2OReactionExample(PNExamples);
            AddSharedComputerSystemExample(PNExamples);
        }

        private void AddSharedComputerSystemExample(ToolStripMenuItem PNExamples)
        {
            ToolStripMenuItem SharedComputerSystemExampleToolStripMenuItem = new ToolStripMenuItem("Shared Computer System");
            SharedComputerSystemExampleToolStripMenuItem.Click += new System.EventHandler((sender, e) =>
            {
                ShowModel(SharedComputerSystem.SharedComputerSystemProblem(), ModuleName);
            });

            PNExamples.DropDownItems.AddRange(new ToolStripItem[] { SharedComputerSystemExampleToolStripMenuItem });
        }

        private void AddH2OReactionExample(ToolStripMenuItem PNExamples)
        {
            ToolStripMenuItem H2OReactionExampleToolStripMenuItem = new ToolStripMenuItem("H2O Reaction");
            H2OReactionExampleToolStripMenuItem.Click += new System.EventHandler((sender, e) =>
            {
                ShowModel(H2OReaction.H2OReactionProblem(), ModuleName);
            });

            PNExamples.DropDownItems.AddRange(new ToolStripItem[] { H2OReactionExampleToolStripMenuItem });
        }

        private void Add2DiningPhilsExample(ToolStripMenuItem PNExamples)
        {
            ToolStripMenuItem twoDiningPhilsExampleToolStripMenuItem = new ToolStripMenuItem("2 Dining Philosophers Problem");
            twoDiningPhilsExampleToolStripMenuItem.Click += new System.EventHandler((sender, e) =>
            {
                ShowModel(TwoDiningPhilosophers.TwoDiningPhilosophersProblem(), ModuleName);
            });

            PNExamples.DropDownItems.AddRange(new ToolStripItem[] { twoDiningPhilsExampleToolStripMenuItem });
        }
    }
}
