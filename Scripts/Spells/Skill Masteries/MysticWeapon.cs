using Server.Items;
using System;

namespace Server.Spells.SkillMasteries
{
    public class MysticWeaponSpell : SkillMasterySpell
    {
        public static string ModName = "MysticWeapon";

        private static readonly SpellInfo m_Info = new SpellInfo(
                "Mystic Weapon", "Vas Ylem Wis",
                -1,
                9002,
                Reagent.FertileDirt,
                Reagent.Bone
            );

        public override double RequiredSkill => 90;
        public override int RequiredMana => 40;
        public override bool PartyEffects => false;

        public override SkillName CastSkill => SkillName.Mysticism;
        public override SkillName DamageSkill
        {
            get
            {
                if (Caster.Skills[SkillName.Focus].Value > Caster.Skills[SkillName.Imbuing].Value)
                    return SkillName.Focus;

                return SkillName.Imbuing;
            }
        }

        private BaseWeapon _Weapon;

        public MysticWeaponSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override void SendCastEffect()
        {
            Caster.FixedEffect(0x37C4, 87, (int)(GetCastDelay().TotalSeconds * 28), 0x66C, 3);
        }

        public override bool CheckCast()
        {
            BaseWeapon weapon = GetWeapon();

            if (weapon == null || weapon is Fists)
            {
                Caster.SendLocalizedMessage(1060179); //You must be wielding a weapon to use this ability!
                return false;
            }
            else if (weapon.ExtendedWeaponAttributes.MysticWeapon > 0 || Enhancement.GetValue(Caster, ExtendedWeaponAttribute.MysticWeapon) > 0)
            {
                Caster.SendMessage("That weapon is already under these effects.");
                return false;
            }

            return base.CheckCast();
        }

        public override void OnCast()
        {
            BaseWeapon weapon = GetWeapon();

            if (weapon == null || weapon is Fists)
                Caster.SendLocalizedMessage(1060179); //You must be wielding a weapon to use this ability!
            else if (CheckSequence())
            {
                double skill = ((Caster.Skills[CastSkill].Value * 1.5) + Caster.Skills[DamageSkill].Value);
                double duration = (skill + (GetMasteryLevel() * 50)) * 2;

                Enhancement.SetValue(Caster, ExtendedWeaponAttribute.MysticWeapon, 25, "MysticWeapon");
                BuffInfo.AddBuff(Caster, new BuffInfo(BuffIcon.MysticWeapon, 1155899, 1156055, TimeSpan.FromSeconds(duration), Caster));

                Effects.SendLocationParticles(EffectItem.Create(Caster.Location, Caster.Map, EffectItem.DefaultDuration), 0x36CB, 1, 14, 0x55C, 7, 9915, 0);
                Caster.PlaySound(0x64E);

                weapon.AddMysticMod(Caster);
                weapon.InvalidateProperties();

                Expires = DateTime.UtcNow + TimeSpan.FromSeconds(duration);
                BeginTimer();

                _Weapon = weapon;
            }

            FinishSequence();
        }

        public override void OnWeaponRemoved(BaseWeapon weapon)
        {
            Expire();
        }

        public override void EndEffects()
        {
            BuffInfo.RemoveBuff(Caster, BuffIcon.MysticWeapon);

            Enhancement.RemoveMobile(Caster);
            _Weapon.RemoveMysticMod();

            Caster.SendLocalizedMessage(1115273); // The enchantment on your weapon has expired.
            Caster.PlaySound(0x1ED);
        }
    }
}