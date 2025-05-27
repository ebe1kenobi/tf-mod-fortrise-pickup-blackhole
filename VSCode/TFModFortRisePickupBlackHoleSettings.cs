using FortRise;

namespace TFModFortRisePickupBlackHole
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
    
    [SettingsName("Random teleportation")]
    public bool random = false;

    [SettingsName("attraction radius")]
    [SettingsNumber(0, 320)]
    public int attractionRadius = 60;

    [SettingsName("attraction force")]
    [SettingsNumber(0, 10)]
    public int attractionForce = 3;

    [SettingsName("teleport radius")]
    [SettingsNumber(5, 20)]
    public int teleportRadius = 15;


  }
}
