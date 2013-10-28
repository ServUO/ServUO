using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class Rebinil : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        [Constructable]
        public Rebinil()
            : base("the healer")
        { 
            this.Name = "Rebinil";
        }

        public Rebinil(Serial serial)
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
			
            this.Hue = 0x83E7;
            this.HairItemID = 0x2FD0;
            this.HairHue = 0x26B;			
        }

        public override void InitOutfit()
        {
            this.AddItem(new Sandals(0x719));
            this.AddItem(new FemaleElvenRobe(0x757));
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