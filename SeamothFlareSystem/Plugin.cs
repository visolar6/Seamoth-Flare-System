using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;
using SeamothFlareSystem.Items;
using SeamothFlareSystem.Utilities;

namespace SeamothFlareSystem
{
    [BepInPlugin(GUID, Name, Version)]
    public class Plugin : BaseUnityPlugin
    {
        public static Options Options { get; } = OptionsPanelHandler.RegisterModOptions<Options>();

        public static ManualLogSource? Log;

        public static ResourceCacheManager? AssetsCache { get; private set; }

        internal const string GUID = "com.visolar6.seamothflaresystem";

        internal const string Name = "Seamoth Flare System";

        internal const string Version = "0.1.0";

        private readonly Harmony _harmony = new(GUID);

        /// <summary>
        /// Awakes the plugin (on game start).
        /// </summary>
        public void Awake()
        {
            Log = Logger;

            AssetsCache = ResourceCacheManager.LoadResources(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "seamothflaresystem.assets"));
        }

        public void Start()
        {
            Log?.LogInfo($"Initializing prefab cache...");
            StartCoroutine(PrefabCacheManager.Initialize());

            Log?.LogInfo($"Patching hooks...");
            _harmony.PatchAll();

            Log?.LogInfo($"Patching localization...");
            LanguagesHandler.GlobalPatch();

            Log?.LogInfo($"Patching seamoth flare module...");
            SeamothFlareModule seamothFlareModule = new();
            seamothFlareModule.Patch();

            Log?.LogInfo($"Initialized!");
        }
    }
}
