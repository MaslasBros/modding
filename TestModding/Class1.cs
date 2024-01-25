using Newtonsoft.Json;
using System;
using System.IO;

namespace TestModding
{
    public class ModPackage
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string Version { get; set; }
        public string Supported { get; set; }
    }

    class SampleJson
    {
        public void CreateJson()
        {
            // Create a sample ModPackage
            ModPackage myPackage = new ModPackage
            { 
                Name = "my_package",
                Description = "",
                Author = "",
                Version = "1.0.0",
                Supported = "^1.0.3"
            };

            // Serialize the ModPackage to JSON
            string json = JsonConvert.SerializeObject(myPackage, Formatting.Indented);

            #region testing          
            // Specifying the directory path, very likely it should be changed to take the path
            string directoryPath = "Var/";

                // Check if the directory exists
                if (!Directory.Exists(directoryPath))
                {
                    // Creating the directory if it doesn't exist
                    Directory.CreateDirectory(directoryPath);
                }
            #endregion

            // Specify the file path
            string filePath = Path.Combine(directoryPath, "modPackage2.json");

            // Saving the JSON string to a file
            File.WriteAllText(filePath, json);
        }
    }
}
