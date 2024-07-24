using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Change = SkinPackCreator.ChangeEvents;
using Click = SkinPackCreator.ClickEvents;

namespace SkinPackCreator
{
    public partial class Form1 : Form
    {
        private Click.ClickEvents ClickEvents { get; set; }
        private Change.ChangeEvents ChangeEvents { get; set; }
        private Utils.Utils Utils { get; set; }

        public Form1()
        {
            ClickEvents = new Click.ClickEvents();
            ChangeEvents = new Change.ChangeEvents();
            Utils = new Utils.Utils();

            InitializeComponent();
        }

        #region Private Events

        private void MenuNewMcpack_Click(object sender, EventArgs e)
        {
            try
            {
                ClickEvents.NewMcpackFile(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void MenuInstalledSkinPacks_Click(object sender, EventArgs e)
        {
            try
            {
                ClickEvents.OpenInstalledSkinFolder();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void MenuSave_Click(object sender, EventArgs e)
        {
            try
            {
                var result = ClickEvents.SaveMcpack(this);
                if (Global.InstallFile && result)
                    Process.Start(Global.McpackFilePath);

                Global.InstallFile = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void MenuOpenMcpack_Click(object sender, EventArgs e)
        {
            try
            {
                ClickEvents.OpenMcPackFile(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void MenuInstall_Click(object sender, EventArgs e)
        {
            try
            {
                if (Global.Skins.SkinList.Count > 0)
                {
                    Global.InstallFile = true;
                    MenuSave_Click(sender, e);
                }
                else
                    MessageBox.Show("There's nothing to install yet.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ButtonBrowseSkin_Click(object sender, EventArgs e)
        {
            try
            {
                if (Global.Skins.SkinList.Count > 0)
                    ClickEvents.OpenTextureFile(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void ButtonAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (!IsMcpackCreated(sender, e))
                    return;

                ClickEvents.AddNewSkin(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ButtonRemove_Click(object sender, EventArgs e)
        {
            try
            {
                if (Global.Skins.SkinList.Count <= 0)
                    return;

                ClickEvents.RemoveSkin(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ButtonMoveUp_Click(object sender, EventArgs e)
        {
            try
            {
                if (Global.Skins.SkinList.Count <= 0)
                    return;

                ClickEvents.MoveUp(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ButtonMoveDown_Click(object sender, EventArgs e)
        {
            try
            {
                if (Global.Skins.SkinList.Count <= 0)
                    return;

                ClickEvents.MoveDown(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void buttonImportMultiple_Click(object sender, EventArgs e)
        {
            try
            {
                if (!IsMcpackCreated(sender, e))
                    return;

                ClickEvents.OpenMultipleTextureFiles(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void ButtonDonate_Click(object sender, EventArgs e)
        {
            try
            {
                Utils.OpenDonate();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void TextBoxName_LostFocus(object sender, EventArgs e)
        {
            try
            {
                if (Global.Skins.SkinList.Count > 0)
                    ChangeEvents.SkinNameChanged(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ComboBoxFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (Global.Skins.SkinList.Count > 0)
                    ChangeEvents.SkinFormatChanged(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ListSkins_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ChangeEvents.SkinListIndexChanged(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void PictureBoxSkin_SizeChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.pictureBoxSkin.Width != this.pictureBoxSkin.Height)
                {
                    this.pictureBoxSkin.Width = this.pictureBoxSkin.Height;
                    this.pictureBoxSkin.Image = Utils.LoadImagePreview(this, this.listSkins.SelectedIndex);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void Form1_Resize(object sender, EventArgs e)
        {
            try
            {
                if (WindowState != FormWindowState.Minimized)
                    PictureBoxSkin_SizeChanged(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                this.Icon = Utils.LoadIconFromBase64(Global.FormIconBase64);
                this.buttonDonate.Image = Utils.LoadImageFromBase64(Global.ButtonDonateImageBase64);
                ResetAllControls();
                Global.LocName = "YourSkinPacksLocName";
                Set_StatusLabel("Let's get started, I'm ready!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Public Methods

        public void ClearSkinFields()
        {
            this.textBoxName.Text = "";
            this.textBoxTexture.Text = "";
            this.textBoxType.Text = "free";
            this.pictureBoxSkin.Image = null;

            Set_StatusLabel("Fields cleared!");

        }
        public void ResetAllControls()
        {
            ClearSkinFields();
            ClearSkinsList();

            this.textBoxPackDescription.Text = "";
            this.textBoxPackName.Text = "";

            Global.Skins = new Models.SkinModel()
            {
                PackLocName = Global.LocName,
                PackSerializeName = Global.LocName,
                SkinList = new List<Models.SkinModel.Skin>()
            };

            Global.Manifest = new Models.ManifestModel()
            {
                Modules = new List<Models.ModulesModel>(),
                Header = new Models.HeaderModel()
            };
        }
        public void Set_PackName(string text)
        {
            this.textBoxPackName.Text = text;
        }
        public string Get_PackName()
        {
            return this.textBoxPackName.Text;
        }
        public void Set_PackDescription(string text)
        {
            this.textBoxPackDescription.Text = text;
        }
        public string Get_PackDescription()
        {
            return this.textBoxPackDescription.Text;
        }
        public void ClearSkinsList()
        {
            this.listSkins.Items.Clear();
        }
        public void Set_SkinListAddItem(string item)
        {
            this.listSkins.Items.Add(item);
        }
        public int Get_SkinListSelectedIndex()
        {
            return this.listSkins.SelectedIndex;
        }
        public void Set_SkinListSelectedItem(int index)
        {
            this.listSkins.SelectedIndex = index;
        }
        public int Get_SkinListItemsCount()
        {
            return this.listSkins.Items.Count;
        }
        public int Get_PictureBoxSize()
        {
            return this.pictureBoxSkin.Width;
        }
        public void Set_StatusLabel(string text)
        {
            this.toolStripStatus.Text = text;
            Application.DoEvents();
        }
        public void Set_SkinName(string text)
        {
            this.textBoxName.Text = text;
        }
        public string Get_SkinName()
        {
            return this.textBoxName.Text;
        }
        public void Set_SkinFormat(string text)
        {
            this.comboBoxFormat.SelectedItem = text;
        }
        public string Get_SkinFormat()
        {
            return this.comboBoxFormat.SelectedItem.ToString();
        }
        public void Set_SkinTexture(string text)
        {
            this.textBoxTexture.Text = text;
        }
        public void Set_SkinType(string text)
        {
            this.textBoxType.Text = text;
        }
        public void Set_SkinBoxPicture(Image image)
        {
            this.pictureBoxSkin.Image = image;
        }
        public void Focus_SkinName()
        {
            this.textBoxName.Focus();
        }
        public void SelectAll_SkinName()
        {
            this.textBoxName.SelectAll();
        }

        #endregion

        #region Private Methods

        private bool IsMcpackCreated(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(Global.McpackFilePath))
            {
                MessageBox.Show("Create a new Skin Pack first!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                MenuNewMcpack_Click(sender, e);

                if (String.IsNullOrEmpty(Global.McpackFilePath))
                    return false;
            }

            return true;
        }

        #endregion
    }

    #region Global Variables

    public static class Global
    {
        public const string FormIconBase64 = "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";
        public const string ButtonDonateImageBase64 = "iVBORw0KGgoAAAANSUhEUgAAACAAAAAQCAYAAAB3AH1ZAAAE4klEQVRIia2UW0wTdhTGj8NkCdnUMWcWl7jF6Pa0ZcaocXHL5pRliYmoMducLplR0ZgMptwqVFF3CSoCRZZtXkBU5FpEWqAVpVfK/VIEL6MtIBYs4nSC5drfHkqckpk9zC/5cl7O+X9fzvmfIzIJ1lhJM8WI1xgpGCMEY6RgihGvNVbSJuc+V1TFS7ApWnDr10P/afDpYKzUH/tP4760HnO0UBUvwc9d3LZXvrbtE8bdSTB+HMZPwNAxGEz1x/ET4DuBryeJqvhnmygq0my01jXqbXUtmGvs6IzVFGgrKNBWUGaowmhrwlLThLWytrG4WBv2uNAcI4y69pP/81KiP3uBiOVTYCAS+nfDw0h2fzIFxedTUW1/i7GOA5hjhMniBYUX7rbecFBe8CuqnYuIXirELBP2fOinYpkQ/YGQuO191BmHMNvqKbxY3CfWWEnrKloBt7/i1NHDlNfeQrErAvrWQ/d6uLOOclMZsYrDZKQcBfcGuopWMPlPnMvOo6fvATEfCwlfBJGyeS6qLfNQbZk/wXmkbJ5LwpevolguXCw1kJOnRkwKGfK2RjBmDwBaiNwex65Nq8C1mLGbS8C5iO83rCJ6x17gKuP2qXhbIzApZOhJAyfTM3H33iU/8Vt+D1/M/pXCgWA/9wcL8SuF+E+FtG3zKf1lJ95ROJuVg1j2CD5nFGO1gs8+HdpmQGsQo80zGW58jdHmmdAaBG0z8NmnM1or+BxRWBT+MdxwOr9pd3U42h0u2l1dtHe66fbcZ2AEhnwwztN4NAI9ffdxdd4iI/MsYosTis9EoAn3r1y1QmhLFtpUwvVU4VqqcC1ZsCcIxihBHyPoC/Zhi/UbaLK30NBop6PrFoNeL7X1Ddhq6ii/YuRCsZYzWTlknstGU6KnwmChuqaO7ttu2q7f4FRGhleqlEL2j0sw1Ruob6ujcwD6gaEJx2PAX0Av0HytnsoGE+f3vkOV0m/AYrVhMlsBcDhd6C6VYzRZuGIwUqzVkpWdx0WNhqZmOw6nE09fH4+8Xi5fMZCkUumkUinDzec/Qq1ajbXzIXXOHlpd3Ti6e+jp9dBxu5frnW6aXD009A6jy4qi8dS7VCplWEREpy+npFTHwOAAxRrtoNlSSWV19YSBEnLzC7lsMNDYZMfhdHHH4+GP9nZOpWcSGvrdaikMl5yO9FnofngDmyWH26PQPwpen78DI8CDcbgzDrWVeZTFBdKRPovCcMkREQkN3bExL19Nu8P1YOvW7WsqDCbaHQ4MJjPa0jI0JaVUGM00NjVjv9pKfX0DuXlqEg4l6kUkUERkjlUp9ObMRhMTiFWtxDMCw0+M4M8xqNOlUBL7Er05s7H62z9nYgECQ0JCQlKOpanPnD1vOn4yfehk+mmKtaWU6fXk5qvJzVejLS2j6KKG4yfTOXIkuUZE5j5eIdUmibPFC56saTQmT6cs9nVsv62hJTuU6hPr0O+dTVPKK7jTX8YWLxxcJzsn3aEXReRNEXlbRBasXrt2w08Jh6qTVWk8ycSkY3fCwsLiJvKeQtDBdbLTqhRupgbgORPAvSyh/5xwL0vwZAZwIzUAq/Kx+LR/O8VPIFBE5ovIwkl8T0SCnlU0TUQWqsMk16KUYYtSsMQJFqVgUcqwOkxyJx75L/H/jTkismCS8wXyz8yfK/4GeNHiQ/D1VSQAAAAASUVORK5CYII=";

        public static readonly string SkinJsonFileName = "skins.json";
        public static readonly string ManifestJsonFileName = "manifest.json";
        public static readonly string LanguagesJsonFileName = "languages.json";
        public static readonly string EnUsLangFileName = "en_US.lang";
        public static readonly string TextsFolderName = "texts";
        public static readonly string PackNameLine = "pack.name";
        public static readonly string PackDescriptionLine = "pack.description";
        public static readonly string DefaultSkinFormat = "geometry.humanoid.custom";

        public static string WorkDir { get; set; }
        public static string McpackFilePath { get; set; }
        public static bool InstallFile { get; set; }
        public static Models.SkinModel Skins { get; set; }
        public static Models.ManifestModel Manifest { get; set; }

        public static string LocName { get; set; }
        public static string SkinJsonFilePath { get => $"{WorkDir}\\{SkinJsonFileName}"; }
        public static string ManifestJsonFilePath { get => $"{WorkDir}\\{ManifestJsonFileName}"; }
        public static string EnUsLangFilePath { get => $"{WorkDir}\\{TextsFolderName}\\{EnUsLangFileName}"; }
        public static string LanguagesJsonFilePath { get => $"{WorkDir}\\{TextsFolderName}\\{LanguagesJsonFileName}"; }
        public static string TextsDir { get => $"{WorkDir}\\{TextsFolderName}"; }

        public static string SkinLine { get => $"skin.{LocName}"; }
        public static string SkinPackLine { get => $"skinpack.{LocName}"; }
    }

    #endregion
}
