using System;
using System.Collections.Generic;

namespace Server.Mobiles 
{ 
    public class InnKeeper : BaseVendor 
    { 
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        [Constructable]
        public InnKeeper()
            : base("the innkeeper")
        { 
        }

        public InnKeeper(Serial serial)
            : base(serial)
        { 
        }

        public override VendorShoeType ShoeType
        {
            get
            {
                return Utility.RandomBool() ? VendorShoeType.Sandals : VendorShoeType.Shoes;
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
            this.m_SBInfos.Add(new SBInnKeeper()); 
			
            if (this.IsTokunoVendor)
                this.m_SBInfos.Add(new SBSEFood());
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