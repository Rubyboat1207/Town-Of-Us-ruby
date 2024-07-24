using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.Patches.CrewmateRoles.LeechMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static Sprite SampleSprite => TownOfUs.SampleSprite;

        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Leech);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = Role.GetRole<Leech>(PlayerControl.LocalPlayer);
            var target = role.ClosestBody;
            if (__instance == role.LeechButton)
            {
                if (role.LeechTimer() != 0) return false;
                if (target == null) return false;
                role.SoulLeech(target);
                Utils.Rpc(CustomRPC.SoulLeech, PlayerControl.LocalPlayer.PlayerId, target.ParentId);

                return false;
            }

            return true;
        }
    }
}
