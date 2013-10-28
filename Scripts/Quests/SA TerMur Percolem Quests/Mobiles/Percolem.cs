using System;

namespace Server.Engines.Quests
{ 
    public class Percolem : MondainQuester
    {
        [Constructable]
        public Percolem()
            : base("Percolem", "the Hunter")
        {
            if (!(this is MondainQuester))

                this.Name = "Percolem";
            this.Title = "the Hunter";
        }

        public Percolem(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        {
            get
            {
                return new Type[] 
                {
                    typeof(PercolemTheHunterTierOneQuest)
                };
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = false;
            this.Race = Race.Human;
			
            this.Hue = 0x840C;
            this.HairItemID = 0x203C;
            this.HairHue = 0x3B3;
        }

        public override void InitOutfit()
        {
            this.CantWalk = true;
            
            this.AddItem(new Server.Items.Boots());
            this.AddItem(new Server.Items.Shirt(1436));
            this.AddItem(new Server.Items.ShortPants(1436));
            this.AddItem(new Server.Items.CompositeBow());
            
            this.Blessed = true;
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