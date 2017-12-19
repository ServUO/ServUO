using System;
using Server;
using Server.Engines.Craft;
using Server.ContextMenus;
using System.Collections.Generic;
using Server.Multis;

namespace Server.Items
{
    public class AddonToolComponent : BaseTool, IChopable
    {
        private CraftSystem _CraftSystem;
        private int _LabelNumber;
        private bool _TurnedOn;

        public override CraftSystem CraftSystem { get { return _CraftSystem; } }
        public override bool BreakOnDepletion { get { return false; } }
        public override bool ForceShowProperties { get { return true; } }
        public override int LabelNumber { get { return _LabelNumber; } }

        public virtual bool NeedsWall { get { return false; } }
        public virtual int MaxUses { get { return 5000; } }
        public virtual Point3D WallPosition { get { return Point3D.Zero; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public CraftAddon Addon { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D Offset { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int InactiveID { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int ActiveID { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool TurnedOn
        {
            get
            {
                return _TurnedOn;
            }
            set
            {
                if (_TurnedOn != value)
                {
                    if (value)
                        this.ItemID = ActiveID;
                    else
                        this.ItemID = InactiveID;
                }

                _TurnedOn = value;
            }
        }

        public AddonToolComponent(CraftSystem system, int inactiveid, int activeid, int cliloc, int uses, CraftAddon addon)
            : base(0, inactiveid)
        {
            _CraftSystem = system;
            _LabelNumber = cliloc;
            Addon = addon;
            Movable = false;

            InactiveID = inactiveid;
            ActiveID = activeid;

            UsesRemaining = uses;
        }

        public void OnChop(Mobile from)
        {
            if (Addon != null && from.InRange(GetWorldLocation(), 3))
                Addon.OnChop(from);
            else
                from.SendLocalizedMessage(500446); // That is too far away.
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (Addon != null)
            {
                if (_TurnedOn)
                    list.Add(502695); // turned on
                else
                    list.Add(502696); // turned off
            }
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (Addon == null)
                return;

            BaseHouse house = BaseHouse.FindHouseAt(this);

            if (house != null && house.HasSecureAccess(from, Addon.Level))
            {
                list.Add(new SimpleContextMenuEntry(from, _TurnedOn ? 1011035 : 1011034, m =>
                    {
                        if (_TurnedOn)
                        {
                            TurnedOn = false;
                        }
                        else
                        {
                            TurnedOn = true;
                        }
                    }, 8)); // Activate this item : Deactivate this item TODO: Correct???

                SetSecureLevelEntry.AddTo(from, Addon, list);
            }
        }

        public override bool CheckAccessible(Mobile m, ref int num)
        {
            if (m.InRange(GetWorldLocation(), 2))
            {
                BaseHouse house = BaseHouse.FindHouseAt(m);

                if (house == null || Addon == null || !house.HasSecureAccess(m, Addon.Level))
                {
                    num = 1061637; // You are not allowed to access this.
                    return false;
                }
            }
            else
            {
                num = 500295;
                return false;
            }

            return true;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (Addon != null)
                Addon.OnCraftComponentUsed(from, this);
        }

        public void SetCraftSystem(CraftSystem system)
        {
            _CraftSystem = system;
        }

        public override void OnLocationChange(Point3D old)
        {
            if (Addon != null)
                Addon.Location = new Point3D(X - Offset.X, Y - Offset.Y, Z - Offset.Z);
        }

        public override void OnMapChange()
        {
            if (Addon != null)
                Addon.Map = Map;
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (Addon != null)
                Addon.Delete();
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (dropped is ITool)
            {
                var tool = dropped as ITool;

                if (tool.CraftSystem == _CraftSystem)
                {
                    if (UsesRemaining >= MaxUses)
                    {
                        from.SendMessage("That is already at its maximum charges.");
                        return false;
                    }
                    else
                    {
                        int toadd = Math.Min(tool.UsesRemaining, MaxUses - UsesRemaining);

                        UsesRemaining += toadd;
                        tool.UsesRemaining -= toadd;

                        if (tool.UsesRemaining <= 0 && !tool.Deleted)
                            tool.Delete();

                        return true;
                    }
                }
            }

            return false;
        }

        public AddonToolComponent(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
            writer.Write(Offset);
            writer.Write(Addon);
            writer.Write(_LabelNumber);
            writer.Write(ActiveID);
            writer.Write(InactiveID);
            writer.Write(TurnedOn);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            Offset = reader.ReadPoint3D();
            Addon = reader.ReadItem() as CraftAddon;
            _LabelNumber = reader.ReadInt();
            ActiveID = reader.ReadInt();
            InactiveID = reader.ReadInt();
            TurnedOn = reader.ReadBool();

            if (Addon != null)
                Addon.OnCraftComponentLoaded(this);
        }
    }
}