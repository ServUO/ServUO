using Server.Items;
using System;

namespace Server.Mobiles
{
    [CorpseName("a sand vortex corpse")]
    public class SandVortex : BaseCreature
    {
        private DateTime m_NextAttack;
        [Constructable]
        public SandVortex()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a sand vortex";
            Body = 790;
            BaseSoundID = 263;

            SetStr(96, 120);
            SetDex(171, 195);
            SetInt(76, 100);

            SetHits(51, 62);

            SetDamage(3, 16);

            SetDamageType(ResistanceType.Physical, 90);
            SetDamageType(ResistanceType.Fire, 10);

            SetResistance(ResistanceType.Physical, 80, 90);
            SetResistance(ResistanceType.Fire, 60, 70);
            SetResistance(ResistanceType.Cold, 60, 70);
            SetResistance(ResistanceType.Poison, 60, 70);
            SetResistance(ResistanceType.Energy, 60, 70);

            SetSkill(SkillName.MagicResist, 150.0);
            SetSkill(SkillName.Tactics, 70.0);
            SetSkill(SkillName.Wrestling, 80.0);

            Fame = 4500;
            Karma = -4500;
        }

        public SandVortex(Serial serial)
            : base(serial)
        {
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Meager, 2);
            AddLoot(LootPack.LootItem<Bone>());
        }

        public override void OnActionCombat()
        {
            Mobile combatant = Combatant as Mobile;

            if (combatant == null || combatant.Deleted || combatant.Map != Map || !InRange(combatant, 12) || !CanBeHarmful(combatant) || !InLOS(combatant))
                return;

            if (DateTime.UtcNow >= m_NextAttack)
            {
                SandAttack(combatant);
                m_NextAttack = DateTime.UtcNow + TimeSpan.FromSeconds(10.0 + (10.0 * Utility.RandomDouble()));
            }
        }

        public void SandAttack(Mobile m)
        {
            DoHarmful(m);

            m.FixedParticles(0x36B0, 10, 25, 9540, 2413, 0, EffectLayer.Waist);

            new InternalTimer(m, this).Start();
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
        }

        private class InternalTimer : Timer
        {
            private readonly Mobile m_Mobile;
            private readonly Mobile m_From;
            public InternalTimer(Mobile m, Mobile from)
                : base(TimeSpan.FromSeconds(1.0))
            {
                m_Mobile = m;
                m_From = from;
                Priority = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                m_Mobile.PlaySound(0x4CF);
                AOS.Damage(m_Mobile, m_From, Utility.RandomMinMax(1, 40), 90, 10, 0, 0, 0);
            }
        }
    }
}
