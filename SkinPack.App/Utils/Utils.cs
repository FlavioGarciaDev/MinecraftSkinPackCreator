using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SkinPackCreator.Utils
{
    public class Utils
    {
        private const string SKINLIST_EMPTY = "Your list is now empty. :(";
        private const string IMAGE_NOT_FOUND = "No image found on the selected item.";
        private const string DONATE_URL = "https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=CMXPUT4TV9QWS&item_name=Minecraft+Skin+Pack+Creator&currency_code=USD";

        public void SelectNextListItem(Form1 form, int index)
        {
            int newIndex;

            if (form.Get_SkinListItemsCount() <= 0)
            {
                form.ClearSkinFields();
                MessageBox.Show(SKINLIST_EMPTY, "Alert", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (index <= Global.Skins.SkinList.Count - 1 && Global.Skins.SkinList.Count > 0)
                newIndex = index;
            else
                newIndex = --index;

            form.Set_SkinListSelectedItem(newIndex);
        }
        public Image LoadImagePreview(Form1 form, int index)
        {
            try
            {
                if (Global.Skins != null)
                {
                    if (!string.IsNullOrEmpty(Global.Skins.SkinList[index].SkinTextureFile))
                    {
                        Image imagePreview;
                        var imagePath = $"{Global.WorkDir}\\{Global.Skins.SkinList[index].SkinTextureFile}";

                        using (var bmpTemp = new Bitmap(imagePath))
                        {
                            imagePreview = new Bitmap(bmpTemp);
                        }

                        return ResizeImage(imagePreview, form.Get_PictureBoxSize());
                    }
                }
                else
                    return null;
            }
            catch (Exception)
            {
                form.Set_StatusLabel(IMAGE_NOT_FOUND);
            }

            return null;
        }
        public Image ResizeImage(Image sourceImage, int size)
        {
            try
            {
                var proportion = size / sourceImage.Width;
                float scale = float.Parse(proportion.ToString());

                var width = (int)(sourceImage.Width * scale);
                var height = (int)(sourceImage.Height * scale);

                Bitmap bitmapImage = new Bitmap(width, height);

                using (Graphics graph = Graphics.FromImage(bitmapImage))
                {
                    graph.InterpolationMode = InterpolationMode.NearestNeighbor;

                    Point[] destination =
                    {
                        new Point(0, 0),
                        new Point(width, 0),
                        new Point(0, height),
                    };

                    Rectangle source = new Rectangle(0, 0, sourceImage.Width, sourceImage.Height);
                    graph.DrawImage(sourceImage, destination, source, GraphicsUnit.Pixel);
                }

                return (Image)bitmapImage;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public string RequestUUID()
        {
            return System.Guid.NewGuid().ToString();
        }
        public Icon LoadIconFromBase64(string imageBase64)
        {
            Bitmap bitmapImage = (Bitmap)LoadImageFromBase64(imageBase64);

            IntPtr pIcon = bitmapImage.GetHicon();
            return Icon.FromHandle(pIcon);
        }
        public Image LoadImageFromBase64(string imageBase64)
        {
            byte[] bytes = Convert.FromBase64String(imageBase64);

            Image image;
            using (MemoryStream ms = new MemoryStream(bytes)) { image = Image.FromStream(ms); }

            return image;
        }
        public void OpenDonate()
        {
            Process.Start(DONATE_URL);
        }
    }
}
