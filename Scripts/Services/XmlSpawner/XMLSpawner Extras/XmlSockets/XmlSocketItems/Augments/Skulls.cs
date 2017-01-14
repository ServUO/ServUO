using System;
using Server;
using Server.Mobiles;
using Server.Engines.XmlSpawner2;

namespace Server.Items
{

    // ---------------------------------------------------
    // Mythic skull
    // ---------------------------------------------------

    public class MythicSkull : BaseSocketAugmentation, IMythicAugment
    {

        [Constructable]
        public MythicSkull() : base(0x1ae4)
        {
            Name = "Mythic skull";
            Hue = 1154;
        }
        
        public override int SocketsRequired {get { return 3; } }
        
        public override int Icon { get { return 0x2203;} }

        public override int IconXOffset { get { return 15;} }

        public override int IconYOffset { get { return 15;} }

        public MythicSkull( Serial serial ) : base( serial )
		{
		}

        public override string OnIdentify(Mobile from)
        {

            return "Weapon: +9 Leech Mana, +9 Leech Life\nArmor: +5 Mana Regen, +5 Hits Regen\nCreature: +85 Hits";
        }

        public override bool OnAugment(Mobile from, object target)
        {
            if(target is BaseArmor)
            {
                BaseArmor a = target as BaseArmor;
                a.Attributes.RegenHits += 5;
			    a.Attributes.RegenMana += 5;
                return true;
            } else
            if(target is BaseWeapon)
            {
                BaseWeapon a = target as BaseWeapon;
                a.WeaponAttributes.HitLeechHits += 9;
			    a.WeaponAttributes.HitLeechMana += 9;
			    return true;
            } else
            if(target is BaseCreature)
            {
                BaseCreature a = target as BaseCreature;
                a.HitsMaxSeed += 85;
                return true;
            }

            return false;
        }


        public override bool CanAugment(Mobile from, object target)
        {
            if(target is BaseArmor || target is BaseWeapon || target is BaseCreature)
            {
                return true;
            }

            return false;
        }
        
        public override bool OnRecover(Mobile from, object target, int version)
        {
            if(target is BaseArmor)
            {
                BaseArmor a = target as BaseArmor;
                a.Attributes.RegenHits -= 5;
			    a.Attributes.RegenMana -= 5;
                return true;
            } else
            if(target is BaseWeapon)
            {
                BaseWeapon a = target as BaseWeapon;
                a.WeaponAttributes.HitLeechHits -= 9;
			    a.WeaponAttributes.HitLeechMana -= 9;
			    return true;
            } else
            if(target is BaseCreature)
            {
                BaseCreature a = target as BaseCreature;
                a.HitsMaxSeed -= 85;
                return true;
            }

            return false;
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


    // ---------------------------------------------------
    // Ancient skull
    // ---------------------------------------------------

    public class AncientSkull : BaseSocketAugmentation, IAncientAugment
    {

        [Constructable]
        public AncientSkull() : base(0x1ae4)
        {
            Name = "Ancient skull";
            Hue = 1150;
        }
        
        public override int SocketsRequired {get { return 1; } }
        
        public override int Icon { get { return 0x2203;} }

        public override int IconXOffset { get { return 15;} }

        public override int IconYOffset { get { return 15;} }

        public AncientSkull( Serial serial ) : base( serial )
		{
		}

        public override string OnIdentify(Mobile from)
        {

            return "Weapon: +2 Leech Mana, +2 Leech Life\nArmor: +1 Mana Regen, +1 Hits Regen\nCreature: +20 Hits";
        }

        public override bool OnAugment(Mobile from, object target)
        {
            if(target is BaseArmor)
            {
                BaseArmor a = target as BaseArmor;
                a.Attributes.RegenHits += 1;
			    a.Attributes.RegenMana += 1;
                return true;
            } else
            if(target is BaseWeapon)
            {
                BaseWeapon a = target as BaseWeapon;
                a.WeaponAttributes.HitLeechHits += 2;
			    a.WeaponAttributes.HitLeechMana += 2;
			    return true;
            } else
            if(target is BaseCreature)
            {
                BaseCreature a = target as BaseCreature;
                a.HitsMaxSeed += 20;
                return true;
            }

            return false;
        }


        public override bool CanAugment(Mobile from, object target)
        {
            if(target is BaseArmor || target is BaseWeapon || target is BaseCreature)
            {
                return true;
            }

            return false;
        }
        
        public override bool OnRecover(Mobile from, object target, int version)
        {
            if(target is BaseArmor)
            {
                BaseArmor a = target as BaseArmor;
                a.Attributes.RegenHits -= 1;
			    a.Attributes.RegenMana -= 1;
                return true;
            } else
            if(target is BaseWeapon)
            {
                BaseWeapon a = target as BaseWeapon;
                a.WeaponAttributes.HitLeechHits -= 2;
			    a.WeaponAttributes.HitLeechMana -= 2;
			    return true;
            } else
            if(target is BaseCreature)
            {
                BaseCreature a = target as BaseCreature;
                a.HitsMaxSeed -= 20;
                return true;
            }

            return false;
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


    // ---------------------------------------------------
    // Legendary skull
    // ---------------------------------------------------

    public class LegendarySkull : BaseSocketAugmentation, ILegendaryAugment
    {

        [Constructable]
        public LegendarySkull() : base(0x1ae4)
        {
            Name = "Legendary skull";
            Hue = 1153;
        }

        public override int SocketsRequired {get { return 2; } }
        
        public override int Icon { get { return 0x2203;} }

        public override int IconXOffset { get { return 15;} }

        public override int IconYOffset { get { return 15;} }

        public LegendarySkull( Serial serial ) : base( serial )
		{
		}

        public override string OnIdentify(Mobile from)
        {

            return "Weapon: +5 Leech Mana, +5 Leech Life\nArmor: +3 Mana Regen, +3 Hits Regen\nCreature: +50 Hits";
        }

        public override bool OnAugment(Mobile from, object target)
        {
            if(target is BaseArmor)
            {
                BaseArmor a = target as BaseArmor;
                a.Attributes.RegenHits += 3;
			    a.Attributes.RegenMana += 3;
                return true;
            } else
            if(target is BaseWeapon)
            {
                BaseWeapon a = target as BaseWeapon;
                a.WeaponAttributes.HitLeechHits += 5;
			    a.WeaponAttributes.HitLeechMana += 5;
			    return true;
            } else
            if(target is BaseCreature)
            {
                BaseCreature a = target as BaseCreature;
                a.HitsMaxSeed += 50;
                return true;
            }

            return false;
        }


        public override bool CanAugment(Mobile from, object target)
        {
            if(target is BaseArmor || target is BaseWeapon || target is BaseCreature)
            {
                return true;
            }

            return false;
        }
        
        public override bool OnRecover(Mobile from, object target, int version)
        {
            if(target is BaseArmor)
            {
                BaseArmor a = target as BaseArmor;
                a.Attributes.RegenHits -= 3;
			    a.Attributes.RegenMana -= 3;
                return true;
            } else
            if(target is BaseWeapon)
            {
                BaseWeapon a = target as BaseWeapon;
                a.WeaponAttributes.HitLeechHits -= 5;
			    a.WeaponAttributes.HitLeechMana -= 5;
			    return true;
            } else
            if(target is BaseCreature)
            {
                BaseCreature a = target as BaseCreature;
                a.HitsMaxSeed -= 50;
                return true;
            }

            return false;
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
