//Created by Neptune
using System;
using Server;


namespace Server.Items
{
              public class AncientDragonLordHelm: DragonHelm
{
              
              [Constructable]
              public AncientDragonLordHelm()
{

                          Weight = 10;
                          Name = "Helm of the Ancient Dragon Lord";
                          Hue = 2355;
              
              Attributes.AttackChance = 10;
              Attributes.BonusDex = 10;
              Attributes.BonusHits = 15;
              Attributes.BonusInt = 10;
              Attributes.BonusMana = 15;
              Attributes.BonusStam = 15;
              Attributes.DefendChance = 10;
              Attributes.ReflectPhysical = 25;
              Attributes.SpellDamage = 25;
              Attributes.WeaponDamage = 20;
	      Attributes.WeaponSpeed = 10;
              ColdBonus = Utility.Random( 10, 20 );
              EnergyBonus = Utility.Random( 10, 20 );
              FireBonus = Utility.Random( 10, 20 );
              PhysicalBonus = Utility.Random( 10, 20 );
              PoisonBonus = Utility.Random( 10, 20 );
              StrBonus = 10;
                  }
              public AncientDragonLordHelm( Serial serial ) : base( serial )
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
