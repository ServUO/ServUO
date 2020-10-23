using Server.Gumps;
using Server.Multis;
using Server.Network;
using Server.Targeting;
using System;
using System.Linq;

namespace Server.Items
{
    public enum DecorateCommand
    {
        None,
        Turn,
        Up,
        Down,
        GetHue
    }

    public class InteriorDecorator : Item
    {
        public override int LabelNumber => 1041280;  // an interior decorator

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
        public DecorateCommand Command { get; set; }

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

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!InHouse(from))
                Command = DecorateCommand.GetHue;

            if (from.FindGump(typeof(InternalGump)) == null)
                from.SendGump(new InternalGump(from, this));

            if (Command != DecorateCommand.None)
                from.Target = new InternalTarget(this);
        }

        private class InternalGump : Gump
        {
            private readonly InteriorDecorator m_Decorator;

            public InternalGump(Mobile from, InteriorDecorator decorator)
                : base(150, 50)
            {
                m_Decorator = decorator;

                AddBackground(0, 0, 170, 260, 2600);

                AddPage(0);

                if (!InHouse(from))
                {
                    AddButton(40, 36, (decorator.Command == DecorateCommand.GetHue ? 2154 : 2152), 2154, 4, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(80, 41, 100, 20, 1158863, false, false); // Get Hue   
                }
                else
                {
                    AddButton(40, 36, (decorator.Command == DecorateCommand.Turn ? 2154 : 2152), 2154, 1, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(80, 41, 100, 20, 1018323, false, false); // Turn

                    AddButton(40, 86, (decorator.Command == DecorateCommand.Up ? 2154 : 2152), 2154, 2, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(80, 91, 100, 20, 1018324, false, false); // Up

                    AddButton(40, 136, (decorator.Command == DecorateCommand.Down ? 2154 : 2152), 2154, 3, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(80, 141, 100, 20, 1018325, false, false); // Down

                    AddButton(40, 186, (decorator.Command == DecorateCommand.GetHue ? 2154 : 2152), 2154, 4, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(80, 191, 100, 20, 1158863, false, false); // Get Hue                    
                }

                AddHtmlLocalized(0, 0, 0, 0, 4, false, false);
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                DecorateCommand command = DecorateCommand.None;
                Mobile m = sender.Mobile;

                int cliloc = 0;

                switch (info.ButtonID)
                {
                    case 1:
                        cliloc = 1073404; // Select an object to turn.
                        command = DecorateCommand.Turn;
                        break;
                    case 2:
                        cliloc = 1073405; // Select an object to increase its height.
                        command = DecorateCommand.Up;
                        break;
                    case 3:
                        cliloc = 1073406; // Select an object to lower its height.
                        command = DecorateCommand.Down;
                        break;
                    case 4:
                        cliloc = 1158864; // Select an object to get the hue.
                        command = DecorateCommand.GetHue;
                        break;
                }

                if (command != DecorateCommand.None)
                {
                    m_Decorator.Command = command;
                    m.SendGump(new InternalGump(m, m_Decorator));

                    if (cliloc != 0)
                        m.SendLocalizedMessage(cliloc);

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

            private static readonly Type[] _IsSpecialTypes = new Type[]
            {
                typeof(BirdLamp),  typeof(DragonLantern),  typeof(KoiLamp),
                typeof(TallLamp)
            };

            private static bool IsSpecialTypes(Item item)
            {
                return _IsSpecialTypes.Any(t => t == item.GetType());
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Decorator.Command == DecorateCommand.GetHue)
                {
                    int hue = 0;

                    if (targeted is Item)
                        hue = ((Item)targeted).Hue;
                    else if (targeted is Mobile)
                        hue = ((Mobile)targeted).Hue;
                    else
                    {
                        from.Target = new InternalTarget(m_Decorator);
                        return;
                    }

                    from.SendLocalizedMessage(1158862, string.Format("{0}", hue)); // That object is hue ~1_HUE~
                }
                else if (targeted is Item && CheckUse(m_Decorator, from))
                {
                    BaseHouse house = BaseHouse.FindHouseAt(from);
                    Item item = (Item)targeted;

                    bool isDecorableComponent = false;

                    if (m_Decorator.Command == DecorateCommand.Turn && IsSpecialTypes(item))
                    {
                        isDecorableComponent = true;
                    }
                    else if (item is AddonComponent || item is AddonContainerComponent || item is BaseAddonContainer || item is TrophyAddon)
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

                        if (count == 1)
                            isDecorableComponent = true;

                        if (item is TrophyAddon)
                            isDecorableComponent = true;

                        if (item is EnormousVenusFlytrapAddon)
                            isDecorableComponent = true;

                        if (m_Decorator.Command == DecorateCommand.Turn)
                        {
                            if (addon != null)
                            {
                                FlipableAddonAttribute[] attributes = (FlipableAddonAttribute[])addon.GetType().GetCustomAttributes(typeof(FlipableAddonAttribute), false);

                                if (attributes.Length > 0)
                                    isDecorableComponent = true;
                            }
                        }
                    }
                    else if (item is Banner && m_Decorator.Command != DecorateCommand.Turn)
                    {
                        isDecorableComponent = true;
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
                        {
                            from.SendLocalizedMessage(1042273); // You cannot turn that.
                        }
                        else if (item is AddonComponent && m_Decorator.Command == DecorateCommand.Up)
                        {
                            from.SendLocalizedMessage(1042274); // You cannot raise it up any higher.
                        }
                        else if (item is AddonComponent && m_Decorator.Command == DecorateCommand.Down)
                        {
                            from.SendLocalizedMessage(1042275); // You cannot lower it down any further.
                        }
                        else
                        {
                            from.SendLocalizedMessage(1042271); // That is not locked down.
                        }
                    }
                    else if (item is VendorRentalContract)
                    {
                        from.SendLocalizedMessage(1062491); // You cannot use the house decorator on that object.
                    }
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
                    from.CloseGump(typeof(InternalGump));
            }

            private static void Turn(Item item, Mobile from)
            {
                if (item is IFlipable)
                {
                    ((IFlipable)item).OnFlip(from);
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

                    if (addon != null)
                    {
                        FlipableAddonAttribute[] aAttributes = (FlipableAddonAttribute[])addon.GetType().GetCustomAttributes(typeof(FlipableAddonAttribute), false);

                        if (aAttributes.Length > 0)
                        {
                            aAttributes[0].Flip(from, (Item)addon);
                            return;
                        }
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
