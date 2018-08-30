// Created by the Script Creator
using System;
using Server;

namespace Server.Items
              {
                  public class GodsMightArmor : BrownBook
                  {
                      [Constructable]
              public GodsMightArmor() : base( "Gods Might Armor", "Hades", 4, false ) // true writable so players can make notes
                      {
                          string[] lines;
                          lines = new string[]
                          {
              "Search out Zeus and",
              "Poseidon among the",
              "dungeons of Felucca",
              "and Titanious among the",
              "Dungeons of Ilshenar",
              "to learn the secret of the",
              "Gods Might armor.",
                          };
		}
                       
              public GodsMightArmor( Serial serial ) : base( serial )
                      {
                      }
              
        public override void Deserialize( GenericReader reader )
                      {
                          base.Deserialize( reader );
              
                          int version = reader.ReadInt();
                      }
              
        public override void Serialize( GenericWriter writer )
                      {
                          base.Serialize( writer );
              
                          writer.Write( (int)0 ); // version
        }
                  }
              }
