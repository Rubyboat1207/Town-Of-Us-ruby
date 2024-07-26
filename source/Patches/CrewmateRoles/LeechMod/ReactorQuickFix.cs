using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmongUs.GameOptions;
using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

namespace TownOfUs.Patches.CrewmateRoles.LeechMod
{
    [HarmonyPatch(typeof(ReactorMinigame), nameof(ReactorMinigame.FixedUpdate))]
    public class ReactorQuickFix
    {
        [HarmonyPostfix]
        public static void Postfix(ReactorMinigame __instance)
        {
            if(__instance.isButtonDown)
            {
                PlayerControl leech = PlayerControl.AllPlayerControls.ToArray().Where((player) => Utils.GetRole(player) == RoleEnum.Leech).FirstOrDefault();
                if (leech == null) return;

                Leech role = Role.GetRole<Leech>(leech);
                if (role.SabotageQuickFix)
                {
                    ShipStatus.Instance.RpcUpdateSystem(SystemTypes.Reactor, 16);
                }
            }
        }
    }
}
