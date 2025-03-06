using FortRise;
using Microsoft.Xna.Framework;
using System;
using TowerFall;

namespace TFModFortRisePickupBlackHole
{
  public class MyPickup
  {
    internal static void Load()
    {
      On.TowerFall.Pickup.CreatePickup += CreatePickup_patch;
    }

    internal static void Unload()
    {
      On.TowerFall.Pickup.CreatePickup -= CreatePickup_patch;
    }

    public static Pickup CreatePickup_patch(
        On.TowerFall.Pickup.orig_CreatePickup orig, Vector2 position, Vector2 targetPosition, Pickups type, int playerIndex)
    {
      if (type == ModRegisters.PickupType<BlackHolePickup>())
      {
        return new BlackHolePickup(position, targetPosition, type);
      }
      return orig(position, targetPosition, type, playerIndex);
    }
  }
}