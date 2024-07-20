using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TownOfUs.Patches.Roles;
using TownOfUs.Roles;
using UnityEngine;
using UnityEngine.UI;

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
                    RectTransform originalRectTransform = state.NameText.GetComponent<RectTransform>();

                    GameObject templateGO = new GameObject("CurseEmblem");
                    templateGO.AddComponent<RectTransform>();
                    var templateImg = templateGO.AddComponent<SpriteRenderer>();

                    templateImg.sprite = TownOfUs.WitchCurseEmblem;
                    GameObject curseEmblemGO = GameObject.Instantiate(templateGO, state.transform, false);
                    curseEmblemGO.layer = 5;

                    var transform = curseEmblemGO.GetComponent<RectTransform>();

                    transform.localPosition = new Vector3(1,0,0);
                    transform.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
                }
            }
        }
    }
}
