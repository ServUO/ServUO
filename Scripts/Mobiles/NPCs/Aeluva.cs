using Server.Items;
using System;

namespace Server.Engines.Quests
{
    public class Aeluva : MondainQuester
    {
        [Constructable]
        public Aeluva()
            : base("Aeluva", "the arcanist")
        {
            SetSkill(SkillName.Meditation, 60.0, 83.0);
            SetSkill(SkillName.Focus, 60.0, 83.0);
        }

        public Aeluva(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests => new Type[] { typeof(PatienceQuest) };
        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = true;
            Race = Race.Elf;

            Hue = 0x8835;
            HairItemID = 0x2FD0;
            HairHue = 0x387;
        }

        public override void InitOutfit()
        {
            AddItem(new ElvenBoots());
            AddItem(new ElvenShirt());
            AddItem(new Skirt());
            AddItem(new Circlet());
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