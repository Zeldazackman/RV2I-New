using HarmonyLib;
using Verse;

namespace RV2_Interactions
{
    [StaticConstructorOnStartup]
    public class Main
    {
        static Main()
        {
            new Harmony("Rutrix.RV2_Interactions").PatchAll();
        }

        public const string Id = "Rutrix.RV2_Interactions";

        public const string ModName = "RV2_Interactions";

        public const string Version = "0.8";
    }
}
