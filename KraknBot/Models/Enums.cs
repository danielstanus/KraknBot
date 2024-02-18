using System;

namespace KraknBot.Models
{
    public enum ReviveOption
    {
        Emergency,
        Standard
    }

    [Flags]
    public enum Targets
    {
        None = 0,
        Npc = 1,
        Monster = 2,
        Box = 4,
        NpcAndBox = Npc | Box,
        NpcAndMonster = Npc | Monster,
        MonsterAndBox = Monster | Box,
        MonsterAndNpc = Npc | Monster | Box,
        All = Npc | Monster | Box
    }

    public enum BotState : byte
    {
        Stopped,
        Running,
    }
}