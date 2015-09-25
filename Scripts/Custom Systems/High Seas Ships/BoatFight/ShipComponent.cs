using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Engines.XmlSpawner2;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Items
{
    public class ShipComponent : AddonComponent
    {
        public ShipComponent(int itemID)
            : base(itemID)
        {
        }

        public ShipComponent(int itemID, string name)
            : base(itemID)
        {
            Name = name;
        }

        public ShipComponent(Serial serial)
            : base(serial)
        {
        }

        public override bool ForceShowProperties
        {
            get
            {
                return true;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsDraggable
        {
            get
            {
                if (Addon is IShipWeapon)
                {
                    return ((IShipWeapon)Addon).IsDraggable;
                }
                return false;
            }
            set
            {
                if (Addon is IShipWeapon)
                {
                    ((IShipWeapon)Addon).IsDraggable = value;
                }
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsPackable
        {
            get
            {
                if (Addon is IShipWeapon)
                {
                    return ((IShipWeapon)Addon).IsPackable;
                }
                return false;
            }
            set
            {
                if (Addon is IShipWeapon)
                {
                    ((IShipWeapon)Addon).IsPackable = value;
                }
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool FixedFacing
        {
            get
            {
                if (Addon is IShipWeapon)
                {
                    return ((IShipWeapon)Addon).FixedFacing;
                }
                return false;
            }
            set
            {
                if (Addon is IShipWeapon)
                {
                    ((IShipWeapon)Addon).FixedFacing = value;
                }
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int Facing
        {
            get
            {
                if (Addon is IShipWeapon)
                {
                    return ((IShipWeapon)Addon).Facing;
                }
                return 0;
            }
            set
            {
                if (Addon is IShipWeapon)
                {
                    ((IShipWeapon)Addon).Facing = value;
                }
            }
        }
        public override void OnDoubleClick(Mobile from)
        {
            if (Addon != null)
            {
                Addon.OnDoubleClick(from);
            }
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            if (Addon is IShipWeapon)
            {
                IShipWeapon weapon = (IShipWeapon)Addon;

                if (!weapon.FixedFacing)
                {
                    list.Add(new RotateNextEntry(weapon));
                    list.Add(new RotatePreviousEntry(weapon));
                }

                if (weapon.IsPackable)
                {
                    list.Add(new BackpackEntry(from, weapon));
                }

                if (weapon.IsDraggable)
                {
                    // does it support dragging?
                    XmlDrag a = (XmlDrag)XmlAttach.FindAttachment(weapon, typeof(XmlDrag));
                    if (a != null)
                    {
                        // is it currently being dragged?
                        if (a.DraggedBy != null && !a.DraggedBy.Deleted)
                        {
                            list.Add(new ReleaseEntry(from, a));
                        }
                        else
                        {
                            list.Add(new ConnectEntry(from, a));
                        }
                    }
                }
            }
            base.GetContextMenuEntries(from, list);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            IShipWeapon weapon = Addon as IShipWeapon;

            if (weapon == null)
                return;

            if (weapon.Projectile == null || weapon.Projectile.Deleted)
            {
                //list.Add(1061169, "empty"); // range ~1_val~
                list.Add(1042975); // It's empty
            }
            else
            {
                list.Add(500767); // Reloaded
                list.Add(1060658, "Type\t{0}", weapon.Projectile.Name); // ~1_val~: ~2_val~

                ISiegeProjectile projectile = weapon.Projectile as ISiegeProjectile;
                if (projectile != null)
                {
                    list.Add(1061169, projectile.Range.ToString()); // range ~1_val~
                }
            }
        }

        public override void OnMapChange()
        {
            if (Addon != null && Map != Map.Internal)
                Addon.Map = Map;
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

        private class RotateNextEntry : ContextMenuEntry
        {
            private readonly IShipWeapon _weapon;
            public RotateNextEntry(IShipWeapon weapon)
                : base(406)
            {
                _weapon = weapon;
            }

            public override void OnClick()
            {
                if (_weapon != null)
                    _weapon.Facing++;
            }
        }

        private class RotatePreviousEntry : ContextMenuEntry
        {
            private readonly IShipWeapon _weapon;
            public RotatePreviousEntry(IShipWeapon weapon)
                : base(405)
            {
                _weapon = weapon;
            }

            public override void OnClick()
            {
                if (_weapon != null)
                    _weapon.Facing--;
            }
        }

        private class BackpackEntry : ContextMenuEntry
        {
            private readonly IShipWeapon _weapon;
            private readonly Mobile m_from;
            public BackpackEntry(Mobile from, IShipWeapon weapon)
                : base(2139)
            {
                _weapon = weapon;
                m_from = from;
            }

            public override void OnClick()
            {
                if (_weapon != null)
                {
                    _weapon.StoreWeapon(m_from);
                }
            }
        }

        private class ReleaseEntry : ContextMenuEntry
        {
            private readonly Mobile m_from;
            private readonly XmlDrag m_drag;
            public ReleaseEntry(Mobile from, XmlDrag drag)
                : base(6118)
            {
                m_from = from;
                m_drag = drag;
            }

            public override void OnClick()
            {
                if (m_drag == null)
                    return;

                BaseCreature pet = m_drag.DraggedBy as BaseCreature;

                // only allow the person dragging it or their pet to release
                if (m_drag.DraggedBy == m_from || (pet != null && (pet.ControlMaster == m_from || pet.ControlMaster == null)))
                {
                    m_drag.DraggedBy = null;
                }
            }
        }

        private class ConnectEntry : ContextMenuEntry
        {
            private readonly Mobile m_from;
            private readonly XmlDrag m_drag;
            public ConnectEntry(Mobile from, XmlDrag drag)
                : base(5119)
            {
                m_from = from;
                m_drag = drag;
            }

            public override void OnClick()
            {
                if (m_drag != null && m_from != null)
                {
                    m_from.SendMessage("Select a mobile to drag the weapon");
                    m_from.Target = new DragTarget(m_drag);
                }
            }
        }

        private class SetupEntry : ContextMenuEntry
        {
            private readonly IShipWeapon _weapon;
            private readonly Mobile m_from;
            public SetupEntry(Mobile from, IShipWeapon weapon)
                : base(97)
            {
                _weapon = weapon;
                m_from = from;
            }

            public override void OnClick()
            {
                if (_weapon != null && m_from != null)
                {
                    _weapon.PlaceWeapon(m_from, m_from.Location, m_from.Map);
                }
            }
        }

        private class DragTarget : Target
        {
            private readonly XmlDrag m_attachment;
            public DragTarget(XmlDrag attachment)
                : base(30, false, TargetFlags.None)
            {
                m_attachment = attachment;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_attachment == null || from == null)
                    return;

                if (!(targeted is Mobile))
                {
                    from.SendMessage("Must target a mobile");
                    return;
                }

                Mobile m = (Mobile)targeted;

                if (m == from || (m is BaseCreature && (((BaseCreature)m).Controlled && ((BaseCreature)m).ControlMaster == from)))
                {
                    m_attachment.DraggedBy = m;
                }
                else
                {
                    from.SendMessage("You dont control that.");
                }
            }
        }
    }
}