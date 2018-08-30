//Created by Atown
using System;
using Server;


namespace Server.Items
{
              public class VoidKnightChest: DragonChest
{
              
              [Constructable]
              public VoidKnightChest()
{

                          Name = "Void Knight Chest";
                          Hue = 1931;
              
              Attributes.AttackChance = 25;
              Attributes.BonusDex = 15;
              Attributes.BonusHits = 20;
              Attributes.DefendChance = 20;
              Attributes.LowerManaCost = 15;
              Attributes.LowerRegCost = 15;
              Attributes.RegenHits = 5;
              Attributes.RegenMana = 5;
              Attributes.RegenStam = 5;
              ArmorAttributes.SelfRepair = 15;
              StrBonus = 10;
                  }
              public VoidKnightChest( Serial serial ) : base( serial )
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
