using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs
{
    public class Leech : Role
    {
        public KillButton _leechButton;
        public DeadBody ClosestBody;
        public DateTime LastLeeched;
        public bool HasIncreasedVision = false;
        public List<LeechBenifit> NonCompletedBenifits = new();
        public bool UploadBuff = false;

        public Leech(PlayerControl player) : base(player) {
            Name = "Leech";
            ImpostorText = () => "Feed off the souls of the dead.";
            TaskText = () => "Feed. Help your friends\nTasks:";
            Color = Patches.Colors.Leech;
            RoleType = RoleEnum.Leech;
            AddToRoleHistory(RoleType);
            Faction = Faction.Crewmates;

            LastLeeched = DateTime.UtcNow;
        }

        public KillButton LeechButton
        {
            get => _leechButton;
            set
            {
                _leechButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public float LeechTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastLeeched;
            var num = CustomGameOptions.CurseCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public void SoulLeech(DeadBody deadBody)
        {
            if (deadBody == null) return;
            LastLeeched = DateTime.UtcNow;

            var reminant = new GameObject("BodyReminant");

            reminant.transform.position = deadBody.transform.position;
            reminant.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            var renderer = reminant.AddComponent<SpriteRenderer>();

            renderer.sprite = TownOfUs.SoulReminantSprite;

            GameObject.Destroy(deadBody.gameObject);

            var avaliableBenifits = LeechBenifit.AllBenifits.Where(b => b.isAllowedToRun(this)).ToList();

            var benifit = avaliableBenifits[UnityEngine.Random.RandomRange(0, avaliableBenifits.Count)];

            avaliableBenifits.Remove(benifit);

            benifit.Action.Invoke(this);
        }
    }
}
