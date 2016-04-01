using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class Abbein : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        [Constructable]
        public Abbein()
            : base("the wise")
        { 
            this.Name = "Elder Abbein";
        }

        public Abbein(Serial serial)
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
			
            this.Hue = 0x824D;
            this.HairItemID = 0x2FD1;
            this.HairHue = 0x321;			
        }

        public override void InitOutfit()
        {
            this.AddItem(new ElvenBoots(0x74B));
            this.AddItem(new FemaleElvenRobe(0x8A8));
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