using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TownOfUs.Roles
{
    public class LeechBenifit
    {
        public Action<Leech> Action { get; private init; }
        public Func<Leech, bool> isAllowedToRun { get; private init; }

        public LeechBenifit(Action<Leech> action, Func<Leech, bool> condition)
        {
            this.Action = action;
            this.isAllowedToRun = condition;
        }

        public static List<LeechBenifit> AllBenifits = new List<LeechBenifit>()
        {
            new LeechBenifit(r =>
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
            }),
            new LeechBenifit(r => r.HasIncreasedVision = true, r => !r.HasIncreasedVision),
            new LeechBenifit(r => r.UploadBuff = true, r => !r.UploadBuff),
        };
    }
}
