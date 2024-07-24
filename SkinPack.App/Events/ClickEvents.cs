using System.IO;
using System.Windows.Forms;
using FileProcessing = SkinPackCreator.FileProcessing;
using FolderProcessing = SkinPackCreator.FolderProcessing;
using System.Threading.Tasks;
using System.Data;
using SkinPackCreator.Models;
using System.Linq;

namespace SkinPackCreator.ClickEvents
{
    public class ClickEvents
    {
        private const string SAVEDIALOG_TITLE = "Choose the folder and the McPack name";
        private const string MCPACK_FILTER = "McPack Files (*.mcpack)|*.mcpack";
        private const string MCPACK_OPEN_TITLE = "Select a McPack file";
        private const string TEXTURE_OPEN_TITLE = "Select a texture file";
        private const string MULTIPLE_TEXTURE_OPEN_TITLE = "Select multiple texture files";
        private const string IMAGE_FILTER = "Images (*.png)|*.png";
        private const string IMAGE_EXTENSION = ".png";
        private const string LOAD_OK = "Everything loaded! :D";
        private const string MCPACK_CREATED = "New McPack created!";
        private const string NEW_SKIN = "A new item has been added!";
        private const string SKIN_DEFAULT_NAME = "New Skin";

        private FileProcessing.FileProcessing FileProcessing { get; set; }
        private FolderProcessing.FolderProcessing FolderProcessing { get; set; }
        private Utils.Utils Utils { get; set; }
        public ClickEvents()
        {
            FileProcessing = new FileProcessing.FileProcessing();
            FolderProcessing = new FolderProcessing.FolderProcessing();
            Utils = new Utils.Utils();
        }

        #region Public Methods

        public void NewMcpackFile(Form1 form)
        {
            SaveFileDialog Sfd = new SaveFileDialog()
            {
                Title = SAVEDIALOG_TITLE,
                Filter = MCPACK_FILTER
            };

            var result = Sfd.ShowDialog();

            if (result == DialogResult.OK)
            {
                Global.McpackFilePath = Sfd.FileName;
                Global.WorkDir = $"{Path.GetDirectoryName(Sfd.FileName)}\\{Path.GetFileNameWithoutExtension(Sfd.FileName)}";

                form.ResetAllControls();

                form.Set_PackName(Path.GetFileNameWithoutExtension(Sfd.FileName));

                FolderProcessing.CreateWorkDir();

                form.Set_StatusLabel(MCPACK_CREATED);
            }
        }
        public bool SaveMcpack(Form1 form)
        {
            if (Global.Skins.SkinList.Count > 0)
            {
                if (!ValidSkins())
                {
                    MessageBox.Show("You have empty skins in your list! Remove or select name/texture file for them.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }

                FileProcessing.CreateSkinsJsonFile(form);
                FileProcessing.CreateEnUsLangFile(form);
                FileProcessing.CreateLanguagesJsonFile(form);
                FileProcessing.CreateManifestJsonFile(form);
                FileProcessing.CreateMcpackFile(form);

                return true;
            }
            else
            {
                MessageBox.Show("There's nothing to save!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            return false;
        }
        public void OpenMcPackFile(Form1 form)
        {
            OpenFileDialog fdb = new OpenFileDialog
            {
                Title = MCPACK_OPEN_TITLE,
                Filter = MCPACK_FILTER
            };

            var result = fdb.ShowDialog();

            if (result == DialogResult.OK)
            {
                Global.McpackFilePath = fdb.FileName;

                FileProcessing.ExtractMcpackFile(form);
                FileProcessing.LoadSkinsJson(form);

                if (Global.Skins.SkinList.Count <= 0)
                    throw new DataException($"No skins found in {Path.GetFileName(Global.McpackFilePath)}.");

                FileProcessing.FillSkinsList(form);
                FileProcessing.LoadManifestJson(form);
                FileProcessing.LoadPackSettings(form);

                form.Set_StatusLabel(LOAD_OK);
            }
        }
        public void OpenTextureFile(Form1 form)
        {
            OpenFileDialog Ofd = new OpenFileDialog
            {
                Title = TEXTURE_OPEN_TITLE,
                Filter = IMAGE_FILTER
            };

            var result = Ofd.ShowDialog();

            if (result == DialogResult.OK)
            {
                var index = form.Get_SkinListSelectedIndex();
                var file = Path.GetFileName(Ofd.FileName);
                var filename = Path.GetFileNameWithoutExtension(Ofd.FileName);

                Global.Skins.SkinList[index].SkinTextureFile = file;

                UpdateDefaultSkinName(form, index, filename);

                form.Set_SkinTexture(file);

                File.Copy(Ofd.FileName, $"{Global.WorkDir}\\{file}", true);

                form.Set_SkinBoxPicture(Utils.LoadImagePreview(form, index));

                form.Set_StatusLabel($"{file} loaded with high quality. Nice!");
            }
        }
        public void OpenMultipleTextureFiles(Form1 form)
        {
            OpenFileDialog Ofd = new OpenFileDialog
            {
                Title = MULTIPLE_TEXTURE_OPEN_TITLE,
                Filter = IMAGE_FILTER,
                Multiselect = true
            };

            var result = Ofd.ShowDialog();

            if (result == DialogResult.OK)
            {
                var imagens = Ofd.FileNames.Where(x => Path.GetExtension(x) == IMAGE_EXTENSION);

                foreach (var item in imagens)
                {
                    var filename = Path.GetFileNameWithoutExtension(item);
                    var file = Path.GetFileName(item);

                    Global.Skins.SkinList.Add(
                       new SkinModel.Skin()
                       {
                           SkinFormat = Global.DefaultSkinFormat,
                           SkinName = filename,
                           SkinTextureFile = file,
                           SkinType = "free"
                       }
                   );

                    form.Set_SkinTexture(item);

                    File.Copy(item, $"{Global.WorkDir}\\{file}", true);

                    FileProcessing.FillSkinsList(form);
                }

                var index = form.Get_SkinListItemsCount() - 1;
                Utils.SelectNextListItem(form, index);
                form.Set_SkinBoxPicture(Utils.LoadImagePreview(form, index));
            }
        }
        public void AddNewSkin(Form1 form)
        {
            Global.Skins.SkinList.Add(
               new SkinModel.Skin()
               {
                   SkinFormat = Global.DefaultSkinFormat,
                   SkinName = $"New Skin ({Global.Skins.SkinList.Count + 1})",
                   SkinTextureFile = "",
                   SkinType = "free"
               }
           );

            FileProcessing.FillSkinsList(form);
            Utils.SelectNextListItem(form, Global.Skins.SkinList.Count - 1);

            form.Focus_SkinName();
            form.SelectAll_SkinName();

            form.Set_StatusLabel(NEW_SKIN);
        }
        public void RemoveSkin(Form1 form)
        {
            var index = form.Get_SkinListSelectedIndex();
            var file = $"{Global.WorkDir}\\{Global.Skins.SkinList[index].SkinTextureFile}";

            if (File.Exists(file))
                File.Delete(file);

            Global.Skins.SkinList.RemoveAt(index);

            FileProcessing.FillSkinsList(form);
            Utils.SelectNextListItem(form, index);
        }
        public void MoveUp(Form1 form)
        {
            var index = form.Get_SkinListSelectedIndex();

            if (index < 1 && Global.Skins.SkinList.Count >= 2)
            {
                MessageBox.Show("The selected skin is not able to move further up!", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (Global.Skins.SkinList.Count <= 1)
            {
                MessageBox.Show("You need at least 2 skins to be able to move the currently selected skin up!", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                var temp = Global.Skins.SkinList[index];

                Global.Skins.SkinList[index] = Global.Skins.SkinList[index - 1];
                Global.Skins.SkinList[index - 1] = temp;

                FileProcessing.FillSkinsList(form);
                Utils.SelectNextListItem(form, index - 1);

                form.Focus_SkinName();
                form.SelectAll_SkinName();

                form.Set_StatusLabel("Skin moved up!");
            }
        }
        public void MoveDown (Form1 form)
        {
            var index = form.Get_SkinListSelectedIndex();

            if (Global.Skins.SkinList.Count <= 1)
            {
                MessageBox.Show("You need at least 2 skins to be able to move the currently selected skin down!", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (index + 1 == Global.Skins.SkinList.Count)
            {
                MessageBox.Show("The selected skin is not able to move further down!", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                var temp = Global.Skins.SkinList[index];

                Global.Skins.SkinList[index] = Global.Skins.SkinList[index + 1];
                Global.Skins.SkinList[index + 1] = temp;

                FileProcessing.FillSkinsList(form);
                Utils.SelectNextListItem(form, index + 1);

                form.Focus_SkinName();
                form.SelectAll_SkinName();

                form.Set_StatusLabel("Skin moved down!");
            }
        }
        public void OpenInstalledSkinFolder()
        {
            FolderProcessing.OpenInstalledPacks();
        }

        #endregion

        #region Private Methods

        private void UpdateDefaultSkinName(Form1 form, int index, string filename)
        {
            if (Global.Skins.SkinList[index].SkinName.Contains(SKIN_DEFAULT_NAME))
            {
                Global.Skins.SkinList[index].SkinName = filename;
                FileProcessing.FillSkinsList(form);
                form.Set_SkinListSelectedItem(index);
            }
        }
        private bool ValidSkins()
        {
            if (Global.Skins.SkinList.Any(x => string.IsNullOrEmpty(x.SkinName) || string.IsNullOrEmpty(x.SkinTextureFile)))
                return false;

            return true;
        }
        #endregion
    }
}
