using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDDL
{
    public class ConditionalPlanTreeNode
    {
        public int ID { get; set; }
        public Action Action { get; set; }
        public ConditionalPlanTreeNode SingleChild { get; set; }
        public ConditionalPlanTreeNode FalseObservationChild { get; set; }
        public ConditionalPlanTreeNode TrueObservationChild { get; set; }

        private static int CountNodes = 0;

        public ConditionalPlanTreeNode()
        {
            ID = CountNodes++;
        }

        private string ToString(string sIndent)
        {
            if (Action == null)
                return "";
            string s = sIndent + ID + ") " + Action.Name + "\n";
            if (SingleChild != null)
                s += SingleChild.ToString(sIndent);
            else
            {
                s += FalseObservationChild.ToString(sIndent + "\t");
                s += "\n";
                s += TrueObservationChild.ToString(sIndent + "\t");
            }
            return s;
        }
        public override string ToString()
        {
            return ToString("");
        }


        public bool ValidatePlan(PartiallySpecifiedState pssCurrent)
        {
            if(pssCurrent == null)
                return false;
            if (pssCurrent.IsGoalState())
                return true;

            if (Action == null)
                return false;
            Formula fObserved = null;
            PartiallySpecifiedState psTrueState, psFalseState;

            pssCurrent.ApplyOffline(Action, out fObserved, out psTrueState, out psFalseState);
            if (Action.Observe == null)
                return SingleChild.ValidatePlan(psTrueState);
            bool bTrueOk = TrueObservationChild.ValidatePlan(psTrueState); 
            bool bFalseOk = FalseObservationChild.ValidatePlan(psFalseState);
            return bTrueOk && bFalseOk;
        }
    }
}
