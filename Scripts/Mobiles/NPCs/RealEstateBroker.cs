using System.Collections.Generic;

namespace Server.Mobiles
{
    public class RealEstateBroker : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();

        [Constructable]
        public RealEstateBroker()
            : base("the real estate broker")
        {
        }

        public RealEstateBroker(Serial serial)
            : base(serial)
        {
        }

        protected override List<SBInfo> SBInfos => m_SBInfos;
    
        public override void InitSBInfo()
        {
            m_SBInfos.Add(new SBScribe(this));
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
