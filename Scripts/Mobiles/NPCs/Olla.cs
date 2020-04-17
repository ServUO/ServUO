using Server.Items;
using System;

namespace Server.Engines.Quests
{
    public class Olla : MondainQuester
    {
        [Constructable]
        public Olla()
            : base("Olla", "the metal weaver")
        {
            SetSkill(SkillName.Meditation, 60.0, 83.0);
            SetSkill(SkillName.Focus, 60.0, 83.0);
        }

        public Olla(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests => new Type[]
                {
                    typeof(CutsBothWaysQuest),
                    typeof(DragonProtectionQuest),
                    typeof(NothingFancyQuest),
                    typeof(TheBulwarkQuest)
                };
        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = true;
            Race = Race.Elf;

            Hue = 0x824E;
            HairItemID = 0x2FCE;
            HairHue = 0x8F;
        }

        public override void InitOutfit()
        {
            AddItem(new ElvenBoots());
            AddItem(new LongPants(0x3B3));
            AddItem(new ElvenShirt());
            AddItem(new SmithHammer());
            AddItem(new FullApron(0x1BB));
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