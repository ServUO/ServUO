//----------------------------------------------------------------------------------//
// Created by Vano. Email: vano2006uo@mail.ru      //
//---------------------------------------------------------------------------------//
using System;
using System.Collections;

namespace Server.Items
{
    public class PsychicAttack : WeaponAbility
    {
        private readonly Hashtable m_Table = new Hashtable();
        public PsychicAttack()
        {
        }

        public override int BaseMana
        {
            get
            {
                return 30;
            }
        }
        public override void OnHit(Mobile attacker, Mobile defender, int damage)
        {
            if (!this.Validate(attacker) || !this.CheckMana(attacker, true))
                return;

            ClearCurrentAbility(attacker);

            attacker.SendMessage("You attack by psychic energy"); 
            defender.SendMessage("You attacking by psychic energy");

            //defender.Mana -= Utility.Random( (int)attacker.Skills[SkillName.Anatomy].Value/10, (int)attacker.Skills[SkillName.Anatomy].Value/5 );
							
            int toDrain = defender.Mana;

            if (toDrain < 0)
                toDrain = 0;
            else if (toDrain > defender.Mana)
                toDrain = defender.Mana;

            if (this.m_Table.Contains(defender))
                toDrain = 0;

            defender.FixedParticles(0x3789, 10, 25, 5032, EffectLayer.Head);
            defender.PlaySound(0x1F8);

            if (toDrain > 0)
            {
                defender.Mana -= toDrain;

                this.m_Table[defender] = Timer.DelayCall(TimeSpan.FromSeconds(5.0), new TimerStateCallback(AosDelay_Callback), new object[] { defender, toDrain });
            }
        }

        private void AosDelay_Callback(object state)
        {
            object[] states = (object[])state;

            Mobile m = (Mobile)states[0];
            int mana = (int)states[1];

            if (m.Alive && !m.IsDeadBondedPet)
            {
                m.Mana += mana;

                m.FixedEffect(0x3779, 10, 25);
                m.PlaySound(0x28E);
            }

            this.m_Table.Remove(m);
        }
    }
}