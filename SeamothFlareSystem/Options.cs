using Nautilus.Options.Attributes;

namespace SeamothFlareSystem
{
    [Menu("Seamoth Flare System")]
    public class Options : Nautilus.Json.ConfigFile
    {
        [Toggle(LabelLanguageId = "Options.SeamothFlareSystem_RequireCrashfish", TooltipLanguageId = "Options.SeamothFlareSystem_RequireCrashfish.Tooltip")]
        public bool RequireCrashfish = true;

        [Slider(LabelLanguageId = "Options.SeamothFlareSystem_LaunchEnergyCost", TooltipLanguageId = "Options.SeamothFlareSystem_LaunchEnergyCost.Tooltip", Min = 0f, Max = 5f, DefaultValue = 0.5f, Step = 0.1f)]
        public float LaunchEnergyCost = 0.5f;

        [Slider(LabelLanguageId = "Options.SeamothFlareSystem_LaunchCooldown", TooltipLanguageId = "Options.SeamothFlareSystem_LaunchCooldown.Tooltip", Min = 0f, Max = 15f, DefaultValue = 3f, Step = 1f)]
        public float LaunchCooldownSeconds = 3f;

        [Slider(LabelLanguageId = "Options.SeamothFlareSystem_LaunchForce", TooltipLanguageId = "Options.SeamothFlareSystem_LaunchForce.Tooltip", Min = 5f, Max = 100f, DefaultValue = 40f, Step = 5f)]
        public float LaunchForceAmount = 40f;

        [Slider(LabelLanguageId = "Options.SeamothFlareSystem_FlareLightIntensity", TooltipLanguageId = "Options.SeamothFlareSystem_FlareLightIntensity.Tooltip", Min = 0.5f, Max = 10f, DefaultValue = 3f, Step = 0.1f)]
        public float FlareLightIntensity = 3f;

        [Slider(LabelLanguageId = "Options.SeamothFlareSystem_FlareLightRange", TooltipLanguageId = "Options.SeamothFlareSystem_FlareLightRange.Tooltip", Min = 5f, Max = 1000f, DefaultValue = 100f, Step = 5f)]
        public float FlareLightRange = 100f;
    }
}
