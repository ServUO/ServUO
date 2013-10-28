using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class Olaeni : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        [Constructable]
        public Olaeni()
            : base("the thaumaturgist")
        { 
            this.Name = "Olaeni";
        }

        public Olaeni(Serial serial)
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
			
            this.Hue = 0x851D;
            this.HairItemID = 0x2FCF;
            this.HairHue = 0x322;			
        }

        public override void InitOutfit()
        {
            this.AddItem(new Shoes(0x736));
            this.AddItem(new FemaleElvenRobe(0x1C));
            this.AddItem(new GemmedCirclet());
            this.AddItem(new Item(0xDF2));
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