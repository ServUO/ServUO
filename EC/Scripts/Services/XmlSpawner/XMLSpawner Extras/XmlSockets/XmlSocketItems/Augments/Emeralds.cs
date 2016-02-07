using System;
using Server;
using Server.Mobiles;
using Server.Engines.XmlSpawner2;

namespace Server.Items
{
    // --------------------------------------------------
    // Mythic Emerald
    // --------------------------------------------------

    public class MythicEmerald : BaseSocketAugmentation, IMythicAugment
    {

        [Constructable]
        public MythicEmerald() : base(0xf26)
        {
            Name = "Mythic Emerald";
            Hue = 1267;
        }

        public MythicEmerald( Serial serial ) : base( serial )
		{
		}

        public override int SocketsRequired {get { return 3; } }

        public override int Icon {get { return 0x9a8; } }
        
        public override bool UseGumpArt {get { return true; } }
        
        public override int IconXOffset { get { return 15;} }

        public override int IconYOffset { get { return 15;} }


        public override string OnIdentify(Mobile from)
        {
            return "Weapon: +40 Hit Poison Area\nShields: +25 poison resist\nArmor: +25 Dex\nCreature: +32 Dex";
        }

        public override bool OnAugment(Mobile from, object target)
        {
            if(target is BaseWeapon)
            {
                ((BaseWeapon)target).WeaponAttributes.HitPoisonArea += 40;
            } else
            if(target is BaseShield)
            {
                BaseShield s = target as BaseShield;

                s.PoisonBonus += 25;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).Attributes.BonusDex += 25;
            } else
            if(target is BaseCreature)
            {
                ((BaseCreature)target).RawDex += 32;
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
                ((BaseWeapon)target).WeaponAttributes.HitPoisonArea -= 40;
            } else
            if(target is BaseShield)
            {
                BaseShield s = target as BaseShield;

                s.PoisonBonus -= 25;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).Attributes.BonusDex -= 25;
            } else
            if(target is BaseCreature)
            {
                ((BaseCreature)target).RawDex -= 32;
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
    // Legendary Emerald
    // --------------------------------------------------

    public class LegendaryEmerald : BaseSocketAugmentation, ILegendaryAugment
    {

        [Constructable]
        public LegendaryEmerald() : base(0xf26)
        {
            Name = "Legendary Emerald";
            Hue = 1268;
        }

        public LegendaryEmerald( Serial serial ) : base( serial )
		{
		}

        public override int SocketsRequired {get { return 2; } }

        public override int Icon {get { return 0x9a8; } }
        
        public override bool UseGumpArt {get { return true; } }
        
        public override int IconXOffset { get { return 15;} }

        public override int IconYOffset { get { return 15;} }


        public override string OnIdentify(Mobile from)
        {
            return "Weapon: +25 Hit Poison Area\nShields: +15 poison resist\nArmor: +15 Dex\nCreature: +20 Dex";
        }

        public override bool OnAugment(Mobile from, object target)
        {
            if(target is BaseWeapon)
            {
                ((BaseWeapon)target).WeaponAttributes.HitPoisonArea += 25;
            } else
            if(target is BaseShield)
            {
                BaseShield s = target as BaseShield;

                s.PoisonBonus += 15;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).Attributes.BonusDex += 15;
            } else
            if(target is BaseCreature)
            {
                ((BaseCreature)target).RawDex += 20;
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
                ((BaseWeapon)target).WeaponAttributes.HitPoisonArea -= 25;
            } else
            if(target is BaseShield)
            {
                BaseShield s = target as BaseShield;

                s.PoisonBonus -= 15;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).Attributes.BonusDex -= 15;
            } else
            if(target is BaseCreature)
            {
                ((BaseCreature)target).RawDex -= 20;
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
    // Ancient Emerald
    // --------------------------------------------------

    public class AncientEmerald : BaseSocketAugmentation, IAncientAugment
    {

        [Constructable]
        public AncientEmerald() : base(0xf26)
        {
            Name = "Ancient Emerald";
            Hue = 76;
        }

        public AncientEmerald( Serial serial ) : base( serial )
		{
		}

        public override int SocketsRequired {get { return 1; } }

        public override int Icon {get { return 0x9a8; } }

        public override bool UseGumpArt {get { return true; } }

        public override int IconXOffset { get { return 15;} }

        public override int IconYOffset { get { return 15;} }


        public override string OnIdentify(Mobile from)
        {
            return "Weapon: +10 Hit Poison Area\nShields: +6 poison resist\nArmor: +6 Dex\nCreature: +8 Dex";
        }

        public override bool OnAugment(Mobile from, object target)
        {
            if(target is BaseWeapon)
            {
                ((BaseWeapon)target).WeaponAttributes.HitPoisonArea += 10;
            } else
            if(target is BaseShield)
            {
                BaseShield s = target as BaseShield;

                s.PoisonBonus += 6;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).Attributes.BonusDex += 6;
            } else
            if(target is BaseCreature)
            {
                ((BaseCreature)target).RawDex += 8;
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
                ((BaseWeapon)target).WeaponAttributes.HitPoisonArea -= 10;
            } else
            if(target is BaseShield)
            {
                BaseShield s = target as BaseShield;

                s.PoisonBonus -= 6;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).Attributes.BonusDex -= 6;
            } else
            if(target is BaseCreature)
            {
                ((BaseCreature)target).RawDex -= 8;
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
