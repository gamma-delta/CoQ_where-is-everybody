using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using Qud.API;
using XRL;
using XRL.World.Conversations.Parts;

namespace at.petrak.whereiseverybody {
  // This is the one used for legendary creatures
  public static class WaterRitualBuySecretMixin {
    // https://stackoverflow.com/questions/39633917/c-getmethod-by-type-generic-list
    // > Longer version: there is no method void c.m(IEnumerable<> Ls),
    // > only overloads where the generic parameter will be some specific –
    // > existing at run time – type where the jitter has needed to create the
    // > method due to some code referencing that instantiation of the generic
    // > method.
    public static MethodInfo REVEAL_ENTRY = AccessTools.Method(
      typeof(IWaterRitualSecretPart),
      nameof(IWaterRitualSecretPart.GetShuffledSecrets)
    );

    public static MethodInfo INJECT = AccessTools.Method(
      typeof(WaterRitualBuySecretMixin), 
      nameof(WaterRitualBuySecretMixin.Injection));
    
    [HarmonyPatch(typeof(WaterRitualBuySecret))]
    [HarmonyPatch(nameof(WaterRitualBuySecret.Share))]
    public static class Patch {
      public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> vanilla) {
        if (REVEAL_ENTRY == null)
          throw new System.Exception("Mixin failed to find RevealEntry");
        if (INJECT == null)
          throw new System.Exception("Mixin failed to find Injection");

        foreach (var i in vanilla) {
          yield return i;
          // This is how we impl @At like in Mixin -- manually.
          if (i.Calls(REVEAL_ENTRY)) {
            // Out params are pass-by-reference, so the stack *types*
            // stay unchanged, (?)
            // Load the list
            yield return new CodeInstruction(OpCodes.Ldloc_1);
            // Mutate it
            yield return new CodeInstruction(OpCodes.Call, INJECT);
            // We've returned the stack to the state it was before but
            // now the list has been mutated.
            // I HOPE
            // i'm at my wit's fucking end here
          }
        }
      }
    }

    public static void Injection(List<IBaseJournalEntry> notes) {
      notes.AddRange(GetAdditionalSecrets.GetFor(The.Speaker));
    }
  }
}
