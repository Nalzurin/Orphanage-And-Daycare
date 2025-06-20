
using RimWorld.BaseGen;
using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using System.Linq;
using RimWorld.QuestGen;

namespace OrphanageAndDaycare
{

    public class QuestNode_Root_OrphanageChildRequest : QuestNode_Root_WandererJoin
    {
        private const int TimeoutTicks = 60000;

        public const float RelationWithColonistWeight = 20f;

        private string signalAccept;

        private string signalReject;
        private Faction requestFaction;
        private Pawn Requester_Parent;
        protected override void RunInt()
        {
            Quest quest = QuestGen.quest;
            Slate slate = QuestGen.slate;
            if (!slate.TryGet<Map>("map", out var var))
            {
                var = QuestGen_Get.GetMap();
            }
            requestFaction = GetFaction();
            Requester_Parent = GenerateOrGetParent();
            Pawn pawn = GeneratePawn();
            AddSpawnPawnQuestParts(quest, var, pawn);
            slate.Set("pawn", pawn);
            if (Requester_Parent.gender == Gender.Female)
            {
                pawn.relations.AddDirectRelation(PawnRelationDefOf.ParentBirth, Requester_Parent);
            }
            else
            {
                pawn.relations.AddDirectRelation(PawnRelationDefOf.Parent, Requester_Parent);
            }

            SendLetter(quest, pawn);
            string inSignal = QuestGenUtility.HardcodedSignalWithQuestID("pawn.Killed");
            string inSignal2 = QuestGenUtility.HardcodedSignalWithQuestID("pawn.PlayerTended");
            string inSignal3 = QuestGenUtility.HardcodedSignalWithQuestID("pawn.LeftMap");
            string inSignal4 = QuestGenUtility.HardcodedSignalWithQuestID("pawn.Recruited");
            quest.End(QuestEndOutcome.Success, 0, null, inSignal2);
            quest.Signal(inSignal, delegate
            {

                quest.AcceptedAfterTicks(AllowKilledBeforeTicks, delegate
                {
                    quest.AnyColonistWithCharityPrecept(delegate
                    {
                        quest.Message("MessageCharityEventRefused".Translate() + ": " + "MessageWandererLeftToDie".Translate(pawn), MessageTypeDefOf.NegativeEvent, getLookTargetsFromSignal: false, null, pawn);
                    });
                    QuestGen_End.End(quest, QuestEndOutcome.Fail);
                }, delegate
                {
                    QuestGen_End.End(quest, QuestEndOutcome.Fail);
                });
            });
            quest.AnyColonistWithCharityPrecept(delegate
            {
                quest.Message("MessageCharityEventFulfilled".Translate() + ": " + "MessageWandererRecruited".Translate(pawn), MessageTypeDefOf.PositiveEvent, getLookTargetsFromSignal: false, null, pawn);
            }, null, inSignal4);
            quest.End(QuestEndOutcome.Success, 0, null, inSignal4);
            quest.Signal(inSignal3, delegate
            {
                AddLeftMapQuestParts(quest, pawn);
            });

            quest.Delay(60000, delegate
            {
                QuestGen_End.End(quest, QuestEndOutcome.Fail);
            });
        }
        public Faction GetFaction()
        {
            return Find.FactionManager.RandomNonHostileFaction(false, false, false);
        }
        public Pawn GenerateOrGetParent()
        {
            if (Rand.Chance(0.5f))
            {

                List<Pawn> possiblePawns = Find.WorldPawns.AllPawnsAlive.Where((Pawn p) => { return p.Faction == requestFaction && p.DevelopmentalStage == DevelopmentalStage.Adult && p != p.Faction.leader; }).ToList();
                if (!possiblePawns.Empty())
                {
                    return possiblePawns.RandomElement();
                }

            }

            Slate slate = QuestGen.slate;
            Gender? fixedGender = null;

            if (!slate.TryGet<PawnGenerationRequest>("overridePawnGenParams", out var var))
            {
                var = new PawnGenerationRequest(PawnKindDefOf.Villager, null, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: true, allowDead: false, allowDowned: true, canGeneratePawnRelations: true, mustBeCapableOfViolence: false, 20f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: true, allowFood: true, allowAddictions: true, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, fixedGender, null, null, null, null, forceNoIdeo: false, forceNoBackstory: false, forbidAnyTitle: false, forceDead: false, null, null, null, null, null, 0f, DevelopmentalStage.Adult, null, null, null, forceRecruitable: true);
            }
            Pawn pawn = PawnGenerator.GeneratePawn(var);
            if (!pawn.IsWorldPawn())
            {
                Find.WorldPawns.PassToWorld(pawn);
            }
            return pawn;


        }
        public override Pawn GeneratePawn()
        {
            Slate slate = QuestGen.slate;
            Gender? fixedGender = null;

            if (!slate.TryGet<PawnGenerationRequest>("overridePawnGenParams", out var var))
            {
                var = new PawnGenerationRequest(PawnKindDefOf.Villager, null, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: true, allowDead: false, allowDowned: true, canGeneratePawnRelations: true, mustBeCapableOfViolence: false, 20f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: false, allowFood: true, allowAddictions: true, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, fixedGender, null, null, null, null, forceNoIdeo: false, forceNoBackstory: false, forbidAnyTitle: false, forceDead: false, null, null, null, null, null, 0f, DevelopmentalStage.Child, null, null, null, forceRecruitable: true, dontGiveWeapon: true);
            }
            //var.AllowedDevelopmentalStages = DevelopmentalStage.Newborn;
            var.AllowedDevelopmentalStages |= DevelopmentalStage.Baby;

            var.ForcedXenotype = Requester_Parent.genes.Xenotype;
            Pawn pawn = PawnGenerator.GeneratePawn(var);
            if (!pawn.IsWorldPawn())
            {
                Find.WorldPawns.PassToWorld(pawn);
            }
            return pawn;
        }

        protected override void AddSpawnPawnQuestParts(Quest quest, Map map, Pawn pawn)
        {
            signalAccept = QuestGenUtility.HardcodedSignalWithQuestID("Accept");
            signalReject = QuestGenUtility.HardcodedSignalWithQuestID("Reject");
            quest.Signal(signalAccept, delegate
            {
                quest.SetFaction(Gen.YieldSingle(pawn), Faction.OfPlayer);
                quest.PawnsArrive(Gen.YieldSingle(pawn), null, map.Parent);
                quest.FactionGoodwillChange(requestFaction, OADSettings.OrphanageRequestGoodWillReward, null, canSendMessage: true, canSendHostilityLetter: false, true, OrphanageAndDaycareDefOf.AcceptedChild);

                QuestGen_End.End(quest, QuestEndOutcome.Success);
            });
            quest.Signal(signalReject, delegate
            {
                quest.GiveDiedOrDownedThoughts(pawn, PawnDiedOrDownedThoughtsKind.DeniedJoining);
                quest.FactionGoodwillChange(requestFaction, OADSettings.OrphanageRequestGoodWillPenalty, null, canSendMessage: true, canSendHostilityLetter: false, true, OrphanageAndDaycareDefOf.RejectedChild);
                QuestGen_End.End(quest, QuestEndOutcome.Fail);
            });
        }

        public override void SendLetter(Quest quest, Pawn pawn)
        {
            TaggedString title = "LetterLabelOrphanageChildRequest".Translate();
            TaggedString letterText = "LetterOrphanageChildRequest".Translate(Requester_Parent.Named("REQUESTER"), requestFaction.Named("FACTION"), pawn.Named("CHILD"), pawn.DevelopmentalStage.Named("AGE"));// Requester_Parent.LabelShort.Named(""), requestFaction.Name, Requester_Parent.LabelShort, pawn.DevelopmentalStage.ToStringSafe(), pawn.LabelShort);  
            AppendCharityInfoToLetter("JoinerCharityInfo".Translate(pawn), ref letterText);
            PawnRelationUtility.TryAppendRelationsWithColonistsInfo(ref letterText, ref title, pawn);

            ChoiceLetter_AcceptJoiner choiceLetter_AcceptJoiner = (ChoiceLetter_AcceptJoiner)LetterMaker.MakeLetter(title, letterText, LetterDefOf.AcceptJoiner);
            choiceLetter_AcceptJoiner.signalAccept = signalAccept;
            choiceLetter_AcceptJoiner.signalReject = signalReject;
            choiceLetter_AcceptJoiner.quest = quest;
            choiceLetter_AcceptJoiner.StartTimeout(60000);
            Find.LetterStack.ReceiveLetter(choiceLetter_AcceptJoiner);
        }

        public static void AppendCharityInfoToLetter(TaggedString charityInfo, ref TaggedString letterText)
        {
            if (!ModsConfig.IdeologyActive)
            {
                return;
            }
            IEnumerable<Pawn> source = IdeoUtility.AllColonistsWithCharityPrecept();
            if (!source.Any())
            {
                return;
            }
            letterText += "\n\n" + charityInfo + "\n\n" + "PawnsHaveCharitableBeliefs".Translate() + ":";
            foreach (IGrouping<Ideo, Pawn> item in from c in source
                                                   group c by c.Ideo)
            {
                letterText += "\n  - " + "BelieversIn".Translate(item.Key.name.Colorize(item.Key.TextColor), item.Select((Pawn f) => f.NameShortColored.Resolve()).ToCommaList());
            }
        }
    }

}
