
using System; 
using Server; 


//--Magery Full Spellbook Start-----------------------------------------------------------

namespace Server.Items 
{ 
   public class FullMagerySpellbook : Spellbook
   { 

      [Constructable] 
      public FullMagerySpellbook()
      {
          
            this.Content = ulong.MaxValue; 
      } 

      public FullMagerySpellbook( Serial serial ) : base( serial ) 
      { 
      } 

      public override void Serialize( GenericWriter writer ) 
      { 
         base.Serialize( writer ); 
         writer.Write( (int) 0 ); 
      } 

      public override void Deserialize( GenericReader reader ) 
      { 
         base.Deserialize( reader ); 
         int version = reader.ReadInt(); 
      } 
   } 
}

//--Full Necromancer Spellbook Start--------------------------------------------------------

namespace Server.Items
{
	public class FullNecroSpellbook : Spellbook
	{
		public override SpellbookType SpellbookType{ get{ return SpellbookType.Necromancer; } }
		public override int BookOffset{ get{ return 100; } }
		public override int BookCount{ get{ return ((Core.SE) ? 17 : 16); } }
      
        [Constructable]
        public FullNecroSpellbook() : this((ulong)0xFFFF)
        {
            this.Content = (ulong)0xFFFF;
        }
       	[Constructable]
		public FullNecroSpellbook( ulong content ) : base( content, 0x2253 )
		{
            Layer = (Core.ML ? Layer.OneHanded : Layer.Invalid);
		}

		public FullNecroSpellbook( Serial serial ) : base( serial )
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
			Layer = Layer.Invalid;
		}
	}
}

//--Full Chivalry Spellbook Start--------------------------------------------------------

namespace Server.Items
{
    public class FullChivalrySpellbook : Spellbook
    {
        public override SpellbookType SpellbookType { get { return SpellbookType.Paladin; } }
        public override int BookOffset { get { return 200; } }
        public override int BookCount { get { return 10; } }
        
        [Constructable]
        public FullChivalrySpellbook() : this((ulong)0x3FF)
        {
            this.Content = (ulong)0x3FF;
        }
        [Constructable]
        public FullChivalrySpellbook(ulong content) : base(content, 0x2252)
        {
            Layer = (Core.ML ? Layer.OneHanded : Layer.Invalid);
        }

        public FullChivalrySpellbook(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            Layer = Layer.Invalid;
        }
    }
}

//--Full Bushido Spellbook Start--------------------------------------------------------

namespace Server.Items
{
    public class FullBushidoSpellbook : Spellbook
    {
        public override SpellbookType SpellbookType { get { return SpellbookType.Samurai; } }
        public override int BookOffset { get { return 400; } }
        public override int BookCount { get { return 6; } }

        [Constructable]
        public FullBushidoSpellbook()  : this((ulong)0x3F)
        {
            this.Content = (ulong)0x3F;
        }
        [Constructable]
        public FullBushidoSpellbook(ulong content) : base(content, 0x238C)
        {
            Layer = (Core.ML ? Layer.OneHanded : Layer.Invalid);
        }

        public FullBushidoSpellbook(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            Layer = Layer.Invalid;
        }
    }
}

//--Full Ninjitsu Spellbook Start--------------------------------------------------------

namespace Server.Items
{
    public class FullNinjitsuSpellbook : Spellbook
    {
        public override SpellbookType SpellbookType { get { return SpellbookType.Ninja; } }
        public override int BookOffset { get { return 500; } }
        public override int BookCount { get { return 8; } }

        [Constructable]
        public FullNinjitsuSpellbook(): this((ulong)0xFF)
        {
            this.Content = (ulong)0xFF;
        }
        [Constructable]
        public FullNinjitsuSpellbook(ulong content)
            : base(content, 0x23A0)
        {
            Layer = (Core.ML ? Layer.OneHanded : Layer.Invalid);
        }

        public FullNinjitsuSpellbook(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            Layer = Layer.Invalid;
        }
    }
}

//--Full Spellweaving Spellbook Start--------------------------------------------------------

namespace Server.Items
{
    public class FullSpellweavingSpellbook : Spellbook
    {
        public override SpellbookType SpellbookType { get { return SpellbookType.Arcanist; } }
        public override int BookOffset { get { return 600; } }
        public override int BookCount { get { return 16; } }

        [Constructable]
        public FullSpellweavingSpellbook() : this((ulong)0)
        {
            this.Content = (ulong)0xFFFF;
        }
        [Constructable]
        public FullSpellweavingSpellbook(ulong content) : base(content, 0x2D50)
        {
            Hue = 0x8A2;

            Layer = Layer.OneHanded;
        }

        public FullSpellweavingSpellbook(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            Layer = Layer.Invalid;
        }
    }
}
//--The End--------------------------------------------------------------------------





