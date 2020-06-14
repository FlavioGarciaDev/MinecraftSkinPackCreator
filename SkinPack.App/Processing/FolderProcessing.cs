using System.Diagnostics;
using System.IO;

namespace SkinPackCreator.FolderProcessing
{
    public class FolderProcessing
    {
        private readonly string INSTALLED_SKIN_FOLDER = "{0}\\AppData\\Local\\Packages\\Microsoft.MinecraftUWP_8wekyb3d8bbwe\\LocalState\\games\\com.mojang\\skin_packs";
        public void CreateWorkDir()
        {
            Directory.CreateDirectory(Global.WorkDir);
            Directory.CreateDirectory(Global.TextsDir);
        }      

        public void OpenInstalledPacks()
        {
            var Folder = string.Format(INSTALLED_SKIN_FOLDER, System.Environment.GetEnvironmentVariable("USERPROFILE"));
            Process.Start(Folder);
        }
    }
}
