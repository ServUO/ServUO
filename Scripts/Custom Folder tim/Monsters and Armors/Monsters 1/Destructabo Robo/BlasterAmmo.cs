/* This file was created with
Ilutzio's Questmaker. Enjoy! */
using System;using Server;namespace Server.Items
{
public class BlasterAmmo : Item
{
[Constructable]
public BlasterAmmo() : this( 1 )
{}
[Constructable]
public BlasterAmmo( int amountFrom, int amountTo ) : this( Utility.RandomMinMax( amountFrom, amountTo ) )
{}
[Constructable]

///////////The hexagon value ont he line below is the ItemID
public BlasterAmmo( int amount ) : base( 6249 )
{


///////////Item name
Name = "Blaster Ammo";

///////////Item hue
Hue = 1985;

///////////Stackable
Stackable = true;

///////////Weight of one item
Weight = 0.01;
Amount = amount;

}
public BlasterAmmo( Serial serial ) : base( serial )
{}
public override void Serialize( GenericWriter writer )
{
base.Serialize( writer );
writer.Write( (int) 0 ); // version
}
public override void Deserialize( GenericReader reader )
{
base.Deserialize( reader ); int version = reader.ReadInt(); }}}
