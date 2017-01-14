using System;
using Server;
using Server.Mobiles;
using Server.Engines.XmlSpawner2;

namespace Server.Items
{

    // --------------------------------------------------
    // Legendary Amethyst
    // --------------------------------------------------

    public class MythicAmethyst : BaseSocketAugmentation, IMythicAugment
    {

        [Constructable]
        public MythicAmethyst() : base(0xf26)
        {
            Name = "Mythic Amethyst";
            Hue = 11;
        }

        public MythicAmethyst( Serial serial ) : base( serial )
		{
		}

        public override int SocketsRequired {get { return 3; } }

        public override int Icon {get { return 0x9a8; } }

        public override bool UseGumpArt {get { return true; } }
        
        public override int IconXOffset { get { return 15;} }

        public override int IconYOffset { get { return 15;} }


        public override string OnIdentify(Mobile from)
        {
            return "Weapon: +17 Damage\nShields: +9 Str\nArmor: +16 Defend Chance\nCreature: +5 Max Damage";
        }

        public override bool OnAugment(Mobile from, object target)
        {
            if(target is BaseWeapon)
            {
                ((BaseWeapon)target).Attributes.WeaponDamage += 17;
            } else
            if(target is BaseShield)
            {
                BaseShield s = target as BaseShield;

                s.Attributes.BonusStr += 9;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).Attributes.DefendChance += 16;
            } else
            if(target is BaseCreature)
            {
                ((BaseCreature)target).DamageMax += 5;
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
                ((BaseWeapon)target).Attributes.WeaponDamage -= 17;
            } else
            if(target is BaseShield)
            {
                BaseShield s = target as BaseShield;

                s.Attributes.BonusStr -= 9;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).Attributes.DefendChance -= 16;
            } else
            if(target is BaseCreature)
            {
                ((BaseCreature)target).DamageMax -= 5;
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
    // Legendary Amethyst
    // --------------------------------------------------

    public class LegendaryAmethyst : BaseSocketAugmentation, ILegendaryAugment
    {

        [Constructable]
        public LegendaryAmethyst() : base(0xf26)
        {
            Name = "Legendary Amethyst";
            Hue = 12;
        }

        public LegendaryAmethyst( Serial serial ) : base( serial )
		{
		}

        public override int SocketsRequired {get { return 2; } }

        public override int Icon {get { return 0x9a8; } }

        public override bool UseGumpArt {get { return true; } }
        
        public override int IconXOffset { get { return 15;} }

        public override int IconYOffset { get { return 15;} }


        public override string OnIdentify(Mobile from)
        {
            return "Weapon: +10 Damage\nShields: +5 Str\nArmor: +10 Defend Chance\nCreature: +3 Max Damage";
        }

        public override bool OnAugment(Mobile from, object target)
        {
            if(target is BaseWeapon)
            {
                ((BaseWeapon)target).Attributes.WeaponDamage += 10;
            } else
            if(target is BaseShield)
            {
                BaseShield s = target as BaseShield;

                s.Attributes.BonusStr += 5;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).Attributes.DefendChance += 10;
            } else
            if(target is BaseCreature)
            {
                ((BaseCreature)target).DamageMax += 3;
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
                ((BaseWeapon)target).Attributes.WeaponDamage -= 10;
            } else
            if(target is BaseShield)
            {
                BaseShield s = target as BaseShield;

                s.Attributes.BonusStr -= 5;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).Attributes.DefendChance -= 10;
            } else
            if(target is BaseCreature)
            {
                ((BaseCreature)target).DamageMax -= 3;
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
    // Ancient Amethyst
    // --------------------------------------------------

    public class AncientAmethyst : BaseSocketAugmentation, IAncientAugment
    {

        [Constructable]
        public AncientAmethyst() : base(0xf26)
        {
            Name = "Ancient Amethyst";
            Hue = 15;
        }

        public AncientAmethyst( Serial serial ) : base( serial )
		{
		}

        public override int SocketsRequired {get { return 1; } }

        public override int Icon {get { return 0x9a8; } }

        public override bool UseGumpArt {get { return true; } }
        
        public override int IconXOffset { get { return 15;} }

        public override int IconYOffset { get { return 15;} }


        public override string OnIdentify(Mobile from)
        {
            return "Weapon: +4 Damage\nShields: +2 Str\nArmor: +4 Defend Chance\nCreature: +1 Max Damage";
        }

        public override bool OnAugment(Mobile from, object target)
        {
            if(target is BaseWeapon)
            {
                ((BaseWeapon)target).Attributes.WeaponDamage += 4;
            } else
            if(target is BaseShield)
            {
                BaseShield s = target as BaseShield;

                s.Attributes.BonusStr += 2;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).Attributes.DefendChance += 4;
            } else
            if(target is BaseCreature)
            {
                ((BaseCreature)target).DamageMax += 1;
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
                ((BaseWeapon)target).Attributes.WeaponDamage -= 4;
            } else
            if(target is BaseShield)
            {
                BaseShield s = target as BaseShield;

                s.Attributes.BonusStr -= 2;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).Attributes.DefendChance -= 4;
            } else
            if(target is BaseCreature)
            {
                ((BaseCreature)target).DamageMax -= 1;
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
