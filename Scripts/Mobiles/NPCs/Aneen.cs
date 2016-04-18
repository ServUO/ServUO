using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class Aneen : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        [Constructable]
        public Aneen()
            : base("the keeper of tradition")
        { 
            this.Name = "Lorekeeper Aneen";
        }

        public Aneen(Serial serial)
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
			
            this.Hue = 0x83E5;
            this.HairItemID = 0x2FBF;
            this.HairHue = 0x90;			
        }

        public override void InitOutfit()
        {
            this.AddItem(new Sandals(0x1BB));
            this.AddItem(new MaleElvenRobe(0x48F));
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