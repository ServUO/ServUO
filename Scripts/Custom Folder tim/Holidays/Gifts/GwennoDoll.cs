//
//	Santa Quest 2013 - version 2.0, by Arachaic
//
//
using System;using Server;namespace Server.Items
{
public class GwennoDoll : Item
{
[Constructable]
public GwennoDoll() : this( 1 )
{}
[Constructable]
public GwennoDoll( int amountFrom, int amountTo ) : this( Utility.RandomMinMax( amountFrom, amountTo ) )
{}
[Constructable]
public GwennoDoll( int amount ) : base( 8455 )
{
Stackable = false;
Weight = 3.00;
Amount = amount;
Name = "Limited Edition Gwenno Doll";
LootType = LootType.Blessed;
Hue = 1153;
}
public GwennoDoll( Serial serial ) : base( serial )
{}
public override void Serialize( GenericWriter writer )
{
base.Serialize( writer );
writer.Write( (int) 0 ); // version
}
public override void Deserialize( GenericReader reader )
{
base.Deserialize( reader ); int version = reader.ReadInt(); }}}
