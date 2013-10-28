using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class Athialon : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        [Constructable]
        public Athialon()
            : base("the expeditionist")
        { 
            this.Name = "Athialon";
        }

        public Athialon(Serial serial)
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
			
            this.Hue = 0x8382;
            this.HairItemID = 0x2FC0;
            this.HairHue = 0x35;			
        }

        public override void InitOutfit()
        {
            this.AddItem(new ElvenBoots(0x901));
            this.AddItem(new DiamondMace());
            this.AddItem(new WoodlandBelt());
			
            Item item;
			
            item = new WoodlandLegs();
            item.Hue = 0x3B2;
            this.AddItem(item);			
			
            item = new WoodlandChest();
            item.Hue = 0x3B2;
            this.AddItem(item);
			
            item = new WoodlandArms();
            item.Hue = 0x3B2;
            this.AddItem(item);
			
            item = new WingedHelm();
            item.Hue = 0x3B2;
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