using Server.Multis;
using Server.Targeting;
using System;
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
                return m_UsesRemaining;
            }
            set
            {
                m_UsesRemaining = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool BooksOnly
        {
            get
            {
                return m_BooksOnly;
            }
            set
            {
                m_BooksOnly = value;
                InvalidateProperties();
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
            if (HueInfo.ContainsKey(DyeType))
            {
                Hue = HueInfo[DyeType].Item1;
            }
        }

        public SpecialNaturalDye(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1112136;

        public override bool ForceShowProperties => true;

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1060584, m_UsesRemaining.ToString()); // uses remaining: ~1_val~

            if (m_BooksOnly)
                list.Add(1157205); // Spellbook Only Dye
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            if (DyeType == DyeType.None)
            {
                base.AddNameProperty(list);
            }
            else if (Amount > 1)
            {
                list.Add(1113276, "{0}\t{1}", Amount, string.Format("#{0}", HueInfo[DyeType].Item2));  // ~1_AMOUNT~ ~2_COLOR~ natural dyes
            }
            else
            {
                list.Add(1112137, string.Format("#{0}", HueInfo[DyeType].Item2));  // ~1_COLOR~ natural dye
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version

            writer.Write((int)m_DyeType);
            writer.Write(m_UsesRemaining);
            writer.Write(m_BooksOnly);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_DyeType = (DyeType)reader.ReadInt();
            m_UsesRemaining = reader.ReadInt();
            m_BooksOnly = reader.ReadBool();
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
                m_Item = item;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Item.Deleted)
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
                                      item is BaseTalisman || item is Spellbook ||
                                      item.IsArtifact || BasePigmentsOfTokuno.IsValidItem(item));

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
                            if (!from.InRange(m_Item.GetWorldLocation(), 1) || !from.InRange(item.GetWorldLocation(), 1))
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

                        if (--m_Item.UsesRemaining > 0)
                            m_Item.InvalidateProperties();
                        else
                            m_Item.Delete();

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
