using Server;
using System;
 
namespace Server.Mobiles
{
	[CorpseName("a lizardman corpse")]
	public class LizardmanWitchdoctor : CovetousCreature
	{
		[Constructable]
		public LizardmanWitchdoctor() : base(AIType.AI_Mage)
		{
			Name = "a lizardman witchdoctor";
			Body = Utility.RandomList(35, 36);
            BaseSoundID = 417;
		}
		
		[Constructable]
		public LizardmanWitchdoctor(int level, bool voidSpawn) : base(AIType.AI_Mage, level, voidSpawn)
		{
            Name = "a lizardman witchdoctor";
			Body = Utility.RandomList(35, 36);
            BaseSoundID = 417;
		}
		
		public LizardmanWitchdoctor(Serial serial) : base(serial)
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
	
	[CorpseName("an orcish corpse")]
	public class OrcFootSoldier : CovetousCreature
	{
		[Constructable]
		public OrcFootSoldier() : base(AIType.AI_Melee)
		{
			Name = "an orc foot soldier";
			Body = 17;
            BaseSoundID = 0x45A;
		}
		
		[Constructable]
		public OrcFootSoldier(int level, bool voidSpawn) : base(AIType.AI_Melee, level, voidSpawn)
		{
			Name = "an orc foot soldier";
			Body = 17;
            BaseSoundID = 0x45A;
		}
		
		public OrcFootSoldier(Serial serial) : base(serial)
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
	
	[CorpseName("a ratman corpse")]
	public class RatmanAssassin : CovetousCreature
	{
		[Constructable]
		public RatmanAssassin() : base(AIType.AI_Melee)
		{
			Name = "ratman assassin";
			Body = 42;
            BaseSoundID = 437;
		}
		
		[Constructable]
		public RatmanAssassin(int level, bool voidSpawn) : base(AIType.AI_Melee, level, voidSpawn)
		{
			Name = "ratman assassin";
			Body = 42;
            BaseSoundID = 437;
		}
		
		public RatmanAssassin(Serial serial) : base(serial)
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
	
	[CorpseName("an ogre corpse")]
	public class OgreBoneCrusher : CovetousCreature
	{
		[Constructable]
		public OgreBoneCrusher() : base(AIType.AI_Melee)
		{
			Name = "an ogre bone crusher";
			Body = 1;
			BaseSoundID = 427;
		}
		
		[Constructable]
		public OgreBoneCrusher(int level, bool voidSpawn) : base(AIType.AI_Melee, level, voidSpawn)
		{
			Name = "an ogre bone crusher";
			Body = 1;
			BaseSoundID = 427;
		}
		
		public OgreBoneCrusher(Serial serial) : base(serial)
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
	
	[CorpseName("a titan corpse")]
	public class TitanRockHunter : CovetousCreature
	{
		[Constructable]
		public TitanRockHunter() : base(AIType.AI_Mage)
		{
			Name = "a titan rockhurler";
			Body = 76;
            BaseSoundID = 609;
		}
		
		[Constructable]
		public TitanRockHunter(int level, bool voidSpawn) : base(AIType.AI_Mage, level, voidSpawn)
		{
			Name = "a titan rockhurler";
			Body = 76;
            BaseSoundID = 609;
		}
		
		public TitanRockHunter(Serial serial) : base(serial)
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
