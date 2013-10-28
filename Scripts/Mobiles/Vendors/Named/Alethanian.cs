using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class Alethanian : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        [Constructable]
        public Alethanian()
            : base("the wise")
        { 
            this.Name = "Elder Alethanian";
        }

        public Alethanian(Serial serial)
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
			
            this.Female = true;
            this.Race = Race.Elf;
			
            this.Hue = 0x876C;
            this.HairItemID = 0x2FC2;
            this.HairHue = 0x368;			
        }

        public override void InitOutfit()
        {
            this.AddItem(new ElvenBoots());
            this.AddItem(new GemmedCirclet());
            this.AddItem(new HidePants());
            this.AddItem(new HideFemaleChest());
            this.AddItem(new HidePauldrons());
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