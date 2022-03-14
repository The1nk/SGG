using System;
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
        private readonly object _lockObject = new object();
        private bool _running = false;

        private ContextMenuStrip _contextMenu;
        private string _setting = string.Empty;

        private Point _selectedMapPoint = new Point(179, 612);
        private Guid _currentGuid;

        public Form1() {
            InitializeComponent();
            _lastCaptures = new Queue<Image>();
            
            Settings.Load();
            
            tbAdbHost.Text = Settings.AdbHost;
            tbAdbPort.Text = Settings.AdbPort;
            cbCollectAchievements.Checked = Settings.CollectAchievements;
            cbCollectWeeklies.Checked = Settings.CollectWeeklies;
            cbWave6.Checked = Settings.StopAtWave6;
            _selectedMapPoint = Settings.MapPoint;
            tbAppRestartInterval.Text = Settings.AppRestartInterval;
            tbDeviceRestartInterval.Text = Settings.DeviceRestartInterval;
            cbSellerGems.Checked = Settings.SellerGems;
            cbSellerOrbs.Checked = Settings.SellerOrbs;
            cbSellerOther.Checked = Settings.SellerOther;
            cbMonitorAttack.Checked = Settings.MonitorAttack;
            cbMonitorCoins.Checked = Settings.MonitorCoins;
            cbMonitorGems.Checked = Settings.MonitorGems;
            cbMonitorOrbs.Checked = Settings.MonitorOrbs;
            cbMonitorOther.Checked = Settings.MonitorOther;
            tbDiscordHookUrl.Text = Settings.WebhookUrl;
        }

        private void UpdateImage(Image obj) {
            Image clone;
            lock (obj) {
                clone = (Image) obj.Clone();
            }

            pictureBox1.Image = clone;
            _lastCaptures.Enqueue(clone);
            while (_lastCaptures.Count > 100)
                _lastCaptures.Dequeue();
        }

        private void Init() {
            DiscordLogger.SetUrl(tbDiscordHookUrl.Text);

            Settings.AdbHost =  tbAdbHost.Text;
            Settings.AdbPort =  tbAdbPort.Text;
            Settings.CollectAchievements =  cbCollectAchievements.Checked;
            Settings.CollectWeeklies =  cbCollectWeeklies.Checked;
            Settings.StopAtWave6 =  cbWave6.Checked;
            Settings.MapPoint =  _selectedMapPoint;
            Settings.AppRestartInterval =  tbAppRestartInterval.Text;
            Settings.DeviceRestartInterval =  tbDeviceRestartInterval.Text;
            Settings.SellerGems =  cbSellerGems.Checked;
            Settings.SellerOrbs =  cbSellerOrbs.Checked;
            Settings.SellerOther =  cbSellerOther.Checked;
            Settings.MonitorAttack =  cbMonitorAttack.Checked;
            Settings.MonitorCoins =  cbMonitorCoins.Checked;
            Settings.MonitorGems =  cbMonitorGems.Checked;
            Settings.MonitorOrbs =  cbMonitorOrbs.Checked;
            Settings.MonitorOther =  cbMonitorOther.Checked;
            Settings.WebhookUrl =  tbDiscordHookUrl.Text;
            Settings.Save();

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
                DiscordLogger.Log(DiscordLogger.MessageType.Error, msg).Wait(500);
                MessageBox.Show(msg);
            }
        }

        private void RunBot(Image obj) {
            if (_running)
                return;

            lock (_lockObject) {
                if (_running)
                    return;

                _running = true;
            }

            try {
                if (DateTime.Now <= _nextActionAvailableAt) return;

                Bitmap bmp;
                lock (obj) {
                    bmp = new Bitmap((Image) obj.Clone());

                }
                var newGuid = Guid.NewGuid();
                _currentGuid = new Guid(newGuid.ToByteArray());
                
                if (cbWave6.Enabled)
                    CheckForWave6Reset(bmp, newGuid);

                CheckForConfirm(bmp, newGuid);
                CheckForMotd(bmp, newGuid);
                CheckForOffer(bmp, newGuid);
                CheckForSpeedMultiplier(bmp, newGuid);
                CheckForWin(bmp, newGuid);
                CheckForLose(bmp, newGuid);

                if (CheckForAd(ImageTemplates.TemplateType.Ad_Attack, bmp, newGuid, cbMonitorAttack)) return;
                if (CheckForAd(ImageTemplates.TemplateType.Ad_Coins, bmp, newGuid, cbMonitorCoins)) return;
                if (CheckForAd(ImageTemplates.TemplateType.Ad_Gems, bmp, newGuid, cbMonitorGems)) return;
                if (CheckForAd(ImageTemplates.TemplateType.Ad_Orbs, bmp, newGuid, cbMonitorOrbs)) return;
                if (CheckForAd(ImageTemplates.TemplateType.Ad_Stones, bmp, newGuid, cbMonitorOther)) return;

                if (CheckForSale(ImageTemplates.TemplateType.Seller_Gems, bmp, newGuid, cbSellerGems)) return;
                if (CheckForSale(ImageTemplates.TemplateType.Seller_Orbs, bmp, newGuid, cbSellerOrbs)) return;
                if (CheckForSale(ImageTemplates.TemplateType.Seller_Stones, bmp, newGuid, cbSellerOther)) return;
            }
            catch (Exception ex) {
                DiscordLogger.Log(DiscordLogger.MessageType.Error, $"{ex.Message}\r\n{ex.StackTrace}").Wait(500);
                Debug.WriteLine($"{ex.Message}\r\n{ex.StackTrace}");
            }
            finally {
                _running = false;
            }
        }

        private bool CheckForAd(ImageTemplates.TemplateType adType, Image clone, Guid guid, CheckBox checkbox) {
            if (!guid.Equals(_currentGuid)) return true;
            var ret = ImageTemplates.GetByType(adType).IsPresentOn(clone);
            var enabled = checkbox.Enabled && checkbox.Checked;

            if (ret) {
                if (enabled) {
                    DiscordLogger.Log(DiscordLogger.MessageType.Info, $"Watching Monitor for {adType}");
                    // Watch ad .. disabled for now
                }
                else {
                    DiscordLogger.Log(DiscordLogger.MessageType.Info, $"Skipping Monitor for {adType}");
                    ClickAt(234, 888);
                    _nextActionAvailableAt = DateTime.Now.AddSeconds(1);
                }
            }

            return ret;
        }

        private bool CheckForSale(ImageTemplates.TemplateType saleType,  Image clone, Guid guid, CheckBox checkbox) {
            if (!guid.Equals(_currentGuid)) return true;
            var ret = ImageTemplates.GetByType(saleType).IsPresentOn( clone);
            var enabled = checkbox.Enabled && checkbox.Checked;

            if (ret) {
                if (enabled) {
                    DiscordLogger.Log(DiscordLogger.MessageType.Info, $"Accepting Seller for {saleType}");
                    ClickAt(494, 915);
                    Thread.Sleep(200);
                    ClickAt(494, 915);
                    _nextActionAvailableAt = DateTime.Now.AddSeconds(1);
                }
                else {
                    DiscordLogger.Log(DiscordLogger.MessageType.Info, $"Skipping Seller for {saleType}");
                    ClickAt(243, 847);
                    _nextActionAvailableAt = DateTime.Now.AddSeconds(1);
                }
            }

            return ret;
        }

        private void CheckForConfirm(Image image, Guid guid) {
            if (!guid.Equals(_currentGuid)) return;
            if (!ImageTemplates.GetByType(ImageTemplates.TemplateType.ConfirmLayout).IsPresentOn(image)) return;

            // Dismiss
            DiscordLogger.Log(DiscordLogger.MessageType.Info, "Confirming layout");
            ClickAt(433, 1505);
            SelectNextMap();
        }

        private void CheckForLose(Image image, Guid guid) {
            if (!guid.Equals(_currentGuid)) return;
            if (!ImageTemplates.GetByType(ImageTemplates.TemplateType.LoseScreen).IsPresentOn(image)) return;

            // Dismiss
            DiscordLogger.Log(DiscordLogger.MessageType.Info, "Dismissing 'Lose' screen");
            ClickAt(835, 277);
            SelectNextMap();
        }

        private void CheckForWin(Image image, Guid guid) {
            if (!guid.Equals(_currentGuid)) return;
            if (!ImageTemplates.GetByType(ImageTemplates.TemplateType.WinScreen).IsPresentOn(image)) return;

            // Dismiss
            DiscordLogger.Log(DiscordLogger.MessageType.Info, "Dismissing 'Win' screen");
            ClickAt(444, 1222);
            SelectNextMap();
        }

        private void CheckForWave6Reset(Image image, Guid guid) {
            if (!guid.Equals(_currentGuid)) return;
            if (!ImageTemplates.GetByType(ImageTemplates.TemplateType.Wave6).IsPresentOn(image)) return;

            // Pop map select
            DiscordLogger.Log(DiscordLogger.MessageType.Info, "Resetting after Wave 6");
            ClickAt(790, 47);
            SelectNextMap();
        }

        private void SelectNextMap() {
            ClickAt(_selectedMapPoint.X, _selectedMapPoint.Y);
            _nextActionAvailableAt = DateTime.Now.AddSeconds(2);
        }

        private void CheckForOffer(Image image, Guid guid) {
            if (!guid.Equals(_currentGuid)) return;
            if (!ImageTemplates.GetByType(ImageTemplates.TemplateType.OfferPopup).IsPresentOn(image)) return;

            // Dismiss
            DiscordLogger.Log(DiscordLogger.MessageType.Info, "Dismissing sale popup");
            ClickAt(835, 277);
            _nextActionAvailableAt = DateTime.Now.AddSeconds(2);
        }

        private void CheckForSpeedMultiplier(Image image, Guid guid) {
            if (!guid.Equals(_currentGuid)) return;
            if (!ImageTemplates.GetByType(ImageTemplates.TemplateType.SpeedMult1x).IsPresentOn(image)) return;

            // Toggle it
            DiscordLogger.Log(DiscordLogger.MessageType.Info, "Enabling 2x speed");
            ClickAt(69, 1394);
            _nextActionAvailableAt = DateTime.Now.AddSeconds(2);
        }

        private void CheckForMotd(Image image, Guid guid) {
            if (!guid.Equals(_currentGuid)) return;
            if (!ImageTemplates.GetByType(ImageTemplates.TemplateType.MotdPopup).IsPresentOn(image)) return;

            // Dismiss it!
            DiscordLogger.Log(DiscordLogger.MessageType.Info, "Dismissing MOTD");
            ClickAt(844, 307);
            _nextActionAvailableAt = DateTime.Now.AddSeconds(2);
        }

        private void ClickAt(int x, int y) {
            _client.ExecuteShellCommand(_device, $"input tap {x} {y}", null);
        }

        private void btnStart_Click(object sender, EventArgs e) {
            Init();
            if (btnStart.Text == "Start") {
                _screenCaptureWorker.Start();
                btnStart.Text = "Stop";
            }
            else if (btnStart.Text == "Stop") {
                _screenCaptureWorker.Stop();
                btnStart.Text = "Start";
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e) {
            var mee = e as MouseEventArgs;
            if (mee.Button == MouseButtons.Left) {
                var dbg = $"{mee.X}.{mee.Y}";
                dbg += "\r\n";
                var pt = Utils.TranslateZoomMousePosition(pictureBox1.Image, pictureBox1, mee.Location);
                dbg += $"{pt.X}.{pt.Y}";

                if (_setting == string.Empty) {
                    MessageBox.Show(dbg);
                } else if (_setting == "Map") {
                    _selectedMapPoint = new Point(pt.X, pt.Y);
                    pictureBox1.Cursor = Cursors.Default;
                }
            } else {
                var counter = 1;
                while (_lastCaptures.TryDequeue(out var img)) {
                    var path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                        "SSG Screens");
                    if (!System.IO.Directory.Exists(path))
                        System.IO.Directory.CreateDirectory(path);

                    img.Save(System.IO.Path.Combine(path, $"img{counter:0000}.bmp"));
                    counter++;
                }
            }
        }

        private void btnSet_Click(object sender, EventArgs e) {
            if (_contextMenu == null) {
                var cmenu = new ContextMenuStrip();
                cmenu.Items.Add("Map").Click += (o, args) => {
                    _setting = "Map";
                    pictureBox1.Cursor = Cursors.Cross;
                };

                _contextMenu = cmenu;
            }
            
            _contextMenu.Show(btnSet, 0, 0);
        }
    }
}