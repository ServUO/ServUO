/////////////////////////////////////////////////
//
// Automatically generated by the
// AddonGenerator script by Arya
//
/////////////////////////////////////////////////
using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class LightHouseLightAddon : BaseAddon
	{
		public override BaseAddonDeed Deed
		{
			get
			{
				return new LightHouseLightAddonDeed();
			}
		}

		[ Constructable ]
		public LightHouseLightAddon()
		{
			AddonComponent ac = null;
			ac = new AddonComponent( 6822 );
			AddComponent( ac, 1, 3, 0 );
			ac = new AddonComponent( 6821 );
			AddComponent( ac, 0, 3, 0 );
			ac = new AddonComponent( 6820 );
			AddComponent( ac, -1, 3, 0 );
			ac = new AddonComponent( 6864 );
			ac.Light = LightType.ArchedWindowEast;
			AddComponent( ac, 2, 2, 0 );
			ac = new AddonComponent( 6855 );
			AddComponent( ac, -1, 1, 0 );
			ac = new AddonComponent( 6832 );
			AddComponent( ac, 1, -1, 0 );
			ac = new AddonComponent( 6829 );
			AddComponent( ac, 2, -1, 0 );
			ac = new AddonComponent( 6852 );
			AddComponent( ac, -1, 2, 0 );
			ac = new AddonComponent( 6838 );
			AddComponent( ac, 0, -2, 0 );
			ac = new AddonComponent( 6846 );
			AddComponent( ac, -2, 0, 0 );
			ac = new AddonComponent( 6835 );
			AddComponent( ac, 1, -2, 0 );
			ac = new AddonComponent( 6849 );
			AddComponent( ac, -2, 1, 0 );
			ac = new AddonComponent( 6858 );
			AddComponent( ac, -1, 0, 0 );
			ac = new AddonComponent( 6860 );
			AddComponent( ac, 0, -1, 0 );
			ac = new AddonComponent( 6823 );
			AddComponent( ac, 2, 3, 0 );
			ac = new AddonComponent( 6825 );
			AddComponent( ac, 3, 2, 0 );
			ac = new AddonComponent( 6824 );
			AddComponent( ac, 3, 3, 0 );
			ac = new AddonComponent( 6826 );
			AddComponent( ac, 3, 1, 0 );
			ac = new AddonComponent( 6827 );
			AddComponent( ac, 3, 0, 0 );
			ac = new AddonComponent( 6828 );
			AddComponent( ac, 3, -1, 0 );
			ac = new AddonComponent( 6859 );
			AddComponent( ac, -1, -1, 0 );
			ac = new AddonComponent( 6861 );
			AddComponent( ac, -2, -1, 0 );
			ac = new AddonComponent( 6863 );
			AddComponent( ac, -1, -2, 0 );
			ac = new AddonComponent( 6862 );
			AddComponent( ac, -2, -2, 0 );
			ac = new AddonComponent( 6843 );
			AddComponent( ac, -3, -3, 0 );
			ac = new AddonComponent( 6844 );
			AddComponent( ac, -3, -2, 0 );
			ac = new AddonComponent( 6842 );
			AddComponent( ac, -2, -3, 0 );
			ac = new AddonComponent( 6841 );
			AddComponent( ac, -1, -3, 0 );
			ac = new AddonComponent( 6845 );
			AddComponent( ac, -3, -1, 0 );

		}

		public LightHouseLightAddon( Serial serial ) : base( serial )
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

	public class LightHouseLightAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new LightHouseLightAddon();
			}
		}

		[Constructable]
		public LightHouseLightAddonDeed()
		{
			Name = "LightHouseLight";
		}

		public LightHouseLightAddonDeed( Serial serial ) : base( serial )
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