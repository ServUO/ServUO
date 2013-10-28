using System;
using Server.Items;

namespace Server.Mobiles
{
    public class Aminia : BaseCreature
    { 
        [Constructable]
        public Aminia()
            : base(AIType.AI_Melee, FightMode.None, 2, 1, 0.5, 2)
        { 
            this.Name = "Aminia";
            this.Title = "the master weaponsmith's wife";
            this.Blessed = true;
			
            this.InitStats(100, 100, 25);
			
            this.Female = true;
            this.Race = Race.Human;
			
            this.Hue = 0x83ED;
            this.HairItemID = 0x203B;
            this.HairHue = 0x454;	
			
            this.AddItem(new Backpack());
            this.AddItem(new Sandals(0x75B));
            this.AddItem(new Tunic(0x4BF));
            this.AddItem(new Skirt(0x8FD));
        }

        public Aminia(Serial serial)
            : base(serial)
        {
        }

        public override void OnThink()
        {
            int hours = 0;
            int minutes = 0;
		
            Clock.GetTime(this.Map, this.Location.X, this.Location.Y, out hours, out minutes);
			
            if (hours == 21)
            {
                this.Blessed = false;
                this.Body = 0x17;
            }
            else
            {
                this.Blessed = true;
                this.Body = 0x191;
            }
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