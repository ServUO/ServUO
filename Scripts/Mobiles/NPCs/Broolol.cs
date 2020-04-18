using Server.Items;
using System;

namespace Server.Engines.Quests
{
    public class Broolol : MondainQuester
    {
        [Constructable]
        public Broolol()
            : base("Lorekeeper Broolol", "the keeper of tradition")
        {
            SetSkill(SkillName.Meditation, 60.0, 83.0);
            SetSkill(SkillName.Focus, 60.0, 83.0);
        }

        public Broolol(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests => new Type[] { typeof(TheAncientWorldQuest) };
        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            Race = Race.Elf;

            Hue = 0x851D;
            HairItemID = 0x2FCF;
            HairHue = 0x323;
        }

        public override void InitOutfit()
        {
            AddItem(new ElvenBoots(0x71B));
            AddItem(new MaleElvenRobe(0x1C));
            AddItem(new WildStaff());
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