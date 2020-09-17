using Server.Items;
using System;

namespace Server.Engines.Quests
{
    public class Synaeva : MondainQuester
    {
        [Constructable]
        public Synaeva()
            : base("Synaeva", "the arcanist")
        {
            SetSkill(SkillName.Meditation, 60.0, 83.0);
            SetSkill(SkillName.Focus, 60.0, 83.0);
        }

        public Synaeva(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests => new Type[] { typeof(FirendOfTheFeyQuest) };
        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = true;
            Race = Race.Elf;

            Hue = 0x8374;
            HairItemID = 0x2B71;
            HairHue = 0x385;
        }

        public override void InitOutfit()
        {
            AddItem(new ElvenBoots());
            AddItem(new LeafArms());
            AddItem(new FemaleLeafChest());
            AddItem(new LeafTonlet());
            AddItem(new WildStaff());

            Item item;

            item = new RavenHelm
            {
                Hue = 0x583
            };
            AddItem(item);
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