using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MelonLoader;

namespace MyBhapticsTactsuit
{
    public class TactsuitVR
    {
        public bool suitDisabled = true;
        public bool systemInitialized = false;

        public Dictionary<String, FileInfo> FeedbackMap = new Dictionary<String, FileInfo>();
        private static ManualResetEvent HeartBeat_mrse = new ManualResetEvent(false);
        private static ManualResetEvent Recharging_mrse = new ManualResetEvent(false);
        private static ManualResetEvent Speed_mrse = new ManualResetEvent(false);
        public static int heartBeatRate = 1000;
        public static string speedEffect;
        public static float speedIntensity;

        public void SpeedFunc()
        {
            while (true)
            {
                Speed_mrse.WaitOne();
                PlaybackHaptics(speedEffect, speedIntensity);
                Thread.Sleep(50);
            }
        }

        public void HeartBeatFunc()
        {
            while (true)
            {
                HeartBeat_mrse.WaitOne();
                PlaybackHaptics("HeartBeat");
                Thread.Sleep(heartBeatRate);
            }
        }
        public void RechargingFunc()
        {
            while (true)
            {
                Recharging_mrse.WaitOne();
                PlaybackHaptics("Heal");
                Thread.Sleep(1000);
            }
        }

        public TactsuitVR()
        {
            LOG("Initializing suit");
            if (!bHaptics.WasError)
            {
                suitDisabled = false;
            }
            RegisterAllTactFiles();
            LOG("Starting HeartBeat thread...");
            Thread HeartBeatThread = new Thread(HeartBeatFunc);
            HeartBeatThread.Start();
            Thread RechargingThread = new Thread(RechargingFunc);
            RechargingThread.Start();
            Thread SpeedThread = new Thread(SpeedFunc);
            SpeedThread.Start();
        }

        public void LOG(string logStr)
        {
            MelonLogger.Msg(logStr);
        }

        void RegisterAllTactFiles()
        {
            string configPath = Directory.GetCurrentDirectory() + "\\Mods\\bHaptics";
            DirectoryInfo d = new DirectoryInfo(configPath);
            FileInfo[] Files = d.GetFiles("*.tact", SearchOption.AllDirectories);
            for (int i = 0; i < Files.Length; i++)
            {
                string filename = Files[i].Name;
                string fullName = Files[i].FullName;
                string prefix = Path.GetFileNameWithoutExtension(filename);
                if (filename == "." || filename == "..")
                    continue;
                string tactFileStr = File.ReadAllText(fullName);
                try
                {
                    bHapticsLib.bHapticsManager.RegisterPatternFromJson(prefix, tactFileStr);
                    LOG("Pattern registered: " + prefix);
                }
                catch (Exception e) { LOG(e.ToString()); }

                FeedbackMap.Add(prefix, Files[i]);
            }
            systemInitialized = true;
        }

        public void PlaybackHaptics(String key, float intensity = 1.0f, float duration = 1.0f)
        {
            if (FeedbackMap.ContainsKey(key))
            {
                bHapticsLib.ScaleOption scaleOption = new bHapticsLib.ScaleOption(intensity, duration);
                bHapticsLib.bHapticsManager.PlayRegistered(key, key, scaleOption);
            }
            else
            {
                LOG("Feedback not registered: " + key);
            }
        }
        public void StartRecharging()
        {
            Recharging_mrse.Set();
        }

        public void StopRecharging()
        {
            Recharging_mrse.Reset();
        }
        public void StartHeartBeat()
        {
            HeartBeat_mrse.Set();
        }

        public void StopHeartBeat()
        {
            HeartBeat_mrse.Reset();
        }
        public void StartSpeed()
        {
            Speed_mrse.Set();
        }

        public void StopSpeed()
        {
            Speed_mrse.Reset();
        }

        public bool IsPlaying(String effect)
        {
            return bHapticsLib.bHapticsManager.IsPlaying(effect);
        }

        public void StopHapticFeedback(String effect)
        {
            bHapticsLib.bHapticsManager.StopPlaying(effect);
        }

        public void StopAllHapticFeedback()
        {
            StopThreads();
            foreach (String key in FeedbackMap.Keys)
            {
                bHapticsLib.bHapticsManager.StopPlaying(key);
            }
        }

        public void StopThreads()
        {
            StopHeartBeat();
            StopRecharging();
            StopSpeed();
        }


    }
}
