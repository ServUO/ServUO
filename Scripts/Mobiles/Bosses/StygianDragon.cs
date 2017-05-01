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
            this.Name = "Stygian Dragon";
            this.Body = 826;
            this.BaseSoundID = 362;

            this.SetStr(702);
            this.SetDex(250);
            this.SetInt(180);

            this.SetHits(30000);
            this.SetStam(431);
            this.SetMana(180);

            this.SetDamage(33, 55);

            this.SetDamageType(ResistanceType.Physical, 25);
            this.SetDamageType(ResistanceType.Fire, 50);
            this.SetDamageType(ResistanceType.Energy, 25);

            this.SetResistance(ResistanceType.Physical, 80, 90);
            this.SetResistance(ResistanceType.Fire, 80, 90);
            this.SetResistance(ResistanceType.Cold, 60, 70);
            this.SetResistance(ResistanceType.Poison, 80, 90);
            this.SetResistance(ResistanceType.Energy, 80, 90);

            this.SetSkill(SkillName.Anatomy, 100.0);
            this.SetSkill(SkillName.MagicResist, 150.0, 155.0);
            this.SetSkill(SkillName.Tactics, 120.7, 125.0);
            this.SetSkill(SkillName.Wrestling, 115.0, 117.7);

            this.Fame = 15000;
            this.Karma = -15000;

            this.VirtualArmor = 60;

            this.Tamable = false;
        }

        public StygianDragon(Serial serial)
            : base(serial)
        {
        }

        public override bool AlwaysMurderer
        {
            get
            {
                return true;
            }
        }
        public override Type[] UniqueSAList
        {
            get
            {
                return new Type[] { typeof(BurningAmber), typeof(DraconisWrath), typeof(DragonHideShield), typeof(FallenMysticsSpellbook), typeof(LifeSyphon), typeof(GargishSignOfOrder), typeof(HumanSignOfOrder), typeof(VampiricEssence) };
            }
        }
        public override Type[] SharedSAList
        {
            get
            {
                return new Type[] { typeof(AxesOfFury), typeof(SummonersKilt), typeof(GiantSteps), typeof(StoneDragonsTooth), typeof(TokenOfHolyFavor) };
            }
        }
        public override bool Unprovokable
        {
            get
            {
                return false;
            }
        }
        public override bool BardImmune
        {
            get
            {
                return false;
            }
        }
        // public override bool GivesSAArtifact { get { return true; } }
        public override bool HasBreath
        {
            get
            {
                return true;
            }
        }// fire breath enabled
        public override bool AutoDispel
        {
            get
            {
                return !this.Controlled;
            }
        }
        public override int Meat
        {
            get
            {
                return 19;
            }
        }
        public override int Hides
        {
            get
            {
                return 30;
            }
        }
        public override HideType HideType
        {
            get
            {
                return HideType.Barbed;
            }
        }
        public override int Scales
        {
            get
            {
                return 7;
            }
        }
        public override ScaleType ScaleType
        {
            get
            {
                return (this.Body == 12 ? ScaleType.Yellow : ScaleType.Red);
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.AosSuperBoss, 4);
            this.AddLoot(LootPack.Gems, 8);
        }

        public override WeaponAbility GetWeaponAbility()
        {
            if (50.0 >= Utility.RandomDouble())
                return WeaponAbility.Bladeweave;
            else
                return WeaponAbility.TalonStrike;
        }

        public override void OnActionCombat()
        {
            if (DateTime.UtcNow > this.m_Delay)
            {
                switch (Utility.Random(2))
                {
                    case 0:
                        Ability.FlameCross(this);
                        this.m_Delay = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(25, 35));
                        break;
                    case 1:
                        Ability.CrimsonMeteor(this, 35);
                        this.m_Delay = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(20, 45));
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
