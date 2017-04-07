using System;
using System.Collections.Generic;
using Server.Engines.BulkOrders;

namespace Server.Mobiles
{
    public class Carpenter : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        [Constructable]
        public Carpenter()
            : base("the carpenter")
        {
            this.SetSkill(SkillName.Carpentry, 85.0, 100.0);
            this.SetSkill(SkillName.Lumberjacking, 60.0, 83.0);
        }

        public Carpenter(Serial serial)
            : base(serial)
        {
        }

        public override NpcGuild NpcGuild
        {
            get
            {
                return NpcGuild.TinkersGuild;
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
            this.m_SBInfos.Add(new SBStavesWeapon());
            this.m_SBInfos.Add(new SBCarpenter());
            this.m_SBInfos.Add(new SBWoodenShields());
			
            if (this.IsTokunoVendor)
                this.m_SBInfos.Add(new SBSECarpenter());
        }

        public override void InitOutfit()
        {
            base.InitOutfit();

            this.AddItem(new Server.Items.HalfApron());
        }

        #region Bulk Orders
        public override BODType BODType { get { return BODType.Carpentry; } }

        public override bool IsValidBulkOrder(Item item)
        {
            return (item is SmallCarpentryBOD || item is LargeCarpentryBOD);
        }

        public override bool SupportsBulkOrders(Mobile from)
        {
            return BulkOrderSystem.NewSystemEnabled && from is PlayerMobile && from.Skills[SkillName.Carpentry].Base > 0;
        }

        public override void OnSuccessfulBulkOrderReceive(Mobile from)
        {
            if (from is PlayerMobile)
                ((PlayerMobile)from).NextCarpentryBulkOrder = TimeSpan.Zero;
        }

        #endregion

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