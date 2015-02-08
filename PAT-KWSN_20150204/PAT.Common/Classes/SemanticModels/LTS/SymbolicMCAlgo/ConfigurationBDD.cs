using System;
using System.Collections.Generic;
using PAT.Common.Classes.Expressions;
using PAT.Common.Classes.ModuleInterface;

namespace PAT.Common.Classes.SemanticModels.LTS.BDD
{
    public class ConfigurationBDD : ConfigurationBase 
    {
        public ConfigurationBDD(string eventName, Valuation valuation)
        {
            this.Event = eventName;
            this.GlobalEnv = valuation;
        }

        public override IEnumerable<ConfigurationBase> MakeOneMove()
        {
            throw new NotImplementedException();
        }

        public override string GetID()
        {
            throw new NotImplementedException();
        }
    }
}
