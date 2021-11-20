using System;
using Server;
using Server.Mobiles;
using Server.Engines.XmlSpawner2;

namespace Server.Items
{
    // --------------------------------------------------
    // Mythic Sapphire
    // --------------------------------------------------

    public class MythicSapphire : BaseSocketAugmentation, IMythicAugment
    {

        [Constructable]
        public MythicSapphire() : base(0xF19)
        {
            Name = "Mythic Sapphire";
            Hue = 190;
        }

        public MythicSapphire( Serial serial ) : base( serial )
		{
		}

        public override int SocketsRequired {get { return 3; } }

        public override int Icon {get { return 0x9a8; } }
        
        public override bool UseGumpArt {get { return true; } }
        
        public override int IconXOffset { get { return 15;} }

        public override int IconYOffset { get { return 15;} }


        public override string OnIdentify(Mobile from)
        {
            return "Weapon: +40 Hit Cold Area\nShields: +25 Cold Resist\nArmor: +15 Int\nCreature: +32 Int";
        }

        public override bool OnAugment(Mobile from, object target)
        {
            if(target is BaseWeapon)
            {
                ((BaseWeapon)target).WeaponAttributes.HitColdArea += 40;
            } else
            if(target is BaseShield)
            {
                BaseShield s = target as BaseShield;

                s.ColdBonus += 25;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).Attributes.BonusInt += 15;
            } else
            if(target is BaseCreature)
            {
                ((BaseCreature)target).RawInt += 32;
            } else
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
                ((BaseWeapon)target).WeaponAttributes.HitColdArea -= 40;
            } else
            if(target is BaseShield)
            {
                BaseShield s = target as BaseShield;

                s.ColdBonus -= 25;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).Attributes.BonusInt -= 15;
            } else
            if(target is BaseCreature)
            {
                ((BaseCreature)target).RawInt -= 32;
            } else
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
    // Legendary Sapphire
    // --------------------------------------------------

    public class LegendarySapphire : BaseSocketAugmentation, ILegendaryAugment
    {

        [Constructable]
        public LegendarySapphire() : base(0xF19)
        {
            Name = "Legendary Sapphire";
            Hue = 100;
        }

        public LegendarySapphire( Serial serial ) : base( serial )
		{
		}

        public override int SocketsRequired {get { return 2; } }

        public override int Icon {get { return 0x9a8; } }
        
        public override bool UseGumpArt {get { return true; } }
        
        public override int IconXOffset { get { return 15;} }

        public override int IconYOffset { get { return 15;} }


        public override string OnIdentify(Mobile from)
        {
            return "Weapon: +25 Hit Cold Area\nShields: +15 Cold Resist\nArmor: +8 Int\nCreature: +20 Int";
        }

        public override bool OnAugment(Mobile from, object target)
        {
            if(target is BaseWeapon)
            {
                ((BaseWeapon)target).WeaponAttributes.HitColdArea += 25;
            } else
            if(target is BaseShield)
            {
                BaseShield s = target as BaseShield;

                s.ColdBonus += 15;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).Attributes.BonusInt += 8;
            } else
            if(target is BaseCreature)
            {
                ((BaseCreature)target).RawInt += 20;
            } else
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
                ((BaseWeapon)target).WeaponAttributes.HitColdArea -= 25;
            } else
            if(target is BaseShield)
            {
                BaseShield s = target as BaseShield;

                s.ColdBonus -= 15;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).Attributes.BonusInt -= 8;
            } else
            if(target is BaseCreature)
            {
                ((BaseCreature)target).RawInt -= 20;
            } else
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
    // Ancient Sapphire
    // --------------------------------------------------

    public class AncientSapphire : BaseSocketAugmentation, IAncientAugment
    {

        [Constructable]
        public AncientSapphire() : base(0xF19)
        {
            Name = "Ancient Sapphire";
            Hue = 94;
        }

        public AncientSapphire( Serial serial ) : base( serial )
		{
		}

        public override int SocketsRequired {get { return 1; } }

        public override int Icon {get { return 0x9a8; } }

        public override bool UseGumpArt {get { return true; } }

        public override int IconXOffset { get { return 15;} }

        public override int IconYOffset { get { return 15;} }


        public override string OnIdentify(Mobile from)
        {
            return "Weapon: +10 Hit Cold Area\nShields: +6 Cold Resist\nArmor: +3 Int\nCreature: +5 Int";
        }

        public override bool OnAugment(Mobile from, object target)
        {
            if(target is BaseWeapon)
            {
                ((BaseWeapon)target).WeaponAttributes.HitColdArea += 10;
            } else
            if(target is BaseShield)
            {
                BaseShield s = target as BaseShield;

                s.ColdBonus += 6;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).Attributes.BonusInt += 3;
            } else
            if(target is BaseCreature)
            {
                ((BaseCreature)target).RawInt += 3;
            } else
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
                ((BaseWeapon)target).WeaponAttributes.HitColdArea -= 10;
            } else
            if(target is BaseShield)
            {
                BaseShield s = target as BaseShield;

                s.ColdBonus -= 6;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).Attributes.BonusInt -= 3;
            } else
            if(target is BaseCreature)
            {
                ((BaseCreature)target).RawInt -= 3;
            } else
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
