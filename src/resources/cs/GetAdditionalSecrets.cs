using System.Collections.Generic;

using Qud.API;
using XRL;
using XRL.World;
using XRL.World.Parts;
using XRL.World.WorldBuilders;
using HistoryKit;

namespace at.petrak.whereiseverybody {
  public static class GetAdditionalSecrets {
    private static WorldInfo worldInfo => (WorldInfo) The.Game.GetObjectGameState("JoppaWorldInfo");

    public static List<IBaseJournalEntry> GetFor(GameObject speaker) {
      var res = new List<IBaseJournalEntry>();
      var faction = speaker.pBrain.GetPrimaryFaction();

      if (speaker.GetPart<GivesRep>() is GivesRep rep) {
        UnityEngine.Debug.Log("I'm " + faction + " and I have these relations:");
        foreach (var relation in rep.relatedFactions) {
          UnityEngine.Debug.Log("  - " + relation.faction);
          if (getSecretOfVillageLocation(relation.faction) is JournalMapNote note) {
            res.Add(note);
          }
        }
      }
      
      var village = HistoryAPI.GetVillageSnapshot(faction);
      if (village != null) {
        // then they are primarily part of a village
        UnityEngine.Debug.Log("live in a village");
        var polarizingCreatures = new List<string>();
        if (village.properties.TryGetValue("worships_creature_id", out string worship)) {
          polarizingCreatures.Add(worship);
        }
        if (village.properties.TryGetValue("despises_creature_id", out string despise)) {
          polarizingCreatures.Add(despise);
        }

        if (polarizingCreatures.Count > 0) {
          UnityEngine.Debug.Log("> have village feelings about : " + string.Join(", ", polarizingCreatures));
          foreach (var lair in worldInfo.lairs) {
            if (polarizingCreatures.Contains(lair.ownerID)) {
              var note = JournalAPI.GetMapNote(lair.secretID);
              if (note != null && !note.revealed) {
                res.Add(note);
              }
            }
          }
        }
      }

      UnityEngine.Debug.Log("Returning:");
      foreach (var secret in res) {
        UnityEngine.Debug.Log("  - " + secret.GetDisplayText());
      }
      return res;
    }

    //nullable
    private static JournalMapNote getSecretOfVillageLocation(string factionName) {
      // exasperated sigh
      if (!factionName.StartsWith("villagers of ")) return null;
      var villageName = factionName.Substring("villagers of ".Length);
      UnityEngine.Debug.Log("attempting to find `" + villageName + "`");

      var villageObj = worldInfo.villages.Find(v => v.name == villageName);
      if (villageObj == null)
        return null;

      UnityEngine.Debug.Log("found village object " + villageObj.secretID);
      var note = JournalAPI.GetMapNote(villageObj.secretID);
      UnityEngine.Debug.Log("found note object " + note?.GetDisplayText());
      if (note == null || note.revealed) return null;
      return note;
    }
  }
}
