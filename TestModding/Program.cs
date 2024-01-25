using System;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;

namespace TestModding
{
    internal class Program
    {
        static Class2 class2 = new Class2();
        static ModPackage modPackage = new ModPackage();
        static SampleJson sampleJson = new SampleJson();

        static void Main()
        {
            //For testing  i will move this to an different script
            //that will call all methods that are needed
            sampleJson.CreateJson();
            LoadAndEvaluateModPackages();
        }

        //public void LoadAndEvaluateModPackages(string jsonScript)
        static void LoadAndEvaluateModPackages()
        {
            // Read the JSON script from a file
            string jsonScriptFromFile = File.ReadAllText("Var/modPackage.json");

            // Deserialize the JSON script to a list of ModPackage
            modPackage = JsonConvert.DeserializeObject<ModPackage>(jsonScriptFromFile);

            // Get the host API version
            string hostApiVersion = class2.GetHostVersion();

            bool isValid = ValidateMod(modPackage, hostApiVersion);

            //Debugs
            Console.WriteLine(modPackage.Supported);
            Console.WriteLine(hostApiVersion);

            if (isValid) Console.WriteLine($"ModPackage '{modPackage.Name}' is valid: {isValid}");
            else { Console.WriteLine("Is not valid"); }

            //Just so the console dont close
            string numInput1 = Console.ReadLine();
        }

        static bool ValidateMod(ModPackage modPackage, string hostApiVersion)
        {
            return modPackage.Supported == hostApiVersion;
        }

        void ModPath(string path)
        {
            // Get the path from the Mod Manager
        }

      /*  List<ModPackage> ListMods()
        {
            // List of available mods and their status
        }

        List<ModPackage> ListFilteredMods(string filter)
        {
            // Return the list filtered by All, Compatible and Incompatible
        }*/

        void ActivateMod(int index)
        {
            // Only one mod can be active at a time
        }

        int GetActiveModIndex()
        {
            return 0;
        }
    }
}