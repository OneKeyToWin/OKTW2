using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using static LeagueSharp.Common.Render;
using LeagueSharp;
using BaseTexture = SharpDX.Direct3D9.BaseTexture;

namespace OneKeyToWin_AIO_Sebby.Core
{
    class ImageLoader
    {
        public static Sprite CreateSummonerSprite(string name)
        {
            var srcBitmap = (Bitmap)Resource1.ResourceManager.GetObject(name);
            if (srcBitmap == null)
            {
                Console.WriteLine("Can't find image: " + name);
                srcBitmap = (Bitmap)Resource1.ResourceManager.GetObject("Default");
            }
            var img = new Bitmap(srcBitmap.Width + 2, srcBitmap.Width + 2);
            var cropRect = new System.Drawing.Rectangle(0, 0, srcBitmap.Width, srcBitmap.Width);

            using (Bitmap sourceImage = srcBitmap)
            {
                using (Bitmap croppedImage = sourceImage.Clone(cropRect, sourceImage.PixelFormat))
                {
                    using (var tb = new TextureBrush(croppedImage))
                    {
                        using (Graphics g = Graphics.FromImage(img))
                        {
                            g.SmoothingMode = SmoothingMode.AntiAlias;
                            g.FillEllipse(tb, 0, 0, srcBitmap.Width, srcBitmap.Width);
                        }
                    }
                }
            }
            srcBitmap.Dispose();
            Sprite finalSprite = new Sprite(img, Vector2.Zero);
            return finalSprite;
        }

        public static Bitmap ResizeBitmap(Bitmap source, int width, int height)
        {
            var destRect = new System.Drawing.Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);
            destImage.SetResolution(source.HorizontalResolution, source.VerticalResolution);
            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(
                        source, destRect, 0, 0, source.Width, source.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }
            return destImage;
        }


        public static Sprite CreateSummonerSprite(SpellDataInst spell)
        {
            var texturePtr = spell.IconTexture;

            Bitmap srcBitmap;
            if (texturePtr == IntPtr.Zero)
            {
                Console.WriteLine("Can't find image: " + spell.Name);
                srcBitmap = (Bitmap)Resource1.ResourceManager.GetObject("Default");
            }
            else
            {
                srcBitmap = (Bitmap)Image.FromStream(BaseTexture.ToStream(new BaseTexture(texturePtr), SharpDX.Direct3D9.ImageFileFormat.Bmp));
            }

            if (srcBitmap.Width > 64 || srcBitmap.Height > 64)
                srcBitmap = ResizeBitmap(srcBitmap, 64, 64);

            var img = new Bitmap(srcBitmap.Width + 2, srcBitmap.Width + 2);
            var cropRect = new System.Drawing.Rectangle(0, 0, srcBitmap.Width, srcBitmap.Width);

            using (Bitmap sourceImage = srcBitmap)
            {
                using (Bitmap croppedImage = sourceImage.Clone(cropRect, sourceImage.PixelFormat))
                {
                    using (var tb = new TextureBrush(croppedImage))
                    {
                        using (Graphics g = Graphics.FromImage(img))
                        {
                            g.SmoothingMode = SmoothingMode.AntiAlias;
                            g.FillEllipse(tb, 0, 0, srcBitmap.Width, srcBitmap.Width);
                        }
                    }
                }
            }
            srcBitmap.Dispose();
            Sprite finalSprite = new Sprite(img, Vector2.Zero);
            return finalSprite;
        }

        public static Sprite CreateMinimapSprite(Obj_AI_Hero hero)
        {
            var texturePtr = hero.SquareIconPortrait;

            Bitmap srcBitmap;
            if (texturePtr == IntPtr.Zero)
            {
                Console.WriteLine("Can't find image: " + hero.ChampionName);
                srcBitmap = (Bitmap)Resource1.ResourceManager.GetObject("Default");
            }
            else
            {
                srcBitmap = (Bitmap)Image.FromStream(BaseTexture.ToStream(new BaseTexture(texturePtr), SharpDX.Direct3D9.ImageFileFormat.Bmp));
            }

            var img = new Bitmap(srcBitmap.Width + 2, srcBitmap.Width + 2);
            var cropRect = new System.Drawing.Rectangle(0, 0, srcBitmap.Width, srcBitmap.Width);

            using (Bitmap sourceImage = srcBitmap)
            {
                using (Bitmap croppedImage = sourceImage.Clone(cropRect, sourceImage.PixelFormat))
                {
                    using (var tb = new TextureBrush(croppedImage))
                    {
                        using (Graphics g = Graphics.FromImage(img))
                        {
                            g.SmoothingMode = SmoothingMode.AntiAlias;
                            g.FillEllipse(tb, 0, 0, srcBitmap.Width, srcBitmap.Width);

                            var p = new Pen(System.Drawing.Color.White, 1) { Alignment = PenAlignment.Center };
                            g.DrawEllipse(p, 0, 0, srcBitmap.Width, srcBitmap.Width);
                        }
                    }
                }
            }
            srcBitmap.Dispose();
            Sprite finalSprite = new Sprite(img, Vector2.Zero);
            finalSprite.Scale = new Vector2(0.2f, 0.2f);

            return finalSprite;
        }

        public static Sprite GetSprite(string name)
        {
            var res = (Bitmap)Resource1.ResourceManager.GetObject(name);

            if (res == null)
            {
                Console.WriteLine("Can't find image: " + name);
                return new Sprite((Bitmap)Resource1.ResourceManager.GetObject("Default"), Vector2.Zero);
            }

            return new Sprite(res, Vector2.Zero);
        }

        public static Sprite CreateRadrarIcon(Obj_AI_Hero hero, System.Drawing.Color color, int opacity = 60)
        {
            var texturePtr = hero.SquareIconPortrait;

            Bitmap srcBitmap;
            if (texturePtr == IntPtr.Zero)
            {
                Console.WriteLine("Can't find image: " + hero.ChampionName);
                srcBitmap = (Bitmap)Resource1.ResourceManager.GetObject("Default");
            }
            else
            {
                srcBitmap = (Bitmap)Image.FromStream(BaseTexture.ToStream(new BaseTexture(texturePtr), SharpDX.Direct3D9.ImageFileFormat.Bmp));
            }

            var img = new Bitmap(srcBitmap.Width + 20, srcBitmap.Width + 20);
            var cropRect = new System.Drawing.Rectangle(0, 0, srcBitmap.Width, srcBitmap.Width);

            using (Bitmap sourceImage = srcBitmap)
            {
                using (Bitmap croppedImage = sourceImage.Clone(cropRect, sourceImage.PixelFormat))
                {
                    using (var tb = new TextureBrush(croppedImage))
                    {
                        using (Graphics g = Graphics.FromImage(img))
                        {
                            g.SmoothingMode = SmoothingMode.AntiAlias;
                            g.FillEllipse(tb, 0, 0, srcBitmap.Width, srcBitmap.Width);

                            var p = new Pen(color, 5) { Alignment = PenAlignment.Inset };
                            g.DrawEllipse(p, 0, 0, srcBitmap.Width, srcBitmap.Width);
                        }
                    }
                }
            }
            srcBitmap.Dispose();
            Sprite finalSprite = new Sprite(ChangeOpacity(img, opacity),Vector2.Zero);
            //finalSprite.X = -25;
            finalSprite.Scale = new Vector2(1f, 1f);
            //finalSprite.Color = System.Drawing.Color.LightGray;
            return finalSprite;
        }

        public static Bitmap ChangeOpacity(Bitmap img, int opacity)
        {
            float iconOpacity = opacity / 100.0f;
            var bmp = new Bitmap(img.Width, img.Height); // Determining Width and Height of Source Image
            Graphics graphics = Graphics.FromImage(bmp);
            var colormatrix = new ColorMatrix { Matrix33 = iconOpacity };
            var imgAttribute = new ImageAttributes();
            imgAttribute.SetColorMatrix(colormatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            graphics.DrawImage(
                img, new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel,
                imgAttribute);
            graphics.Dispose(); // Releasing all resource used by graphics
            img.Dispose();
            return bmp;
        }
    }
}