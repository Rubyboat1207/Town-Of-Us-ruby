using System.Reflection;
using System;
using System.Linq;
using System.Collections.Generic;
using Hazel;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs
{
    public enum CustomRPC
    {
        SetRole = 100,
        SetModifier,
        SetCouple,
        SetAssassin,
        SetTarget,
        SetGATarget,

        SetPhantom,
        CatchPhantom,

        SetHaunter,
        CatchHaunter,

        SetTraitor,
        TraitorSpawn,

        LoveWin,
        GlitchWin,
        JuggernautWin,
        ArsonistWin,
        PhantomWin,
        NobodyWins,
        PlaguebearerWin,
        PestilenceWin,
        WerewolfWin,
        SurvivorOnlyWin,
        VampireWin,

        JanitorClean,
        SoulLeech,
        LeechAbility,
        FixLights,
        EngineerFix,
        SetSwaps,
        Protect,
        AttemptSound,
        Morph,
        Mine,
        Swoop,
        Curse,
        AltruistRevive,
        BarryButton,
        Drag,
        Drop,
        AssassinKill,
        VigilanteKill,
        DoomsayerKill,
        FlashGrenade,
        Alert,
        Remember,
        BaitReport,
        Transport,
        SetUntransportable,
        Mediate,
        Vest,
        GAProtect,
        Blackmail,
        Infect,
        TurnPestilence,
        Disperse,
        Escape,
        Revive,
        Convert,
        ChameleonSwoop,
        Imitate,
        StartImitate,
        Bite,
        Reveal,
        Prosecute,
        Confess,
        Bless,
        Camouflage,
        SnitchCultistReveal,
        HunterStalk,
        HunterCatchPlayer,
        Retribution,

        BypassKill,
        BypassMultiKill,
        SetMimic,
        RpcResetAnim,
        SetHacked,

        ExecutionerToJester,
        GAToSurv,

        Start,
        SyncCustomSettings,
        FixAnimation,
        SetPos,
        SetSettings,
        
        RemoveAllBodies,
        CheckMurder,

        SubmergedFixOxygen,

        ContentAddition
    }

    public class RPCFunction
    {
        public MethodInfo Method { get; }
        public Type[] ParameterTypes { get; }
        public string[] ParameterNames { get; }

        public RPCFunction(MethodInfo method)
        {
            Method = method;
            var parameters = method.GetParameters();
            ParameterTypes = parameters.Select(p => p.ParameterType).ToArray();
            ParameterNames = parameters.Select(p => p.Name).ToArray();
        }

        public void Execute(MessageReader reader)
        {
            object executingObject = null;
            byte firstId = reader.ReadByte();
            bool usedPlayerAsFirstId = false;
            if (Method.DeclaringType.IsInstanceOfType(typeof(Role)))
            {
                usedPlayerAsFirstId = true;
                executingObject = Role.GetRole(Utils.PlayerById(firstId));
            }

            List<object> args = new List<object>();

            for (int i = 0; i < ParameterTypes.Length; i++)
            {
                var name = ParameterNames[i];
                var type = ParameterTypes[i];

                if (name == "__playerInstance" && type == typeof(PlayerControl))
                {
                    if(usedPlayerAsFirstId)
                    {
                        args.Add(Utils.PlayerById(firstId));
                    }
                    else
                    {
                        args.Add(Utils.PlayerById(reader.ReadByte()));
                    }
                    
                }
                if (type == typeof(DeadBody))
                {
                    var deadBodyId = reader.ReadByte();
                    var deadBodies2 = UnityEngine.Object.FindObjectsOfType<DeadBody>();
                    bool added = false;
                    foreach (var body in deadBodies2)
                    {
                        if (body.ParentId == deadBodyId)
                        {
                            args.Add(body);
                            added = true;
                        }
                    }
                    if(!added)
                    {
                        args.Add(null);
                    }       
                }
                else if(type == typeof(byte))
                {
                    args.Add(reader.ReadByte());
                }
                else if (type == typeof(bool))
                {
                    args.Add(reader.ReadBoolean());
                }
                else if (type == typeof(float))
                {
                    args.Add(reader.ReadSingle());
                }
                else if (type == typeof(string))
                {
                    args.Add(reader.ReadString());
                }
            }

            Method.Invoke(executingObject, args.ToArray());
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class CustomRPCFuncAttribute : Attribute
    {
        public CustomRPC RPCType { get; }

        public CustomRPCFuncAttribute(CustomRPC rpcType)
        {
            RPCType = rpcType;
        }
    }

    public static class RPCRegistry
    {
        public static Dictionary<CustomRPC, RPCFunction> RPCFunctions { get; private set; }

        static RPCRegistry()
        {
            InitializeRPCFunctions();
        }

        public static void ExecuteIfExists(CustomRPC RPCID, MessageReader reader)
        {
            if(RPCFunctions.ContainsKey(RPCID))
            {
                RPCFunctions[RPCID].Execute(reader);
            }
        }

        private static void InitializeRPCFunctions()
        {
            RPCFunctions = new Dictionary<CustomRPC, RPCFunction>();

            // Find all methods with the CustomRPCFunc attribute
            var methods = Assembly.GetExecutingAssembly().GetTypes()
                .SelectMany(t => t.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                .Where(m => m.GetCustomAttributes(typeof(CustomRPCFuncAttribute), false).Length > 0);

            foreach (var method in methods)
            {
                var attribute = (CustomRPCFuncAttribute)method.GetCustomAttribute(typeof(CustomRPCFuncAttribute));
                var rpcFunction = new RPCFunction(method);
                RPCFunctions[attribute.RPCType] = rpcFunction;
            }
        }
    }
}
