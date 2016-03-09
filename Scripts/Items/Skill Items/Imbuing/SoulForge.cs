using System;
using Server;

namespace Server.Items
{
	public class SoulForge : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new SoulForgeDeed(); } }

		[Constructable]
		public SoulForge()
		{
            AddComponent(new AddonComponent(0x4277), 0, 0, 0);
            AddComponent(new AddonComponent(0x4278), +1, 0, 0);
            AddComponent(new AddonComponent(0x4279), +2, 0, 0);
            AddComponent(new AddonComponent(0x427A), +3, 0, 0);

            AddComponent(new AddonComponent(0x427B), 0, +1, 0);
            AddComponent(new AddonComponent(0x427C), +1, +1, 0);
            AddComponent(new AddonComponent(0x427D), +2, +1, 0);
            AddComponent(new AddonComponent(0x427E), +3, +1, 0);

            AddComponent(new AddonComponent(0x427F), 0, +2, 0);
            AddComponent(new AddonComponent(0x4280), +1, +2, 0);
            AddComponent(new AddonComponent(0x4281), +2, +2, 0);
            AddComponent(new AddonComponent(0x4282), +3, +2, 0);

            AddComponent(new AddonComponent(0x4283), 0, +3, 0);
            AddComponent(new AddonComponent(0x4284), +1, +3, 0);
            AddComponent(new AddonComponent(0x4285), +2, +3, 0);
            AddComponent(new AddonComponent(0x4286), +3, +3, 0);
        }

        public SoulForge( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class SoulForgeDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new SoulForge(); } }
		public override int LabelNumber{ get{ return 1031696; } }

		[Constructable]
		public SoulForgeDeed()
		{
		}

		public SoulForgeDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}