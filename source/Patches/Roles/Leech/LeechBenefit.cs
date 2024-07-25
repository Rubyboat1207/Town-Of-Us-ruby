using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class LeechBenefit
    {
        public Action<Leech> Action { get; private init; }
        public Func<Leech, bool> isAllowedToRun { get; private init; }
        public Sprite Sprite { get; private set; }

        public LeechBenefit(Action<Leech> action, Func<Leech, bool> condition, Sprite sprite)
        {
            this.Action = action;
            this.isAllowedToRun = condition;
            this.Sprite = sprite;
        }

        public static List<LeechBenefit> AllBenifits = new List<LeechBenefit>()
        {
            new LeechBenefit(r =>
            {
                var incompleteTasks = r.Player.myTasks.ToArray().Where(t => !t.IsComplete).ToList();
                int tasksAutoCompleted = 0;
                foreach (var task in incompleteTasks)
                {
                    if(tasksAutoCompleted > incompleteTasks.Count / 2)
                    {
                        break;
                    }

                    if(!task.IsComplete)
                    {
                        tasksAutoCompleted++;
                        task.Complete();
                    }
                }
                return;
            }, r =>
            {
                return r.Player.myTasks.ToArray().Where(t => !t.IsComplete).Count() > 4;
            }, null),
            new LeechBenefit(r => r.HasIncreasedVision = true, r => !r.HasIncreasedVision, TownOfUs.LeechLightSprite),
            new LeechBenefit(r => r.UploadBuff = true, r => !r.UploadBuff, TownOfUs.LeechUploadSprite),
/*            new LeechBenifit(r => r.KillDistanceDebuff = true, r => !r.KillDistanceDebuff, TownOfUs.LeechKillDistanceSprite),*/
        };
    }
}
