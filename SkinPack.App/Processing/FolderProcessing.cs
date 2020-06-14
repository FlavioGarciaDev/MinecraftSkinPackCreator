using System.IO;

namespace SkinPackCreator.FolderProcessing
{
    public class FolderProcessing
    {
        public void CreateWorkDir()
        {
            Directory.CreateDirectory(Global.WorkDir);
            Directory.CreateDirectory(Global.TextsDir);
        }
        
    }
}
