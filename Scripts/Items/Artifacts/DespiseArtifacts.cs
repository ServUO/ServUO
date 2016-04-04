using Server;
using System;

namespace Server.Items
{
	public class CompassionsEye : GoldRing, ITokunoDyable
	{
		public override bool IsArtifact { get { return true; } }
		public override int LabelNumber { get { return 1153288; } } // Compassion's Eye
		
        [Constructable]
		public CompassionsEye()
		{
			Hue = 1174;
			
			Attributes.BonusInt = 10;
			Attributes.BonusMana = 10;
			Attributes.RegenMana = 2;
			Attributes.Luck = 250;
			Attributes.SpellDamage = 20;
			Attributes.LowerRegCost = 20;
		}
		
		public CompassionsEye(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
		}
	}
	
	public class UnicornManeWovenSandals : Sandals, ITokunoDyable
	{
		public override bool IsArtifact { get { return true; } }
		public override int LabelNumber { get { return 1153289; } } // Unicorn Mane Woven Sandals 

        [Constructable]
		public UnicornManeWovenSandals()
		{
			Hue = 1154;
			
			switch(Utility.Random(6))
			{
				case 0: SAAbsorptionAttributes.EaterKinetic = 2; break;
                case 1: SAAbsorptionAttributes.EaterFire = 2; break;
                case 2: SAAbsorptionAttributes.EaterCold = 2; break;
                case 3: SAAbsorptionAttributes.EaterPoison = 2; break;
                case 4: SAAbsorptionAttributes.EaterEnergy = 2; break;
                case 5: SAAbsorptionAttributes.EaterDamage = 2; break;
			}
			
			Attributes.NightSight = 1;
		}
		
		public UnicornManeWovenSandals(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
		}
	}
	
	public class UnicornManeWovenTalons : LeatherTalons, ITokunoDyable
	{
		public override bool IsArtifact { get { return true; } }
		public override int LabelNumber { get { return 1153314; } } // Unicorn Mane Woven Talons

        [Constructable]
		public UnicornManeWovenTalons()
		{
			Hue = 1154;
			
			switch(Utility.Random(6))
			{
                case 0: SAAbsorptionAttributes.EaterKinetic = 2; break;
                case 1: SAAbsorptionAttributes.EaterFire = 2; break;
                case 2: SAAbsorptionAttributes.EaterCold = 2; break;
                case 3: SAAbsorptionAttributes.EaterPoison = 2; break;
                case 4: SAAbsorptionAttributes.EaterEnergy = 2; break;
                case 5: SAAbsorptionAttributes.EaterDamage = 2; break;
			}
			
			Attributes.NightSight = 1;
		}
		
		public UnicornManeWovenTalons(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
		}
	}

	public class DespicableQuiver : BaseQuiver, ITokunoDyable
	{
		public override bool IsArtifact { get { return true; } }
		public override int LabelNumber { get { return 1153290; } } // Despicable Quiver

        [Constructable]
		public DespicableQuiver() : base(0x2B02)
		{
			Hue = 2671;
			
			DamageIncrease = 10;
			WeightReduction = 30;
			Attributes.BonusDex = 5;
			SkillBonuses.SetValues( 0, SkillName.Archery, 5.0 );
			Attributes.ReflectPhysical = 5;
			Attributes.AttackChance = 5;
			
			switch(Utility.Random(5))
			{
				case 0: Resistances.Physical = 10; break;
                case 1: Resistances.Fire = 10; break;
                case 2: Resistances.Cold = 10; break;
                case 3: Resistances.Poison = 10; break;
                case 4: Resistances.Energy = 10; break;
			}
		}
		
		public DespicableQuiver(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
		}
	}
	
	public class UnforgivenVeil : GargishLeatherWingArmor, ITokunoDyable
	{
		public override bool IsArtifact { get { return true; } }
		public override int LabelNumber { get { return 1153291; } } // Unforgiven Veil 
		
		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

        [Constructable]
		public UnforgivenVeil()
		{
			Hue = 2671;
			
			Attributes.BonusDex = 5;
			SkillBonuses.SetValues( 0, SkillName.Throwing, 5.0 );
			Attributes.ReflectPhysical = 5;
			Attributes.AttackChance = 5;
			
			switch(Utility.Random(5))
			{
                case 0: PhysicalBonus = 10; break;
                case 1: FireBonus = 10; break;
                case 2: ColdBonus = 10; break;
                case 3: PoisonBonus = 10; break;
                case 4: EnergyBonus = 10; break;
			}
		}
		
		public UnforgivenVeil(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
		}
	}
	
	public class HailstormHuman : WarFork, ITokunoDyable
	{
		public override bool IsArtifact { get { return true; } }
		public override int LabelNumber { get { return 1153292; } } // Hailstorm
		
		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

        [Constructable]
		public HailstormHuman()
		{
			Hue = 2714; 
			
			WeaponAttributes.HitLightning = 15;
			WeaponAttributes.HitColdArea = 100;
			WeaponAttributes.HitLeechMana = 30;
			Attributes.AttackChance = 20;
			Attributes.WeaponSpeed = 25;
			Attributes.WeaponDamage = 50;
			AosElementDamages.Cold = 100;
		}
		
		public HailstormHuman(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
		}
	}
	
	public class HailstormGargoyle : GargishWarFork, ITokunoDyable
	{
		public override bool IsArtifact { get { return true; } }
		public override int LabelNumber { get { return 1153292; } } // Hailstorm

        [Constructable]
		public HailstormGargoyle()
		{
			Hue = 2714;
			
			WeaponAttributes.HitLightning = 15;
			WeaponAttributes.HitColdArea = 100;
			WeaponAttributes.HitLeechMana = 30;
			Attributes.AttackChance = 20;
			Attributes.WeaponSpeed = 25;
			Attributes.WeaponDamage = 50;
			AosElementDamages.Cold = 100;
		}
		
		public HailstormGargoyle(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
		}
	}
}