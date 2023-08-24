using XRL;
using XRL.Wish;
using XRL.World.WorldBuilders;

namespace at.petrak.whereiseverybody {
  [HasWishCommand]
  public static class Wishes {
    private static WorldInfo worldInfo => (WorldInfo) The.Game.GetObjectGameState("JoppaWorldInfo");

    [WishCommand(Command = "pk_villages")]
    public static void PrintVillages() {
      UnityEngine.Debug.Log("all the villages:");
      foreach(var v in worldInfo.villages) {
        UnityEngine.Debug.Log("  - `" + v.name + "`");
      }
    }

    [WishCommand(Command = "pk_lairs")]
    public static void PrintLairs() {
      UnityEngine.Debug.Log("all the lairs:");
      foreach (var lair in worldInfo.lairs) {
          UnityEngine.Debug.Log("  - name: " + lair.name + ", owner id: " + lair.ownerID + ", secret id: " + lair.secretID);
      }
    }
  }
}
