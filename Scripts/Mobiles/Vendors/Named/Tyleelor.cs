using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class Tyleelor : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        [Constructable]
        public Tyleelor()
            : base("the expeditionist")
        { 
            this.Name = "Tyeelor";
        }

        public Tyleelor(Serial serial)
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
			
            this.Hue = 0x8367;
            this.HairItemID = 0x2FC1;
            this.HairHue = 0x38;			
        }

        public override void InitOutfit()
        {
            this.AddItem(new ElvenBoots(0x1BB));
			
            Item item;
			
            item = new WoodlandLegs();
            item.Hue = 0x236;
            this.AddItem(item);			
			
            item = new WoodlandChest();
            item.Hue = 0x236;
            this.AddItem(item);
			
            item = new WoodlandArms();
            item.Hue = 0x236;
            this.AddItem(item);
			
            item = new WoodlandBelt();
            item.Hue = 0x237;
            this.AddItem(item);
			
            item = new VultureHelm();
            item.Hue = 0x236;
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