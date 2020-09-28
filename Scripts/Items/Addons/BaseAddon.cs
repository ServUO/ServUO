#region References
using Server.Multis;

using System;
using System.Collections.Generic;
#endregion

namespace Server.Items
{
    public enum AddonFitResult
    {
        Valid,
        Blocked,
        NotInHouse,
        DoorTooClose,
        NoWall,
        OwnerNotInHouse,
        FoundationStairs,
        InternalStairs
    }

    public interface IAddon : IEntity, IChopable
    {
        Item Deed { get; }

        bool CouldFit(IPoint3D p, Map map);
    }

    public abstract class BaseAddon : Item, IChopable, IAddon
    {
        #region Mondain's Legacy
        private CraftResource m_Resource;

        [CommandProperty(AccessLevel.Decorator)]
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
        #endregion

        private List<AddonComponent> m_Components;

        public void AddComponent(AddonComponent c, int x, int y, int z)
        {
            if (Deleted)
                return;

            m_Components.Add(c);

            c.Addon = this;
            c.Offset = new Point3D(x, y, z);
            c.MoveToWorld(new Point3D(X + x, Y + y, Z + z), Map);
        }

        public BaseAddon()
            : base(1)
        {
            Movable = false;
            Visible = false;

            m_Components = new List<AddonComponent>();
        }

        public void ApplyLight(LightType light)
        {
            Light = light;

            foreach (AddonComponent c in Components)
            {
                c.Light = light;
            }
        }

        public virtual bool RetainDeedHue => Hue != 0 && CraftResources.GetHue(Resource) != Hue;
        public virtual bool RetainComponentHue => false;

        public virtual void OnChop(Mobile from)
        {
            BaseHouse house = BaseHouse.FindHouseAt(this);

            #region High Seas
            BaseBoat boat = BaseBoat.FindBoatAt(from, from.Map);
            if (boat != null && boat is BaseGalleon)
            {
                ((BaseGalleon)boat).OnChop(this, from);
                return;
            }
            #endregion

            if (house != null && (house.IsOwner(from) || (house.Addons.ContainsKey(this) && house.Addons[this] == from)))
            {
                Effects.PlaySound(GetWorldLocation(), Map, 0x3B3);
                from.SendLocalizedMessage(500461); // You destroy the item.

                int hue = 0;

                if (RetainDeedHue)
                {
                    for (int i = 0; hue == 0 && i < m_Components.Count; ++i)
                    {
                        AddonComponent c = m_Components[i];

                        if (c.Hue != 0)
                            hue = c.Hue;
                    }
                }

                Delete();

                house.Addons.Remove(this);

                BaseAddonDeed deed = GetDeed();

                if (deed != null)
                {
                    if (!RetainComponentHue)
                    {
                        if (RetainDeedHue)
                            deed.Hue = hue;
                        else
                            deed.Hue = 0;
                    }

                    deed.IsReDeed = true;

                    from.AddToBackpack(deed);
                }
            }
            else
            {
                from.SendLocalizedMessage(1113134); // You can only redeed items in your own house!
            }
        }

        public virtual BaseAddonDeed Deed => null;

        public virtual BaseAddonDeed GetDeed()
        {
            BaseAddonDeed deed = Deed;

            if (deed != null)
            {
                deed.Resource = Resource;
            }

            return deed;
        }

        Item IAddon.Deed => GetDeed();

        public List<AddonComponent> Components => m_Components;

        public BaseAddon(Serial serial)
            : base(serial)
        { }

        public bool CouldFit(IPoint3D p, Map map)
        {
            BaseHouse h = null;
            return (CouldFit(p, map, null, ref h) == AddonFitResult.Valid);
        }

        public virtual AddonFitResult CouldFit(IPoint3D p, Map map, Mobile from, ref BaseHouse house)
        {
            if (Deleted)
            {
                return AddonFitResult.Blocked;
            }

            foreach (AddonComponent c in m_Components)
            {
                Point3D p3D = new Point3D(p.X + c.Offset.X, p.Y + c.Offset.Y, p.Z + c.Offset.Z);

                if (!map.CanFit(p3D.X, p3D.Y, p3D.Z, Math.Max(1, c.ItemData.Height), false, true, (c.Z == 0), true))
                {
                    return AddonFitResult.Blocked;
                }

                if (!CheckHouse(from, p3D, map, c.ItemData.Height, ref house))
                {
                    return AddonFitResult.NotInHouse;
                }

                if (from != null)
                {
                    var fromHouse = BaseHouse.FindHouseAt(from.Location, map, 16);

                    if (fromHouse == null || house != fromHouse)
                    {
                        return AddonFitResult.OwnerNotInHouse;
                    }
                }

                if (c.NeedsWall)
                {
                    Point3D wall = c.WallPosition;

                    if (!IsWall(p3D.X + wall.X, p3D.Y + wall.Y, p3D.Z + wall.Z, map))
                    {
                        return AddonFitResult.NoWall;
                    }
                }

                if (house != null)
                {
                    bool frontStairs;

                    if (house.IsStairArea(p, out frontStairs))
                    {
                        if (frontStairs)
                        {
                            return AddonFitResult.FoundationStairs;
                        }
                        else
                        {
                            return AddonFitResult.InternalStairs;
                        }
                    }
                }
            }

            if (house != null)
            {
                List<Item> doors = house.Doors;

                for (int i = 0; i < doors.Count; ++i)
                {
                    BaseDoor door = doors[i] as BaseDoor;

                    Point3D doorLoc = door.GetWorldLocation();
                    int doorHeight = door.ItemData.CalcHeight;

                    foreach (AddonComponent c in m_Components)
                    {
                        Point3D addonLoc = new Point3D(p.X + c.Offset.X, p.Y + c.Offset.Y, p.Z + c.Offset.Z);
                        int addonHeight = c.ItemData.CalcHeight;

                        if (Utility.InRange(doorLoc, addonLoc, 1) && (addonLoc.Z == doorLoc.Z ||
                                                                      ((addonLoc.Z + addonHeight) > doorLoc.Z && (doorLoc.Z + doorHeight) > addonLoc.Z)))
                            return AddonFitResult.DoorTooClose;
                    }
                }
            }

            return AddonFitResult.Valid;
        }

        public static bool CheckHouse(Mobile from, Point3D p, Map map, int height, ref BaseHouse house)
        {
            house = BaseHouse.FindHouseAt(p, map, height);

            if (house == null || (from != null && !house.IsCoOwner(from)))
            {
                return false;
            }

            return true;
        }

        public static bool IsWall(int x, int y, int z, Map map)
        {
            if (map == null)
                return false;

            StaticTile[] tiles = map.Tiles.GetStaticTiles(x, y, true);

            for (int i = 0; i < tiles.Length; ++i)
            {
                StaticTile t = tiles[i];
                ItemData id = TileData.ItemTable[t.ID & TileData.MaxItemValue];

                if ((id.Flags & TileFlag.Wall) != 0 && (z + 16) > t.Z && (t.Z + t.Height) > z)
                    return true;
            }

            return false;
        }

        public virtual void OnComponentLoaded(AddonComponent c)
        { }

        public virtual void OnComponentUsed(AddonComponent c, Mobile from)
        { }

        public override void OnLocationChange(Point3D oldLoc)
        {
            if (Deleted)
                return;

            foreach (AddonComponent c in m_Components)
                c.Location = new Point3D(X + c.Offset.X, Y + c.Offset.Y, Z + c.Offset.Z);
        }

        public override void OnMapChange()
        {
            if (Deleted)
                return;

            foreach (AddonComponent c in m_Components)
                c.Map = Map;
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            foreach (AddonComponent c in m_Components)
                c.Delete();
        }

        public virtual bool ShareHue => true;

        [Hue, CommandProperty(AccessLevel.Decorator)]
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
                        foreach (AddonComponent c in m_Components)
                            c.Hue = value;
                    }
                }
            }
        }

        public virtual void UpdateProperties()
        {
            InvalidateProperties();

            foreach (AddonComponent o in Components)
            {
                o.InvalidateProperties();
            }
        }

        public virtual void GetProperties(ObjectPropertyList list, AddonComponent c)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(2); // version

            writer.Write((int)m_Resource);

            writer.WriteItemList(m_Components);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 2:
                    m_Resource = (CraftResource)reader.ReadInt();
                    goto case 1;
                case 1:
                case 0:
                    {
                        m_Components = reader.ReadStrongItemList<AddonComponent>();
                        break;
                    }
            }

            if (version < 1 && Weight == 0)
                Weight = -1;
        }
    }
}
