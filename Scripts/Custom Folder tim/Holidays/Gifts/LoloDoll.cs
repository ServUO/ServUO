//
//	Santa Quest 2013 - version 2.0, by Arachaic
//
//
using System;using Server;namespace Server.Items
{
public class LoloDoll : Item
{
[Constructable]
public LoloDoll() : this( 1 )
{}
[Constructable]
public LoloDoll( int amountFrom, int amountTo ) : this( Utility.RandomMinMax( amountFrom, amountTo ) )
{}
[Constructable]
public LoloDoll( int amount ) : base( 8454 )
{
Stackable = false;
Weight = 3.00;
Amount = amount;
Name = "Limited Edition Lolo Doll";
LootType = LootType.Blessed;
Hue = 1153;
}
public LoloDoll( Serial serial ) : base( serial )
{}
public override void Serialize( GenericWriter writer )
{
base.Serialize( writer );
writer.Write( (int) 0 ); // version
}
public override void Deserialize( GenericReader reader )
{
base.Deserialize( reader ); int version = reader.ReadInt(); }}}
