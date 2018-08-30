using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class AmericanFlagMeetingRoomAddon : BaseAddon
	{
		public override BaseAddonDeed Deed
		{
			get
			{
				return new AmericanFlagMeetingRoomAddonDeed();
			}
		}

		[ Constructable ]
		public AmericanFlagMeetingRoomAddon()
		{
			AddComponent( new AddonComponent( 1296 ), 4, 0, 0 );
			AddComponent( new AddonComponent( 1296 ), 4, -1, 0 );
			AddComponent( new AddonComponent( 1295 ), 6, 0, 0 );
			AddComponent( new AddonComponent( 1296 ), 6, -1, 0 );
			AddComponent( new AddonComponent( 1296 ), 6, -2, 0 );
			AddComponent( new AddonComponent( 1295 ), 6, -3, 0 );
			AddComponent( new AddonComponent( 1295 ), 6, -4, 0 );
			AddComponent( new AddonComponent( 2602 ), 0, 2, 0 );
			AddComponent( new AddonComponent( 1296 ), 0, 2, 0 );
			AddComponent( new AddonComponent( 1295 ), 0, 1, 0 );
			AddComponent( new AddonComponent( 1296 ), 2, -2, 0 );
			AddComponent( new AddonComponent( 1295 ), 2, -3, 0 );
			AddComponent( new AddonComponent( 2602 ), 2, 4, 0 );
			AddComponent( new AddonComponent( 1295 ), 2, 4, 0 );
			AddComponent( new AddonComponent( 1296 ), -1, 0, 0 );
			AddComponent( new AddonComponent( 9007 ), -1, 0, 0 );
			AddComponent( new AddonComponent( 1295 ), -1, -3, 0 );
			AddComponent( new AddonComponent( 9007 ), -1, -2, 0 );
			AddComponent( new AddonComponent( 1295 ), -1, -2, 0 );
			AddComponent( new AddonComponent( 1295 ), 6, 3, 0 );
			AddComponent( new AddonComponent( 1295 ), -4, -2, 0 );
			AddComponent( new AddonComponent( 1296 ), -5, 0, 0 );
			AddComponent( new AddonComponent( 9007 ), -5, 0, 0 );
			AddComponent( new AddonComponent( 1296 ), -5, -3, 0 );
			AddComponent( new AddonComponent( 9007 ), -5, -2, 0 );
			AddComponent( new AddonComponent( 1295 ), -5, -2, 0 );
			AddComponent( new AddonComponent( 2867 ), -5, 3, 0 );
			AddComponent( new AddonComponent( 1295 ), -5, 3, 0 );
			AddComponent( new AddonComponent( 1295 ), 3, -4, 0 );
			AddComponent( new AddonComponent( 1296 ), 4, -4, 0 );
			AddComponent( new AddonComponent( 1295 ), 5, -4, 0 );
			AddComponent( new AddonComponent( 1295 ), -1, 3, 0 );
			AddComponent( new AddonComponent( 2602 ), -1, 3, 0 );
			AddComponent( new AddonComponent( 2602 ), -1, 4, 0 );
			AddComponent( new AddonComponent( 1295 ), -1, 4, 0 );
			AddComponent( new AddonComponent( 2602 ), 0, 4, 0 );
			AddComponent( new AddonComponent( 1295 ), 0, 4, 0 );
			AddComponent( new AddonComponent( 2602 ), 0, 3, 0 );
			AddComponent( new AddonComponent( 1295 ), 0, 3, 0 );
			AddComponent( new AddonComponent( 1296 ), 0, -2, 0 );
			AddComponent( new AddonComponent( 9007 ), -2, -1, 0 );
			AddComponent( new AddonComponent( 1296 ), -2, -1, 0 );
			AddComponent( new AddonComponent( 1295 ), -4, -4, 0 );
			AddComponent( new AddonComponent( 9007 ), -3, -4, 0 );
			AddComponent( new AddonComponent( 1296 ), -3, -4, 0 );
			AddComponent( new AddonComponent( 1296 ), 5, 1, 0 );
			AddComponent( new AddonComponent( 1296 ), 5, -2, 0 );
			AddComponent( new AddonComponent( 1295 ), 5, 3, 0 );
			AddComponent( new AddonComponent( 1296 ), 5, 4, 0 );
			AddComponent( new AddonComponent( 1295 ), -2, -4, 0 );
			AddComponent( new AddonComponent( 9007 ), -3, -2, 0 );
			AddComponent( new AddonComponent( 1296 ), -3, -2, 0 );
			AddComponent( new AddonComponent( 1295 ), -3, -1, 0 );
			AddComponent( new AddonComponent( 1296 ), -3, 0, 0 );
			AddComponent( new AddonComponent( 9007 ), -3, 0, 0 );
			AddComponent( new AddonComponent( 1296 ), -3, 1, 0 );
			AddComponent( new AddonComponent( 2867 ), -5, 4, 0 );
			AddComponent( new AddonComponent( 1296 ), -5, 4, 0 );
			AddComponent( new AddonComponent( 1296 ), -4, 4, 0 );
			AddComponent( new AddonComponent( 7619 ), -4, 4, 0 );
			AddComponent( new AddonComponent( 1296 ), -4, 3, 0 );
			AddComponent( new AddonComponent( 7619 ), -4, 3, 0 );
			AddComponent( new AddonComponent( 1295 ), -2, 4, 0 );
			AddComponent( new AddonComponent( 2602 ), -2, 4, 0 );
			AddComponent( new AddonComponent( 1296 ), -2, 3, 0 );
			AddComponent( new AddonComponent( 2602 ), -2, 3, 0 );
			AddComponent( new AddonComponent( 2602 ), 1, 3, 0 );
			AddComponent( new AddonComponent( 1295 ), 1, 3, 0 );
			AddComponent( new AddonComponent( 2602 ), 1, 4, 0 );
			AddComponent( new AddonComponent( 1296 ), 1, 4, 0 );
			AddComponent( new AddonComponent( 1295 ), 1, -3, 0 );
			AddComponent( new AddonComponent( 9007 ), -5, -4, 0 );
			AddComponent( new AddonComponent( 1295 ), -5, -4, 0 );
			AddComponent( new AddonComponent( 1295 ), -2, -2, 0 );
			AddComponent( new AddonComponent( 9007 ), -2, -3, 0 );
			AddComponent( new AddonComponent( 1296 ), -2, -3, 0 );
			AddComponent( new AddonComponent( 1296 ), 4, 2, 0 );
			AddComponent( new AddonComponent( 1296 ), 4, -3, 0 );
			AddComponent( new AddonComponent( 1296 ), 4, 4, 0 );
			AddComponent( new AddonComponent( 1295 ), 4, 3, 0 );
			AddComponent( new AddonComponent( 1296 ), 3, -1, 0 );
			AddComponent( new AddonComponent( 1296 ), 3, 0, 0 );
			AddComponent( new AddonComponent( 1296 ), 3, -3, 0 );
			AddComponent( new AddonComponent( 1295 ), 3, -2, 0 );
			AddComponent( new AddonComponent( 1295 ), 3, 3, 0 );
			AddComponent( new AddonComponent( 2602 ), 2, 3, 0 );
			AddComponent( new AddonComponent( 1296 ), 2, 3, 0 );
			AddComponent( new AddonComponent( 1295 ), 3, 1, 0 );
			AddComponent( new AddonComponent( 1295 ), 3, 2, 0 );
			AddComponent( new AddonComponent( 1296 ), 6, 5, 0 );
			AddComponent( new AddonComponent( 1295 ), 5, 5, 0 );
			AddComponent( new AddonComponent( 1295 ), 4, 5, 0 );
			AddComponent( new AddonComponent( 1296 ), 3, 5, 0 );
			AddComponent( new AddonComponent( 2602 ), 2, 5, 0 );
			AddComponent( new AddonComponent( 1295 ), 2, 5, 0 );
			AddComponent( new AddonComponent( 2602 ), 1, 5, 0 );
			AddComponent( new AddonComponent( 1295 ), 1, 5, 0 );
			AddComponent( new AddonComponent( 1295 ), 0, 5, 0 );
			AddComponent( new AddonComponent( 2602 ), 0, 5, 0 );
			AddComponent( new AddonComponent( 2602 ), -1, 5, 0 );
			AddComponent( new AddonComponent( 1295 ), -1, 5, 0 );
			AddComponent( new AddonComponent( 1295 ), -2, 5, 0 );
			AddComponent( new AddonComponent( 2602 ), -2, 5, 0 );
			AddComponent( new AddonComponent( 1296 ), -3, 5, 0 );
			AddComponent( new AddonComponent( 1296 ), -4, 5, 0 );
			AddComponent( new AddonComponent( 7617 ), -4, 5, 0 );
			AddComponent( new AddonComponent( 1296 ), -5, 5, 0 );
			AddComponent( new AddonComponent( 2602 ), 2, 2, 0 );
			AddComponent( new AddonComponent( 1295 ), 2, 2, 0 );
			AddComponent( new AddonComponent( 1295 ), 2, 1, 0 );
			AddComponent( new AddonComponent( 1296 ), 6, 4, 0 );
			AddComponent( new AddonComponent( 1295 ), 5, 2, 0 );
			AddComponent( new AddonComponent( 1296 ), 6, 2, 0 );
			AddComponent( new AddonComponent( 1295 ), 6, 1, 0 );
			AddComponent( new AddonComponent( 1295 ), -3, -3, 0 );
			AddComponent( new AddonComponent( 9007 ), -4, -3, 0 );
			AddComponent( new AddonComponent( 1295 ), -4, -3, 0 );
			AddComponent( new AddonComponent( 1296 ), -4, 0, 0 );
			AddComponent( new AddonComponent( 1296 ), -4, -1, 0 );
			AddComponent( new AddonComponent( 9007 ), -4, -1, 0 );
			AddComponent( new AddonComponent( 7618 ), -4, 2, 0 );
			AddComponent( new AddonComponent( 1296 ), -4, 2, 0 );
			AddComponent( new AddonComponent( 9007 ), -4, 1, 0 );
			AddComponent( new AddonComponent( 1295 ), -4, 1, 0 );
			AddComponent( new AddonComponent( 2849 ), -4, 1, 1 );
			AddComponent( new AddonComponent( 1296 ), -5, 1, 0 );
			AddComponent( new AddonComponent( 1295 ), -5, 2, 0 );
			AddComponent( new AddonComponent( 1296 ), 2, 0, 0 );
			AddComponent( new AddonComponent( 1295 ), 2, -1, 0 );
			AddComponent( new AddonComponent( 1296 ), -1, 1, 0 );
			AddComponent( new AddonComponent( 1296 ), -1, 2, 0 );
			AddComponent( new AddonComponent( 2602 ), -1, 2, 0 );
			AddComponent( new AddonComponent( 1295 ), -1, -1, 0 );
			AddComponent( new AddonComponent( 1295 ), 1, -2, 0 );
			AddComponent( new AddonComponent( 1296 ), 1, -1, 0 );
			AddComponent( new AddonComponent( 1295 ), 1, 0, 0 );
			AddComponent( new AddonComponent( 1296 ), 5, -3, 0 );
			AddComponent( new AddonComponent( 1296 ), 4, 1, 0 );
			AddComponent( new AddonComponent( 1296 ), 5, -1, 0 );
			AddComponent( new AddonComponent( 1296 ), 5, 0, 0 );
			AddComponent( new AddonComponent( 1296 ), -5, -1, 0 );
			AddComponent( new AddonComponent( 9007 ), -1, -4, 0 );
			AddComponent( new AddonComponent( 1295 ), -1, -4, 0 );
			AddComponent( new AddonComponent( 1295 ), 0, -4, 0 );
			AddComponent( new AddonComponent( 1295 ), 1, -4, 0 );
			AddComponent( new AddonComponent( 3025 ), 2, -4, 0 );
			AddComponent( new AddonComponent( 1295 ), 2, -4, 0 );
			AddComponent( new AddonComponent( 1296 ), -3, 2, 0 );
			AddComponent( new AddonComponent( 1295 ), -3, 3, 0 );
			AddComponent( new AddonComponent( 1296 ), -3, 4, 0 );
			AddComponent( new AddonComponent( 1296 ), 1, 1, 0 );
			AddComponent( new AddonComponent( 2602 ), 1, 2, 0 );
			AddComponent( new AddonComponent( 1295 ), 1, 2, 0 );
			AddComponent( new AddonComponent( 1295 ), -2, 2, 0 );
			AddComponent( new AddonComponent( 2602 ), -2, 2, 0 );
			AddComponent( new AddonComponent( 1296 ), -2, 1, 0 );
			AddComponent( new AddonComponent( 9007 ), -2, 1, 0 );
			AddComponent( new AddonComponent( 1296 ), -2, 0, 0 );
			AddComponent( new AddonComponent( 1295 ), 0, -3, 0 );
			AddComponent( new AddonComponent( 1295 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 1295 ), 0, -1, 0 );
			AddComponent( new AddonComponent( 1296 ), 4, -2, 0 );
			AddComponent( new AddonComponent( 1296 ), 3, 4, 0 );
			AddonComponent ac;
			ac = new AddonComponent( 7618 );
			ac.Hue = 1153;
			AddComponent( ac, -4, 2, 0 );
			ac = new AddonComponent( 7619 );
			ac.Hue = 1153;
			AddComponent( ac, -4, 3, 0 );
			ac = new AddonComponent( 7619 );
			ac.Hue = 38;
			AddComponent( ac, -4, 4, 0 );
			ac = new AddonComponent( 3025 );
			ac.Hue = 38;
			ac.Name = "Meeting Room/Event Room/Flag Room";
			AddComponent( ac, 2, -4, 0 );

		}

		public AmericanFlagMeetingRoomAddon( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class AmericanFlagMeetingRoomAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new AmericanFlagMeetingRoomAddon();
			}
		}

		[Constructable]
		public AmericanFlagMeetingRoomAddonDeed()
		{
			Name = "AG_American Flag Meeting Room";
		}

		public AmericanFlagMeetingRoomAddonDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 ); // Version
		}

		public override void	Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}