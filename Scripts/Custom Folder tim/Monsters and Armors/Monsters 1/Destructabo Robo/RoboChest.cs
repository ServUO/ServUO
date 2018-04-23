
using System;
using Server;
using Server.Items;


namespace Server.Items
{
              public class RoboChest: PlateChest
{
              
              [Constructable]
              public RoboChest()
{

                          Weight = 3;
                          Name = "Chest of Destructabo Robo";
                          Hue = 1985;
              
              Attributes.AttackChance = 50;
              Attributes.BonusInt = 50;
              Attributes.BonusHits = 100;
              Attributes.DefendChance = 25;
              Attributes.LowerManaCost = 5;
              Attributes.LowerRegCost = 10;
              Attributes.ReflectPhysical = 15;
              Attributes.RegenStam = 25;
              Attributes.SpellDamage = 10;
              ArmorAttributes.SelfRepair = 5;
              ColdBonus = Utility.Random( 1, 10 );
              EnergyBonus = Utility.Random( 1, 10 );
              FireBonus = Utility.Random( 1, 10 );
              PhysicalBonus = Utility.Random( 1, 10 );
              PoisonBonus = Utility.Random( 1, 10 );
                  }
              public RoboChest( Serial serial ) : base( serial )
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
