using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using SGG.Models;
using SharpAdbClient;
using SixteenTons.Utils;
using SixteenTons.Workers;

namespace ScratchPad {
    public partial class Form1 : Form {
        private bool _initialized;
        private AdbClient _client;
        private DeviceData _device;
        private readonly Queue<Image> _lastCaptures;
        private ScreenCaptureWorker _screenCaptureWorker;

        private ContextMenuStrip _contextMenu;
        private string _setting = string.Empty;

        private Point _selectedMapPoint = new Point(448, 1283);

        public Form1() {
            InitializeComponent();
            _lastCaptures = new Queue<Image>();
        }

        private void ScreenCaptureWorker_NewImageCaptured(Image obj) {
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
                _screenCaptureWorker.NewImageCaptured += ScreenCaptureWorker_NewImageCaptured;

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