using System;
using System.Collections.Generic;
using System.Linq;
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
        public List<LeechBenefit> CompletedBenifits = new();
        public bool UploadBuff = false;
        public bool KillDistanceDebuff = false;
        public bool SabotageQuickFix = false;

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
            var num = 20 * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public void SoulLeech(DeadBody deadBody)
        {
            Debug.Log("Soul Leech Called");
            if (deadBody == null) return;
            LastLeeched = DateTime.UtcNow;

            var reminant = new GameObject("BodyReminant");

            reminant.transform.position = deadBody.transform.position;
            reminant.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            var renderer = reminant.AddComponent<SpriteRenderer>();

            renderer.sprite = TownOfUs.SoulReminantSprite;

            GameObject.Destroy(deadBody.gameObject);

            var avaliableBenifits = LeechBenefit.AllBenifits.Where(b => (!CompletedBenifits.Contains(b)) && b.isAllowedToRun(this)).ToList();

            if(avaliableBenifits.Count == 0)
            {
                Debug.Log("Count was zero?");
                LeechBenefit.AllBenifits.ForEach(b =>
                {
                    Debug.Log(LeechBenefit.AllBenifits.IndexOf(b) + " - allowed to run: " + (b.isAllowedToRun(this) ? "true" : "false"));
                });
                return;
            }

            var benifit = avaliableBenifits[UnityEngine.Random.RandomRange(0, avaliableBenifits.Count)];

            CompletedBenifits.Add(benifit);
            Debug.Log("Benifit" + LeechBenefit.AllBenifits.IndexOf(benifit));

            Utils.Rpc(CustomRPC.LeechAbility, Player.PlayerId, (byte)LeechBenefit.AllBenifits.IndexOf(benifit));

            benifit.Action.Invoke(this);
            UpdateHUD(benifit);
        }

        //[CustomRPCFunc(CustomRPC.SoulLeech)]
        public void SoulLeechNoReward(DeadBody deadBody)
        {
            Debug.Log("Soul Leech Called");
            if (deadBody == null) return;
            LastLeeched = DateTime.UtcNow;

            var reminant = new GameObject("BodyReminant");

            reminant.transform.position = deadBody.transform.position;
            reminant.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            var renderer = reminant.AddComponent<SpriteRenderer>();

            renderer.sprite = TownOfUs.SoulReminantSprite;

            GameObject.Destroy(deadBody.gameObject);
        }

        // This one gets called only by RPC
        //[CustomRPCFunc(CustomRPC.LeechAbility)]
        public void ActivateSoulLeechReward(byte benifitIndex)
        {
            Debug.Log("REWARDED");
            var benifit = LeechBenefit.AllBenifits[benifitIndex];
            CompletedBenifits.Add(benifit);

            benifit.Action.Invoke(this);

            UpdateHUD(benifit);
        }

        public void UpdateHUD(LeechBenefit addedBenifit)
        {
            Debug.Log("Updating HUD");
            Debug.Log(addedBenifit.Sprite == null ? "NULL" : "NOT NULL");

            if (addedBenifit.Sprite == null)
            {
                return;
            }
            var hudManager = HudManager.Instance;

            GameObject leechHudStuff = GameObject.Find("LeechHUDStuff");

            if(!leechHudStuff)
            {
                leechHudStuff = new GameObject("LeechHUDStuff");

                leechHudStuff.AddComponent<RectTransform>();
                leechHudStuff.transform.SetParent(hudManager.TaskStuff.transform.GetChild(0));

                leechHudStuff.transform.localPosition = new Vector3(4.2f, -0.3f, 0);
            }

            GameObject leechBenifitEmblem = new GameObject("BenifitEmblem");
            var transform = leechBenifitEmblem.AddComponent<RectTransform>();
            var emblemRenderer = leechBenifitEmblem.AddComponent<SpriteRenderer>();

            emblemRenderer.sprite = addedBenifit.Sprite;
            leechBenifitEmblem.layer = 5;
            transform.SetParent(leechHudStuff.transform);

            transform.localPosition = Vector3.zero;
            transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            transform.localPosition += new Vector3(0.6f, 0, 0) * (CompletedBenifits.Where(b => b.Sprite != null).Count() - 1);
        }
    }
}
