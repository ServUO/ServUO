/* This file was created with
Ilutzio's Questmaker. Enjoy! */
using System;using Server;namespace Server.Items
{
public class TitanEssence : Item
{
[Constructable]
public TitanEssence() : this( 1 )
{}
[Constructable]
public TitanEssence( int amountFrom, int amountTo ) : this( Utility.RandomMinMax( amountFrom, amountTo ) )
{}
[Constructable]

///////////The hexagon value ont he line below is the ItemID
public TitanEssence( int amount ) : base( 6249 )
{


///////////Item name
Name = "The Essence of a Titan's Might";

///////////Item hue
Hue = 2786;

///////////Stackable
Stackable = false;

///////////Weight of one item
Weight = 0.01;
Amount = amount;

}
public TitanEssence( Serial serial ) : base( serial )
{}
public override void Serialize( GenericWriter writer )
{
base.Serialize( writer );
writer.Write( (int) 0 ); // version
}
public override void Deserialize( GenericReader reader )
{
base.Deserialize( reader ); int version = reader.ReadInt(); }}}
