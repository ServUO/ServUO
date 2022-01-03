using System;
using Server;
using Server.Mobiles;
using Server.Engines.XmlSpawner2;

namespace Server.Items
{


    // --------------------------------------------------
    // Radiant Rho Crystal
    // --------------------------------------------------

    public class RadiantRhoCrystal : BaseSocketAugmentation
    {

        [Constructable]
        public RadiantRhoCrystal() : base(0xF8E)
        {
            Name = "Radiant Rho Crystal";
            Hue = 13;
        }

        public RadiantRhoCrystal( Serial serial ) : base( serial )
		{
		}

        public override int SocketsRequired {get { return 4; } }

        public override int IconXOffset { get { return 15;} }

        public override int IconYOffset { get { return 15;} }


        public override string OnIdentify(Mobile from)
        {
            return "Weapon, Shield, Armor, Jewelry: +3 Faster Casting";
        }

        public override bool OnAugment(Mobile from, object target)
        {
            if(target is BaseWeapon)
            {
                ((BaseWeapon)target).Attributes.CastSpeed += 3;
            } else
            if(target is BaseShield)
            {
                ((BaseShield)target).Attributes.CastSpeed += 3;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).Attributes.CastSpeed += 3;
            } else
            if(target is BaseJewel)
            {
                ((BaseJewel)target).Attributes.CastSpeed += 3;
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
                ((BaseWeapon)target).Attributes.CastSpeed -= 3;
            } else
            if(target is BaseShield)
            {
                ((BaseShield)target).Attributes.CastSpeed -= 3;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).Attributes.CastSpeed -= 3;
            } else
            if(target is BaseJewel)
            {
                ((BaseJewel)target).Attributes.CastSpeed -= 3;
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
    // Radiant Rys Crystal
    // --------------------------------------------------

    public class RadiantRysCrystal : BaseSocketAugmentation
    {

        [Constructable]
        public RadiantRysCrystal() : base(0xF8E)
        {
            Name = "Radiant Rys Crystal";
            Hue = 18;
        }

        public RadiantRysCrystal( Serial serial ) : base( serial )
		{
		}

        public override int SocketsRequired {get { return 4; } }

        public override int IconXOffset { get { return 15;} }

        public override int IconYOffset { get { return 15;} }


        public override string OnIdentify(Mobile from)
        {
            return "Weapon, Shield, Armor, Jewelry: +3 Faster Cast Recovery";
        }

        public override bool OnAugment(Mobile from, object target)
        {
            if(target is BaseWeapon)
            {
                ((BaseWeapon)target).Attributes.CastRecovery += 3;
            } else
            if(target is BaseShield)
            {
                ((BaseShield)target).Attributes.CastRecovery += 3;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).Attributes.CastRecovery += 3;
            } else
            if(target is BaseJewel)
            {
                ((BaseJewel)target).Attributes.CastRecovery += 3;
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
                ((BaseWeapon)target).Attributes.CastRecovery -= 3;
            } else
            if(target is BaseShield)
            {
                ((BaseShield)target).Attributes.CastRecovery -= 3;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).Attributes.CastRecovery -= 3;
            } else
            if(target is BaseJewel)
            {
                ((BaseJewel)target).Attributes.CastRecovery -= 3;
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
    // Radiant Wyr Crystal
    // --------------------------------------------------

    public class RadiantWyrCrystal : BaseSocketAugmentation
    {

        [Constructable]
        public RadiantWyrCrystal() : base(0xF8E)
        {
            Name = "Radiant Wyr Crystal";
            Hue = 23;
        }

        public RadiantWyrCrystal( Serial serial ) : base( serial )
		{
		}

        public override int SocketsRequired {get { return 4; } }

        public override int IconXOffset { get { return 15;} }

        public override int IconYOffset { get { return 15;} }


        public override string OnIdentify(Mobile from)
        {
            return "Weapon, Shield, Armor, Jewelry: +200 Luck";
        }

        public override bool OnAugment(Mobile from, object target)
        {
            if(target is BaseWeapon)
            {
                ((BaseWeapon)target).Attributes.Luck += 200;
            } else
            if(target is BaseShield)
            {
                ((BaseShield)target).Attributes.Luck += 200;
			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).Attributes.Luck += 200;
            } else
            if(target is BaseJewel)
            {
                ((BaseJewel)target).Attributes.Luck += 200;
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
                ((BaseWeapon)target).Attributes.Luck -= 200;
            } else
            if(target is BaseShield)
            {
                ((BaseShield)target).Attributes.Luck -= 200;
			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).Attributes.Luck -= 200;
            } else
            if(target is BaseJewel)
            {
                ((BaseJewel)target).Attributes.Luck -= 200;
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
    // Radiant Fre Crystal
    // --------------------------------------------------

    public class RadiantFreCrystal : BaseSocketAugmentation
    {

        [Constructable]
        public RadiantFreCrystal() : base(0xF8E)
        {
            Name = "Radiant Fre Crystal";
            Hue = 28;
        }

        public RadiantFreCrystal( Serial serial ) : base( serial )
		{
		}

        public override int SocketsRequired {get { return 4; } }

        public override int IconXOffset { get { return 15;} }

        public override int IconYOffset { get { return 15;} }


        public override string OnIdentify(Mobile from)
        {
            return "Weapon, Shield, Armor, Jewelry: +25 Enhance potions";
        }

        public override bool OnAugment(Mobile from, object target)
        {
            if(target is BaseWeapon)
            {
                ((BaseWeapon)target).Attributes.EnhancePotions += 25;
            } else
            if(target is BaseShield)
            {
                ((BaseShield)target).Attributes.EnhancePotions += 25;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).Attributes.EnhancePotions += 25;
            } else
            if(target is BaseJewel)
            {
                ((BaseJewel)target).Attributes.EnhancePotions += 25;
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
                ((BaseWeapon)target).Attributes.EnhancePotions -= 25;
            } else
            if(target is BaseShield)
            {
                ((BaseShield)target).Attributes.EnhancePotions -= 25;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).Attributes.EnhancePotions -= 25;
            } else
            if(target is BaseJewel)
            {
                ((BaseJewel)target).Attributes.EnhancePotions -= 25;
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
    // Radiant Tor Crystal
    // --------------------------------------------------

    public class RadiantTorCrystal : BaseSocketAugmentation
    {

        [Constructable]
        public RadiantTorCrystal() : base(0xF8E)
        {
            Name = "Radiant Tor Crystal";
            Hue = 33;
        }

        public RadiantTorCrystal( Serial serial ) : base( serial )
		{
		}

        public override int SocketsRequired {get { return 4; } }

        public override int IconXOffset { get { return 15;} }

        public override int IconYOffset { get { return 15;} }


        public override string OnIdentify(Mobile from)
        {
            return "Weapon, Shield, Armor: +25 Lower reagent cost";
        }

        public override bool OnAugment(Mobile from, object target)
        {
            if(target is BaseWeapon)
            {
                ((BaseWeapon)target).Attributes.LowerRegCost += 25;
            } else
            if(target is BaseShield)
            {
                ((BaseShield)target).Attributes.LowerRegCost += 25;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).Attributes.LowerRegCost += 25;
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
                ((BaseWeapon)target).Attributes.LowerRegCost -= 25;
            } else
            if(target is BaseShield)
            {
                ((BaseShield)target).Attributes.LowerRegCost -= 25;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).Attributes.LowerRegCost -= 25;
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
    // Radiant Vel Crystal
    // --------------------------------------------------

    public class RadiantVelCrystal : BaseSocketAugmentation
    {

        [Constructable]
        public RadiantVelCrystal() : base(0xF8E)
        {
            Name = "Radiant Vel Crystal";
            Hue = 38;
        }

        public RadiantVelCrystal( Serial serial ) : base( serial )
		{
		}

        public override int SocketsRequired {get { return 4; } }

        public override int IconXOffset { get { return 15;} }

        public override int IconYOffset { get { return 15;} }


        public override string OnIdentify(Mobile from)
        {
            return "Weapon, Shield, Armor, Jewelry: +10 Lower mana cost";
        }

        public override bool OnAugment(Mobile from, object target)
        {
            if(target is BaseWeapon)
            {
                ((BaseWeapon)target).Attributes.LowerManaCost += 10;
            } else
            if(target is BaseShield)
            {
                ((BaseShield)target).Attributes.LowerManaCost += 10;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).Attributes.LowerManaCost += 10;
            } else
            if(target is BaseJewel)
            {
                ((BaseJewel)target).Attributes.LowerManaCost += 10;
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
                ((BaseWeapon)target).Attributes.LowerManaCost -= 10;
            } else
            if(target is BaseShield)
            {
                ((BaseShield)target).Attributes.LowerManaCost -= 10;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).Attributes.LowerManaCost -= 10;
            } else
            if(target is BaseJewel)
            {
                ((BaseJewel)target).Attributes.LowerManaCost -= 10;
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
    // Radiant Xen Crystal
    // --------------------------------------------------

    public class RadiantXenCrystal : BaseSocketAugmentation
    {

        [Constructable]
        public RadiantXenCrystal() : base(0xF8E)
        {
            Name = "Radiant Xen Crystal";
            Hue = 43;
        }

        public RadiantXenCrystal( Serial serial ) : base( serial )
		{
		}

        public override int SocketsRequired {get { return 4; } }

        public override int IconXOffset { get { return 15;} }

        public override int IconYOffset { get { return 15;} }


        public override string OnIdentify(Mobile from)
        {
            return "Weapon, Shield, Armor, Jewelry: +12 Spell damage";
        }

        public override bool OnAugment(Mobile from, object target)
        {
            if(target is BaseWeapon)
            {
                ((BaseWeapon)target).Attributes.SpellDamage += 12;
            } else
            if(target is BaseShield)
            {
                ((BaseShield)target).Attributes.SpellDamage += 12;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).Attributes.SpellDamage += 12;
            } else
            if(target is BaseJewel)
            {
                ((BaseJewel)target).Attributes.SpellDamage += 12;
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
                ((BaseWeapon)target).Attributes.SpellDamage -= 12;
            } else
            if(target is BaseShield)
            {
                ((BaseShield)target).Attributes.SpellDamage -= 12;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).Attributes.SpellDamage -= 12;
            } else
            if(target is BaseJewel)
            {
                ((BaseJewel)target).Attributes.SpellDamage -= 12;
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
    // Radiant Pol Crystal
    // --------------------------------------------------

    public class RadiantPolCrystal : BaseSocketAugmentation
    {

        [Constructable]
        public RadiantPolCrystal() : base(0xF8E)
        {
            Name = "Radiant Pol Crystal";
            Hue = 48;
        }

        public RadiantPolCrystal( Serial serial ) : base( serial )
		{
		}

        public override int SocketsRequired {get { return 4; } }

        public override int IconXOffset { get { return 15;} }

        public override int IconYOffset { get { return 15;} }


        public override string OnIdentify(Mobile from)
        {
            return "Weapon, Shield, Armor: +125 Durability\nJewelry: +15 Attack Chance";
        }

        public override bool OnAugment(Mobile from, object target)
        {
            if(target is BaseWeapon)
            {
                ((BaseWeapon)target).WeaponAttributes.DurabilityBonus += 125;
            } else
            if(target is BaseShield)
            {
                ((BaseShield)target).ArmorAttributes.DurabilityBonus += 125;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).ArmorAttributes.DurabilityBonus += 125;
            } else
            if(target is BaseJewel)
            {
                ((BaseJewel)target).Attributes.AttackChance += 15;
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
                ((BaseWeapon)target).WeaponAttributes.DurabilityBonus -= 125;
            } else
            if(target is BaseShield)
            {
                ((BaseShield)target).ArmorAttributes.DurabilityBonus -= 125;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).ArmorAttributes.DurabilityBonus -= 125;
            } else
            if(target is BaseJewel)
            {
                ((BaseJewel)target).Attributes.AttackChance -= 15;
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
    // Radiant Wol Crystal
    // --------------------------------------------------

    public class RadiantWolCrystal : BaseSocketAugmentation
    {

        [Constructable]
        public RadiantWolCrystal() : base(0xF8E)
        {
            Name = "Radiant Wol Crystal";
            Hue = 53;
        }

        public RadiantWolCrystal( Serial serial ) : base( serial )
		{
		}

        public override int SocketsRequired {get { return 4; } }

        public override int IconXOffset { get { return 15;} }

        public override int IconYOffset { get { return 15;} }


        public override string OnIdentify(Mobile from)
        {
            return "Weapon, Shield, Armor: +5 Self-repair\nJewelry: +15 Defend Chance";
        }

        public override bool OnAugment(Mobile from, object target)
        {
            if(target is BaseWeapon)
            {
                ((BaseWeapon)target).WeaponAttributes.SelfRepair += 5;
            } else
            if(target is BaseShield)
            {
                ((BaseShield)target).ArmorAttributes.SelfRepair += 5;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).ArmorAttributes.SelfRepair += 5;
            } else
            if(target is BaseJewel)
            {
                ((BaseJewel)target).Attributes.DefendChance += 15;
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
                ((BaseWeapon)target).WeaponAttributes.SelfRepair -= 5;
            } else
            if(target is BaseShield)
            {
                ((BaseShield)target).ArmorAttributes.SelfRepair -= 5;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).ArmorAttributes.SelfRepair -= 5;
            } else
            if(target is BaseJewel)
            {
                ((BaseJewel)target).Attributes.DefendChance -= 15;
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
    // Radiant Bal Crystal
    // --------------------------------------------------

    public class RadiantBalCrystal : BaseSocketAugmentation
    {

        [Constructable]
        public RadiantBalCrystal() : base(0xF8E)
        {
            Name = "Radiant Bal Crystal";
            Hue = 133;
        }

        public RadiantBalCrystal( Serial serial ) : base( serial )
		{
		}

        public override int SocketsRequired {get { return 4; } }

        public override int IconXOffset { get { return 15;} }

        public override int IconYOffset { get { return 15;} }


        public override string OnIdentify(Mobile from)
        {
            return "Weapon, Shield, Armor, Jewelry: +25 Resist fire";
        }

        public override bool OnAugment(Mobile from, object target)
        {
            if(target is BaseWeapon)
            {
                ((BaseWeapon)target).WeaponAttributes.ResistFireBonus += 25;
            } else
            if(target is BaseShield)
            {
                ((BaseShield)target).FireBonus += 25;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).FireBonus += 25;
            } else
            if(target is BaseJewel)
            {
                ((BaseJewel)target).Resistances.Fire += 25;
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
                ((BaseWeapon)target).WeaponAttributes.ResistFireBonus -= 25;
            } else
            if(target is BaseShield)
            {
                ((BaseShield)target).FireBonus -= 25;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).FireBonus -= 25;
            } else
            if(target is BaseJewel)
            {
                ((BaseJewel)target).Resistances.Fire -= 25;
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
    // Radiant Tal Crystal
    // --------------------------------------------------

    public class RadiantTalCrystal : BaseSocketAugmentation
    {

        [Constructable]
        public RadiantTalCrystal() : base(0xF8E)
        {
            Name = "Radiant Tal Crystal";
            Hue = 88;
        }

        public RadiantTalCrystal( Serial serial ) : base( serial )
		{
		}

        public override int SocketsRequired {get { return 4; } }

        public override int IconXOffset { get { return 15;} }

        public override int IconYOffset { get { return 15;} }


        public override string OnIdentify(Mobile from)
        {
            return "Weapon, Shield, Armor, Jewelry: +25 Resist cold";
        }

        public override bool OnAugment(Mobile from, object target)
        {
            if(target is BaseWeapon)
            {
                ((BaseWeapon)target).WeaponAttributes.ResistColdBonus += 25;
            } else
            if(target is BaseShield)
            {
                ((BaseShield)target).ColdBonus += 25;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).ColdBonus += 25;
            } else
            if(target is BaseJewel)
            {
                ((BaseJewel)target).Resistances.Cold += 25;
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
                ((BaseWeapon)target).WeaponAttributes.ResistColdBonus -= 25;
            } else
            if(target is BaseShield)
            {
                ((BaseShield)target).ColdBonus -= 25;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).ColdBonus -= 25;
            } else
            if(target is BaseJewel)
            {
                ((BaseJewel)target).Resistances.Cold -= 25;
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
    // Radiant Jal Crystal
    // --------------------------------------------------

    public class RadiantJalCrystal : BaseSocketAugmentation
    {

        [Constructable]
        public RadiantJalCrystal() : base(0xF8E)
        {
            Name = "Radiant Jal Crystal";
            Hue = 70;
        }

        public RadiantJalCrystal( Serial serial ) : base( serial )
		{
		}

        public override int SocketsRequired {get { return 4; } }

        public override int IconXOffset { get { return 15;} }

        public override int IconYOffset { get { return 15;} }


        public override string OnIdentify(Mobile from)
        {
            return "Weapon, Shield, Armor, Jewelry: +25 Resist poison";
        }

        public override bool OnAugment(Mobile from, object target)
        {
            if(target is BaseWeapon)
            {
                ((BaseWeapon)target).WeaponAttributes.ResistPoisonBonus += 25;
            } else
            if(target is BaseShield)
            {
                ((BaseShield)target).PoisonBonus += 25;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).PoisonBonus += 25;
            } else
            if(target is BaseJewel)
            {
                ((BaseJewel)target).Resistances.Poison += 25;
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
                ((BaseWeapon)target).WeaponAttributes.ResistPoisonBonus -= 25;
            } else
            if(target is BaseShield)
            {
                ((BaseShield)target).PoisonBonus -= 25;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).PoisonBonus -= 25;
            } else
            if(target is BaseJewel)
            {
                ((BaseJewel)target).Resistances.Poison -= 25;
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
    // Radiant Ral Crystal
    // --------------------------------------------------

    public class RadiantRalCrystal : BaseSocketAugmentation
    {

        [Constructable]
        public RadiantRalCrystal() : base(0xF8E)
        {
            Name = "Radiant Ral Crystal";
            Hue = 75;
        }

        public RadiantRalCrystal( Serial serial ) : base( serial )
		{
		}

        public override int SocketsRequired {get { return 4; } }

        public override int IconXOffset { get { return 15;} }

        public override int IconYOffset { get { return 15;} }


        public override string OnIdentify(Mobile from)
        {
            return "Weapon, Shield, Armor, Jewelry: +25 Resist energy";
        }

        public override bool OnAugment(Mobile from, object target)
        {
            if(target is BaseWeapon)
            {
                ((BaseWeapon)target).WeaponAttributes.ResistEnergyBonus += 25;
            } else
            if(target is BaseShield)
            {
                ((BaseShield)target).EnergyBonus += 25;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).EnergyBonus += 25;
            } else
            if(target is BaseJewel)
            {
                ((BaseJewel)target).Resistances.Energy += 25;
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
                ((BaseWeapon)target).WeaponAttributes.ResistEnergyBonus -= 25;
            } else
            if(target is BaseShield)
            {
                ((BaseShield)target).EnergyBonus -= 25;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).EnergyBonus -= 25;
            } else
            if(target is BaseJewel)
            {
                ((BaseJewel)target).Resistances.Energy -= 25;
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
    // Radiant Kal Crystal
    // --------------------------------------------------

    public class RadiantKalCrystal : BaseSocketAugmentation
    {

        [Constructable]
        public RadiantKalCrystal() : base(0xF8E)
        {
            Name = "Kal Crystal";
            Hue = 46;
        }

        public RadiantKalCrystal( Serial serial ) : base( serial )
		{
		}

        public override int SocketsRequired {get { return 4; } }

        public override int IconXOffset { get { return 15;} }

        public override int IconYOffset { get { return 15;} }


        public override string OnIdentify(Mobile from)
        {
            return "Weapon, Shield, Armor, Jewelry: +25 Resist physical";
        }

        public override bool OnAugment(Mobile from, object target)
        {
            if(target is BaseWeapon)
            {
                ((BaseWeapon)target).WeaponAttributes.ResistPhysicalBonus += 25;
            } else
            if(target is BaseShield)
            {
                ((BaseShield)target).PhysicalBonus += 25;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).PhysicalBonus += 25;
            } else
            if(target is BaseJewel)
            {
                ((BaseJewel)target).Resistances.Physical += 25;
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
                ((BaseWeapon)target).WeaponAttributes.ResistPhysicalBonus -= 25;
            } else
            if(target is BaseShield)
            {
                ((BaseShield)target).PhysicalBonus -= 25;

			} else
            if(target is BaseArmor)
            {
                ((BaseArmor)target).PhysicalBonus -= 25;
            } else
            if(target is BaseJewel)
            {
                ((BaseJewel)target).Resistances.Physical -= 25;
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
