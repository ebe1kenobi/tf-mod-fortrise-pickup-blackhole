using System;
using FortRise;
using TFModFortRiseAiExample;
using TowerFall;
using Monocle;

namespace TFModFortRisePickupBlackHole
{
  [Fort("com.ebe1.kenobi.TFModBlackHole", "TFModBlackHole")]
  public class TFModFortRisePickupBlackHoleModule : FortModule
  {
    public static TFModFortRisePickupBlackHoleModule Instance;
    public Atlas Atlas;
    public override Type SettingsType => typeof(TFModFortRisePickupBlackHoleSettings);
    public static TFModFortRisePickupBlackHoleSettings Settings => (TFModFortRisePickupBlackHoleSettings)Instance.InternalSettings;
    public TFModFortRisePickupBlackHoleModule()
    {
      Instance = this;
      //Logger.Init("TFModFortRisePickupBlackHoleLOG");
    }

    public override void Load()
    {
      MyPickup.Load();
      MySession.Load();
      MyTreasureSpawner.Load();
    }

    public override void Unload()
    {
      MyPickup.Unload();
      MySession.Unload();
      MyTreasureSpawner.Unload();
      Instance = null;
    }

    public override void LoadContent()
    {
      Atlas = Content.LoadAtlas("Atlas/atlas.xml", "Atlas/atlas.png");
    }

    public static bool activated()
    {
      return VariantManager.GetCustomVariant("BlackHole") || TFModFortRisePickupBlackHoleModule.Settings.activated;
    }

    public override void OnVariantsRegister(VariantManager manager, bool noPerPlayer = false)
    {
      var icon = new CustomVariantInfo(
          "BlackHole", VariantManager.GetVariantIconFromName("BlackHole", Atlas),
          CustomVariantFlags.None
          );
      manager.AddVariant(icon);
    }
  }
}
