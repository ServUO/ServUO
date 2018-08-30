using System;
using Server;
using Server.Items;

namespace Server.Items
{
   public class LeprechaunCloak : Item
   {

      [Constructable]
      public LeprechaunCloak() : base( 0x1515 )
      {

         Weight = 5.0;
         Name = "A Leprechaun Cloak";
         Hue = 0x23D;
         Layer = Layer.Cloak;
      }

      public LeprechaunCloak( Serial serial ) : base( serial )
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
      public class LeprechaunPants : Item
      {

      [Constructable]
      public LeprechaunPants() : base( 0x1539 )
      {

         Weight = 5.0;
         Name = "A pair of Leprechaun Pants";
         Hue = 0x23D;
         Layer = Layer.Pants;
      }

      public LeprechaunPants( Serial serial ) : base( serial )
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
      public class LeprechaunBoots : Item
      {

      [Constructable]
      public LeprechaunBoots() : base( 0x170B )
      {

         Weight = 5.0;
         Name = "A pair of Leprechaun Boots";
         Hue = 1;
         Layer = Layer.Shoes;
      }

      public LeprechaunBoots( Serial serial ) : base( serial )
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
      public class LeprechaunShirt : Item
      {

      [Constructable]
      public LeprechaunShirt() : base( 0x1EFD )
      {

         Weight = 5.0;
         Name = "A Leprechaun Shirt";
         Layer = Layer.Shirt;
      }

      public LeprechaunShirt( Serial serial ) : base( serial )
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
      public class LeprechaunHat1 : Item
      {

      [Constructable]
      public LeprechaunHat1() : base( 0x171A )
      {

         Weight = 5.0;
         Name = "A Leprechaun hat";
         Hue = 0x23D;
         Layer = Layer.Helm;
      }

      public LeprechaunHat1( Serial serial ) : base( serial )
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
      public class LeprechaunHat2 : Item
      {

      [Constructable]
      public LeprechaunHat2() : base( 0x1718 )
      {

         Weight = 5.0;
         Name = "A Leprechaun hat";
         Hue = 0x23D;
         Layer = Layer.Helm;
      }

      public LeprechaunHat2( Serial serial ) : base( serial )
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
