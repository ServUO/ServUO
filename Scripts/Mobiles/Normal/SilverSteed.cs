using System;

namespace Server.Mobiles
{
    [CorpseName("a silver steed corpse")]
    public class SilverSteed : BaseMount
    {
        [Constructable]
        public SilverSteed()
            : this("a silver steed")
        {
        }

        [Constructable]
        public SilverSteed(string name)
            : base(name, 0x75, 0x3EA8, AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            this.InitStats(Utility.Random(50, 30), Utility.Random(50, 30), 10);
            this.Skills[SkillName.MagicResist].Base = 25.0 + (Utility.RandomDouble() * 5.0);
            this.Skills[SkillName.Wrestling].Base = 35.0 + (Utility.RandomDouble() * 10.0);
            this.Skills[SkillName.Tactics].Base = 30.0 + (Utility.RandomDouble() * 15.0);

            this.ControlSlots = 1;
            this.Tamable = true;
            this.MinTameSkill = 103.1;
        }

        public SilverSteed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}