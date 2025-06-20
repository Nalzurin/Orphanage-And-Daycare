using RimWorld.BaseGen;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld.QuestGen;

namespace OrphanageAndDaycare
{
    public class GenStep_OrphanCamp : GenStep
    {
        public int size = 12;
        public int sizeToys = 6;

        public int requiredWorshippedTerminalRooms = 0;

        public bool allowGeneratingThronerooms = false;

        public bool settlementDontGeneratePawns = true;

        public bool allowGeneratingFarms = false;

        public bool generateLoot = true;

        public bool unfogged;

        public bool attackWhenPlayerBecameEnemy;

        public FloatRange defaultPawnGroupPointsRange = SymbolResolver_Settlement.DefaultPawnsPoints;

        private const int SpawnRadius = 2;

        private static List<CellRect> possibleRects = new List<CellRect>();
        private int childCount = 0;

        public override int SeedPart => 398638181;

        public override void Generate(Map map, GenStepParams parms)
        {
            childCount = Rand.RangeInclusive(1, 3);
            if (!MapGenerator.TryGetVar<CellRect>("RectOfInterest", out var var))
            {
                var = CellRect.SingleCell(map.Center);
            }
            if (!MapGenerator.TryGetVar<List<CellRect>>("UsedRects", out var var2))
            {
                var2 = new List<CellRect>();
                MapGenerator.SetVar("UsedRects", var2);
            }
            Faction faction = ((map.ParentFaction != null && map.ParentFaction != Faction.OfPlayer) ? map.ParentFaction : null);
            ResolveParams resolveParams = default(ResolveParams);
            CellRect OutpostRect = GetOutpostRect(var, var2, map);
            BaseGen.globalSettings.map = map;
            resolveParams.rect = new CellRect(OutpostRect.minX + sizeToys/2, OutpostRect.minZ + sizeToys/2, sizeToys, sizeToys);
            resolveParams.singleThingDef = Rand.Element(ThingDefOf.BabyDecoration);
            resolveParams.singleThingStuff = ThingDefOf.WoodLog;
            resolveParams.bedCount = childCount;
            BaseGen.symbolStack.Push("fillWithBeds", resolveParams);
            resolveParams.singleThingDef = ThingDefOf.Bedroll;
            resolveParams.singleThingStuff = ThingDefOf.Leather_Plain;
            resolveParams.bedCount = childCount;
            BaseGen.symbolStack.Push("fillWithBeds", resolveParams);
            resolveParams = default(ResolveParams);
            resolveParams.rect = OutpostRect;
            resolveParams.sitePart = parms.sitePart;
            resolveParams.wallStuff = null;
            resolveParams.chanceToSkipWallBlock = 0.4f;
            resolveParams.clearEdificeOnly = false;
            resolveParams.noRoof = false;
            resolveParams.spawnPawnsOnEdge = true;



            BaseGen.symbolStack.Push("ancientRuins", resolveParams);
            BaseGen.Generate();
            if (unfogged)
            {
                foreach (IntVec3 item in resolveParams.rect)
                {
                    MapGenerator.rootsToUnfog.Add(item);
                }
            }
            var2.Add(resolveParams.rect);
            List<Pawn> children = new List<Pawn>();
            for (int i = 0; i < childCount; i++)
            {
                children.Add(GeneratePawn());
            }
            SpawnPawns(map, children, map.Center, SpawnRadius);
        }

        private CellRect GetOutpostRect(CellRect rectToDefend, List<CellRect> usedRects, Map map)
        {
            possibleRects.Add(new CellRect(rectToDefend.minX - 1 - size, rectToDefend.CenterCell.z - size / 2, size, size));
            possibleRects.Add(new CellRect(rectToDefend.maxX + 1, rectToDefend.CenterCell.z - size / 2, size, size));
            possibleRects.Add(new CellRect(rectToDefend.CenterCell.x - size / 2, rectToDefend.minZ - 1 - size, size, size));
            possibleRects.Add(new CellRect(rectToDefend.CenterCell.x - size / 2, rectToDefend.maxZ + 1, size, size));
            CellRect mapRect = new CellRect(0, 0, map.Size.x, map.Size.z);
            possibleRects.RemoveAll((CellRect x) => !x.FullyContainedWithin(mapRect));
            if (possibleRects.Any())
            {
                IEnumerable<CellRect> source = possibleRects.Where((CellRect x) => !usedRects.Any((CellRect y) => x.Overlaps(y)));
                if (!source.Any())
                {
                    possibleRects.Add(new CellRect(rectToDefend.minX - 1 - size * 2, rectToDefend.CenterCell.z - size / 2, size, size));
                    possibleRects.Add(new CellRect(rectToDefend.maxX + 1 + size, rectToDefend.CenterCell.z - size / 2, size, size));
                    possibleRects.Add(new CellRect(rectToDefend.CenterCell.x - size / 2, rectToDefend.minZ - 1 - size * 2, size, size));
                    possibleRects.Add(new CellRect(rectToDefend.CenterCell.x - size / 2, rectToDefend.maxZ + 1 + size, size, size));
                }
                if (source.Any())
                {
                    return source.RandomElement();
                }
                return possibleRects.RandomElement();
            }
            return rectToDefend;
        }
        private Pawn GeneratePawn()
        {
            Slate slate = QuestGen.slate;
            Gender? fixedGender = null;

            if (!slate.TryGet<PawnGenerationRequest>("overridePawnGenParams", out var var))
            {
                var = new PawnGenerationRequest(PawnKindDefOf.Villager, Faction.OfPlayer, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: true, allowDead: false, allowDowned: true, canGeneratePawnRelations: true, mustBeCapableOfViolence: false, 20f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: false, allowFood: true, allowAddictions: true, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, fixedGender, null, null, null, null, forceNoIdeo: false, forceNoBackstory: false, forbidAnyTitle: false, forceDead: false, null, null, null, null, null, 0f, DevelopmentalStage.Child, null, null, null, forceRecruitable: true, dontGiveWeapon: true);
            }
            Pawn pawn = PawnGenerator.GeneratePawn(var);
            return pawn;
        }
        public static void SpawnPawns(Map map, IEnumerable<Pawn> pawns, IntVec3 root, int radius)
        {
            foreach (Pawn pawn in pawns)
            {
                if (!RCellFinder.TryFindRandomCellNearWith(root, (IntVec3 c) => c.Standable(map), map, out var result, radius))
                {
                    break;
                }
                GenSpawn.Spawn(pawn, result, map);
            }
        }
    }
}
