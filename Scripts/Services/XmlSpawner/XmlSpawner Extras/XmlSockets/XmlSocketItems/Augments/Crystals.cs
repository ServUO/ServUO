using System;
using Server;
using Server.Mobiles;
using Server.Engines.XmlSpawner2;

namespace Server.Items
{
    // --------------------------------------------------
    // Fen Crystal
    // --------------------------------------------------

    public class FenCrystal : BaseSocketAugmentation
    {

        [Constructable]
        public FenCrystal() : base(0xF8E)
        {
            Name = "Fen Crystal";
            Hue = 8;
        }

        public FenCrystal( Serial serial ) : base( serial )
		{
		}

        public override int SocketsRequired {get { return 2; } }

        public override int IconXOffset { get { return 15;} }

        public override int IconYOffset { get { return 15;} }


        public override string OnIdentify(Mobile from)
        {
            return "Weapon: +Spell Channeling\nShields: +Spell Channeling\nArmor: Mage armor";
        }

        public override bool OnAugment(Mobile from, object target)
        {
            if(target is BaseWeapon)
            {
                ((BaseWeapon)target).Attributes.SpellChanneling = 1;
            } else
            if(target is BaseShield)
            {
                ((BaseShield)target).Attributes.SpellChanneling = 1;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).ArmorAttributes.MageArmor = 1;
            } else
			{
                return false;
			}

            return true;
        }

        
        public override bool CanAugment(Mobile from, object target)
        {
            return (target is BaseWeapon || target is BaseArmor);
        }

        public override bool OnRecover(Mobile from, object target, int version)
        {
            if(target is BaseWeapon)
            {
                ((BaseWeapon)target).Attributes.SpellChanneling = 0;
            } else
            if(target is BaseShield)
            {
                ((BaseShield)target).Attributes.SpellChanneling = 0;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).ArmorAttributes.MageArmor = 0;
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
    // Rho Crystal
    // --------------------------------------------------

    public class RhoCrystal : BaseSocketAugmentation, ICrystalAugment
    {

        [Constructable]
        public RhoCrystal() : base(0xF8E)
        {
            Name = "Rho Crystal";
            Hue = 13;
        }

        public RhoCrystal( Serial serial ) : base( serial )
		{
		}

        public override int SocketsRequired {get { return 2; } }

        public override int IconXOffset { get { return 15;} }

        public override int IconYOffset { get { return 15;} }


        public override string OnIdentify(Mobile from)
        {
            return "Weapon, Shield, Armor, Jewelry: +1 Faster Casting";
        }

        public override bool OnAugment(Mobile from, object target)
        {
            if(target is BaseWeapon)
            {
                ((BaseWeapon)target).Attributes.CastSpeed += 1;
            } else
            if(target is BaseShield)
            {
                ((BaseShield)target).Attributes.CastSpeed += 1;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).Attributes.CastSpeed += 1;
            } else
            if(target is BaseJewel)
            {
                ((BaseJewel)target).Attributes.CastSpeed += 1;
            } else
			{
                return false;
			}

            return true;
        }

        
        public override bool CanAugment(Mobile from, object target)
        {
            return (target is BaseWeapon || target is BaseArmor || target is BaseJewel);
        }
        
        public override bool OnRecover(Mobile from, object target, int version)
        {
            if(target is BaseWeapon)
            {
                ((BaseWeapon)target).Attributes.CastSpeed -= 1;
            } else
            if(target is BaseShield)
            {
                ((BaseShield)target).Attributes.CastSpeed -= 1;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).Attributes.CastSpeed -= 1;
            } else
            if(target is BaseJewel)
            {
                ((BaseJewel)target).Attributes.CastSpeed -= 1;
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
    // Rys Crystal
    // --------------------------------------------------

    public class RysCrystal : BaseSocketAugmentation, ICrystalAugment
    {

        [Constructable]
        public RysCrystal() : base(0xF8E)
        {
            Name = "Rys Crystal";
            Hue = 18;
        }

        public RysCrystal( Serial serial ) : base( serial )
		{
		}

        public override int SocketsRequired {get { return 2; } }

        public override int IconXOffset { get { return 15;} }

        public override int IconYOffset { get { return 15;} }


        public override string OnIdentify(Mobile from)
        {
            return "Weapon, Shield, Armor, Jewelry: +1 Faster Cast Recovery";
        }

        public override bool OnAugment(Mobile from, object target)
        {
            if(target is BaseWeapon)
            {
                ((BaseWeapon)target).Attributes.CastRecovery += 1;
            } else
            if(target is BaseShield)
            {
                ((BaseShield)target).Attributes.CastRecovery += 1;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).Attributes.CastRecovery += 1;
            } else
            if(target is BaseJewel)
            {
                ((BaseJewel)target).Attributes.CastRecovery += 1;
            } else
			{
                return false;
			}

            return true;
        }

        
        public override bool CanAugment(Mobile from, object target)
        {
            return (target is BaseWeapon || target is BaseArmor || target is BaseJewel);
        }
        
        public override bool OnRecover(Mobile from, object target, int version)
        {
            if(target is BaseWeapon)
            {
                ((BaseWeapon)target).Attributes.CastRecovery -= 1;
            } else
            if(target is BaseShield)
            {
                ((BaseShield)target).Attributes.CastRecovery -= 1;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).Attributes.CastRecovery -= 1;
            } else
            if(target is BaseJewel)
            {
                ((BaseJewel)target).Attributes.CastRecovery -= 1;
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
    // Wyr Crystal
    // --------------------------------------------------

    public class WyrCrystal : BaseSocketAugmentation, ICrystalAugment
    {

        [Constructable]
        public WyrCrystal() : base(0xF8E)
        {
            Name = "Wyr Crystal";
            Hue = 23;
        }

        public WyrCrystal( Serial serial ) : base( serial )
		{
		}

        public override int SocketsRequired {get { return 2; } }

        public override int IconXOffset { get { return 15;} }

        public override int IconYOffset { get { return 15;} }


        public override string OnIdentify(Mobile from)
        {
            return "Weapon, Shield, Armor, Jewelry: +80 Luck";
        }

        public override bool OnAugment(Mobile from, object target)
        {
            if(target is BaseWeapon)
            {
                ((BaseWeapon)target).Attributes.Luck += 80;
            } else
            if(target is BaseShield)
            {
                ((BaseShield)target).Attributes.Luck += 80;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).Attributes.Luck += 80;
            } else
            if(target is BaseJewel)
            {
                ((BaseJewel)target).Attributes.Luck += 80;
            } else
			{
                return false;
			}

            return true;
        }


        public override bool CanAugment(Mobile from, object target)
        {
            return (target is BaseWeapon || target is BaseArmor || target is BaseJewel);
        }
        
        public override bool OnRecover(Mobile from, object target, int version)
        {
            if(target is BaseWeapon)
            {
                ((BaseWeapon)target).Attributes.Luck -= 80;
            } else
            if(target is BaseShield)
            {
                ((BaseShield)target).Attributes.Luck -= 80;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).Attributes.Luck -= 80;
            } else
            if(target is BaseJewel)
            {
                ((BaseJewel)target).Attributes.Luck -= 80;
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
    // Fre Crystal
    // --------------------------------------------------

    public class FreCrystal : BaseSocketAugmentation, ICrystalAugment
    {

        [Constructable]
        public FreCrystal() : base(0xF8E)
        {
            Name = "Fre Crystal";
            Hue = 28;
        }

        public FreCrystal( Serial serial ) : base( serial )
		{
		}

        public override int SocketsRequired {get { return 2; } }

        public override int IconXOffset { get { return 15;} }

        public override int IconYOffset { get { return 15;} }


        public override string OnIdentify(Mobile from)
        {
            return "Weapon, Shield, Armor, Jewelry: +10 Enhance potions";
        }

        public override bool OnAugment(Mobile from, object target)
        {
            if(target is BaseWeapon)
            {
                ((BaseWeapon)target).Attributes.EnhancePotions += 10;
            } else
            if(target is BaseShield)
            {
                ((BaseShield)target).Attributes.EnhancePotions += 10;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).Attributes.EnhancePotions += 10;
            } else
            if(target is BaseJewel)
            {
                ((BaseJewel)target).Attributes.EnhancePotions += 10;
            } else
			{
                return false;
			}

            return true;
        }


        public override bool CanAugment(Mobile from, object target)
        {
            return (target is BaseWeapon || target is BaseArmor || target is BaseJewel);
        }
        
        public override bool OnRecover(Mobile from, object target, int version)
        {
            if(target is BaseWeapon)
            {
                ((BaseWeapon)target).Attributes.EnhancePotions -= 10;
            } else
            if(target is BaseShield)
            {
                ((BaseShield)target).Attributes.EnhancePotions -= 10;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).Attributes.EnhancePotions -= 10;
            } else
            if(target is BaseJewel)
            {
                ((BaseJewel)target).Attributes.EnhancePotions -= 10;
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
    // Tor Crystal
    // --------------------------------------------------

    public class TorCrystal : BaseSocketAugmentation, ICrystalAugment
    {

        [Constructable]
        public TorCrystal() : base(0xF8E)
        {
            Name = "Tor Crystal";
            Hue = 33;
        }

        public TorCrystal( Serial serial ) : base( serial )
		{
		}

        public override int SocketsRequired {get { return 2; } }

        public override int IconXOffset { get { return 15;} }

        public override int IconYOffset { get { return 15;} }


        public override string OnIdentify(Mobile from)
        {
            return "Weapon, Shield, Armor, Jewelry: +10 Lower reagent cost";
        }

        public override bool OnAugment(Mobile from, object target)
        {
            if(target is BaseWeapon)
            {
                ((BaseWeapon)target).Attributes.LowerRegCost += 10;
            } else
            if(target is BaseShield)
            {
                ((BaseShield)target).Attributes.LowerRegCost += 10;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).Attributes.LowerRegCost += 10;
            } else
            if(target is BaseJewel)
            {
                ((BaseJewel)target).Attributes.LowerRegCost += 10;
            } else
			{
                return false;
			}

            return true;
        }


        public override bool CanAugment(Mobile from, object target)
        {
            return (target is BaseWeapon || target is BaseArmor || target is BaseJewel);
        }
        
        public override bool OnRecover(Mobile from, object target, int version)
        {
            if(target is BaseWeapon)
            {
                ((BaseWeapon)target).Attributes.LowerRegCost -= 10;
            } else
            if(target is BaseShield)
            {
                ((BaseShield)target).Attributes.LowerRegCost -= 10;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).Attributes.LowerRegCost -= 10;
            } else
            if(target is BaseJewel)
            {
                ((BaseJewel)target).Attributes.LowerRegCost -= 10;
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
    // Vel Crystal
    // --------------------------------------------------

    public class VelCrystal : BaseSocketAugmentation, ICrystalAugment
    {

        [Constructable]
        public VelCrystal() : base(0xF8E)
        {
            Name = "Vel Crystal";
            Hue = 38;
        }

        public VelCrystal( Serial serial ) : base( serial )
		{
		}

        public override int SocketsRequired {get { return 2; } }

        public override int IconXOffset { get { return 15;} }

        public override int IconYOffset { get { return 15;} }


        public override string OnIdentify(Mobile from)
        {
            return "Weapon, Shield, Armor, Jewelry: +4 Lower mana cost";
        }

        public override bool OnAugment(Mobile from, object target)
        {
            if(target is BaseWeapon)
            {
                ((BaseWeapon)target).Attributes.LowerManaCost += 4;
            } else
            if(target is BaseShield)
            {
                ((BaseShield)target).Attributes.LowerManaCost += 4;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).Attributes.LowerManaCost += 4;
            } else
            if(target is BaseJewel)
            {
                ((BaseJewel)target).Attributes.LowerManaCost += 4;
            } else
			{
                return false;
			}

            return true;
        }

        
        public override bool CanAugment(Mobile from, object target)
        {
            return (target is BaseWeapon || target is BaseArmor || target is BaseJewel);
        }

        public override bool OnRecover(Mobile from, object target, int version)
        {
            if(target is BaseWeapon)
            {
                ((BaseWeapon)target).Attributes.LowerManaCost -= 4;
            } else
            if(target is BaseShield)
            {
                ((BaseShield)target).Attributes.LowerManaCost -= 4;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).Attributes.LowerManaCost -= 4;
            } else
            if(target is BaseJewel)
            {
                ((BaseJewel)target).Attributes.LowerManaCost -= 4;
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
    // Xen Crystal
    // --------------------------------------------------

    public class XenCrystal : BaseSocketAugmentation, ICrystalAugment
    {

        [Constructable]
        public XenCrystal() : base(0xF8E)
        {
            Name = "Xen Crystal";
            Hue = 43;
        }

        public XenCrystal( Serial serial ) : base( serial )
		{
		}

        public override int SocketsRequired {get { return 2; } }

        public override int IconXOffset { get { return 15;} }

        public override int IconYOffset { get { return 15;} }


        public override string OnIdentify(Mobile from)
        {
            return "Weapon, Shield, Armor, Jewelry: +5 Spell damage";
        }

        public override bool OnAugment(Mobile from, object target)
        {
            if(target is BaseWeapon)
            {
                ((BaseWeapon)target).Attributes.SpellDamage += 5;
            } else
            if(target is BaseShield)
            {
                ((BaseShield)target).Attributes.SpellDamage += 5;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).Attributes.SpellDamage += 5;
            } else
            if(target is BaseJewel)
            {
                ((BaseJewel)target).Attributes.SpellDamage += 5;
            } else
			{
                return false;
			}

            return true;
        }

        
        public override bool CanAugment(Mobile from, object target)
        {
            return (target is BaseWeapon || target is BaseArmor || target is BaseJewel);
        }
        
        public override bool OnRecover(Mobile from, object target, int version)
        {
            if(target is BaseWeapon)
            {
                ((BaseWeapon)target).Attributes.SpellDamage -= 5;
            } else
            if(target is BaseShield)
            {
                ((BaseShield)target).Attributes.SpellDamage -= 5;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).Attributes.SpellDamage -= 5;
            } else
            if(target is BaseJewel)
            {
                ((BaseJewel)target).Attributes.SpellDamage -= 5;
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
    // Pol Crystal
    // --------------------------------------------------

    public class PolCrystal : BaseSocketAugmentation, ICrystalAugment
    {

        [Constructable]
        public PolCrystal() : base(0xF8E)
        {
            Name = "Pol Crystal";
            Hue = 48;
        }

        public PolCrystal( Serial serial ) : base( serial )
		{
		}

        public override int SocketsRequired {get { return 2; } }

        public override int IconXOffset { get { return 15;} }

        public override int IconYOffset { get { return 15;} }


        public override string OnIdentify(Mobile from)
        {
            return "Weapon, Shield, Armor: +50 Durability\nJewelry: +6 Attack Chance";
        }

        public override bool OnAugment(Mobile from, object target)
        {
            if(target is BaseWeapon)
            {
                ((BaseWeapon)target).WeaponAttributes.DurabilityBonus += 50;
            } else
            if(target is BaseShield)
            {
                ((BaseShield)target).ArmorAttributes.DurabilityBonus += 50;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).ArmorAttributes.DurabilityBonus += 50;
            } else
            if(target is BaseJewel)
            {
                ((BaseJewel)target).Attributes.AttackChance += 6;
            } else
			{
                return false;
			}

            return true;
        }

        
        public override bool CanAugment(Mobile from, object target)
        {
            return (target is BaseWeapon || target is BaseArmor || target is BaseJewel);
        }
        
        public override bool OnRecover(Mobile from, object target, int version)
        {
            if(target is BaseWeapon)
            {
                ((BaseWeapon)target).WeaponAttributes.DurabilityBonus -= 50;
            } else
            if(target is BaseShield)
            {
                ((BaseShield)target).ArmorAttributes.DurabilityBonus -= 50;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).ArmorAttributes.DurabilityBonus -= 50;
            } else
            if(target is BaseJewel)
            {
                ((BaseJewel)target).Attributes.AttackChance -= 6;
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
    // Wol Crystal
    // --------------------------------------------------

    public class WolCrystal : BaseSocketAugmentation, ICrystalAugment
    {

        [Constructable]
        public WolCrystal() : base(0xF8E)
        {
            Name = "Wol Crystal";
            Hue = 53;
        }

        public WolCrystal( Serial serial ) : base( serial )
		{
		}

        public override int SocketsRequired {get { return 2; } }

        public override int IconXOffset { get { return 15;} }

        public override int IconYOffset { get { return 15;} }


        public override string OnIdentify(Mobile from)
        {
            return "Weapon, Shield, Armor: +2 Self-repair\nJewelry: +6 Defend Chance";
        }

        public override bool OnAugment(Mobile from, object target)
        {
            if(target is BaseWeapon)
            {
                ((BaseWeapon)target).WeaponAttributes.SelfRepair += 2;
            } else
            if(target is BaseShield)
            {
                ((BaseShield)target).ArmorAttributes.SelfRepair += 2;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).ArmorAttributes.SelfRepair += 2;
            } else
            if(target is BaseJewel)
            {
                ((BaseJewel)target).Attributes.DefendChance += 6;
            } else
			{
                return false;
			}

            return true;
        }

        
        public override bool CanAugment(Mobile from, object target)
        {
            return (target is BaseWeapon || target is BaseArmor || target is BaseJewel);
        }
        
        public override bool OnRecover(Mobile from, object target, int version)
        {
            if(target is BaseWeapon)
            {
                ((BaseWeapon)target).WeaponAttributes.SelfRepair -= 2;
            } else
            if(target is BaseShield)
            {
                ((BaseShield)target).ArmorAttributes.SelfRepair -= 2;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).ArmorAttributes.SelfRepair -= 2;
            } else
            if(target is BaseJewel)
            {
                ((BaseJewel)target).Attributes.DefendChance -= 6;
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
    // Bal Crystal
    // --------------------------------------------------

    public class BalCrystal : BaseSocketAugmentation, ICrystalAugment
    {

        [Constructable]
        public BalCrystal() : base(0xF8E)
        {
            Name = "Bal Crystal";
            Hue = 133;
        }

        public BalCrystal( Serial serial ) : base( serial )
		{
		}

        public override int SocketsRequired {get { return 2; } }

        public override int IconXOffset { get { return 15;} }

        public override int IconYOffset { get { return 15;} }


        public override string OnIdentify(Mobile from)
        {
            return "Weapon, Shield, Armor, Jewelry: +10 Resist fire";
        }

        public override bool OnAugment(Mobile from, object target)
        {
            if(target is BaseWeapon)
            {
                ((BaseWeapon)target).WeaponAttributes.ResistFireBonus += 10;
            } else
            if(target is BaseShield)
            {
                ((BaseShield)target).FireBonus += 10;
			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).FireBonus += 10;
            } else
            if(target is BaseJewel)
            {
                ((BaseJewel)target).Resistances.Fire += 10;
            } else
			{
                return false;
			}

            return true;
        }

        
        public override bool CanAugment(Mobile from, object target)
        {
            return (target is BaseWeapon || target is BaseArmor || target is BaseJewel);
        }
        
        public override bool OnRecover(Mobile from, object target, int version)
        {
            if(target is BaseWeapon)
            {
                ((BaseWeapon)target).WeaponAttributes.ResistFireBonus -= 10;
            } else
            if(target is BaseShield)
            {
                ((BaseShield)target).FireBonus -= 10;
			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).FireBonus -= 10;
            } else
            if(target is BaseJewel)
            {
                ((BaseJewel)target).Resistances.Fire -= 10;
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
    // Tal Crystal
    // --------------------------------------------------

    public class TalCrystal : BaseSocketAugmentation, ICrystalAugment
    {

        [Constructable]
        public TalCrystal() : base(0xF8E)
        {
            Name = "Tal Crystal";
            Hue = 88;
        }

        public TalCrystal( Serial serial ) : base( serial )
		{
		}

        public override int SocketsRequired {get { return 2; } }

        public override int IconXOffset { get { return 15;} }

        public override int IconYOffset { get { return 15;} }


        public override string OnIdentify(Mobile from)
        {
            return "Weapon, Shield, Armor, Jewelry: +10 Resist cold";
        }

        public override bool OnAugment(Mobile from, object target)
        {
            if(target is BaseWeapon)
            {
                ((BaseWeapon)target).WeaponAttributes.ResistColdBonus += 10;
            } else
            if(target is BaseShield)
            {
                ((BaseShield)target).ColdBonus += 10;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).ColdBonus += 10;
            } else
            if(target is BaseJewel)
            {
                ((BaseJewel)target).Resistances.Cold += 10;
            } else
			{
                return false;
			}

            return true;
        }

        
        public override bool CanAugment(Mobile from, object target)
        {
            return (target is BaseWeapon || target is BaseArmor || target is BaseJewel);
        }
        
        public override bool OnRecover(Mobile from, object target, int version)
        {
            if(target is BaseWeapon)
            {
                ((BaseWeapon)target).WeaponAttributes.ResistColdBonus -= 10;
            } else
            if(target is BaseShield)
            {
                ((BaseShield)target).ColdBonus -= 10;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).ColdBonus -= 10;
            } else
            if(target is BaseJewel)
            {
                ((BaseJewel)target).Resistances.Cold -= 10;
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
    // Jal Crystal
    // --------------------------------------------------

    public class JalCrystal : BaseSocketAugmentation, ICrystalAugment
    {

        [Constructable]
        public JalCrystal() : base(0xF8E)
        {
            Name = "Jal Crystal";
            Hue = 70;
        }

        public JalCrystal( Serial serial ) : base( serial )
		{
		}

        public override int SocketsRequired {get { return 2; } }

        public override int IconXOffset { get { return 15;} }

        public override int IconYOffset { get { return 15;} }


        public override string OnIdentify(Mobile from)
        {
            return "Weapon, Shield, Armor, Jewelry: +10 Resist poison";
        }

        public override bool OnAugment(Mobile from, object target)
        {
            if(target is BaseWeapon)
            {
                ((BaseWeapon)target).WeaponAttributes.ResistPoisonBonus += 10;
            } else
            if(target is BaseShield)
            {
                ((BaseShield)target).PoisonBonus += 10;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).PoisonBonus += 10;
            } else
            if(target is BaseJewel)
            {
                ((BaseJewel)target).Resistances.Poison += 10;
            } else
			{
                return false;
			}

            return true;
        }

        
        public override bool CanAugment(Mobile from, object target)
        {
            return (target is BaseWeapon || target is BaseArmor || target is BaseJewel);
        }
        
        public override bool OnRecover(Mobile from, object target, int version)
        {
            if(target is BaseWeapon)
            {
                ((BaseWeapon)target).WeaponAttributes.ResistPoisonBonus -= 10;
            } else
            if(target is BaseShield)
            {
                ((BaseShield)target).PoisonBonus -= 10;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).PoisonBonus -= 10;
            } else
            if(target is BaseJewel)
            {
                ((BaseJewel)target).Resistances.Poison -= 10;
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
    // Ral Crystal
    // --------------------------------------------------

    public class RalCrystal : BaseSocketAugmentation, ICrystalAugment
    {

        [Constructable]
        public RalCrystal() : base(0xF8E)
        {
            Name = "Ral Crystal";
            Hue = 75;
        }

        public RalCrystal( Serial serial ) : base( serial )
		{
		}

        public override int SocketsRequired {get { return 2; } }

        public override int IconXOffset { get { return 15;} }

        public override int IconYOffset { get { return 15;} }


        public override string OnIdentify(Mobile from)
        {
            return "Weapon, Shield, Armor, Jewelry: +10 Resist energy";
        }

        public override bool OnAugment(Mobile from, object target)
        {
            if(target is BaseWeapon)
            {
                ((BaseWeapon)target).WeaponAttributes.ResistEnergyBonus += 10;
            } else
            if(target is BaseShield)
            {
                ((BaseShield)target).EnergyBonus += 10;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).EnergyBonus += 10;
            } else
            if(target is BaseJewel)
            {
                ((BaseJewel)target).Resistances.Energy += 10;
            } else
			{
                return false;
			}

            return true;
        }

        
        public override bool CanAugment(Mobile from, object target)
        {
            return (target is BaseWeapon || target is BaseArmor || target is BaseJewel);
        }
        
        public override bool OnRecover(Mobile from, object target, int version)
        {
            if(target is BaseWeapon)
            {
                ((BaseWeapon)target).WeaponAttributes.ResistEnergyBonus -= 10;
            } else
            if(target is BaseShield)
            {
                ((BaseShield)target).EnergyBonus -= 10;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).EnergyBonus -= 10;
            } else
            if(target is BaseJewel)
            {
                ((BaseJewel)target).Resistances.Energy -= 10;
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
    // Kal Crystal
    // --------------------------------------------------

    public class KalCrystal : BaseSocketAugmentation, ICrystalAugment
    {

        [Constructable]
        public KalCrystal() : base(0xF8E)
        {
            Name = "Kal Crystal";
            Hue = 46;
        }

        public KalCrystal( Serial serial ) : base( serial )
		{
		}

        public override int SocketsRequired {get { return 2; } }

        public override int IconXOffset { get { return 15;} }

        public override int IconYOffset { get { return 15;} }


        public override string OnIdentify(Mobile from)
        {
            return "Weapon, Shield, Armor, Jewelry: +10 Resist physical";
        }

        public override bool OnAugment(Mobile from, object target)
        {
            if(target is BaseWeapon)
            {
                ((BaseWeapon)target).WeaponAttributes.ResistPhysicalBonus += 10;
            } else
            if(target is BaseShield)
            {
                ((BaseShield)target).PhysicalBonus += 10;
			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).PhysicalBonus += 10;
            } else
            if(target is BaseJewel)
            {
                ((BaseJewel)target).Resistances.Physical += 10;
            } else
			{
                return false;
			}

            return true;
        }

        
        public override bool CanAugment(Mobile from, object target)
        {
            return (target is BaseWeapon || target is BaseArmor || target is BaseJewel);
        }
        
        public override bool OnRecover(Mobile from, object target, int version)
        {
            if(target is BaseWeapon)
            {
                ((BaseWeapon)target).WeaponAttributes.ResistPhysicalBonus -= 10;
            } else
            if(target is BaseShield)
            {
                ((BaseShield)target).PhysicalBonus -= 10;
			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).PhysicalBonus -= 10;
            } else
            if(target is BaseJewel)
            {
                ((BaseJewel)target).Resistances.Physical -= 10;
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
