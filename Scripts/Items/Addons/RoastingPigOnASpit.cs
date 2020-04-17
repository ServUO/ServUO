using Server.Gumps;
using Server.Multis;
using Server.Network;
using System;

namespace Server.Items
{
    public class RoastingPigOnASpitAddon : BaseAddon
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextEatTime { get; set; }

        [Constructable]
        public RoastingPigOnASpitAddon(DirectionType type)
        {
            NextEatTime = DateTime.UtcNow + TimeSpan.FromDays(1);

            switch (type)
            {
                case DirectionType.South:
                    AddComponent(new RoastingPigOnASpitComponent(0x9995), 0, 0, 0); //Roasting Pig on a Spit    
                    AddComponent(new LocalizedAddonComponent(0x9994, 1123328), -1, 1, 0); // Pile of Logs
                    break;
                case DirectionType.East:
                    AddComponent(new RoastingPigOnASpitComponent(0x9989), 0, 0, 0); //Roasting Pig on a Spit
                    AddComponent(new LocalizedAddonComponent(0x9988, 1123328), 1, -1, 0); // Pile of Logs
                    break;
            }
        }

        public override void OnComponentUsed(AddonComponent c, Mobile from)
        {
            if (c.LabelNumber != 1123329)
                return;

            if ((from.InRange(c.Location, 3)))
            {
                BaseHouse house = BaseHouse.FindHouseAt(from);

                if (house != null && (house.IsOwner(from) || (house.LockDowns.ContainsKey(this) && house.LockDowns[this] == from)))
                {
                    if (DateTime.UtcNow > NextEatTime)
                    {
                        if (0.6 >= Utility.RandomDouble())
                            from.AddToBackpack(new PulledPorkPlatter());
                        else
                            from.AddToBackpack(new PulledPorkSandwich());

                        from.LocalOverheadMessage(MessageType.Regular, 0x35, 1154556); // *You cut some meat from the roasting pig. It smells delicious!*

                        NextEatTime = DateTime.UtcNow + TimeSpan.FromDays(7);
                    }
                    else
                    {
                        from.SendLocalizedMessage(1154555); //The pig is not quite ready for eating yet.
                    }
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

        public RoastingPigOnASpitAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new RoastingPigOnASpitDeed();

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(NextEatTime);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            NextEatTime = reader.ReadDateTime();
        }
    }

    public class RoastingPigOnASpitComponent : LocalizedAddonComponent
    {
        public override bool ForceShowProperties => true;

        public RoastingPigOnASpitComponent(int id)
            : base(id, 1123329) // Roasting Pig on a Spit
        {
        }

        public RoastingPigOnASpitComponent(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class RoastingPigOnASpitDeed : BaseAddonDeed, IRewardOption
    {
        public override int LabelNumber => 1154557;  // Deed for a Roasting Pig on a Spit

        public override BaseAddon Addon => new RoastingPigOnASpitAddon(_Direction);

        private DirectionType _Direction;

        [Constructable]
        public RoastingPigOnASpitDeed()
            : base()
        {
        }

        public RoastingPigOnASpitDeed(Serial serial)
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
