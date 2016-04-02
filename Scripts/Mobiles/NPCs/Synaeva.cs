using System;
using Server.Items;

namespace Server.Engines.Quests
{ 
    public class Synaeva : MondainQuester
    { 
        [Constructable]
        public Synaeva()
            : base("Synaeva", "the arcanist")
        { 
            this.SetSkill(SkillName.Meditation, 60.0, 83.0);
            this.SetSkill(SkillName.Focus, 60.0, 83.0);
        }

        public Synaeva(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        {
            get
            {
                return new Type[] { typeof(FirendOfTheFeyQuest) };
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = true;
            this.Race = Race.Elf;
			
            this.Hue = 0x8374;
            this.HairItemID = 0x2B71;
            this.HairHue = 0x385;
        }

        public override void InitOutfit()
        {
            this.AddItem(new ElvenBoots());
            this.AddItem(new LeafArms());
            this.AddItem(new FemaleLeafChest());
            this.AddItem(new LeafTonlet());
            this.AddItem(new WildStaff());
			
            Item item;
			
            item = new RavenHelm();
            item.Hue = 0x583;					
            this.AddItem(item); 
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