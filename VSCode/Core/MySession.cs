using TFModFortRiseAiExample;

namespace TFModFortRisePickupBlackHole
{
  public class MySession
  {
    public static int NbBlackHolePickupActivated { get; set; }

    internal static void Load()
    {
      On.TowerFall.Session.StartGame += StartGame_patch;
      On.TowerFall.Session.GotoNextRound += GotoNextRound_patch;
    }

    internal static void Unload()
    {
      On.TowerFall.Session.StartGame -= StartGame_patch;
      On.TowerFall.Session.GotoNextRound -= GotoNextRound_patch;
    }
    public MySession()
    {
    }

    public static void StartGame_patch(On.TowerFall.Session.orig_StartGame orig, global::TowerFall.Session self)
    {
      if (TFModFortRisePickupBlackHoleModule.Settings.periodicity == TFModFortRisePickupBlackHoleSettings.OncePerMatch)
      {
        NbBlackHolePickupActivated = 0;
      }
      orig(self);
    }

    public static void GotoNextRound_patch(On.TowerFall.Session.orig_GotoNextRound orig, global::TowerFall.Session self)
    {
      if (TFModFortRisePickupBlackHoleModule.Settings.periodicity == TFModFortRisePickupBlackHoleSettings.OncePerRound)
      {
        NbBlackHolePickupActivated = 0;
      }
      if (TFModFortRisePickupBlackHoleModule.Settings.periodicity == TFModFortRisePickupBlackHoleSettings.Test)
      {
        NbBlackHolePickupActivated = 0;
      }
      orig(self);
    }
  }
}
