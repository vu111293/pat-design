using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PAT.Common.Classes.Expressions.ExpressionClass;

namespace PAT.PN.LTS
{
    /// <summary>
    /// Store the Place Model.
    /// Note: Do not store the current tokens here. Store that values in the Valuation
    /// </summary>
    public class PNPlace
    {
        public int InitialTokens { get; set; }

        #region Copy from PAT.Common.Classes.SemanticModels.LTS.BDD.PNPlace. Change Transition --> PNTransition

        public List<PNTransition> OutgoingPNTransitions = new List<PNTransition>();
        public List<PNTransition> IncomingPNTransition = new List<PNTransition>();

        public string Name;
        public bool HavePriority = false;
        public string Label;

        /// <summary>
        /// Integer type
        /// </summary>
        public string ID;

        public PNPlace(string name, string id)
        {
            Name = name;
            ID = id;
        }

        public PNPlace(string name, string id, bool HavePriority)
        {
            Name = name;
            ID = id;
            this.HavePriority = HavePriority;
        }

        public PNPlace(string name, string id, string label)
        {
            Name = name;
            ID = id;
            Label = label;
        }

        public PNPlace ClearConstant(Dictionary<string, Expression> constMapping)
        {
            return new PNPlace(Name, ID);
        }

        public override string ToString()
        {
            return Name;
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return ID == (obj as PNPlace).ID;
        }

        public void AddTransition(PNTransition tran)
        {
            if (OutgoingPNTransitions == null)
                OutgoingPNTransitions = new List<PNTransition>();

            OutgoingPNTransitions.Add(tran);
        }
        #endregion
    }
}
