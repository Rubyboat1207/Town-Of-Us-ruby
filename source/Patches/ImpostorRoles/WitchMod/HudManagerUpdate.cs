using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TownOfUs.Patches.Roles;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.Patches.ImpostorRoles.WitchMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static Sprite CurseSprite => TownOfUs.CurseSprite;

        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Witch)) return;
            var role = Role.GetRole<Witch>(PlayerControl.LocalPlayer);
            if (role.CurseButton == null)
            {
                role.CurseButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.CurseButton.graphic.enabled = true;
                role.CurseButton.gameObject.SetActive(false);
            }

            role.CurseButton.graphic.sprite = CurseSprite;
            role.CurseButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            role.CurseButton.SetCoolDown(role.CurseTimer(), CustomGameOptions.CurseCd);

            Utils.SetTarget(ref role.ClosestPlayer, role._curseButton);

        }
    }

}
