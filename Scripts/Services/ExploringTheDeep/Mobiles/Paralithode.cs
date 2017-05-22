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
            this.Name = "Paralithode";
            this.Body = 729;
            this.Hue = 1922;

            this.SetStr(642, 729);
            this.SetDex(87, 103);
            this.SetInt(25, 30);

            this.SetHits(1800, 2000);
            this.SetMana(315, 343);

            this.SetDamage(20, 24);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 65, 75);
            this.SetResistance(ResistanceType.Fire, 50, 60);
            this.SetResistance(ResistanceType.Cold, 60, 70);
            this.SetResistance(ResistanceType.Poison, 50, 60);
            this.SetResistance(ResistanceType.Energy, 50, 60);

            this.SetSkill(SkillName.MagicResist, 68.7, 75.0);
            this.SetSkill(SkillName.Anatomy, 98.0, 100.6);
            this.SetSkill(SkillName.Tactics, 95.8, 100.9);
            this.SetSkill(SkillName.Wrestling, 100.2, 109.0);
            this.SetSkill(SkillName.Parry, 100.0, 110.0);
            this.SetSkill(SkillName.Ninjitsu, 100.2, 109.0);
            this.SetSkill(SkillName.DetectHidden, 50.0);

            this.Fame = 2500;
            this.Karma = -2500;

            this.Tamable = true;
            this.ControlSlots = 2;
            this.MinTameSkill = 47.1;            

            if (!this.Controlled)
            {
                this.PackItem(new FertileDirt(2));
                this.m_Timer = new HideTimer(this);
                this.m_Timer.Start();
            }
        }

        public Paralithode(Serial serial)
            : base(serial)
        {
        }
        
        public override void OnAfterDelete()
        {
            if (this.m_Timer != null)
                this.m_Timer.Stop();

            this.m_Timer = null;

            base.OnAfterDelete();
        }

        public override void OnAfterTame(Mobile tamer)
        {
            if (this.m_Timer != null)
                this.m_Timer.Stop();

            this.CantWalk = false;
            this.Hidden = false;
        }

        private class HideTimer : Timer
        {
            private readonly Paralithode m_Creature;

            public HideTimer(Paralithode owner)
                : base(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0))
            {
                this.m_Creature = owner;
                this.Priority = TimerPriority.TwoFiftyMS;
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

                    foreach (Mobile m in this.m_Creature.GetMobilesInRange(5))
                    {
                        if (m == this.m_Creature || (m is Paralithode) || !this.m_Creature.CanBeHarmful(m))
                            continue;

                        m_Creature.CantWalk = false;
                    }
                }
                else
                {
                    this.Stop();
                    m_Creature.CantWalk = false;
                    m_Creature.Hidden = false;
                }
            }
        }
        
        public void PerformHide()
        {
            if (Deleted)
                return;

            this.Hidden = true;
            this.CantWalk = true;
        }

        public override bool IsScaredOfScaryThings { get { return false; } }
        public override bool IsBondable { get { return false; } }
        public override FoodType FavoriteFood { get { return FoodType.FruitsAndVegies; } }
        public override bool BleedImmune { get { return true; } }
        public override bool DeleteOnRelease { get { return true; } }
        public override bool BardImmune { get { return !Core.AOS || this.Controlled; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }
        public override bool CanAngerOnTame { get { return true; } }
        public override bool StatLossAfterTame { get { return true; } }

        public override WeaponAbility GetWeaponAbility()
        {
            switch (Utility.Random(2))
            {
                default:
                case 1:
                    return WeaponAbility.DualWield;
                case 2:
                    return WeaponAbility.ForceOfNature;
            }
        }

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
            if (!this.Controlled)
                return 542;

            return base.GetIdleSound();
        }

        public override int GetDeathSound()
        {
            if (!this.Controlled)
                return 545;

            return base.GetDeathSound();
        }

        public override int GetAttackSound()
        {
            return 562;
        }

        public override int GetHurtSound()
        {
            if (this.Controlled)
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

            if (!this.Controlled)
            {
                this.m_Timer = new HideTimer(this);
                this.m_Timer.Start();
            }
        }
    }
}
