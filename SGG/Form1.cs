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

                MessageBox.Show(
                    ex.Message.StartsWith("No connection could be made because the target machine actively refused it.")
                        ? $"Unable to connect to ADB server. Please ensure it's running.\r\n\r\n{ex.Message}"
                        : ex.Message);
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

                if (CheckForAd(ImageTemplates.TemplateType.Ad_Attack, bmp, newGuid, cbMonitorAttack.Enabled)) return;
                if (CheckForAd(ImageTemplates.TemplateType.Ad_Coins, bmp, newGuid, cbMonitorCoins.Enabled)) return;
                if (CheckForAd(ImageTemplates.TemplateType.Ad_Gems, bmp, newGuid, cbMonitorGems.Enabled)) return;
                if (CheckForAd(ImageTemplates.TemplateType.Ad_Orbs, bmp, newGuid, cbMonitorOrbs.Enabled)) return;
                if (CheckForAd(ImageTemplates.TemplateType.Ad_Stones, bmp, newGuid, cbMonitorOther.Enabled)) return;

                if (CheckForSale(ImageTemplates.TemplateType.Seller_Gems, bmp, newGuid, cbSellerGems.Enabled)) return;
                if (CheckForSale(ImageTemplates.TemplateType.Seller_Orbs, bmp, newGuid, cbSellerOrbs.Enabled)) return;
                if (CheckForSale(ImageTemplates.TemplateType.Seller_Stones, bmp, newGuid, cbSellerOther.Enabled)) return;
            }
            catch (Exception ex) {
                Debug.WriteLine($"{ex.Message}\r\n{ex.StackTrace}");
            }
            finally {
                _running = false;
            }
        }

        private bool CheckForAd(ImageTemplates.TemplateType adType, Image clone, Guid guid, bool enabled) {
            if (!guid.Equals(_currentGuid)) return true;
            var ret = ImageTemplates.GetByType(adType).IsPresentOn(clone);

            if (ret) {
                if (enabled) {
                    // Watch ad .. disabled for now
                }
                else {
                    ClickAt(234, 888);
                    _nextActionAvailableAt = DateTime.Now.AddSeconds(.5);
                }
            }

            return ret;
        }

        private bool CheckForSale(ImageTemplates.TemplateType saleType,  Image clone, Guid guid, bool enabled) {
            if (!guid.Equals(_currentGuid)) return true;
            var ret = ImageTemplates.GetByType(saleType).IsPresentOn( clone);

            if (ret) {
                if (enabled) {
                    ClickAt(494, 915);
                    Thread.Sleep(200);
                    ClickAt(494, 915);
                    _nextActionAvailableAt = DateTime.Now.AddSeconds(.5);
                }
                else {
                    ClickAt(243, 847);
                    _nextActionAvailableAt = DateTime.Now.AddSeconds(.5);
                }
            }

            return ret;
        }

        private void CheckForConfirm(Image image, Guid guid) {
            if (!guid.Equals(_currentGuid)) return;
            if (!ImageTemplates.GetByType(ImageTemplates.TemplateType.ConfirmLayout).IsPresentOn(image)) return;

            // Dismiss
            ClickAt(433, 1505);
            SelectNextMap();
        }

        private void CheckForLose(Image image, Guid guid) {
            if (!guid.Equals(_currentGuid)) return;
            if (!ImageTemplates.GetByType(ImageTemplates.TemplateType.LoseScreen).IsPresentOn(image)) return;

            // Dismiss
            ClickAt(835, 277);
            SelectNextMap();
        }

        private void CheckForWin(Image image, Guid guid) {
            if (!guid.Equals(_currentGuid)) return;
            if (!ImageTemplates.GetByType(ImageTemplates.TemplateType.WinScreen).IsPresentOn(image)) return;

            // Dismiss
            ClickAt(444, 1222);
            SelectNextMap();
        }

        private void CheckForWave6Reset(Image image, Guid guid) {
            if (!guid.Equals(_currentGuid)) return;
            if (!ImageTemplates.GetByType(ImageTemplates.TemplateType.Wave6).IsPresentOn(image)) return;

            // Pop map select
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
            ClickAt(835, 277);
            _nextActionAvailableAt = DateTime.Now.AddSeconds(2);
        }

        private void CheckForSpeedMultiplier(Image image, Guid guid) {
            if (!guid.Equals(_currentGuid)) return;
            if (!ImageTemplates.GetByType(ImageTemplates.TemplateType.SpeedMult1x).IsPresentOn(image)) return;

            // Toggle it
            ClickAt(69, 1394);
            _nextActionAvailableAt = DateTime.Now.AddSeconds(2);
        }

        private void CheckForMotd(Image image, Guid guid) {
            if (!guid.Equals(_currentGuid)) return;
            if (!ImageTemplates.GetByType(ImageTemplates.TemplateType.MotdPopup).IsPresentOn(image)) return;

            // Dismiss it!
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