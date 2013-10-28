using System;
using System.Collections.Generic;

namespace Server.Mobiles 
{ 
    public class LeatherWorker : BaseVendor 
    { 
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        [Constructable]
        public LeatherWorker()
            : base("the leather worker")
        { 
        }

        public LeatherWorker(Serial serial)
            : base(serial)
        { 
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
            this.m_SBInfos.Add(new SBLeatherArmor()); 
            this.m_SBInfos.Add(new SBStuddedArmor()); 
            this.m_SBInfos.Add(new SBLeatherWorker()); 
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