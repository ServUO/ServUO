using System;
using Server.Items;

namespace Server.Engines.Quests
{ 
    public class Aernya : MondainQuester
    {
        [Constructable]
        public Aernya()
            : base("Aernya", "the mistress of admissions")
        { 
            this.SetSkill(SkillName.Focus, 60.0, 83.0);
        }

        public Aernya(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        {
            get
            {
                return new Type[] { typeof(MistakenIdentityQuest) };
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = true;
            this.Race = Race.Human;
			
            this.Hue = 0x8404;
            this.HairItemID = 0x2047;
            this.HairHue = 0x465;
        }

        public override void InitOutfit()
        {
            this.AddItem(new Backpack());
            this.AddItem(new Sandals(0x743));
            this.AddItem(new FancyShirt(0x3B3));
            this.AddItem(new Cloak(0x3));
            this.AddItem(new Skirt());
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