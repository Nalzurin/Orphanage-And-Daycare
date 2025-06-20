using RimWorld.QuestGen;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace OrphanageAndDaycare
{
    public class QuestNode_GetChildToAdopt : QuestNode
    {
        [NoTranslate]
        public SlateRef<string> storeAs;

        [NoTranslate]
        public SlateRef<string> marketValue;
        protected override void RunInt()
        {
            SetVars(QuestGen.slate);
        }

        private void SetVars(Slate slate)
        {
            string value = storeAs.GetValue(slate);
            string value1 = marketValue.GetValue(slate);
            Pawn pawn = PawnsFinder.HomeMaps_FreeColonistsSpawned.Find(PawnCounts);
            slate.Set(value, pawn);
            slate.Set(value1, pawn.MarketValue);
           
        }
        protected override bool TestRunInt(Slate slate)
        {
          
            Pawn pawn =  PawnsFinder.HomeMaps_FreeColonistsSpawned.Find(PawnCounts);
            if(pawn == null)
            {
                return false;
            }
            SetVars(slate);
            return true;
        }

        private bool PawnCounts(Pawn pawn)
        {
            if (pawn.IsQuestLodger())
            {
                return false;
            }
            if(pawn.ageTracker.AgeBiologicalYears >= 13)
            {
                return false;
            }
            foreach(Pawn family in pawn.relations.FamilyByBlood)
            {
                if(PawnsFinder.AllMaps_FreeColonistsSpawned.Contains(family)) { return false; }
            }
            return true;
        }
    }
}
