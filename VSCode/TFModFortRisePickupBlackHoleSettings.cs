using FortRise;

namespace TFModFortRiseAiExample
{
  public class TFModFortRisePickupBlackHoleSettings : ModuleSettings
  {
    [SettingsName("Pickup Activated")]
    public bool activated = false;

    public const int OncePerMatch = 0;
    public const int OncePerRound = 1;
    public const int Test = 2;
    [SettingsOptions("OncePerMatch", "OncePerRound", "Test")]
    public int periodicity = 0;
  }
}
