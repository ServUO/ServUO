using Server.Items;
using System;

namespace Server.Mobiles
{
    [CorpseName("a paralithode corpse")]
    public class Paralithode : BaseCreature
    {
        private DateTime _HideCheck;

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

            SetWeaponAbility(WeaponAbility.DualWield);
            SetWeaponAbility(WeaponAbility.ForceOfNature);
        }

        public Paralithode(Serial serial)
            : base(serial)
        {
        }

        public override void OnAfterTame(Mobile tamer)
        {
            CantWalk = false;
            Hidden = false;

            base.OnAfterTame(tamer);
        }

        public override void OnThink()
        {
            base.OnThink();

            if (_HideCheck < DateTime.UtcNow)
            {
                CheckHide();

                _HideCheck = DateTime.UtcNow + TimeSpan.FromSeconds(1);
            }
        }

        private void CheckHide()
        {
            if (!Controlled)
            {
                if (!Warmode && !Hidden)
                {
                    PerformHide();
                }
                else if (Warmode)
                {
                    CantWalk = false;
                    return;
                }

                IPooledEnumerable eable = GetMobilesInRange(5);

                foreach (Mobile m in eable)
                {
                    if (m == this || (m is Paralithode) || !CanBeHarmful(m))
                        continue;

                    CantWalk = false;
                }

                eable.Free();
            }
            else
            {
                CantWalk = false;
                Hidden = false;
            }
        }

        public void PerformHide()
        {
            if (Deleted)
                return;

            Hidden = true;
            CantWalk = true;
        }

        public override bool IsScaredOfScaryThings => false;
        public override bool IsBondable => false;
        public override FoodType FavoriteFood => FoodType.FruitsAndVegies;
        public override bool BleedImmune => true;
        public override bool DeleteOnRelease => true;
        public override bool BardImmune => Controlled;
        public override Poison PoisonImmune => Poison.Lethal;
        public override bool CanAngerOnTame => true;
        public override bool StatLossAfterTame => true;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Gems, 2);
            AddLoot(LootPack.LootItem<FertileDirt>(2, true));
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
        public override int Meat => 9;
        public override int Hides => 20;
        public override HideType HideType => HideType.Horned;

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
    }
}
