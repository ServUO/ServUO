using System;
using Server.Engines.Plants;
using Server.Multis;
using Server.Targeting;

namespace Server.Items
{
    public class NaturalDye : Item, IPigmentHue
    {
        private PlantPigmentHue m_Hue;
        private int m_UsesRemaining;
        [Constructable]
        public NaturalDye()
            : this(PlantPigmentHue.None)
        {
        }

        [Constructable]
        public NaturalDye(PlantPigmentHue hue)
            : base(0x182B)
        {
            this.Weight = 1.0;
            this.PigmentHue = hue;
            this.m_UsesRemaining = 5;
        }

        public NaturalDye(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public PlantPigmentHue PigmentHue
        {
            get
            {
                return this.m_Hue;
            }
            set
            {
                this.m_Hue = value;
                // set any invalid pigment hue to Plain
                if (this.m_Hue != PlantPigmentHueInfo.GetInfo(this.m_Hue).PlantPigmentHue)
                    this.m_Hue = PlantPigmentHue.Plain;
                this.Hue = PlantPigmentHueInfo.GetInfo(this.m_Hue).Hue;
                this.InvalidateProperties();
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
        public override int LabelNumber
        {
            get
            {
                return 1112136;
            }
        }// natural dye
        public bool RetainsColorFrom
        {
            get
            {
                return true;
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
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            PlantPigmentHueInfo hueInfo = PlantPigmentHueInfo.GetInfo(this.m_Hue);
            
            if (this.Amount > 1)
                list.Add(PlantPigmentHueInfo.IsBright(m_Hue) ? 1113277 : 1113276, "{0}\t{1}", this.Amount, "#" + hueInfo.Name);  // ~1_COLOR~ Softened Reeds
            else
                list.Add(hueInfo.IsBright() ? 1112138 : 1112137, "#" + hueInfo.Name);  // ~1_COLOR~ natural dye
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            writer.Write((int)this.m_Hue);
            writer.Write((int)this.m_UsesRemaining);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 1:
                    this.m_Hue = (PlantPigmentHue)reader.ReadInt();
                    this.m_UsesRemaining = reader.ReadInt();
                    break;
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            from.SendLocalizedMessage(1112139); // Select the item you wish to dye.
            from.Target = new InternalTarget(this);
        }

        private class InternalTarget : Target
        {
            private readonly NaturalDye m_Item;
            public InternalTarget(NaturalDye item)
                : base(1, false, TargetFlags.None)
            {
                this.m_Item = item;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (this.m_Item.Deleted)
                    return;

                Item item = targeted as Item;
                if (null != item)
                {
                    bool valid = (item is IDyable ||
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

                    // need to add any bags, chests, boxes, crates not IDyable but dyable by natural dyes

                    if (valid)
                    {
                        item.Hue = PlantPigmentHueInfo.GetInfo(this.m_Item.PigmentHue).Hue;
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
    }
}