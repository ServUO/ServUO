using System;
using Server.Items;
using System.Collections;

namespace Server.Mobiles
{
    [CorpseName("a seaweed corpse")]
    public class DiabolicalSeaweed : BaseCreature
    {
        private PullTimer m_Timer;
        [Constructable]

        public DiabolicalSeaweed()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.0, 0.0)
        {
            Name = "Diabolical Seaweed";
            Body = 129;
            Hue = 1914;

            SetStr(452, 485);
            SetDex(401, 420);
            SetInt(126, 140);

            SetHits(501, 532);

            SetDamage(10, 23);

            SetDamageType(ResistanceType.Physical, 60);
            SetDamageType(ResistanceType.Poison, 40);

            SetResistance(ResistanceType.Physical, 35, 40);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 10, 20);
            SetResistance(ResistanceType.Poison, 100);
            SetResistance(ResistanceType.Energy, 10, 20);

            SetSkill(SkillName.MagicResist, 50.1, 54.7);
            SetSkill(SkillName.Tactics, 100.3, 114.8);
            SetSkill(SkillName.Wrestling, 45.1, 59.5);

            Fame = 3000;
            Karma = -3000;
            CantWalk = true;

            VirtualArmor = 60;

            m_Timer = new PullTimer(this);
            m_Timer.Start();

            switch (Utility.Random(8))
            {
                case 0: PackItem(new BlueDiamond()); break;
                case 1: PackItem(new FireRuby()); break;
                case 2: PackItem(new BrilliantAmber()); break;
                case 3: PackItem(new PerfectEmerald()); break;
                case 4: PackItem(new DarkSapphire()); break;
                case 5: PackItem(new Turquoise()); break;
                case 6: PackItem(new EcruCitrine()); break;
                case 7: PackItem(new WhitePearl()); break;
            }
            PackItem(new ParasiticPlant());
            PackItem(new LuminescentFungi());
        }

        public DiabolicalSeaweed(Serial serial)
            : base(serial)
        {
        }

        public override void OnAfterDelete()
        {
            if (m_Timer != null)
                m_Timer.Stop();

            m_Timer = null;

            base.OnAfterDelete();
        }

        private class PullTimer : Timer
        {
            private static readonly ArrayList m_ToDrain = new ArrayList();
            private readonly DiabolicalSeaweed m_Owner;

            public PullTimer(DiabolicalSeaweed owner)
                : base(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))
            {
                m_Owner = owner;
                Priority = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                if (m_Owner.Deleted)
                {
                    Stop();
                    return;
                }

                IPooledEnumerable eable = m_Owner.GetMobilesInRange(9);
                foreach (Mobile m in eable)
                {
                    if (m == null || m is DiabolicalSeaweed || !m_Owner.CanBeHarmful(m) || m.AccessLevel == AccessLevel.Player)
                        continue;

                    int offsetX = Math.Abs(m.Location.X - m_Owner.Location.X);
                    int offsetY = Math.Abs(m.Location.Y - m_Owner.Location.Y);

                    if (offsetX < 2 && offsetY < 2)
                        continue;

                    if (m is BaseCreature)
                    {
                        BaseCreature bc = m as BaseCreature;

                        if (bc.Controlled || bc.Summoned)
                            m_ToDrain.Add(m);
                    }
                    else if (m.Player)
                    {
                        m_ToDrain.Add(m);
                    }
                }

                eable.Free();

                foreach (Mobile m in m_ToDrain)
                {
                    m_Owner.DoHarmful(m);

                    //m.FixedParticles( 0x376A, 10, 15, 5052, EffectLayer.Waist );
                    //m.PlaySound(0x1F1);
                    int drain = Utility.RandomMinMax(1, 10);
                    int ownerlocX = m_Owner.Location.X + Utility.RandomMinMax(-1, 1);
                    int ownerlocY = m_Owner.Location.Y + Utility.RandomMinMax(-1, 1);
                    int ownerlocZ = m_Owner.Location.Z;
                    m.MoveToWorld(new Point3D(ownerlocX, ownerlocY, ownerlocZ), m_Owner.Map);
                    m.Damage(drain, m_Owner);
                }

                m_ToDrain.Clear();
            }
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
            for (int i = Utility.Random(5, 6); i > 1; i--)
            {
                Item ReagentLoot = Loot.RandomReagent();
                ReagentLoot.Amount = Utility.Random(4, 5);
                c.DropItem(ReagentLoot);
            }

        }

        public override bool CanRummageCorpses { get { return true; } }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Meager);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_Timer = new PullTimer(this);
            m_Timer.Start();

        }
    }
}
