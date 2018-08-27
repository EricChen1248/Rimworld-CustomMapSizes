using System.Reflection;
using Harmony;
using HugsLib;

namespace CustomMapSizes
{
    public class CustomMapSizes : ModBase
    {
        public override string ModIdentifier => "CustomMapSizes";
        
        public override void DefsLoaded() 
        {
            var harmony = HarmonyInstance.Create("CustomMapSizes");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
