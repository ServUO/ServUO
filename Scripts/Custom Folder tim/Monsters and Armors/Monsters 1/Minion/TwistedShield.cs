//Created by Tom Sibilsky aka Neptune
using System;
using Server;
using Server.Items;


namespace Server.Items
{
              public class TwistedShield: ChaosShield
{
              
              [Constructable]
              public TwistedShield()
{

                          Weight = 3;
                          Name = "Shield of Twisted Metal";
                          Hue = 2022;
              
              Attributes.AttackChance = 30;
              Attributes.BonusDex = 10;
	      Attributes.BonusStr = 10;
              Attributes.BonusHits = 50;
              Attributes.BonusInt = 10;
              Attributes.BonusMana = 50;
              Attributes.BonusStam = 50;
              Attributes.DefendChance = 30;
              Attributes.ReflectPhysical = 20;
              Attributes.SpellDamage = 40;
              Attributes.WeaponDamage = 35;
              ArmorAttributes.DurabilityBonus = 10;
              ArmorAttributes.SelfRepair = 10;
              ColdBonus = 15;
              EnergyBonus = 15;
              FireBonus = 15;
              PhysicalBonus = 15;
              PoisonBonus = 15;
              StrBonus = 20;
                  }
              public TwistedShield( Serial serial ) : base( serial )
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
