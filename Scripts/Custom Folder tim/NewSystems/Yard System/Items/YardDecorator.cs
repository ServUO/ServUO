using System;
using Server;
using Server.Items;
using Server.Network;
using Server.Regions;
using Server.Multis;
using Server.Gumps;
using Server.Targeting;

namespace Server.ACC.YS
{
    public class YardDecorator : InteriorDecorator
    {
        [Constructable]
        public YardDecorator()
            : base()
        {
            Name = "Yard Decorator";
            Weight = 1.0;
            LootType = LootType.Blessed;
            ItemID = 0xFC1;
        }

        public override int LabelNumber { get { return 1041280; } } // an interior decorator

        public YardDecorator(Serial serial)
            : base(serial)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (Command != DecorateCommand.None)
                list.Add(1018322 + (int)Command); // Turn/Up/Down
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

        public override void OnDoubleClick(Mobile from)
        {
            if (!CheckUse(this, from))
                return;

            if (Command == DecorateCommand.None)
                from.SendGump(new InternalGump(this));
            else
                from.Target = new InternalTarget(this);
        }

        private class InternalGump : Gump
        {
            private YardDecorator m_Decorator;

            public InternalGump(YardDecorator decorator)
                : base(150, 50)
            {
                m_Decorator = decorator;

                AddBackground(0, 0, 200, 150, 2600);

                AddButton(50, 47, 2152, 2154, 2, GumpButtonType.Reply, 0);
                AddHtmlLocalized(90, 50, 70, 40, 1018324, false, false); // Up

                AddButton(50, 87, 2152, 2154, 3, GumpButtonType.Reply, 0);
                AddHtmlLocalized(90, 100, 70, 40, 1018325, false, false); // Down
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                DecorateCommand command = DecorateCommand.None;

                switch (info.ButtonID)
                {
                    case 2: command = DecorateCommand.Up; break;
                    case 3: command = DecorateCommand.Down; break;
                }

                if (command != DecorateCommand.None)
                {
                    m_Decorator.Command = command;
                    sender.Mobile.Target = new InternalTarget(m_Decorator);
                }
            }
        }

        private class InternalTarget : Target
        {
            private YardDecorator m_Decorator;

            public InternalTarget(YardDecorator decorator)
                : base(-1, false, TargetFlags.None)
            {
                CheckLOS = false;

                m_Decorator = decorator;
            }

            protected override void OnTargetNotAccessible(Mobile from, object targeted)
            {
                OnTarget(from, targeted);
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted == m_Decorator)
                {
                    m_Decorator.Command = DecorateCommand.None;
                    from.SendGump(new InternalGump(m_Decorator));
                }
                else if (targeted is Item && InteriorDecorator.CheckUse(m_Decorator, from))
                {
                    BaseHouse house = BaseHouse.FindHouseAt(from);
                    Item item = (Item)targeted;

                    if (item is YardPiece || item is YardItem ||
                        item is YardIronGate || item is YardShortIronGate ||
                        item is YardLightWoodGate || item is YardDarkWoodGate ||
                        item is YardStair)
                    {
                        switch (m_Decorator.Command)
                        {
                            case DecorateCommand.Up: Up(item, from); break;
                            case DecorateCommand.Down: Down(item, from); break;
                        }
                    }
                    else if (house == null || !house.IsCoOwner(from))
                    {
                        from.SendLocalizedMessage(502092); // You must be in your house to do this.
                    }
                    else if (item.Parent != null || !house.IsInside(item))
                    {
                        from.SendLocalizedMessage(1042270); // That is not in your house.
                    }
                    else if (!house.IsLockedDown(item) && !house.IsSecure(item))
                    {
                        from.SendLocalizedMessage(1042271); // That is not locked down.
                    }
                    else if (item.TotalWeight + item.PileWeight > 100)
                    {
                        from.SendLocalizedMessage(1042272); // That is too heavy.
                    }
                    else
                    {
                        switch (m_Decorator.Command)
                        {
                            case DecorateCommand.Up: Up(item, from); break;
                            case DecorateCommand.Down: Down(item, from); break;
                        }
                    }
                }
            }

            private static void Up(Item item, Mobile from)
            {
                int floorZ = GetFloorZ(item);

                if (item is YardPiece || item is YardItem ||
                    item is YardIronGate || item is YardShortIronGate ||
                    item is YardLightWoodGate || item is YardDarkWoodGate ||
                    item is YardStair)
                {
                    item.Location = new Point3D(item.Location, item.Z + 1);
                }
                else if (floorZ > int.MinValue && item.Z < (floorZ + 15)) // Confirmed : no height checks here
                {
                    item.Location = new Point3D(item.Location, item.Z + 1);
                }
                else
                {
                    from.SendLocalizedMessage(1042274); // You cannot raise it up any higher.
                }
            }

            private static void Down(Item item, Mobile from)
            {
                int floorZ = GetFloorZ(item);
                if (item is YardPiece || item is YardItem ||
                    item is YardIronGate || item is YardShortIronGate ||
                    item is YardLightWoodGate || item is YardDarkWoodGate ||
                    item is YardStair)
                {
                    item.Location = new Point3D(item.Location, item.Z - 1);
                }
                else if (floorZ > int.MinValue && item.Z > GetFloorZ(item))
                {
                    item.Location = new Point3D(item.Location, item.Z - 1);
                }
                else
                {
                    from.SendLocalizedMessage(1042275); // You cannot lower it down any further.
                }
            }

            private static int GetFloorZ(Item item)
            {
                Map map = item.Map;

                if (map == null)
                    return int.MinValue;

                StaticTile[] tiles = map.Tiles.GetStaticTiles(item.X, item.Y, true);

                int z = int.MinValue;

                for (int i = 0; i < tiles.Length; ++i)
                {
                    StaticTile tile = tiles[i];
                    ItemData id = TileData.ItemTable[tile.ID & 0x3FFF];

                    int top = tile.Z; // Confirmed : no height checks here

                    if (id.Surface && !id.Impassable && top > z && top <= item.Z)
                        z = top;
                }

                return z;
            }
        }
    }
}