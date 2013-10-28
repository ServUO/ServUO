using System;
using Server.Items;

namespace Server.Engines.Quests
{
    public class Broolol : MondainQuester
    {
        [Constructable]
        public Broolol()
            : base("Lorekeeper Broolol", "the keeper of tradition")
        { 
            this.SetSkill(SkillName.Meditation, 60.0, 83.0);
            this.SetSkill(SkillName.Focus, 60.0, 83.0);
        }

        public Broolol(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        {
            get
            {
                return new Type[] { typeof(TheAncientWorldQuest) };
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = false;
            this.Race = Race.Elf;
			
            this.Hue = 0x851D;
            this.HairItemID = 0x2FCF;
            this.HairHue = 0x323;
        }

        public override void InitOutfit()
        {
            this.AddItem(new ElvenBoots(0x71B));
            this.AddItem(new MaleElvenRobe(0x1C));
            this.AddItem(new WildStaff());
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