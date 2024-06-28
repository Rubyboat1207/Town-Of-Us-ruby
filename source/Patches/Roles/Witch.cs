using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TownOfUs.Roles;

namespace TownOfUs.Patches.Roles
{
    public class Witch : Role
    {
        public KillButton _curseButton;
        public List<PlayerControl> CursedPlayers;
        public PlayerControl ClosestPlayer;
        public DateTime LastCursed;

        public Witch(PlayerControl player) : base(player)
        {
            Name = "Witch";
            ImpostorText = () => "Curse Crewmates";
            TaskText = () => "Curse your enemies!\nFake Tasks:";
            Color = Patches.Colors.Impostor;
            RoleType = RoleEnum.Witch;
            AddToRoleHistory(RoleType);
            Faction = Faction.Impostors;
            LastCursed = DateTime.UtcNow;
        }

        public KillButton CurseButton
        {
            get => _curseButton;
            set
            {
                _curseButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public float CurseTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastCursed;
            var num = CustomGameOptions.CurseCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public void Curse(PlayerControl playerControl)
        {
            CursedPlayers.Add(playerControl);
            LastCursed = DateTime.UtcNow;
        }
    }
}
