using Server.Items;
using Server.Spells;
using System;
using System.Linq;

namespace Server.Mobiles
{
    [CorpseName("a monstrous interred grizzle corpse")]
    public class MonstrousInterredGrizzle : BasePeerless
    {
        private static readonly int[] m_Tiles =
        {
            -2, 0,
            2, 0,
            2, -2,
            2, 2,
            -2, -2,
            -2, 2,
            0, 2,
            1, 0,
            0, -2
        };

        private readonly DateTime m_NextDrop = DateTime.UtcNow;

        [Constructable]
        public MonstrousInterredGrizzle()
            : base(AIType.AI_Spellweaving, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a monstrous interred grizzle";
            Body = 0x103;
            BaseSoundID = 589;

            SetStr(1198, 1207);
            SetDex(127, 135);
            SetInt(595, 646);

            SetHits(50000);

            SetDamage(27, 31);

            SetDamageType(ResistanceType.Physical, 60);
            SetDamageType(ResistanceType.Fire, 20);
            SetDamageType(ResistanceType.Energy, 20);

            SetResistance(ResistanceType.Physical, 48, 52);
            SetResistance(ResistanceType.Fire, 77, 82);
            SetResistance(ResistanceType.Cold, 56, 61);
            SetResistance(ResistanceType.Poison, 32, 40);
            SetResistance(ResistanceType.Energy, 69, 71);

            SetSkill(SkillName.Wrestling, 112.6, 116.9);
            SetSkill(SkillName.Tactics, 118.5, 119.2);
            SetSkill(SkillName.MagicResist, 120);
            SetSkill(SkillName.Anatomy, 111.0, 111.7);
            SetSkill(SkillName.Magery, 100.0);
            SetSkill(SkillName.EvalInt, 100);
            SetSkill(SkillName.Meditation, 100);
            SetSkill(SkillName.Spellweaving, 100.0);

            Fame = 24000;
            Karma = -24000;

            SetSpecialAbility(SpecialAbility.HowlOfCacophony);
        }

        public MonstrousInterredGrizzle(Serial serial)
            : base(serial)
        {
        }

        public override bool GivesMLMinorArtifact => true;
        public override int TreasureMapLevel => 5;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.SuperBoss, 8);
            AddLoot(LootPack.ArcanistScrolls, Utility.RandomMinMax(1, 6));
            AddLoot(LootPack.PeerlessResource, 8);
            AddLoot(LootPack.Talisman, 5);
            AddLoot(LootPack.LootItem<GrizzledBones>());

            AddLoot(LootPack.RandomLootItem(new[] { typeof(TombstoneOfTheDamned), typeof(GlobOfMonstreousInterredGrizzle), typeof(MonsterousInterredGrizzleMaggots), typeof(GrizzledSkullCollection) }));

            AddLoot(LootPack.LootItem<ParrotItem>(60.0));
            AddLoot(LootPack.LootItem<GrizzledMareStatuette>(5.0));

            AddLoot(LootPack.RandomLootItem(new[] { typeof(GrizzleGauntlets), typeof(GrizzleGreaves), typeof(GrizzleHelm), typeof(GrizzleTunic), typeof(GrizzleVambraces) }, 5.0, 1));
        }

        public override int GetDeathSound()
        {
            return 0x57F;
        }

        public override int GetAttackSound()
        {
            return 0x580;
        }

        public override int GetIdleSound()
        {
            return 0x581;
        }

        public override int GetAngerSound()
        {
            return 0x582;
        }

        public override int GetHurtSound()
        {
            return 0x583;
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

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            if (Utility.RandomDouble() < 0.06)
                SpillAcid(null, Utility.RandomMinMax(1, 3));

            base.OnDamage(amount, from, willKill);
        }

        public override Item NewHarmfulItem()
        {
            return new InfernalOoze(this, Utility.RandomBool());
        }
    }

    public class InfernalOoze : Item
    {
        private bool m_Corrosive;
        private readonly int m_Damage;
        private readonly Mobile m_Owner;
        private Timer m_Timer;

        private readonly DateTime m_StartTime;

        public InfernalOoze(Mobile owner)
            : this(owner, false)
        {
        }

        public InfernalOoze(Mobile owner, bool corrosive, int damage = 40)
            : base(0x122A)
        {
            Movable = false;
            m_Owner = owner;
            Hue = 0x95;

            m_Damage = damage;

            m_Corrosive = corrosive;
            m_StartTime = DateTime.UtcNow;

            m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), OnTick);
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Corrosive
        {
            get { return m_Corrosive; }
            set { m_Corrosive = value; }
        }

        private void OnTick()
        {
            if (ItemID == 0x122A && m_StartTime + TimeSpan.FromSeconds(30) < DateTime.UtcNow)
            {
                ItemID++;
            }
            else if (m_StartTime + TimeSpan.FromSeconds(35) < DateTime.UtcNow)
            {
                Delete();
                return;
            }

            if (m_Owner == null)
                return;

            if (!Deleted && Map != Map.Internal && Map != null)
            {
                foreach (Mobile m in SpellHelper.AcquireIndirectTargets(m_Owner, Location, Map, 0).OfType<Mobile>())
                {
                    OnMoveOver(m);
                }
            }
        }

        public override bool OnMoveOver(Mobile m)
        {
            if (Map == null)
                return base.OnMoveOver(m);

            if ((m is BaseCreature && ((BaseCreature)m).GetMaster() is PlayerMobile) || m.Player)
            {
                Damage(m);
            }

            return base.OnMoveOver(m);
        }

        public virtual void Damage(Mobile m)
        {
            if (m_Corrosive)
            {
                for (int i = 0; i < m.Items.Count; i++)
                {
                    IDurability item = m.Items[i] as IDurability;

                    if (item != null && Utility.RandomDouble() < 0.25)
                    {
                        if (item.HitPoints > 10)
                            item.HitPoints -= 10;
                        else
                            item.HitPoints -= 1;
                    }
                }
            }
            else
            {
                int dmg = m_Damage;

                if (m is PlayerMobile)
                {
                    PlayerMobile pm = m as PlayerMobile;
                    dmg = (int)BalmOfProtection.HandleDamage(pm, dmg);
                    AOS.Damage(m, m_Owner, dmg, 0, 0, 0, 100, 0);
                }
                else
                {
                    AOS.Damage(m, m_Owner, dmg, 0, 0, 0, 100, 0);
                }
            }
        }

        public override void Delete()
        {
            base.Delete();

            if (m_Timer != null)
            {
                m_Timer.Stop();
                m_Timer = null;
            }
        }

        public InfernalOoze(Serial serial)
            : base(serial)
        {
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

            Delete();
        }
    }
}
