using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Forms;

namespace SGG.Models
{
    public static class ImageTemplates
    {
        private static IList<ImageTemplate> _templates;
        
        public enum TemplateType {
            SpeedMult1x,
            SpeedMult2x,
            Ad_Attack,
            Ad_Coins,
            Ad_Gems,
            Ad_Orbs,
            Ad_Stones,
            ConfirmLayout,
            LoseScreen,
            MotdPopup,
            OfferPopup,
            Seller_Gems,
            Seller_Orbs,
            Seller_Stones,
            On_A_Map,
            Wave1,
            Wave2,
            Wave3,
            Wave4,
            Wave5,
            WinScreen,
            MapSelect,
            OfflineGold,
            SmilingSummoner
        }

        static ImageTemplates() {
            var lst = new List<ImageTemplate>();
            lst.Add(ImageTemplate.FromBitmap(TemplateType.SpeedMult1x, "1X Speed Multiplier.bmp"));
            lst.Add(ImageTemplate.FromBitmap(TemplateType.SpeedMult2x, "2X Speed Multiplier.bmp"));
            lst.Add(ImageTemplate.FromBitmap(TemplateType.Ad_Attack, "Ad_Attack.bmp"));
            lst.Add(ImageTemplate.FromBitmap(TemplateType.Ad_Coins, "Ad_Coins.bmp"));
            lst.Add(ImageTemplate.FromBitmap(TemplateType.Ad_Gems, "Ad_Gems.bmp"));
            lst.Add(ImageTemplate.FromBitmap(TemplateType.Ad_Orbs, "Ad_Orbs.bmp"));
            lst.Add(ImageTemplate.FromBitmap(TemplateType.Ad_Stones, "Ad_Stones.bmp"));
            lst.Add(ImageTemplate.FromBitmap(TemplateType.ConfirmLayout, "Confirm Layout.bmp"));
            lst.Add(ImageTemplate.FromBitmap(TemplateType.LoseScreen, "Lose Screen.bmp"));
            lst.Add(ImageTemplate.FromBitmap(TemplateType.MotdPopup, "MOTD Popup.bmp"));
            lst.Add(ImageTemplate.FromBitmap(TemplateType.OfferPopup, "Offer popup.bmp"));
            lst.Add(ImageTemplate.FromBitmap(TemplateType.Seller_Gems, "Seller_Gems.bmp"));
            lst.Add(ImageTemplate.FromBitmap(TemplateType.Seller_Orbs, "Seller_Orbs.bmp"));
            lst.Add(ImageTemplate.FromBitmap(TemplateType.Seller_Stones, "Seller_Stones.bmp"));
            lst.Add(ImageTemplate.FromBitmap(TemplateType.On_A_Map, "On A Map.bmp"));
            lst.Add(ImageTemplate.FromBitmap(TemplateType.Wave1, "Wave 1.bmp"));
            lst.Add(ImageTemplate.FromBitmap(TemplateType.Wave2, "Wave 2.bmp"));
            lst.Add(ImageTemplate.FromBitmap(TemplateType.Wave3, "Wave 3.bmp"));
            lst.Add(ImageTemplate.FromBitmap(TemplateType.Wave4, "Wave 4.bmp"));
            lst.Add(ImageTemplate.FromBitmap(TemplateType.Wave5, "Wave 5.bmp"));
            lst.Add(ImageTemplate.FromBitmap(TemplateType.WinScreen, "Win Screen.bmp"));
            lst.Add(ImageTemplate.FromBitmap(TemplateType.MapSelect, "Map Select.bmp"));
            lst.Add(ImageTemplate.FromBitmap(TemplateType.OfflineGold, "Offline Gold.bmp"));
            lst.Add(ImageTemplate.FromBitmap(TemplateType.SmilingSummoner, "Smiling Summoner.bmp"));

            _templates = lst;
        }

        public static ImageTemplate GetByType(TemplateType type) {
            return _templates.First(t => t.Type == type);
        }
    }

    public class ImageTemplate {
        public ImageTemplates.TemplateType Type;
        private IList<Tuple<Point, Color>> _points;
        private int _height;
        private int _width;

        public static ImageTemplate FromBitmap(ImageTemplates.TemplateType type, string fileName) {
            var lst = new List<Tuple<Point, Color>>();
            
            Debug.WriteLine($"Starting {fileName}");

            using var bmp = new Bitmap(Path.Combine(AppContext.BaseDirectory, $"Templates\\{fileName}"));
            
            var ret = new ImageTemplate {
                Type = type,
                _points = lst,
                _height = bmp.Height,
                _width =  bmp.Width
            };

            for (var row = 0; row < bmp.Height; row++)
            for (var col = 0; col < bmp.Width; col++) {
                var c = bmp.GetPixel(col, row);
                if (!(c.R == 0xFF && c.G == 0x00 && c.B == 0xff))
                    // *Not* magic pink!
                    lst.Add(new Tuple<Point, Color>(new Point(col, row), c));
            }
            
            Debug.WriteLine($"... Has {lst.Count:#,##0} points");

            return ret;
        }

        public bool IsPresentOn(Image image) {
            if (image.Height != this._height || image.Width != this._width)
                return false;

            lock (image) {
                if (image is Bitmap bmp) {
#if DEBUG
                    var good = 0;
                    var bad = 0;
                    foreach (var pt in _points) {
                        var px = bmp.GetPixel(pt.Item1.X, pt.Item1.Y);

                        if (pt.Item2 == px) {
                            good++;
                        }
                        else {
                            bad++;
                        }
                    }
#endif

                    var goodPixels = _points.Count(pt => bmp.GetPixel(pt.Item1.X, pt.Item1.Y) == pt.Item2);
                    var pct = Convert.ToDouble(goodPixels) / _points.Count;

#if DEBUG
                    Debug.WriteLine($"Template '{Type}' passed?\t{bad == 0}\tGood:{good}\tBad:{bad}\tPct:{pct}");
#endif
                    var ret = (pct >= 0.8);

                    return ret;
                }

                return false;
            }
        }
    }
}
