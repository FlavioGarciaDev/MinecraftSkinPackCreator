using System.IO;
using System.Windows.Forms;
using FileProcessing = SkinPackCreator.FileProcessing;
using FolderProcessing = SkinPackCreator.FolderProcessing;
using System.Threading.Tasks;
using System.Data;
using SkinPackCreator.Models;

namespace SkinPackCreator.ClickEvents
{
    public class ClickEvents
    {
        private const string SAVEDIALOG_TITLE = "Choose the folder and the McPack name";
        private const string MCPACK_FILTER = "McPack Files (*.mcpack)|*.mcpack";
        private const string MCPACK_OPEN_TITLE = "Select a McPack file";
        private const string IMAGE_FILTER = "Images (*.png)|*.png";
        private const string LOAD_OK = "Everything loaded! :D";
        private const string MCPACK_CREATED = "New McPack created!";
        private const string NEW_SKIN = "A new item has been added!";

        private FileProcessing.FileProcessing FileProcessing { get; set; }
        private FolderProcessing.FolderProcessing FolderProcessing { get; set; }
        private Utils.Utils Utils { get; set; }
        public ClickEvents()
        {
            FileProcessing = new FileProcessing.FileProcessing();
            FolderProcessing = new FolderProcessing.FolderProcessing();
            Utils = new Utils.Utils();
        }

        public void NewMcpackFile(Form1 form)
        {
            SaveFileDialog Sfd = new SaveFileDialog()
            {
                Title = SAVEDIALOG_TITLE,
                Filter = MCPACK_FILTER
            };

            var Result = Sfd.ShowDialog();

            if (Result == DialogResult.OK)
            {
                Global.McpackFilePath = Sfd.FileName;
                Global.WorkDir = $"{Path.GetDirectoryName(Sfd.FileName)}\\{Path.GetFileNameWithoutExtension(Sfd.FileName)}";

                form.ResetAllControls();

                form.Set_PackName(Path.GetFileNameWithoutExtension(Sfd.FileName));

                FolderProcessing.CreateWorkDir();

                form.Set_StatusLabel(MCPACK_CREATED);

            }
        }
        public async Task SaveMcpack(Form1 form)
        {
            if (Global.Skins.SkinList.Count > 0)
            {
                FileProcessing.CreateSkinsJsonFile(form);
                FileProcessing.CreateEnUsLangFile(form);
                FileProcessing.CreateLanguagesJsonFile(form);
                await FileProcessing.CreateManifestJsonFile(form);
                FileProcessing.CreateMcpackFile(form);
            }
            else
            {
                MessageBox.Show("There's nothing to save!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        public void OpenMcPackFile(Form1 form)
        {
            OpenFileDialog fdb = new OpenFileDialog
            {
                Filter = MCPACK_FILTER,
                Title = MCPACK_OPEN_TITLE
            };

            var Result = fdb.ShowDialog();

            if (Result == DialogResult.OK)
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
                Filter = IMAGE_FILTER
            };

            var Result = Ofd.ShowDialog();

            if (Result == DialogResult.OK)
            {
                int Index = form.Get_SkinListSelectedIndex();
                var NomeArquivo = Path.GetFileName(Ofd.FileName);

                Global.Skins.SkinList[Index].SkinTextureFile = NomeArquivo;
                form.Set_SkinTexture(NomeArquivo);

                File.Copy(Ofd.FileName, $"{Global.WorkDir}\\{NomeArquivo}", true);

                form.Set_SkinBoxPicture(Utils.LoadImagePreview(form, Index));

                form.Set_StatusLabel($"{NomeArquivo} loaded with high quality. Nice!");
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
            int Index = form.Get_SkinListSelectedIndex();
            var ImageFile = $"{Global.WorkDir}\\{Global.Skins.SkinList[Index].SkinTextureFile}";

            if (File.Exists(ImageFile))
                File.Delete(ImageFile);

            Global.Skins.SkinList.RemoveAt(Index);

            FileProcessing.FillSkinsList(form);
            Utils.SelectNextListItem(form, Index);
        }
        public void OpenInstalledSkinFolder()
        {
            FolderProcessing.OpenInstalledPacks();
        }
    }
}
