using Server.Items;
using Server.Spells;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Mobiles
{
    public enum ShadowlordType
    {
        Astaroth,
        Faulinei,
        Nosfentor
    };

    [CorpseName("a shadowlord corpse")]
    public class Shadowlord : BasePeerless
    {
        //private static readonly List<Shadowlord> m_Instances = new List<Shadowlord>();
        //public static List<Shadowlord> Instances => m_Instances;

        private ShadowlordType m_Type;
        public virtual Type[] ArtifactDrops => _ArtifactTypes;

        private readonly Type[] _ArtifactTypes = new Type[]
        {
            typeof(Abhorrence),         typeof(CaptainJohnesBlade),             typeof(Craven),
            typeof(Equivocation),       typeof(GargishCaptainJohnesBlade),      typeof(GargishEquivocation),
            typeof(GargishPincer),      typeof(Pincer)
        };

        [CommandProperty(AccessLevel.GameMaster)]
        public ShadowlordType Type
        {
            get
            {
                return m_Type;
            }
            set
            {
                m_Type = value;
                InvalidateProperties();
            }
        }

        public List<DarkWisp> Wisps { get; set; } = new List<DarkWisp>();

        [Constructable]
        public Shadowlord()
            : base(AIType.AI_NecroMage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            //m_Instances.Add(this);

            m_Type = (ShadowlordType)Utility.Random(3);
            Name = m_Type.ToString();

            Body = 146;
            BaseSoundID = 0x4B0;
            Hue = 902;

            SetStr(981, 1078);
            SetDex(1003, 1114);
            SetInt(1098, 1245);

            SetHits(50000, 55000);
            SetStam(1003, 1114);

            SetDamage(35, 41);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Fire, 20);
            SetDamageType(ResistanceType.Cold, 20);
            SetDamageType(ResistanceType.Poison, 20);
            SetDamageType(ResistanceType.Energy, 20);

            SetResistance(ResistanceType.Physical, 70, 80);
            SetResistance(ResistanceType.Fire, 70, 80);
            SetResistance(ResistanceType.Cold, 70, 80);
            SetResistance(ResistanceType.Poison, 70, 80);
            SetResistance(ResistanceType.Energy, 70, 80);

            SetSkill(SkillName.EvalInt, 140.0);
            SetSkill(SkillName.Magery, 120.0);
            SetSkill(SkillName.Meditation, 140.0);
            SetSkill(SkillName.MagicResist, 110.2, 120.0);
            SetSkill(SkillName.Tactics, 110.1, 115.0);
            SetSkill(SkillName.Wrestling, 110.1, 115.0);
            SetSkill(SkillName.Necromancy, 120.0);
            SetSkill(SkillName.SpiritSpeak, 120.0);
            SetSkill(SkillName.Anatomy, 10.0, 20.0);

            Fame = 24000;
            Karma = -24000;

            SetSpecialAbility(SpecialAbility.LifeDrain);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add("#{0}", 1154453 + (int)m_Type); // Shadowlord of ..
        }

        public Shadowlord(Serial serial)
            : base(serial)
        {
            //m_Instances.Add(this);
        }

        public override void OnAfterDelete()
        {
            //m_Instances.Remove(this);

            base.OnAfterDelete();
        }

        public override bool AlwaysMurderer => true;
        public override bool DropPrimer => false;
        public override bool GiveMLSpecial => false;

        public override int GetAngerSound() { return 1550; }
        public override int GetHurtSound() { return 1552; }
        public override int GetDeathSound() { return 1551; }

        public override Poison PoisonImmune => Poison.Lethal;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.SuperBoss, 4);
            AddLoot(LootPack.FilthyRich);
        }

        public override void CheckReflect(Mobile caster, ref bool reflect)
        {
            reflect = Wisps.Any(w => !w.Deleted && w.InRange(Location, 20));
        }

        public override void OnDrainLife(Mobile victim)
        {
            if (Map == null || Altar == null)
                return;

            var count = Altar == null ? 0 : Altar.Helpers.Count;

            foreach (Mobile m in SpellHelper.AcquireIndirectTargets(this, Location, Map, 20).OfType<Mobile>())
            {
                if (Altar.Helpers.Count < 6)
                {
                    SpawnHelper(new DarkWisp(), Location);
                }

                if (Region.IsPartOf("Underwater World") && (Map == Map.Trammel || Map == Map.Felucca))
                {
                    int teleportchance = Hits / HitsMax;

                    if (teleportchance < Utility.RandomDouble() && m.Alive)
                    {
                        switch (Utility.Random(6))
                        {
                            case 0: m.MoveToWorld(new Point3D(6431, 1664, 0), Map); break;
                            case 1: m.MoveToWorld(new Point3D(6432, 1634, 0), Map); break;
                            case 2: m.MoveToWorld(new Point3D(6401, 1657, 0), Map); break;
                            case 3: m.MoveToWorld(new Point3D(6401, 1637, 0), Map); break;
                            default: m.MoveToWorld(Location, Map); break;
                        }
                    }
                }
            }
        }

        public override void OnDeath(Container c)
        {
            List<DamageStore> rights = GetLootingRights();

            foreach (DamageStore ds in rights.Where(s => s.m_HasRight))
            {
                int luck = ds.m_Mobile is PlayerMobile ? ((PlayerMobile)ds.m_Mobile).RealLuck : ds.m_Mobile.Luck;
                int chance = 75 + (luck / 15);

                if (chance > Utility.Random(5000))
                {
                    Mobile m = ds.m_Mobile;
                    Item artifact = Loot.Construct(ArtifactDrops[Utility.Random(ArtifactDrops.Length)]);

                    if (artifact != null)
                    {
                        if (m.Backpack == null || !m.Backpack.TryDropItem(m, artifact, false))
                        {
                            m.BankBox.DropItem(artifact);
                            m.SendMessage("For your valor in combating the fallen beast, a special reward has been placed in your bankbox.");
                        }
                        else
                            m.SendLocalizedMessage(1062317); // For your valor in combating the fallen beast, a special reward has been bestowed on you.
                    }
                }
            }

            base.OnDeath(c);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version

            writer.Write((int)m_Type);

        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_Type = (ShadowlordType)reader.ReadInt();
        }
    }
}
