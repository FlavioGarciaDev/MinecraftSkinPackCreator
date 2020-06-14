using SkinPackCreator.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Ionic.Zip;

namespace SkinPackCreator.FileProcessing
{
    public class FileProcessing
    {
        private const string SKINLIST_FILL = "Filling the skins list...";
        private const string SKINLIST_OK = "List filled!";
        private const string MANIFEST_LOAD = "Loading manifest.json...";
        private const string MANIFEST_OK = "manifest.json loaded!";

        private Utils.Utils Utils { get; set; }

        public FileProcessing()
        {
            Utils = new Utils.Utils();
        }

        public void CreateSkinsJsonFile(Form1 form)
        {
            form.Set_StatusLabel($"Creating {Global.SkinJsonFilePath}...");

            var LocName = form.Get_PackName().Replace(" ", "");
            if (!String.IsNullOrEmpty(LocName))
            {
                Global.Skins.PackLocName = LocName;
                Global.Skins.PackSerializeName = LocName;
                Global.LocName = LocName;
            }

            var JsonSkins = JsonConvert.SerializeObject(Global.Skins, Formatting.Indented);

            File.WriteAllText(Global.SkinJsonFilePath, JsonSkins);

            form.Set_StatusLabel($"{Global.SkinJsonFilePath} created!");

        }
        public void CreateEnUsLangFile(Form1 form)
        {
            var EnUsLangFile = Global.EnUsLangFilePath;

            form.Set_StatusLabel($"Creating {EnUsLangFile}...");

            var Lines = new StringBuilder();

            Lines.AppendLine($"{Global.SkinPackLine}={form.Get_PackName()}");
            Lines.AppendLine($"{Global.PackNameLine}={form.Get_PackName()}");
            Lines.AppendLine($"{Global.PackDescriptionLine}={form.Get_PackDescription()}");

            foreach (var item in Global.Skins.SkinList)
            {
                Lines.AppendLine($"{Global.SkinLine}.{item.SkinName}={item.SkinName}");
            }

            File.WriteAllText(EnUsLangFile, Lines.ToString());

            form.Set_StatusLabel($"{EnUsLangFile} created!");
        }
        public void CreateLanguagesJsonFile(Form1 form)
        {
            var LanguagesJsonFile = Global.LanguagesJsonFilePath;
            var Content = $"[\"{Path.GetFileNameWithoutExtension(Global.EnUsLangFilePath)}\"]";

            form.Set_StatusLabel($"Creating {LanguagesJsonFile}...");

            File.WriteAllText(LanguagesJsonFile, Content);

            form.Set_StatusLabel($"{LanguagesJsonFile} created!");
        }
        public async Task CreateManifestJsonFile(Form1 form)
        {
            var ManifestJsonFile = Global.ManifestJsonFilePath;

            if (String.IsNullOrEmpty(Global.Manifest.Header.UUID))
            {
                form.Set_StatusLabel($"Generating UUIDs...");

                var Uuid1 = await Utils.RequestUUID();
                var Uuid2 = await Utils.RequestUUID();

                form.Set_StatusLabel($"Creating {ManifestJsonFile}...");

                Global.Manifest = new ManifestModel()
                {
                    Header = new HeaderModel()
                    {
                        Name = form.Get_PackName(),
                        Description = form.Get_PackDescription(),
                        Version = new int[] { 1, 0, 0 },
                        UUID = Uuid1
                    },
                    Modules = new List<ModulesModel>()
                {
                    new ModulesModel()
                    {
                        Version = new int[] { 1, 0, 0 },
                        Type = "skin_pack",
                        UUID = Uuid2
                    }
                },
                    FormatVersion = 1
                };
            }
            else
            {
                Global.Manifest.Header.Name = form.Get_PackName();
                Global.Manifest.Header.Description = form.Get_PackDescription();
            }

            File.WriteAllText(ManifestJsonFile, JsonConvert.SerializeObject(Global.Manifest, Formatting.Indented));

            form.Set_StatusLabel($"{ManifestJsonFile} created!");
        }
        public void CreateMcpackFile(Form1 form)
        {
            form.Set_StatusLabel("Creating the McPack file...");

            if (File.Exists(Global.McpackFilePath))
                File.Delete(Global.McpackFilePath);

            using (ZipFile Zip = new ZipFile())
            {
                Zip.AddDirectory(Global.WorkDir);
                Zip.Save(Global.McpackFilePath);
            }

            form.Set_StatusLabel($"{Global.McpackFilePath} created!");
        }
        public void ExtractMcpackFile(Form1 form)
        {
            string McpackFilePath = Global.McpackFilePath;

            form.Set_StatusLabel("Extracting McPack...");

            Global.WorkDir = $"{Path.GetDirectoryName(McpackFilePath)}\\{Path.GetFileNameWithoutExtension(McpackFilePath)}";

            using (ZipFile Zip = ZipFile.Read(McpackFilePath))
            {
                Zip.ExtractAll(Global.WorkDir, ExtractExistingFileAction.OverwriteSilently);
            }

            form.Set_StatusLabel($"{Path.GetFileName(Global.McpackFilePath)} extracted!");
        }
        public void LoadSkinsJson(Form1 form)
        {
            form.Set_StatusLabel("Loading skins...");

            Global.Skins = JsonConvert.DeserializeObject<SkinModel>(File.ReadAllText(Global.SkinJsonFilePath));

            form.Set_StatusLabel($"{Path.GetFileName(Global.SkinJsonFilePath)} loaded!");
        }
        public void FillSkinsList(Form1 form)
        {
            form.Set_StatusLabel(SKINLIST_FILL);
            form.ClearSkinsList();

            foreach (var item in Global.Skins.SkinList)
                form.Set_SkinListAddItem(item.SkinName);

            if (Global.Skins.SkinList.Count > 0)
                form.Set_SkinListSelectedItem(0);

            form.Set_StatusLabel(SKINLIST_OK);
        }
        public void LoadManifestJson(Form1 form)
        {
            form.Set_StatusLabel(MANIFEST_LOAD);

            Global.Manifest = JsonConvert.DeserializeObject<ManifestModel>(File.ReadAllText(Global.ManifestJsonFilePath));

            form.Set_StatusLabel(MANIFEST_OK);
        }
        public void LoadPackSettings(Form1 form)
        {
            var EnUsLangFileContent = File.ReadAllLines(Global.EnUsLangFilePath);

            foreach (var item in EnUsLangFileContent)
            {
                if (item.Contains(Global.PackNameLine))
                {
                    var Name = item.Split('=');
                    form.Set_PackName(Name[1]);
                }
                else if (item.Contains(Global.PackDescriptionLine))
                {
                    var Desc = item.Split('=');
                    form.Set_PackDescription(Desc[1]);
                }
            }
        }

    }
}
