//Created by Script Creator
using System;
using Server;

namespace Server.Items
{
              public class SuicideBoots: FurBoots
{
              
              [Constructable]
              public SuicideBoots()
{

                          Weight = 5;
                          Name = "Suicide Boots";
                          Hue = 1995;
              
              Attributes.AttackChance = 25;
              Attributes.DefendChance = 25;
              Attributes.LowerManaCost = 25;
              Attributes.LowerRegCost = 25;
              Attributes.ReflectPhysical = 25;
              Attributes.RegenHits = 5;
              Attributes.RegenMana = 5;
              Attributes.RegenStam = 5;
                  }
              public SuicideBoots( Serial serial ) : base( serial )
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
