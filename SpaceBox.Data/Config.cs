using System;
using System.IO;
using System.Xml.Serialization;

namespace SpaceBox.Data
{
    public static class Config
    {
        public static string SpaceBoxFolderLocation { get; set; } =
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        public static string SpaceBoxFolderName { get; set; } = "SpaceBox";

        public static SpaceboxConfig GetSpaceBoxConfig(string path)
        {
            string configPath = Path.Combine(SpaceBoxFolderLocation, SpaceBoxFolderName, path);
            if (!File.Exists(configPath))
                return null;

            XmlSerializer serializer = new XmlSerializer(typeof(SpaceboxConfig));

            using (FileStream stream = new FileStream(configPath, FileMode.Open))
                return (SpaceboxConfig) serializer.Deserialize(stream);
        }

        public static void SaveSpaceBoxConfig(SpaceboxConfig config, string path)
        {
            string configPath = Path.Combine(SpaceBoxFolderLocation, SpaceBoxFolderName, path);
            if (!Directory.Exists(Path.Combine(SpaceBoxFolderLocation, SpaceBoxFolderName)))
                Directory.CreateDirectory(Path.Combine(SpaceBoxFolderLocation, SpaceBoxFolderName));

            XmlSerializer serializer = new XmlSerializer(typeof(SpaceboxConfig));
            
            using (FileStream stream = new FileStream(configPath, FileMode.Create))
                serializer.Serialize(stream, config);
        }
    }
}