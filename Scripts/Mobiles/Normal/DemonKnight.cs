using System;
using System.Collections.Generic;
using Server.Items;
using System.Linq;

namespace Server.Mobiles
{
    [CorpseName("a demon knight corpse")]
    public class DemonKnight : BaseCreature
    {
        private DateTime m_NextArea;
        private bool m_InHere;

        public static Type[] DoomArtifact { get { return m_DoomArtifact; } }
        private static Type[] m_DoomArtifact = new Type[]
        {
            typeof(LegacyOfTheDreadLord),       typeof(TheTaskmaster),              typeof(TheDragonSlayer),
            typeof(ArmorOfFortune),             typeof(GauntletsOfNobility),        typeof(HelmOfInsight),
            typeof(HolyKnightsBreastplate),     typeof(JackalsCollar),              typeof(LeggingsOfBane),
            typeof(MidnightBracers),            typeof(OrnateCrownOfTheHarrower),   typeof(ShadowDancerLeggings),
            typeof(TunicOfFire),                typeof(VoiceOfTheFallenKing),       typeof(BraceletOfHealth),
            typeof(OrnamentOfTheMagician),      typeof(RingOfTheElements),          typeof(RingOfTheVile),
            typeof(Aegis),                      typeof(ArcaneShield),               typeof(AxeOfTheHeavens),
            typeof(BladeOfInsanity),            typeof(BoneCrusher),                typeof(BreathOfTheDead),
            typeof(Frostbringer),               typeof(SerpentsFang),               typeof(StaffOfTheMagi),
            typeof(TheBeserkersMaul),           typeof(TheDryadBow),                typeof(DivineCountenance),
            typeof(HatOfTheMagi),               typeof(HuntersHeaddress),           typeof(SpiritOfTheTotem)
        };

        public static Type[][] RewardTable { get { return m_RewardTable; } }
        private static Type[][] m_RewardTable = new Type[][]
        {
            new Type[] { typeof(HatOfTheMagi) },            new Type[] { typeof(StaffOfTheMagi) },      new Type[] { typeof(OrnamentOfTheMagician) },
            new Type[] { typeof(ShadowDancerLeggings) },    new Type[] {typeof(RingOfTheElements) },    new Type[] { typeof(GauntletsOfNobility) },
            new Type[] { typeof(LeggingsOfBane) },          new Type[] { typeof(MidnightBracers) },     new Type[] { typeof(Glenda) },
            new Type[] { typeof(BowOfTheInfiniteSwarm) },   new Type[] { typeof(TheDeceiver) },         new Type[] { typeof(TheScholarsHalo) },
            new Type[] { typeof(DoomRecipeScroll) },
            new Type[] 
            {
                typeof(LegacyOfTheDreadLord),       typeof(TheTaskmaster),
                typeof(ArmorOfFortune),             typeof(HelmOfInsight),
                typeof(HolyKnightsBreastplate),     typeof(JackalsCollar),              
                typeof(OrnateCrownOfTheHarrower),   typeof(TheDragonSlayer),
                typeof(TunicOfFire),                typeof(VoiceOfTheFallenKing),
                typeof(RingOfTheVile),              typeof(BraceletOfHealth),
                typeof(Aegis),                      typeof(ArcaneShield),
                typeof(BladeOfInsanity),            typeof(BoneCrusher),
                typeof(Frostbringer),               typeof(SerpentsFang),
                typeof(TheBeserkersMaul),           typeof(TheDryadBow),
                typeof(HuntersHeaddress),           typeof(SpiritOfTheTotem),
                typeof(AxeOfTheHeavens),            typeof(BreathOfTheDead),
                typeof(DivineCountenance)
            }
        };

        [Constructable]
        public DemonKnight()
            : base(AIType.AI_NecroMage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = NameList.RandomName("demon knight");
            Title = "the Dark Father";
            Body = 318;
            BaseSoundID = 0x165;

            SetStr(500);
            SetDex(100);
            SetInt(1000);

            SetHits(30000);
            SetMana(5000);

            SetDamage(17, 21);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Fire, 20);
            SetDamageType(ResistanceType.Cold, 20);
            SetDamageType(ResistanceType.Poison, 20);
            SetDamageType(ResistanceType.Energy, 20);

            SetResistance(ResistanceType.Physical, 60, 70);
            SetResistance(ResistanceType.Fire, 50, 60);
            SetResistance(ResistanceType.Cold, 70, 80);
            SetResistance(ResistanceType.Poison, 70, 80);
            SetResistance(ResistanceType.Energy, 70, 80);

            SetSkill(SkillName.Wrestling, 120.0);
            SetSkill(SkillName.Tactics, 100.0);
            SetSkill(SkillName.MagicResist, 150.0);
            SetSkill(SkillName.DetectHidden, 100.0);
            SetSkill(SkillName.Magery, 100.0);
            SetSkill(SkillName.EvalInt, 100.0);
            SetSkill(SkillName.Meditation, 120.0);
            SetSkill(SkillName.Necromancy, 120.0);
            SetSkill(SkillName.SpiritSpeak, 120.0);

            Fame = 28000;
            Karma = -28000;

            VirtualArmor = 64;

            m_NextArea = DateTime.UtcNow;

            SetWeaponAbility(WeaponAbility.CrushingBlow);
            SetWeaponAbility(WeaponAbility.WhirlwindAttack);
        }

        public DemonKnight(Serial serial)
            : base(serial)
        {
        }
       
        public override bool IgnoreYoungProtection
        {
            get
            {
                return Core.ML;
            }
        }
        public override bool BardImmune
        {
            get
            {
                return !Core.SE;
            }
        }
        public override bool Unprovokable
        {
            get
            {
                return Core.SE;
            }
        }
        public override bool AreaPeaceImmune
        {
            get
            {
                return Core.SE;
            }
        }
        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Lethal;
            }
        }
        public override int TreasureMapLevel
        {
            get
            {
                return 6;
            }
        }
        private static bool CheckLocation(Mobile m)
        {
            Region r = m.Region;

            if (r.IsPartOf<Server.Regions.HouseRegion>() || Server.Multis.BaseBoat.FindBoatAt(m, m.Map) != null)
                return false;
            //TODO: a CanReach of something check as opposed to above?

            if (r.IsPartOf("GauntletRegion"))
                return true;

            return (m.Map == Map.Malas);
        }

        public static void HandleKill(Mobile victim, Mobile killer)
        {
            PlayerMobile pm = killer as PlayerMobile;
            BaseCreature bc = victim as BaseCreature;

            if (!Core.AOS)
                return;

            if ( pm == null || bc == null || bc.NoKillAwards/*|| !CheckLocation(bc) || !CheckLocation(pm)*/)
                return;

            //Make sure its a boss we killed!!
            bool boss = bc is Impaler || bc is DemonKnight || bc is DarknightCreeper || bc is FleshRenderer  || bc is ShadowKnight || bc is AbysmalHorror;
            if (!boss)
                return;
             
            double gpoints = pm.GauntletPoints;
            int luck = Math.Max(0, pm.RealLuck);

            pm.GauntletPoints += (int)Math.Max(0, (bc.Fame * (1 + Math.Sqrt(luck) / 100)) / 2);

            const double A = 0.000863316841;
            const double B = 0.00000425531915;

            double chance = A * Math.Pow(10, B * gpoints);
            double roll = Utility.RandomDouble();

            if (chance > roll)
            {
                Item i = null;

                if (Core.TOL)
                {
                    int ran = Utility.Random(m_RewardTable.Length + 1);

                    if (ran >= m_RewardTable.Length)
                    {
                        i = Loot.RandomArmorOrShieldOrWeaponOrJewelry(LootPackEntry.IsInTokuno(killer), LootPackEntry.IsMondain(killer), LootPackEntry.IsStygian(killer));
                        RunicReforging.GenerateRandomArtifactItem(i, luck, Utility.RandomMinMax(1000, 1200));
                        NegativeAttributes attrs = RunicReforging.GetNegativeAttributes(i);

                        if (attrs != null)
                        {
                            attrs.Prized = 1;
                            attrs.Brittle = 0;
                            attrs.Massive = 0;
                            attrs.Unwieldly = 0;
                            attrs.Antique = 0;
                            attrs.NoRepair = 0;
                        }
                    }
                    else
                    {
                        Type[] list = m_RewardTable[ran];
                        Type t = list.Length == 1 ? list[0] : list[Utility.Random(list.Length)];

                        i = Activator.CreateInstance(t) as Item;
                    }
                }
                else
                {
                    i = Activator.CreateInstance(m_DoomArtifact[Utility.Random(m_DoomArtifact.Length)]) as Item;
                }

                if (i != null)
                {
                    pm.SendLocalizedMessage(1062317); // For your valor in combating the fallen beast, a special artifact has been bestowed on you.

                    pm.PlaySound(0x5B4);

                    if (!pm.PlaceInBackpack(i))
                    {
                        if (pm.BankBox != null && pm.BankBox.TryDropItem(killer, i, false))
                            pm.SendLocalizedMessage(1079730); // The item has been placed into your bank box.
                        else
                        {
                            pm.SendLocalizedMessage(1072523); // You find an artifact, but your backpack and bank are too full to hold it.
                            i.MoveToWorld(pm.Location, pm.Map);
                        }
                    }

                    pm.GauntletPoints = 0;
                }
            }
        }

        public override void OnDeath(Container c)
        {
            List<DamageStore> rights = GetLootingRights();

            int top = 0;
            Item blood = null;

            foreach (Mobile m in rights.Select(x => x.m_Mobile).Distinct().Take(3))
            {
                if (top == 0)
                    blood = new BloodOfTheDarkFather(5);
                else if (top == 1)
                    blood = new BloodOfTheDarkFather(3);
                else if (top == 2)
                    blood = new BloodOfTheDarkFather(2);

                top++;

                if (m.Backpack == null || !m.Backpack.TryDropItem(m, blood, false))
                {
                    m.BankBox.DropItem(blood);
                }
            }

            base.OnDeath(c);
        }

        public static Mobile FindRandomPlayer(BaseCreature creature)
        {
            List<DamageStore> rights = creature.GetLootingRights();

            for (int i = rights.Count - 1; i >= 0; --i)
            {
                DamageStore ds = rights[i];

                if (!ds.m_HasRight)
                    rights.RemoveAt(i);
            }

            if (rights.Count > 0)
                return rights[Utility.Random(rights.Count)].m_Mobile;

            return null;
        }

        public override void OnThink()
        {
            if (Core.TOL && DateTime.UtcNow > m_NextArea)
                Teleport();
        }

        private void Teleport()
        {
            System.Collections.Generic.List<Mobile> toTele = new System.Collections.Generic.List<Mobile>();

            IPooledEnumerable eable = GetMobilesInRange(12);
            foreach (Mobile mob in eable)
            {
                if (mob is BaseCreature)
                {
                    BaseCreature bc = mob as BaseCreature;

                    if (!bc.Controlled)
                        continue;
                }

                if (mob != this && mob.Alive && mob.Player && CanBeHarmful(mob, false) && mob.AccessLevel == AccessLevel.Player)
                    toTele.Add(mob);
            }
            eable.Free();

            if (toTele.Count > 0)
            {
                Mobile from = toTele[Utility.Random(toTele.Count)];

                if (from != null)
                {
                    Combatant = from;

                    from.MoveToWorld(GetSpawnPosition(1), Map);
                    from.FixedParticles(0x376A, 9, 32, 0x13AF, EffectLayer.Waist);
                    from.PlaySound(0x1FE);
                }
            }

            m_NextArea = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(20, 30)); // too much
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.SuperBoss, 2);
            AddLoot(LootPack.HighScrolls, Utility.RandomMinMax(6, 60));
        }
        
        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            if (from != null && from != this && !m_InHere)
            {
                m_InHere = true;
                AOS.Damage(from, this, Utility.RandomMinMax(8, 20), 100, 0, 0, 0, 0);

                MovingEffect(from, 0xECA, 10, 0, false, false, 0, 0);
                PlaySound(0x491);

                if (0.05 > Utility.RandomDouble())
                    Timer.DelayCall(TimeSpan.FromSeconds(1.0), new TimerStateCallback(CreateBones_Callback), from);

                m_InHere = false;
            }
        }

        public virtual void CreateBones_Callback(object state)
        {
            Mobile from = (Mobile)state;
            Map map = from.Map;

            if (map == null)
                return;

            int count = Utility.RandomMinMax(1, 3);

            for (int i = 0; i < count; ++i)
            {
                int x = from.X + Utility.RandomMinMax(-1, 1);
                int y = from.Y + Utility.RandomMinMax(-1, 1);
                int z = from.Z;

                if (!map.CanFit(x, y, z, 16, false, true))
                {
                    z = map.GetAverageZ(x, y);

                    if (z == from.Z || !map.CanFit(x, y, z, 16, false, true))
                        continue;
                }

                UnholyBone bone = new UnholyBone();

                bone.Hue = 0;
                bone.Name = "unholy bones";
                bone.ItemID = Utility.Random(0xECA, 9);

                bone.MoveToWorld(new Point3D(x, y, z), map);
            }
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

            m_NextArea = DateTime.UtcNow;
        }
    }
}
