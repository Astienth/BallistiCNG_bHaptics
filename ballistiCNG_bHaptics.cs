using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MelonLoader;
using bHapticsLib;
using UnityEngine;
using MyBhapticsTactsuit;

namespace BallistiCNG_bHaptics
{
    public class ballistiCNG_bHaptics : MelonMod
    {
        public static TactsuitVR tactsuitVr;

        public override void OnInitializeMelon()
        {
            tactsuitVr = new TactsuitVR();
            LoggerInstance.Msg("Mods bHaptics loaded");
            tactsuitVr.PlaybackHaptics("HeartBeat");
        }

        /*
        [HarmonyPatch(typeof(BaseGameController), "OnSlowTime", new Type[] { typeof(float) })]
        public class bhaptics_SlowTime
        {
            [HarmonyPostfix]
            public static void Postfix(BaseGameController __instance)
            {
                Melon<ballistiCNG_bHaptics>.Logger.Msg("LOGGER");
                tactsuitVr.PlaybackHaptics("SloMo");
            }
        }
        */
    }
}
