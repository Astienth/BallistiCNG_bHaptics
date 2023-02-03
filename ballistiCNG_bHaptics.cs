using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MelonLoader;
using bHapticsLib;
using UnityEngine;
using MyBhapticsTactsuit;
using NgShips;
using HarmonyLib;

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
                
        [HarmonyPatch(typeof(ShipController), "TakeDamage")]
        public class bhaptics_TakeDamage
        {
            [HarmonyPostfix]
            public static void Postfix(bool __result, float amount)
            {
                if (tactsuitVr.suitDisabled || !__result || amount < 1.5f)
                {
                    return;
                }
                MelonLogger.Msg("DAMAGE AMOUNT " + amount);
                ballistiCNG_bHaptics.tactsuitVr.PlaybackHaptics("VehicleImpact_Vest");
                ballistiCNG_bHaptics.tactsuitVr.PlaybackHaptics("VehicleImpact_Arms");
            }
        }

        [HarmonyPatch(typeof(ShipController), "FixedUpdate")]
        public class bhaptics_FixedUpdate
        {
            [HarmonyPostfix]
            public static void Postfix(ShipController __instance)
            {
                if (tactsuitVr.suitDisabled)
                {
                    return;
                }

                //rechargin pits
                if (__instance.IsRecharging)
                {
                    ballistiCNG_bHaptics.tactsuitVr.StartRecharging();
                }
                else
                {
                    ballistiCNG_bHaptics.tactsuitVr.StopRecharging();
                }
            }
        }
    }
}
