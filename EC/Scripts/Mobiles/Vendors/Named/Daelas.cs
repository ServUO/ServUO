using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class Daelas : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        [Constructable]
        public Daelas()
            : base("the aborist")
        { 
            this.Name = "Daelas";
        }

        public Daelas(Serial serial)
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
			
            this.Hue = 0x84DE;
            this.HairItemID = 0x2FCF;
            this.HairHue = 0x8F;			
        }

        public override void InitOutfit()
        {
            this.AddItem(new ElvenBoots(0x901));
            this.AddItem(new ElvenPants(0x8AB));
			
            Item item;
			
            item = new LeafGloves();
            item.Hue = 0x1BB;
            this.AddItem(item);			
			
            item = new LeafChest();
            item.Hue = 0x8B0;
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