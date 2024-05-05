using RimVore2;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace RV2_Interactions
{
    internal class VoreSocialInteractionDef : InteractionDef
    {

        public float weight;
        public int willing;
        public int struggle;
        public int hostile;
        public int minOpinion;
        public int maxOpinion;
        public int endo;
        public int sapientPred;
        public int sapientPrey;
        public bool compatOnly;
        public bool loversOnly;
        public bool nuzzlePred;
        public bool nuzzlePrey;
        public bool predatorPred;
        public bool predatorPrey;
        public bool farmPred;
        public bool farmPrey;
        public List<VoreGoalDef> validGoals;
        public List<VoreTypeDef> validTypes;

        public bool ValidInteraction(VoreTrackerRecord record)
        {
            VoreSocialInteractionDef interaction = this;
            if (interaction.willing == Convert.ToInt32(record.IsForced))
                return false;

            if (interaction.struggle == Convert.ToInt32(!record.StruggleManager.ShouldStruggle))
                return false;

            if (interaction.hostile == Convert.ToInt32(!record.Predator.HostileTo(record.Prey)))
                return false;

            if (interaction.endo == Convert.ToInt32(record.VoreGoal.IsLethal))
                return false;

            if (interaction.minOpinion > -100
             && (interaction.minOpinion > GetAdjOpinion(record.Prey, record.Predator)
              || interaction.minOpinion > GetAdjOpinion(record.Predator, record.Prey)))
                return false;
            if (interaction.maxOpinion < 100
             && (interaction.maxOpinion < GetAdjOpinion(record.Prey, record.Predator)
              || interaction.maxOpinion < GetAdjOpinion(record.Predator, record.Prey)))
                return false;

            if (interaction.sapientPred < 2
                && (interaction.sapientPred != Convert.ToInt32((record.Predator.IsHumanoid() || record.Predator.IsColonistPlayerControlled))))  // Check looks odd due to sapient animals
                if (sapientPred == Convert.ToInt32(!record.Predator.IsHumanoid()))                                                              // and other like mods
                    return false;
            if (interaction.sapientPrey < 2
                && (interaction.sapientPrey != Convert.ToInt32((record.Prey.IsHumanoid() || record.Prey.IsColonistPlayerControlled))))          // This makes sure your sapient dragon
                if (sapientPrey == Convert.ToInt32(!record.Prey.IsHumanoid()))                                                                  // can still give out belly nuzzles 
                    return false;

            if (interaction.compatOnly && record.Predator.relations.SecondaryRomanceChanceFactor(record.Prey) < 0.45f)
                return false;
            if (interaction.loversOnly != record.Predator.GetLoveCluster().Contains(record.Prey))
                return false;

            if (interaction.nuzzlePred)
            {
                if (record.Predator.IsHumanoid() || record.Predator.RaceProps.nuzzleMtbHours < 0f)
                    return false;

                if (!Rand.Chance(6f / record.Predator.RaceProps.nuzzleMtbHours))
                    return false;
            }
            if (interaction.nuzzlePrey)
            {
                if (record.Prey.IsHumanoid() || record.Prey.RaceProps.nuzzleMtbHours < 0f)
                    return false;

                if (!Rand.Chance(6f / record.Prey.RaceProps.nuzzleMtbHours))
                    return false;
            }

            if (interaction.predatorPred && interaction.predatorPred != record.Predator.RaceProps.predator)
                return false;
            if (interaction.predatorPrey && interaction.predatorPrey != record.Prey.RaceProps.predator)
                return false;

            if (interaction.farmPred && interaction.farmPred != record.Predator.RaceProps.FenceBlocked)
                return false;
            if (interaction.farmPrey && interaction.farmPrey != record.Prey.RaceProps.FenceBlocked)
                return false;

            bool goodGoal = true;
            bool goodType = true;

            if (interaction.validGoals != null)
                foreach (VoreGoalDef voreGoal in interaction.validGoals)
                    if (record.VoreGoal == voreGoal)
                    {
                        goodGoal = true;
                        break;
                    }
                    else
                        goodGoal = false;

            if (interaction.validTypes != null)
                foreach (VoreTypeDef voreType in interaction.validTypes)
                    if (record.VoreType == voreType)
                    {
                        goodType = true;
                        break;
                    }
                    else
                        goodType = false;

            return goodGoal && goodType;
        }

        private int Clamped(int n, int min, int max)
        {
            if (n < min) return min;
            if (n > max) return max;
            return n;
        }

        private int GetAdjOpinion(Pawn pawnA, Pawn pawnB)
        {
            SettingsContainer_Interactions settings = Patch_RV2Interaction_Settings.RV2Interaction_Settings.interactions;
            int mod = settings.BaseOpinionMod;
            if (pawnA.IsHumanoid() || pawnB.IsHumanoid())
            {
                if (pawnA.IsHumanoid() && pawnA.story.traits.HasTrait(TraitDefOf.Kind))
                    mod = settings.KindOpinionMod;
                if (pawnB.IsHumanoid() && pawnB.story.traits.HasTrait(TraitDefOf.Kind))
                    mod += settings.KindOpinionMod;

            }
            if (pawnA.IsHumanoid() && pawnB.IsHumanoid())
            {
                int skillmod = Math.Min(20, Math.Max(pawnA.skills.GetSkill(SkillDefOf.Social).levelInt, pawnB.skills.GetSkill(SkillDefOf.Social).levelInt));
                mod += settings.SkillOpinionMod * (skillmod/20);
                return Clamped(pawnA.relations.OpinionOf(pawnB) + mod, -100, 100);
            }

            if ((!pawnA.IsHumanoid() || !pawnB.IsHumanoid()) && !(!pawnA.IsHumanoid() && !pawnB.IsHumanoid()))
            {
                if (pawnA.IsHumanoid())
                {
                    if (pawnB.relations.GetFirstDirectRelationPawn(PawnRelationDefOf.Bond) == pawnA)
                        return Clamped(50 + mod, -100, 100);

                    if (pawnB.Faction == pawnA.Faction)
                        return Clamped(settings.SkillOpinionMod * (Math.Min(pawnA.skills.GetSkill(SkillDefOf.Animals).levelInt, 20)/20) + mod, -100, 100);

                    if (!pawnB.HostileTo(pawnA))
                        return Clamped(settings.SkillOpinionMod * (Math.Min(pawnA.skills.GetSkill(SkillDefOf.Animals).levelInt, 20) / 20), -100, 100);

                    return mod;
                }
                if (pawnB.IsHumanoid())
                {
                    if (pawnA.relations.GetFirstDirectRelationPawn(PawnRelationDefOf.Bond) == pawnB)
                        return Clamped(50 + mod, -100, 100);

                    if (pawnA.Faction == pawnB.Faction)
                        return Clamped(settings.SkillOpinionMod * (Math.Min(pawnB.skills.GetSkill(SkillDefOf.Animals).levelInt, 20) / 20) + mod, -100, 100);

                    if (!pawnA.HostileTo(pawnB))
                        return Clamped(settings.SkillOpinionMod * (Math.Min(pawnB.skills.GetSkill(SkillDefOf.Animals).levelInt, 20) / 20), -100, 100);

                    return Clamped(0 + mod, -100, 100);
                }
            }
            if (pawnA.relations.FamilyByBlood.Contains(pawnB))
                return Clamped(30 + mod, -100, 100);

            if (pawnA.Faction == pawnB.Faction)
                return Clamped(10 + mod, -100, 100);

            return mod;
        }
        public override IEnumerable<string> ConfigErrors()
        {
            foreach (string error in base.ConfigErrors())
                yield return error;

            if (weight <= 0f)
                yield return "weight needs be greater than 0; zero weight interactions are never chosen";

            if (willing < 0 || willing > 2)
                yield return "willing needs be set to 0 (false), 1 (true) or 2 (ignore)";

            if (struggle < 0 || struggle > 2)
                yield return "struggle needs be set to 0 (false), 1 (true) or 2 (ignore)";

            if (hostile < 0 || hostile > 2)
                yield return "hostile needs be set to 0 (false), 1 (true) or 2 (ignore)";

            if (endo < 0 || endo > 2)
                yield return "endo needs be set to 0 (false), 1 (true) or 2 (ignore)";

            if (sapientPred < 0 || sapientPred > 2)
                yield return "sapientPred needs be set to 0 (false), 1 (true) or 2 (ignore)";

            if (sapientPrey < 0 || sapientPrey > 2)
                yield return "sapientPrey needs be set to 0 (false), 1 (true) or 2 (ignore)";

            if (minOpinion < -100 || minOpinion > 100)
                yield return "minOpinion needs be between -100 and 100; opinion never leaves these bounds";

            if (maxOpinion < -100 || maxOpinion > 100)
                yield return "maxOpinion needs be between -100 and 100; opinion never leaves these bounds";

            if (validGoals != null)
                foreach (VoreGoalDef voreGoalDef in validGoals)
                    foreach (string error in voreGoalDef.ConfigErrors())
                        yield return error;

            if (validTypes != null)
                foreach (VoreTypeDef voreTypeDef in validTypes)
                    foreach (string error in voreTypeDef.ConfigErrors())
                        yield return error;
        }

    }
}
