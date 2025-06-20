using RimWorld.QuestGen;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld.Planet;

namespace OrphanageAndDaycare
{
    public class QuestNode_GeneratePawnChild : QuestNode
    {
        [NoTranslate]
        public SlateRef<string> storeAs;

        [NoTranslate]
        public SlateRef<string> addToList;

        [NoTranslate]
        public SlateRef<IEnumerable<string>> addToLists;

        public SlateRef<PawnKindDef> kindDef;

        public SlateRef<Faction> faction;

        public SlateRef<bool> forbidAnyTitle;

        public SlateRef<bool> ensureNonNumericName;

        public SlateRef<IEnumerable<TraitDef>> forcedTraits;

        public SlateRef<IEnumerable<TraitDef>> prohibitedTraits;

        public SlateRef<Pawn> extraPawnForExtraRelationChance;

        public SlateRef<float> relationWithExtraPawnChanceFactor;


        public SlateRef<Gender?> fixedGender;

        public SlateRef<bool> giveDependentDrugs;

        private const int MinExpertSkill = 11;

        protected override bool TestRunInt(Slate slate)
        {
            return true;
        }

        protected override void RunInt()
        {
            Slate slate = QuestGen.slate;
            PawnKindDef value = kindDef.GetValue(slate);
            Faction value2 = faction.GetValue(slate);
            bool flag = false;
            bool value3 = false;
            IEnumerable<TraitDef> value4 = forcedTraits.GetValue(slate);
            IEnumerable<TraitDef> value5 = prohibitedTraits.GetValue(slate);
            float value6 =0f;
            PawnGenerationRequest request = new PawnGenerationRequest(mustBeCapableOfViolence: false, colonistRelationChanceFactor: 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: value3, allowFood: true, allowAddictions: flag, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, biocodeWeaponChance: value6, extraPawnForExtraRelationChance: extraPawnForExtraRelationChance.GetValue(slate), relationWithExtraPawnChanceFactor: relationWithExtraPawnChanceFactor.GetValue(slate), fixedGender: fixedGender.GetValue(slate), kind: value, faction: value2, context: PawnGenerationContext.NonPlayer, tile: -1, forceGenerateNewPawn: false, allowDead: false, allowDowned: true, canGeneratePawnRelations: true, biocodeApparelChance: 1f, validatorPreGear: null, validatorPostGear: null, forcedTraits: value4, prohibitedTraits: value5, minChanceToRedressWorldPawn: null, fixedBiologicalAge: null, fixedChronologicalAge: null, fixedLastName: null, fixedBirthName: null, fixedTitle: null, fixedIdeo: null, forceNoIdeo: false, forceNoBackstory: false, forbidAnyTitle: false, forceDead: false, forcedXenogenes: null, forcedEndogenes: null, forcedXenotype: null, forcedCustomXenotype: null, allowedXenotypes: null, forceBaselinerChance: 0f, developmentalStages: DevelopmentalStage.Baby);
            request.AllowedDevelopmentalStages |= DevelopmentalStage.Child;
            request.BiocodeApparelChance = 1f;
            request.ForbidAnyTitle = forbidAnyTitle.GetValue(slate);
            Pawn pawn = PawnGenerator.GeneratePawn(request);
            if (ensureNonNumericName.GetValue(slate) && (pawn.Name == null || pawn.Name.Numerical))
            {
                pawn.Name = PawnBioAndNameGenerator.GeneratePawnName(pawn);
            }
            if (giveDependentDrugs.GetValue(slate) && ModsConfig.BiotechActive && pawn.genes != null)
            {
                foreach (Gene item in pawn.genes.GenesListForReading)
                {
                    Gene_ChemicalDependency dep;
                    if (item.Active && (dep = item as Gene_ChemicalDependency) != null && DefDatabase<ThingDef>.AllDefs.Where((ThingDef x) => x.IsDrug && x.GetCompProperties<CompProperties_Drug>().chemical == dep.def.chemical).TryRandomElementByWeight((ThingDef x) => x.generateCommonality, out var result))
                    {
                        Thing thing = ThingMaker.MakeThing(result);
                        thing.stackCount = Rand.Range(1, 3);
                        pawn.inventory.innerContainer.TryAddOrTransfer(thing);
                    }
                }
            }
            if (storeAs.GetValue(slate) != null)
            {
                QuestGen.slate.Set(storeAs.GetValue(slate), pawn);
            }
            if (addToList.GetValue(slate) != null)
            {
                QuestGenUtility.AddToOrMakeList(QuestGen.slate, addToList.GetValue(slate), pawn);
            }
            if (addToLists.GetValue(slate) != null)
            {
                foreach (string item2 in addToLists.GetValue(slate))
                {
                    QuestGenUtility.AddToOrMakeList(QuestGen.slate, item2, pawn);
                }
            }
            QuestGen.AddToGeneratedPawns(pawn);
            if (!pawn.IsWorldPawn())
            {
                Find.WorldPawns.PassToWorld(pawn);
            }
        }
    }
}
