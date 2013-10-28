using System;
using Server.Items;

namespace Server.Engines.Quests
{
    public class Canir : MondainQuester
    {
        [Constructable]
        public Canir()
            : base("Canir", "the thaumaturgist")
        { 
            this.SetSkill(SkillName.Focus, 60.0, 83.0);
        }

        public Canir(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        { 
            get
            {
                return new Type[] 
                {
                    typeof(TroglodytesQuest),
                    typeof(TrogAndHisDogQuest)
                };
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = true;
            this.CantWalk = true;
            this.Race = Race.Elf;
			
            this.Hue = 0x876C;
            this.HairItemID = 0x2FD0;
            this.HairHue = 0x33;
        }

        public override void InitOutfit()
        {
            this.AddItem(new Sandals(0x1BB));
            this.AddItem(new MaleElvenRobe(0x5A5));
            this.AddItem(new GemmedCirclet());
            this.AddItem(RandomWand.CreateWand());
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