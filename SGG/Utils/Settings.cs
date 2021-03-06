using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Newtonsoft.Json;
using CallConvThiscall = System.Runtime.CompilerServices.CallConvThiscall;

namespace SGG.Utils
{
    public class SettingsInstance {
        public string AdbHost = "localhost";
        public string AdbPort = "1234";
        public bool ShowImages = true;
        public int FramesPerSecondToSave = 5;
        public int SecondsToSave = 30;

        public Point MapPoint = new Point(179, 612);
        public bool CollectAchievements = false;
        public bool CollectWeeklies = false;
        public bool StopAtWave6 = false;
        public bool SleepySummonerMode = true;
        public bool CollectOfflineGold = false;

        public string AppRestartInterval = "-1";
        public string DeviceRestartInterval = "-1";
        public string ResetAfterXMinutesOfNoActivity = "10";

        public bool SellerGems = false;
        public bool SellerOrbs = true;
        public bool SellerOther = true;

        public bool MonitorGems = false;
        public bool MonitorOrbs = true;
        public bool MonitorAttack = false;
        public bool MonitorCoins = false;
        public bool MonitorOther = true;

        public string WebhookUrl = "";

        public SettingsInstance() {
        }
    }

    public static class Settings {
        private static readonly string _settingsFile;

        public static SettingsInstance Instance {
            get;
            private set;
        }

        static Settings() {
            _settingsFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");
            Instance = new SettingsInstance();
        }

        public static void Save() {
            if (File.Exists(_settingsFile))
                File.Delete(_settingsFile);
            
            File.WriteAllText(_settingsFile, JsonConvert.SerializeObject(Settings.Instance));
        }

        public static void Load() {
            var tmp = new SettingsInstance();

            if (!File.Exists(_settingsFile))
                return;

            var text = File.ReadAllText(_settingsFile);
            try {
                Instance = JsonConvert.DeserializeObject<SettingsInstance>(text);
            }
            catch {
                Instance = new SettingsInstance();
                // ignore
            }

            Instance ??= new SettingsInstance();

            Instance.MonitorAttack = false;
            Instance.MonitorCoins = false;
            Instance.MonitorGems = false;
            Instance.MonitorOther = false;
            Instance.MonitorOrbs = false;

            if (Instance.FramesPerSecondToSave == 0)
                Instance.FramesPerSecondToSave = tmp.FramesPerSecondToSave;

            if (Instance.SecondsToSave == 0)
                Instance.SecondsToSave = tmp.SecondsToSave;
        }
    }
}
