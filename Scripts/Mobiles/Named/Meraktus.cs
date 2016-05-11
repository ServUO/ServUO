using System;
using System.Collections;
using Server.Engines.CannedEvil;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("the remains of Meraktus")]
    public class Meraktus : BaseChampion
    {
        public override ChampionSkullType SkullType
        {
            get
            {
                return ChampionSkullType.Pain;
            }
        }

        public override Type[] UniqueList
        {
            get
            {
                return new Type[] { typeof(Subdue) };
            }
        }
        public override Type[] SharedList
        {
            get
            {
                return new Type[]
                {
                    typeof(RoyalGuardSurvivalKnife),
                    typeof(TheMostKnowledgePerson),
                    typeof(OblivionsNeedle)
                };
            }
        }
        public override Type[] DecorativeList
        {
            get
            {
                return new Type[]
                {
                    typeof(ArtifactLargeVase),
                    typeof(ArtifactVase),
                    typeof(MinotaurStatueDeed)
                };
            }
        }

        public override MonsterStatuetteType[] StatueTypes
        {
            get
            {
                return new MonsterStatuetteType[]
                {
                    MonsterStatuetteType.Minotaur
                };
            }
        }

        public override WeaponAbility GetWeaponAbility()
        {
            return WeaponAbility.Dismount;
        }

        [Constructable]
        public Meraktus()
            : base(AIType.AI_Melee)
        {
            this.Name = "Meraktus";
            this.Title = "the Tormented";
            this.Body = 263;
            this.BaseSoundID = 680;
            this.Hue = 0x835;

            this.SetStr(1419, 1438);
            this.SetDex(309, 413);
            this.SetInt(129, 131);

            this.SetHits(4100, 4200);

            this.SetDamage(16, 30);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 65, 90);
            this.SetResistance(ResistanceType.Fire, 65, 70);
            this.SetResistance(ResistanceType.Cold, 50, 60);
            this.SetResistance(ResistanceType.Poison, 40, 60);
            this.SetResistance(ResistanceType.Energy, 50, 55);

            //SetSkill( SkillName.Meditation, Unknown );
            //SetSkill( SkillName.EvalInt, Unknown );
            //SetSkill( SkillName.Magery, Unknown );
            //SetSkill( SkillName.Poisoning, Unknown );
            this.SetSkill(SkillName.Anatomy, 0);
            this.SetSkill(SkillName.MagicResist, 107.0, 111.3);
            this.SetSkill(SkillName.Tactics, 107.0, 117.0);
            this.SetSkill(SkillName.Wrestling, 100.0, 105.0);

            this.Fame = 70000;
            this.Karma = -70000;

            this.VirtualArmor = 28; // Don't know what it should be

            for (int i = 0; i < Utility.RandomMinMax(0, 1); i++)
            {
                this.PackItem(Loot.RandomScroll(0, Loot.ArcanistScrollTypes.Length, SpellbookType.Arcanist));
            }

            this.NoKillAwards = true;

            if (Core.ML)
            {
                this.PackResources(8);
                this.PackTalismans(5);
            }

            Timer.DelayCall(TimeSpan.FromSeconds(1), new TimerCallback(SpawnTormented));
        }

        public virtual void PackResources(int amount)
        {
            for (int i = 0; i < amount; i++)
                switch (Utility.Random(6))
                {
                    case 0:
                        this.PackItem(new Blight());
                        break;
                    case 1:
                        this.PackItem(new Scourge());
                        break;
                    case 2:
                        this.PackItem(new Taint());
                        break;
                    case 3:
                        this.PackItem(new Putrefication());
                        break;
                    case 4:
                        this.PackItem(new Corruption());
                        break;
                    case 5:
                        this.PackItem(new Muculent());
                        break;
                }
        }

        public virtual void PackTalismans(int amount)
        {
            int count = Utility.Random(amount);

            for (int i = 0; i < count; i++)
                this.PackItem(new RandomTalisman());
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Core.ML)
            {
                c.DropItem(new MalletAndChisel());

                switch (Utility.Random(3))
                {
                    case 0:
                        c.DropItem(new MinotaurHedge());
                        break;
                    case 1:
                        c.DropItem(new BonePile());
                        break;
                    case 2:
                        c.DropItem(new LightYarn());
                        break;
                }

                if (Utility.RandomBool())
                    c.DropItem(new TormentedChains());

                if (Utility.RandomDouble() < 0.025)
                    c.DropItem(new CrimsonCincture());
            }
        }

        public override void GenerateLoot()
        {
            if (Core.ML)
            {
                this.AddLoot(LootPack.AosSuperBoss, 5);  // Need to verify
            }
        }

        public override int GetAngerSound()
        {
            return 0x597;
        }

        public override int GetIdleSound()
        {
            return 0x596;
        }

        public override int GetAttackSound()
        {
            return 0x599;
        }

        public override int GetHurtSound()
        {
            return 0x59a;
        }

        public override int GetDeathSound()
        {
            return 0x59c;
        }

        public override int Meat
        {
            get
            {
                return 2;
            }
        }
        public override int Hides
        {
            get
            {
                return 10;
            }
        }
        public override HideType HideType
        {
            get
            {
                return HideType.Regular;
            }
        }
        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Regular;
            }
        }
        public override int TreasureMapLevel
        {
            get
            {
                return 3;
            }
        }
        public override bool BardImmune
        {
            get
            {
                return true;
            }
        }
        public override bool Unprovokable
        {
            get
            {
                return true;
            }
        }
        public override bool Uncalmable
        {
            get
            {
                return true;
            }
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);
            if (0.2 >= Utility.RandomDouble())
                this.Earthquake();
        }

        public void Earthquake()
        {
            Map map = this.Map;
            if (map == null)
                return;
            ArrayList targets = new ArrayList();
            foreach (Mobile m in this.GetMobilesInRange(8))
            {
                if (m == this || !this.CanBeHarmful(m))
                    continue;
                if (m is BaseCreature && (((BaseCreature)m).Controlled || ((BaseCreature)m).Summoned || ((BaseCreature)m).Team != this.Team))
                    targets.Add(m);
                else if (m.Player)
                    targets.Add(m);
            }
            this.PlaySound(0x2F3);
            for (int i = 0; i < targets.Count; ++i)
            {
                Mobile m = (Mobile)targets[i];
                if (m != null && !m.Deleted && m is PlayerMobile)
                {
                    PlayerMobile pm = m as PlayerMobile;
                    if (pm != null && pm.Mounted)
                    {
                        pm.Mount.Rider = null;
                    }
                }
                double damage = m.Hits * 0.6;//was .6
                if (damage < 10.0)
                    damage = 10.0;
                else if (damage > 75.0)
                    damage = 75.0;
                this.DoHarmful(m);
                AOS.Damage(m, this, (int)damage, 100, 0, 0, 0, 0);
                if (m.Alive && m.Body.IsHuman && !m.Mounted)
                    m.Animate(20, 7, 1, true, false, 0); // take hit
            }
        }

        public Meraktus(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        #region SpawnHelpers
        public void SpawnTormented()
        {
            BaseCreature spawna = new TormentedMinotaur();
            spawna.MoveToWorld(this.Location, this.Map);

            BaseCreature spawnb = new TormentedMinotaur();
            spawnb.MoveToWorld(this.Location, this.Map);

            BaseCreature spawnc = new TormentedMinotaur();
            spawnc.MoveToWorld(this.Location, this.Map);

            BaseCreature spawnd = new TormentedMinotaur();
            spawnd.MoveToWorld(this.Location, this.Map);
        }
        #endregion
    }
}