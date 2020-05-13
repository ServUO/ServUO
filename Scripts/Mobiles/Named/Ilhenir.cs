using Server.Engines.CannedEvil;
using Server.Items;
using Server.Network;
using System;
using System.Collections.Generic;

namespace Server.Mobiles
{
    [CorpseName("a corpse of Ilhenir")]
    public class Ilhenir : BaseChampion
    {
        private readonly DateTime m_NextDrop = DateTime.UtcNow;

        [Constructable]
        public Ilhenir()
            : base(AIType.AI_Mage)
        {
            Name = "Ilhenir";
            Title = "the Stained";
            Body = 0x103;
            Hue = 1164;

            BaseSoundID = 589;

            SetStr(1105, 1350);
            SetDex(82, 160);
            SetInt(505, 750);

            SetHits(9000);

            SetDamage(21, 28);

            SetDamageType(ResistanceType.Physical, 60);
            SetDamageType(ResistanceType.Fire, 20);
            SetDamageType(ResistanceType.Poison, 20);

            SetResistance(ResistanceType.Physical, 55, 65);
            SetResistance(ResistanceType.Fire, 50, 60);
            SetResistance(ResistanceType.Cold, 55, 65);
            SetResistance(ResistanceType.Poison, 70, 90);
            SetResistance(ResistanceType.Energy, 65, 75);

            SetSkill(SkillName.EvalInt, 100);
            SetSkill(SkillName.Magery, 100);
            SetSkill(SkillName.Meditation, 0);
            SetSkill(SkillName.Poisoning, 5.4);
            SetSkill(SkillName.Anatomy, 117.5);
            SetSkill(SkillName.MagicResist, 120.0);
            SetSkill(SkillName.Tactics, 119.9);
            SetSkill(SkillName.Wrestling, 119.9);

            Fame = 24000;
            Karma = -24000;

            SetSpecialAbility(SpecialAbility.DragonBreath);
        }

        public Ilhenir(Serial serial)
            : base(serial)
        {
        }

        public override ChampionSkullType SkullType => ChampionSkullType.Pain;
        public override Type[] UniqueList => new Type[] { };
        public override Type[] SharedList => new Type[]
                {
                    typeof(ANecromancerShroud),
                    typeof(LieutenantOfTheBritannianRoyalGuard),
                    typeof(OblivionsNeedle),
                    typeof(TheRobeOfBritanniaAri)
                };
        public override Type[] DecorativeList => new Type[] { typeof(MonsterStatuette) };
        public override MonsterStatuetteType[] StatueTypes => new MonsterStatuetteType[] { };
        public override bool Unprovokable => true;
        public override bool Uncalmable => true;
        public override Poison PoisonImmune => Poison.Lethal;
        public override int TreasureMapLevel => 5;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich, 4);
            AddLoot(LootPack.FilthyRich);
            AddLoot(LootPack.Talisman, 5);
            AddLoot(LootPack.ArcanistScrolls, Utility.RandomMinMax(1, 3));
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            if (Utility.RandomDouble() < 0.1)
                DropOoze();

            base.OnDamage(amount, from, willKill);
        }

        public override int GetAngerSound()
        {
            return 0x581;
        }

        public override int GetIdleSound()
        {
            return 0x582;
        }

        public override int GetAttackSound()
        {
            return 0x580;
        }

        public override int GetHurtSound()
        {
            return 0x583;
        }

        public override int GetDeathSound()
        {
            return 0x584;
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

        public virtual void DropOoze()
        {
            int amount = Utility.RandomMinMax(1, 3);
            bool corrosive = Utility.RandomBool();

            for (int i = 0; i < amount; i++)
            {
                Item ooze = new StainedOoze(corrosive);
                Point3D p = new Point3D(Location);

                for (int j = 0; j < 5; j++)
                {
                    p = GetSpawnPosition(2);
                    bool found = false;

                    foreach (Item item in Map.GetItemsInRange(p, 0))
                        if (item is StainedOoze)
                        {
                            found = true;
                            break;
                        }

                    if (!found)
                        break;
                }

                ooze.MoveToWorld(p, Map);
            }

            if (Combatant is PlayerMobile)
            {
                if (corrosive)
                    ((PlayerMobile)Combatant).SendLocalizedMessage(1072071); // A corrosive gas seeps out of your enemy's skin!
                else
                    ((PlayerMobile)Combatant).SendLocalizedMessage(1072072); // A poisonous gas seeps out of your enemy's skin!
            }
        }
    }

    public class StainedOoze : Item
    {
        private bool m_Corrosive;
        private Timer m_Timer;
        private int m_Ticks;

        [Constructable]
        public StainedOoze()
            : this(false)
        {
        }

        [Constructable]
        public StainedOoze(bool corrosive)
            : base(0x122A)
        {
            Movable = false;
            Hue = 0x95;

            m_Corrosive = corrosive;
            m_Timer = Timer.DelayCall(TimeSpan.Zero, TimeSpan.FromSeconds(1), OnTick);
            m_Ticks = 0;
        }

        public StainedOoze(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Corrosive
        {
            get
            {
                return m_Corrosive;
            }
            set
            {
                m_Corrosive = value;
            }
        }
        public override void OnAfterDelete()
        {
            if (m_Timer != null)
            {
                m_Timer.Stop();
                m_Timer = null;
            }
        }

        public void Damage(Mobile m)
        {
            if (m_Corrosive)
            {
                List<Item> items = m.Items;
                bool damaged = false;

                for (int i = 0; i < items.Count; ++i)
                {
                    IDurability wearable = items[i] as IDurability;

                    if (wearable != null && wearable.HitPoints >= 10 && Utility.RandomDouble() < 0.25)
                    {
                        wearable.HitPoints -= (wearable.HitPoints == 10) ? Utility.Random(1, 5) : 10;
                        damaged = true;
                    }
                }

                if (damaged)
                {
                    m.LocalOverheadMessage(MessageType.Regular, 0x21, 1072070); // The infernal ooze scorches you, setting you and your equipment ablaze!
                    return;
                }
            }

            AOS.Damage(m, 40, 0, 0, 0, 100, 0);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version

            writer.Write(m_Corrosive);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_Corrosive = reader.ReadBool();

            m_Timer = Timer.DelayCall(TimeSpan.Zero, TimeSpan.FromSeconds(1), OnTick);
            m_Ticks = (ItemID == 0x122A) ? 0 : 30;
        }

        private void OnTick()
        {
            List<Mobile> toDamage = new List<Mobile>();
            IPooledEnumerable eable = GetMobilesInRange(0);

            foreach (Mobile m in eable)
            {
                if (m is BaseCreature)
                {
                    BaseCreature bc = (BaseCreature)m;

                    if (!bc.Controlled && !bc.Summoned)
                        continue;
                }
                else if (!m.Player)
                {
                    continue;
                }

                if (m.Alive && !m.IsDeadBondedPet && m.CanBeDamaged())
                    toDamage.Add(m);
            }

            eable.Free();

            for (int i = 0; i < toDamage.Count; ++i)
                Damage(toDamage[i]);

            ++m_Ticks;

            if (m_Ticks >= 35)
                Delete();
            else if (m_Ticks == 30)
                ItemID = 0x122B;
        }
    }
}
