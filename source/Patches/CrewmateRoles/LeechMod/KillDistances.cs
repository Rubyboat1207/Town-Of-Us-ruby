/*using AmongUs.GameOptions;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TownOfUs.Roles;

namespace TownOfUs.Patches.CrewmateRoles.LeechMod
{
    [HarmonyPatch(typeof(GameOptionsData), nameof(GameOptionsData.KillDistances))]
    internal class KillDistances
    {
        [HarmonyPostfix]
        public static void Postfix(ref float __result)
        {
            PlayerControl leech = PlayerControl.AllPlayerControls.ToArray().Where((player) => Utils.GetRole(player) == RoleEnum.Leech).FirstOrDefault();
            if (leech == null) return;

            Leech role = Role.GetRole<Leech>(leech);

            if(role.KillDistanceDebuff)
            {
                __result *= 0.75f;
            }
        }
    }
}
*/