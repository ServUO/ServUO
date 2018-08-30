//Created by Tom Sibilsky aka Neptune
using System;
using Server;
using Server.Items;


namespace Server.Items
{
              public class TwistedGloves: DragonGloves
{
              
              [Constructable]
              public TwistedGloves()
{

                          Weight = 3;
                          Name = "Gloves of Twisted Metal";
                          Hue = 2022;
              
              Attributes.AttackChance = 30;
              Attributes.BonusDex = 22;
	      Attributes.BonusStr = 30;
              Attributes.BonusHits = 30;
              Attributes.BonusInt = 21;
              Attributes.BonusMana = 70;
              Attributes.BonusStam = 20;
              Attributes.CastRecovery = 6;
              Attributes.CastSpeed = 3;
              Attributes.DefendChance = 25;
              Attributes.LowerManaCost = 26;
              Attributes.LowerRegCost = 29;
              Attributes.NightSight = 1;
              Attributes.ReflectPhysical = 20;
              Attributes.RegenHits = 6;
              Attributes.RegenMana = 6;
              Attributes.RegenStam = 6;
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
              public TwistedGloves( Serial serial ) : base( serial )
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
