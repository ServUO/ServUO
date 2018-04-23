using System;
using Server;
using Server.Items;

namespace Server.Items
{
              public class RingOfTheGoddess: SilverRing

{
             public override int ArtifactRarity{ get{ return 19; } } 

              [Constructable]
              public RingOfTheGoddess()
{

                          Weight = 1 ;
                          Name = "Ring of The Goddess";
                          Hue = 1150;
              
              Attributes.BonusDex = 5;
              Attributes.BonusHits = 10;
              Attributes.BonusInt = 5;
              Attributes.BonusMana = 10;
              Attributes.BonusStam = 10;
              Attributes.CastRecovery = 1;
              Attributes.CastSpeed = 1;
              Attributes.LowerManaCost = 5;
              Attributes.LowerRegCost = 15;
              Attributes.NightSight = 1;
                  }
              public RingOfTheGoddess( Serial serial ) : base( serial )
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
