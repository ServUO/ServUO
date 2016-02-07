using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class Mallew : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        [Constructable]
        public Mallew()
            : base("the wise")
        { 
            this.Name = "Elder Mallew";
        }

        public Mallew(Serial serial)
            : base(serial)
        {
        }

        public override bool CanTeach
        {
            get
            {
                return false;
            }
        }
        public override bool IsInvulnerable
        {
            get
            {
                return true;
            }
        }
        protected override List<SBInfo> SBInfos
        {
            get
            {
                return this.m_SBInfos;
            }
        }
        public override void InitSBInfo()
        { 
        }

        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = false;
            this.Race = Race.Elf;
			
            this.Hue = 0x876C;
            this.HairItemID = 0x2FD1;
            this.HairHue = 0x31E;			
        }

        public override void InitOutfit()
        {
            this.AddItem(new ElvenBoots(0x1BB));
            this.AddItem(new Circlet());
            this.AddItem(new Cloak(0x3B2));
			
            Item item;
			
            item = new LeafChest();
            item.Hue = 0x53E;
            this.AddItem(item);
			
            item = new LeafArms();
            item.Hue = 0x53E;
            this.AddItem(item);
			
            item = new LeafTonlet();
            item.Hue = 0x53E;
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