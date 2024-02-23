using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace MaslasBros.Mod
{
    /// <summary>
    /// Represents a mod package containing basic information essential for compatibility checks.
    /// </summary>
    public struct ModPackage
    {
        ///<summary> The name of the mod. </summary>
        public string Name;
        ///<summary> The description of the mod. </summary>
        public string Description;
        ///<summary> The author of the mod. </summary>
        public string Author;
        ///<summary> The version of the mod. </summary>
        public string Version;
        ///<summary> The version of the game the mod is supported on. </summary>
        public string Supported;
    }

    /// <summary>
    /// Handles the management of mod packages, including loading, evaluation for compatibility and activation.
    /// Providing methods for accessing the lists of mod packages.  
    /// Performs the process of evaluation of a given path and return the proper one;
    /// </summary>
    public class ModManager
    {
        /// <summary> Instance of VersionCollection</summary>
        VersionCollection versionCollection;
        /// <summary> Instance of ValidationProcedure</summary>
        ValidationProcedure validationProcedure;
        /// <summary> Path to the Mods folder. </summary>
        string modsFolderPath;
        /// <summary> Path to the fallback folder. </summary>
        string monitoredFolder;
        /// <summary> List to save all loaded mod packages. </summary>
        List<ModPackage> allMods = new List<ModPackage>();
        /// <summary> List to save all loaded mods folder name. </summary>
        List<string> allModsDirectories = new List<string>();
        /// <summary> List to save all compatible mod packages.</summary>
        List<int> compatibleMods = new List<int>();
        /// <summary> List to save all incompatibles mod packages. </summary>
        List<int> incompatibleMods = new List<int>();
        /// <summary> Index of the currently active mod package. </summary>
        int activeModIndex = -1;

        ///<summary> Returns the index of the currently active mod package. </summary>
        public int ActiveModIndex => activeModIndex;

        /// <summary> Represents a method that provides a version collection to be used for compatibility checks. </summary>
        public delegate string VersionCollection();
        /// <summary> Represents a method that checks for mod compatibility based on a version. </summary>
        public delegate bool ValidationProcedure(string version, string dependency);

        /// <summary>
        /// Initializes a new instance of the ModManager class with parameters.
        /// </summary>
        /// <param name="monitoredPath"> The monitored folder will act as the fallback folder for file cross-check </param>
        /// <param name="modsFolderPath"> The Mod folder path where mod folders are located. </param>
        /// <param name="verCollection">The version collection of host(API) used for compatibility checks. </param>
        /// <param name="valProcedure"> The validation procedure of host(API) used for compatibility checks. </param>
        public ModManager(string monitoredFolder, string modsFolderPath, VersionCollection verCollection, ValidationProcedure valProcedure)
        {
            if (!Directory.Exists(monitoredFolder)) throw new Exception("The monitored path doesn't exist.");
            if (!Directory.Exists(modsFolderPath)) Directory.CreateDirectory(modsFolderPath);

#if UNITY_EDITOR
            this.monitoredFolder = Path.GetFullPath(monitoredFolder).Trim();
            this.modsFolderPath = Path.GetFullPath(modsFolderPath).Trim();
#else
            this.monitoredFolder = monitoredFolder;
            this.modsFolderPath = modsFolderPath;
#endif

            versionCollection = verCollection;
            validationProcedure = valProcedure;
            ReadAndDeserializeJsonFiles(this.modsFolderPath, ref allMods);
            LoadAndEvaluateModPackages(ref allMods, ref incompatibleMods, ref compatibleMods);
        }

        /// <summary>
        /// Loads and evaluates mod packages, categorizing them into compatible and incompatible lists based on version compatibility and validation procedure.
        /// </summary>
        /// <param name="allMods"> A reference to the list of allMods we want to check and add into other lists. </param>
        /// <param name="incombatibleMods"> A reference to the list of incompatible mod to fill. </param>
        /// <param name="combatibleMods"> A reference to the list of compatible mod to fill. </param>
        void LoadAndEvaluateModPackages(ref List<ModPackage> allMods, ref List<int> incombatibleMods, ref List<int> combatibleMods)
        {
            for (int i = 0; i < allMods.Count; i++)
            {
                if (validationProcedure(versionCollection(), allMods[i].Supported))
                { combatibleMods.Add(i); }
                else
                { incombatibleMods.Add(i); }
            }
        }

        /// <summary>
        /// Read and deserialize json files within the specified folder path, filling out the provided list with mod packages.
        /// Gets the directory name containing the JSON file and adds it to allModsdirectorieslist.
        /// </summary>
        /// <param name="folderPath"> The path to the Mods folder that contains the subfolders that contains the JSON files. </param>
        /// <param name="allMods"> A reference to the list of mod packages to fill. </param>
        /// <exception cref="Exception"> Thrown when a mod package contains more than one JSON file. </exception>
        void ReadAndDeserializeJsonFiles(string folderPath, ref List<ModPackage> allMods)
        {
            if (!Directory.Exists(folderPath)) return;

            string[] subdirectories = Directory.GetDirectories(folderPath);

            for (int i = 0; i < subdirectories.Length; i++)
            {
                string[] jsonFile = Directory.GetFiles(subdirectories[i], "*.json");

                if (jsonFile.Length > 1)
                { throw new Exception("Each mod package should contain only one json file!"); }

                if (jsonFile.Length > 0)
                {
                    string directoryPath = Path.GetFileName(Path.GetDirectoryName(jsonFile[0]));
                    allModsDirectories.Add(directoryPath);

                    string jsonContent = File.ReadAllText(jsonFile[0]);
                    ModPackage modPackage = JsonConvert.DeserializeObject<ModPackage>(jsonContent);
                    allMods.Add(modPackage);
                }
            }
        }

        /// <summary>
        /// Activates a mod base on the given index.
        /// </summary>
        /// <param name="modIndex"> The index of the mod to activate. </param>
        /// <returns> True if the mod was activated, false if it was deactivated. </returns>
        public bool ToggleMod(int modIndex)
        {
            bool ActivateMod = false;
            modIndex = Math.Clamp(modIndex, 0, allMods.Count - 1);

            if (compatibleMods.Contains(modIndex))
            {
                if (modIndex == activeModIndex)
                {
                    ActivateMod = false;
                    activeModIndex = -1;
                }
                else
                {
                    ActivateMod = true;
                    activeModIndex = modIndex;
                }
            }

            return ActivateMod;
        }

        /// <summary>
        /// Retrieves a read-only collection of all available mod packages.
        /// </summary>
        /// <returns> A collection of all available mod packages. </returns>
        public IReadOnlyCollection<ModPackage> GetAllMods()
        {
            ModPackage[] allMod = new ModPackage[allMods.Count];

            for (int i = 0; i < allMod.Length; i++)
            {
                allMod[i] = allMods[i];
            }

            return allMod;
        }

        /// <summary>
        ///  Retrieves a read-only collection of mod packages that are compatible based on their index within the 'allMods' collection. 
        /// </summary>
        /// <returns>  A read-only collection of mod packages that are compatible. </returns>
        public IReadOnlyCollection<ModPackage> GetCompatibleMods()
        {
            ModPackage[] compatibleMod = new ModPackage[compatibleMods.Count];

            for (int i = 0; i < compatibleMod.Length; i++)
            {
                int compatblesModIndex = compatibleMods[i];
                compatibleMod[i] = allMods[compatblesModIndex];
            }

            return compatibleMod;
        }

        /// <summary>
        /// Retrieves a read-only collection of mod packages that are incompatible based on their index within the 'allMods' collection 
        /// </summary>
        /// <returns> A read-only collection of mod packages that are incompatible. </returns>
        public IReadOnlyCollection<ModPackage> GetIncompatibleMods()
        {
            ModPackage[] incompatibleMod = new ModPackage[incompatibleMods.Count];

            for (int i = 0; i < incompatibleMod.Length; i++)
            {
                int incompatblesModIndex = incompatibleMods[i];
                incompatibleMod[i] = allMods[incompatblesModIndex];
            }

            return incompatibleMod;
        }

        /// <summary>
        /// Return the mod package based on a specified index.
        /// </summary>
        /// <param name="modIndex"> The index of the mod package to get. </param>
        /// <returns> The mod package from allMods list at the specified index. </returns>
        public ModPackage GetMod(int modIndex)
        {
            modIndex = Math.Clamp(modIndex, 0, allMods.Count - 1);
            ModPackage mod = allMods[modIndex];
            return mod;
        }

        /// <summary>   
        /// Evaluates the provided path and returns an absolute path based on certain conditions.
        /// </summary>
        /// <param name="pathToEvaluate"> The path to be evaluated. </param>
        /// <returns> The given path or a combined path. </returns>
        public string EvaluatePath(string pathToEvaluate)
        {
            string path = string.Empty;

            if (Path.IsPathRooted(pathToEvaluate))
            {
                pathToEvaluate = pathToEvaluate.Trim();

                if (!IsPathPartOf(modsFolderPath, pathToEvaluate) && !IsPathPartOf(monitoredFolder, pathToEvaluate))
                { throw new Exception("Passed path not pointing to either monitored or mods folder"); }

                path = pathToEvaluate;
            }
            else
            { path = Path.Combine(modsFolderPath, pathToEvaluate); } //we default at mods folder searching

            if (activeModIndex == -1)
            {
                if (!PathExistsInMonitored(pathToEvaluate))
                { throw new Exception("Passed path to evaluate is not present in monitored folder."); }
                else
                { return Path.Combine(monitoredFolder, pathToEvaluate); }
            }

            string activeModName = allModsDirectories[activeModIndex];
            string combinedPath = Path.Combine(modsFolderPath, activeModName, pathToEvaluate);

            if (File.Exists(combinedPath))
            { path = combinedPath; }
            else
            {
                if (!PathExistsInMonitored(pathToEvaluate))
                { throw new Exception("Passed path to evaluate is not present in monitored folder."); }
                else
                { return Path.Combine(monitoredFolder, pathToEvaluate); }
            }

            return path;
        }

        /// <summary>
        /// Checks whether a specified path is part of a base path.
        /// </summary>
        /// <param name="basePath"> The base path. </param>
        /// <param name="fullPath"> The full path to be checked. </param>
        /// <returns> True if the full path is part of the base path.. otherwise, false. </returns>
        bool IsPathPartOf(string basePath, string fullPath)
        {
            string fullBasePath = Path.GetFullPath(basePath);
            string fullFullPath = Path.GetFullPath(fullPath);

            return fullFullPath.StartsWith(fullBasePath, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Checks if the specified path exists within the monitored folder.
        /// </summary>
        /// <param name="pathToEvaluate"> The path to be checked. </param>
        /// <returns> True if the path exists within the monitored folder.. otherwise, false. </returns>
        bool PathExistsInMonitored(string pathToEvaluate)
        {
            string path = Path.Combine(monitoredFolder, pathToEvaluate);
            return File.Exists(Path.GetFullPath(path, monitoredFolder));
        }
    }
}