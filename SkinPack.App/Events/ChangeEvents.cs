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

        public void SkinListIndexChanged(Form1 form)
        {
            int Index = form.Get_SkinListSelectedIndex();

            form.Set_SkinName(Global.Skins.SkinList[Index].SkinName);
            form.Set_SkinFormat(Global.Skins.SkinList[Index].SkinFormat);
            form.Set_SkinTexture(Global.Skins.SkinList[Index].SkinTextureFile);
            form.Set_SkinType(Global.Skins.SkinList[Index].SkinType);
            form.Set_SkinBoxPicture(Utils.LoadImagePreview(form, Index));
            

            form.Set_StatusLabel($"{Global.Skins.SkinList[Index].SkinName} loaded.");
        }
        public void SkinNameChanged(Form1 form)
        {
            int Index = form.Get_SkinListSelectedIndex();
            string ChangedSkinName = form.Get_SkinName();
            if (Index < 0)
                return;

            if (Global.Skins.SkinList[Index].SkinName != ChangedSkinName && !String.IsNullOrEmpty(ChangedSkinName))
            {
                string OldName = Global.Skins.SkinList[Index].SkinName;
                string NewName = ChangedSkinName;

                Global.Skins.SkinList[Index].SkinName = ChangedSkinName;

                Process.FillSkinsList(form);
                Utils.SelectNextListItem(form, Index);

                form.Set_StatusLabel($"{OldName} renamed to {NewName}.");
            }
        }
        public void SkinFormatChanged(Form1 form)
        {
            int Index = form.Get_SkinListSelectedIndex();

            Global.Skins.SkinList[Index].SkinFormat = form.Get_SkinFormat();
        }
    }
}
