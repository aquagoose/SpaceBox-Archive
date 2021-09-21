using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using OpenTK.Mathematics;

namespace SpaceBox.Data
{
    public static class Data
    {
        public static string SpaceBoxFolderLocation { get; set; } =
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        public static string SpaceBoxFolderName { get; set; } = "SpaceBox";

        public static string SavesFolderName { get; set; } = "Saves";

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

        public static void SaveWorld(string worldName, Vector3 playerPosition, Vector3 playerRotation, List<Vector3> cubePositions)
        {
            string savePath = Path.Combine(SpaceBoxFolderLocation, SpaceBoxFolderName, SavesFolderName, worldName);
            if (!Directory.Exists(Path.Combine(SpaceBoxFolderLocation, SpaceBoxFolderName, SavesFolderName)))
                Directory.CreateDirectory(Path.Combine(SpaceBoxFolderLocation, SpaceBoxFolderName, SavesFolderName));

            XmlSerializer serializer = new XmlSerializer(typeof(SaveGame));

            SaveGame saveGame = new SaveGame()
            {
                WorldName = worldName,
                PlayerPosition = playerPosition,
                PlayerRotation = playerRotation,
                BlockPositions = cubePositions.ToArray()
            };
            
            using (FileStream stream =
                new FileStream(
                    Path.Combine(SpaceBoxFolderLocation, SpaceBoxFolderName, SavesFolderName,
                        worldName.Replace(' ', '_') + ".world"), FileMode.Create))
                serializer.Serialize(stream, saveGame);
        }

        public static SaveGame LoadSave(string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SaveGame));

            SaveGame game;
            
            using (FileStream stream = new FileStream(path, FileMode.Open))
                game = (SaveGame) serializer.Deserialize(stream);

            return game;
        }
    }
}