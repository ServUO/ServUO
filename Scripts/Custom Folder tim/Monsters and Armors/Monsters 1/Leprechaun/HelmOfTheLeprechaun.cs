//Created by Cherokee/Mule II aka Hotshot
using System;
using Server;
using Server.Items;
namespace Server.Items{

              public class HelmOfTheLeprechaun: PlateHelm
{
              
              [Constructable]
              public HelmOfTheLeprechaun()
{

                          Weight = 1;
                          Name = "Helm Of The Leprechaun";
                          Hue = 69;
              
              Attributes.AttackChance = 25;
              Attributes.LowerManaCost = 50;
              Attributes.LowerRegCost = 50;
              Attributes.Luck = 1000;
              Attributes.NightSight = 1;
              Attributes.ReflectPhysical = 25;
              Attributes.SpellDamage = 25;
              Attributes.WeaponDamage = 25;
              ArmorAttributes.SelfRepair = 5;
                  }
              public HelmOfTheLeprechaun( Serial serial ) : base( serial )
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
