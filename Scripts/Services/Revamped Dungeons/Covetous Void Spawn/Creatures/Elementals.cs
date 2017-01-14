using Server;
using System;
 
namespace Server.Mobiles
{
	[CorpseName("an earth elemental corpse")]
	public class CovetousEarthElemental : CovetousCreature
	{
		[Constructable]
		public CovetousEarthElemental() : base(AIType.AI_Melee)
		{
			Name = "an earth elemental";
			Body = 14;
            BaseSoundID = 268;
		}
		
		[Constructable]
		public CovetousEarthElemental(int level, bool voidSpawn) : base(AIType.AI_Melee, level, voidSpawn)
		{
			Name = "an earth elemental";
			Body = 14;
            BaseSoundID = 268;
		}
		
		public CovetousEarthElemental(Serial serial) : base(serial)
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
	
	[CorpseName("a water elemental corpse")]
	public class CovetousWaterElemental : CovetousCreature
	{
		[Constructable]
		public CovetousWaterElemental() : base(AIType.AI_Mage)
		{
			Name = "a water elemental";
			Body = 16;
            BaseSoundID = 278;
		}
		
		[Constructable]
		public CovetousWaterElemental(int level, bool voidSpawn) : base(AIType.AI_Melee, level, voidSpawn)
		{
			Name = "a water elemental";
			Body = 16;
            BaseSoundID = 278;
		}
		
		public CovetousWaterElemental(Serial serial) : base(serial)
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
	
	[CorpseName("a vortex elemental corpse")]
	public class VortexElemental : CovetousCreature
	{
		[Constructable]
		public VortexElemental() : base(AIType.AI_Melee)
		{
			Name = "a vortex elemental";
			Body = 13;
            Hue = 0x4001;
            BaseSoundID = 655;
		}
		
		[Constructable]
		public VortexElemental(int level, bool voidSpawn) : base(AIType.AI_Melee, level, voidSpawn)
		{
			Name = "a vortex elemental";
			Body = 13;
            Hue = 0x4001;
            BaseSoundID = 655;
		}
		
		public VortexElemental(Serial serial) : base(serial)
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
	
	[CorpseName("a searing elemental corpse")]
	public class SearingElemental : CovetousCreature
	{
		[Constructable]
		public SearingElemental() : base(AIType.AI_Mage)
		{
			Name = "a searing elemental";
			Body = 15;
            BaseSoundID = 838;
		}
		
		[Constructable]
		public SearingElemental(int level, bool voidSpawn) : base(AIType.AI_Mage, level, voidSpawn)
		{
			Name = "a searing elemental";
			Body = 15;
            BaseSoundID = 838;
		}
		
		public SearingElemental(Serial serial) : base(serial)
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
	
	[CorpseName("a venom elemental corpse")]
	public class VenomElemental : CovetousCreature
	{
		[Constructable]
		public VenomElemental() : base(AIType.AI_Mage)
		{
			Name = "a venom elemental";
			Body = 162;
            BaseSoundID = 263;
		}
		
		[Constructable]
		public VenomElemental(int level, bool voidSpawn) : base(AIType.AI_Mage, level, voidSpawn)
		{
			Name = "a venom elemental";
			Body = 162;
            BaseSoundID = 263;
		}
		
		public override bool BleedImmune { get { return true; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }
        public override Poison HitPoison { get { return Poison.Lethal; } }
        public override double HitPoisonChance { get { return 0.75; } }
        public override int TreasureMapLevel { get { return 5; } }
		
		public VenomElemental(Serial serial) : base(serial)
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