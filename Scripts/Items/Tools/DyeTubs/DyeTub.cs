using Server.ContextMenus;
using Server.Gumps;
using Server.Multis;
using Server.Targeting;
using System.Collections.Generic;

using System;
using System.Linq;

namespace Server.Items
{
    public interface IDyable
    {
        bool Dye(Mobile from, DyeTub sender);
    }

    public class DyeTub : Item, ISecurable
    {
        private bool m_Redyable;
        private int m_DyedHue;
        private SecureLevel m_SecureLevel;

        [Constructable]
        public DyeTub()
            : base(0xFAB)
        {
            Weight = 10.0;
            m_Redyable = true;
        }

        public DyeTub(Serial serial)
            : base(serial)
        {
        }

        public virtual CustomHuePicker CustomHuePicker => null;
        public virtual bool AllowRunebooks => false;
        public virtual bool AllowFurniture => false;
        public virtual bool AllowStatuettes => false;
        public virtual bool AllowLeather => false;
        public virtual bool AllowDyables => true;
        public virtual bool AllowMetal => false;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Redyable
        {
            get { return m_Redyable; }
            set { m_Redyable = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int DyedHue
        {
            get { return m_DyedHue; }
            set
            {
                if (m_Redyable)
                {
                    m_DyedHue = value;
                    Hue = value;
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level
        {
            get { return m_SecureLevel; }
            set { m_SecureLevel = value; }
        }

        public virtual int TargetMessage => 500859;  // Select the clothing to dye.        
        public virtual int FailMessage => 1042083;  // You can not dye that.

        public virtual Type[] ForcedDyables => new Type[0];

        public virtual bool CanForceDye(Item item)
        {
            return ForcedDyables != null && ForcedDyables.Any(t => t == item.GetType());
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1); // version

            writer.Write((int)m_SecureLevel);
            writer.Write(m_Redyable);
            writer.Write(m_DyedHue);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
			
			m_SecureLevel = (SecureLevel)reader.ReadInt();
			m_Redyable = reader.ReadBool();
            m_DyedHue = reader.ReadInt();
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);
            SetSecureLevelEntry.AddTo(from, this, list);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(GetWorldLocation(), 1))
            {
                from.SendLocalizedMessage(TargetMessage);
                from.Target = new InternalTarget(this);
            }
            else
            {
                from.SendLocalizedMessage(500446); // That is too far away.
            }
        }

        private class InternalTarget : Target
        {
            private readonly DyeTub m_Tub;

            public InternalTarget(DyeTub tub)
                : base(1, false, TargetFlags.None)
            {
                m_Tub = tub;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is Item)
                {
                    Item item = (Item)targeted;

                    if (item is IDyable && m_Tub.AllowDyables)
                    {
                        if (!from.InRange(m_Tub.GetWorldLocation(), 1) || !from.InRange(item.GetWorldLocation(), 1))
                            from.SendLocalizedMessage(500446); // That is too far away.
						else if (item.IsLockedDown)
                            from.SendLocalizedMessage(1061637); // You are not allowed to access this.
                        else if (item.Parent is Mobile)
                            from.SendLocalizedMessage(500861); // Can't Dye clothing that is being worn.
                        else if (((IDyable)item).Dye(from, m_Tub))
                            from.PlaySound(0x23E);
                    }
                    else if (m_Tub.AllowFurniture && (FurnitureAttribute.Check(item) || m_Tub.CanForceDye(item)))
                    {
                        if (!from.InRange(m_Tub.GetWorldLocation(), 1) || !from.InRange(item.GetWorldLocation(), 1))
                        {
                            from.SendLocalizedMessage(500446); // That is too far away.
                        }
                        else
                        {
                            bool okay = (item.IsChildOf(from.Backpack));

                            if (!okay)
                            {
                                if (item.Parent == null)
                                {
                                    BaseHouse house = BaseHouse.FindHouseAt(item);

                                    if (!house.IsCoOwner(from))
                                        from.SendLocalizedMessage(501023); // You must be the owner to use this item.
                                    else if (house == null || (!house.IsLockedDown(item) && !house.IsSecure(item)) && (!(item is AddonComponent) || !house.Addons.ContainsKey(((AddonComponent)item).Addon)))
                                        from.SendLocalizedMessage(501022); // Furniture must be locked down to paint it.
                                    else
                                        okay = true;
                                }
                                else
                                {
                                    from.SendLocalizedMessage(1048135); // The furniture must be in your backpack to be painted.
                                }
                            }

                            if (okay)
                            {
                                item.Hue = m_Tub.DyedHue;
                                from.PlaySound(0x23E);
                            }
                        }
                    }
                    else if (m_Tub.AllowRunebooks && (item is Runebook || item is RecallRune || m_Tub.CanForceDye(item)))
                    {
                        if (!from.InRange(m_Tub.GetWorldLocation(), 1) || !from.InRange(item.GetWorldLocation(), 1))
                        {
                            from.SendLocalizedMessage(500446); // That is too far away.
                        }
                        else if (!item.Movable)
                        {
                            from.SendLocalizedMessage(1049776); // You cannot dye runes or runebooks that are locked down.
                        }
                        else
                        {
                            item.Hue = m_Tub.DyedHue;
                            from.PlaySound(0x23E);
                        }
                    }
                    else if (m_Tub.AllowStatuettes && (item is MonsterStatuette || m_Tub.CanForceDye(item)))
                    {
                        if (!from.InRange(m_Tub.GetWorldLocation(), 1) || !from.InRange(item.GetWorldLocation(), 1))
                        {
                            from.SendLocalizedMessage(500446); // That is too far away.
                        }
                        else if (!item.Movable)
                        {
                            from.SendLocalizedMessage(1049779); // You cannot dye statuettes that are locked down.
                        }
                        else
                        {
                            item.Hue = m_Tub.DyedHue;
                            from.PlaySound(0x23E);
                        }
                    }
                    else if (m_Tub.AllowLeather)
                    {
                        var armor = item as BaseArmor;
                        var clothing = item as BaseClothing;

                        if ((armor != null && (armor.MaterialType == ArmorMaterialType.Leather ||
                            armor.MaterialType == ArmorMaterialType.Studded)) ||
                            (clothing != null && (clothing.DefaultResource == CraftResource.RegularLeather)) ||
                            m_Tub.CanForceDye(item))
                        {
                            if (!from.InRange(m_Tub.GetWorldLocation(), 1) || !from.InRange(item.GetWorldLocation(), 1))
                            {
                                from.SendLocalizedMessage(500446); // That is too far away.
                            }
                            else if (!item.Movable)
                            {
                                from.SendLocalizedMessage(1042419); // You may not dye leather items which are locked down.
                            }
                            else if (item.Parent is Mobile)
                            {
                                from.SendLocalizedMessage(500861); // Can't Dye clothing that is being worn.
                            }
                            else
                            {
                                item.Hue = m_Tub.DyedHue;
                                from.PlaySound(0x23E);
                            }
                        }
                        else
                        {
                            from.SendLocalizedMessage(m_Tub.FailMessage);
                        }
                    }
                    else if (m_Tub.AllowMetal)
                    {
                        var armor = item as BaseArmor;

                        if ((armor != null && armor.MaterialType >= ArmorMaterialType.Ringmail && armor.MaterialType <= ArmorMaterialType.Plate) || m_Tub.CanForceDye(item))
                        {
                            if (!from.InRange(m_Tub.GetWorldLocation(), 1) || !from.InRange(item.GetWorldLocation(), 1))
                            {
                                from.SendLocalizedMessage(500446); // That is too far away.
                            }
                            else if (!item.Movable)
                            {
                                from.SendLocalizedMessage(1080395); // You may not dye metal items which are locked down.
                            }
                            else if (item.Parent is Mobile)
                            {
                                from.SendLocalizedMessage(500861); // Can't Dye clothing that is being worn.
                            }
                            else
                            {
                                item.Hue = m_Tub.DyedHue;
                                from.PlaySound(0x23E);
                            }
                        }
                        else
                        {
                            from.SendLocalizedMessage(m_Tub.FailMessage);
                        }
                    }
                    else
                    {
                        from.SendLocalizedMessage(m_Tub.FailMessage);
                    }
                }
                else
                {
                    from.SendLocalizedMessage(m_Tub.FailMessage);
                }
            }
        }
    }
}
