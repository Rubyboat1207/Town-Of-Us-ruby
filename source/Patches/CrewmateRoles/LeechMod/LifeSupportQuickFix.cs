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
    [HarmonyPatch(typeof(EnterCodeMinigame), nameof(EnterCodeMinigame.AcceptDigits))]
    public class LifeSupportQuickFix
    {
        [HarmonyPostfix]
        public static void Postfix(EnterCodeMinigame __instance)
        {
            var oxygen = ShipStatus.Instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();
            if (oxygen.IsActive)
            {
                PlayerControl leech = PlayerControl.AllPlayerControls.ToArray().Where((player) => Utils.GetRole(player) == RoleEnum.Leech).FirstOrDefault();
                if (leech == null) return;

                Leech role = Role.GetRole<Leech>(leech);
                if (role.SabotageQuickFix)
                {
                    ShipStatus.Instance.RpcUpdateSystem(SystemTypes.LifeSupp, 16);
                }
            }
        }
    }

    [HarmonyPatch(typeof(KeypadGame), nameof(KeypadGame.Enter))]
    public class LifeSupportQuickFix2
    {
        [HarmonyPostfix]
        public static void Postfix(KeypadGame __instance)
        {
            var oxygen = ShipStatus.Instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();
            if (oxygen.IsActive)
            {
                PlayerControl leech = PlayerControl.AllPlayerControls.ToArray().Where((player) => Utils.GetRole(player) == RoleEnum.Leech).FirstOrDefault();
                if (leech == null) return;

                Leech role = Role.GetRole<Leech>(leech);
                if (role.SabotageQuickFix)
                {
                    ShipStatus.Instance.RpcUpdateSystem(SystemTypes.LifeSupp, 16);
                }
            }
        }
    }
}
