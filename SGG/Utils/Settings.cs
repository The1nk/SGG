using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace SGG.Utils
{
    public static class Settings {
        public static string AdbHost = "localhost";
        public static string AdbPort = "1234";
        public static bool CollectAchievements = false;
        public static bool CollectWeeklies = false;
        public static bool StopAtWave6 = false;
        public static Point MapPoint = new Point(179, 612);
        public static string AppRestartInterval = "-1";
        public static string DeviceRestartInterval = "-1";

        public static bool SellerGems = false;
        public static bool SellerOrbs = true;
        public static bool SellerOther = true;

        public static bool MonitorGems = false;
        public static bool MonitorOrbs = true;
        public static bool MonitorAttack = false;
        public static bool MonitorCoins = false;
        public static bool MonitorOther = true;

        public static string WebhookUrl = "";

        public static void Save() {
            var sb = new StringBuilder();
            sb.AppendLine(AdbHost);
            sb.AppendLine(AdbPort);
            sb.AppendLine(CollectAchievements.ToString());
            sb.AppendLine(CollectWeeklies.ToString());
            sb.AppendLine(StopAtWave6.ToString());
            sb.AppendLine(MapPoint.X.ToString());
            sb.AppendLine(MapPoint.Y.ToString());
            sb.AppendLine(AppRestartInterval);
            sb.AppendLine(DeviceRestartInterval);
            
            sb.AppendLine(SellerGems.ToString());
            sb.AppendLine(SellerOrbs.ToString());
            sb.AppendLine(SellerOther.ToString());

            sb.AppendLine(MonitorGems.ToString());
            sb.AppendLine(MonitorOrbs.ToString());
            sb.AppendLine(MonitorAttack.ToString());
            sb.AppendLine(MonitorCoins.ToString());
            sb.AppendLine(MonitorOther.ToString());

            sb.AppendLine(WebhookUrl);

            if (File.Exists("settings.txt"))
                File.Delete("settings.txt");
            File.WriteAllText("settings.txt", sb.ToString());
        }

        public static void Load() {
            if (!File.Exists("settings.txt"))
                return;

            var lines = File.ReadAllLines("settings.txt");
            
            AdbHost = lines[0];
            AdbPort = lines[1];
            CollectAchievements = bool.Parse(lines[2]);
            CollectWeeklies = bool.Parse(lines[3]);
            StopAtWave6 = bool.Parse(lines[4]);
            MapPoint = new Point(int.Parse(lines[5]), int.Parse(lines[6]));
            AppRestartInterval = lines[7];
            DeviceRestartInterval = lines[8];

            SellerGems = bool.Parse(lines[9]);
            SellerOrbs = bool.Parse(lines[10]);
            SellerOther = bool.Parse(lines[11]);

            MonitorGems = bool.Parse(lines[12]);
            MonitorOrbs = bool.Parse(lines[13]);
            MonitorAttack = bool.Parse(lines[14]);
            MonitorCoins = bool.Parse(lines[15]);
            MonitorOther = bool.Parse(lines[16]);

            WebhookUrl = lines[17];
        }
    }
}
