using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a stygian dragon corpse")]
    public class StygianDragon : BaseSABosses
    {
        private DateTime m_Delay;

        [Constructable]
        public StygianDragon()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.3, 0.5)
        {
            Name = "Stygian Dragon";
            Body = 826;
            BaseSoundID = 362;

            SetStr(702);
            SetDex(250);
            SetInt(180);

            SetHits(30000);
            SetStam(431);
            SetMana(180);

            SetDamage(33, 55);

            SetDamageType(ResistanceType.Physical, 25);
            SetDamageType(ResistanceType.Fire, 50);
            SetDamageType(ResistanceType.Energy, 25);

            SetResistance(ResistanceType.Physical, 80, 90);
            SetResistance(ResistanceType.Fire, 80, 90);
            SetResistance(ResistanceType.Cold, 60, 70);
            SetResistance(ResistanceType.Poison, 80, 90);
            SetResistance(ResistanceType.Energy, 80, 90);

            SetSkill(SkillName.Anatomy, 100.0);
            SetSkill(SkillName.MagicResist, 150.0, 155.0);
            SetSkill(SkillName.Tactics, 120.7, 125.0);
            SetSkill(SkillName.Wrestling, 115.0, 117.7);

            Fame = 15000;
            Karma = -15000;

            VirtualArmor = 60;

            Tamable = false;

            SetWeaponAbility(WeaponAbility.Bladeweave);
            SetWeaponAbility(WeaponAbility.TalonStrike);
        }

        public StygianDragon(Serial serial)
            : base(serial)
        {
        }        

        public override Type[] UniqueSAList
        {
            get
            {
                return new Type[]
                {
                    typeof(BurningAmber), typeof(DraconisWrath), typeof(DragonHideShield), typeof(FallenMysticsSpellbook),
                    typeof(LifeSyphon), typeof(GargishSignOfOrder), typeof(HumanSignOfOrder), typeof(VampiricEssence)
                };
            }
        }
        public override Type[] SharedSAList
        {
            get
            {
                return new Type[]
                {
                    typeof(AxesOfFury), typeof(SummonersKilt), typeof(GiantSteps), typeof(StoneDragonsTooth),
                    typeof(TokenOfHolyFavor)
                };
            }
        }

        public override bool CausesTrueFear { get { return true; } }
        public override bool AlwaysMurderer { get { return true; } }
        public override bool Unprovokable { get { return false; } }
        public override bool BardImmune { get { return false; } }
        public override bool HasBreath { get { return true; } } // fire breath enabled
        public override bool AutoDispel { get { return !Controlled; } }
        public override int Meat { get { return 19; } }
        public override int Hides { get { return 30; } }
        public override HideType HideType { get { return HideType.Barbed; } }
        public override int Scales { get { return 7; } }
        public override ScaleType ScaleType { get { return (Body == 12 ? ScaleType.Yellow : ScaleType.Red); } }
        public override int DragonBlood { get { return 48; } }
        public override bool CanFlee { get { return false; } }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.AosSuperBoss, 4);
            AddLoot(LootPack.Gems, 8);
        }

        public override void OnActionCombat()
        {
            if (DateTime.UtcNow > m_Delay)
            {
                switch (Utility.Random(2))
                {
                    case 0:
                        Ability.FlameCross(this);
                        m_Delay = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(25, 35));
                        break;
                    case 1:
                        Ability.CrimsonMeteor(this, 35);
                        m_Delay = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(20, 45));
                        break;
                }
            }

            base.OnActionCombat();
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            c.DropItem(new StygianDragonHead());
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
