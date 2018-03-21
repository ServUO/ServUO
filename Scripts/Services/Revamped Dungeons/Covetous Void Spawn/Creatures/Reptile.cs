using Server;
using System;
 
namespace Server.Mobiles
{
	[CorpseName("an alligator corpse")]
	public class WarAlligator : CovetousCreature
	{
		[Constructable]
		public WarAlligator() : base(AIType.AI_Melee)
		{
			Name = "a war alligator";
			Body = 0xCA;
            BaseSoundID = 660;
		}
		
		[Constructable]
		public WarAlligator(int level, bool voidSpawn) : base(AIType.AI_Melee, level, voidSpawn)
		{
			Name = "a war alligator";
			Body = 0xCA;
            BaseSoundID = 660;
		}
		
		public WarAlligator(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
		}
	}
	
	[CorpseName("a magma lizard corpse")]
	public class MagmaLizard : CovetousCreature
	{
		[Constructable]
		public MagmaLizard() : base(AIType.AI_Melee)
		{
			Name = "a magma lizard";
			Body = 0xCE;
            Hue = Utility.RandomList(0x647, 0x650, 0x659, 0x662, 0x66B, 0x674);
            BaseSoundID = 0x5A;
		}
		
		[Constructable]
		public MagmaLizard(int level, bool voidSpawn) : base(AIType.AI_Melee, level, voidSpawn)
		{
			Name = "a magma lizard";
			Body = 0xCE;
            Hue = Utility.RandomList(0x647, 0x650, 0x659, 0x662, 0x66B, 0x674);
            BaseSoundID = 0x5A;
		}
		
		public MagmaLizard(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
		}
	}
	
	[CorpseName("a drake corpse")]
	public class ViciousDrake : CovetousCreature
	{
		[Constructable]
		public ViciousDrake() : base(AIType.AI_Melee)
		{
			Name = "a vicious drake";
			Body = Utility.RandomList(60, 61);
            BaseSoundID = 362;
		}
		
		[Constructable]
		public ViciousDrake(int level, bool voidSpawn) : base(AIType.AI_Melee, level, voidSpawn)
		{
			Name = "a vicious drake";
			Body = Utility.RandomList(60, 61);
            BaseSoundID = 362;
		}
		
		public ViciousDrake(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
		}
	}
	
	[CorpseName("a wyvern corpse")]
	public class CorruptedWyvern : CovetousCreature
	{
		[Constructable]
		public CorruptedWyvern() : base(AIType.AI_Mage)
		{
			Name = "a corrupted wyvern";
			Body = 62;
            BaseSoundID = 362;
		}
		
		[Constructable]
		public CorruptedWyvern(int level, bool voidSpawn) : base(AIType.AI_Mage, level, voidSpawn)
		{
			Name = "a corrupted wyvern";
			Body = 62;
            BaseSoundID = 362;
		}
		
		public CorruptedWyvern(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
		}
	}
	
	[CorpseName("a covetous wyrm corpse")]
	public class CovetousWyrm : CovetousCreature
	{
		[Constructable]
        public CovetousWyrm()
            : base(AIType.AI_Necro)
		{
			Name = "a covetous wyrm";
			Body = 106;
            BaseSoundID = 362;
		}
		
		[Constructable]
		public CovetousWyrm(int level, bool voidSpawn) : base(AIType.AI_Mage, level, voidSpawn)
		{
			Name = "a covetous wyrm";
			Body = 106;
            BaseSoundID = 362;
		}
		
		public CovetousWyrm(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
		}
	}
}