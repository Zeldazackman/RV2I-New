using HarmonyLib;
using RimVore2;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace RV2_Interactions
{
    [HarmonyPatch(typeof(VoreTrackerRecord), "TickRare")]
    public class Patch_AddInteractions
    {
        [HarmonyPostfix]
        static void DoInteractions(VoreTrackerRecord __instance)
        {
            if (__instance.Predator.Map != null && __instance.Predator.Spawned)
            {
                float modifier = (float)__instance.VoreTracker.VoreTrackerRecords.Count;
                if (__instance.Predator.HostileTo(__instance.Prey))
                    modifier *= 2f;

                if (Rand.Chance(0.125f / modifier)
                 && (!__instance.HasReachedEnd && !__instance.HasReachedEntrance)
                 && (__instance.Predator.health.capacities.CanBeAwake && __instance.Prey.health.capacities.CanBeAwake)
                 && (!__instance.Predator.health.InPainShock && !__instance.Prey.health.InPainShock)
                 && (InteractionUtility.CanInitiateInteraction(__instance.Predator, null) && __instance.Predator.jobs.curDriver.DesiredSocialMode() != RandomSocialMode.Off)
                 && !__instance.Predator.interactions.InteractedTooRecentlyToInteract())
                {
                    //Log.Message("Attempting Interaction");
                    List<VoreSocialInteractionDef> validInteractions = new List<VoreSocialInteractionDef>();
                    float totalWeight = 0f;
                    List<VoreSocialInteractionDef> loadedInteractions = DefDatabase<VoreSocialInteractionDef>.AllDefsListForReading;

                    //Log.Message(loadedInteractions.Count + " interactions loaded");
                    foreach (VoreSocialInteractionDef interaction in loadedInteractions)
                        if (interaction.ValidInteraction(__instance) && interaction.weight > 0f)
                        {
                            validInteractions.Add(interaction);
                            totalWeight += interaction.weight;
                        }

                    //Log.Message(validInteractions.Count + " interactions valid");
                    if (!validInteractions.NullOrEmpty())
                    {
                        foreach (VoreSocialInteractionDef interactionDef in validInteractions)
                        {
                            if (Rand.Range(0f, totalWeight - interactionDef.weight) < interactionDef.weight)
                            {
                                //Log.Message(interactionDef.defName + " chosen");
                                DoFakeInteraction(__instance.Prey, interactionDef, __instance);
                                break;
                            }
                            totalWeight -= interactionDef.weight;
                        }
                    }
                }
            }

            bool DoFakeInteraction(Pawn recipient, InteractionDef intDef, VoreTrackerRecord record)
            {
                Pawn predator = record.Predator;
                if (predator == recipient)
                {
                    Log.Warning(predator?.ToString() + " tried to interact with self, interaction=" + intDef.defName + ", recipiant should have been " + record.Prey?.ToString());
                    return false;
                }
                if (!intDef.ignoreTimeSinceLastInteraction && record.Predator.interactions.InteractedTooRecentlyToInteract())
                {
                    return false;
                }
                List<RulePackDef> list = new List<RulePackDef>();
                if (intDef.initiatorThought != null && predator.needs.mood != null)
                {
                    Pawn_InteractionsTracker.AddInteractionThought(predator, recipient, intDef.initiatorThought);
                }
                if (intDef.recipientThought != null && recipient.needs.mood != null)
                {
                    Pawn_InteractionsTracker.AddInteractionThought(recipient, predator, intDef.recipientThought);
                }
                if (intDef.initiatorXpGainSkill != null && predator.RaceProps.Humanlike)
                {
                    predator.skills.Learn(intDef.initiatorXpGainSkill, (float)intDef.initiatorXpGainAmount, false);
                }
                if (intDef.recipientXpGainSkill != null && recipient.RaceProps.Humanlike)
                {
                    recipient.skills.Learn(intDef.recipientXpGainSkill, (float)intDef.recipientXpGainAmount, false);
                }
                intDef.Worker.Interacted(predator, recipient, list, out string text, out string text2, out LetterDef letterDef, out LookTargets lookTargets);
                MoteMaker.MakeInteractionBubble(predator, predator, intDef.interactionMote, intDef.GetSymbol(predator.Faction, predator.Ideo), intDef.GetSymbolColor(predator.Faction));
                PlayLogEntry_Interaction playLogEntry_Interaction = new PlayLogEntry_Interaction(intDef, predator, recipient, list);
                Find.PlayLog.Add(playLogEntry_Interaction);
                if (letterDef != null)
                {
                    string text3 = playLogEntry_Interaction.ToGameStringFromPOV(predator, false);
                    if (!text.NullOrEmpty())
                    {
                        text3 = text3 + "\n\n" + text;
                    }
                    Find.LetterStack.ReceiveLetter(text2, text3, letterDef, lookTargets ?? predator, null, null, null, null);
                }
                return true;
            }
        }
    }
}
