using Server.ContextMenus;
using Server.Engines.Points;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Items
{
	[FlipableAddon(Direction.South, Direction.East)]
	public class SacrificialAltarAddon : BaseTrashAddon
	{
		public override int LabelNumber => 1074818;  // Sacrificial Altar

		public override bool Security => false;

		public override int DefaultGumpID => 0x9;
		public override int DefaultDropSound => 740;

		public override BaseAddonContainerDeed Deed => new SacrificialAltarDeed();

		[Constructable]
		public SacrificialAltarAddon()
			: base(0x2A9B)
		{
			Direction = Direction.South;

			AddComponent(new LocalizedContainerComponent(0x2A9A, 1074818), 1, 0, 0);
		}

		public SacrificialAltarAddon(Serial serial)
			: base(serial)
		{
		}

		public virtual void Flip(Direction direction)
		{
			switch (direction)
			{
				case Direction.East:
				{
					ItemID = 0x2A9C;

					AddComponent(new LocalizedContainerComponent(0x2A9D, 1074818), 0, -1, 0);
				}
				break;

				case Direction.South:
				{
					ItemID = 0x2A9B;

					AddComponent(new LocalizedContainerComponent(0x2A9A, 1074818), 1, 0, 0);
				}
				break;
			}
		}

		protected override void OnEmpty()
		{
			base.OnEmpty();

			var location = new Point3D(X, Y, Z + 10);

			Effects.SendLocationEffect(location, Map, 0x3709, 10, 10, 0x356, 0);
			Effects.PlaySound(location, Map, 0x32E);
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

    public class SacrificialAltarDeed : BaseAddonContainerDeed
    {
        public override int LabelNumber => 1074818;  // Sacrificial Altar

        [Constructable]
        public SacrificialAltarDeed()
        {
            LootType = LootType.Blessed;
        }

        public SacrificialAltarDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonContainer Addon => new SacrificialAltarAddon();

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadEncodedInt();
        }
    }
}
