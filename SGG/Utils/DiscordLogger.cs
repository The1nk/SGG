using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Discord.Webhook;

namespace SGG.Utils
{
    public static class DiscordLogger {
        private static string _hookUrl;

        public enum MessageType {
            Info, Warn, Error,
            Debug
        }

        public static void SetUrl(string url) => _hookUrl = url;

        public static async void Log(MessageType type, string message) {
            if (string.IsNullOrEmpty(_hookUrl))
                return;

            try {
                using var c = new DiscordWebhookClient(_hookUrl);
                var msg =
                    $"`{DateTime.Now.ToString("s", System.Globalization.CultureInfo.InvariantCulture)}\t{type}\t{message}`";

                if ((type == MessageType.Debug && Debugger.IsAttached) ||
                    type != MessageType.Debug)
                    await c.SendMessageAsync(msg);
            }
            catch {
                ;
            }
        }
    }
}
