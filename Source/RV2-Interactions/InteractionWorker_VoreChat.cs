using RimWorld;
using Verse;

namespace RV2_Interactions
{
    internal class InteractionWorker_VoreChat : InteractionWorker
    {
        public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
        {
            return 0f;  // blap
        }
    }
}
