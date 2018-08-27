using Verse;

namespace CustomMapSizes
{
    public class Settings : ModSettings
    {
        public static int CustomSize = 500;
        
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref CustomSize, "CustomSize", 0, false);
        }
    }
}
