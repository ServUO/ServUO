using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a demon knight corpse")]
    public class DemonKnight : BaseCreature
    {
              
        private static readonly Type[] m_DoomArtifact = new Type[]
        {
            typeof(LegacyOfTheDreadLord),
            typeof(TheTaskmaster),
            typeof(TheDragonSlayer),
            typeof(ArmorOfFortune),
            typeof(GauntletsOfNobility),
            typeof(HelmOfInsight),
            typeof(HolyKnightsBreastplate),
            typeof(JackalsCollar),
            typeof(LeggingsOfBane),
            typeof(MidnightBracers),
            typeof(OrnateCrownOfTheHarrower),
            typeof(ShadowDancerLeggings),
            typeof(TunicOfFire),
            typeof(VoiceOfTheFallenKing),
            typeof(BraceletOfHealth),
            typeof(OrnamentOfTheMagician),
            typeof(RingOfTheElements),
            typeof(RingOfTheVile),
            typeof(Aegis),
            typeof(ArcaneShield),
            typeof(AxeOfTheHeavens),
            typeof(BladeOfInsanity),
            typeof(BoneCrusher),
            typeof(BreathOfTheDead),
            typeof(Frostbringer),
            typeof(SerpentsFang),
            typeof(StaffOfTheMagi),
            typeof(TheBeserkersMaul),
            typeof(TheDryadBow),
            typeof(DivineCountenance),
            typeof(HatOfTheMagi),
            typeof(HuntersHeaddress),
            typeof(SpiritOfTheTotem)
        };
        private static bool m_InHere;
        [Constructable]
        public DemonKnight()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = NameList.RandomName("demon knight");
            this.Title = "the Dark Father";
            this.Body = 318;
            this.BaseSoundID = 0x165;

            this.SetStr(500);
            this.SetDex(100);
            this.SetInt(1000);

            this.SetHits(30000);
            this.SetMana(5000);

            this.SetDamage(17, 21);

            this.SetDamageType(ResistanceType.Physical, 20);
            this.SetDamageType(ResistanceType.Fire, 20);
            this.SetDamageType(ResistanceType.Cold, 20);
            this.SetDamageType(ResistanceType.Poison, 20);
            this.SetDamageType(ResistanceType.Energy, 20);

            this.SetResistance(ResistanceType.Physical, 30);
            this.SetResistance(ResistanceType.Fire, 30);
            this.SetResistance(ResistanceType.Cold, 30);
            this.SetResistance(ResistanceType.Poison, 30);
            this.SetResistance(ResistanceType.Energy, 30);

            this.SetSkill(SkillName.Necromancy, 120, 120.0);
            this.SetSkill(SkillName.SpiritSpeak, 120.0, 120.0);

            this.SetSkill(SkillName.DetectHidden, 80.0);
            this.SetSkill(SkillName.EvalInt, 100.0);
            this.SetSkill(SkillName.Magery, 100.0);
            this.SetSkill(SkillName.Meditation, 120.0);
            this.SetSkill(SkillName.MagicResist, 150.0);
            this.SetSkill(SkillName.Tactics, 100.0);
            this.SetSkill(SkillName.Wrestling, 120.0);

            this.Fame = 28000;
            this.Karma = -28000;

            this.VirtualArmor = 64;
        }

        public DemonKnight(Serial serial)
            : base(serial)
        {
        }

        public static Type[] DoomArtifact   
        {
            get
            {
                return m_DoomArtifact;
            }
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

            if (r.IsPartOf(typeof(Server.Regions.HouseRegion)) || Server.Multis.BaseBoat.FindBoatAt(m, m.Map) != null)
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
            //CHeck if players are in the region with the bosses
            if ( pm == null || bc == null || !CheckLocation(bc) || !CheckLocation(pm))
                return;
            //Make sure its a boss we killed!!
            bool boss = bc is Impaler || bc is DemonKnight || bc is DarknightCreeper || bc is FleshRenderer  || bc is ShadowKnight || bc is AbysmalHorror;
            if (!boss)
                return;
             
            double gpoints = pm.GauntletPoints;

            pm.GauntletPoints += (int)(bc.Fame * (1 + Math.Sqrt(pm.Luck) / 100))/2;


            const double A = 0.000863316841;
            const double B = 0.00000425531915;

            double chance = A * Math.Pow(10, B * gpoints);
           

            double roll = Utility.RandomDouble();

            killer.PlaySound(0x5B4);

            if (chance > roll )
            {
                Item i = null;

                try
                {
                    i = Activator.CreateInstance(m_DoomArtifact[Utility.Random(m_DoomArtifact.Length)]) as Item;
                }
                catch
                {
                }

                if (i != null)
                {
                    pm.SendLocalizedMessage(1062317); // For your valor in combating the fallen beast, a special artifact has been bestowed on you.

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
      
        public override WeaponAbility GetWeaponAbility()
        {
            switch ( Utility.Random(3) )
            {
                default:
                case 0:
                    return WeaponAbility.DoubleStrike;
                case 1:
                    return WeaponAbility.WhirlwindAttack;
                case 2:
                    return WeaponAbility.CrushingBlow;
            }
        }

        
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.SuperBoss, 2);
            this.AddLoot(LootPack.HighScrolls, Utility.RandomMinMax(6, 60));
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            if (from != null && from != this && !m_InHere)
            {
                m_InHere = true;
                AOS.Damage(from, this, Utility.RandomMinMax(8, 20), 100, 0, 0, 0, 0);

                this.MovingEffect(from, 0xECA, 10, 0, false, false, 0, 0);
                this.PlaySound(0x491);

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
        }
    }
}
