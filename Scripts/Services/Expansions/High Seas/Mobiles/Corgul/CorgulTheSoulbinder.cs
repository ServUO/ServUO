using Server;
using System;
using System.Collections.Generic;
using Server.Items;
using Server.Misc;
using Server.Regions;
using System.Linq;

namespace Server.Mobiles
{
    public class CorgulTheSoulBinder : BaseSeaChampion
    {
        private DateTime m_NextDismount;
        private DateTime m_NextArea;
        private DateTime m_NextReturn;
        private bool m_HasDone2ndSpawn;
        private CorgulAltar m_Altar;
        private List<BaseCreature> m_Helpers = new List<BaseCreature>();

        public override bool CanDamageBoats { get { return false; } }

        public override bool TaintedLifeAura { get { return true; } }
        public override int Meat { get { return 5; } }
        public override double TreasureMapChance { get { return .25; } }
        public override int TreasureMapLevel { get { return 7; } }
        public override Poison PoisonImmune { get { return Poison.Deadly; } }

        public override bool TeleportsTo { get { return true; } }
        public override TimeSpan TeleportDuration { get { return TimeSpan.FromSeconds(Utility.RandomMinMax(10, 50)); } }
        public override double TeleportProb { get { return 1.0; } }
        public override bool TeleportsPets { get { return true; } }

        public override Type[] UniqueList { get { return new Type[] { typeof(CorgulsEnchantedSash), typeof(CorgulsHandbookOnMysticism), typeof(CorgulsHandbookOnTheUndead) }; } }
        public override Type[] SharedList { get { return new Type[] { typeof(HelmOfVengence), typeof(RingOfTheSoulbinder), typeof(RuneEngravedPegLeg), typeof(CullingBlade) }; } }
        public override Type[] DecorativeList { get { return new Type[] { typeof(EnchantedBladeDeed), typeof(EnchantedVortexDeed) }; } }

        public override bool NoGoodies { get { return true; } }
        public override bool CanGivePowerscrolls { get { return false; } }

        [Constructable]
        public CorgulTheSoulBinder()
            : this(null)
        {
        }

        public CorgulTheSoulBinder(CorgulAltar altar) : base(null, AIType.AI_NecroMage, FightMode.Closest)
        {
            m_Altar = altar;
            Name = "Corgul the Soulbinder";
            BaseSoundID = 609;
            Body = 0x4C;
            Hue = 2076;

            m_NextDismount = DateTime.UtcNow;
            m_NextArea = DateTime.UtcNow;
            m_HasDone2ndSpawn = false;

            SetStr(800, 900);
            SetDex(121, 165);
            SetInt(300, 400);

            SetMana(4500);
            SetHits(65000);

            SetDamage(19, 24);

            SetDamageType(ResistanceType.Physical, 10);
            SetDamageType(ResistanceType.Fire, 10);
            SetDamageType(ResistanceType.Cold, 30);
            SetDamageType(ResistanceType.Poison, 40);
            SetDamageType(ResistanceType.Energy, 10);

            SetResistance(ResistanceType.Physical, 50, 60);
            SetResistance(ResistanceType.Fire, 80, 90);
            SetResistance(ResistanceType.Cold, 85, 95);
            SetResistance(ResistanceType.Poison, 80, 90);
            SetResistance(ResistanceType.Energy, 80, 90);

            SetSkill(SkillName.Wrestling, 110.0, 120.0);
            SetSkill(SkillName.Tactics, 110.0, 120.0);
            SetSkill(SkillName.Magery, 110.9, 120.0);
            SetSkill(SkillName.EvalInt, 110.9, 120.0);
            SetSkill(SkillName.Meditation, 110.9, 120.0);
            SetSkill(SkillName.Necromancy, 110.9, 120.0);
            SetSkill(SkillName.SpiritSpeak, 110.9, 120.0);

            Fame = 25000;
            Karma = -25000;

            m_NextReturn = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(120, 180));
        }

        public double SharedChance { get { return this.Map != null && this.Map.Rules == MapRules.FeluccaRules ? .12 : .08; } }
        public double DecorativeChance { get { return this.Map != null && this.Map.Rules == MapRules.FeluccaRules ? .40 : .25; } }

        public override bool OnBeforeDeath()
        {
            List<DamageStore> rights = GetLootingRights();

            Mobile winner = null;

            if (rights != null && rights.Count > 0)
            {
                rights.Sort();

                if(rights.Count >= 5)
                    winner = rights[Utility.Random(5)].m_Mobile;
                else if(rights.Count > 1)
                    winner = rights[Utility.Random(rights.Count)].m_Mobile;
                else
                    winner = rights[0].m_Mobile;
            }

            if(winner != null)
                GiveArtifact(winner, CreateArtifact(UniqueList));

            return base.OnBeforeDeath();
        }

        public override Item GetArtifact()
        {
            double random = Utility.RandomDouble();

            if (SharedChance >= random)
                return CreateArtifact(SharedList);
            else if (DecorativeChance >= random)
                return CreateArtifact(DecorativeList);
            return null;
        }

        public void SpawnHelpers()
        {
            for (int i = 0; i < Utility.RandomMinMax(1, 3); i++)
            {
                BaseCreature bs = new BoundSoul();
                SpawnMobile(bs);
                m_Helpers.Add(bs);
            }

            for (int i = 0; i < 5; i++)
            {
                BaseCreature mage = new SoulboundApprenticeMage();
                SpawnMobile(mage);
                m_Helpers.Add(mage);
            }

            for (int i = 0; i < 2; i++)
            {
                BaseCreature bmage = new SoulboundBattleMage();
                SpawnMobile(bmage);
                m_Helpers.Add(bmage);
            }

            BaseCreature capt = new SoulboundPirateCaptain();
            SpawnMobile(capt);
            m_Helpers.Add(capt);

            BaseCreature raider = new SoulboundPirateRaider();
            SpawnMobile(raider);
            m_Helpers.Add(raider);

            BaseCreature slinger = new SoulboundSpellSlinger();
            SpawnMobile(slinger);
            m_Helpers.Add(slinger);

            for (int i = 0; i < 3; i++)
            {
                BaseCreature sb = new SoulboundSwashbuckler();
                SpawnMobile(sb);
                m_Helpers.Add(sb);
            }
        }

        public void SpawnMobile(BaseCreature bc)
        {
            if(this.Map == null || bc == null)
            {
                if(bc != null)
                    bc.Delete();
                return;
            }

            int x = this.X;
            int y = this.Y;
            int z = this.Z;
            Point3D p = new Point3D(x, y, z);

            for(int i = 0; i < 25; i++)
            {
                x = Utility.RandomMinMax(this.X - 15, this.X + 15);
                y = Utility.RandomMinMax(this.Y - 15, this.Y + 15);
                z = this.Map.GetAverageZ(x, y);

                if (this.Map.CanSpawnMobile(x, y, z))
                {
                    p = new Point3D(x, y, z);
                    break;
                }
            }

            bc.MoveToWorld(p, this.Map);
        }

        public override void OnThink()
        {
            base.OnThink();

            if (m_NextReturn < DateTime.UtcNow)
            {
                Point3D p = CorgulAltar.SpawnLoc;

                if (this.Region.IsPartOf(typeof(CorgulRegion)) && !Utility.InRange(this.Location, p, 15))
                {
                    PlaySound(0x1FE);
                    FixedParticles(0x376A, 9, 32, 0x13AF, EffectLayer.Waist);

                    Location = p;
                    ProcessDelta();

                    PlaySound(0x1FE);
                    FixedParticles(0x376A, 9, 32, 0x13AF, EffectLayer.Waist);

                    m_NextReturn = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(120, 180));
                }
            }

            if (Combatant == null)
                return;

            if (DateTime.UtcNow > m_NextDismount && 0.1 > Utility.RandomDouble())
                DoDismount();

            else if (DateTime.UtcNow > m_NextArea && 0.1 > Utility.RandomDouble())
                DoAreaAttack();

            if (!m_HasDone2ndSpawn)
            {
                if (m_Helpers.Where(bc => bc.Alive && !bc.Deleted).Count() == 0)
                {
                    SpawnHelpers();
                    m_HasDone2ndSpawn = true;
                }
            }
        }

        public void DoDismount()
        {
            List<Mobile> targets = new List<Mobile>();

            IPooledEnumerable eable = this.GetMobilesInRange(12);
            foreach (Mobile mob in eable)
            {
                if (!CanBeHarmful(mob) || mob == this)
                    continue;
                if (mob is BaseCreature && (((BaseCreature)mob).Controlled || ((BaseCreature)mob).Summoned || ((BaseCreature)mob).Team != this.Team))
                    targets.Add(mob);
                else if (mob is PlayerMobile && mob.Alive)
                    targets.Add(mob);
            }
            eable.Free();

            PlaySound(0x2F3);
            for (int i = 0; i < targets.Count; ++i)
            {
                Mobile m = (Mobile)targets[i];

                if (m != null && !m.Deleted && m is PlayerMobile)
                {
                    PlayerMobile pm = m as PlayerMobile;

                    if (pm != null && (pm.Mounted || pm.Flying))
                    {
                        pm.SetMountBlock(BlockMountType.DismountRecovery, TimeSpan.FromSeconds(10), true);
                    }
                }

                double damage = m.Hits * 0.6;
                if (damage < 10.0)
                    damage = 10.0;
                else if (damage > 75.0)
                    damage = 75.0;
                DoHarmful(m);
                AOS.Damage(m, this, (int)damage, 100, 0, 0, 0, 0);
                if (m.Alive && m.Body.IsHuman && !m.Mounted)
                    m.Animate(20, 7, 1, true, false, 0);
            }


            m_NextDismount = DateTime.UtcNow + TimeSpan.FromMinutes(2);
        }

        public void DoAreaAttack()
        {
            int range = 18;

            new InternalTimer(this, range);

            IPooledEnumerable eable = this.GetMobilesInRange(range);
            foreach (Mobile m in eable)
            {
                if ((m is PlayerMobile || (m is BaseCreature && ((BaseCreature)m).GetMaster() is PlayerMobile)) && CanBeHarmful(m))
                    Timer.DelayCall(TimeSpan.FromSeconds(1), new TimerStateCallback(DoDamage_Callback), m);
            }
            eable.Free();

            m_NextArea = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(60, 180));
        }

        public void DoDamage_Callback(object o)
        {
            Mobile m = (Mobile)o;

            if (m != null)
            {
                DoHarmful(m);
                AOS.Damage(m, this, Utility.RandomMinMax(100, 150), 0, 100, 0, 0, 0);

                if (Utility.RandomBool())
                {
                    WeaponAbility bleed = WeaponAbility.BleedAttack;
                    bleed.OnHit(this, m, 0);
                }
            }
        }

        public void DoEffect(Point3D p, Map map)
        {
            int[] effect = new int[] { 14000, 14013 };
            Effects.PlaySound(p, map, 0x307);
            Effects.SendLocationEffect(p, map, Utility.RandomBool() ? 14000 : 14013, 20);
        }

        private class InternalTimer : Timer
        {
            private CorgulTheSoulBinder m_Mobile;
            private int m_Tick;

            public InternalTimer(CorgulTheSoulBinder mob, int range)
                : base(TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(100), range)
            {
                m_Tick = 1;
                m_Mobile = mob;
                Priority = TimerPriority.FiftyMS;
                Start();
            }

            protected override void OnTick()
            {
                Geometry.Circle2D(m_Mobile.Location, m_Mobile.Map, m_Tick, new DoEffect_Callback(m_Mobile.DoEffect));

                m_Tick++;
            }
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.SuperBoss, 6);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (m_Altar != null)
                m_Altar.OnBossKilled();

            c.DropItem(new MessageInABottle(c.Map));
            c.DropItem(new SpecialFishingNet());
            c.DropItem(new SpecialFishingNet());

            if (m_Helpers != null)
            {
                foreach (BaseCreature bc in m_Helpers)
                {
                    RegisterDamageTo(bc);

                    if (bc != null && bc.Alive)
                        bc.Kill();
                }
            }
        }

        public CorgulTheSoulBinder(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);
            writer.Write(m_HasDone2ndSpawn);
            writer.Write(m_Altar);
            writer.Write(m_Helpers.Count);
            foreach (BaseCreature bc in m_Helpers)
                writer.Write(bc);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    m_HasDone2ndSpawn = reader.ReadBool();
                    goto case 0;
                case 0:
                    m_Altar = reader.ReadItem() as CorgulAltar;

                    int cnt = reader.ReadInt();
                    for (int i = 0; i < cnt; i++)
                    {
                        BaseCreature bc = reader.ReadMobile() as BaseCreature;
                        if (bc != null)
                            m_Helpers.Add(bc);
                    }
                    break;
            }

            m_NextDismount = DateTime.UtcNow;
            m_NextArea = DateTime.UtcNow;
        }
    }
}
