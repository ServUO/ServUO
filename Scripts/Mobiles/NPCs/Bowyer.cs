using Server.Engines.BulkOrders;
using System;
using System.Collections.Generic;

namespace Server.Mobiles
{
    [TypeAlias("Server.Mobiles.Bower")]
    public class Bowyer : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        [Constructable]
        public Bowyer()
            : base("the bowyer")
        {
            this.SetSkill(SkillName.Fletching, 80.0, 100.0);
            this.SetSkill(SkillName.Archery, 80.0, 100.0);
        }

        public Bowyer(Serial serial)
            : base(serial)
        {
        }

        public override VendorShoeType ShoeType => this.Female ? VendorShoeType.ThighBoots : VendorShoeType.Boots;
        protected override List<SBInfo> SBInfos => this.m_SBInfos;
        public override int GetShoeHue()
        {
            return 0;
        }

        public override void InitOutfit()
        {
            base.InitOutfit();

            this.AddItem(new Server.Items.Bow());
            this.AddItem(new Server.Items.LeatherGorget());
        }

        public override void InitSBInfo()
        {
            this.m_SBInfos.Add(new SBBowyer());
            this.m_SBInfos.Add(new SBRangedWeapon());

            if (this.IsTokunoVendor)
                this.m_SBInfos.Add(new SBSEBowyer());
        }

        #region Bulk Orders
        public override BODType BODType => BODType.Fletching;

        public override bool IsValidBulkOrder(Item item)
        {
            return (item is SmallFletchingBOD || item is LargeFletchingBOD);
        }

        public override bool SupportsBulkOrders(Mobile from)
        {
            return BulkOrderSystem.NewSystemEnabled && from is PlayerMobile && from.Skills[SkillName.Fletching].Base > 0;
        }

        public override void OnSuccessfulBulkOrderReceive(Mobile from)
        {
            if (from is PlayerMobile)
                ((PlayerMobile)from).NextFletchingBulkOrder = TimeSpan.Zero;
        }

        #endregion

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