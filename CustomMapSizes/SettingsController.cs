using RimWorld;
using UnityEngine;
using Verse;

namespace CustomMapSizes
{
    public class SettingsController : Mod
    {
        private string _settingsString = Settings.CustomSize.ToStringSafe();
        public override string SettingsCategory()
        {
            return "Custom Map Sizes";
        }
        
        public override void DoSettingsWindowContents(Rect rect)
        {
            var listingStandard = new Listing_Standard();
            listingStandard.Begin(rect);
            listingStandard.Label($"Current Map Size: {Settings.CustomSize}x{Settings.CustomSize}");
            listingStandard.Label("Change to: ");
            _settingsString = Widgets.TextField(new Rect(85f, 26f, 60f, 20f), _settingsString);
            if (Widgets.ButtonText(new Rect(160f, 26f, 40f, 22f), "Apply", true, false, true))
            {
                if (int.TryParse(_settingsString, out var result) && result > 0)
                {
                    Settings.CustomSize = result;
                }
                else
                {
                    Messages.Message("Must be an positive integer", MessageTypeDefOf.NegativeEvent);
                    _settingsString = Settings.CustomSize.ToStringSafe();
                }
            }
            listingStandard.End();
        }

        public SettingsController(ModContentPack content) : base(content)
        {
            GetSettings<Settings>();
        }
    }
}
