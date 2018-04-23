//Created by Script Creator
using System;
using Server;

namespace Server.Items
{
              public class AncientRobe: Robe
{
              
              [Constructable]
              public AncientRobe()
{

                          Weight = 5;
                          Name = "Robe of the Ancients";
                          Hue = 0;
              
              Attributes.AttackChance = 100;
              Attributes.BonusInt = 100;
              Attributes.DefendChance = 100;
              Attributes.LowerManaCost = 50;
              Attributes.LowerRegCost = 100;
                  }
              public AncientRobe( Serial serial ) : base( serial )
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
