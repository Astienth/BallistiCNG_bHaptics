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
using NgData;
using NgSettings;
using NgGame;
using NgEvents;

namespace BallistiCNG_bHaptics
{
    public class ballistiCNG_bHaptics : MelonMod
    {
        public static TactsuitVR tactsuitVr;
        public static bool isUsingLeftAirBrake = false;
        public static bool isUsingRightAirBrake = false;

        public override void OnInitializeMelon()
        {
            tactsuitVr = new TactsuitVR();
            LoggerInstance.Msg("Mods bHaptics loaded");
            tactsuitVr.PlaybackHaptics("HeartBeat");
        }

        public override void OnLateInitializeMelon()
        {          
            NgRaceEvents.OnEventComplete += new NgRaceEvents.EventCompleteDelegate(this.OnGamePaused);
            NgUiEvents.OnGamePause += new NgUiEvents.GamePausedDelegate(this.OnGamePaused);
        }

        public void OnGamePaused()
        {
            tactsuitVr.StopThreads();
        }
        
       [HarmonyPatch(typeof(ShipController), "OnEliminated")]
        public class bhaptics_OnEliminated
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                if (tactsuitVr.suitDisabled)
                {
                    return;
                }
                ballistiCNG_bHaptics.tactsuitVr.PlaybackHaptics("Death");
                tactsuitVr.StopThreads();
            }
        }

        [HarmonyPatch(typeof(ShipController), "TakeDamage")]
        public class bhaptics_TakeDamage
        {
            [HarmonyPostfix]
            public static void Postfix(ShipController __instance, bool __result, float amount)
            {
                if (tactsuitVr.suitDisabled || !__instance.IsPlayerOne
                    || !__result || amount < 1.5f)
                {
                    return;
                }
                ballistiCNG_bHaptics.tactsuitVr.PlaybackHaptics("VehicleImpact_Vest");
                ballistiCNG_bHaptics.tactsuitVr.PlaybackHaptics("VehicleImpact_Arms");
            }
        }

        [HarmonyPatch(typeof(ShipInput), "Update")]
        public class bhaptics_ShipAirbrake
        {
            [HarmonyPostfix]
            public static void Postfix(ShipInput __instance)
            {
                if (tactsuitVr.suitDisabled 
                    || !__instance.r.IsPlayerOne)
                {
                    return;
                }

                isUsingLeftAirBrake = Traverse.Create(__instance).Field("_leftAirbrakePressed").GetValue<bool>();
                isUsingRightAirBrake = Traverse.Create(__instance).Field("_rightAirbrakePressed").GetValue<bool>();               
            }
        }

        [HarmonyPatch(typeof(ShipController), "FixedUpdate")]
        public class bhaptics_FixedUpdate
        {
            [HarmonyPostfix]
            public static void Postfix(ShipController __instance)
            {
                if (tactsuitVr.suitDisabled || !__instance.IsPlayerOne)
                {
                    return;
                }

                //recharging pits
                if (__instance.IsRecharging)
                {
                    ballistiCNG_bHaptics.tactsuitVr.StartRecharging();
                }
                else
                {
                    ballistiCNG_bHaptics.tactsuitVr.StopRecharging();
                }

                //energy critical
                bool energyCritical = Traverse.Create(__instance).Field("_energyCriticalWarned").GetValue<bool>();
                bool energyMoreCritical = Traverse.Create(__instance).Field("_energyEvenMoreCriticalWarned").GetValue<bool>();
                if (energyCritical || energyMoreCritical)
                {
                    ballistiCNG_bHaptics.tactsuitVr.StartHeartBeat();
                    TactsuitVR.heartBeatRate = (energyMoreCritical) ? 500 : 1000;
                }
                else
                {
                    ballistiCNG_bHaptics.tactsuitVr.StopHeartBeat();
                }

                // LAST CHECKED !!
                // acceleration and deceleration
                float speed = __instance.RBody.velocity.sqrMagnitude;
                float speedClass = 100f;
                switch (Race.Speedclass)
                {
                    case ESpeedClass.Toxic:
                        speedClass = __instance.Settings.ENGINE_MAXSPEED_SPARK;
                        break;
                    case ESpeedClass.Apex:
                        speedClass = __instance.Settings.ENGINE_MAXSPEED_TOXIC;
                        break;
                    case ESpeedClass.Halberd:
                        speedClass = __instance.Settings.ENGINE_MAXSPEED_APEX;
                        break;
                    case ESpeedClass.Spectre:
                        speedClass = __instance.Settings.ENGINE_MAXSPEED_HALBERD;
                        break;
                    case ESpeedClass.Zen:
                        speedClass = __instance.Settings.ENGINE_MAXSPEED_SPECTRE;
                        break;
                }
                speedClass *= 10;

                if (speed > 20)
                {
                    if (!__instance.PysSim.isShipBraking)
                    {
                        TactsuitVR.speedEffect = "Acceleration";
                        //when using airbrakes
                        if (isUsingLeftAirBrake || isUsingRightAirBrake)
                        {
                            TactsuitVR.speedIntensity = 0.15f * speed / speedClass;
                            ballistiCNG_bHaptics.tactsuitVr.StopSpeed();
                            ballistiCNG_bHaptics.tactsuitVr.PlaybackHaptics(
                                "AirBrakeVest_"+(isUsingLeftAirBrake ? "L" : "R")
                                , TactsuitVR.speedIntensity);
                            ballistiCNG_bHaptics.tactsuitVr.PlaybackHaptics(
                                "AirBrakeArm_" + (isUsingLeftAirBrake ? "L" : "R")
                                , TactsuitVR.speedIntensity);
                            return;
                        }
                        else
                        {
                            TactsuitVR.speedIntensity = 0.4f * speed / speedClass;
                        }
                    }
                    else
                    {
                        TactsuitVR.speedEffect = "Deceleration";
                        TactsuitVR.speedIntensity = 0.6f * speed / speedClass;
                    }
                    ballistiCNG_bHaptics.tactsuitVr.StartSpeed();
                }
                else
                {
                    ballistiCNG_bHaptics.tactsuitVr.StopSpeed();
                }
            }
        }
    }
}
