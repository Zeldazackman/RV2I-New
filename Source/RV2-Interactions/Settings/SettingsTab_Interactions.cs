using RimVore2;
using System;
using UnityEngine;
using static RV2_Interactions.Patch_RV2Interaction_Settings;

namespace RV2_Interactions
{
    public class SettingsTab_Interactions : SettingsTab
    {
        public SettingsTab_Interactions(string label, Action clickedAction, bool selected)
            : base(label, clickedAction, selected)
        {
        }

        public SettingsTab_Interactions(string label, Action clickedAction, Func<bool> selected)
            : base(label, clickedAction, selected)
        {
        }

        public override SettingsContainer AssociatedContainer => RV2Interaction_Settings.interactions;
        private SettingsContainer_Interactions Interactions => (SettingsContainer_Interactions)this.AssociatedContainer;

        public override void FillRect(Rect inRect)
        {
            this.Interactions.FillRect(inRect);
        }
    }
}