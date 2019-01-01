using System;
using Server.Gumps;
using Server.Multis;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
    public enum DecorateCommand
    {
        None,
        Turn,
        Up,
        Down
    }

    public class InteriorDecorator : Item
    {
        public override int LabelNumber { get { return 1041280; } } // an interior decorator

        private DecorateCommand m_Command;

        [Constructable]
        public InteriorDecorator()
            : base(0xFC1)
        {
            Weight = 1.0;
            LootType = LootType.Blessed;
        }

        public InteriorDecorator(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DecorateCommand Command
        {
            get { return m_Command; }
            set
            {
                m_Command = value;
                InvalidateProperties();
            }
        }

        public static bool InHouse(Mobile from)
        {
            BaseHouse house = BaseHouse.FindHouseAt(from);

            return (house != null && house.IsCoOwner(from));
        }

        public static bool CheckUse(InteriorDecorator tool, Mobile from)
        {
            if (!InHouse(from))
                from.SendLocalizedMessage(502092); // You must be in your house to do this.
            else
                return true;

            return false;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_Command != DecorateCommand.None)
                list.Add(1018322 + (int)m_Command); // Turn/Up/Down
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

            if (from.FindGump(typeof(InternalGump)) == null)
                from.SendGump(new InternalGump(this));

            if (m_Command != DecorateCommand.None)
                from.Target = new InternalTarget(this);
        }

        private class InternalGump : Gump
        {
            private readonly InteriorDecorator m_Decorator;

            public InternalGump(InteriorDecorator decorator)
                : base(150, 50)
            {
                m_Decorator = decorator;

                AddBackground(0, 0, 200, 200, 2600);

                AddButton(50, 45, (decorator.Command == DecorateCommand.Turn ? 2154 : 2152), 2154, 1, GumpButtonType.Reply, 0);
                AddHtmlLocalized(90, 50, 70, 40, 1018323, false, false); // Turn

                AddButton(50, 95, (decorator.Command == DecorateCommand.Up ? 2154 : 2152), 2154, 2, GumpButtonType.Reply, 0);
                AddHtmlLocalized(90, 100, 70, 40, 1018324, false, false); // Up

                AddButton(50, 145, (decorator.Command == DecorateCommand.Down ? 2154 : 2152), 2154, 3, GumpButtonType.Reply, 0);
                AddHtmlLocalized(90, 150, 70, 40, 1018325, false, false); // Down
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                DecorateCommand command = DecorateCommand.None;
                Mobile m = sender.Mobile;

                switch (info.ButtonID)
                {
                    case 1:
                        command = DecorateCommand.Turn;
                        break;
                    case 2:
                        command = DecorateCommand.Up;
                        break;
                    case 3:
                        command = DecorateCommand.Down;
                        break;
                }

                if (command != DecorateCommand.None)
                {
                    m_Decorator.Command = command;
                    m.SendGump(new InternalGump(m_Decorator));
                    m.Target = new InternalTarget(m_Decorator);
                }
                else
                {
                    Target.Cancel(m);
                }
            }
        }

        private class InternalTarget : Target
        {
            private readonly InteriorDecorator m_Decorator;

            public InternalTarget(InteriorDecorator decorator)
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
                if (targeted is Item && CheckUse(m_Decorator, from))
                {
                    BaseHouse house = BaseHouse.FindHouseAt(from);
                    Item item = (Item)targeted;

                    bool isDecorableComponent = false;

                    if (item is AddonComponent || item is AddonContainerComponent || item is BaseAddonContainer)
                    {
                        object addon = null;
                        int count = 0;

                        if (item is AddonComponent)
                        {
                            AddonComponent component = (AddonComponent)item;
                            count = component.Addon.Components.Count;
                            addon = component.Addon;
                        }
                        else if (item is AddonContainerComponent)
                        {
                            AddonContainerComponent component = (AddonContainerComponent)item;
                            count = component.Addon.Components.Count;
                            addon = component.Addon;
                        }
                        else if (item is BaseAddonContainer)
                        {
                            BaseAddonContainer container = (BaseAddonContainer)item;
                            count = container.Components.Count;
                            addon = container;
                        }

                        if (count == 1 && Core.SE)
                            isDecorableComponent = true;

                        if (m_Decorator.Command == DecorateCommand.Turn)
                        {
                            FlipableAddonAttribute[] attributes = (FlipableAddonAttribute[])addon.GetType().GetCustomAttributes(typeof(FlipableAddonAttribute), false);

                            if (attributes.Length > 0)
                                isDecorableComponent = true;
                        }
                    }

                    if (house == null || !house.IsCoOwner(from))
                    {
                        from.SendLocalizedMessage(502092); // You must be in your house to do 
                    }
                    else if (item.Parent != null || !house.IsInside(item))
                    {
                        from.SendLocalizedMessage(1042270); // That is not in your house.
                    }
                    else if (!house.IsLockedDown(item) && !house.IsSecure(item) && !isDecorableComponent)
                    {
                        if (item is AddonComponent && m_Decorator.Command == DecorateCommand.Turn)
                            from.SendLocalizedMessage(1042273); // You cannot turn that.
                        else if (item is AddonComponent && m_Decorator.Command == DecorateCommand.Up)
                            from.SendLocalizedMessage(1042274); // You cannot raise it up any higher.
                        else if (item is AddonComponent && m_Decorator.Command == DecorateCommand.Down)
                            from.SendLocalizedMessage(1042275); // You cannot lower it down any further.
                        else
                            from.SendLocalizedMessage(1042271); // That is not locked down.
                    }
                    else if (item is VendorRentalContract)
                    {
                        from.SendLocalizedMessage(1062491); // You cannot use the house decorator on that object.
                    }
                    /*else if (item.TotalWeight + item.PileWeight > 100)
                    {
                        from.SendLocalizedMessage(1042272); // That is too heavy.
                    }*/
                    else
                    {
                        switch (m_Decorator.Command)
                        {
                            case DecorateCommand.Up:
                                Up(item, from);
                                break;
                            case DecorateCommand.Down:
                                Down(item, from);
                                break;
                            case DecorateCommand.Turn:
                                Turn(item, from);
                                break;
                        }
                    }
                }

                from.Target = new InternalTarget(m_Decorator);
            }

            protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
            {
                if (cancelType == TargetCancelType.Canceled)
                    from.CloseGump(typeof(InteriorDecorator.InternalGump));
            }

            private static void Turn(Item item, Mobile from)
            {
                if (item is IFlipable)
                {
                    ((IFlipable)item).OnFlip();
                    return;
                }

                if (item is AddonComponent || item is AddonContainerComponent || item is BaseAddonContainer)
                {
                    object addon = null;

                    if (item is AddonComponent)
                        addon = ((AddonComponent)item).Addon;
                    else if (item is AddonContainerComponent)
                        addon = ((AddonContainerComponent)item).Addon;
                    else if (item is BaseAddonContainer)
                        addon = (BaseAddonContainer)item;

                    FlipableAddonAttribute[] aAttributes = (FlipableAddonAttribute[])addon.GetType().GetCustomAttributes(typeof(FlipableAddonAttribute), false);

                    if (aAttributes.Length > 0)
                    {
                        aAttributes[0].Flip(from, (Item)addon);
                        return;
                    }
                }

                FlipableAttribute[] attributes = (FlipableAttribute[])item.GetType().GetCustomAttributes(typeof(FlipableAttribute), false);

                if (attributes.Length > 0)
                    attributes[0].Flip(item);
                else
                    from.SendLocalizedMessage(1042273); // You cannot turn that.
            }

            private static void Up(Item item, Mobile from)
            {
                int floorZ = GetFloorZ(item);

                if (floorZ > int.MinValue && item.Z < (floorZ + 15)) // Confirmed : no height checks here
                    item.Location = new Point3D(item.Location, item.Z + 1);
                else
                    from.SendLocalizedMessage(1042274); // You cannot raise it up any higher.
            }

            private static void Down(Item item, Mobile from)
            {
                int floorZ = GetFloorZ(item);

                if (floorZ > int.MinValue && item.Z > GetFloorZ(item))
                    item.Location = new Point3D(item.Location, item.Z - 1);
                else
                    from.SendLocalizedMessage(1042275); // You cannot lower it down any further.
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
                    ItemData id = TileData.ItemTable[tile.ID & TileData.MaxItemValue];

                    int top = tile.Z; // Confirmed : no height checks here

                    if (id.Surface && !id.Impassable && top > z && top <= item.Z)
                        z = top;
                }

                return z;
            }
        }
    }
}
