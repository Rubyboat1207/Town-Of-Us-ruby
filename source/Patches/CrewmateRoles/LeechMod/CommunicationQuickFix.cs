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
    [HarmonyPatch(typeof(TuneRadioMinigame), nameof(TuneRadioMinigame.Update))]
    public class CommunicationQuickFix
    {
        [HarmonyPostfix]
        public static void Postfix(TuneRadioMinigame __instance)
        {
            if (__instance.Tolerance == 0.4f)
            {
                return;
            }
            PlayerControl leech = PlayerControl.AllPlayerControls.ToArray().Where((player) => Utils.GetRole(player) == RoleEnum.Leech).FirstOrDefault();
            if (leech == null) return;

            Leech role = Role.GetRole<Leech>(leech);
            if (role.SabotageQuickFix)
            {
                __instance.Tolerance = 0.4f;
            }
        }
    }

    [HarmonyPatch(typeof(AuthGame), nameof(AuthGame.Enter))]
    public class CommunicationQuickFix2
    {
        [HarmonyPostfix]
        public static void Postfix(AuthGame __instance)
        {
            PlayerControl leech = PlayerControl.AllPlayerControls.ToArray().Where((player) => Utils.GetRole(player) == RoleEnum.Leech).FirstOrDefault();
            if (leech == null) return;

            Leech role = Role.GetRole<Leech>(leech);
            if(role.SabotageQuickFix)
            {
                if(__instance.amClosing == Minigame.CloseState.Closing)
                {
                    ShipStatus.Instance.RpcUpdateSystem(SystemTypes.Comms, 16);
                }
            }
        }
    }
}
