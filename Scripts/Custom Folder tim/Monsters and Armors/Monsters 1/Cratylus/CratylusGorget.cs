//Created by Neptune
using System;
using Server;
using Server.Items;


namespace Server.Items
{
              public class CratylusGorget: LeatherGorget
{
              
              [Constructable]
              public CratylusGorget() : base( 11126 )
{

                          Name = "Cratylus Hide Gorget";
                          Hue = 2167;
              ItemID = 11126;
              Attributes.AttackChance = 25;
              Attributes.BonusDex = 10;
              Attributes.BonusHits = 15;
              Attributes.BonusInt = 10;
              Attributes.BonusMana = 15;
              Attributes.BonusStam = 15;
              Attributes.DefendChance = 25;
              Attributes.LowerManaCost = 10;
              Attributes.LowerRegCost = 20;
              Attributes.ReflectPhysical = 25;
              Attributes.SpellDamage = 25;
              ColdBonus = 14;
              EnergyBonus = 14;
              FireBonus = 14;
              PhysicalBonus = 14;
              PoisonBonus = 14;
              StrBonus = 10;
                  }
              public CratylusGorget( Serial serial ) : base( serial )
                      {
                      }
              
              public override void Serialize( GenericWriter writer )
                      {
                          base.Serialize( writer );
                          writer.Write( (int) 0 );
                      }
              
              public override void Deserialize(GenericReader reader)
                      {
                          base.Deserialize( reader );
                          int version = reader.ReadInt();
                      }
                  }
              }
