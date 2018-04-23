//Created by Cherokee/Mule II aka Hotshot
using System;
using Server;
using Server.Items;

namespace Server.Items{

              public class LeprechaunProtection: Buckler
{
              
              [Constructable]
              public LeprechaunProtection()
{

                          Weight = 1;
                          Name = "Leprechaun Protection";
                          Hue = 69;
              
              Attributes.AttackChance = 20;
              Attributes.BonusDex = 20;
              Attributes.BonusHits = 20;
              Attributes.BonusInt = 20;
              Attributes.BonusMana = 20;
              Attributes.BonusStam = 20;
              Attributes.DefendChance = 20;
              Attributes.EnhancePotions = 100;
              Attributes.LowerManaCost = 50;
              Attributes.LowerRegCost = 50;
              Attributes.Luck = 1000;
              Attributes.ReflectPhysical = 20;
              Attributes.RegenHits = 10;
              Attributes.RegenMana = 10;
              Attributes.RegenStam = 10;
              Attributes.SpellChanneling = 1;
              ArmorAttributes.LowerStatReq = 100;
              ArmorAttributes.SelfRepair = 20;
                  }
              public LeprechaunProtection( Serial serial ) : base( serial )
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
