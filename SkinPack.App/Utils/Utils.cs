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
        private const string UUID_API_URL = "https://www.uuidgenerator.net/api/version4";
        private readonly string DonateUrl = "https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=CMXPUT4TV9QWS&currency_code=USD&source=url";

        public void SelectNextListItem(Form1 form, int index)
        {
            int NewIndex;

            if (form.Get_SkinListItemsCount() <= 0)
            {
                form.ClearSkinFields();
                MessageBox.Show(SKINLIST_EMPTY, "Alert", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (index <= Global.Skins.SkinList.Count - 1 && Global.Skins.SkinList.Count > 0)
                NewIndex = index;
            else
                NewIndex = --index;

            form.Set_SkinListSelectedItem(NewIndex);
        }
        public Image LoadImagePreview(Form1 form, int Index)
        {
            try
            {
                if (Global.Skins != null)
                {
                    if (!string.IsNullOrEmpty(Global.Skins.SkinList[Index].SkinTextureFile))
                    {
                        Image Imagem;
                        var ImagePath = $"{Global.WorkDir}\\{Global.Skins.SkinList[Index].SkinTextureFile}";

                        using (var BmpTemp = new Bitmap(ImagePath))
                        {
                            Imagem = new Bitmap(BmpTemp);
                        }

                        return ResizeImage(Imagem, form.Get_PictureBoxSize());
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
                var Proportion = size / sourceImage.Width;
                float Scale = float.Parse(Proportion.ToString());

                int Width = (int)(sourceImage.Width * Scale);
                int Height = (int)(sourceImage.Height * Scale);

                Bitmap Bitmap = new Bitmap(Width, Height);

                using (Graphics Graph = Graphics.FromImage(Bitmap))
                {
                    Graph.InterpolationMode = InterpolationMode.NearestNeighbor;

                    Point[] Destination =
                    {
                        new Point(0, 0),
                        new Point(Width, 0),
                        new Point(0, Height),
                    };

                    Rectangle Source = new Rectangle(0, 0, sourceImage.Width, sourceImage.Height);
                    Graph.DrawImage(sourceImage, Destination, Source, GraphicsUnit.Pixel);
                }

                return (Image)Bitmap;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<string> RequestUUID()
        {
            HttpResponseMessage result = await new HttpClient().GetAsync(UUID_API_URL);
            return result.Content.ReadAsStringAsync().Result;
        }
        public Icon LoadIconFromBase64(string imageBase64)
        {
            Bitmap Bitmap = (Bitmap)LoadImageFromBase64(imageBase64);

            IntPtr pIcon = Bitmap.GetHicon();
            return Icon.FromHandle(pIcon);
        }
        public Image LoadImageFromBase64(string imageBase64)
        {
            byte[] Bytes = Convert.FromBase64String(imageBase64);

            Image Image;
            using (MemoryStream Ms = new MemoryStream(Bytes)) { Image = Image.FromStream(Ms); }

            return Image;
        }
        public void OpenDonate()
        {
            Process.Start(DonateUrl);
            //if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            //{
            //    DonateUrl = DonateUrl.Replace("&", "^&");
            //    Process.Start(new ProcessStartInfo("cmd", $"/c start {DonateUrl}") { CreateNoWindow = true });
            //}

        }
    }
}
