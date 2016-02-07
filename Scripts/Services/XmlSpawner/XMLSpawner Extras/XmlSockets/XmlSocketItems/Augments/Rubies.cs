using System;
using Server;
using Server.Mobiles;
using Server.Engines.XmlSpawner2;

namespace Server.Items
{

	public class MythicRuby : BaseSocketAugmentation, IMythicAugment
	{

		[Constructable]
		public MythicRuby() : base(0xF13)
		{
			Name = "Mythic Ruby";
			Hue = 32;
		}

		public MythicRuby( Serial serial ) : base( serial )
		{
		}
    
		public override int SocketsRequired {get { return 3; } }

		public override int Icon {get { return 0x9a8; } }
        
		public override bool UseGumpArt {get { return true; } }
        
		public override int IconXOffset { get { return 15;} }

		public override int IconYOffset { get { return 15;} }

		public override int Version { get { return 0;} }

		public override string OnIdentify(Mobile from)
		{
			switch(Version)
			{
				case 0:
					return "Weapon: +40 Hit Fire Area\nShields: +25 Fire Resist\nArmor: +32 Hits\nCreature: +32 Armor";
				case 1:
					return "Weapon: +30 Hit Fire Area\nShields: +20 Fire Resist\nArmor: +25 Hits\nCreature: +28 Armor";
			}

			return null;
		}

		public override bool CanAugment(Mobile from, object target)
		{
			return (target is BaseWeapon || target is BaseArmor || target is BaseCreature);
		}

		public override bool OnAugment(Mobile from, object target)
		{
			switch(Version)
			{
				case 0:
					// stronger version
					if(target is BaseWeapon)
					{
						((BaseWeapon)target).WeaponAttributes.HitFireArea += 40;
					} 
					else
						if(target is BaseShield)
					{
						BaseShield s = target as BaseShield;

						s.FireBonus += 25;
					} 
					else
						if(target is BaseArmor)
					{
						((BaseArmor)target).Attributes.BonusHits += 32;
					} 
					else
						if(target is BaseCreature)
					{
						((BaseCreature)target).VirtualArmor += 32;
					} 
					else
					{
						return false;
					}
					break;
				case 1:
					// weaker version
					if(target is BaseWeapon)
					{
						((BaseWeapon)target).WeaponAttributes.HitFireArea += 30;
					} 
					else
						if(target is BaseShield)
					{
						BaseShield s = target as BaseShield;

						s.FireBonus += 20;
					} 
					else
						if(target is BaseArmor)
					{
						((BaseArmor)target).Attributes.BonusHits += 25;
					} 
					else
						if(target is BaseCreature)
					{
						((BaseCreature)target).VirtualArmor += 28;
					} 
					else
					{
						return false;
					}
					break;

			}

			return true;
		}
       
		public override bool OnRecover(Mobile from, object target, int version)
		{
			switch(version)
			{
				case 0:
					// stronger version
					if(target is BaseWeapon)
					{
						((BaseWeapon)target).WeaponAttributes.HitFireArea -= 40;
					} 
					else
						if(target is BaseShield)
					{
						BaseShield s = target as BaseShield;

						s.FireBonus -= 25;
					} 
					else
						if(target is BaseArmor)
					{
						((BaseArmor)target).Attributes.BonusHits -= 32;
					} 
					else
						if(target is BaseCreature)
					{
						((BaseCreature)target).VirtualArmor -= 32;
					} 
					else
					{
						return false;
					}
					break;
				case 1:
					// weaker version
					if(target is BaseWeapon)
					{
						((BaseWeapon)target).WeaponAttributes.HitFireArea -= 30;
					} 
					else
						if(target is BaseShield)
					{
						BaseShield s = target as BaseShield;

						s.FireBonus -= 20;
					} 
					else
						if(target is BaseArmor)
					{
						((BaseArmor)target).Attributes.BonusHits -= 25;
					} 
					else
						if(target is BaseCreature)
					{
						((BaseCreature)target).VirtualArmor -= 28;
					} 
					else
					{
						return false;
					}
					break;
				
			}

			return true;
		}

		public override bool CanRecover(Mobile from, object target, int version)
		{
			return true;
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

	// --------------------------------------------------
	// Legendary Ruby
	// --------------------------------------------------

	public class LegendaryRuby : BaseSocketAugmentation, ILegendaryAugment
	{

		[Constructable]
		public LegendaryRuby() : base(0xF13)
		{
			Name = "Legendary Ruby";
			Hue = 33;
		}

		public LegendaryRuby( Serial serial ) : base( serial )
		{
		}
    
		public override int SocketsRequired {get { return 2; } }

		public override int Icon {get { return 0x9a8; } }
        
		public override bool UseGumpArt {get { return true; } }
        
		public override int IconXOffset { get { return 15;} }

		public override int IconYOffset { get { return 15;} }

    
		public override string OnIdentify(Mobile from)
		{
			return "Weapon: +25 Hit Fire Area\nShields: +15 fire resist\nArmor: +20 Hits\nCreature: +20 Armor";
		}

		public override bool OnAugment(Mobile from, object target)
		{
			if(target is BaseWeapon)
			{
				((BaseWeapon)target).WeaponAttributes.HitFireArea += 25;
			} 
			else
				if(target is BaseShield)
			{
				BaseShield s = target as BaseShield;

				s.FireBonus += 15;
			} 
			else
				if(target is BaseArmor)
			{
				((BaseArmor)target).Attributes.BonusHits += 20;
			} 
			else
				if(target is BaseCreature)
			{
				((BaseCreature)target).VirtualArmor += 20;
			} 
			else
			{
				return false;
			}

			return true;
		}

		public override bool CanAugment(Mobile from, object target)
		{
			return (target is BaseWeapon || target is BaseArmor || target is BaseCreature);
		}
        
		public override bool OnRecover(Mobile from, object target, int version)
		{
			if(target is BaseWeapon)
			{
				((BaseWeapon)target).WeaponAttributes.HitFireArea -= 25;
			} 
			else
				if(target is BaseShield)
			{
				BaseShield s = target as BaseShield;

				s.FireBonus -= 15;
			} 
			else
				if(target is BaseArmor)
			{
				((BaseArmor)target).Attributes.BonusHits -= 20;
			} 
			else
				if(target is BaseCreature)
			{
				((BaseCreature)target).VirtualArmor -= 20;
			} 
			else
			{
				return false;
			}

			return true;
		}

		public override bool CanRecover(Mobile from, object target, int version)
		{
			return true;
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

	// --------------------------------------------------
	// Ancient Ruby
	// --------------------------------------------------

	public class AncientRuby : BaseSocketAugmentation, IAncientAugment
	{

		[Constructable]
		public AncientRuby() : base(0xF13)
		{
			Name = "Ancient Ruby";
			Hue = 30;
		}

		public AncientRuby( Serial serial ) : base( serial )
		{
		}

		public override int SocketsRequired {get { return 1; } }

		public override int Icon {get { return 0x9a8; } }

		public override bool UseGumpArt {get { return true; } }

		public override int IconXOffset { get { return 15;} }

		public override int IconYOffset { get { return 15;} }


		public override string OnIdentify(Mobile from)
		{
			return "Weapon: +10 Hit Fire Area\nShields: +6 fire resist\nArmor: +8 Hits\nCreature: +8 Armor";
		}

		public override bool OnAugment(Mobile from, object target)
		{
			if(target is BaseWeapon)
			{
				((BaseWeapon)target).WeaponAttributes.HitFireArea += 10;
			} 
			else
				if(target is BaseShield)
			{
				BaseShield s = target as BaseShield;

				s.FireBonus += 6;
			} 
			else
				if(target is BaseArmor)
			{
				((BaseArmor)target).Attributes.BonusHits += 8;
			} 
			else
				if(target is BaseCreature)
			{
				((BaseCreature)target).VirtualArmor += 8;
			} 
			else
			{
				return false;
			}

			return true;
		}

		public override bool CanAugment(Mobile from, object target)
		{
			return (target is BaseWeapon || target is BaseArmor || target is BaseCreature);
		}
        
		public override bool OnRecover(Mobile from, object target, int version)
		{
			if(target is BaseWeapon)
			{
				((BaseWeapon)target).WeaponAttributes.HitFireArea -= 10;
			} 
			else
				if(target is BaseShield)
			{
				BaseShield s = target as BaseShield;

				s.FireBonus -= 6;
			} 
			else
				if(target is BaseArmor)
			{
				((BaseArmor)target).Attributes.BonusHits -= 8;
			} 
			else
				if(target is BaseCreature)
			{
				((BaseCreature)target).VirtualArmor -= 8;
			} 
			else
			{
				return false;
			}

			return true;
		}

		public override bool CanRecover(Mobile from, object target, int version)
		{
			return true;
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
