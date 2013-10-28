using System;
using System.Collections.Generic;

namespace Server.Mobiles 
{ 
    public class Beekeeper : BaseVendor 
    { 
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        [Constructable]
        public Beekeeper()
            : base("the beekeeper")
        { 
        }

        public Beekeeper(Serial serial)
            : base(serial)
        { 
        }

        public override VendorShoeType ShoeType
        {
            get
            {
                return VendorShoeType.Boots;
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
            this.m_SBInfos.Add(new SBBeekeeper()); 
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