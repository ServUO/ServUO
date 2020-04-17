using Server.Gumps;
using Server.Multis;
using Server.Network;

namespace Server.Items
{
    public class ClawFootTubAddon : BaseAddon
    {
        [Constructable]
        public ClawFootTubAddon(DirectionType type)
        {
            switch (type)
            {
                case DirectionType.South:
                    AddComponent(new LocalizedAddonComponent(0x996D, 1123299), 0, -1, 0);
                    AddComponent(new LocalizedAddonComponent(0x996C, 1123299), 0, 0, 0);
                    AddComponent(new LocalizedAddonComponent(0x996B, 1123299), 0, 1, 0);
                    break;
                case DirectionType.East:
                    AddComponent(new LocalizedAddonComponent(0x9978, 1123299), -1, 0, 0);
                    AddComponent(new LocalizedAddonComponent(0x9977, 1123299), 0, 0, 0);
                    AddComponent(new LocalizedAddonComponent(0x9976, 1123299), 1, 0, 0);
                    break;
            }
        }

        public override void OnComponentUsed(AddonComponent c, Mobile from)
        {
            if ((from.InRange(c.Location, 3)))
            {
                BaseHouse house = BaseHouse.FindHouseAt(from);

                if (house != null && (house.IsOwner(from) || (house.LockDowns.ContainsKey(this) && house.LockDowns[this] == from)))
                {
                    Components.ForEach(x =>
                    {
                        switch (x.ItemID)
                        {
                            case 0x996D:
                                {
                                    x.ItemID = 0x9972;
                                    break;
                                }
                            case 0x9972:
                                {
                                    x.ItemID = 0x996D;
                                    break;
                                }
                            case 0x9978:
                                {
                                    x.ItemID = 0x997D;
                                    break;
                                }
                            case 0x997D:
                                {
                                    x.ItemID = 0x9978;
                                    break;
                                }
                        }
                    });
                }
                else
                {
                    from.SendLocalizedMessage(502092); // You must be in your house to do this.
                }
            }
            else
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            }
        }

        public ClawFootTubAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new ClawFootTubDeed();

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

    public class ClawFootTubDeed : BaseAddonDeed, IRewardOption
    {
        public override int LabelNumber => 1154632;  // Claw Foot Tub

        public override BaseAddon Addon => new ClawFootTubAddon(_Direction);

        private DirectionType _Direction;

        [Constructable]
        public ClawFootTubDeed()
            : base()
        {
            LootType = LootType.Blessed;
        }

        public ClawFootTubDeed(Serial serial)
            : base(serial)
        {
        }

        public void GetOptions(RewardOptionList list)
        {
            list.Add((int)DirectionType.South, 1075386); // South
            list.Add((int)DirectionType.East, 1075387); // East
        }

        public void OnOptionSelected(Mobile from, int choice)
        {
            _Direction = (DirectionType)choice;

            if (!Deleted)
                base.OnDoubleClick(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.CloseGump(typeof(AddonOptionGump));
                from.SendGump(new AddonOptionGump(this, 1154194)); // Choose a Facing:
            }
            else
            {
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
            }
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
