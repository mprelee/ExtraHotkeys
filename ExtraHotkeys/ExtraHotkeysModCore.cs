using System.Collections.Generic;
using Assets.Code;
using HarmonyLib;

namespace ExtraHotkeys
{
    public class ExtraHotkeysModCore : Assets.Code.Modding.ModKernel
    {
        Harmony harmony;
        public override void onStartGamePresssed(Map map, List<God> gods)
        {
            harmony = new Harmony("com.github.mprelee");
            // add null checks to the following lines, they are omitted for clarity
            // when possible, don't use string and instead use nameof(...)
            var original = typeof(UIInputs).GetMethod("hotkeys");
            var postfix = typeof(ExtraHotkeys).GetMethod("Hotkeys");
            harmony.Patch(original, postfix: new HarmonyMethod(postfix));
        }
    }
}


