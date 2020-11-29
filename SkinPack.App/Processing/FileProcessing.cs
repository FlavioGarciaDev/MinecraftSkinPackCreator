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

            var locName = form.Get_PackName().Replace(" ", "");
            if (!String.IsNullOrEmpty(locName))
            {
                Global.Skins.PackLocName = locName;
                Global.Skins.PackSerializeName = locName;
                Global.LocName = locName;
            }

            var jsonSkins = JsonConvert.SerializeObject(Global.Skins, Formatting.Indented);

            File.WriteAllText(Global.SkinJsonFilePath, jsonSkins);

            form.Set_StatusLabel($"{Global.SkinJsonFilePath} created!");

        }
        public void CreateEnUsLangFile(Form1 form)
        {
            var enUsLangFile = Global.EnUsLangFilePath;

            form.Set_StatusLabel($"Creating {enUsLangFile}...");

            var lines = new StringBuilder();

            lines.AppendLine($"{Global.SkinPackLine}={form.Get_PackName()}");
            lines.AppendLine($"{Global.PackNameLine}={form.Get_PackName()}");
            lines.AppendLine($"{Global.PackDescriptionLine}={form.Get_PackDescription()}");

            foreach (var item in Global.Skins.SkinList)
            {
                lines.AppendLine($"{Global.SkinLine}.{item.SkinName}={item.SkinName}");
            }

            File.WriteAllText(enUsLangFile, lines.ToString());

            form.Set_StatusLabel($"{enUsLangFile} created!");
        }
        public void CreateLanguagesJsonFile(Form1 form)
        {
            var languagesJsonFile = Global.LanguagesJsonFilePath;
            var content = $"[\"{Path.GetFileNameWithoutExtension(Global.EnUsLangFilePath)}\"]";

            form.Set_StatusLabel($"Creating {languagesJsonFile}...");

            File.WriteAllText(languagesJsonFile, content);

            form.Set_StatusLabel($"{languagesJsonFile} created!");
        }
        public async Task CreateManifestJsonFile(Form1 form)
        {
            var manifestJsonFile = Global.ManifestJsonFilePath;

            if (String.IsNullOrEmpty(Global.Manifest.Header.UUID))
            {
                form.Set_StatusLabel($"Generating UUIDs...");

                var uuid1 = await Utils.RequestUUID();
                var uuid2 = await Utils.RequestUUID();

                form.Set_StatusLabel($"Creating {manifestJsonFile}...");

                Global.Manifest = new ManifestModel()
                {
                    Header = new HeaderModel()
                    {
                        Name = form.Get_PackName(),
                        Description = form.Get_PackDescription(),
                        Version = new int[] { 1, 0, 0 },
                        UUID = uuid1
                    },
                    Modules = new List<ModulesModel>()
                {
                    new ModulesModel()
                    {
                        Version = new int[] { 1, 0, 0 },
                        Type = "skin_pack",
                        UUID = uuid2
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

            File.WriteAllText(manifestJsonFile, JsonConvert.SerializeObject(Global.Manifest, Formatting.Indented));

            form.Set_StatusLabel($"{manifestJsonFile} created!");
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
            string mcpackFilePath = Global.McpackFilePath;

            form.Set_StatusLabel("Extracting McPack...");

            Global.WorkDir = $"{Path.GetDirectoryName(mcpackFilePath)}\\{Path.GetFileNameWithoutExtension(mcpackFilePath)}";

            using (ZipFile zip = ZipFile.Read(mcpackFilePath))
            {
                zip.ExtractAll(Global.WorkDir, ExtractExistingFileAction.OverwriteSilently);
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
            var enUsLangFileContent = File.ReadAllLines(Global.EnUsLangFilePath);

            foreach (var item in enUsLangFileContent)
            {
                if (item.Contains(Global.PackNameLine))
                {
                    var name = item.Split('=');
                    form.Set_PackName(name[1]);
                }
                else if (item.Contains(Global.PackDescriptionLine))
                {
                    var desc = item.Split('=');
                    form.Set_PackDescription(desc[1]);
                }
            }
        }

    }
}
