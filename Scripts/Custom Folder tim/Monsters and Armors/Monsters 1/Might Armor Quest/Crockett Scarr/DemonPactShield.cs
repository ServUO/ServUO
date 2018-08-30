//Created by Script Creator
using System;
using Server;
using Server.Items;


namespace Server.Items
{
              public class DemonPactShield: ChaosShield
{
              
              [Constructable]
              public DemonPactShield()
{

                          Weight = 5.0;
                          Name = "Demon Pact Shield";
              
              Attributes.AttackChance = 25;
              Attributes.DefendChance = 25;
              Attributes.ReflectPhysical = 25;
              Attributes.WeaponDamage = 25;
              ArmorAttributes.SelfRepair = 5;
              PhysicalBonus = 25;
                  }
              public DemonPactShield( Serial serial ) : base( serial )
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
