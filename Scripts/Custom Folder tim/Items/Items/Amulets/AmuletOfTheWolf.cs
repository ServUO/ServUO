//Created by David aka EvilPounder
//Shard: Lords of UO

using System;
using Server;
using Server.Items;

namespace Server.Items
	{
		public class AmuletOfTheWolf : SilverNecklace
	{
	[Constructable]
	public AmuletOfTheWolf()
	{
		Weight = 0.2;
		Name = "Amulet Of The Wolf";
		Layer = Layer.Neck;
		Hue = 0;
	}

		public override void OnDoubleClick( Mobile m )
	{
		if( Parent != m )
	{
		m.SendMessage( "Put It On Then Try.....durr.." );
	}
		else
	{
		if ( m.Body == 400 )
	{
		m.SendMessage( "you feel the amulet's power changeing you." );
		m.PlaySound( 230 );
		m.BodyMod = 100;
		m.RemoveItem(this);
		m.EquipItem(this);
		if( m.Kills >= 5)
	{
		m.Criminal = true;
	}
		if( m.GuildTitle != null)
	{
		m.DisplayGuildTitle = true;
	}
	}
		else if ( m.BodyMod == 100 )
	{
		m.SendMessage( "you feel the amulet's power go away." );
		m.PlaySound( 73 );
		m.BodyMod = 0;
		m.DisplayGuildTitle = false;
		m.Criminal = false;
		m.RemoveItem(this);
		m.EquipItem(this);
	}
		else if ( m.Body == 401 )
	{
		m.SendMessage( "you feel the amulet's power changeing you." );
		m.PlaySound( 230 );
		m.BodyMod = 100;
		m.DisplayGuildTitle = false;
		m.Criminal = false;
		m.RemoveItem(this);
		m.EquipItem(this);
	}
		else if ( m.BodyMod == 100 )
	{
		m.SendMessage( "you feel the amulet's power go away." );
		m.PlaySound( 73 );
		m.BodyMod = 0;
		m.DisplayGuildTitle = false;
		m.Criminal = false;
		m.RemoveItem(this);
		m.EquipItem(this);
	}

	}
	}


	public override void OnRemoved( Object o )
	{
		if( o is Mobile )
	{
		((Mobile)o).NameMod = null;
	}
		if( o is Mobile && ((Mobile)o).Kills >= 5)
	{
		((Mobile)o).Criminal = true;
	}
		if( o is Mobile && ((Mobile)o).GuildTitle != null )
	{
		((Mobile)o).DisplayGuildTitle = true;
	}
		base.OnRemoved( o );
	}

	public AmuletOfTheWolf( Serial serial ) : base( serial )
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