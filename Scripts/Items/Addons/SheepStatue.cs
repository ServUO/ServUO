using Server.Engines.VeteranRewards;
using Server.Multis;
using Server.Network;
using System;

namespace Server.Items
{
    public class SheepStatue : BaseAddon, IRewardItem
    {
        private bool m_IsRewardItem;
        private int m_ResourceCount;

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextResourceCount { get; set; }

        public override bool ForceShowProperties => true;

        [Constructable]
        public SheepStatue(int itemID)
            : base()
        {
            AddComponent(new InternalAddonComponent(itemID), 0, 0, 0);
            NextResourceCount = DateTime.UtcNow + TimeSpan.FromDays(1);
        }

        public SheepStatue(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                SheepStatueDeed deed = new SheepStatueDeed
                {
                    IsRewardItem = m_IsRewardItem,
                    ResourceCount = m_ResourceCount
                };

                return deed;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem
        {
            get
            {
                return m_IsRewardItem;
            }
            set
            {
                m_IsRewardItem = value;
                UpdateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int ResourceCount
        {
            get
            {
                return m_ResourceCount;
            }
            set
            {
                m_ResourceCount = value;

                if (Components.Count > 0)
                {
                    if (m_ResourceCount == 0 && Components[0].ItemID != 0x4A95)
                        Components[0].ItemID = 0x4A95;
                    else if (m_ResourceCount > 0 && Components[0].ItemID != 0x4A94)
                        Components[0].ItemID = 0x4A94;
                }

                UpdateProperties();
            }
        }

        public override void OnComponentUsed(AddonComponent c, Mobile from)
        {
            BaseHouse house = BaseHouse.FindHouseAt(this);

            /*
            * Unique problems have unique solutions.  OSI does not have a problem with 1000s of mining carts
            * due to the fact that they have only a miniscule fraction of the number of 10 year vets that a
            * typical RunUO shard will have (RunUO's scaled down account aging system makes this a unique problem),
            * and the "freeness" of free accounts. We also dont have mitigating factors like inactive (unpaid)
            * accounts not gaining veteran time.
            *
            * The lack of high end vets and vet rewards on OSI has made testing the *exact* ranging/stacking
            * behavior of these things all but impossible, so either way its just an estimation.
            *
            * If youd like your shard's carts/stumps to work the way they did before, simply replace the check
            * below with this line of code:
            *
            * if (!from.InRange(GetWorldLocation(), 2)
            *
            * However, I am sure these checks are more accurate to OSI than the former version was.
            *
            */

            if (!from.InRange(GetWorldLocation(), 2) || !from.InLOS(this) || !((from.Z - Z) > -3 && (from.Z - Z) < 3))
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            }
            else if (house != null && house.HasSecureAccess(from, SecureLevel.Friends))
            {
                if (m_ResourceCount > 0)
                {
                    Item res = null;

                    switch (Utility.Random(5))
                    {
                        case 0: res = new Wool(); break;
                        case 1: res = new Leather(); break;
                        case 2: res = new SpinedLeather(); break;
                        case 3: res = new HornedLeather(); break;
                        case 4: res = new BarbedLeather(); break;
                    }

                    int amount = Math.Min(10, m_ResourceCount);

                    if (res != null)
                    {
                        res.Amount = amount;

                        if (!from.PlaceInBackpack(res))
                        {
                            res.Delete();
                            from.SendLocalizedMessage(1078837); // Your backpack is full! Please make room and try again.
                        }
                        else
                        {
                            ResourceCount -= amount;
                            PublicOverheadMessage(MessageType.Regular, 0, 1151834, m_ResourceCount.ToString()); // Resources: ~1_COUNT~
                        }
                    }
                }
                else
                    from.SendLocalizedMessage(1094725); // There are no more ResourceCounts available at this time.
            }
            else
                from.SendLocalizedMessage(1061637); // You are not allowed to access 
        }

        private class InternalAddonComponent : AddonComponent
        {
            public InternalAddonComponent(int id)
                : base(id)
            {
            }

            public override void GetProperties(ObjectPropertyList list)
            {
                base.GetProperties(list);

                if (Addon is SheepStatue)
                {
                    list.Add(1151834, ((SheepStatue)Addon).ResourceCount.ToString()); // Resources: ~1_val~
                }
            }

            public InternalAddonComponent(Serial serial)
                : base(serial)
            {
            }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);

                writer.WriteEncodedInt(0); // version
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);

                int version = reader.ReadEncodedInt();
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version

            TryGiveResourceCount();

            writer.Write(m_IsRewardItem);
            writer.Write(m_ResourceCount);

            writer.Write(NextResourceCount);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            m_IsRewardItem = reader.ReadBool();
            m_ResourceCount = reader.ReadInt();

            NextResourceCount = reader.ReadDateTime();
        }

        private void TryGiveResourceCount()
        {
            if (NextResourceCount < DateTime.UtcNow)
            {
                ResourceCount = Math.Min(100, m_ResourceCount + 10);
                NextResourceCount = DateTime.UtcNow + TimeSpan.FromDays(1);
            }
        }
    }

    public class SheepStatueDeed : BaseAddonDeed, IRewardItem
    {
        private bool m_IsRewardItem;
        private int m_ResourceCount;

        [Constructable]
        public SheepStatueDeed()
            : base()
        {
            LootType = LootType.Blessed;
        }

        public SheepStatueDeed(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1151835;// Sheep Statue Deed

        public override BaseAddon Addon
        {
            get
            {
                SheepStatue addon = new SheepStatue(m_ResourceCount > 0 ? 0x4A94 : 0x4A95)
                {
                    IsRewardItem = m_IsRewardItem,
                    ResourceCount = m_ResourceCount
                };

                return addon;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem
        {
            get
            {
                return m_IsRewardItem;
            }
            set
            {
                m_IsRewardItem = value;
                InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int ResourceCount
        {
            get
            {
                return m_ResourceCount;
            }
            set
            {
                m_ResourceCount = value;
                InvalidateProperties();
            }
        }
        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_IsRewardItem)
                list.Add(1076223); // 7th Year Veteran Reward
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (m_IsRewardItem && !RewardSystem.CheckIsUsableBy(from, this, null))
                return;

            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
            }
            else
                base.OnDoubleClick(from);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version

            writer.Write(m_IsRewardItem);
            writer.Write(m_ResourceCount);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            m_IsRewardItem = reader.ReadBool();
            m_ResourceCount = reader.ReadInt();
        }
    }
}
