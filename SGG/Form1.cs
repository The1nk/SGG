using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SGG.Models;
using SGG.Utils;
using SharpAdbClient;
using SharpAdbClient.DeviceCommands;
using SixteenTons.Utils;
using SixteenTons.Workers;

namespace ScratchPad {
    public partial class Form1 : Form {
        private bool _initialized;
        private AdbClient _client;
        private DeviceData _device;
        private readonly Queue<Image> _lastCaptures;
        private ScreenCaptureWorker _screenCaptureWorker;
        private DateTime _nextActionAvailableAt;
        private DateTime _lastAction;
        private readonly object _lockObject = new object();
        private bool _running;
        private int _currentRun = 0;
        private int _currentRunAbs = 0;
        private bool _disableAutoSave = false;

        private ContextMenuStrip _contextMenu;

        private Point _selectedMapPoint = new Point(179, 612);
        private DateTime _waitUntilNowForSleepySummoner = DateTime.MinValue;
        private int _restarts = 0;

        public Form1() {
            InitializeComponent();
            _lastCaptures = new Queue<Image>();
            
            Settings.Load();

            _disableAutoSave = true;
            tbAdbHost.Text = Settings.Instance.AdbHost;
            tbAdbPort.Text = Settings.Instance.AdbPort;
            cbShowImages.Checked = Settings.Instance.ShowImages;
            
            _selectedMapPoint = Settings.Instance.MapPoint;
            cbCollectAchievements.Checked = Settings.Instance.CollectAchievements;
            cbCollectWeeklies.Checked = Settings.Instance.CollectWeeklies;
            cbWave6.Checked = Settings.Instance.StopAtWave6;
            cbSleepy.Checked = Settings.Instance.SleepySummonerMode;
            cbOfflineGold.Checked = Settings.Instance.CollectOfflineGold;

            tbAppRestartInterval.Text = Settings.Instance.AppRestartInterval;
            tbDeviceRestartInterval.Text = Settings.Instance.DeviceRestartInterval;
            tbResetAfterMinutesWithoutActivity.Text = Settings.Instance.ResetAfterXMinutesOfNoActivity;

            cbSellerGems.Checked = Settings.Instance.SellerGems;
            cbSellerOrbs.Checked = Settings.Instance.SellerOrbs;
            cbSellerOther.Checked = Settings.Instance.SellerOther;

            cbMonitorAttack.Checked = Settings.Instance.MonitorAttack;
            cbMonitorCoins.Checked = Settings.Instance.MonitorCoins;
            cbMonitorGems.Checked = Settings.Instance.MonitorGems;
            cbMonitorOrbs.Checked = Settings.Instance.MonitorOrbs;
            cbMonitorOther.Checked = Settings.Instance.MonitorOther;
            
            tbDiscordHookUrl.Text = Settings.Instance.WebhookUrl;
            
            _disableAutoSave = false;
        }

        private void UpdateImage(Image obj) {
            Image clone;
            lock (obj) {
                clone = (Image) obj.Clone();
            }

            if (Settings.Instance.ShowImages)
                pictureBox1.Image = clone;

            _lastCaptures.Enqueue(clone);
            while (_lastCaptures.Count > 100)
                _lastCaptures.Dequeue();
        }

        private void Init() {
            SaveSettings();

            if (_initialized)
                return;

            var z = ImageTemplates.GetByType(ImageTemplates.TemplateType.SpeedMult1x);

            try {
                var ep = new DnsEndPoint(tbAdbHost.Text, int.Parse(tbAdbPort.Text));
                _client = new AdbClient();
                _client.Connect(ep);
                _device = _client.GetDevices().First();

                _screenCaptureWorker = new ScreenCaptureWorker(_client, _device);
                _screenCaptureWorker.NewImageCaptured += UpdateImage;
                _screenCaptureWorker.NewImageCaptured += RunBot;

                _initialized = true;
            }
            catch (Exception ex) {
                _initialized = false;

                string msg =
                    ex.Message.StartsWith("No connection could be made because the target machine actively refused it.")
                        ? $"Unable to connect to ADB server. Please ensure it's running.\r\n\r\n{ex.Message}"
                        : ex.Message;
                DiscordLogger.Log(DiscordLogger.MessageType.Error, msg);
                MessageBox.Show(msg);
            }
        }

        private void SaveSettings() {
            DiscordLogger.SetUrl(tbDiscordHookUrl.Text);

            Settings.Instance.AdbHost =  tbAdbHost.Text;
            Settings.Instance.AdbPort =  tbAdbPort.Text;
            Settings.Instance.ShowImages = cbShowImages.Checked;
            
            Settings.Instance.MapPoint =  _selectedMapPoint;
            Settings.Instance.CollectAchievements =  cbCollectAchievements.Checked;
            Settings.Instance.CollectWeeklies =  cbCollectWeeklies.Checked;
            Settings.Instance.StopAtWave6 =  cbWave6.Checked;
            Settings.Instance.SleepySummonerMode = cbSleepy.Checked;
            Settings.Instance.CollectOfflineGold = cbOfflineGold.Checked;

            Settings.Instance.AppRestartInterval =  tbAppRestartInterval.Text;
            Settings.Instance.DeviceRestartInterval =  tbDeviceRestartInterval.Text;
            Settings.Instance.ResetAfterXMinutesOfNoActivity = tbResetAfterMinutesWithoutActivity.Text;

            Settings.Instance.SellerGems =  cbSellerGems.Checked;
            Settings.Instance.SellerOrbs =  cbSellerOrbs.Checked;
            Settings.Instance.SellerOther =  cbSellerOther.Checked;
            
            Settings.Instance.MonitorAttack =  cbMonitorAttack.Checked;
            Settings.Instance.MonitorCoins =  cbMonitorCoins.Checked;
            Settings.Instance.MonitorGems =  cbMonitorGems.Checked;
            Settings.Instance.MonitorOrbs =  cbMonitorOrbs.Checked;
            Settings.Instance.MonitorOther =  cbMonitorOther.Checked;
            
            Settings.Instance.WebhookUrl =  tbDiscordHookUrl.Text;

            Settings.Save();
        }

        private void RecordLastActionAndSetDelay(int millisecondsDelay = 0) {
            _nextActionAvailableAt = DateTime.Now.AddMilliseconds(millisecondsDelay);
            _lastAction = DateTime.Now;
        }

        private async void RunBot(Image obj) {
            try {
                if (_running || DateTime.Now <= _nextActionAvailableAt)
                    return;

                lock (_lockObject) {
                    if (_running)
                        return;

                    _running = true;
                }
                
                Bitmap bmp;
                lock (obj) {
                    bmp = new Bitmap((Image) obj.Clone());

                }

                if (await CheckForStuckApp()) return;
                if (await CheckForMotd(bmp)) return;
                if (await CheckForOffer(bmp)) return;

                if (Settings.Instance.SleepySummonerMode) {
                    if (DateTime.Now < _waitUntilNowForSleepySummoner)
                        return;
                }

                if (await CheckForAd(ImageTemplates.TemplateType.Ad_Attack, bmp, Settings.Instance.MonitorAttack)) return;
                if (await CheckForAd(ImageTemplates.TemplateType.Ad_Coins, bmp, Settings.Instance.MonitorCoins)) return;
                if (await CheckForAd(ImageTemplates.TemplateType.Ad_Gems, bmp, Settings.Instance.MonitorGems)) return;
                if (await CheckForAd(ImageTemplates.TemplateType.Ad_Orbs, bmp, Settings.Instance.MonitorOrbs)) return;
                if (await CheckForAd(ImageTemplates.TemplateType.Ad_Stones, bmp, Settings.Instance.MonitorOther)) return;

                if (await CheckForSale(ImageTemplates.TemplateType.Seller_Gems, bmp, Settings.Instance.SellerGems)) return;
                if (await CheckForSale(ImageTemplates.TemplateType.Seller_Orbs, bmp, Settings.Instance.SellerOrbs)) return;
                if (await CheckForSale(ImageTemplates.TemplateType.Seller_Stones, bmp, Settings.Instance.SellerOther)) return;

                if (Settings.Instance.SleepySummonerMode && await CheckForSmiles(bmp))
                    return;
                
                if (await CheckForOfflineGold(bmp)) return;
                if (await CheckForWave6Reset(bmp)) return;
                if (await CheckForWin(bmp)) return;
                if (await CheckForLose(bmp)) return;
                if (await CheckForMapSelect(bmp)) return;
                if (await CheckForConfirm(bmp)) return;

                if (await CheckForSpeedMultiplier(bmp)) return;
            }
            catch (Exception ex) {
                DiscordLogger.Log(DiscordLogger.MessageType.Error, $"{ex.Message}\r\n{ex.StackTrace}");
            }
            finally {
                lock (_lockObject) {
                    _running = false;
                }
            }
        }

        private async Task<bool> CheckForStuckApp() {
            if (Settings.Instance.ResetAfterXMinutesOfNoActivity == "-1") return false;

            if (_lastAction.AddMinutes(Double.Parse(Settings.Instance.ResetAfterXMinutesOfNoActivity)) >
                DateTime.Now) return false;

            DiscordLogger.Log(DiscordLogger.MessageType.Info,
                $"Threshold of {Settings.Instance.ResetAfterXMinutesOfNoActivity} minutes with no activity reached. Restarting app.");
            CloseAndRestartApp();
            RecordLastActionAndSetDelay(0);
            return true;
        }

        private async Task<bool> CheckForOfflineGold(Image image) {
            if (!Settings.Instance.CollectOfflineGold) return false;
            if (!ImageTemplates.GetByType(ImageTemplates.TemplateType.OfflineGold).IsPresentOn(image)) return false;

            DiscordLogger.Log(DiscordLogger.MessageType.Info, "Collecting offline gold");
            ClickAt(448, 1204, 1);
            ClickAt(250, 1084);
            ClickAt(250, 900);
            RecordLastActionAndSetDelay(200);
            return false;
        }

        private async Task<bool> CheckForAd(ImageTemplates.TemplateType adType, Image clone, bool enabled) {
            var ret = ImageTemplates.GetByType(adType).IsPresentOn(clone);
            
            if (ret) {
                if (enabled) {
                    DiscordLogger.Log(DiscordLogger.MessageType.Info, $"Watching Monitor for {adType}");
                    ClickAt(494, 915, 45);
                    SendBackButton(2);
                    ClickAt(494, 915);
                }
                else {
                    DiscordLogger.Log(DiscordLogger.MessageType.Debug, $"Skipping Monitor for {adType}");
                    ClickAt(243, 847);
                    RecordLastActionAndSetDelay(2000);
                }
            }

            return ret;
        }

        private void SendBackButton(int secondsDelay) {
            _client.ExecuteShellCommand(_device, $"input keyevent 4;sleep {secondsDelay}", null);
        }

        private async Task<bool> CheckForSale(ImageTemplates.TemplateType saleType,  Image clone, bool enabled) {
            var ret = ImageTemplates.GetByType(saleType).IsPresentOn( clone);

            if (ret) {
                if (enabled) {
                    DiscordLogger.Log(DiscordLogger.MessageType.Info, $"Accepting Seller for {saleType}");
                    ClickAt(494, 915, 1, 1.5);
                    ClickAt(494, 915);
                    RecordLastActionAndSetDelay(2000);
                }
                else {
                    DiscordLogger.Log(DiscordLogger.MessageType.Debug, $"Skipping Seller for {saleType}");
                    ClickAt(243, 847);
                    RecordLastActionAndSetDelay(2000);
                }
            }

            return ret;
        }

        private async Task<bool> CheckForConfirm(Image image) {
            if (!ImageTemplates.GetByType(ImageTemplates.TemplateType.ConfirmLayout).IsPresentOn(image)) return false;

            // Dismiss
            DiscordLogger.Log(DiscordLogger.MessageType.Info, "Confirming layout");
            ClickAt(433, 1505);
            RecordLastActionAndSetDelay(500);
            return true;
        }

        private async Task<bool> CheckForLose(Image image) {
            if (!ImageTemplates.GetByType(ImageTemplates.TemplateType.LoseScreen).IsPresentOn(image)) return false;

            // Dismiss
            DiscordLogger.Log(DiscordLogger.MessageType.Info, "Dismissing 'Lose' screen");
            ClickAt(252, 1211, 2);
            ClickAt(283, 1059);
            RecordLastActionAndSetDelay(1000);
            return true;
        }

        private async Task<bool> CheckForWin(Image image) {
            if (!ImageTemplates.GetByType(ImageTemplates.TemplateType.WinScreen).IsPresentOn(image)) return false;

            // Dismiss
            DiscordLogger.Log(DiscordLogger.MessageType.Info, "Dismissing 'Win' screen");
            ClickAt(444, 1222);
            RecordLastActionAndSetDelay(5000);
            return true;
        }

        private async Task<bool> CheckForWave6Reset(Image image) {
            if (!Settings.Instance.StopAtWave6) return false;

            if (!ImageTemplates.GetByType(ImageTemplates.TemplateType.On_A_Map).IsPresentOn(image)) return false;
            if (ImageTemplates.GetByType(ImageTemplates.TemplateType.Wave1).IsPresentOn(image)) return false;
            if (ImageTemplates.GetByType(ImageTemplates.TemplateType.Wave2).IsPresentOn(image)) return false;
            if (ImageTemplates.GetByType(ImageTemplates.TemplateType.Wave3).IsPresentOn(image)) return false;
            if (ImageTemplates.GetByType(ImageTemplates.TemplateType.Wave4).IsPresentOn(image)) return false;
            if (ImageTemplates.GetByType(ImageTemplates.TemplateType.Wave5).IsPresentOn(image)) return false;

            // Pop map select
            DiscordLogger.Log(DiscordLogger.MessageType.Info, "Resetting after Wave 6");
            ClickAt(790, 47);
            RecordLastActionAndSetDelay(2000);
            return true;
        }

        private async Task<bool> CheckForMapSelect(Image image) {
            if (!ImageTemplates.GetByType(ImageTemplates.TemplateType.MapSelect).IsPresentOn(image)) return false;

            int.TryParse(tbAppRestartInterval.Text.Trim(), out var restartInterval);

            _currentRun++;
            _currentRunAbs++;
            DiscordLogger.Log(DiscordLogger.MessageType.Info, $"Starting run #{_currentRunAbs:#,##0}..");

            if (restartInterval > 0) {
                DiscordLogger.Log(DiscordLogger.MessageType.Info,
                    $"Run #{_currentRun:#,##0} of {restartInterval:#,##0}..");

                if (_currentRun > restartInterval) {
                    DiscordLogger.Log(DiscordLogger.MessageType.Info,
                        "Threshold of consecutive runs met, restarting app");
                    CloseAndRestartApp();
                }
            }

            // Dismiss
            DiscordLogger.Log(DiscordLogger.MessageType.Info, "Selecting map");
            ClickAt(_selectedMapPoint.X, _selectedMapPoint.Y);
            RecordLastActionAndSetDelay(2000);
            return true;
        }

        private void CloseAndRestartApp() {
            // Kill app
            DiscordLogger.Log(DiscordLogger.MessageType.Info, $"Closing and re-opening app");
            _client.ExecuteShellCommand(_device, "am force-stop com.pixio.google.mtd;sleep 5;monkey -p com.pixio.google.mtd 15;sleep 5", null);
            DiscordLogger.Log(DiscordLogger.MessageType.Debug, $"App should be open");
            _currentRun = 0;
            _restarts++;
        }

        private async Task<bool> CheckForOffer(Image image) {
            if (!ImageTemplates.GetByType(ImageTemplates.TemplateType.OfferPopup).IsPresentOn(image)) return false;

            // Dismiss
            DiscordLogger.Log(DiscordLogger.MessageType.Debug, "Dismissing sale popup");
            ClickAt(835, 277);
            RecordLastActionAndSetDelay(2000);
            return true;
        }

        private async Task<bool> CheckForSpeedMultiplier(Image image) {
            if (!ImageTemplates.GetByType(ImageTemplates.TemplateType.SpeedMult1x).IsPresentOn(image)) return false;

            // Toggle it
            DiscordLogger.Log(DiscordLogger.MessageType.Info, "Enabling 2x speed");
            ToggleSpeed();
            return true;
        }

        private async Task<bool> CheckForSpeedMultiplier2x(Image image) {
            if (!ImageTemplates.GetByType(ImageTemplates.TemplateType.SpeedMult2x).IsPresentOn(image)) {
                DiscordLogger.Log(DiscordLogger.MessageType.Info, "Couldn't find 2x..");
                return false;
            }

            // Toggle it
            DiscordLogger.Log(DiscordLogger.MessageType.Info, "Disabling 2x speed");
            ToggleSpeed();
            return true;
        }

        private async Task<bool> CheckForSmiles(Image image) {
            if (!ImageTemplates.GetByType(ImageTemplates.TemplateType.SmilingSummoner).IsPresentOn(image)) return false;

            // Toggle it
            DiscordLogger.Log(DiscordLogger.MessageType.Info, "Summoner is smiling! Singing lullabies..");
            if (!await CheckForSpeedMultiplier2x(image)) return false;

            WaitForSleeping();
            return true;
        }

        private void ToggleSpeed() {
            ClickAt(69, 1394);
            RecordLastActionAndSetDelay(2000);
        }

        private async Task<bool> CheckForMotd(Image image) {
            if (!ImageTemplates.GetByType(ImageTemplates.TemplateType.MotdPopup).IsPresentOn(image)) return false;

            // Dismiss it!
            DiscordLogger.Log(DiscordLogger.MessageType.Debug, "Dismissing MOTD");
            ClickAt(844, 307);
            RecordLastActionAndSetDelay(2000);
            return true;
        }

        private void WaitForSleeping() {
            _waitUntilNowForSleepySummoner = DateTime.Now.AddSeconds(40);
        }

        private void ClickAt(int x, int y) {
            _client.ExecuteShellCommand(_device, $"input tap {x} {y}", null);
        }

        private void ClickAt(int x, int y, int secondsDelay, double prefixDelay = 0) {
            var command = $"input tap {x} {y}";
            if (prefixDelay != 0)
                command = $"sleep {prefixDelay};" + command;
            if (secondsDelay != 0)
                command += $";sleep {secondsDelay}";

            _client.ExecuteShellCommand(_device, command, null);
        }

        private void Start_Click(object sender, EventArgs e) {
            Init();
            if (btnStart.Text == "Start") {
                RecordLastActionAndSetDelay();
                DiscordLogger.Log(DiscordLogger.MessageType.Info, "Starting bot");
                _screenCaptureWorker.Start();
                btnStart.Text = "Stop";
            }
            else if (btnStart.Text == "Stop") {
                DiscordLogger.Log(DiscordLogger.MessageType.Info, "Stopping bot");
                _screenCaptureWorker.Stop();
                btnStart.Text = "Start";
            }
        }

        private void PictureBox1_Click(object sender, EventArgs e) {
            var mee = e as MouseEventArgs;
            if (mee.Button == MouseButtons.Middle) {
                var dbg = $"{mee.X}.{mee.Y}";
                dbg += "\r\n";
                var pt = Utils.TranslateZoomMousePosition(pictureBox1.Image, pictureBox1, mee.Location);
                dbg += $"{pt.X}.{pt.Y}";
                MessageBox.Show(dbg);
            } else if (mee.Button == MouseButtons.Right) {
                var counter = 1;
                var path = ("C:\\SSG Screens");
                if (System.IO.Directory.Exists(path))
                    System.IO.Directory.Delete(path, true);

                if (!System.IO.Directory.Exists(path))
                    System.IO.Directory.CreateDirectory(path);

                while (_lastCaptures.TryDequeue(out var img)) {
                    img.Save(System.IO.Path.Combine(path, $"img{counter:0000}.bmp"));
                    counter++;
                }
            }
        }

        private void Set_Click(object sender, EventArgs e) {
            if (_contextMenu == null) {
                var cmenu = new ContextMenuStrip();
                cmenu.Font = new Font(FontFamily.GenericMonospace, cmenu.Font.SizeInPoints);

                cmenu.Items.Add("The King                 Normal").Click += (o, args) => {
                    _selectedMapPoint = new Point(171, 606);
                    SaveSettings();
                };
                cmenu.Items.Add("The King                 Hard").Click += (o, args) => {
                    _selectedMapPoint = new Point(451, 606);
                    SaveSettings();
                };
                cmenu.Items.Add("The King                 Nightmare").Click += (o, args) => {
                    _selectedMapPoint = new Point(732, 606);
                    SaveSettings();
                };
                cmenu.Items.Add("-------------");

                cmenu.Items.Add("Ragefist Chieftain       Normal").Click += (o, args) => {
                    _selectedMapPoint = new Point(171, 955);
                    SaveSettings();
                };
                cmenu.Items.Add("Ragefist Chieftain       Hard").Click += (o, args) => {
                    _selectedMapPoint = new Point(451, 955);
                    SaveSettings();
                };
                cmenu.Items.Add("Ragefist Chieftain       Nightmare").Click += (o, args) => {
                    _selectedMapPoint = new Point(732, 955);
                    SaveSettings();
                };
                cmenu.Items.Add("-------------");

                cmenu.Items.Add("The Joint Revenge        Normal").Click += (o, args) => {
                    _selectedMapPoint = new Point(171, 1266);
                    SaveSettings();
                };
                cmenu.Items.Add("The Joint Revenge        Hard").Click += (o, args) => {
                    _selectedMapPoint = new Point(451, 1266);
                    SaveSettings();
                };
                cmenu.Items.Add("The Joint Revenge        Nightmare").Click += (o, args) => {
                    _selectedMapPoint = new Point(732, 1266);
                    SaveSettings();
                };


                _contextMenu = cmenu;
            }
            
            _contextMenu.Show(btnSet, 0, 0);
        }

        private void TextBox_Leave(object sender, EventArgs e)
        {
            if (!_disableAutoSave)
                SaveSettings();
        }

        private void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!_disableAutoSave)
                SaveSettings();
        }
    }
}