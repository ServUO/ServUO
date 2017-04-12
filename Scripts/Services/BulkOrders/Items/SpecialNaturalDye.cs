using System;
using Server.Engines.Plants;
using Server.Multis;
using Server.Targeting;
using System.Collections.Generic;

namespace Server.Items
{
    public enum DyeType
    {
        None,
        WindAzul,
        DullRuby,
        PoppieWhite,
        ZentoOrchid,
        UmbranViolet
    }

    public class SpecialNaturalDye : Item
    {
        private DyeType m_DyeType;
        private int m_UsesRemaining;
        private bool m_BooksOnly;

        [CommandProperty(AccessLevel.GameMaster)]
        public DyeType DyeType
        {
            get { return m_DyeType; }
            set
            {
                DyeType old = m_DyeType;
                m_DyeType = value;

                if (m_DyeType != old)
                    ValidateHue();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int UsesRemaining
        {
            get
            {
                return this.m_UsesRemaining;
            }
            set
            {
                this.m_UsesRemaining = value;
                this.InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool BooksOnly
        {
            get
            {
                return this.m_BooksOnly;
            }
            set
            {
                this.m_BooksOnly = value;
                this.InvalidateProperties();
            }
        }

        [Constructable]
        public SpecialNaturalDye(DyeType type)
            : this(type, false)
        {
        }

        [Constructable]
        public SpecialNaturalDye(DyeType type, bool booksonly)
            : base(0x182B)
        {
            Weight = 1.0;
            DyeType = type;
            UsesRemaining = 5;

            BooksOnly = booksonly;
        }

        public void ValidateHue()
        {
            if (HueInfo.ContainsKey(this.DyeType))
            {
                Hue = HueInfo[this.DyeType].Item1;
            }
        }

        public SpecialNaturalDye(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1112136;
            }
        }

        public override bool ForceShowProperties
        {
            get
            {
                return ObjectPropertyList.Enabled;
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1060584, this.m_UsesRemaining.ToString()); // uses remaining: ~1_val~

            if (m_BooksOnly)
                list.Add(1157205); // Spellbook Only Dye
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            if (this.DyeType == DyeType.None)
            {
                base.AddNameProperty(list);
            }
            else if (this.Amount > 1)
            {
                list.Add(1113276, "{0}\t{1}", this.Amount, String.Format("#{0}", HueInfo[this.DyeType].Item2));  // ~1_AMOUNT~ ~2_COLOR~ natural dyes
            }
            else
            {
                list.Add(1112137, String.Format("#{0}", HueInfo[this.DyeType].Item2));  // ~1_COLOR~ natural dye
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write((int)this.m_DyeType);
            writer.Write((int)this.m_UsesRemaining);
            writer.Write(m_BooksOnly);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            this.m_DyeType = (DyeType)reader.ReadInt();
            this.m_UsesRemaining = reader.ReadInt();
            this.m_BooksOnly = reader.ReadBool();
        }

        public override void OnDoubleClick(Mobile from)
        {
            from.SendLocalizedMessage(1112139); // Select the item you wish to dye.
            from.Target = new InternalTarget(this);
        }

        private class InternalTarget : Target
        {
            private readonly SpecialNaturalDye m_Item;

            public InternalTarget(SpecialNaturalDye item)
                : base(1, false, TargetFlags.None)
            {
                this.m_Item = item;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (this.m_Item.Deleted)
                    return;

                Item item = targeted as Item;
                bool valid = false;

                if (item != null)
                {
                    if (m_Item.BooksOnly && !(item is Spellbook))
                    {
                        valid = false;
                    }
                    else
                    {
                        valid = (item is IDyable ||
                                      item is BaseBook || item is BaseClothing ||
                                      item is BaseJewel || item is BaseStatuette ||
                                      item is BaseWeapon || item is Runebook ||
                                      item is Spellbook);

                        if (!valid && item is BaseArmor)
                        {
                            CraftResourceType restype = CraftResources.GetType(((BaseArmor)item).Resource);
                            if ((CraftResourceType.Leather == restype || CraftResourceType.Metal == restype) &&
                                ArmorMaterialType.Bone != ((BaseArmor)item).MaterialType)
                            {
                                valid = true;
                            }
                        }

                        if (!valid && FurnitureAttribute.Check(item))
                        {
                            if (!from.InRange(this.m_Item.GetWorldLocation(), 1) || !from.InRange(item.GetWorldLocation(), 1))
                            {
                                from.SendLocalizedMessage(500446); // That is too far away.
                                return;
                            }
                            else
                            {
                                BaseHouse house = BaseHouse.FindHouseAt(item);

                                if (house == null || (!house.IsLockedDown(item) && !house.IsSecure(item)))
                                {
                                    from.SendLocalizedMessage(501022); // Furniture must be locked down to paint it.
                                    return;
                                }
                                else if (!house.IsCoOwner(from))
                                {
                                    from.SendLocalizedMessage(501023); // You must be the owner to use this item.
                                    return;
                                }
                                else
                                    valid = true;
                            }
                        }
                    }

                    if (valid)
                    {
                        item.Hue = m_Item.Hue;
                        from.PlaySound(0x23E);

                        if (--this.m_Item.UsesRemaining > 0)
                            this.m_Item.InvalidateProperties();
                        else
                            this.m_Item.Delete();

                        return;
                    }
                }

                from.SendLocalizedMessage(1042083); // You cannot dye that.
            }
        }

        public static Dictionary<DyeType, Tuple<int, int>> HueInfo;

        public static void Configure()
        {
            HueInfo = new Dictionary<DyeType, Tuple<int, int>>();

            HueInfo[DyeType.WindAzul] = new Tuple<int, int>(2741, 1157277);
            HueInfo[DyeType.DullRuby] = new Tuple<int, int>(2731, 1157267);
            HueInfo[DyeType.PoppieWhite] = new Tuple<int, int>(2735, 1157271);
            HueInfo[DyeType.ZentoOrchid] = new Tuple<int, int>(2732, 1157268);
            HueInfo[DyeType.UmbranViolet] = new Tuple<int, int>(2740, 1157276);
        }
    }
}