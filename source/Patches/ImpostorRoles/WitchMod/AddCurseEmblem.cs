using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TownOfUs.Patches.Roles;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.Patches.ImpostorRoles.WitchMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class AddCurseEmblem
    {
        public static void Postfix(MeetingHud __instance)
        {
            PlayerControl witch = PlayerControl.AllPlayerControls.ToArray().Where((player) => Utils.GetRole(player) == RoleEnum.Witch).First();

            if (witch == null || witch.Data.IsDead)
                return;
            Witch witchRole = Role.GetRole<Witch>(witch);

            var cursedIds = witchRole.CursedPlayers.ToArray().Select(p => p.PlayerId);

            foreach (var state in __instance.playerStates)
            {
                if(cursedIds.Contains(state.TargetPlayerId))
                {
                    var cursedText = GameObject.Instantiate(state.NameText, state.NameText.transform);
                    cursedText.text = "CURSED";
                }
            }
        }
    }
}
