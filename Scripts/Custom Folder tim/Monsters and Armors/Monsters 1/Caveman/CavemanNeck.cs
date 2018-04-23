//Created by Blake Miller (wangchung)
using System;
using Server;
using Server.Items;

namespace Server.Items
{

              public class CavemanNeck: GoldNecklace
{
              
              [Constructable]
              public CavemanNeck()
{

                          Weight = 4;
                          Name = "Bone Shard Necklace of the Caveman";
                          Hue = 2101;
              
              Attributes.AttackChance = 30;
              Attributes.BonusHits = 30;
              Attributes.BonusMana = 30;
	      Attributes.BonusStam = 30;
              Attributes.CastSpeed = 5;
              Attributes.EnhancePotions = 10;
              Attributes.Luck = 450;
              Attributes.NightSight = 1;
              Attributes.ReflectPhysical = 35;
              Attributes.RegenHits = 15;
              Attributes.RegenMana = 15;
              Attributes.RegenStam = 15;
              Attributes.SpellChanneling = 1;
              Attributes.SpellDamage = 35;
              Attributes.WeaponDamage = 35;
	      Attributes.WeaponSpeed = 35;
	      Attributes.BonusStr = 30;
		Attributes.BonusDex = 30;
		Attributes.BonusInt = 30;
                  }
              public CavemanNeck( Serial serial ) : base( serial )
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
