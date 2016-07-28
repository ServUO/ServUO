using Server;
using System;

namespace Server.Mobiles
{
    [CorpseName("a mongbat corpse")]
    public class DaemonMongbat : CovetousCreature
    {
        [Constructable]
        public DaemonMongbat()
            : base(AIType.AI_Melee)
        {
            Name = "a dameon mongbat";
            Body = 39;
            BaseSoundID = 422;
        }

        [Constructable]
        public DaemonMongbat(int level, bool voidSpawn)
            : base(AIType.AI_Melee, level, voidSpawn)
        {
            Name = "a dameon mongbat";
            Body = 39;
            BaseSoundID = 422;
        }

        public DaemonMongbat(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    [CorpseName("a gargoyle corpse")]
    public class GargoyleAssassin : CovetousCreature
    {
        [Constructable]
        public GargoyleAssassin()
            : base(AIType.AI_Mage)
        {
            Name = "a gargoyle assassin";
            Body = 0x2F1;
            BaseSoundID = 0x174;
        }

        [Constructable]
        public GargoyleAssassin(int level, bool voidSpawn)
            : base(AIType.AI_Mage, level, voidSpawn)
        {
            Name = "a gargoyle assassin";
            Body = 0x2F1;
            BaseSoundID = 0x174;
        }

        public GargoyleAssassin(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    [CorpseName("a doppleganger corpse")]
    public class CovetousDoppleganger : CovetousCreature
    {
        [Constructable]
        public CovetousDoppleganger()
            : base(AIType.AI_Melee)
        {
            Name = "a doppleganger";
            Body = 0x309;
            BaseSoundID = 0x451;
        }

        [Constructable]
        public CovetousDoppleganger(int level, bool voidSpawn)
            : base(AIType.AI_Melee, level, voidSpawn)
        {
            Name = "a doppleganger";
            Body = 0x309;
            BaseSoundID = 0x451;
        }

        public CovetousDoppleganger(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    [CorpseName("an oni corpse")]
    public class LesserOni : CovetousCreature
    {
        [Constructable]
        public LesserOni()
            : base(AIType.AI_Mage)
        {
            Name = "a lesser oni";
            Body = 241;
        }

        [Constructable]
        public LesserOni(int level, bool voidSpawn)
            : base(AIType.AI_Mage, level, voidSpawn)
        {
            Name = "a lesser oni";
            Body = 241;
        }

        public override int GetAngerSound() { return 0x4E3; }
        public override int GetIdleSound() { return 0x4E2; }
        public override int GetAttackSound() { return 0x4E1; }
        public override int GetHurtSound() { return 0x4E4; }
        public override int GetDeathSound() { return 0x4E0; }

        public LesserOni(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    [CorpseName("a fire daemon corpse")]
    public class CovetousFireDaemon : CovetousCreature
    {
        [Constructable]
        public CovetousFireDaemon()
            : base(AIType.AI_Mage)
        {
            Name = "a fire daemon";
            Body = 9;
            BaseSoundID = 357;
        }

        [Constructable]
        public CovetousFireDaemon(int level, bool voidSpawn)
            : base(AIType.AI_Mage, level, voidSpawn)
        {
            Name = "a fire daemon";
            Body = 9;
            BaseSoundID = 357;
        }

        public CovetousFireDaemon(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}