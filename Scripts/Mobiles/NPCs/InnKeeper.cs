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

        public override VendorShoeType ShoeType => Utility.RandomBool() ? VendorShoeType.Sandals : VendorShoeType.Shoes;
        protected override List<SBInfo> SBInfos => m_SBInfos;
        public override void InitSBInfo()
        {
            m_SBInfos.Add(new SBInnKeeper());

            if (IsTokunoVendor)
                m_SBInfos.Add(new SBSEFood());
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