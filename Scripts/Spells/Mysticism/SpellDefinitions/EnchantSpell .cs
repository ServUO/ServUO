using System;
using Server.Gumps;
using Server.Items;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Spells;
using Server.Network;

namespace Server.Spells.Mystic
{
	public class EnchantSpell : MysticSpell
	{
        private static readonly string ModName = "EnchantAttribute";

        public override SpellCircle Circle { get { return SpellCircle.Second; } }
        public override bool ClearHandsOnCast { get { return false; } }

        public BaseWeapon Weapon { get; set; }
        public AosWeaponAttribute Attribute { get; set; }

		private static SpellInfo m_Info = new SpellInfo(
                "Enchant", "In Ort Ylem",
				230,
				9022,
				Reagent.SpidersSilk,
				Reagent.MandrakeRoot,
				Reagent.SulfurousAsh
			);

		public EnchantSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

        public EnchantSpell( Mobile caster, Item scroll, BaseWeapon weapon, AosWeaponAttribute attribute ) : base( caster, scroll, m_Info )
        {
            Weapon = weapon;
            this.Attribute = attribute;
        }

        public override bool CheckCast()
        {
            if (Weapon == null)
            {
                BaseWeapon wep = Caster.Weapon as BaseWeapon;

                if (wep == null)
                    Caster.SendLocalizedMessage(501078); // You must be holding a weapon.
                else
                {
                    if (Caster.HasGump(typeof(EnchantSpellGump)))
                        Caster.CloseGump(typeof(EnchantSpellGump));

                    Caster.SendGump(new EnchantSpellGump(Caster, Scroll, wep));
                }
            }
            else if (IsUnderSpellEffects(Caster, Weapon) || Weapon.Immolating || Weapon.Consecrated)
                Caster.SendLocalizedMessage(1080128); //You cannot use this ability while your weapon is enchanted.
            else if (Weapon.FocusWeilder != null)
                Caster.SendLocalizedMessage(1080446); // You cannot enchant an item that is under the effects of the ninjitsu focus attack ability.
            else
                return true;

            return false;
        }

        public override void OnCast()
        {
            if (CheckWSequence() && CheckSequence() && Caster.Weapon == Weapon)
            {
                Caster.PlaySound(0x64E);
                Caster.FixedEffect(0x37C4, 10, 14, 4, 3);

                Caster.SendMessage("You enchant the weapon.");

                int prim = (int)Caster.Skills[CastSkill].Value;
                int sec = (int)Caster.Skills[DamageSkill].Value;

                int value = (50 * (prim + sec)) / 240;
                double duration = Math.Max(30, (double)(prim + sec) / 2.0);

                if (Table == null)
                    Table = new Dictionary<Mobile, EnchantmentTimer>();

                if (Table.ContainsKey(Caster))
                    RemoveEnchantment(Caster);

                Enhancement.SetValue(Caster, this.Attribute, value, ModName);

                if ((Weapon.ArtifactRarity > 0 || Weapon.Name != null) && Weapon.Attributes.CastSpeed >= 0)
                    Enhancement.SetValue(Caster, AosAttribute.CastSpeed, -1, ModName + "CastSpeed");

                Table[Caster] = new EnchantmentTimer(Caster, Weapon, this.Attribute, value, duration);

                Weapon.EnchantedWeilder = Caster;

                Weapon.InvalidateProperties();
            }

            FinishSequence();
        }

        public static Dictionary<Mobile, EnchantmentTimer> Table { get; set; }

        public static bool IsUnderSpellEffects(Mobile Caster, BaseWeapon wep)
        {
            if (Table == null)
                return false;

            return Table.ContainsKey(Caster) && Table[Caster].Weapon == wep;
        }

        public static AosWeaponAttribute BonusAttribute(Mobile from)
        {
            if (Table.ContainsKey(from))
                return Table[from].WeaponAttribute;

            return AosWeaponAttribute.HitColdArea;
        }

        public static int BonusValue(Mobile from)
        {
            if (Table.ContainsKey(from))
                return Table[from].AttributeValue;

            return 0;
        }

        public static bool CastingMalus(Mobile from, BaseWeapon weapon)
        {
            if (Table.ContainsKey(from))
            {
                BaseWeapon wep = Table[from].Weapon;
                return weapon != null && (weapon.ArtifactRarity > 0 || weapon.Name != null) && weapon.Attributes.CastSpeed >= 0;
            }

            return false;
        }

        public static void RemoveEnchantment(Mobile Caster)
        {
            if(Table != null && Table.ContainsKey(Caster))
            {
                Table[Caster].Stop();
                Table[Caster] = null;
                Table.Remove(Caster);

                Enhancement.RemoveMobile(Caster);
            }
        }

        public static void OnWeaponRemoved(BaseWeapon wep, Mobile from)
        {
            if(IsUnderSpellEffects(from, wep))
                RemoveEnchantment(from);

            if (wep.EnchantedWeilder != null)
                wep.EnchantedWeilder = null;
        }

        private bool CheckWSequence()
        {
            BaseWeapon wep = Caster.Weapon as BaseWeapon;

            if (wep == null || wep is Fists)
            {
                Caster.SendMessage("You must have a weapon equipped to cast this spell.");
                return false;
            }

            int v = wep.WeaponAttributes.HitFireball;
            v += wep.WeaponAttributes.HitLightning;
            v += wep.WeaponAttributes.HitMagicArrow;
            v += wep.WeaponAttributes.HitHarm;
            v += wep.WeaponAttributes.HitDispel;

            if (v > 0)
            {
                Caster.SendLocalizedMessage(1080127); //This weapon already has a hit spell effect and cannot be enchanted.
                return false;
            }

            return true;
        }
	}

    public class EnchantmentTimer : Timer
    {
        public Mobile Owner { get; set; }
        public BaseWeapon Weapon { get; set; }
        public AosWeaponAttribute WeaponAttribute { get; set; }
        public int AttributeValue { get; set; }

        public EnchantmentTimer(Mobile owner, BaseWeapon wep, AosWeaponAttribute attribute, int value, double duration) : base(TimeSpan.FromSeconds(duration))
        {
            Owner = owner;
            Weapon = wep;
            WeaponAttribute = attribute;
            AttributeValue = value;

            this.Start();
        }

        protected override void OnTick()
        {
            EnchantSpell.RemoveEnchantment(Owner);
        }
    }
}