using Server.Items;
using System;

namespace Server.Engines.Quests
{
    public class Landy : MondainQuester
    {
        [Constructable]
        public Landy()
            : base("Landy", "the soil nurturer")
        {
            SetSkill(SkillName.Meditation, 60.0, 83.0);
            SetSkill(SkillName.Focus, 60.0, 83.0);
        }

        public Landy(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests => new Type[]
                {
                    typeof(CreepyCrawliesQuest),
                    typeof(MongbatMenaceQuest),
                    typeof(SpecimensQuest),
                    typeof(ThinningTheHerdQuest)
                };
        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            Race = Race.Elf;

            Hue = 0x8384;
            HairItemID = 0x2FCE;
            HairHue = 0x91;
        }

        public override void InitOutfit()
        {
			SetWearable(new Sandals(), 0x901, 1);
            SetWearable(new Tunic(), 0x719, 1);
            SetWearable(new ShortPants(), 0x1BB, 1);
            SetWearable(new LeafGloves(), 0x1BB, 1);
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