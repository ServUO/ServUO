using Server.Items;
using Server.Mobiles;
using Server.Network;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Engines.Shadowguard
{
    public class ShadowguardBoss : BaseCreature
    {
        public const int MaxSummons = 3;

        public List<BaseCreature> SummonedHelpers { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsLastBoss { get; set; }

        private DateTime _NextSummon;

        public virtual Type[] SummonTypes => null;
        public virtual Type[] ArtifactDrops => _ArtifactTypes;

        public virtual bool CanSummon => Hits <= HitsMax - (HitsMax / 4);

        private readonly Type[] _ArtifactTypes = new Type[]
        {
            typeof(AnonsBoots),                 typeof(AnonsSpellbook),         typeof(BalakaisShamanStaff),
            typeof(EnchantressCameo),           typeof(GrugorsShield),          typeof(WamapsBoneEarrings),
            typeof(HalawasHuntingBow),          typeof(HawkwindsRobe),          typeof(JumusSacredHide),
            typeof(JuonarsGrimoire),            typeof(LereisHuntingSpear),     typeof(UnstableTimeRift),
            typeof(MinaxsSandles),              typeof(MocapotlsObsidianSword), typeof(OzymandiasObi),
            typeof(ShantysWaders),              typeof(TotemOfTheTribe),        typeof(BalakaisShamanStaffGargoyle)
        };

        public ShadowguardBoss(AIType ai) : base(ai, FightMode.Closest, 10, 1, .15, .3)
        {
            _NextSummon = DateTime.UtcNow;

            SetHits(9500, 10000);
            SetMana(4500);
            SetStam(250);

            SetStr(225);
            SetInt(225);
            SetDex(250);

            Fame = 32000;
            Karma = -32000;
        }

        public override Poison PoisonImmune => Poison.Lethal;
        public override bool AlwaysMurderer => true;

        public override void GenerateLoot()
        {
            if (IsLastBoss)
            {
                AddLoot(LootPack.SuperBoss, 7);
            }
        }

        public ShadowguardBoss(Serial serial) : base(serial)
        {
        }

        public int TotalSummons()
        {
            if (SummonedHelpers == null || SummonedHelpers.Count == 0)
                return 0;

            return SummonedHelpers.Where(bc => bc != null && bc.Alive).Count();
        }

        public override void OnGotMeleeAttack(Mobile m)
        {
            if (m is PlayerMobile && CanSummon && !(m is GreaterDragon) && _NextSummon < DateTime.UtcNow)
                Summon();

            base.OnGotMeleeAttack(m);
        }

        public override void OnDamagedBySpell(Mobile m)
        {
            if (m is PlayerMobile && CanSummon && !(m is GreaterDragon) && _NextSummon < DateTime.UtcNow)
                Summon();

            base.OnDamagedBySpell(m);
        }

        public override void OnDeath(Container c)
        {
            if (IsLastBoss)
            {
                List<DamageStore> rights = GetLootingRights();

                foreach (DamageStore ds in rights.Where(s => s.m_HasRight))
                {
                    int luck = ds.m_Mobile is PlayerMobile ? ((PlayerMobile)ds.m_Mobile).RealLuck : ds.m_Mobile.Luck;

                    int chance = 1000 + (luck / 15);

                    if (chance > Utility.Random(5000))
                    {
                        Mobile m = ds.m_Mobile;
                        Item artifact = Loot.Construct(ArtifactDrops[Utility.Random(ArtifactDrops.Length)]);

                        if (artifact != null)
                        {
                            if (m.Backpack == null || !m.Backpack.TryDropItem(m, artifact, false))
                            {
                                m.BankBox.DropItem(artifact);
                                m.SendMessage("For your valor in combating the fallen beast, a special reward has been placed in your bank box.");
                            }
                            else
                                m.SendLocalizedMessage(1062317); // For your valor in combating the fallen beast, a special reward has been bestowed on you.
                        }
                    }
                }
            }
            base.OnDeath(c);
        }

        public override bool OnBeforeDeath()
        {
            if (IsLastBoss)
            {
                DoGoldSpray(Map);
            }

            return base.OnBeforeDeath();
        }

        private void DoGoldSpray(Map map)
        {
            if (Map != null)
            {
                for (int x = -12; x <= 12; ++x)
                {
                    for (int y = -12; y <= 12; ++y)
                    {
                        double dist = Math.Sqrt(x * x + y * y);

                        if (dist <= 12)
                            new GoodiesTimer(map, X + x, Y + y).Start();
                    }
                }
            }
        }

        private class GoodiesTimer : Timer
        {
            private readonly Map m_Map;
            private readonly int m_X, m_Y;

            public GoodiesTimer(Map map, int x, int y)
                : base(TimeSpan.FromSeconds(Utility.RandomDouble() * 10.0))
            {
                m_Map = map;
                m_X = x;
                m_Y = y;
            }

            protected override void OnTick()
            {
                int z = m_Map.GetAverageZ(m_X, m_Y);
                bool canFit = m_Map.CanFit(m_X, m_Y, z, 6, false, false);

                for (int i = -3; !canFit && i <= 3; ++i)
                {
                    canFit = m_Map.CanFit(m_X, m_Y, z + i, 6, false, false);

                    if (canFit)
                        z += i;
                }

                if (!canFit)
                    return;

                Gold g = new Gold(500, 1000);
                g.MoveToWorld(new Point3D(m_X, m_Y, z), m_Map);

                if (0.5 >= Utility.RandomDouble())
                {
                    switch (Utility.Random(3))
                    {
                        case 0: // Fire column
                            {
                                Effects.SendLocationParticles(EffectItem.Create(g.Location, g.Map, EffectItem.DefaultDuration), 0x3709, 10, 30, 5052);
                                Effects.PlaySound(g, g.Map, 0x208);

                                break;
                            }
                        case 1: // Explosion
                            {
                                Effects.SendLocationParticles(EffectItem.Create(g.Location, g.Map, EffectItem.DefaultDuration), 0x36BD, 20, 10, 5044);
                                Effects.PlaySound(g, g.Map, 0x307);

                                break;
                            }
                        case 2: // Ball of fire
                            {
                                Effects.SendLocationParticles(EffectItem.Create(g.Location, g.Map, EffectItem.DefaultDuration), 0x36FE, 10, 10, 5052);

                                break;
                            }
                    }
                }
            }
        }

        public virtual void Summon()
        {
            int max = MaxSummons;
            Map map = Map;

            ShadowguardEncounter inst = ShadowguardController.GetEncounter(Location, Map);

            if (inst != null)
                max += inst.PartySize() * 2;

            if (map == null || SummonTypes == null || SummonTypes.Length == 0 || TotalSummons() > max)
                return;

            int count = Utility.RandomList(1, 2, 2, 2, 3, 3, 4, 5);

            for (int i = 0; i < count; i++)
            {
                if (Combatant == null)
                    return;

                Point3D p = Combatant.Location;

                for (int j = 0; j < 10; j++)
                {
                    int x = Utility.RandomMinMax(p.X - 3, p.X + 3);
                    int y = Utility.RandomMinMax(p.Y - 3, p.Y + 3);
                    int z = map.GetAverageZ(x, y);

                    if (map.CanSpawnMobile(x, y, z))
                    {
                        p = new Point3D(x, y, z);
                        break;
                    }
                }

                BaseCreature spawn = Activator.CreateInstance(SummonTypes[Utility.Random(SummonTypes.Length)]) as BaseCreature;

                if (spawn != null)
                {
                    spawn.MoveToWorld(p, map);
                    spawn.Team = Team;
                    spawn.SummonMaster = this;

                    Timer.DelayCall(TimeSpan.FromSeconds(1), (o) =>
                    {
                        BaseCreature s = o as BaseCreature;

                        if (s != null && s.Combatant != null)
                        {
                            if (!(s.Combatant is PlayerMobile) || !((PlayerMobile)s.Combatant).HonorActive)
                                s.Combatant = Combatant;
                        }

                    }, spawn);

                    AddHelper(spawn);
                }
            }

            _NextSummon = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(20, 40));
        }

        protected virtual void AddHelper(BaseCreature bc)
        {
            if (SummonedHelpers == null)
                SummonedHelpers = new List<BaseCreature>();

            if (!SummonedHelpers.Contains(bc))
                SummonedHelpers.Add(bc);
        }

        public override void Delete()
        {
            base.Delete();

            if (SummonedHelpers != null)
            {
                ColUtility.Free(SummonedHelpers);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(IsLastBoss);

            writer.Write(SummonedHelpers == null ? 0 : SummonedHelpers.Count);

            if (SummonedHelpers != null)
                SummonedHelpers.ForEach(m => writer.Write(m));
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            IsLastBoss = reader.ReadBool();

            int count = reader.ReadInt();

            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    BaseCreature summon = reader.ReadMobile() as BaseCreature;

                    if (summon != null)
                    {
                        if (SummonedHelpers == null)
                            SummonedHelpers = new List<BaseCreature>();

                        SummonedHelpers.Add(summon);
                    }
                }
            }

            _NextSummon = DateTime.UtcNow;
        }
    }

    public enum Form
    {
        Human = 0x190,
        Earth = 14,
        Fire = 15,
        Cold = 163,
        Poison = 158,
        Energy = 220
    }

    public class Anon : ShadowguardBoss
    {
        public override Type[] SummonTypes => _SummonTypes;
        private readonly Type[] _SummonTypes = new Type[] { typeof(ElderGazer), typeof(EvilMage), typeof(Wisp) };

        private DateTime _LastChange;
        private Form _Form;

        public bool CanChange => _LastChange + TimeSpan.FromSeconds(Utility.RandomMinMax(20, 35)) < DateTime.UtcNow;

        [CommandProperty(AccessLevel.GameMaster)]
        public Form Form
        {
            get { return _Form; }
            set
            {
                Form old = _Form;

                if (old != value)
                {
                    _Form = value;
                    InvalidateForm();
                    _LastChange = DateTime.UtcNow;
                }
            }
        }

        [Constructable]
        public Anon() : base(AIType.AI_Mage)
        {
            Name = "Anon";
            Title = "the Mage";

            Body = 0x190;
            HairItemID = 0x203C;

            Hue = Race.RandomSkinHue();

            SetStam(100, 125);
            SetMana(800, 900);
            SetStr(150, 200);
            SetInt(175, 200);
            SetDex(100, 200);

            SetDamage(14, 17);

            SetDamageType(ResistanceType.Physical, 100);

            SetSkill(SkillName.Wrestling, 220.0, 240.0);
            SetSkill(SkillName.Tactics, 110.0, 125.0);
            SetSkill(SkillName.MagicResist, 120.0, 140.0);
            SetSkill(SkillName.Magery, 120.0);
            SetSkill(SkillName.EvalInt, 150.0);
            SetSkill(SkillName.Meditation, 120.0);

            SetResistance(ResistanceType.Physical, 65, 70);
            SetResistance(ResistanceType.Fire, 65, 70);
            SetResistance(ResistanceType.Cold, 65, 70);
            SetResistance(ResistanceType.Poison, 65, 70);
            SetResistance(ResistanceType.Energy, 65, 70);

            SetWearable(new Robe(), 1320);
            SetWearable(new WizardsHat(), 1320);
            SetWearable(new GnarledStaff(), 1320);
            SetWearable(new LeatherGloves(), 1320);

            _LastChange = DateTime.MinValue;
        }

        public override void OnThink()
        {
            base.OnThink();

            if (Form != Form.Human && _LastChange + TimeSpan.FromSeconds(45) < DateTime.UtcNow)
                Form = Form.Human;
        }

        private void SetHighResistance(ResistanceType type)
        {
            SetResistance(ResistanceType.Physical, type == ResistanceType.Physical ? 80 : 50, type == ResistanceType.Physical ? 90 : 60);
            SetResistance(ResistanceType.Fire, type == ResistanceType.Fire ? 80 : 50, type == ResistanceType.Fire ? 90 : 60);
            SetResistance(ResistanceType.Cold, type == ResistanceType.Cold ? 80 : 50, type == ResistanceType.Cold ? 90 : 60);
            SetResistance(ResistanceType.Poison, type == ResistanceType.Poison ? 80 : 50, type == ResistanceType.Poison ? 90 : 60);
            SetResistance(ResistanceType.Energy, type == ResistanceType.Energy ? 80 : 50, type == ResistanceType.Energy ? 90 : 60);
        }

        private void SetBaseResistances()
        {
            SetResistance(ResistanceType.Physical, 50, 60);
            SetResistance(ResistanceType.Fire, 50, 60);
            SetResistance(ResistanceType.Cold, 50, 60);
            SetResistance(ResistanceType.Poison, 50, 60);
            SetResistance(ResistanceType.Energy, 50, 60);
        }


        public void InvalidateForm()
        {
            switch (_Form)
            {
                case Form.Human:
                    if (Body != (int)Form.Human)
                    {
                        Body = (int)Form.Human;
                        HueMod = -1;
                        SetBaseResistances();
                    }
                    break;
                case Form.Earth:
                    if (Body != (int)Form.Earth)
                    {
                        Body = (int)Form.Earth;
                        HueMod = 0;
                        SetHighResistance(ResistanceType.Physical);
                    }
                    break;
                case Form.Fire:
                    if (Body != (int)Form.Fire)
                    {
                        Body = (int)Form.Fire;
                        HueMod = 0;
                        SetHighResistance(ResistanceType.Fire);
                    }
                    break;
                case Form.Cold:
                    if (Body != (int)Form.Cold)
                    {
                        Body = (int)Form.Cold;
                        HueMod = 0;
                        SetHighResistance(ResistanceType.Cold);
                    }
                    break;
                case Form.Poison:
                    if (Body != (int)Form.Poison)
                    {
                        Body = (int)Form.Poison;
                        HueMod = 0;
                        SetHighResistance(ResistanceType.Poison);
                    }
                    break;
                case Form.Energy:
                    if (Body != (int)Form.Energy)
                    {
                        Body = (int)Form.Energy;
                        HueMod = 0x76;
                        SetHighResistance(ResistanceType.Energy);
                    }
                    break;
            }
        }

       
        public override void OnGotMeleeAttack(Mobile m)
        {
            base.OnGotMeleeAttack(m);

            if (_LastChange == DateTime.MinValue)
                _LastChange = DateTime.UtcNow;

            if (CanChange)
                CheckChange(m);
        }

        public void CheckChange(Mobile m)
        {
            if (Form != Form.Human)
            {
                Form = Form.Human;
            }
            else
            {
                BaseWeapon weapon = m.Weapon as BaseWeapon;

                int highest;
                int type = GetHighestDamageType(m, weapon, out highest);

                if (weapon != null)
                {
                    switch (type)
                    {
                        case 0: if (Form != Form.Earth) Form = Form.Earth; break;
                        case 1: if (Form != Form.Fire) Form = Form.Fire; break;
                        case 2: if (Form != Form.Cold) Form = Form.Cold; break;
                        case 3: if (Form != Form.Poison) Form = Form.Poison; break;
                        case 4: if (Form != Form.Energy) Form = Form.Energy; break;
                    }
                }
            }
        }

        private int GetHighestDamageType(Mobile m, BaseWeapon weapon, out int highest)
        {
            int phys, fire, cold, pois, nrgy, chaos, direct;
            weapon.GetDamageTypes(m, out phys, out fire, out cold, out pois, out nrgy, out chaos, out direct);

            int type = 0;
            highest = phys;

            if (fire > highest) { type = 1; highest = fire; }
            if (cold > highest) { type = 2; highest = cold; }
            if (pois > highest) { type = 3; highest = pois; }
            if (nrgy > highest) { type = 4; highest = nrgy; }

            return type;
        }

        public override void AlterMeleeDamageFrom(Mobile m, ref int damage)
        {
            base.AlterMeleeDamageFrom(m, ref damage);

            BaseWeapon weapon = m.Weapon as BaseWeapon;

            if (weapon != null)
            {
                SlayerEntry slayer = SlayerGroup.GetEntryByName(weapon.Slayer);

                if (slayer != null && slayer.Slays(m))
                {
                    if (slayer == slayer.Group.Super)
                        damage *= 2;
                    else
                        damage *= 3;
                }

                int highest;
                int type = GetHighestDamageType(m, weapon, out highest);
                int heal = (int)(damage * (highest / 100.0));

                switch (Form)
                {
                    case Form.Human:
                        /*if(type == 0)
						{
							damage -= heal;
							Hits = Math.Min(Hits + heal, HitsMax);
						}*/
                        break;
                    case Form.Earth:
                        if (type == 0)
                        {
                            damage -= heal;
                            Hits = Math.Min(Hits + heal, HitsMax);
                        }
                        break;
                    case Form.Fire:
                        if (type == 1)
                        {
                            damage -= heal;
                            Hits = Math.Min(Hits + heal, HitsMax);
                        }
                        break;
                    case Form.Cold:
                        if (type == 2)
                        {
                            damage -= heal;
                            Hits = Math.Min(Hits + heal, HitsMax);
                        }
                        break;
                    case Form.Poison:
                        if (type == 3)
                        {
                            damage -= heal;
                            Hits = Math.Min(Hits + heal, HitsMax);
                        }
                        break;
                    case Form.Energy:
                        if (type == 4)
                        {
                            damage -= heal;
                            Hits = Math.Min(Hits + heal, HitsMax);
                        }
                        break;
                }
            }
        }

        public Anon(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            _LastChange = DateTime.UtcNow;
        }
    }

    public class Juonar : ShadowguardBoss
    {
        public override Type[] SummonTypes => _SummonTypes;
        private readonly Type[] _SummonTypes = new Type[] { typeof(SkeletalDragon), typeof(LichLord), typeof(WailingBanshee), typeof(FleshGolem) };

        public override bool CanDiscord => true;
        public override bool PlayInstrumentSound => false;

        private DateTime _NextTeleport;

        [Constructable]
        public Juonar()
            : base(AIType.AI_NecroMage)
        {
            Name = "Juo'nar";
            Body = 78;
            BaseSoundID = 412;
            Hue = 2951;

            SetStam(100, 125);
            SetMana(5000, 5500);
            SetStr(500, 560);
            SetInt(1000, 1200);
            SetDex(100, 125);

            SetDamage(17, 21);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Fire, 20);
            SetDamageType(ResistanceType.Cold, 20);
            SetDamageType(ResistanceType.Poison, 20);
            SetDamageType(ResistanceType.Energy, 20);

            SetSkill(SkillName.Wrestling, 120.0);
            SetSkill(SkillName.Tactics, 100.0);
            SetSkill(SkillName.MagicResist, 150.0);
            SetSkill(SkillName.Magery, 100.0);
            SetSkill(SkillName.EvalInt, 100.0);
            SetSkill(SkillName.Meditation, 120.0);
            SetSkill(SkillName.Necromancy, 120.0);
            SetSkill(SkillName.SpiritSpeak, 120.0);

            SetSkill(SkillName.Musicianship, 120.0);
            SetSkill(SkillName.Discordance, 80.0);

            SetResistance(ResistanceType.Physical, 30);
            SetResistance(ResistanceType.Fire, 30);
            SetResistance(ResistanceType.Cold, 30);
            SetResistance(ResistanceType.Poison, 30);
            SetResistance(ResistanceType.Energy, 30);

            _NextTeleport = DateTime.UtcNow;
        }

        public override void OnThink()
        {
            base.OnThink();

            if (Combatant == null)
                return;

            if (Combatant is Mobile)
            {
                Mobile m = Combatant as Mobile;

                if (InRange(m.Location, 10) && !InRange(m.Location, 2) && m.Alive && CanBeHarmful(m, false) && m.AccessLevel == AccessLevel.Player)
                {
                    if (_NextTeleport < DateTime.UtcNow)
                    {
                        _NextTeleport = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(20, 30)); // too much

                        m.MoveToWorld(GetSpawnPosition(1), Map);
                        m.FixedParticles(0x376A, 9, 32, 0x13AF, EffectLayer.Waist);
                        m.PlaySound(0x1FE);
                    }
                }
            }
        }

        public Juonar(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            _NextTeleport = DateTime.UtcNow;
        }
    }

    public class Virtuebane : ShadowguardBoss
    {
        public override Type[] SummonTypes => _SummonTypes;
        private readonly Type[] _SummonTypes = new Type[] { typeof(MinotaurCaptain), typeof(Daemon), typeof(Titan) };

        public override bool BardImmune => true;

        private DateTime _NextNuke;
        private DateTime _NextDismount;

        [Constructable]
        public Virtuebane() : base(AIType.AI_Mage)
        {
            Name = "Virtuebane";

            Body = 1071;
            SpeechHue = 452;

            SetStam(500, 650);
            SetMana(525, 650);
            SetStr(525, 500);
            SetInt(525, 600);
            SetDex(500, 650);

            SetDamage(24, 33);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Energy, 50);

            SetSkill(SkillName.Wrestling, 120.0, 130.0);
            SetSkill(SkillName.Tactics, 115.0, 130.0);
            SetSkill(SkillName.MagicResist, 150.0, 200.0);
            SetSkill(SkillName.Magery, 135.0, 150.0);
            SetSkill(SkillName.EvalInt, 130.0, 150.0);
            SetSkill(SkillName.Meditation, 0.0);

            SetResistance(ResistanceType.Physical, 60, 85);
            SetResistance(ResistanceType.Fire, 70, 90);
            SetResistance(ResistanceType.Cold, 40, 50);
            SetResistance(ResistanceType.Poison, 90, 95);
            SetResistance(ResistanceType.Energy, 50, 75);

            _NextNuke = DateTime.UtcNow + TimeSpan.FromMinutes(1);
            _NextDismount = DateTime.UtcNow + TimeSpan.FromMinutes(1);
        }

        public override int GetDeathSound() { return 0x596; }
        public override int GetAttackSound() { return 0x597; }
        public override int GetIdleSound() { return 0x598; }
        public override int GetAngerSound() { return 0x599; }
        public override int GetHurtSound() { return 0x59A; }

        public override void OnThink()
        {
            base.OnThink();

            if (Combatant == null)
                return;

            if (Combatant is Mobile && InRange(Combatant.Location, 10))
            {
                if (_NextNuke < DateTime.UtcNow && 0.05 > Utility.RandomDouble())
                {
                    _NextNuke = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(60, 90));

                    Say(1112362); // You will burn to a pile of ash! yellow hue
                    Point3D p = Combatant.Location;

                    Timer.DelayCall(TimeSpan.FromSeconds(3), () =>
                    {
                        DoNuke(Location);
                    });
                }
                else if (_NextDismount < DateTime.UtcNow)
                {
                    _NextDismount = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(60, 90));

                    DoDismount((Mobile)Combatant);
                }
            }
        }

        public void DoNuke(Point3D p)
        {
            if (!Alive || Map == null)
                return;

            int range = 8;

            //Flame Columns
            for (int i = 0; i < 2; i++)
            {
                Misc.Geometry.Circle2D(Location, Map, i, (pnt, map) =>
                    {
                        Effects.SendLocationParticles(EffectItem.Create(pnt, map, EffectItem.DefaultDuration), 0x3709, 10, 30, 5052);
                    });
            }

            //Flash then boom
            Timer.DelayCall(TimeSpan.FromSeconds(1.5), () =>
                {
                    if (Alive && Map != null)
                    {
                        Packet flash = ScreenLightFlash.Instance;
                        IPooledEnumerable e = Map.GetClientsInRange(p, (range * 4) + 5);

                        foreach (NetState ns in e)
                        {
                            if (ns.Mobile != null)
                                ns.Mobile.Send(flash);
                        }

                        e.Free();

                        for (int i = 0; i < range; i++)
                        {
                            Misc.Geometry.Circle2D(Location, Map, i, (pnt, map) =>
                            {
                                Effects.SendLocationEffect(pnt, map, 14000, 14, 10, Utility.RandomMinMax(2497, 2499), 2);
                            });
                        }
                    }
                });

            IPooledEnumerable eable = GetMobilesInRange(range);

            foreach (Mobile m in eable)
            {
                if ((m is PlayerMobile || (m is BaseCreature && ((BaseCreature)m).GetMaster() is PlayerMobile)) && CanBeHarmful(m))
                    Timer.DelayCall(TimeSpan.FromSeconds(1.75), new TimerStateCallback(DoDamage_Callback), m);
            }

            eable.Free();
        }

        private void DoDamage_Callback(object o)
        {
            Mobile m = o as Mobile;
            Map map = Map;

            if (m != null && map != null)
            {
                DoHarmful(m);
                AOS.Damage(m, this, Utility.RandomMinMax(100, 150), 50, 50, 0, 0, 0);

                Direction d = Utility.GetDirection(this, m);
                int range = 0;
                int x = m.X;
                int y = m.Y;
                int orx = x;
                int ory = y;

                while (range < 12)
                {
                    range++;
                    int lastx = x;
                    int lasty = y;

                    Movement.Movement.Offset(d, ref x, ref y);

                    if (!map.CanSpawnMobile(x, y, map.GetAverageZ(x, y)))
                    {
                        m.MoveToWorld(new Point3D(lastx, lasty, map.GetAverageZ(lastx, lasty)), Map);
                        break;
                    }

                    if (range >= 12 && (orx != x || ory != y))
                    {
                        m.MoveToWorld(new Point3D(x, y, map.GetAverageZ(x, y)), Map);
                    }
                }

                m.Paralyze(TimeSpan.FromSeconds(3));
            }
        }

        public void DoDismount(Mobile m)
        {
            MovingParticles(m, 0x36D4, 7, 0, false, true, 9502, 4019, 0x160);
            PlaySound(0x15E);

            double range = m.GetDistanceToSqrt(this);

            Timer.DelayCall(TimeSpan.FromMilliseconds(250 * range), () =>
            {
                IMount mount = m.Mount;

                if (mount != null)
                {
                    if (m is PlayerMobile)
                        ((PlayerMobile)m).SetMountBlock(BlockMountType.Dazed, TimeSpan.FromSeconds(10), true);
                    else
                        mount.Rider = null;
                }
                else if (m.Flying)
                {
                    ((PlayerMobile)m).SetMountBlock(BlockMountType.Dazed, TimeSpan.FromSeconds(10), true);
                }

                AOS.Damage(m, this, Utility.RandomMinMax(15, 25), 100, 0, 0, 0, 0);
            });
        }

        public Virtuebane(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class Ozymandias : ShadowguardBoss
    {
        public override Type[] SummonTypes => _SummonTypes;
        private readonly Type[] _SummonTypes = new Type[] { typeof(LesserHiryu), typeof(EliteNinja), typeof(TsukiWolf) };

        public override double WeaponAbilityChance => 0.4;

        [Constructable]
        public Ozymandias() : base(AIType.AI_Melee)
        {
            Name = "Ozymandias";
            Title = "the Lord of Castle Barataria";

            Hue = Race.RandomSkinHue();
            Body = 0x190;
            FacialHairItemID = 0x2040;

            SetInt(225, 250);
            SetDex(225);

            SetDamage(25, 32);

            SetDamageType(ResistanceType.Physical, 100);

            SetSkill(SkillName.Wrestling, 150.0);
            SetSkill(SkillName.Archery, 150.0);
            SetSkill(SkillName.Anatomy, 100.0);
            SetSkill(SkillName.Tactics, 125.0);
            SetSkill(SkillName.MagicResist, 110.0);

            SetResistance(ResistanceType.Physical, 60, 70);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 60, 70);
            SetResistance(ResistanceType.Poison, 60, 70);
            SetResistance(ResistanceType.Energy, 60, 70);

            SetWearable(new LeatherDo());
            SetWearable(new LeatherSuneate());
            SetWearable(new Yumi());
            SetWearable(new Waraji());
            SetWearable(new BoneArms());

            Scimitar scimitar = new Scimitar
            {
                Movable = false
            };

            PackItem(scimitar);

            _ = new LesserHiryu
            {
                Rider = this
            };

            SetWeaponAbility(WeaponAbility.Dismount);
        }

        public override void GenerateLoot()
        {
            base.GenerateLoot();

            AddLoot(LootPack.LootItem<Arrow>(25, true));
        }

        private DateTime _NextWeaponSwitch;

        public override void OnThink()
        {
            base.OnThink();

            if (Combatant == null || Backpack == null || _NextWeaponSwitch > DateTime.UtcNow)
                return;

            BaseWeapon wep = Weapon as BaseWeapon;

            if ((wep is Fists || wep is BaseRanged) && InRange(Combatant.Location, 1) && 0.1 > Utility.RandomDouble())
            {
                Item scimitar = Backpack.FindItemByType(typeof(Scimitar));

                if (scimitar != null)
                {
                    if (wep is BaseRanged)
                        Backpack.DropItem(wep);

                    SetWearable(scimitar);

                    _NextWeaponSwitch = DateTime.UtcNow + TimeSpan.FromSeconds(10);
                }
            }
            else if ((wep is Fists || !(wep is BaseRanged)) && !InRange(Combatant.Location, 1) && 0.1 > Utility.RandomDouble())
            {
                Item yumi = Backpack.FindItemByType(typeof(Yumi));

                if (yumi != null)
                {
                    if (!(wep is Fists))
                        Backpack.DropItem(wep);

                    SetWearable(yumi);

                    _NextWeaponSwitch = DateTime.UtcNow + TimeSpan.FromSeconds(10);
                }
            }
        }

        public Ozymandias(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
