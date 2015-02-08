using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PAT.Common.Classes.DataStructure;
using PAT.Common.Classes.Expressions;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.LTS;

namespace PAT.PN.LTS
{
    public class PNValuation : Valuation
    {
        public StringDictionaryWithKey<int> ValuationToken;

        public PNValuation()
        {
            ValuationToken = new StringDictionaryWithKey<int>();
        }

        public PNValuation GetPNValuationClone()
        {
            var newEnv = new PNValuation();

            #region Based on Valuation.GetClone() code
            if (Variables != null)
            {
                var newVars = new StringDictionaryWithKey<ExpressionValue>(Variables);
                for (int i = 0; i < Variables._entries.Length; i++)
                {
                    StringDictionaryEntryWithKey<ExpressionValue> pair = Variables._entries[i];
                    if (pair != null)
                        newVars._entries[i] = new StringDictionaryEntryWithKey<ExpressionValue>(pair.HashA, pair.HashB, pair.Value.GetClone(), pair.Key);
                }

                newEnv.Variables = newVars;
            }

            if (Channels != null)
                newEnv.Channels = new Dictionary<string, ChannelQueue>(this.Channels);
            #endregion

            newEnv.ValuationToken = new StringDictionaryWithKey<int>(ValuationToken);
            return newEnv;
        }

        public override Valuation GetClone()
        {
            return GetPNValuationClone();
        }

        public int GetToken(string id)
        {
            return ValuationToken.GetContainsKey(id);
        }

        public void SetToken(string id, int value)
        {
            if (ValuationToken.ContainsKey(id))
                ValuationToken.SetValue(id, value);
            else
                ValuationToken.Add(id, value);
        }
    }
}
