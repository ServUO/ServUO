using System;
using Server.Items;
using System.Collections;
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
    public class Shadowlord : BaseCreature
    {
        private static readonly ArrayList m_Instances = new ArrayList();
        public static ArrayList Instances { get { return m_Instances; } }

        private ShadowlordType m_Type;
        public virtual Type[] ArtifactDrops { get { return _ArtifactTypes; } }

        private Type[] _ArtifactTypes = new Type[]
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
                return this.m_Type;
            }
            set
            {
                this.m_Type = value;
                this.InvalidateProperties();
            }
        }

        [Constructable]
        public Shadowlord()
            : base(AIType.AI_NecroMage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            m_Instances.Add(this);

            this.m_Type = (ShadowlordType)Utility.Random(3);
            this.Name = this.m_Type.ToString();

            this.Body = 146;
            this.BaseSoundID = 0x4B0;

            this.SetStr(981, 1078);
            this.SetDex(1003, 1114);
            this.SetInt(1098, 1245);

            this.SetHits(50000, 55000);
            this.SetStam(1003, 1114);

            this.SetDamage(35, 41);

            this.SetDamageType(ResistanceType.Physical, 20);
            this.SetDamageType(ResistanceType.Fire, 20);
            this.SetDamageType(ResistanceType.Cold, 20);
            this.SetDamageType(ResistanceType.Poison, 20);
            this.SetDamageType(ResistanceType.Energy, 20);

            this.SetResistance(ResistanceType.Physical, 70, 80);
            this.SetResistance(ResistanceType.Fire, 70, 80);
            this.SetResistance(ResistanceType.Cold, 70, 80);
            this.SetResistance(ResistanceType.Poison, 70, 80);
            this.SetResistance(ResistanceType.Energy, 70, 80);

            this.SetSkill(SkillName.EvalInt, 140.0);
            this.SetSkill(SkillName.Magery, 120.0);
            this.SetSkill(SkillName.Meditation, 140.0);
            this.SetSkill(SkillName.MagicResist, 110.2, 120.0);
            this.SetSkill(SkillName.Tactics, 110.1, 115.0);
            this.SetSkill(SkillName.Wrestling, 110.1, 115.0);
			this.SetSkill(SkillName.Necromancy, 120.0);
            this.SetSkill(SkillName.SpiritSpeak, 120.0);
            this.SetSkill(SkillName.Anatomy, 10.0, 20.0);

            this.Fame = 24000;
            this.Karma = -24000;

            this.VirtualArmor = 20;
            this.Hue = 902;
            Timer SelfDeleteTimer = new InternalSelfDeleteTimer(this);
            SelfDeleteTimer.Start();

        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add("#{0}", 1154453 + (int)m_Type); // Shadowlord of ..
        }

        public Shadowlord(Serial serial)
            : base(serial)
        {
            m_Instances.Add(this);
        }

        public override void OnAfterDelete()
        {
            m_Instances.Remove(this);

            base.OnAfterDelete();
        }

        public override bool AlwaysMurderer { get { return true; } }

        public override int GetAngerSound() { return 1550; }
        public override int GetHurtSound() { return 1552; }
        public override int GetDeathSound() { return 1551; }
        
        public class InternalSelfDeleteTimer : Timer
        {
            private Shadowlord Mare;

            public InternalSelfDeleteTimer(Mobile p) : base(TimeSpan.FromMinutes(180))
            {
                Priority = TimerPriority.FiveSeconds;
                Mare = ((Shadowlord)p);
            }
            protected override void OnTick()
            {
                if (Mare.Map != Map.Internal)
                {
                    Mare.Delete();
                    this.Stop();
                }
            }
        }

        public static Shadowlord Spawn(Point3D platLoc, Map platMap)
        {
            if (m_Instances.Count > 0)
                return null;

            Shadowlord creature = new Shadowlord();
            creature.Home = platLoc;
            creature.RangeHome = 4;
            creature.MoveToWorld(platLoc, platMap);

            return creature;
        }

        public override Poison PoisonImmune { get { return Poison.Lethal; } }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.SuperBoss, 4);
            this.AddLoot(LootPack.FilthyRich);
        }

        public override void CheckReflect(Mobile caster, ref bool reflect)
        {
            int c = 0;
            IPooledEnumerable eable = GetMobilesInRange(20);

            foreach (Mobile m in eable)
            {
                if (m != null && m is DarkWisp)
                    c++;
                continue;
            }
            eable.Free();
            if (c > 0)
                reflect = true; // Reflect spells if ShadowLord having wisps around
        }

        public override bool DrainsLife { get { return true; } }
        public override double DrainsLifeChance { get { return 0.25; } }

        public override void DrainLife()
        {
            if (this.Map == null)
                return;

            ArrayList list = new ArrayList();
            int count = 0;
            IPooledEnumerable eable = GetMobilesInRange(20);

            foreach (Mobile m in eable)
            {
                if (m == this || !this.CanBeHarmful(m))
                {
                    if (m is DarkWisp) { count++; }
                    continue;
                }

                if (m is BaseCreature && (((BaseCreature)m).Controlled || ((BaseCreature)m).Summoned || ((BaseCreature)m).Team != this.Team))
                    list.Add(m);
                else if (m.Player)
                    list.Add(m);
            }

            eable.Free();

            foreach (Mobile m in list)
            {
                this.DoHarmful(m);

                m.FixedParticles(0x374A, 10, 15, 5013, 0x496, 0, EffectLayer.Waist);
                m.PlaySound(0x231);

                m.SendMessage("You feel the life drain out of you!");

                int toDrain = count * 10;

                this.Hits += toDrain;
                (new DarkWisp()).MoveToWorld(new Point3D(this.Location), this.Map);
                int teleportchance = Hits / HitsMax;

                if (teleportchance < Utility.RandomDouble() && m.Alive)
                    switch (Utility.Random(6))
                    {
                        case 0: m.MoveToWorld(new Point3D(6431, 1664, 0), this.Map); break;
                        case 1: m.MoveToWorld(new Point3D(6432, 1634, 0), this.Map); break;
                        case 2: m.MoveToWorld(new Point3D(6401, 1657, 0), this.Map); break;
                        case 3: m.MoveToWorld(new Point3D(6401, 1637, 0), this.Map); break;
                        default: m.MoveToWorld(new Point3D(this.Location), this.Map); break;
                    }

                m.Damage(toDrain, this);
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
            writer.Write((int)0); // version

            writer.Write((int)this.m_Type);

        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        this.m_Type = (ShadowlordType)reader.ReadInt();

                        break;
                    }
            }

            Timer SelfDeleteTimer = new InternalSelfDeleteTimer(this);
            SelfDeleteTimer.Start();
        }
    }
}
