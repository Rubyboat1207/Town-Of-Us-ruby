using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.Patches.ImpostorRoles.WitchMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static Sprite SampleSprite => TownOfUs.SampleSprite;

        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Witch);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = Role.GetRole<Roles.Witch>(PlayerControl.LocalPlayer);
            var target = role.ClosestPlayer;
            if (__instance == role.CurseButton)
            {
                if (role.CurseTimer() != 0) return false;
                if (target == null) return false;
                role.Curse(target);
                Utils.Rpc(CustomRPC.Curse, PlayerControl.LocalPlayer.PlayerId, target.PlayerId);

                return false;
            }

            return true;
        }
    }
}
