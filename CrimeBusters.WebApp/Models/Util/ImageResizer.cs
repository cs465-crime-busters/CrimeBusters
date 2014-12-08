using System;
using System.Drawing;


namespace CrimeBusters.WebApp.Models.Util
{
    public class ImageResizer
    {
        // Got this implementation here:
        // http://www.aspdotnetmatters.com/2010/10/image-handling-in-aspnet.html
        public Image ResizeImage(Image img, int maxWidth, int maxHeight)
        {
            if (img.Height < maxHeight && img.Width < maxWidth)
            {
                return img;
            }

            Double xRatio = (double)img.Width / maxWidth;
            Double yRatio = (double)img.Height / maxHeight;
            Double ratio = Math.Max(xRatio, yRatio);
            int NewX = (int)Math.Floor(img.Width / ratio);
            int NewY = (int)Math.Floor(img.Height / ratio);

            Bitmap imgCopy = new Bitmap(NewX, NewY, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics graphicsImg = Graphics.FromImage(imgCopy);
            graphicsImg.Clear(Color.Transparent);
            graphicsImg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            //HighQualityBicubic gives best quality when resizing images  
            graphicsImg.DrawImage(img,
                new Rectangle(0, 0, NewX, NewY),
                new Rectangle(0, 0, img.Width, img.Height),
                GraphicsUnit.Pixel);
            graphicsImg.Dispose();
            img.Dispose();

            return imgCopy;
        }
    }
}