using System;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.BulkOrders
{
    [TypeAlias("Scripts.Engines.BulkOrders.SmallBOD")]
    public abstract class SmallBOD : Item
    {
        private int m_AmountCur, m_AmountMax;
        private Type m_Type;
        private int m_Number;
        private int m_Graphic;
        private bool m_RequireExceptional;
        private BulkMaterialType m_Material;
        [Constructable]
        public SmallBOD(int hue, int amountMax, Type type, int number, int graphic, bool requireExeptional, BulkMaterialType material)
            : base(Core.AOS ? 0x2258 : 0x14EF)
        {
            this.Weight = 1.0;
            this.Hue = hue; // Blacksmith: 0x44E; Tailoring: 0x483
            this.LootType = LootType.Blessed;

            this.m_AmountMax = amountMax;
            this.m_Type = type;
            this.m_Number = number;
            this.m_Graphic = graphic;
            this.m_RequireExceptional = requireExeptional;
            this.m_Material = material;
        }

        public SmallBOD()
            : base(Core.AOS ? 0x2258 : 0x14EF)
        {
            this.Weight = 1.0;
            this.LootType = LootType.Blessed;
        }

        public SmallBOD(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int AmountCur
        {
            get
            {
                return this.m_AmountCur;
            }
            set
            {
                this.m_AmountCur = value;
                this.InvalidateProperties();
            }
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
        public Type Type
        {
            get
            {
                return this.m_Type;
            }
            set
            {
                this.m_Type = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int Number
        {
            get
            {
                return this.m_Number;
            }
            set
            {
                this.m_Number = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int Graphic
        {
            get
            {
                return this.m_Graphic;
            }
            set
            {
                this.m_Graphic = value;
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
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Complete
        {
            get
            {
                return (this.m_AmountCur == this.m_AmountMax);
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

        public static BulkMaterialType GetMaterial(CraftResource resource)
        {
            switch ( resource )
            {
                case CraftResource.DullCopper:
                    return BulkMaterialType.DullCopper;
                case CraftResource.ShadowIron:
                    return BulkMaterialType.ShadowIron;
                case CraftResource.Copper:
                    return BulkMaterialType.Copper;
                case CraftResource.Bronze:
                    return BulkMaterialType.Bronze;
                case CraftResource.Gold:
                    return BulkMaterialType.Gold;
                case CraftResource.Agapite:
                    return BulkMaterialType.Agapite;
                case CraftResource.Verite:
                    return BulkMaterialType.Verite;
                case CraftResource.Valorite:
                    return BulkMaterialType.Valorite;
                case CraftResource.SpinedLeather:
                    return BulkMaterialType.Spined;
                case CraftResource.HornedLeather:
                    return BulkMaterialType.Horned;
                case CraftResource.BarbedLeather:
                    return BulkMaterialType.Barbed;
            }

            return BulkMaterialType.None;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1060654); // small bulk order

            if (this.m_RequireExceptional)
                list.Add(1045141); // All items must be exceptional.

            if (this.m_Material != BulkMaterialType.None)
                list.Add(SmallBODGump.GetMaterialNumberFor(this.m_Material)); // All items must be made with x material.

            list.Add(1060656, this.m_AmountMax.ToString()); // amount to make: ~1_val~
            list.Add(1060658, "#{0}\t{1}", this.m_Number, this.m_AmountCur); // ~1_val~: ~2_val~
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (this.IsChildOf(from.Backpack) || this.InSecureTrade || this.RootParent is PlayerVendor)
                from.SendGump(new SmallBODGump(from, this));
            else
                from.SendLocalizedMessage(1045156); // You must have the deed in your backpack to use it.
        }

        public override void OnDoubleClickNotAccessible(Mobile from)
        {
            this.OnDoubleClick(from);
        }

        public override void OnDoubleClickSecureTrade(Mobile from)
        {
            this.OnDoubleClick(from);
        }

        public void BeginCombine(Mobile from)
        {
            if (this.m_AmountCur < this.m_AmountMax)
                from.Target = new SmallBODTarget(this);
            else
                from.SendLocalizedMessage(1045166); // The maximum amount of requested items have already been combined to this deed.
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

        public void EndCombine(Mobile from, object o)
        {
            if (o is Item && ((Item)o).IsChildOf(from.Backpack))
            {
                Type objectType = o.GetType();

                if (this.m_AmountCur >= this.m_AmountMax)
                {
                    from.SendLocalizedMessage(1045166); // The maximum amount of requested items have already been combined to this deed.
                }
                else if (this.m_Type == null || (objectType != this.m_Type && !objectType.IsSubclassOf(this.m_Type)) || (!(o is BaseWeapon) && !(o is BaseArmor) && !(o is BaseClothing)))
                {
                    from.SendLocalizedMessage(1045169); // The item is not in the request.
                }
                else
                {
                    BulkMaterialType material = BulkMaterialType.None;

                    if (o is BaseWeapon)
                        material = GetMaterial(((BaseWeapon)o).Resource);
                    else if (o is BaseArmor)
                        material = GetMaterial(((BaseArmor)o).Resource);
                    else if (o is BaseClothing)
                        material = GetMaterial(((BaseClothing)o).Resource);

                    if (this.m_Material >= BulkMaterialType.DullCopper && this.m_Material <= BulkMaterialType.Valorite && material != this.m_Material)
                    {
                        from.SendLocalizedMessage(1045168); // The item is not made from the requested ore.
                    }
                    else if (this.m_Material >= BulkMaterialType.Spined && this.m_Material <= BulkMaterialType.Barbed && material != this.m_Material)
                    {
                        from.SendLocalizedMessage(1049352); // The item is not made from the requested leather type.
                    }
                    else
                    {
                        bool isExceptional = false;

                        if (o is BaseWeapon)
                            isExceptional = (((BaseWeapon)o).Quality == WeaponQuality.Exceptional);
                        else if (o is BaseArmor)
                            isExceptional = (((BaseArmor)o).Quality == ArmorQuality.Exceptional);
                        else if (o is BaseClothing)
                            isExceptional = (((BaseClothing)o).Quality == ClothingQuality.Exceptional);

                        if (this.m_RequireExceptional && !isExceptional)
                        {
                            from.SendLocalizedMessage(1045167); // The item must be exceptional.
                        }
                        else
                        {
                            ((Item)o).Delete();
                            ++this.AmountCur;

                            from.SendLocalizedMessage(1045170); // The item has been combined with the deed.

                            from.SendGump(new SmallBODGump(from, this));

                            if (this.m_AmountCur < this.m_AmountMax)
                                this.BeginCombine(from);
                        }
                    }
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

            writer.Write(this.m_AmountCur);
            writer.Write(this.m_AmountMax);
            writer.Write(this.m_Type == null ? null : this.m_Type.FullName);
            writer.Write(this.m_Number);
            writer.Write(this.m_Graphic);
            writer.Write(this.m_RequireExceptional);
            writer.Write((int)this.m_Material);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        this.m_AmountCur = reader.ReadInt();
                        this.m_AmountMax = reader.ReadInt();

                        string type = reader.ReadString();

                        if (type != null)
                            this.m_Type = ScriptCompiler.FindTypeByFullName(type);

                        this.m_Number = reader.ReadInt();
                        this.m_Graphic = reader.ReadInt();
                        this.m_RequireExceptional = reader.ReadBool();
                        this.m_Material = (BulkMaterialType)reader.ReadInt();

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