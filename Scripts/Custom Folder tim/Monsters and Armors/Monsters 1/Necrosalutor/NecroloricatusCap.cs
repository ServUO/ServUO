//Created by Script Creator
using System;
using Server;

namespace Server.Items
{
              public class NecroloricatusCap: Cap
{
              
              [Constructable]
              public NecroloricatusCap()
{

                          Weight = 1;
                          Name = "Necroloricatus";
                          Hue = 0;
              
              Attributes.AttackChance = 15;
              Attributes.BonusHits = 50;
              Attributes.BonusMana = 50;
              Attributes.BonusStam = 50;
              Attributes.CastRecovery = 10;
              Attributes.CastSpeed = 10;
              Attributes.DefendChance = 15;
              Attributes.LowerManaCost = 15;
              Attributes.LowerRegCost = 30;
              Attributes.ReflectPhysical = 15;
              Attributes.RegenHits = 10;
              Attributes.RegenMana = 10;
              Attributes.RegenStam = 10;
              Attributes.SpellDamage = 30;
                  }
              public NecroloricatusCap( Serial serial ) : base( serial )
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
