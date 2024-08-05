using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Il2CppSystem.Threading.Tasks;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.Patches.CustomOption
{
    [HarmonyPatch(typeof(GameData), nameof(GameData.CompleteTask))]
    public class Communism
    {
        public static void Postfix(PlayerControl pc)
        {
            //Debug.Log("Task Completed");
            if(CustomGameOptions.CommunismActive && !pc.Data.IsDead && pc.PlayerId == PlayerControl.LocalPlayer.PlayerId)
            {
                //Debug.Log("Trying to execute communism");
                if (pc.AllTasksCompleted())
                {
                    //Debug.Log("All my tasks are done");
                    var playersWithTasks = PlayerControl.AllPlayerControls.ToArray().ToList().Where(pc => !pc.AllTasksCompleted() && Role.GetRole(pc).Faction == Faction.Crewmates);
                    if (playersWithTasks.Count() > 0)
                    {
                        var randomPlayer = playersWithTasks.ToList()[UnityEngine.Random.RandomRange(0, playersWithTasks.Count())];
                        var acceptableTasks = randomPlayer.myTasks.ToArray().Where(t => !t.IsComplete).ToList();

                        var task = acceptableTasks[UnityEngine.Random.RandomRange(0, acceptableTasks.Count)];
                        //Debug.Log("Swapping with " + randomPlayer.Data.PlayerName);
                        Utils.Rpc(CustomRPC.SwapTasks, task.Owner.PlayerId, task.Id, pc.PlayerId);
                        CommunismUtility.SwapTasks(task.Owner.PlayerId, task.Id, pc.PlayerId, false);
                    }
                }
            }
        }
    }

    public static class CommunismUtility
    {
        public static void SwapTasks(byte ownerId, uint taskId, byte playerId, bool isRPC=true)
        {
            //Debug.Log("SwapTasks");
            if (isRPC && playerId == PlayerControl.LocalPlayer.PlayerId)
            {
                return;
            }
            var owner = Utils.PlayerById(ownerId);
            var recipient = Utils.PlayerById(playerId);
            var taskToSwap = owner.myTasks.ToArray().ToList().Find(t => t.Id == taskId);
            var previousOwnerInfo = taskToSwap.Owner.Data;
            var recipientInfo = recipient.Data;
            var taskInfo = previousOwnerInfo.FindTaskById(taskToSwap.Id);


            taskInfo.Id = (uint)recipientInfo.Tasks.Count;
            taskToSwap.Id = (uint)recipientInfo.Tasks.Count;

            recipient.myTasks.Add(taskToSwap);
            recipientInfo.Tasks.Add(taskInfo);

            taskToSwap.Owner.myTasks.Remove(taskToSwap);
            taskToSwap.Owner = recipient;
            previousOwnerInfo.Tasks.Remove(taskInfo);
        }

    }
}
