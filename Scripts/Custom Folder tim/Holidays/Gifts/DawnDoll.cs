//
//	Santa Quest 2013 - version 2.0, by Arachaic
//
//
using System;using Server;namespace Server.Items
{
public class DawnDoll : Item
{
[Constructable]
public DawnDoll() : this( 1 )
{}
[Constructable]
public DawnDoll( int amountFrom, int amountTo ) : this( Utility.RandomMinMax( amountFrom, amountTo ) )
{}
[Constructable]
public DawnDoll( int amount ) : base( 9723 )
{
Stackable = false;
Weight = 3.00;
Amount = amount;
Name = "Dawn Doll";
Hue = 0x0;
}
public DawnDoll( Serial serial ) : base( serial )
{}
public override void Serialize( GenericWriter writer )
{
base.Serialize( writer );
writer.Write( (int) 0 ); // version
}
public override void Deserialize( GenericReader reader )
{
base.Deserialize( reader ); int version = reader.ReadInt(); }}}
