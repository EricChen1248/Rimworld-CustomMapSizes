using Harmony;
using RimWorld;
using UnityEngine;
using Verse;

namespace CustomMapSizes
{
    [HarmonyPatch(typeof(Dialog_AdvancedGameConfig), "DoWindowContents")]
    public class CustomMap
    {
        private static int CustomSize = 500;
        private static string _settingsString = "500";
        private static bool Prefix(ref Dialog_AdvancedGameConfig __instance, Rect inRect)
        {
            var listingStandard = new Listing_Standard {ColumnWidth = 200f};

            listingStandard.Begin(inRect.AtZero());
            listingStandard.Label("MapSize".Translate(), -1f);
            var mapSizes = Traverse.Create(__instance).Field("MapSizes").GetValue<int[]>();

            foreach (var mapSize in mapSizes)
            {
                switch (mapSize)
                {
                    case 200:
                        listingStandard.Label("MapSizeSmall".Translate(), -1f);
                        break;
                    case 250:
                        listingStandard.Label("MapSizeMedium".Translate(), -1f);
                        break;
                    case 300:
                        listingStandard.Label("MapSizeLarge".Translate(), -1f);
                        break;
                    case 350:
                        listingStandard.Label("MapSizeExtreme".Translate(), -1f);
                        break;
                }

                var label = "MapSizeDesc".Translate(mapSize, mapSize * mapSize);
                if (listingStandard.RadioButton(label, Find.GameInitData.mapSize == mapSize))
                    Find.GameInitData.mapSize = mapSize;
            }
            
            listingStandard.Label($"Custom Map Size: {CustomSize}x{CustomSize}");
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
            var lab = "MapSizeDesc".Translate(Settings.CustomSize, (Settings.CustomSize * Settings.CustomSize));
            if (listingStandard.RadioButton(lab, Find.GameInitData.mapSize == Settings.CustomSize, 0.0f))
                Find.GameInitData.mapSize = Settings.CustomSize;


            listingStandard.NewColumn();
            GenUI.SetLabelAlign(TextAnchor.MiddleCenter);
            listingStandard.Label("MapStartSeason".Translate(), -1f);
            var label1 = Find.GameInitData.startingSeason != Season.Undefined
                ? Find.GameInitData.startingSeason.LabelCap()
                : "MapStartSeasonDefault".Translate();
            var gridLayout = new GridLayout(listingStandard.GetRect(32f), 5, 1, 0.0f, 4f);
            if (Widgets.ButtonText(gridLayout.GetCellRectByIndex(0, 1, 1), "-", true, false, true))
            {
                var startingSeason = Find.GameInitData.startingSeason;
                Find.GameInitData.startingSeason =
                    startingSeason != Season.Undefined ? startingSeason - 1 : Season.Winter;
            }

            Widgets.Label(gridLayout.GetCellRectByIndex(1, 3, 1), label1);
            if (Widgets.ButtonText(gridLayout.GetCellRectByIndex(4, 1, 1), "+", true, false, true))
            {
                var startingSeason = Find.GameInitData.startingSeason;
                Find.GameInitData.startingSeason =
                    startingSeason != Season.Winter ? startingSeason + 1 : Season.Undefined;
            }

            GenUI.ResetLabelAlign();

            var selTile = Traverse.Create(__instance).Field("selTile").GetValue<int>();
            if (selTile >= 0 && Find.GameInitData.startingSeason != Season.Undefined &&
                GenTemperature.AverageTemperatureAtTileForTwelfth(selTile,
                    Find.GameInitData.startingSeason.GetFirstTwelfth(Find.WorldGrid.LongLatOf(selTile).y)) <
                3.0)
                listingStandard.Label("MapTemperatureDangerWarning".Translate(), -1f);
            if (Find.GameInitData.mapSize > 250)
                listingStandard.Label("MapSizePerformanceWarning".Translate(), -1f);
            listingStandard.End();

            return false;
        }
    }
}