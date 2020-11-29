using System;
using Process = SkinPackCreator.FileProcessing;

namespace SkinPackCreator.ChangeEvents
{
    public class ChangeEvents
    {
        private Utils.Utils Utils { get; set; }
        private Process.FileProcessing Process { get; set; }
        public ChangeEvents()
        {
            Utils = new Utils.Utils();
            Process = new Process.FileProcessing();
        }

        #region Public Methods

        public void SkinListIndexChanged(Form1 form)
        {
            var index = form.Get_SkinListSelectedIndex();

            form.Set_SkinName(Global.Skins.SkinList[index].SkinName);
            form.Set_SkinFormat(Global.Skins.SkinList[index].SkinFormat);
            form.Set_SkinTexture(Global.Skins.SkinList[index].SkinTextureFile);
            form.Set_SkinType(Global.Skins.SkinList[index].SkinType);
            form.Set_SkinBoxPicture(Utils.LoadImagePreview(form, index));
            

            form.Set_StatusLabel($"{Global.Skins.SkinList[index].SkinName} loaded.");
        }
        public void SkinNameChanged(Form1 form)
        {
            int index = form.Get_SkinListSelectedIndex();
            if (index < 0)
                return;

            var changedSkinName = form.Get_SkinName();

            if (Global.Skins.SkinList[index].SkinName != changedSkinName && !String.IsNullOrEmpty(changedSkinName))
            {
                var oldName = Global.Skins.SkinList[index].SkinName;
                var newName = changedSkinName;

                Global.Skins.SkinList[index].SkinName = changedSkinName;

                Process.FillSkinsList(form);
                Utils.SelectNextListItem(form, index);

                form.Set_StatusLabel($"{oldName} renamed to {newName}.");
            }
        }
        public void SkinFormatChanged(Form1 form)
        {
            var index = form.Get_SkinListSelectedIndex();

            Global.Skins.SkinList[index].SkinFormat = form.Get_SkinFormat();
        }

        #endregion
    }
}
