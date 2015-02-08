using System.Collections.Generic;


namespace PAT.Common.Classes.SemanticModels.Prob.SystemStructure
{
    public class RewardStruct
    {
        public string name;		// name (optional)
        public List<RewardItem> items;		// list of items
      
        public RewardStruct(string name, List<RewardItem> items)
        {
            this.name = name;
            this.items = items;
        }

        public override string ToString()
        {
            string s = "";

            s += "rewards";
            if (!string.IsNullOrEmpty(name)) s += " \"" + name + "\"";
            s += "\n";
            foreach (var rewardItem in items)
            {
                s += "\t" + rewardItem + "\n";
            }

            s += "endrewards";

            return s;
        }
    }
}
