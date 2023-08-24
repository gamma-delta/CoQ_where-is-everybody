using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using Qud.API;
using XRL;
using XRL.World;

namespace at.petrak.whereiseverybody {
  // This is used for mayors
  public static class WaterRitualNodeMixin {
    public static ConstructorInfo NEW_CHOICES = AccessTools.Constructor(typeof(List<IBaseJournalEntry>));
public static MethodInfo INJECT = AccessTools.Method(typeof(WaterRitualNodeMixin), nameof(WaterRitualNodeMixin.Injection));

    [HarmonyPatch(typeof(WaterRitualNode))]
    [HarmonyPatch(nameof(WaterRitualNode.CreateBuySecret))]
    public static class Patch {
      public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> insns) {
        if (NEW_CHOICES == null)
          throw new System.Exception("Mixin failed to find List ctor");
        if (INJECT == null)
          throw new System.Exception(" Mixin failed to find Injection");

        bool found = false;
        foreach (var insn in insns) {
          yield return insn;
          if (insn.Is(OpCodes.Newobj, NEW_CHOICES) && !found) {
            found = true;
            // on top of the stack will be the choices
            yield return new CodeInstruction(OpCodes.Call, INJECT);
          }
        }
      }
    }

    public static List<IBaseJournalEntry> Injection(List<IBaseJournalEntry> notes) {
        notes.AddRange(GetAdditionalSecrets.GetFor(The.Speaker));
        return notes;
    }
  }
}
