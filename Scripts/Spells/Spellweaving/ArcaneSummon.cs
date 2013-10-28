using System;
using Server.Mobiles;

namespace Server.Spells.Spellweaving
{
    public abstract class ArcaneSummon<T> : ArcanistSpell where T : BaseCreature
    {
        public ArcaneSummon(Mobile caster, Item scroll, SpellInfo info)
            : base(caster, scroll, info)
        {
        }

        public abstract int Sound { get; }
        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;

            if ((this.Caster.Followers + 1) > this.Caster.FollowersMax)
            {
                this.Caster.SendLocalizedMessage(1074270); // You have too many followers to summon another one.
                return false;
            }

            return true;
        }

        public override void OnCast()
        {
            if (this.CheckSequence())
            {
                TimeSpan duration = TimeSpan.FromMinutes(this.Caster.Skills.Spellweaving.Value / 24 + this.FocusLevel * 2);
                int summons = Math.Min(1 + this.FocusLevel, this.Caster.FollowersMax - this.Caster.Followers);

                for (int i = 0; i < summons; i++)
                {
                    BaseCreature bc;

                    try
                    {
                        bc = Activator.CreateInstance<T>();
                    }
                    catch
                    {
                        break;
                    }

                    SpellHelper.Summon(bc, this.Caster, this.Sound, duration, false, false);
                }

                this.FinishSequence();
            }
        }
    }
}