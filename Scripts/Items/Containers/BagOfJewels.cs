  using System;
     using Server;
     using Server.Items;

     namespace Server.Items
     {
          public class BagOfJewels : Bag
          {
                    [Constructable]
                    public BagOfJewels() : this( 1 )
                    {
                        this.LootType = LootType.Blessed;
                        this.Weight = 4.0;
                        this.Movable = false;
                        this.Hue = 53;
                    }
                    [Constructable]
                    public BagOfJewels( int amount )
                    {
                         DropItem( new Diamond( 10 ) );
                         DropItem( new Ruby( 10 ) );
                         DropItem( new Emerald( 10 ) );
                    }

                    public override int LabelNumber
                    {
                        get
                        {
                            return 1075307; // Bag of Jewels
                        }
                    }

                    public BagOfJewels(Serial serial)
                        : base(serial)
               {
               }

               public override void Serialize( GenericWriter writer )
               {
                    base.Serialize( writer );

                     writer.Write( (int) 0 ); // version
               }

               public override void Deserialize( GenericReader reader )
               {
                    base.Deserialize( reader );

                    int version = reader.ReadInt();
               }
          }
     }