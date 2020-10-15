#region References
using Server.Multis;
using System.Collections.Generic;
#endregion

namespace Server.Items
{
    public abstract class BaseAddonContainer : BaseContainer, IChopable, IAddon
    {
        private CraftResource m_Resource;
        private List<AddonContainerComponent> m_Components;

        public BaseAddonContainer(int itemID)
            : base(itemID)
        {
            Movable = false;

            AddonComponent.ApplyLightTo(this);

            m_Components = new List<AddonContainerComponent>();
        }

        public BaseAddonContainer(Serial serial)
            : base(serial)
        { }

        public override bool DisplayWeight => false;

        [Hue, CommandProperty(AccessLevel.GameMaster)]
        public override int Hue
        {
            get { return base.Hue; }
            set
            {
                if (base.Hue != value)
                {
                    base.Hue = value;

                    if (!Deleted && ShareHue && m_Components != null)
                    {
                        Hue = value;

                        foreach (AddonContainerComponent c in m_Components)
                            c.Hue = value;
                    }
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public CraftResource Resource
        {
            get { return m_Resource; }
            set
            {
                if (m_Resource != value)
                {
                    m_Resource = value;
                    Hue = CraftResources.GetHue(m_Resource);

                    InvalidateProperties();
                }
            }
        }

        public virtual bool RetainDeedHue => false;
        public virtual bool NeedsWall => false;
        public virtual bool ShareHue => true;
        public virtual Point3D WallPosition => Point3D.Zero;
        public virtual BaseAddonContainerDeed Deed => null;
        public List<AddonContainerComponent> Components => m_Components;
        Item IAddon.Deed => Deed;

        public override void OnLocationChange(Point3D oldLoc)
        {
            base.OnLocationChange(oldLoc);

            if (Deleted)
                return;

            foreach (AddonContainerComponent c in m_Components)
                c.Location = new Point3D(X + c.Offset.X, Y + c.Offset.Y, Z + c.Offset.Z);
        }

        public override void OnMapChange()
        {
            base.OnMapChange();

            if (Deleted)
                return;

            foreach (AddonContainerComponent c in m_Components)
                c.Map = Map;
        }

        public override void OnDelete()
        {
            BaseHouse house = BaseHouse.FindHouseAt(this);

            if (house != null)
                house.Addons.Remove(this);

            List<AddonContainerComponent> components = new List<AddonContainerComponent>(m_Components);

            foreach (AddonContainerComponent component in components)
            {
                component.Addon = null;
                component.Delete();
            }

            components.Clear();
            components.TrimExcess();

            base.OnDelete();
        }

        public virtual void UpdateProperties()
        {
            InvalidateProperties();

            foreach (AddonContainerComponent o in Components)
            {
                o.InvalidateProperties();
            }
        }

        public virtual void GetProperties(ObjectPropertyList list, AddonContainerComponent c)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (!CraftResources.IsStandard(m_Resource))
                list.Add(CraftResources.GetLocalizationNumber(m_Resource));
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            foreach (AddonContainerComponent c in m_Components)
                c.Delete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version

            writer.WriteItemList(m_Components);
            writer.Write((int)m_Resource);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_Components = reader.ReadStrongItemList<AddonContainerComponent>();
            m_Resource = (CraftResource)reader.ReadInt();

            AddonComponent.ApplyLightTo(this);
        }

        public virtual void DropItemsToGround()
        {
            for (int i = Items.Count - 1; i >= 0; i--)
                Items[i].MoveToWorld(Location);
        }

        public void AddComponent(AddonContainerComponent c, int x, int y, int z)
        {
            if (Deleted)
                return;

            m_Components.Add(c);

            c.Addon = this;
            c.Offset = new Point3D(x, y, z);
            c.MoveToWorld(new Point3D(X + x, Y + y, Z + z), Map);
        }

        public AddonFitResult CouldFit(IPoint3D p, Map map, Mobile from, ref BaseHouse house)
        {
            if (Deleted)
                return AddonFitResult.Blocked;

            foreach (AddonContainerComponent c in m_Components)
            {
                Point3D p3D = new Point3D(p.X + c.Offset.X, p.Y + c.Offset.Y, p.Z + c.Offset.Z);

                if (!map.CanFit(p3D.X, p3D.Y, p3D.Z, c.ItemData.Height, false, true, (c.Z == 0)))
                    return AddonFitResult.Blocked;
                if (!BaseAddon.CheckHouse(from, p3D, map, c.ItemData.Height, ref house))
                    return AddonFitResult.NotInHouse;

                if (c.NeedsWall)
                {
                    Point3D wall = c.WallPosition;

                    if (!BaseAddon.IsWall(p3D.X + wall.X, p3D.Y + wall.Y, p3D.Z + wall.Z, map))
                        return AddonFitResult.NoWall;
                }
            }

            Point3D p3 = new Point3D(p.X, p.Y, p.Z);

            if (!map.CanFit(p3.X, p3.Y, p3.Z, ItemData.Height, false, true, (Z == 0)))
                return AddonFitResult.Blocked;
            if (!BaseAddon.CheckHouse(from, p3, map, ItemData.Height, ref house))
                return AddonFitResult.NotInHouse;

            if (NeedsWall)
            {
                Point3D wall = WallPosition;

                if (!BaseAddon.IsWall(p3.X + wall.X, p3.Y + wall.Y, p3.Z + wall.Z, map))
                    return AddonFitResult.NoWall;
            }

            if (house != null)
            {
                List<Item> doors = house.Doors;

                for (int i = 0; i < doors.Count; ++i)
                {
                    BaseDoor door = doors[i] as BaseDoor;

                    if (door == null)
                        continue;

                    Point3D doorLoc = door.GetWorldLocation();
                    int doorHeight = door.ItemData.CalcHeight;

                    foreach (AddonContainerComponent c in m_Components)
                    {
                        Point3D addonLoc = new Point3D(p.X + c.Offset.X, p.Y + c.Offset.Y, p.Z + c.Offset.Z);
                        int addonHeight = c.ItemData.CalcHeight;

                        if (Utility.InRange(doorLoc, addonLoc, 1) && (addonLoc.Z == doorLoc.Z ||
                                                                      ((addonLoc.Z + addonHeight) > doorLoc.Z && (doorLoc.Z + doorHeight) > addonLoc.Z)))
                            return AddonFitResult.DoorTooClose;
                    }

                    Point3D addonLo = new Point3D(p.X, p.Y, p.Z);
                    int addonHeigh = ItemData.CalcHeight;

                    if (Utility.InRange(doorLoc, addonLo, 1) && (addonLo.Z == doorLoc.Z ||
                                                                 ((addonLo.Z + addonHeigh) > doorLoc.Z && (doorLoc.Z + doorHeight) > addonLo.Z)))
                        return AddonFitResult.DoorTooClose;
                }
            }

            return AddonFitResult.Valid;
        }

        public bool CouldFit(IPoint3D p, Map map)
        {
            BaseHouse house = null;

            return (CouldFit(p, map, null, ref house) == AddonFitResult.Valid);
        }

        public virtual void OnChop(Mobile from)
        {
            BaseHouse house = BaseHouse.FindHouseAt(this);

            if (house != null && house.IsOwner(from))
            {
                if (!IsSecure)
                {
                    Effects.PlaySound(GetWorldLocation(), Map, 0x3B3);
                    from.SendLocalizedMessage(500461); // You destroy the item.

                    int hue = 0;

                    if (RetainDeedHue)
                    {
                        for (int i = 0; hue == 0 && i < m_Components.Count; ++i)
                        {
                            AddonContainerComponent c = m_Components[i];

                            if (c.Hue != 0)
                                hue = c.Hue;
                        }
                    }

                    DropItemsToGround();

                    Delete();

                    house.Addons.Remove(this);

                    BaseAddonContainerDeed deed = Deed;

                    if (deed != null)
                    {
                        deed.Resource = Resource;

                        if (RetainDeedHue)
                            deed.Hue = hue;

                        from.AddToBackpack(deed);
                    }
                }
                else
                    from.SendLocalizedMessage(1074870); // This item must be unlocked/unsecured before re-deeding it.
            }
        }

        public virtual void OnComponentLoaded(AddonContainerComponent c)
        { }

        public virtual void OnComponentUsed(AddonContainerComponent c, Mobile from)
        {
            if (!Deleted)
            {
                OnDoubleClick(from);
            }
        }
    }
}
