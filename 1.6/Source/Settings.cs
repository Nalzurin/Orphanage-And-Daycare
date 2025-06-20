using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace OrphanageAndDaycare
{
    public class OADSettings : ModSettings
    {

        public static int OrphanageRequestGoodWillPenalty = -5;
        public static int OrphanageRequestGoodWillReward = 15;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref OrphanageRequestGoodWillPenalty, "OrphanageRequestGoodWillPenalty", defaultValue: -5, forceSave: true);
            Scribe_Values.Look(ref OrphanageRequestGoodWillReward, "OrphanageRequestGoodWillReward", defaultValue: 15, forceSave: true);
            base.ExposeData();
        }
        public void DoWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            listingStandard.Label("OrphanageRequestGoodwillAccept".Translate() + ": " + OrphanageRequestGoodWillReward.ToString());
            OrphanageRequestGoodWillReward = (int)listingStandard.Slider(OrphanageRequestGoodWillReward, -100, 100);
            listingStandard.Label("OrphanageRequestGoodwillDecline".Translate() + ": " + OrphanageRequestGoodWillPenalty.ToString());
            OrphanageRequestGoodWillPenalty = (int)listingStandard.Slider(OrphanageRequestGoodWillPenalty, -100, 100);
            listingStandard.End();
        }
    }
    public class OADMod : Mod
    {

        public static OADSettings settings;

        public OADMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<OADSettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            
            base.DoSettingsWindowContents(inRect);
            settings.DoWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "OADModName".Translate();
        }
    }
}
