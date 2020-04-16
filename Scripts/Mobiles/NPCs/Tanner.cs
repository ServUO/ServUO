using System.Collections.Generic;

namespace Server.Mobiles
{
    public class Tanner : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        [Constructable]
        public Tanner()
            : base("the tanner")
        {
            this.SetSkill(SkillName.Tailoring, 36.0, 68.0);
        }

        public Tanner(Serial serial)
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
            if (!this.IsStygianVendor)
            {
                this.m_SBInfos.Add(new SBTanner());
            }
            else
            {
                this.m_SBInfos.Add(new SBSATanner());
            }
        }

        public override bool ValidateBought(Mobile buyer, Item item)
        {
            if (item is Server.Items.TaxidermyKit && buyer.Skills[SkillName.Carpentry].Value < 90.1)
            {
                this.SayTo(buyer, 1042603, 0x3B2); // You would not understand how to use the kit.
                return false;
            }

            return true;
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