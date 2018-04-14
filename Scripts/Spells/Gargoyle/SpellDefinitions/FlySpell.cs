using System;
using Server.Mobiles;
using Server.Network;

namespace Server.Spells
{
    public class FlySpell : Spell
    {
        private static readonly SpellInfo m_Info = new SpellInfo("Gargoyle Flight", null, -1, 9002);

        public FlySpell(Mobile caster)
            : base(caster, null, m_Info)
        {
        }

        public override bool ClearHandsOnCast { get { return false; } }
        public override bool RevealOnCast { get { return false; } }
        public override double CastDelayFastScalar { get { return 0; } }
        public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds(2.0); } }
        public override TimeSpan GetCastRecovery() { return TimeSpan.Zero; }
        public override int GetMana() { return 0; }
        public override bool ConsumeReagents() { return true; }
        public override bool CheckFizzle() { return true; }
        public override bool CheckNextSpellTime { get { return false; } }

        public override bool CheckDisturb(DisturbType type, bool checkFirst, bool resistable)
        {
            if (type == DisturbType.EquipRequest || type == DisturbType.UseRequest)
                return false;

            return true;
        }

        public override void SayMantra()
        {
        }

        public override void OnDisturb(DisturbType type, bool message)
        {
            Caster.Flying = false;
            BuffInfo.RemoveBuff(this.Caster, BuffIcon.Fly);

            if (message)
                Caster.SendLocalizedMessage(1113192); // You have been disrupted while attempting to fly!
        }

        public static bool CheckFlyingAllowed(Mobile mob, bool message)
        {
            if (mob.Region != null && !mob.Region.AllowFlying(mob))
            {
                mob.SendMessage("You may not fly here.");
                return false;
            }

            return true;
        }

        public override bool CheckCast()
        {
            if (!CheckFlyingAllowed(Caster, true))
            {
                return false;
            }
            else if (!Caster.Alive)
            {
                Caster.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1113082); // You may not fly while dead.
            }
            else if (Factions.Sigil.ExistsOn(Caster))
            {
                Caster.SendLocalizedMessage(1061632); // You can't do that while carrying the sigil.
            }
            else if (!Caster.CanBeginAction(typeof(Seventh.PolymorphSpell)))
            {
                Caster.SendLocalizedMessage(1061628); // You can't do that while polymorphed.
            }
            else if (Ninjitsu.AnimalForm.UnderTransformation(Caster) || Mysticism.StoneFormSpell.IsEffected(Caster) || (TransformationSpellHelper.UnderTransformation(Caster)
                && !TransformationSpellHelper.UnderTransformation(Caster, typeof(Spells.Necromancy.VampiricEmbraceSpell))) || (Caster.IsBodyMod && !Caster.Body.IsHuman))
            {
                Caster.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1112453); // You can't fly in your current form!
            }
            else if (Server.Mobiles.BaseMount.CheckMountAllowed(Caster, true, true))
            {
                Caster.Flying = true;
                BuffInfo.AddBuff(Caster, new BuffInfo(BuffIcon.Fly, 1112193, 1112567)); // Flying & You are flying.
                Caster.Animate(AnimationType.TakeOff, 0);

                return true;
            }

            return false;
        }

        public override void OnCast()
        {
            FinishSequence();
        }
    }
}
