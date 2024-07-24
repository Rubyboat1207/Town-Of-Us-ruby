using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TownOfUs.Patches.Roles;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.Patches.CrewmateRoles.LeechMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.SetTarget))]
    public class LeechKillButtonTarget
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Leech)) return true;
            return __instance == DestroyableSingleton<HudManager>.Instance.KillButton;
        }

        public static void SetTarget(KillButton __instance, DeadBody target, Leech role)
        {
            if (role.ClosestBody && role.ClosestBody != target)
            {
                foreach (var body in role.ClosestBody.bodyRenderers) body.material.SetFloat("_Outline", 0f);
            }

            role.ClosestBody = target;
            if (role.ClosestBody && __instance.enabled)
            {
                SpriteRenderer component = null;
                foreach (var body in role.ClosestBody.bodyRenderers) component = body;
                component.material.SetFloat("_Outline", 1f);
                component.material.SetColor("_OutlineColor", Color.yellow);
                __instance.graphic.color = Palette.EnabledColor;
                __instance.graphic.material.SetFloat("_Desat", 0f);
                return;
            }

            __instance.graphic.color = Palette.DisabledClear;
            __instance.graphic.material.SetFloat("_Desat", 1f);
        }
    }
}
