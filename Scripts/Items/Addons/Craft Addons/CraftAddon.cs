using Server.Engines.Craft;
using Server.Multis;
using Server.Network;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Items
{
    public abstract class CraftAddon : BaseAddon, Gumps.ISecurable
    {
        public List<AddonToolComponent> Tools { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level { get; set; }

        public abstract CraftSystem CraftSystem { get; }

        [Hue, CommandProperty(AccessLevel.GameMaster)]
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

                    if (!Deleted && ShareHue && Tools != null)
                    {
                        foreach (AddonToolComponent tool in Tools)
                            tool.Hue = value;
                    }
                }
            }
        }

        public override BaseAddonDeed Deed => null;

        [Constructable]
        public CraftAddon()
        {
            Tools = new List<AddonToolComponent>();
        }

        public void AddCraftComponent(AddonToolComponent tool, int x, int y, int z)
        {
            if (Deleted)
                return;

            Tools.Add(tool);
            Level = SecureLevel.CoOwners;

            tool.Addon = this;
            tool.Offset = new Point3D(x, y, z);
            tool.MoveToWorld(new Point3D(X + x, Y + y, Z + z), Map);
        }

        public override AddonFitResult CouldFit(IPoint3D p, Map map, Mobile from, ref BaseHouse house)
        {
            AddonFitResult result = base.CouldFit(p, map, from, ref house);

            if (result == AddonFitResult.Valid)
            {
                foreach (AddonToolComponent c in Tools)
                {
                    Point3D p3D = new Point3D(p.X + c.Offset.X, p.Y + c.Offset.Y, p.Z + c.Offset.Z);

                    if (!map.CanFit(p3D.X, p3D.Y, p3D.Z, c.ItemData.Height, false, true, (c.Z == 0)))
                        return AddonFitResult.Blocked;
                    else if (!CheckHouse(from, p3D, map, c.ItemData.Height, ref house))
                        return AddonFitResult.NotInHouse;

                    if (c.NeedsWall)
                    {
                        Point3D wall = c.WallPosition;

                        if (!IsWall(p3D.X + wall.X, p3D.Y + wall.Y, p3D.Z + wall.Z, map))
                            return AddonFitResult.NoWall;
                    }
                }
            }

            return result;
        }

        public virtual void OnCraftComponentUsed(Mobile from, AddonToolComponent tool)
        {
            if (!tool.TurnedOn)
                return;

            if (from.InRange(tool.Location, 2))
            {
                BaseHouse house = BaseHouse.FindHouseAt(from);

                if (house != null && house.HasSecureAccess(from, Level))
                {
                    from.SendGump(new CraftGump(from, CraftSystem, tool, null));
                }
                else
                {
                    tool.PublicOverheadMessage(MessageType.Regular, 0x3E9, 1061637); // You are not allowed to access this.
                }
            }
            else
            {
                from.SendLocalizedMessage(500325); // I am too far away to do that.
            }
        }

        public virtual void OnCraftComponentLoaded(AddonToolComponent tool)
        {
        }

        public override void OnLocationChange(Point3D old)
        {
            base.OnLocationChange(old);

            Tools.ForEach(t => t.Location = new Point3D(X + t.Offset.X, Y + t.Offset.Y, Z + t.Offset.Z));
        }

        public override void OnMapChange()
        {
            base.OnMapChange();

            Tools.ForEach(t => t.Map = Map);
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            Tools.ForEach(t => t.Delete());
        }

        public CraftAddon(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write((int)Level);

            writer.Write(Tools.Count);
            Tools.ForEach(t => writer.Write(t));
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Level = (SecureLevel)reader.ReadInt();

            Tools = new List<AddonToolComponent>();

            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                AddonToolComponent tool = reader.ReadItem() as AddonToolComponent;

                if (tool != null)
                {
                    tool.SetCraftSystem(CraftSystem);
                    Tools.Add(tool);
                }
            }
        }

        public class ToolDropComponent : LocalizedAddonComponent
        {
            public override bool ForceShowProperties => true;

            public ToolDropComponent(int id, int cliloc)
                : base(id, cliloc)
            {
            }

            public override bool OnDragDrop(Mobile from, Item dropped)
            {
                BaseHouse house = BaseHouse.FindHouseAt(this);
                CraftAddon addon = Addon as CraftAddon;

                if (house != null && addon != null && house.HasSecureAccess(from, addon.Level))
                {
                    if (dropped is ITool && !(dropped is BaseRunicTool))
                    {
                        ITool tool = dropped as ITool;

                        if (tool.CraftSystem == addon.CraftSystem)
                        {
                            AddonToolComponent comp = addon.Tools.FirstOrDefault(t => t != null);

                            if (comp == null)
                                return false;

                            if (comp.UsesRemaining >= comp.MaxUses)
                            {
                                from.SendLocalizedMessage(1155740); // Adding this to the power tool would put it over the max number of charges the tool can hold.
                                return false;
                            }
                            else
                            {
                                int toadd = Math.Min(tool.UsesRemaining, comp.MaxUses - comp.UsesRemaining);

                                comp.UsesRemaining += toadd;
                                tool.UsesRemaining -= toadd;

                                if (tool.UsesRemaining <= 0 && !tool.Deleted)
                                    tool.Delete();

                                from.SendLocalizedMessage(1155741); // Charges have been added to the power tool.

                                Effects.PlaySound(Location, Map, 0x42);

                                return false;
                            }
                        }
                        else
                        {
                            from.SendLocalizedMessage(1074836); // The container cannot hold that type of object.
                            return false;
                        }
                    }
                    else
                    {
                        from.SendLocalizedMessage(1074836); // The container cannot hold that type of object.
                        return false;
                    }
                }
                else
                {
                    SendLocalizedMessageTo(from, 1061637); // You are not allowed to access this.
                    return false;
                }
            }

            public ToolDropComponent(Serial serial)
                : base(serial)
            {
            }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);
                writer.Write(0);
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);
                int version = reader.ReadInt();
            }
        }
    }
}
