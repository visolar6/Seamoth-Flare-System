using Nautilus.Options.Attributes;

namespace SeamothFlareSystem
{
    [Menu("Seamoth Flare System")]
    public class Options : Nautilus.Json.ConfigFile
    {
        [Slider("Flare Launch Force", 10f, 100f, DefaultValue = 40f, Step = 5f, LabelLanguageId = "Options.SeamothFlareSystem_LaunchForce", TooltipLanguageId = "Options.SeamothFlareSystem_LaunchForce.Tooltip")]
        public float LaunchForceAmount = 40f;
    }
}
