//
//	Santa Quest 2013 - version 2.0, by Arachaic
//
//
using System;using Server;namespace Server.Items
{
public class LordBlackthornDoll : Item
{
[Constructable]
public LordBlackthornDoll() : this( 1 )
{}
[Constructable]
public LordBlackthornDoll( int amountFrom, int amountTo ) : this( Utility.RandomMinMax( amountFrom, amountTo ) )
{}
[Constructable]
public LordBlackthornDoll( int amount ) : base( 9721 )
{
Stackable = false;
Weight = 3.00;
Amount = amount;
Name = "Lord Blackthorn Doll";
Hue = 0x0;
}
public LordBlackthornDoll( Serial serial ) : base( serial )
{}
public override void Serialize( GenericWriter writer )
{
base.Serialize( writer );
writer.Write( (int) 0 ); // version
}
public override void Deserialize( GenericReader reader )
{
base.Deserialize( reader ); int version = reader.ReadInt(); }}}
