using System.Collections.Generic;

namespace PAT.Common.Classes.ModuleInterface
{
    public class ModelCheckingOptions
    {
        public List<string> AddimissibleBehaviorsNames;
        public List<AddimissibleBehavior> AddimissibleBehaviors;

        public ModelCheckingOptions()
        {
            AddimissibleBehaviors = new List<AddimissibleBehavior>();
            AddimissibleBehaviorsNames = new List<string>();
        }

        public void AddAddimissibleBehavior(string name, List<string> VerificationEngines)
        {
            AddimissibleBehaviors.Add(new AddimissibleBehavior(name, VerificationEngines));
            AddimissibleBehaviorsNames.Add(name);
        }
    }

    public class AddimissibleBehavior
    {
        public string Name;
        public List<string> VerificationEngines;
        public AddimissibleBehavior(string name, List<string> engines)
        {
            Name = name;
            VerificationEngines = engines;
        }
    }
}
