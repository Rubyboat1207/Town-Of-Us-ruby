using HarmonyLib;
using Reactor.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TownOfUs.Patches.NeutralRoles;
using TownOfUs.Patches.Roles;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.Patches.ImpostorRoles.WitchMod
{
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.Begin))]
    internal class MeetingExiledEnd
    {
        private static void Postfix(ExileController __instance)
        {

            if (__instance.exiled != null && Utils.GetRole(__instance.exiled.Object) == RoleEnum.Witch)
                return;
            PlayerControl witch = PlayerControl.AllPlayerControls.ToArray().Where((player) => Utils.GetRole(player) == RoleEnum.Witch).First();

            if (witch == null || witch.Data.IsDead)
                return;
            Witch witchRole = Role.GetRole<Witch>(witch);

            foreach (PlayerControl control in witchRole.CursedPlayers)
            {
                //Utils.RpcMurderPlayer(witch, control);
                control.Die(DeathReason.Exile, false);
            }

            witchRole.CursedPlayers.Clear();

        }
    }
}
