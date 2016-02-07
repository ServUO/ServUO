using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Engines.BulkOrders
{
    [TypeAlias("Scripts.Engines.BulkOrders.LargeBOD")]
    public abstract class LargeBOD : Item
    {
        private int m_AmountMax;
        private bool m_RequireExceptional;
        private BulkMaterialType m_Material;
        private LargeBulkEntry[] m_Entries;
        public LargeBOD(int hue, int amountMax, bool requireExeptional, BulkMaterialType material, LargeBulkEntry[] entries)
            : base(Core.AOS ? 0x2258 : 0x14EF)
        {
            this.Weight = 1.0;
            this.Hue = hue; // Blacksmith: 0x44E; Tailoring: 0x483
            this.LootType = LootType.Blessed;

            this.m_AmountMax = amountMax;
            this.m_RequireExceptional = requireExeptional;
            this.m_Material = material;
            this.m_Entries = entries;
        }

        public LargeBOD()
            : base(Core.AOS ? 0x2258 : 0x14EF)
        {
            this.Weight = 1.0;
            this.LootType = LootType.Blessed;
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
                return this.m_AmountMax;
            }
            set
            {
                this.m_AmountMax = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool RequireExceptional
        {
            get
            {
                return this.m_RequireExceptional;
            }
            set
            {
                this.m_RequireExceptional = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public BulkMaterialType Material
        {
            get
            {
                return this.m_Material;
            }
            set
            {
                this.m_Material = value;
                this.InvalidateProperties();
            }
        }
        public LargeBulkEntry[] Entries
        {
            get
            {
                return this.m_Entries;
            }
            set
            {
                this.m_Entries = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Complete
        {
            get
            {
                for (int i = 0; i < this.m_Entries.Length; ++i)
                {
                    if (this.m_Entries[i].Amount < this.m_AmountMax)
                        return false;
                }

                return true;
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
            gold = this.ComputeGold();
            fame = this.ComputeFame();

            List<Item> rewards = this.ComputeRewards(false);

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

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1060655); // large bulk order

            if (this.m_RequireExceptional)
                list.Add(1045141); // All items must be exceptional.

            if (this.m_Material != BulkMaterialType.None)
                list.Add(LargeBODGump.GetMaterialNumberFor(this.m_Material)); // All items must be made with x material.

            list.Add(1060656, this.m_AmountMax.ToString()); // amount to make: ~1_val~

            for (int i = 0; i < this.m_Entries.Length; ++i)
                list.Add(1060658 + i, "#{0}\t{1}", this.m_Entries[i].Details.Number, this.m_Entries[i].Amount); // ~1_val~: ~2_val~
        }

        public override void OnDoubleClickNotAccessible(Mobile from)
        {
            this.OnDoubleClick(from);
        }

        public override void OnDoubleClickSecureTrade(Mobile from)
        {
            this.OnDoubleClick(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (this.IsChildOf(from.Backpack) || this.InSecureTrade || this.RootParent is PlayerVendor)
                from.SendGump(new LargeBODGump(from, this));
            else
                from.SendLocalizedMessage(1045156); // You must have the deed in your backpack to use it.
        }

        public void BeginCombine(Mobile from)
        {
            if (!this.Complete)
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

                    for (int i = 0; entry == null && i < this.m_Entries.Length; ++i)
                    {
                        if (this.m_Entries[i].Details.Type == small.Type)
                            entry = this.m_Entries[i];
                    }

                    if (entry == null)
                    {
                        from.SendLocalizedMessage(1045160); // That is not a bulk order for this large request.
                    }
                    else if (this.m_RequireExceptional && !small.RequireExceptional)
                    {
                        from.SendLocalizedMessage(1045161); // Both orders must be of exceptional quality.
                    }
                    else if (this.m_Material >= BulkMaterialType.DullCopper && this.m_Material <= BulkMaterialType.Valorite && small.Material != this.m_Material)
                    {
                        from.SendLocalizedMessage(1045162); // Both orders must use the same ore type.
                    }
                    else if (this.m_Material >= BulkMaterialType.Spined && this.m_Material <= BulkMaterialType.Barbed && small.Material != this.m_Material)
                    {
                        from.SendLocalizedMessage(1049351); // Both orders must use the same leather type.
                    }
                    else if (this.m_AmountMax != small.AmountMax)
                    {
                        from.SendLocalizedMessage(1045163); // The two orders have different requested amounts and cannot be combined.
                    }
                    else if (small.AmountCur < small.AmountMax)
                    {
                        from.SendLocalizedMessage(1045164); // The order to combine with is not completed.
                    }
                    else if (entry.Amount >= this.m_AmountMax)
                    {
                        from.SendLocalizedMessage(1045166); // The maximum amount of requested items have already been combined to this deed.
                    }
                    else
                    {
                        entry.Amount += small.AmountCur;
                        small.Delete();

                        from.SendLocalizedMessage(1045165); // The orders have been combined.

                        from.SendGump(new LargeBODGump(from, this));

                        if (!this.Complete)
                            this.BeginCombine(from);
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

            writer.Write((int)0); // version

            writer.Write(this.m_AmountMax);
            writer.Write(this.m_RequireExceptional);
            writer.Write((int)this.m_Material);

            writer.Write((int)this.m_Entries.Length);

            for (int i = 0; i < this.m_Entries.Length; ++i)
                this.m_Entries[i].Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        this.m_AmountMax = reader.ReadInt();
                        this.m_RequireExceptional = reader.ReadBool();
                        this.m_Material = (BulkMaterialType)reader.ReadInt();

                        this.m_Entries = new LargeBulkEntry[reader.ReadInt()];

                        for (int i = 0; i < this.m_Entries.Length; ++i)
                            this.m_Entries[i] = new LargeBulkEntry(this, reader);

                        break;
                    }
            }

            if (this.Weight == 0.0)
                this.Weight = 1.0;

            if (Core.AOS && this.ItemID == 0x14EF)
                this.ItemID = 0x2258;

            if (this.Parent == null && this.Map == Map.Internal && this.Location == Point3D.Zero)
                this.Delete();
        }
    }
}