using System;
using Server;

namespace Server.Mobiles
{
    public enum ClockworkCreatureType
    {
        ExodusOverseer = 1075914,
        Betrayer = 1029734,
        ClockworkScorpion = 1112861,
        Vollem = 1112860,
        Juggernaut = 1029746,
        ExodusMinion = 1075915,
        LeatherWolf = 1112859
    }

    public class ClockworkCreatureDef
    {
        private ClockworkCreatureType m_CreatureType;
        private string m_Name;
        private int m_BodyId;

        public ClockworkCreatureType CreatureType { get { return m_CreatureType; } }
        public string Name { get { return m_Name; } }
        public int BodyId { get { return m_BodyId; } }

        public ClockworkCreatureDef(ClockworkCreatureType type, string name, int bodyId)
        {
            m_CreatureType = type;
            m_Name = name;
            m_BodyId = bodyId;
        }
    }

    public class ClockworkCreature : Mobile
    {
        public static ClockworkCreatureDef[] Definitions { get { return m_Definitions; } }

        private static readonly ClockworkCreatureDef[] m_Definitions = new ClockworkCreatureDef[]
        {
            new ClockworkCreatureDef( ClockworkCreatureType.ExodusOverseer,     "an exodus overseer",   0x2F4 ),
            new ClockworkCreatureDef( ClockworkCreatureType.Betrayer,           "a betrayer",           0x2FF ),
            new ClockworkCreatureDef( ClockworkCreatureType.ClockworkScorpion,  "a clockwork scorpion", 0x2CD ),
            new ClockworkCreatureDef( ClockworkCreatureType.Vollem,             "a vollem",             0x125 ),
            new ClockworkCreatureDef( ClockworkCreatureType.Juggernaut,         "a juggernaut",         0x300 ),
            new ClockworkCreatureDef( ClockworkCreatureType.ExodusMinion,       "an exodus minion",     0x2F5 ),
            new ClockworkCreatureDef( ClockworkCreatureType.LeatherWolf,        "a leather wolf",       0x2E3 ),
        };

        [Constructable]
        public ClockworkCreature(ClockworkCreatureDef def)
        {
            Name = def.Name;
            Body = def.BodyId;

            Hits = HitsMax;

            Blessed = true;
            Frozen = true;
        }

        public ClockworkCreature(Serial serial)
            : base(serial)
        {
        }

        public override bool CanBeDamaged()
        {
            return false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }
}