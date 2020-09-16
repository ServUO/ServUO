using Server.Engines.BulkOrders;
using System;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class Armorer : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();

        [Constructable]
        public Armorer()
            : base("the armourer")
        {
            SetSkill(SkillName.ArmsLore, 64.0, 100.0);
            SetSkill(SkillName.Blacksmith, 60.0, 83.0);
        }

        public Armorer(Serial serial)
            : base(serial)
        {
        }

        public override VendorShoeType ShoeType => VendorShoeType.Boots;
        protected override List<SBInfo> SBInfos => m_SBInfos;
        public override void InitSBInfo()
        {
            switch (Utility.Random(4))
            {
                case 0:
                    {
                        m_SBInfos.Add(new SBLeatherArmor());
                        m_SBInfos.Add(new SBStuddedArmor());
                        m_SBInfos.Add(new SBMetalShields());
                        m_SBInfos.Add(new SBPlateArmor());
                        m_SBInfos.Add(new SBHelmetArmor());
                        m_SBInfos.Add(new SBChainmailArmor());
                        m_SBInfos.Add(new SBRingmailArmor());
                        break;
                    }
                case 1:
                    {
                        m_SBInfos.Add(new SBStuddedArmor());
                        m_SBInfos.Add(new SBLeatherArmor());
                        m_SBInfos.Add(new SBMetalShields());
                        m_SBInfos.Add(new SBHelmetArmor());
                        break;
                    }
                case 2:
                    {
                        m_SBInfos.Add(new SBMetalShields());
                        m_SBInfos.Add(new SBPlateArmor());
                        m_SBInfos.Add(new SBHelmetArmor());
                        m_SBInfos.Add(new SBChainmailArmor());
                        m_SBInfos.Add(new SBRingmailArmor());
                        break;
                    }
                case 3:
                    {
                        m_SBInfos.Add(new SBMetalShields());
                        m_SBInfos.Add(new SBHelmetArmor());
                        break;
                    }
            }
            if (IsTokunoVendor)
            {
                m_SBInfos.Add(new SBSELeatherArmor());
                m_SBInfos.Add(new SBSEArmor());
            }
        }

        #region Bulk Orders
        public override BODType BODType => BODType.Smith;

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
            return (from is PlayerMobile && from.Skills[SkillName.Blacksmith].Base > 0);
        }

        public override TimeSpan GetNextBulkOrder(Mobile from)
        {
            if (from is PlayerMobile)
                return ((PlayerMobile)from).NextSmithBulkOrder;

            return TimeSpan.Zero;
        }

        public override void OnSuccessfulBulkOrderReceive(Mobile from)
        {
            if (from is PlayerMobile)
                ((PlayerMobile)from).NextSmithBulkOrder = TimeSpan.Zero;
        }

        #endregion

        public override void InitOutfit()
        {
            base.InitOutfit();

            AddItem(new Items.HalfApron(Utility.RandomYellowHue()));
            AddItem(new Items.Bascinet());
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version == 0)
            {
                Title = "the armourer";
            }
        }
    }
}
