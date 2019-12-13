using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Visitors
{
    public class CountCyclesOpVisitor : AutoVisitor
    {
		public int ForCount = 0;
		public int TotalStatemetsCount = 0;
		public int MidCount()
		{
			return TotalStatemetsCount / ForCount;
		}
		public override void VisitCycleNode(CycleNode c)
		{
			ForCount += 1;
			base.VisitCycleNode(c);
		}

		public override void VisitAssignNode(AssignNode a)
		{
			TotalStatemetsCount++;
			base.VisitAssignNode(a);
		}

		public override void VisitVarDefNode(VarDefNode w)
		{
			TotalStatemetsCount++;
			base.VisitVarDefNode(w);
		}

	}
}
