using HarmonyLib;
using RimVore2;
using Verse;

namespace RV2_Interactions
{
    internal class Patch_RV2Interaction_Settings
    {
        public class RV2Interactions_Mod : Mod
        {
            public static RV2Interactions_Mod mod;
            public RV2Interactions_Mod(ModContentPack content) : base(content)
            {
                mod = this;
                GetSettings<RV2Interaction_Settings>();  // create !static! settings
                WriteSettings();
            }
        }

        public class RV2Interaction_Settings : ModSettings
        {
            public static SettingsContainer_Interactions interactions;

            public RV2Interaction_Settings()
            {
                interactions = new SettingsContainer_Interactions();
            }

            public override void ExposeData()
            {
                base.ExposeData();
                Scribe_Deep.Look(ref interactions, "interactions", new object[0]);
            }
        }

        [HarmonyPatch(typeof(RV2Settings), "DefsLoaded")]
        public static class Patch_RV2Settings_DefsLoaded
        {
            [HarmonyPostfix]
            private static void AddRutSettings()
            {
                RV2Interaction_Settings.interactions.DefsLoaded();
            }
        }

        [HarmonyPatch(typeof(RV2Settings), "Reset")]
        public static class Patch_RV2Settings_Reset
        {
            [HarmonyPostfix]
            private static void AddRutSettings()
            {
                RV2Interaction_Settings.interactions.Reset();
            }
        }

        [HarmonyPatch(typeof(RV2Mod), "WriteSettings")]
        public static class Patch_RV2Mod_WriteSettings
        {
            [HarmonyPostfix]
            private static void AddRutSettings()
            {
                RV2Interactions_Mod.mod.WriteSettings();
            }
        }

        [HarmonyPatch(typeof(Window_Settings), "InitializeTabs")]
        public static class Patch_Window_Settings
        {
            [HarmonyPostfix]
            private static void AddRutSettings()
            {
                Window_Settings.AddTab(new SettingsTab_Interactions("RV2Interactions_Tab".Translate(), null, null));
            }
        }
    }
}
