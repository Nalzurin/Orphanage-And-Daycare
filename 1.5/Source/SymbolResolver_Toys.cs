using RimWorld.BaseGen;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace OrphanageAndDaycare
{
    public class SymbolResolver_FillWithToys : SymbolResolver
    {
        public override void Resolve(ResolveParams rp)
        {
            Map map = BaseGen.globalSettings.map;
            ThingDef thingDef = Rand.Element(ThingDefOf.BabyDecoration, ThingDefOf.ToyBox);
            ThingDef singleThingStuff = ThingDefOf.WoodLog;
            bool @bool = Rand.Bool;
            int num = Rand.RangeInclusive(1, 5);
            int num2 = 0;
            foreach (IntVec3 item in rp.rect)
            {
                if (@bool)
                {
                    if (item.x % 3 != 0 || item.z % 2 != 0)
                    {
                        continue;
                    }
                }
                else if (item.x % 2 != 0 || item.z % 3 != 0)
                {
                    continue;
                }
                if (!GenSpawn.WouldWipeAnythingWith(item, Rot4.North, thingDef, map, (Thing x) => x.def.category == ThingCategory.Building) && !BaseGenUtility.AnyDoorAdjacentCardinalTo(GenAdj.OccupiedRect(item, Rot4.North, thingDef.Size), map))
                {
                    ResolveParams resolveParams = rp;
                    resolveParams.rect = GenAdj.OccupiedRect(item, Rot4.North, thingDef.size);
                    resolveParams.singleThingDef = thingDef;
                    resolveParams.singleThingStuff = singleThingStuff;
                    resolveParams.thingRot = Rot4.North;
                    BaseGen.symbolStack.Push("orphanageAndDaycare.toy", resolveParams);
                    num2++;
                    if (num2 >= num)
                    {
                        break;
                    }
                }
            }
        }
    }
    public class SymbolResolver_Toy : SymbolResolver
    {
        public override void Resolve(ResolveParams rp)
        {
            ThingDef singleThingDef = rp.singleThingDef ?? Rand.Element(ThingDefOf.BabyDecoration, ThingDefOf.ToyBox);
            ResolveParams resolveParams = rp;
            resolveParams.singleThingDef = singleThingDef;
            resolveParams.skipSingleThingIfHasToWipeBuildingOrDoesntFit = rp.skipSingleThingIfHasToWipeBuildingOrDoesntFit ?? true;
            BaseGen.symbolStack.Push("thing", resolveParams);
        }
    }
}
