using RimVore2;
using System;
using UnityEngine;
using Verse;

namespace RV2_Interactions
{
    internal class SettingsContainer_Interactions : SettingsContainer
    {

        private FloatSmartSetting interactionChance;
        private FloatSmartSetting opinionModifier;
        private FloatSmartSetting multiPreyModifier;

        private FloatSmartSetting skillOpinionMod;
        private FloatSmartSetting kindOpinionMod;

        public float InteractionChance => this.interactionChance.value / 100f;
        public float OpinionModifier => this.opinionModifier.value / 100f;
        public float MultiPreyModifier => this.multiPreyModifier.value / 100f;
        public int SkillOpinionMod => (int)Math.Floor(this.skillOpinionMod.value);
        public int KindOpinionMod => (int)Math.Floor(this.kindOpinionMod.value);

        public override void EnsureSmartSettingDefinition()
        {
            if (this.interactionChance == null || this.interactionChance.IsInvalid())
                this.interactionChance = new FloatSmartSetting("RV2Interaction_Settings_InteractionChance", 15f, 15f, 0f, 100f, "RV2Interaction_Settings_InteractionChanceTip", "0", "%");
            if (this.opinionModifier == null || this.opinionModifier.IsInvalid())
                this.opinionModifier = new FloatSmartSetting("RV2Interaction_Settings_OpinionModifier", 50f, 50f, 0f, 100f, "RV2Interaction_Settings_OpinionModifierTip", "0", "%");
            if (this.multiPreyModifier == null || this.multiPreyModifier.IsInvalid())
                this.multiPreyModifier = new FloatSmartSetting("RV2Interaction_Settings_MultiPreyModifier", 100f, 100f, 0f, 100f, "RV2Interaction_Settings_MultiPreyModifier_Tip", "0", "%");
            if (this.kindOpinionMod == null || this.kindOpinionMod.IsInvalid())
                this.kindOpinionMod = new FloatSmartSetting("RV2Interaction_Settings_KindOpinionMod", 10f, 10f, 0f, 50f, "RV2Interaction_Settings_KindOpinionMod_Tip", "0");
            if (this.skillOpinionMod == null || this.skillOpinionMod.IsInvalid())
                this.skillOpinionMod = new FloatSmartSetting("RV2Interaction_Settings_SkillOpinionMod", 20f, 20f, 0f, 50f, "RV2Interaction_Settings_SkillOpinionMod_Tip", "0");

        }

        private bool heightStale = true;

        private float height;

        private Vector2 scrollPosition;

        public override void Reset()
        {
            this.interactionChance = null;
            this.opinionModifier = null;
            this.multiPreyModifier = null;
            this.skillOpinionMod = null;
            this.kindOpinionMod = null;

            EnsureSmartSettingDefinition();
        }
        public void FillRect(Rect inRect)
        {
            UIUtility.MakeAndBeginScrollView(inRect, this.height, ref this.scrollPosition, out Listing_Standard listing_Standard);
            if (listing_Standard.ButtonText("RV2_Settings_Reset".Translate(), null))
                this.Reset();

            this.interactionChance.DoSetting(listing_Standard);
            this.multiPreyModifier.DoSetting(listing_Standard);
            this.opinionModifier.DoSetting(listing_Standard);
            this.skillOpinionMod.DoSetting(listing_Standard);
            this.kindOpinionMod.DoSetting(listing_Standard);
            listing_Standard.EndScrollView(ref this.height, ref this.heightStale);
        }
        public override void ExposeData()
        {
            if (Scribe.mode == LoadSaveMode.Saving || Scribe.mode == LoadSaveMode.LoadingVars)
                this.EnsureSmartSettingDefinition();

            Scribe_Deep.Look<FloatSmartSetting>(ref this.interactionChance, "interactionChance", new object[0]);
            Scribe_Deep.Look<FloatSmartSetting>(ref this.opinionModifier, "opinionModifier", new object[0]);
            Scribe_Deep.Look<FloatSmartSetting>(ref this.multiPreyModifier, "multiPreyModifier", new object[0]);
            Scribe_Deep.Look<FloatSmartSetting>(ref this.kindOpinionMod, "kindOpinionMod", new object[0]);
            Scribe_Deep.Look<FloatSmartSetting>(ref this.skillOpinionMod, "skillOpinionMod", new object[0]);
            this.PostExposeData();
        }
    }
}
