using System;
using System.Collections;
using System.Collections.Generic;
using Server.Multis;

namespace Server.Items
{
    public enum AddonFitResult
    {
        Valid,
        Blocked,
        NotInHouse,
        DoorTooClose,
        NoWall,
        DoorsNotClosed
    }

    public interface IAddon
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
            get
            {
                return this.m_Resource;
            }
            set
            {
                if (this.m_Resource != value)
                {
                    this.m_Resource = value;
                    this.Hue = CraftResources.GetHue(this.m_Resource);
					
                    this.InvalidateProperties();
                }
            }
        }
        #endregion
        private List<AddonComponent> m_Components;

        public void AddComponent(AddonComponent c, int x, int y, int z)
        {
            if (this.Deleted)
                return;

            this.m_Components.Add(c);

            c.Addon = this;
            c.Offset = new Point3D(x, y, z);
            c.MoveToWorld(new Point3D(this.X + x, this.Y + y, this.Z + z), this.Map);
        }

        public BaseAddon()
            : base(1)
        {
            this.Movable = false;
            this.Visible = false;

            this.m_Components = new List<AddonComponent>();
        }

        public virtual bool RetainDeedHue
        {
            get
            {
                return false;
            }
        }

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

            if (house != null && house.IsOwner(from) && house.Addons.Contains(this))
            {
                Effects.PlaySound(this.GetWorldLocation(), this.Map, 0x3B3);
                from.SendLocalizedMessage(500461); // You destroy the item.

                int hue = 0;

                if (this.RetainDeedHue)
                {
                    for (int i = 0; hue == 0 && i < this.m_Components.Count; ++i)
                    {
                        AddonComponent c = this.m_Components[i];

                        if (c.Hue != 0)
                            hue = c.Hue;
                    }
                }

                this.Delete();

                house.Addons.Remove(this);

                BaseAddonDeed deed = this.GetDeed();

                if (deed != null)
                {
					if (this.RetainDeedHue)
						deed.Hue = hue;
					else
						deed.Hue = 0;

                    from.AddToBackpack(deed);
                }
            }
        }

        public virtual BaseAddonDeed Deed
        {
            get
            {
                return null;
            }
        }

		public virtual BaseAddonDeed GetDeed()
		{
			BaseAddonDeed deed = Deed;
			if(deed != null)
			{
				deed.Resource = this.Resource;
			}
			return deed;
		}

		Item IAddon.Deed
        {
            get
            {
				return GetDeed();
            }
        }

        public List<AddonComponent> Components
        {
            get
            {
                return this.m_Components;
            }
        }

        public BaseAddon(Serial serial)
            : base(serial)
        {
        }

        public bool CouldFit(IPoint3D p, Map map)
        {
            BaseHouse h = null;
            return (this.CouldFit(p, map, null, ref h) == AddonFitResult.Valid);
        }

        public virtual AddonFitResult CouldFit(IPoint3D p, Map map, Mobile from, ref BaseHouse house)
        {
            BaseGalleon boat = null;
            return CouldFit(p, map, from, ref house, ref boat);
        }

        public virtual AddonFitResult CouldFit(IPoint3D p, Map map, Mobile from, ref BaseHouse house, ref BaseGalleon boat)
        {
            if (this.Deleted)
                return AddonFitResult.Blocked;

            foreach (AddonComponent c in this.m_Components)
            {
                Point3D p3D = new Point3D(p.X + c.Offset.X, p.Y + c.Offset.Y, p.Z + c.Offset.Z);

                if (!map.CanFit(p3D.X, p3D.Y, p3D.Z, c.ItemData.Height, false, true, (c.Z == 0)))
                    return AddonFitResult.Blocked;
                else if (!CheckHouse(from, p3D, map, c.ItemData.Height, ref house) && !CheckBoat(from, p3D, map, ref boat))
                    return AddonFitResult.NotInHouse;

                if (c.NeedsWall)
                {
                    Point3D wall = c.WallPosition;

                    if (!IsWall(p3D.X + wall.X, p3D.Y + wall.Y, p3D.Z + wall.Z, map))
                        return AddonFitResult.NoWall;
                }
            }

            if (house != null)
            {
                ArrayList doors = house.Doors;

                for (int i = 0; i < doors.Count; ++i)
                {
                    BaseDoor door = doors[i] as BaseDoor;

                    Point3D doorLoc = door.GetWorldLocation();
                    int doorHeight = door.ItemData.CalcHeight;

                    foreach (AddonComponent c in this.m_Components)
                    {
                        Point3D addonLoc = new Point3D(p.X + c.Offset.X, p.Y + c.Offset.Y, p.Z + c.Offset.Z);
                        int addonHeight = c.ItemData.CalcHeight;

                        if (Utility.InRange(doorLoc, addonLoc, 1) && (addonLoc.Z == doorLoc.Z || ((addonLoc.Z + addonHeight) > doorLoc.Z && (doorLoc.Z + doorHeight) > addonLoc.Z)))
                            return AddonFitResult.DoorTooClose;
                    }
                }
            }

            return AddonFitResult.Valid;
        }

        public static bool CheckHouse(Mobile from, Point3D p, Map map, int height, ref BaseHouse house)
        {
            house = BaseHouse.FindHouseAt(p, map, height);

            if (house == null || (from != null && !house.IsOwner(from)))
                return false;

            return true;
        }

        #region High Seas

        private static int[] m_ShipAddonTiles = new int[]
        {
            23664, 23665, 23718, 23719, 23610, 23611,
            23556, 23557, 23664, 23665, 23718, 23719,
            23610, 23611, 23556, 23557
        };

        public static bool CheckBoat(Mobile from, Point3D p, Map map, ref BaseGalleon boat)
        {
            BaseBoat b = BaseBoat.FindBoatAt(p, map);
            if (b is BaseGalleon)
                boat = b as BaseGalleon;

            if (boat != null)
            {
                if (boat.Addons.Count >= boat.MaxAddons)
                    return false;

                IPooledEnumerable eable = boat.Map.GetItemsInRange(p, 0);

                foreach (Item item in eable)
                {
                    foreach (int id in m_ShipAddonTiles)
                    {
                        if (id == item.ItemID)
                        {
                            eable.Free();
                            return true;
                        }
                    }
                }

                eable.Free();
            }
            return false;
        }
        #endregion

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
        {
        }

        public virtual void OnComponentUsed(AddonComponent c, Mobile from)
        {
        }

        public override void OnLocationChange(Point3D oldLoc)
        {
            if (this.Deleted)
                return;

            foreach (AddonComponent c in this.m_Components)
                c.Location = new Point3D(this.X + c.Offset.X, this.Y + c.Offset.Y, this.Z + c.Offset.Z);
        }

        public override void OnMapChange()
        {
            if (this.Deleted)
                return;

            foreach (AddonComponent c in this.m_Components)
                c.Map = this.Map;
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            foreach (AddonComponent c in this.m_Components)
                c.Delete();
        }

        public virtual bool ShareHue
        {
            get
            {
                return true;
            }
        }

        [Hue, CommandProperty(AccessLevel.Decorator)]
        public override int Hue
        {
            get
            {
                return base.Hue;
            }
            set
            {
                if (base.Hue != value)
                {
                    base.Hue = value;

                    if (!this.Deleted && this.ShareHue && this.m_Components != null)
                    {
                        foreach (AddonComponent c in this.m_Components)
                            c.Hue = value;
                    }
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            writer.WriteItemList<AddonComponent>(this.m_Components);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 1:
                case 0:
                    {
                        this.m_Components = reader.ReadStrongItemList<AddonComponent>();
                        break;
                    }
            }

            if (version < 1 && this.Weight == 0)
                this.Weight = -1;
        }
    }
}