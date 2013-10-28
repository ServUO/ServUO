using System;
using Server.Gumps;
using Server.Items;

namespace Server.Spells.Mystic
{
    public class EnchantSpell : MysticSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Enchant", "In Ort Ylem",
            680,
            9022, //change to correct number.
            Reagent.MandrakeRoot,
            Reagent.SulfurousAsh,
            Reagent.SpidersSilk);
        public EnchantSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        // Temporarily enchants a held weapon with a hit spell effect.
        public override int RequiredMana
        {
            get
            {
                return 6;
            }
        }
        public override double RequiredSkill
        {
            get
            {
                return 8;
            }
        }
        public override TimeSpan CastDelayBase
        {
            get
            {
                return TimeSpan.FromSeconds(0.75);
            }
        }
        public override bool ClearHandsOnCast
        {
            get
            {
                return false;
            }
        }
        public override void OnCast()
        {
            BaseWeapon weapon = this.Caster.Weapon as BaseWeapon;

            if (weapon == null || weapon is Fists)
            {
                this.Caster.SendLocalizedMessage(501078); // You must be holding a weapon.
                return;
            }

            if (!this.Caster.CanBeginAction(typeof(EnchantSpell)))
            {
                this.Caster.SendLocalizedMessage(1005559); // This spell is already in effect.
            }
            else
            {
                if (this.CheckSequence())
                {
                    if (this.Caster.HasGump(typeof(EnchantGump)))
                        this.Caster.CloseGump(typeof(EnchantGump));

                    this.Caster.SendGump(new EnchantGump());

                    this.Caster.PlaySound(0x387);
                    this.Caster.FixedParticles(0x3779, 1, 15, 9905, 32, 2, EffectLayer.Head);
                    this.Caster.FixedParticles(0x37B9, 1, 14, 9502, 32, 5, (EffectLayer)255);
                }
            }
        }
    }
}