using System;
using Server;
using Server.Mobiles;
using Server.Engines.XmlSpawner2;

namespace Server.Items
{

    // --------------------------------------------------
    // Mythic Diamond
    // --------------------------------------------------

    public class MythicDiamond : BaseSocketAugmentation, IMythicAugment
    {

        [Constructable]
        public MythicDiamond() : base(0xf26)
        {
            Name = "Mythic Diamond";
            Hue = 1153;
        }

        public MythicDiamond( Serial serial ) : base( serial )
		{
		}

        public override int SocketsRequired {get { return 3; } }

        public override int Icon {get { return 0x9a8; } }
        
        public override bool UseGumpArt {get { return true; } }
        
        public override int IconXOffset { get { return 15;} }

        public override int IconYOffset { get { return 15;} }


        public override string OnIdentify(Mobile from)
        {
            return "Weapon: +40 Hit Physical Area\nShields: +5 all resists\nArmor: +32 Attack Chance\nCreature: +32 Str";
        }

        public override bool OnAugment(Mobile from, object target)
        {
            if(target is BaseWeapon)
            {
                ((BaseWeapon)target).WeaponAttributes.HitPhysicalArea += 40;
            } else
            if(target is BaseShield)
            {
                BaseShield s = target as BaseShield;

                s.EnergyBonus += 5;
                s.FireBonus += 5;
                s.PoisonBonus += 5;
                s.PhysicalBonus += 5;
                s.ColdBonus += 5;
			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).Attributes.AttackChance += 32;
            } else
            if(target is BaseCreature)
            {
                ((BaseCreature)target).RawStr += 32;
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
                ((BaseWeapon)target).WeaponAttributes.HitPhysicalArea -= 40;
            } else
            if(target is BaseShield)
            {
                BaseShield s = target as BaseShield;

                s.EnergyBonus -= 5;
                s.FireBonus -= 5;
                s.PoisonBonus -= 5;
                s.PhysicalBonus -= 5;
                s.ColdBonus -= 5;
			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).Attributes.AttackChance -= 32;
            } else
            if(target is BaseCreature)
            {
                ((BaseCreature)target).RawStr -= 32;
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
    // Legendary Diamond
    // --------------------------------------------------

    public class LegendaryDiamond : BaseSocketAugmentation, ILegendaryAugment
    {

        [Constructable]
        public LegendaryDiamond() : base(0xf26)
        {
            Name = "Legendary Diamond";
            Hue = 1150;
        }

        public LegendaryDiamond( Serial serial ) : base( serial )
		{
		}

        public override int SocketsRequired {get { return 2; } }

        public override int Icon {get { return 0x9a8; } }
        
        public override bool UseGumpArt {get { return true; } }
        
        public override int IconXOffset { get { return 15;} }

        public override int IconYOffset { get { return 15;} }


        public override string OnIdentify(Mobile from)
        {
            return "Weapon: +25 Hit Physical Area\nShields: +3 all resists\nArmor: +20 Attack Chance\nCreature: +20 Str";
        }

        public override bool OnAugment(Mobile from, object target)
        {
            if(target is BaseWeapon)
            {
                ((BaseWeapon)target).WeaponAttributes.HitPhysicalArea += 25;
            } else
            if(target is BaseShield)
            {
                BaseShield s = target as BaseShield;

                s.EnergyBonus += 3;
                s.FireBonus += 3;
                s.PoisonBonus += 3;
                s.PhysicalBonus += 3;
                s.ColdBonus += 3;
			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).Attributes.AttackChance += 20;
            } else
            if(target is BaseCreature)
            {
                ((BaseCreature)target).RawStr += 20;
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
                ((BaseWeapon)target).WeaponAttributes.HitPhysicalArea -= 25;
            } else
            if(target is BaseShield)
            {
                BaseShield s = target as BaseShield;

                s.EnergyBonus -= 3;
                s.FireBonus -= 3;
                s.PoisonBonus -= 3;
                s.PhysicalBonus -= 3;
                s.ColdBonus -= 3;
			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).Attributes.AttackChance -= 20;
            } else
            if(target is BaseCreature)
            {
                ((BaseCreature)target).RawStr -= 20;
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
    // Ancient Diamond
    // --------------------------------------------------

    public class AncientDiamond : BaseSocketAugmentation, IAncientAugment
    {

        [Constructable]
        public AncientDiamond() : base(0xf26)
        {
            Name = "Ancient Diamond";
            Hue = 1151;
        }

        public AncientDiamond( Serial serial ) : base( serial )
		{
		}

        public override int SocketsRequired {get { return 1; } }

        public override int Icon {get { return 0x9a8; } }

        public override bool UseGumpArt {get { return true; } }
        
        public override int IconXOffset { get { return 15;} }

        public override int IconYOffset { get { return 15;} }


        public override string OnIdentify(Mobile from)
        {
            return "Weapon: +10 Hit Physical Area\nShields: +1 all resists\nArmor: +8 Attack Chance\nCreature: +8 str";
        }

        public override bool OnAugment(Mobile from, object target)
        {
            if(target is BaseWeapon)
            {
                ((BaseWeapon)target).WeaponAttributes.HitPhysicalArea += 10;
            } else
            if(target is BaseShield)
            {
                BaseShield s = target as BaseShield;

                s.EnergyBonus += 1;
                s.FireBonus += 1;
                s.PoisonBonus += 1;
                s.PhysicalBonus += 1;
                s.ColdBonus += 1;
			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).Attributes.AttackChance += 8;
            } else
            if(target is BaseCreature)
            {
                ((BaseCreature)target).RawStr += 8;
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
                ((BaseWeapon)target).WeaponAttributes.HitPhysicalArea -= 10;
            } else
            if(target is BaseShield)
            {
                BaseShield s = target as BaseShield;

                s.EnergyBonus -= 1;
                s.FireBonus -= 1;
                s.PoisonBonus -= 1;
                s.PhysicalBonus -= 1;
                s.ColdBonus -= 1;
			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).Attributes.AttackChance -= 8;
            } else
            if(target is BaseCreature)
            {
                ((BaseCreature)target).RawStr -= 8;
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
