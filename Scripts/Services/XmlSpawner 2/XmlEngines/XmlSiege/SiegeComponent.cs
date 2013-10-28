using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Engines.XmlSpawner2;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Items
{
    public class SiegeComponent : AddonComponent
    {
        public SiegeComponent(int itemID)
            : base(itemID)
        {
        }

        public SiegeComponent(int itemID, string name)
            : base(itemID)
        {
            this.Name = name;
        }

        public SiegeComponent(Serial serial)
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
                if (this.Addon is ISiegeWeapon)
                {
                    return ((ISiegeWeapon)this.Addon).IsDraggable;
                }
                return false;
            }
            set
            {
                if (this.Addon is ISiegeWeapon)
                {
                    ((ISiegeWeapon)this.Addon).IsDraggable = value;
                }
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsPackable
        {
            get
            {
                if (this.Addon is ISiegeWeapon)
                {
                    return ((ISiegeWeapon)this.Addon).IsPackable;
                }
                return false;
            }
            set
            {
                if (this.Addon is ISiegeWeapon)
                {
                    ((ISiegeWeapon)this.Addon).IsPackable = value;
                }
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool FixedFacing
        {
            get
            {
                if (this.Addon is ISiegeWeapon)
                {
                    return ((ISiegeWeapon)this.Addon).FixedFacing;
                }
                return false;
            }
            set
            {
                if (this.Addon is ISiegeWeapon)
                {
                    ((ISiegeWeapon)this.Addon).FixedFacing = value;
                }
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int Facing
        {
            get
            {
                if (this.Addon is ISiegeWeapon)
                {
                    return ((ISiegeWeapon)this.Addon).Facing;
                }
                return 0;
            }
            set
            {
                if (this.Addon is ISiegeWeapon)
                {
                    ((ISiegeWeapon)this.Addon).Facing = value;
                }
            }
        }
        public override void OnDoubleClick(Mobile from)
        {
            if (this.Addon != null)
            {
                this.Addon.OnDoubleClick(from);
            }
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            if (this.Addon is ISiegeWeapon)
            {
                ISiegeWeapon weapon = (ISiegeWeapon)this.Addon;

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

            ISiegeWeapon weapon = this.Addon as ISiegeWeapon;

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
            if (this.Addon != null && this.Map != Map.Internal)
                this.Addon.Map = this.Map;
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
            private readonly ISiegeWeapon m_weapon;
            public RotateNextEntry(ISiegeWeapon weapon)
                : base(406)
            {
                this.m_weapon = weapon;
            }

            public override void OnClick()
            {
                if (this.m_weapon != null)
                    this.m_weapon.Facing++;
            }
        }

        private class RotatePreviousEntry : ContextMenuEntry
        {
            private readonly ISiegeWeapon m_weapon;
            public RotatePreviousEntry(ISiegeWeapon weapon)
                : base(405)
            {
                this.m_weapon = weapon;
            }

            public override void OnClick()
            {
                if (this.m_weapon != null)
                    this.m_weapon.Facing--;
            }
        }

        private class BackpackEntry : ContextMenuEntry
        {
            private readonly ISiegeWeapon m_weapon;
            private readonly Mobile m_from;
            public BackpackEntry(Mobile from, ISiegeWeapon weapon)
                : base(2139)
            {
                this.m_weapon = weapon;
                this.m_from = from;
            }

            public override void OnClick()
            {
                if (this.m_weapon != null)
                {
                    this.m_weapon.StoreWeapon(this.m_from);
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
                this.m_from = from;
                this.m_drag = drag;
            }

            public override void OnClick()
            {
                if (this.m_drag == null)
                    return;

                BaseCreature pet = this.m_drag.DraggedBy as BaseCreature;

                // only allow the person dragging it or their pet to release
                if (this.m_drag.DraggedBy == this.m_from || (pet != null && (pet.ControlMaster == this.m_from || pet.ControlMaster == null)))
                {
                    this.m_drag.DraggedBy = null;
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
                this.m_from = from;
                this.m_drag = drag;
            }

            public override void OnClick()
            {
                if (this.m_drag != null && this.m_from != null)
                {
                    this.m_from.SendMessage("Select a mobile to drag the weapon");
                    this.m_from.Target = new DragTarget(this.m_drag);
                }
            }
        }

        private class SetupEntry : ContextMenuEntry
        {
            private readonly ISiegeWeapon m_weapon;
            private readonly Mobile m_from;
            public SetupEntry(Mobile from, ISiegeWeapon weapon)
                : base(97)
            {
                this.m_weapon = weapon;
                this.m_from = from;
            }

            public override void OnClick()
            {
                if (this.m_weapon != null && this.m_from != null)
                {
                    this.m_weapon.PlaceWeapon(this.m_from, this.m_from.Location, this.m_from.Map);
                }
            }
        }

        private class DragTarget : Target
        {
            private readonly XmlDrag m_attachment;
            public DragTarget(XmlDrag attachment)
                : base(30, false, TargetFlags.None)
            {
                this.m_attachment = attachment;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (this.m_attachment == null || from == null)
                    return;

                if (!(targeted is Mobile))
                {
                    from.SendMessage("Must target a mobile");
                    return;
                }

                Mobile m = (Mobile)targeted;

                if (m == from || (m is BaseCreature && (((BaseCreature)m).Controlled && ((BaseCreature)m).ControlMaster == from)))
                {
                    this.m_attachment.DraggedBy = m;
                }
                else
                {
                    from.SendMessage("You dont control that.");
                }
            }
        }
    }
}