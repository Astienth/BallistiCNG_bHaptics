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
using System.IO;
using NgPickups;
using NgPickups.Physical;

namespace BallistiCNG_bHaptics
{
    public class ballistiCNG_bHaptics : MelonMod
    {
        public static TactsuitVR tactsuitVr;
        public static bool isUsingLeftAirBrake = false;
        public static bool isUsingRightAirBrake = false;
        private MelonPreferences_Category UserConfig;
        public static int speedIntensityPercentage;
        public static MelonPreferences_Entry<int> userValue;

        public override void OnInitializeMelon()
        {
            tactsuitVr = new TactsuitVR();
            LoggerInstance.Msg("Mods bHaptics loaded");
            tactsuitVr.PlaybackHaptics("HeartBeat");
            //Config
            if(!Directory.Exists("UserConfig"))
            {
                Directory.CreateDirectory("UserConfig");
            }
            UserConfig = MelonPreferences.CreateCategory("UserConfig");
            userValue = UserConfig.CreateEntry<int>("speedIntensityPercentage", 75);
            UserConfig.SetFilePath("UserConfig/BallistiCNG_bHaptics.cfg");
            UserConfig.SaveToFile();
            if (userValue.Value <= 200 && userValue.Value > 0)
            {
                speedIntensityPercentage = userValue.Value;
            }
            else
            {
                speedIntensityPercentage = 75;
                LoggerInstance.Msg("Invalid user speedIntensity value in config file, default to 75 percent");
            }
        }

        public override void OnLateInitializeMelon()
        {          
            NgRaceEvents.OnEventComplete += new NgRaceEvents.EventCompleteDelegate(this.OnGamePaused);
            NgUiEvents.OnGamePause += new NgUiEvents.GamePausedDelegate(this.OnGamePaused);
            MelonEvents.OnApplicationQuit.Subscribe(FixWeirdUserFolder);
        }

        public void FixWeirdUserFolder()
        {
            //trying to fix weird bug with melonloader
            for (int i = 1; i <= 10; i++)
            {
                if (Directory.Exists("UserData_BCK"+i))
                {
                    if (Directory.Exists("UserData"))
                    {
                        Directory.Delete("UserData", true);
                    }
                    Directory.Move("UserData_BCK"+i, "UserData");
                }
            }
        }

        public void OnGamePaused()
        {
            tactsuitVr.StopThreads();
        }

        #region WEAPONS

        [HarmonyPatch(typeof(PickupRockets), "OnUse")]
        public class bhaptics_OnUsePickupRockets
        {
            [HarmonyPostfix]
            public static void Postfix(PickupRockets __instance)
            {
                if (tactsuitVr.suitDisabled
                    || !__instance.R.IsPlayer)
                {
                    return;
                }
                ballistiCNG_bHaptics.tactsuitVr.PlaybackHaptics("RecoilVest");
                ballistiCNG_bHaptics.tactsuitVr.PlaybackHaptics("RecoilArm");
            }
        }

        [HarmonyPatch(typeof(PickupMissiles), "OnUse")]
        public class bhaptics_OnUsePickupMissiles
        {
            [HarmonyPostfix]
            public static void Postfix(PickupMissiles __instance)
            {
                if (tactsuitVr.suitDisabled
                    || !__instance.R.IsPlayer)
                {
                    return;
                }
                ballistiCNG_bHaptics.tactsuitVr.PlaybackHaptics("RecoilVest");
                ballistiCNG_bHaptics.tactsuitVr.PlaybackHaptics("RecoilArm");
            }
        }

        [HarmonyPatch(typeof(PickupHunter), "OnUse")]
        public class bhaptics_OnUsePickupHunter
        {
            [HarmonyPostfix]
            public static void Postfix(PickupHunter __instance)
            {
                if (tactsuitVr.suitDisabled
                    || !__instance.R.IsPlayer)
                {
                    return;
                }
                ballistiCNG_bHaptics.tactsuitVr.PlaybackHaptics("RecoilVest", 0.5f);
                ballistiCNG_bHaptics.tactsuitVr.PlaybackHaptics("RecoilArm", 0.5f);
            }
        }
        
        [HarmonyPatch(typeof(PickupQuake), "OnUse")]
        public class bhaptics_OnUsePickupQuake
        {
            [HarmonyPostfix]
            public static void Postfix(PickupQuake __instance)
            {
                if (tactsuitVr.suitDisabled
                    || !__instance.R.IsPlayer)
                {
                    return;
                }
                ballistiCNG_bHaptics.tactsuitVr.PlaybackHaptics("RecoilVest");
                ballistiCNG_bHaptics.tactsuitVr.PlaybackHaptics("RecoilArm");
            }
        }

        [HarmonyPatch(typeof(PickupHellstorm), "OnUse")]
        public class bhaptics_OnUsePickupHellstorm
        {
            [HarmonyPostfix]
            public static void Postfix(PickupHellstorm __instance)
            {
                if (tactsuitVr.suitDisabled
                    || !__instance.R.IsPlayer)
                {
                    return;
                }
                ballistiCNG_bHaptics.tactsuitVr.PlaybackHaptics("RecoilVest", 0.5f);
                ballistiCNG_bHaptics.tactsuitVr.PlaybackHaptics("RecoilArm", 0.5f);
            }
        }

        [HarmonyPatch(typeof(CannonShot), "Spawn")]
        public class bhaptics_OnUseCannonShot
        {
            [HarmonyPostfix]
            public static void Postfix(ShipController r)
            {
                if (tactsuitVr.suitDisabled
                    || !r.IsPlayer)
                {
                    return;
                }
                ballistiCNG_bHaptics.tactsuitVr.PlaybackHaptics("RecoilVest", 0.4f);
                ballistiCNG_bHaptics.tactsuitVr.PlaybackHaptics("RecoilArm", 0.4f);
            }
        }

        [HarmonyPatch(typeof(PlasmaShot), "Spawn")]
        public class bhaptics_OnUsePlasmaShot
        {
            [HarmonyPostfix]
            public static void Postfix(ShipController r)
            {
                if (tactsuitVr.suitDisabled
                    || !r.IsPlayer)
                {
                    return;
                }
                ballistiCNG_bHaptics.tactsuitVr.PlaybackHaptics("RecoilVest", 2f);
                ballistiCNG_bHaptics.tactsuitVr.PlaybackHaptics("RecoilArm", 2f);
            }
        }        

       [HarmonyPatch(typeof(PickupBase), "OnAbsorb")]
        public class bhaptics_OnAbsordPickup
        {
            [HarmonyPostfix]
            public static void Postfix(PickupBase __instance)
            {
                if (tactsuitVr.suitDisabled
                    || !__instance.R.IsPlayer || __instance.AbsorbAmount == 0)
                {
                    return;
                }
                ballistiCNG_bHaptics.tactsuitVr.PlaybackHaptics("Heal");
            }
        }

        #endregion

        #region PHYSICS

        [HarmonyPatch(typeof(ShipController), "OnEliminated")]
        public class bhaptics_OnEliminated
        {
            [HarmonyPostfix]
            public static void Postfix(ShipController __instance)
            {
                if (tactsuitVr.suitDisabled || !__instance.IsPlayer)
                {
                    return;
                }
                ballistiCNG_bHaptics.tactsuitVr.PlaybackHaptics("Death");
                tactsuitVr.StopThreads();
            }
        }

        [HarmonyPatch(typeof(ShipInput), "RumbleUpdate")]
        public class bhaptics_CheckBoostStart
        {
            [HarmonyPostfix]
            public static void Postfix(ShipInput __instance)
            {
                if (tactsuitVr.suitDisabled || !__instance.r.IsPlayer)
                {
                    return;
                }
                if(!Race.HasCountdownFinished && (double)__instance.r.PysSim.enginePower > 0.600000023841858 
                    && (double)__instance.r.PysSim.enginePower < 0.800000011920929 
                    && !Cheats.ModernPhysics)
                {
                    ballistiCNG_bHaptics.tactsuitVr.PlaybackHaptics("RecoilVest", 0.1f);
                    ballistiCNG_bHaptics.tactsuitVr.PlaybackHaptics("RecoilArm", 0.1f);
                }
            }
        }

        [HarmonyPatch(typeof(ShipController), "TakeDamage")]
        public class bhaptics_TakeDamage
        {
            [HarmonyPostfix]
            public static void Postfix(ShipController __instance, bool __result, float amount)
            {
                if (tactsuitVr.suitDisabled || !__instance.IsPlayer
                    || !__result || amount < 1.5f)
                {
                    return;
                }
                ballistiCNG_bHaptics.tactsuitVr.PlaybackHaptics("VehicleImpact_Vest", 1.5f);
                ballistiCNG_bHaptics.tactsuitVr.PlaybackHaptics("VehicleImpact_Arms", 1.5f);
            }
        }

        [HarmonyPatch(typeof(ShipInput), "PlayerInput")]
        public class bhaptics_ShipAirbrake
        {
            [HarmonyPostfix]
            public static void Postfix(ShipInput __instance)
            {
                if (tactsuitVr.suitDisabled 
                    || !__instance.r.IsPlayer)
                {
                    return;
                }
                isUsingLeftAirBrake = __instance.AxisLeftAirbrake != 0;
                isUsingRightAirBrake = __instance.AxisRightAirbrake != 0;               
            }
        }
        
       [HarmonyPatch(typeof(ShipController), "FixedUpdate")]
        public class bhaptics_FixedUpdate
        {
            [HarmonyPostfix]
            public static void Postfix(ShipController __instance)
            {
                if (tactsuitVr.suitDisabled || !__instance.IsPlayer)
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
                    if (__instance.Eliminated)
                    {
                        ballistiCNG_bHaptics.tactsuitVr.StopThreads();
                        return;
                    }
                    else
                    {
                        ballistiCNG_bHaptics.tactsuitVr.StartHeartBeat();
                        TactsuitVR.heartBeatRate = (energyMoreCritical) ? 500 : 1000;
                    }

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
                            TactsuitVR.speedIntensity = (float) speedIntensityPercentage
                                * speed / (speedClass * 10) / 25;

                            ballistiCNG_bHaptics.tactsuitVr.StopSpeed();
                            ballistiCNG_bHaptics.tactsuitVr.PlaybackHaptics(
                                "AirBrakeVest_"+(isUsingLeftAirBrake ? "L" : "R")
                                , TactsuitVR.speedIntensity * 4.5f);
                            ballistiCNG_bHaptics.tactsuitVr.PlaybackHaptics(
                                "AirBrakeArm_" + (isUsingLeftAirBrake ? "L" : "R")
                                , TactsuitVR.speedIntensity * 1.5f);
                        }
                        else
                        {
                            TactsuitVR.speedIntensity = (float)speedIntensityPercentage
                                * speed / (speedClass * 10) / 10;
                        }
                    }
                    else
                    {
                        TactsuitVR.speedEffect = "Deceleration";
                        TactsuitVR.speedIntensity = 4f * speed / speedClass;
                    }
                    ballistiCNG_bHaptics.tactsuitVr.StartSpeed();
                }
                else
                {
                    ballistiCNG_bHaptics.tactsuitVr.StopSpeed();
                }
            }
        }

        #endregion
    }
}
