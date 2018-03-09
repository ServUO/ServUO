using Server.Items;
using System;

namespace Server.Mobiles
{
    [CorpseName("a paralithode corpse")]
    public class Paralithode : BaseCreature
    {
        private HideTimer m_Timer;

        [Constructable]
        public Paralithode()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Paralithode";
            Body = 729;
            Hue = 1922;

            SetStr(642, 729);
            SetDex(87, 103);
            SetInt(25, 30);

            SetHits(1800, 2000);
            SetMana(315, 343);

            SetDamage(20, 24);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 65, 75);
            SetResistance(ResistanceType.Fire, 50, 60);
            SetResistance(ResistanceType.Cold, 60, 70);
            SetResistance(ResistanceType.Poison, 50, 60);
            SetResistance(ResistanceType.Energy, 50, 60);

            SetSkill(SkillName.MagicResist, 68.7, 75.0);
            SetSkill(SkillName.Anatomy, 98.0, 100.6);
            SetSkill(SkillName.Tactics, 95.8, 100.9);
            SetSkill(SkillName.Wrestling, 100.2, 109.0);
            SetSkill(SkillName.Parry, 100.0, 110.0);
            SetSkill(SkillName.Ninjitsu, 100.2, 109.0);
            SetSkill(SkillName.DetectHidden, 50.0);

            Fame = 2500;
            Karma = -2500;

            Tamable = true;
            ControlSlots = 4;
            MinTameSkill = 47.1;            

            PackItem(new FertileDirt(2));
            m_Timer = new HideTimer(this);
            m_Timer.Start();

            SetWeaponAbility(WeaponAbility.DualWield);
            SetWeaponAbility(WeaponAbility.ForceOfNature);
        }

        public Paralithode(Serial serial)
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

        public override void OnAfterTame(Mobile tamer)
        {
            if (m_Timer != null)
                m_Timer.Stop();

            CantWalk = false;
            Hidden = false;
        }

        private class HideTimer : Timer
        {
            private readonly Paralithode m_Creature;

            public HideTimer(Paralithode owner)
                : base(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0))
            {
                m_Creature = owner;
                Priority = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                if (!m_Creature.Controlled)
                {
                    if (m_Creature.Warmode == false && m_Creature.Hidden == false)
                        m_Creature.PerformHide();
                    else if (m_Creature.Warmode == true)
                    {
                        m_Creature.CantWalk = false;
                        return;
                    }

                    IPooledEnumerable eable = m_Creature.GetMobilesInRange(5);

                    foreach (Mobile m in eable)
                    {
                        if (m == m_Creature || (m is Paralithode) || !m_Creature.CanBeHarmful(m))
                            continue;

                        m_Creature.CantWalk = false;
                    }

                    eable.Free();
                }
                else
                {
                    Stop();
                    m_Creature.CantWalk = false;
                    m_Creature.Hidden = false;
                }
            }
        }
        
        public void PerformHide()
        {
            if (Deleted)
                return;

            Hidden = true;
            CantWalk = true;
        }

        public override bool IsScaredOfScaryThings { get { return false; } }
        public override bool IsBondable { get { return false; } }
        public override FoodType FavoriteFood { get { return FoodType.FruitsAndVegies; } }
        public override bool BleedImmune { get { return true; } }
        public override bool DeleteOnRelease { get { return true; } }
        public override bool BardImmune { get { return !Core.AOS || Controlled; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }
        public override bool CanAngerOnTame { get { return true; } }
        public override bool StatLossAfterTame { get { return true; } }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Gems, 2);
        }

        public override int GetAngerSound()
        {
            return 541;
        }

        public override int GetIdleSound()
        {
            if (!Controlled)
                return 542;

            return base.GetIdleSound();
        }

        public override int GetDeathSound()
        {
            if (!Controlled)
                return 545;

            return base.GetDeathSound();
        }

        public override int GetAttackSound()
        {
            return 562;
        }

        public override int GetHurtSound()
        {
            if (Controlled)
                return 320;

            return base.GetHurtSound();
        }
        public override int Meat { get { return 9; } }
        public override int Hides { get { return 20; } }
        public override HideType HideType { get { return HideType.Horned; } }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (!Controlled)
            {
                m_Timer = new HideTimer(this);
                m_Timer.Start();
            }
        }
    }
}
