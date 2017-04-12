using System;
using System.Collections.Generic;
using Server.Engines.BulkOrders;

namespace Server.Mobiles
{
    public class Weaponsmith : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos
        {
            get
            {
                return this.m_SBInfos;
            }
        }

        [Constructable]
        public Weaponsmith()
            : base("the weaponsmith")
        {
            this.SetSkill(SkillName.ArmsLore, 64.0, 100.0);
            this.SetSkill(SkillName.Blacksmith, 65.0, 88.0);
            this.SetSkill(SkillName.Fencing, 45.0, 68.0);
            this.SetSkill(SkillName.Macing, 45.0, 68.0);
            this.SetSkill(SkillName.Swords, 45.0, 68.0);
            this.SetSkill(SkillName.Tactics, 36.0, 68.0);
        }

        public override void InitSBInfo()
        {
            this.m_SBInfos.Add(new SBWeaponSmith());
			
            if (this.IsTokunoVendor)
                this.m_SBInfos.Add(new SBSEWeapons());
        }

        public override VendorShoeType ShoeType
        {
            get
            {
                return Utility.RandomBool() ? VendorShoeType.Boots : VendorShoeType.ThighBoots;
            }
        }

        public override int GetShoeHue()
        {
            return 0;
        }

        public override void InitOutfit()
        {
            base.InitOutfit();

            this.AddItem(new Server.Items.HalfApron());
        }

        #region Bulk Orders
        public override BODType BODType { get { return BODType.Smith; } }

        public override Item CreateBulkOrder(Mobile from, bool fromContextMenu)
        {
            PlayerMobile pm = from as PlayerMobile;

            if (pm != null && pm.NextSmithBulkOrder == TimeSpan.Zero && (fromContextMenu || 0.2 > Utility.RandomDouble()))
            {
                double theirSkill = pm.Skills[SkillName.Blacksmith].Base;

                if (theirSkill >= 70.1)
                    pm.NextSmithBulkOrder = TimeSpan.FromHours(6.0);
                else if (theirSkill >= 50.1)
                    pm.NextSmithBulkOrder = TimeSpan.FromHours(2.0);
                else
                    pm.NextSmithBulkOrder = TimeSpan.FromHours(1.0);

                if (theirSkill >= 70.1 && ((theirSkill - 40.0) / 300.0) > Utility.RandomDouble())
                    return new LargeSmithBOD();

                return SmallSmithBOD.CreateRandomFor(from);
            }

            return null;
        }

        public override bool IsValidBulkOrder(Item item)
        {
            return (item is SmallSmithBOD || item is LargeSmithBOD);
        }

        public override bool SupportsBulkOrders(Mobile from)
        {
            return (from is PlayerMobile && Core.AOS && from.Skills[SkillName.Blacksmith].Base > 0);
        }

        public override TimeSpan GetNextBulkOrder(Mobile from)
        {
            if (from is PlayerMobile)
                return ((PlayerMobile)from).NextSmithBulkOrder;

            return TimeSpan.Zero;
        }

        public override void OnSuccessfulBulkOrderReceive(Mobile from)
        {
            if (Core.SE && from is PlayerMobile)
                ((PlayerMobile)from).NextSmithBulkOrder = TimeSpan.Zero;
        }

        #endregion

        public Weaponsmith(Serial serial)
            : base(serial)
        {
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