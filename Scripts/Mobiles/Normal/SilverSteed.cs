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
            : base(name, 0x75, 0x3EA8, AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            InitStats(Utility.Random(50, 30), Utility.Random(50, 30), 10);
            Skills[SkillName.MagicResist].Base = 25.0 + (Utility.RandomDouble() * 5.0);
            Skills[SkillName.Wrestling].Base = 35.0 + (Utility.RandomDouble() * 10.0);
            Skills[SkillName.Tactics].Base = 30.0 + (Utility.RandomDouble() * 15.0);

            ControlSlots = 1;
            Tamable = false;
            MinTameSkill = 103.1;
        }

        public SilverSteed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
