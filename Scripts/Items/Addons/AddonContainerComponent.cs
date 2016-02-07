using System;
using System.Collections.Generic;
using Server.ContextMenus;

namespace Server.Items
{
    public class AddonContainerComponent : Item, IChopable
    {
        private Point3D m_Offset;
        private BaseAddonContainer m_Addon;
        [Constructable]
        public AddonContainerComponent(int itemID)
            : base(itemID)
        {
            this.Movable = false;

            AddonComponent.ApplyLightTo(this);
        }

        public AddonContainerComponent(Serial serial)
            : base(serial)
        {
        }

        public virtual bool NeedsWall
        {
            get
            {
                return false;
            }
        }
        public virtual Point3D WallPosition
        {
            get
            {
                return Point3D.Zero;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public BaseAddonContainer Addon
        {
            get
            {
                return this.m_Addon;
            }
            set
            {
                this.m_Addon = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D Offset
        {
            get
            {
                return this.m_Offset;
            }
            set
            {
                this.m_Offset = value;
            }
        }
        [Hue, CommandProperty(AccessLevel.GameMaster)]
        public override int Hue
        {
            get
            {
                return base.Hue;
            }
            set
            {
                base.Hue = value;

                if (this.m_Addon != null && this.m_Addon.ShareHue)
                    this.m_Addon.Hue = value;
            }
        }
        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (this.Addon != null)
                return this.Addon.OnDragDrop(from, dropped);

            return false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (this.m_Addon != null)
                this.m_Addon.OnComponentUsed(this, from);
        }

        public override void OnLocationChange(Point3D old)
        {
            if (this.m_Addon != null)
                this.m_Addon.Location = new Point3D(this.X - this.m_Offset.X, this.Y - this.m_Offset.Y, this.Z - this.m_Offset.Z);
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            if (this.m_Addon != null)
                this.m_Addon.GetContextMenuEntries(from, list);
        }

        public override void OnMapChange()
        {
            if (this.m_Addon != null)
                this.m_Addon.Map = this.Map;
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (this.m_Addon != null)
                this.m_Addon.Delete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(this.m_Addon);
            writer.Write(this.m_Offset);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            this.m_Addon = reader.ReadItem() as BaseAddonContainer;
            this.m_Offset = reader.ReadPoint3D();

            if (this.m_Addon != null)
                this.m_Addon.OnComponentLoaded(this);

            AddonComponent.ApplyLightTo(this);
        }

        public virtual void OnChop(Mobile from)
        {
            if (this.m_Addon != null && from.InRange(this.GetWorldLocation(), 3))
                this.m_Addon.OnChop(from);
            else
                from.SendLocalizedMessage(500446); // That is too far away.
        }
    }

    public class LocalizedContainerComponent : AddonContainerComponent
    {
        private int m_LabelNumber;
        public LocalizedContainerComponent(int itemID, int labelNumber)
            : base(itemID)
        {
            this.m_LabelNumber = labelNumber;
        }

        public LocalizedContainerComponent(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                if (this.m_LabelNumber > 0)
                    return this.m_LabelNumber;

                return base.LabelNumber;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(this.m_LabelNumber);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            this.m_LabelNumber = reader.ReadInt();
        }
    }
}