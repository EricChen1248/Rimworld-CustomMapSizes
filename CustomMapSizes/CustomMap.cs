using System.Linq;
using Harmony;
using RimWorld;
using UnityEngine;
using Verse;

namespace CustomMapSizes
{
    [HarmonyPatch(typeof(Dialog_AdvancedGameConfig), "DoWindowContents")]
    public class CustomMap
    {
        private static int _customSize = 500;
        private static string _settingsString = "500";

        private static bool Prefix(ref Dialog_AdvancedGameConfig __instance, Rect inRect)
        {
            var listingStandard = new Listing_Standard {ColumnWidth = 200f};

            listingStandard.Begin(inRect.AtZero());
            listingStandard.Label("MapSize".Translate());
            var mapSizes = Traverse.Create(__instance).Field("MapSizes").GetValue<int[]>();

            foreach (var mapSize in mapSizes)
            {
                switch (mapSize)
                {
                    case 200:
                        listingStandard.Label("MapSizeSmall".Translate());
                        break;
                    case 250:
                        listingStandard.Label("MapSizeMedium".Translate());
                        break;
                    case 300:
                        listingStandard.Label("MapSizeLarge".Translate());
                        break;
                    case 350:
                        listingStandard.Label("MapSizeExtreme".Translate());
                        break;
                }

                var label = "MapSizeDesc".Translate(mapSize, mapSize * mapSize);
                if (listingStandard.RadioButton(label, Find.GameInitData.mapSize == mapSize))
                {
                    Find.GameInitData.mapSize = mapSize;
                }
            }

            listingStandard.Label("Custom Map Size");

            var lab = "MapSizeDesc".Translate(_customSize, _customSize * _customSize);
            if (listingStandard.RadioButton(lab, Find.GameInitData.mapSize == _customSize))
            {
                Find.GameInitData.mapSize = _customSize;
            }

            listingStandard.Label("New Size:");
            _settingsString = Widgets.TextField(new Rect(90f, 362f, 60f, 20f), _settingsString);
            if (Widgets.ButtonText(new Rect(160f, 362f, 40f, 22f), "Apply"))
            {
                if (int.TryParse(_settingsString, out var result) && result > 0)
                {
                    if (mapSizes.Contains(result))
                    {
                        Messages.Message("Built in maps already has these dimensions", MessageTypeDefOf.NegativeEvent);
                        _settingsString = _customSize.ToStringSafe();
                    }
                    else
                    {
                        _customSize = result;
                    }
                }
                else
                {
                    Messages.Message("Must be an positive integer", MessageTypeDefOf.NegativeEvent);
                    _settingsString = _customSize.ToStringSafe();
                }
            }

            listingStandard.NewColumn();
            GenUI.SetLabelAlign(TextAnchor.MiddleCenter);
            listingStandard.Label("MapStartSeason".Translate());
            var label1 = Find.GameInitData.startingSeason != Season.Undefined
                ? Find.GameInitData.startingSeason.LabelCap()
                : "MapStartSeasonDefault".Translate();
            var gridLayout = new GridLayout(listingStandard.GetRect(32f), 5, 1, 0.0f);
            if (Widgets.ButtonText(gridLayout.GetCellRectByIndex(0), "-"))
            {
                var startingSeason = Find.GameInitData.startingSeason;
                Find.GameInitData.startingSeason =
                    startingSeason != Season.Undefined ? startingSeason - 1 : Season.Winter;
            }

            Widgets.Label(gridLayout.GetCellRectByIndex(1, 3), label1);
            if (Widgets.ButtonText(gridLayout.GetCellRectByIndex(4), "+"))
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
                listingStandard.Label("MapTemperatureDangerWarning".Translate());
            if (Find.GameInitData.mapSize > 250)
                listingStandard.Label("MapSizePerformanceWarning".Translate());
            listingStandard.End();

            return false;
        }
    }
}