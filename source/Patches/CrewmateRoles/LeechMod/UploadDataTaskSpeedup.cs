using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TownOfUs.Roles;

namespace TownOfUs.Patches.CrewmateRoles.LeechMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class UploadDataTaskSpeedup
    {
        private static void Postfix(HudManager __instance)
        {
            PlayerControl leech = PlayerControl.AllPlayerControls.ToArray().Where((player) => Utils.GetRole(player) == RoleEnum.Witch).FirstOrDefault();
            if (leech == null) return;

            Leech role = Role.GetRole<Leech>(leech);
            if (role.UploadBuff)
                __instance.transform.parent.GetComponentInChildren<UploadDataGame>().timer += 1 * Time.deltaTime;
        }
    }
}
