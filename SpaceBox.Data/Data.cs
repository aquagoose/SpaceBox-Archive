using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Cubic.Data.Serialization;
using OpenTK.Mathematics;
using SpaceBox.Data.Serialization;
using SpaceBox.Sandbox.Grids;

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

        public static void SaveWorld(string worldName, Vector3 playerPosition, Quaternion playerRotation, List<Grid> grids)
        {
            Console.WriteLine("Saving...");
            string savePath = Path.Combine(SpaceBoxFolderLocation, SpaceBoxFolderName, SavesFolderName, worldName);
            if (!Directory.Exists(Path.Combine(SpaceBoxFolderLocation, SpaceBoxFolderName, SavesFolderName)))
                Directory.CreateDirectory(Path.Combine(SpaceBoxFolderLocation, SpaceBoxFolderName, SavesFolderName));

            XmlSerializer serializer = new XmlSerializer(typeof(SaveGame));

            List<SerializableGrid> sGrids = new List<SerializableGrid>();
            List<SerializableBlock> sBlocks = new List<SerializableBlock>();

            foreach (Grid grid in grids)
            {
                foreach (Block block in grid.Blocks)
                    sBlocks.Add(new SerializableBlock()
                    {
                        Name = "Temp",
                        Coord = block.Coord.ToSerializable()
                    });
                
                sGrids.Add(new SerializableGrid()
                {
                    Name = "Temp",
                    Blocks = sBlocks.ToArray(),
                    GridSize = grid.Size,
                    GridType = grid.GridType,
                    Orientation = grid.Orientation.ToSerializable(),
                    Position = grid.Position.ToSerializable()
                });
                sBlocks.Clear();
            }

            SaveGame saveGame = new SaveGame()
            {
                WorldName = worldName,
                PlayerPosition = playerPosition.ToSerializable(),
                PlayerRotation = playerRotation.ToSerializable(),
                Grids = sGrids.ToArray()
            };
            
            using (FileStream stream =
                new FileStream(
                    Path.Combine(SpaceBoxFolderLocation, SpaceBoxFolderName, SavesFolderName,
                        worldName.Replace(' ', '_') + ".world"), FileMode.Create))
                serializer.Serialize(stream, saveGame);
            Console.WriteLine("!! SAVED !!");
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