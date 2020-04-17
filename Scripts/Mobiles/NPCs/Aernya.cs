using Server.Items;
using System;

namespace Server.Engines.Quests
{
    public class Aernya : MondainQuester
    {
        [Constructable]
        public Aernya()
            : base("Aernya", "the mistress of admissions")
        {
            SetSkill(SkillName.Focus, 60.0, 83.0);
        }

        public Aernya(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests => new Type[] { typeof(MistakenIdentityQuest) };
        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = true;
            Race = Race.Human;

            Hue = 0x8404;
            HairItemID = 0x2047;
            HairHue = 0x465;
        }

        public override void InitOutfit()
        {
            AddItem(new Backpack());
            AddItem(new Sandals(0x743));
            AddItem(new FancyShirt(0x3B3));
            AddItem(new Cloak(0x3));
            AddItem(new Skirt());
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