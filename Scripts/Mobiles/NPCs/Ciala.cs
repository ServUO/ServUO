using System;
using Server.Items;

namespace Server.Engines.Quests
{ 
    public class Ciala : MondainQuester
    { 
        [Constructable]
        public Ciala()
            : base("Ciala", "the aborist")
        { 
            this.SetSkill(SkillName.Meditation, 60.0, 83.0);
            this.SetSkill(SkillName.Focus, 60.0, 83.0);
        }

        public Ciala(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        { 
            get
            {
                return new Type[] 
                {
                    typeof(GlassyFoeQuest),
                    typeof(CircleOfLifeQuest),
                    typeof(DustToDustQuest),
                    typeof(ArchSupportQuest)
                };
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = true;
            this.Race = Race.Elf;
			
            this.Hue = 0x8374;
            this.HairItemID = 0x2FD0;
            this.HairHue = 0x31D;
        }

        public override void InitOutfit()
        {
            this.AddItem(new Boots(0x1BB));
            this.AddItem(new ElvenShirt(0x737));
            this.AddItem(new Skirt(0x21));
            this.AddItem(new RoyalCirclet());
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