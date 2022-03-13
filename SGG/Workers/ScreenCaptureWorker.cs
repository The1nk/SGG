using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using SharpAdbClient;

namespace SixteenTons.Workers {
    internal class ScreenCaptureWorker {
        private readonly AdbClient _client;
        private readonly DeviceData _device;
        private bool _working;

        public ScreenCaptureWorker(AdbClient client, DeviceData device) {
            this._client = client;
            this._device = device;

            var t = Task.Run(DoWork);
        }

        public Image LastImage { get; private set; }

        public event Action<Image> NewImageCaptured;


        public void Start() {
            _working = true;
        }

        public void Stop() {
            _working = false;
        }

        private async void DoWork() {
            while (true) {
                if (!_working) {
                    Thread.Sleep(100);
                    continue;
                }

                LastImage = await _client.GetFrameBufferAsync(_device, CancellationToken.None);
                NewImageCaptured?.Invoke(LastImage);
            }
        }
    }
}