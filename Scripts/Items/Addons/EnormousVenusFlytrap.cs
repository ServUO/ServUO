using Server.ContextMenus;
using Server.Engines.Points;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Items
{
    [FlipableAddon(Direction.South, Direction.East)]
    public class EnormousVenusFlytrapAddon : BaseTrashAddon
	{
		public override int EmptyTriggerCount => 0;

		public override TextDefinition EmptyBroadcast => Utility.Random(1042891, 8);

		public override TextDefinition EmptyMessage => 0;
		public override TextDefinition EmptyWarning => 1154485; // The item will be digested in three minutes

		public override int LabelNumber => 1154462;  // Enormous Venus Flytrap

        public override bool Security => false;

        public override int DefaultGumpID => 0x9;

		public override BaseAddonContainerDeed Deed => new EnormousVenusFlytrapAddonDeed(Hue);

        [Constructable]
        public EnormousVenusFlytrapAddon(int hue)
            : base(0x9967)
        {
            Direction = Direction.South;
            Hue = hue;
        }

        public EnormousVenusFlytrapAddon(Serial serial)
            : base(serial)
        {
        }

        public virtual void Flip(Direction direction)
        {
            switch (direction)
            {
                case Direction.East:
                    ItemID = 0x9968;
                    break;

                case Direction.South:
                    ItemID = 0x9967;
                    break;
            }
        }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.WriteEncodedInt(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadEncodedInt(); // version
		}
	}

    public class EnormousVenusFlytrapAddonDeed : BaseAddonContainerDeed
    {
        public override int LabelNumber => 1154462;  // Enormous Venus Flytrap

        public override BaseAddonContainer Addon => new EnormousVenusFlytrapAddon(Hue);

        [Constructable]
        public EnormousVenusFlytrapAddonDeed()
            : this(Utility.RandomList(26, 33, 233, 1931, 2067))
        {
        }

        [Constructable]
        public EnormousVenusFlytrapAddonDeed(int hue)
        {
            Hue = hue;
            LootType = LootType.Blessed;
        }

        public EnormousVenusFlytrapAddonDeed(Serial serial)
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

            reader.ReadInt();
        }
    }
}
