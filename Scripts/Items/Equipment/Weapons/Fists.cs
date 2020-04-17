using System;

namespace Server.Items
{
    public class Fists : BaseMeleeWeapon
    {
        public static void Initialize()
        {
            if (Mobile.DefaultWeapon == null)
                Mobile.DefaultWeapon = new Fists();
        }

        public override WeaponAbility PrimaryAbility => WeaponAbility.Disarm;
        public override WeaponAbility SecondaryAbility => WeaponAbility.ParalyzingBlow;

        public override int StrengthReq => 0;
        public override int MinDamage => 1;
        public override int MaxDamage => 6;
        public override float Speed => 2.50f;

        public override int DefHitSound => -1;
        public override int DefMissSound => -1;

        public override SkillName DefSkill => SkillName.Wrestling;
        public override WeaponType DefType => WeaponType.Fists;
        public override WeaponAnimation DefAnimation => WeaponAnimation.Wrestle;

        public Fists()
            : base(0)
        {
            Visible = false;
            Movable = false;
            Quality = ItemQuality.Normal;
        }

        public Fists(Serial serial)
            : base(serial)
        {
        }

        public override double GetDefendSkillValue(Mobile attacker, Mobile defender)
        {
            double wresValue = defender.Skills[SkillName.Wrestling].Value;
            double anatValue = defender.Skills[SkillName.Anatomy].Value;
            double evalValue = defender.Skills[SkillName.EvalInt].Value;
            double incrValue = (anatValue + evalValue + 20.0) * 0.5;

            if (incrValue > 120.0)
                incrValue = 120.0;

            if (wresValue > incrValue)
                return wresValue;
            else
                return incrValue;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (Mobile.DefaultWeapon == null)
                Mobile.DefaultWeapon = this;
            else
                Delete();
        }

        /* Wrestling moves */

        private static bool CheckMove(Mobile m, SkillName other)
        {
            double wresValue = m.Skills[SkillName.Wrestling].Value;
            double scndValue = m.Skills[other].Value;

            /* 40% chance at 80, 80
            * 50% chance at 100, 100
            * 60% chance at 120, 120
            */

            double chance = (wresValue + scndValue) / 400.0;

            return (chance >= Utility.RandomDouble());
        }

        private static bool HasFreeHands(Mobile m)
        {
            Item item = m.FindItemOnLayer(Layer.OneHanded);

            if (item != null && !(item is Spellbook))
                return false;

            return m.FindItemOnLayer(Layer.TwoHanded) == null;
        }

        private class MoveDelayTimer : Timer
        {
            private readonly Mobile m_Mobile;

            public MoveDelayTimer(Mobile m)
                : base(TimeSpan.FromSeconds(10.0))
            {
                m_Mobile = m;

                Priority = TimerPriority.TwoFiftyMS;

                m_Mobile.BeginAction(typeof(Fists));
            }

            protected override void OnTick()
            {
                m_Mobile.EndAction(typeof(Fists));
            }
        }

        private static void StartMoveDelay(Mobile m)
        {
            new MoveDelayTimer(m).Start();
        }
    }
}
