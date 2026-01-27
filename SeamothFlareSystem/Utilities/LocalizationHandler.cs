using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Nautilus.Handlers;

namespace SeamothFlareSystem.Utilities
{
    namespace LocalizationHandler
    {
        [XmlRoot("LocalizationPackages")]
        public class LocalizationPackages
        {
            [XmlElement("LocalizationPackage")]
            public LocalizationPackage[]? Localizations;
        }

        /// <summary>
        /// LocalizationPackage is a Localization Package ref - 
        /// </summary>
        public class LocalizationPackage
        {
            [XmlAttribute("Lang")]
            public string? Lang;

            [XmlElement("Text")]
            public Text[]? Texts;
        }

        public class Text
        {
            [XmlAttribute]
            public string? key;
            [XmlText]
            public string? value;
        }
    }

    public class LanguagesHandler
    {
        private static readonly string ModPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static readonly string filename = "Localizations.xml";
        public static void LanguagePatch()
        {
            Plugin.Log?.LogInfo("Starting patching the languages !");
            XmlSerializer serializer = new(typeof(LocalizationHandler.LocalizationPackages));

            FileStream fs = new(Path.Combine(ModPath, filename), FileMode.Open, FileAccess.Read);
            LocalizationHandler.LocalizationPackages lps;

            Plugin.Log?.LogInfo(Language.main.GetCurrentLanguage());

            lps = (LocalizationHandler.LocalizationPackages)serializer.Deserialize(fs);

            foreach (LocalizationHandler.LocalizationPackage localizationpack in lps.Localizations ?? Enumerable.Empty<LocalizationHandler.LocalizationPackage>())
                Plugin.Log?.LogInfo(localizationpack.Lang);
            Plugin.Log?.LogInfo("All LPs logged.");

            var localizations = lps.Localizations;
            if (localizations != null)
            {
                var selectedPackage = localizations.SingleOrDefault(lp => localizations.Any(lp1 => lp1.Lang == Language.main.GetCurrentLanguage()) ? lp.Lang == Language.main.GetCurrentLanguage() : lp.Lang == Language.defaultLanguage);
                if (selectedPackage != null && selectedPackage.Texts != null)
                {
                    foreach (LocalizationHandler.Text text in selectedPackage.Texts)
                    {
                        Plugin.Log?.LogInfo($"Checking string, key {text.key}");
                        if (Language.main.Get(text.key) != null)
                        {
                            LanguageHandler.SetLanguageLine(text.key, text.value);
                            Plugin.Log?.LogInfo($"Patched key {text.key} with text '{(text.value?.Length > 50 ? text.value.Substring(0, 50) + "..." : text.value)}'");
                        }
                        else
                        {
                            Plugin.Log?.LogInfo($"Key {text.key} does not reference any key in game. Please check the case.");
                        }
                    }
                }
                else
                {
                    Plugin.Log?.LogInfo("No matching localization package or texts found.");
                }
            }
            else
            {
                Plugin.Log?.LogInfo("No localizations found in the XML file.");
            }
            Plugin.Log?.LogInfo("Language patching done.");
        }

        public static void GlobalPatch()
        {
            XmlSerializer serializer = new(typeof(LocalizationHandler.LocalizationPackages));

            FileStream fs = new(Path.Combine(ModPath, filename), FileMode.Open, FileAccess.Read);
            LocalizationHandler.LocalizationPackages lps;

            lps = (LocalizationHandler.LocalizationPackages)serializer.Deserialize(fs);

            if (lps.Localizations != null)
            {
                foreach (LocalizationHandler.LocalizationPackage locpack in lps.Localizations)
                {
                    Plugin.Log?.LogInfo(locpack.Lang);

                    foreach (LocalizationHandler.Text text in locpack.Texts ?? Enumerable.Empty<LocalizationHandler.Text>())
                    {
                        if (string.IsNullOrEmpty(text.value))
                            continue;
                        LanguageHandler.SetLanguageLine(text.key, text.value, locpack.Lang);
                    }
                }
            }
            else
            {
                Plugin.Log?.LogInfo("No localizations found in the XML file.");
            }
        }
    }
}
