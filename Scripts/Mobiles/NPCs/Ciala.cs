using Server.Items;
using System;

namespace Server.Engines.Quests
{
    public class Ciala : MondainQuester
    {
        [Constructable]
        public Ciala()
            : base("Ciala", "the aborist")
        {
            SetSkill(SkillName.Meditation, 60.0, 83.0);
            SetSkill(SkillName.Focus, 60.0, 83.0);
        }

        public Ciala(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests => new Type[]
                {
                    typeof(GlassyFoeQuest),
                    typeof(CircleOfLifeQuest),
                    typeof(DustToDustQuest),
                    typeof(ArchSupportQuest)
                };
        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = true;
            Race = Race.Elf;

            Hue = 0x8374;
            HairItemID = 0x2FD0;
            HairHue = 0x31D;
        }

        public override void InitOutfit()
        {
            SetWearable(new Boots(), 0x1BB, 1);
            SetWearable(new ElvenShirt(), 0x737, 1);
            SetWearable(new Skirt(), 0x21, 1);
            SetWearable(new RoyalCirclet(), dropChance: 1);
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