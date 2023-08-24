using XRL;
using XRL.Wish;
using XRL.World.WorldBuilders;
using Qud.API;

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

    [WishCommand(Command = "pk_sultans")]
    public static void PrintSultans() {
      UnityEngine.Debug.Log("all the sultans:");
      foreach (var sultan in HistoryAPI.GetSultans()) {
        UnityEngine.Debug.Log("  - " + sultan.id);
      }
    }
    
    [WishCommand(Command = "pk_villageprops")]
    public static void PrintVillageProps(string rest) {
      UnityEngine.Debug.Log("properties for " + rest);
      var village = HistoryAPI.GetVillageSnapshot(rest);
      if (village == null) {
        UnityEngine.Debug.Log("no village found");
      } else {
        foreach (var pair in village.properties) {
          UnityEngine.Debug.Log("  - " + pair.Key + " : " + pair.Value);
        }
      }
    }
  }
}
