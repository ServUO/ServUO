using Server.ContextMenus;
using Server.Engines.Craft;
using Server.Multis;
using Server.Network;
using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class AddonToolComponent : BaseTool, IChopable
    {
        private CraftSystem _CraftSystem;
        private int _LabelNumber;
        private bool _TurnedOn;

        public override CraftSystem CraftSystem => _CraftSystem;
        public override bool BreakOnDepletion => false;
        public override bool ForceShowProperties => true;
        public override int LabelNumber => _LabelNumber;

        public virtual bool NeedsWall => false;
        public virtual int MaxUses => 5000;
        public virtual Point3D WallPosition => Point3D.Zero;

        [CommandProperty(AccessLevel.GameMaster)]
        public CraftAddon Addon { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D Offset { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int InactiveID { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int ActiveID { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int InactiveMessage { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int ActiveMessage { get; set; }

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
                        ItemID = ActiveID;
                    else
                        ItemID = InactiveID;
                }

                _TurnedOn = value;
            }
        }

        public AddonToolComponent(CraftSystem system, int inactiveid, int activeid, int cliloc, int uses, CraftAddon addon)
            : this(system, inactiveid, activeid, 0, 0, cliloc, uses, addon)
        {
        }

        public AddonToolComponent(CraftSystem system, int inactiveid, int activeid, int inactivemessage, int activemessage, int cliloc, int uses, CraftAddon addon)
            : base(0, inactiveid)
        {
            _CraftSystem = system;
            _LabelNumber = cliloc;
            Addon = addon;
            Movable = false;

            InactiveID = inactiveid;
            ActiveID = activeid;

            InactiveMessage = inactivemessage;
            ActiveMessage = activemessage;

            UsesRemaining = uses;
        }

        public void OnChop(Mobile from)
        {
            if (Addon != null && from.InRange(GetWorldLocation(), 3))
                Addon.OnChop(from);
            else
                from.SendLocalizedMessage(500446); // That is too far away.
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (Addon == null)
                return;

            BaseHouse house = BaseHouse.FindHouseAt(this);

            if (house != null && house.HasSecureAccess(from, Addon.Level))
            {
                list.Add(new SimpleContextMenuEntry(from, 1155742, m => // Toggle: On/Off
                {
                    if (_TurnedOn)
                    {
                        TurnedOn = false;

                        if (InactiveMessage != 0)
                            PrivateOverheadMessage(MessageType.Regular, 0x3B2, InactiveMessage, from.NetState);
                    }
                    else
                    {
                        TurnedOn = true;

                        if (ActiveMessage != 0)
                        {
                            PrivateOverheadMessage(MessageType.Regular, 0x3B2, ActiveMessage, from.NetState);
                            from.PlaySound(84);
                        }
                    }
                }, 8));

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
                    num = 1061637; // You are not allowed to access 
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
            BaseHouse house = BaseHouse.FindHouseAt(this);

            if (house != null && Addon != null && house.HasSecureAccess(from, Addon.Level))
            {
                if (dropped is ITool && !(dropped is BaseRunicTool))
                {
                    ITool tool = dropped as ITool;

                    if (tool.CraftSystem == _CraftSystem)
                    {
                        if (UsesRemaining >= MaxUses)
                        {
                            from.SendLocalizedMessage(1155740); // Adding this to the power tool would put it over the max number of charges the tool can hold.
                        }
                        else
                        {
                            int toadd = Math.Min(tool.UsesRemaining, MaxUses - UsesRemaining);

                            UsesRemaining += toadd;
                            tool.UsesRemaining -= toadd;

                            if (tool.UsesRemaining <= 0 && !tool.Deleted)
                                tool.Delete();

                            from.SendLocalizedMessage(1155741); // Charges have been added to the power tool.

                            return true;
                        }
                    }
                    else
                    {
                        from.SendLocalizedMessage(1074836); // The container cannot hold that type of object.
                    }
                }
                else
                {
                    from.SendLocalizedMessage(1074836); // The container cannot hold that type of object.
                }
            }
            else
            {
                from.SendLocalizedMessage(1074836); // The container cannot hold that type of object.
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
            writer.Write(1);

            writer.Write(InactiveMessage);
            writer.Write(ActiveMessage);
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

            switch (version)
            {
                case 1:
                    {
                        InactiveMessage = reader.ReadInt();
                        ActiveMessage = reader.ReadInt();
                        goto case 0;
                    }
                case 0:
                    {
                        Offset = reader.ReadPoint3D();
                        Addon = reader.ReadItem() as CraftAddon;
                        _LabelNumber = reader.ReadInt();
                        ActiveID = reader.ReadInt();
                        InactiveID = reader.ReadInt();
                        TurnedOn = reader.ReadBool();
                        break;
                    }
            }

            if (Addon != null)
                Addon.OnCraftComponentLoaded(this);
        }
    }
}
