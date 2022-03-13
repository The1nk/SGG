using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
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
            Wave6,
            WinScreen
        }

        static ImageTemplates() {
            var lst = new List<ImageTemplate>();
            lst.Add(ImageTemplate.FromBitmap(TemplateType.SpeedMult1x, "1X Speed Multiplier.bmp"));
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
            lst.Add(ImageTemplate.FromBitmap(TemplateType.Wave6, "Wave 6.bmp"));
            lst.Add(ImageTemplate.FromBitmap(TemplateType.WinScreen, "Win Screen.bmp"));

            _templates = lst;
        }

        public static ImageTemplate GetByType(TemplateType type) {
            return _templates.First(t => t.Type == type);
        }
    }

    public class ImageTemplate {
        public ImageTemplates.TemplateType Type;
        private IList<Tuple<Point, Color>> _points;

        public static ImageTemplate FromBitmap(ImageTemplates.TemplateType type, string fileName) {
            var lst = new List<Tuple<Point, Color>>();
            var ret = new ImageTemplate {
                Type = type,
                _points = lst
            };

            Debug.WriteLine($"Starting {fileName}");

            using var bmp = new Bitmap($"Templates\\{fileName}");
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
            lock (image) {
                Bitmap bmp;
                if (image is Bitmap)
                    bmp = (Bitmap) image;
                else
                    bmp = new Bitmap(image);

                return _points.All(pt => bmp.GetPixel(pt.Item1.X, pt.Item1.Y) == pt.Item2);
            }
        }
    }
}
