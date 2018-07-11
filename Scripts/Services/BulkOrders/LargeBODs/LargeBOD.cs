using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Engines.BulkOrders
{
    [TypeAlias("Scripts.Engines.BulkOrders.LargeBOD")]
    public abstract class LargeBOD : Item, IBOD
    {
        public abstract BODType BODType { get; }

        private int m_AmountMax;
        private bool m_RequireExceptional;
        private BulkMaterialType m_Material;
        private LargeBulkEntry[] m_Entries;

        public LargeBOD(int hue, int amountMax, bool requireExeptional, BulkMaterialType material, LargeBulkEntry[] entries)
            : base(Core.AOS ? 0x2258 : 0x14EF)
        {
            Weight = 1.0;
            Hue = hue; // Blacksmith: 0x44E; Tailoring: 0x483
            LootType = LootType.Blessed;

            m_AmountMax = amountMax;
            m_RequireExceptional = requireExeptional;
            m_Material = material;
            m_Entries = entries;
        }

        public LargeBOD()
            : base(Core.AOS ? 0x2258 : 0x14EF)
        {
            Weight = 1.0;
            LootType = LootType.Blessed;
        }

        public LargeBOD(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int AmountMax
        {
            get
            {
                return m_AmountMax;
            }
            set
            {
                m_AmountMax = value;
                InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool RequireExceptional
        {
            get
            {
                return m_RequireExceptional;
            }
            set
            {
                m_RequireExceptional = value;
                InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public BulkMaterialType Material
        {
            get
            {
                return m_Material;
            }
            set
            {
                m_Material = value;
                InvalidateProperties();
            }
        }
        public LargeBulkEntry[] Entries
        {
            get
            {
                return m_Entries;
            }
            set
            {
                m_Entries = value;
                InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Complete
        {
            get
            {
                for (int i = 0; i < m_Entries.Length; ++i)
                {
                    if (m_Entries[i].Amount < m_AmountMax)
                        return false;
                }

                return true;
            }
            set
            {
                if (value)
                {
                    for (int i = 0; i < m_Entries.Length; ++i)
                    {
                        m_Entries[i].Amount = m_AmountMax;
                    }
                }
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1045151;
            }
        }// a bulk order deed
        public static BulkMaterialType GetRandomMaterial(BulkMaterialType start, double[] chances)
        {
            double random = Utility.RandomDouble();

            for (int i = 0; i < chances.Length; ++i)
            {
                if (random < chances[i])
                    return (i == 0 ? BulkMaterialType.None : start + (i - 1));

                random -= chances[i];
            }

            return BulkMaterialType.None;
        }

        public abstract List<Item> ComputeRewards(bool full);

        public abstract int ComputeGold();

        public abstract int ComputeFame();

        public virtual void GetRewards(out Item reward, out int gold, out int fame)
        {
            reward = null;
            gold = ComputeGold();
            fame = ComputeFame();

            if (!BulkOrderSystem.NewSystemEnabled)
            {
                List<Item> rewards = ComputeRewards(false);

                if (rewards.Count > 0)
                {
                    reward = rewards[Utility.Random(rewards.Count)];

                    for (int i = 0; i < rewards.Count; ++i)
                    {
                        if (rewards[i] != reward)
                            rewards[i].Delete();
                    }
                }
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1060655); // large bulk order

            if (m_RequireExceptional)
                list.Add(1045141); // All items must be exceptional.

            if (m_Material != BulkMaterialType.None)
                list.Add(SmallBODGump.GetMaterialNumberFor(m_Material)); // All items must be made with x material.

            list.Add(1060656, m_AmountMax.ToString()); // amount to make: ~1_val~

            for (int i = 0; i < m_Entries.Length; ++i)
                list.Add(1060658 + i, "#{0}\t{1}", m_Entries[i].Details.Number, m_Entries[i].Amount); // ~1_val~: ~2_val~
        }

        public override void OnDoubleClickNotAccessible(Mobile from)
        {
            OnDoubleClick(from);
        }

        public override void OnDoubleClickSecureTrade(Mobile from)
        {
            OnDoubleClick(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack) || InSecureTrade || RootParent is PlayerVendor)
			{
				EventSink.InvokeBODUsed(new BODUsedEventArgs(from, this));
				from.SendGump(new LargeBODGump(from, this));
			}
			else
			{
				from.SendLocalizedMessage(1045156); // You must have the deed in your backpack to use it.
			}
		}

        public void BeginCombine(Mobile from)
        {
            if (!Complete)
                from.Target = new LargeBODTarget(this);
            else
                from.SendLocalizedMessage(1045166); // The maximum amount of requested items have already been combined to this deed.
        }

        public void EndCombine(Mobile from, object o)
        {
            if (o is Item && ((Item)o).IsChildOf(from.Backpack))
            {
                if (o is SmallBOD)
                {
                    SmallBOD small = (SmallBOD)o;

                    LargeBulkEntry entry = null;

                    for (int i = 0; entry == null && i < m_Entries.Length; ++i)
                    {
                        if (small.CheckType(m_Entries[i].Details.Type))
                            entry = m_Entries[i];
                    }

                    if (entry == null)
                    {
                        from.SendLocalizedMessage(1045160); // That is not a bulk order for this large request.
                    }
                    else if (m_RequireExceptional && !small.RequireExceptional)
                    {
                        from.SendLocalizedMessage(1045161); // Both orders must be of exceptional quality.
                    }
                    else if (small.Material != m_Material && m_Material != BulkMaterialType.None)
                    {
                        from.SendLocalizedMessage(1157311); // Both orders must use the same resource type.
                    }
                    else if (m_AmountMax != small.AmountMax)
                    {
                        from.SendLocalizedMessage(1045163); // The two orders have different requested amounts and cannot be combined.
                    }
                    else if (small.AmountCur < small.AmountMax)
                    {
                        from.SendLocalizedMessage(1045164); // The order to combine with is not completed.
                    }
                    else if (entry.Amount >= m_AmountMax)
                    {
                        from.SendLocalizedMessage(1045166); // The maximum amount of requested items have already been combined to this deed.
                    }
                    else
                    {
                        entry.Amount += small.AmountCur;
                        small.Delete();

                        from.SendLocalizedMessage(1045165); // The orders have been combined.

                        from.SendGump(new LargeBODGump(from, this));

                        if (!Complete)
                            BeginCombine(from);
                    }
                }
                else
                {
                    from.SendLocalizedMessage(1045159); // That is not a bulk order.
                }
            }
            else
            {
                from.SendLocalizedMessage(1045158); // You must have the item in your backpack to target it.
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            writer.Write(m_AmountMax);
            writer.Write(m_RequireExceptional);
            writer.Write((int)m_Material);

            writer.Write((int)m_Entries.Length);

            for (int i = 0; i < m_Entries.Length; ++i)
                m_Entries[i].Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 1:
                case 0:
                    {
                        m_AmountMax = reader.ReadInt();
                        m_RequireExceptional = reader.ReadBool();
                        m_Material = (BulkMaterialType)reader.ReadInt();

                        m_Entries = new LargeBulkEntry[reader.ReadInt()];

                        for (int i = 0; i < m_Entries.Length; ++i)
                            m_Entries[i] = new LargeBulkEntry(this, reader, version);
                        break;
                    }
            }

            if (Weight == 0.0)
                Weight = 1.0;

            if (Core.AOS && ItemID == 0x14EF)
                ItemID = 0x2258;

            if (Parent == null && Map == Map.Internal && Location == Point3D.Zero)
                Delete();
        }
    }
}
