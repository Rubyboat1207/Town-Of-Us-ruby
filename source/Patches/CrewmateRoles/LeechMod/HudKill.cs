using AmongUs.GameOptions;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TownOfUs.Extensions;
using TownOfUs.Patches.Roles;
using TownOfUs.Roles;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace TownOfUs.Patches.CrewmateRoles.LeechMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDKill
    {
        private static KillButton KillButton;

        public static void Postfix(HudManager __instance)
        {
            UpdateKillButton(__instance);
        }

        private static void UpdateKillButton(HudManager __instance)
        {
            KillButton = __instance.KillButton;
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Leech))
            {
                var role = Role.GetRole<Leech>(PlayerControl.LocalPlayer);
                KillButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
                KillButton.SetCoolDown(role.LeechTimer(), 20); // todo make into LeechCd setting
                if (role.LeechButton == null)
                {
                    role.LeechButton = KillButton;
                }
                KillButton.graphic.sprite = TownOfUs.LeechButtonSprite;
                role.LeechButton.graphic.sprite = TownOfUs.LeechButtonSprite;
                KillButton.buttonLabelText.text = "";
                var data = PlayerControl.LocalPlayer.Data;
                var truePosition = PlayerControl.LocalPlayer.GetTruePosition();
                var maxDistance = GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
                var flag = (GameOptionsManager.Instance.currentNormalGameOptions.GhostsDoTasks || !data.IsDead) &&
                           (!AmongUsClient.Instance || !AmongUsClient.Instance.IsGameOver) &&
                           PlayerControl.LocalPlayer.CanMove;
                var allocs = Physics2D.OverlapCircleAll(truePosition, maxDistance,
                    LayerMask.GetMask(new[] { "Players", "Ghost" }));
                DeadBody closestBody = null;
                var closestDistance = float.MaxValue;

                foreach (var collider2D in allocs)
                {
                    if (!flag || data.IsDead || collider2D.tag != "DeadBody") continue;
                    var component = collider2D.GetComponent<DeadBody>();
                    if (!(Vector2.Distance(truePosition, component.TruePosition) <=
                          maxDistance)) continue;

                    var distance = Vector2.Distance(truePosition, component.TruePosition);
                    if (!(distance < closestDistance)) continue;
                    closestBody = component;
                    closestDistance = distance;
                }

                LeechKillButtonTarget.SetTarget(role.LeechButton, closestBody, role);
                role.LeechButton.SetCoolDown(PlayerControl.LocalPlayer.killTimer, GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown);
            }
        }
    }
}
