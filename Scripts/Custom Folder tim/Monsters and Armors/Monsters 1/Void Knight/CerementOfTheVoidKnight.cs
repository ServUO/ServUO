//Created by Atown
using System;
using Server;


namespace Server.Items
{
              public class CerementOfTheVoidKnight: HoodedShroudOfShadows
{
              
              [Constructable]
              public CerementOfTheVoidKnight()
{

                          Name = "Cerement Of The Void Knight";
                          Hue = 1931;
              
              Attributes.AttackChance = 15;
              Attributes.BonusHits = 20;
              Attributes.CastRecovery = 5;
              Attributes.CastSpeed = 5;
              Attributes.DefendChance = 10;
              Attributes.RegenHits = 5;
              Attributes.BonusStr = 15;
                  }
              public CerementOfTheVoidKnight( Serial serial ) : base( serial )
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
