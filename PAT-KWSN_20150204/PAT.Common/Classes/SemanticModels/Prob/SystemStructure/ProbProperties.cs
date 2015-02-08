using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PAT.Common.Classes.SemanticModels.Prob.SystemStructure
{
    public class ProbProperties
    {
        public Modules modules;

        public FormulaList formulaList;
        public LabelList labelList;
        public LabelList combinedLabelList;
        public ConstantList constantList;
        public List<Property> properties;

        public List<String> allIdentsUsed;

        public Values constantValues;

        public ProbProperties()
        {
            formulaList = new FormulaList();
            labelList = new LabelList();
            constantList = new ConstantList();
            properties = new List<Property>();
        }

        public void AddProperty(Property newProperty)
        {
            properties.Add(newProperty);
        }
    }
}
