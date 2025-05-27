using FortRise;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using TowerFall;
using MonoMod.Utils;

namespace TFModFortRisePickupBlackHole
{
  public class MyTreasureSpawner
  {
    internal static void Load()
    {
      On.TowerFall.TreasureSpawner.GetChestSpawnsForLevel += GetChestSpawnsForLevel_patch;
    }

    internal static void Unload()
    {
      On.TowerFall.TreasureSpawner.GetChestSpawnsForLevel -= GetChestSpawnsForLevel_patch;
    }

    public static List<TreasureChest> GetChestSpawnsForLevel_patch(
       On.TowerFall.TreasureSpawner.orig_GetChestSpawnsForLevel orig,
       TowerFall.TreasureSpawner self,
       List<Vector2> chestPositions,
       List<Vector2> bigChestPositions)
    { 
      List<TreasureChest> chestSpawnsForLevel = orig(self, chestPositions, bigChestPositions);
      if (chestSpawnsForLevel.Count == 0)
      {
        return chestSpawnsForLevel;
      }

      if (!TFModFortRisePickupBlackHoleModule.activated()) return chestSpawnsForLevel;

      if (MySession.NbBlackHolePickupActivated == 0)
      {
        Random rnd = new Random();
        int draw;
        if (TFModFortRisePickupBlackHoleModule.Settings.periodicity == TFModFortRisePickupBlackHoleSettings.Test)
        {
          draw = 1;
        }
        else
        {
          draw = rnd.Next(0, 10);
        }
        if (draw == 1)
        {
          var dynData = DynamicData.For(chestSpawnsForLevel[0]);
          List<Pickups> pickups = (List<Pickups>)dynData.Get("pickups");
          pickups[0] = ModRegisters.PickupType<BlackHolePickup>();
          MySession.NbBlackHolePickupActivated++;
          dynData.Dispose();
        }
      }

      return chestSpawnsForLevel;
    }
  }
}