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
        foreach (var relation in rep.relatedFactions) {
          if (getSecretOfVillageLocation(relation.faction) is JournalMapNote note) {
            res.Add(note);
          }
        }
      }
      
      var village = HistoryAPI.GetVillageSnapshot(faction);
      if (village != null) {
        // then they are primarily part of a village
        villageLegendaryCreatures(village, res);
        villageSultan(village, res);
        villageOtherFactions(village, res);
      }

      UnityEngine.Debug.Log("Returning:");
      foreach (var secret in res) {
        UnityEngine.Debug.Log("  - " + secret.GetShortText());
      }
      return res;
    }

    //nullable
    private static JournalMapNote getSecretOfVillageLocation(string factionName) {
      // exasperated sigh
      if (!factionName.StartsWith("villagers of ")) return null;
      var villageName = factionName.Substring("villagers of ".Length);

      var villageObj = worldInfo.villages.Find(v => v.name == villageName);
      if (villageObj == null)
        return null;

      var note = JournalAPI.GetMapNote(villageObj.secretID);
      if (note == null || note.revealed) return null;
      return note;
    }

    // Get the locations of creatures the village reveres or hates
    private static void villageLegendaryCreatures(HistoricEntitySnapshot village, List<IBaseJournalEntry> mutOut) {
      var polarizingCreatures = new List<string>();
      if (village.properties.TryGetValue("worships_creature_id", out string worship)) {
        polarizingCreatures.Add(worship);
      }
      if (village.properties.TryGetValue("despises_creature_id", out string despise)) {
        polarizingCreatures.Add(despise);
      }

      if (polarizingCreatures.Count > 0) {
        foreach (var lair in worldInfo.lairs) {
          if (polarizingCreatures.Contains(lair.ownerID)) {
            var note = JournalAPI.GetMapNote(lair.secretID);
            if (note != null && !note.revealed) {
              mutOut.Add(note);
            }
          }
        }
      }      
    }

    // Get secrets of sultans the village loves or hates
    private static void villageSultan(HistoricEntitySnapshot village, List<IBaseJournalEntry> mutOut) {
      // Add secrets about sultans they love
      var polarizingSultans = new List<(string, bool)>();
      if (village.properties.TryGetValue("worships_sultan_id", out string worship)) {
        polarizingSultans.Add((worship, true));
      }
      if (village.properties.TryGetValue("despises_sultan_id", out string despise)) {
        polarizingSultans.Add((despise, false));
      }

      foreach (var (sultan, wantGood) in polarizingSultans) {
        var notes = JournalAPI.GetNotesForSultan(sultan);
        foreach (var note in notes) {
          var evt = The.Game.sultanHistory.GetEvent(note.eventId);
          var kind = evt.getEventProperty("tombInscriptionCategory");
          var isGood = isSultanEventGood(kind);
          if (!note.revealed) {
            if (isGood == null || isGood == wantGood) {
              mutOut.Add(note);
            }
          }
        }
      }
    }

    // null = good and bad
    private static bool? isSultanEventGood(string kind) {
      switch(kind) {
        case "Dies":
        case "BodyExperienceBad":
        case "CommitsFolly":
        case "EnduresHardship":
        case "DoesBureaucracy":
        case "DoesSomethingDestructive":
          return false;

        case "IsBorn":
        case "DoesSomethingRad": // real code written by real developers
        case "WieldsItemInBattle":
        case "Trysts":
        case "CreatesSomething":
        case "LearnsSecret":
        case "Treats":
        case "HasInspiringExperience":
        case "Slays":
        case "CrownedSultan":
        case "MeetsWithCounselors":
          return true;

        case "DoesSomethingHumble":
        // This is the type both for freeing AND sacking a city, which I can't distinguish.
        // waiting on the next FF
        case "Resists":
          return null;
        
        default: // give them the benefit of the doubt if i missed something
          UnityEngine.Debug.Log("whoops! i forgot to write if `" + kind + "` was a good or bad thing");
          return true;

      }
    }
    
    private static void villageOtherFactions(HistoricEntitySnapshot village, List<IBaseJournalEntry> mutOut) {
      var polarizingFactions = new List<string>();
      if (village.properties.TryGetValue("worships_faction", out string worship)) {
        polarizingFactions.Add(worship);
      }
      if (village.properties.TryGetValue("despises_faction", out string despise)) {
        polarizingFactions.Add(despise);
      }

      if (polarizingFactions.Count > 0) {
        UnityEngine.Debug.Log("> have village feelings about faction : " + string.Join(", ", polarizingFactions));
        foreach (var faction in polarizingFactions) {
          if (getSecretOfVillageLocation(faction) is JournalMapNote note) {
            mutOut.Add(note);
          }
        }
      }      
    }
  }
}
