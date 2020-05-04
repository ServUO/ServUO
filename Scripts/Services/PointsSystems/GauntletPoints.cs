using Server.Items;
using Server.Mobiles;
using System;

namespace Server.Engines.Points
{
    public class DoomGauntlet : PointsSystem
    {
        public override PointsType Loyalty => PointsType.GauntletPoints;
        public override TextDefinition Name => m_Name;
        public override bool AutoAdd => true;
        public override double MaxPoints => double.MaxValue;
        public override bool ShowOnLoyaltyGump => false;

        private readonly TextDefinition m_Name = new TextDefinition("Gauntlet Points");

        public override void SendMessage(PlayerMobile from, double old, double points, bool quest)
        {
        }

        public override TextDefinition GetTitle(PlayerMobile from)
        {
            return new TextDefinition("Gauntlet Points");
        }

        public override void ProcessKill(Mobile victim, Mobile killer)
        {
            PlayerMobile pm = killer as PlayerMobile;
            BaseCreature bc = victim as BaseCreature;

            if (pm == null || bc == null || bc.NoKillAwards || !pm.Alive)
                return;

            //Make sure its a boss we killed!!
            bool boss = bc is Impaler || bc is DemonKnight || bc is DarknightCreeper || bc is FleshRenderer || bc is ShadowKnight || bc is AbysmalHorror;

            if (!boss)
                return;

            int luck = Math.Max(0, pm.RealLuck);
            AwardPoints(pm, (int)Math.Max(0, (bc.Fame * (1 + Math.Sqrt(luck) / 100)) / 2));

            double gpoints = GetPoints(pm);
            const double A = 0.000863316841;
            const double B = 0.00000425531915;

            double chance = A * Math.Pow(10, B * gpoints);
            double roll = Utility.RandomDouble();

            if (chance > roll)
            {
                Item i = null;

                int ran = Utility.Random(m_RewardTable.Length + 1);

                if (ran >= m_RewardTable.Length)
                {
                    i = Loot.RandomArmorOrShieldOrWeaponOrJewelry(LootPackEntry.IsInTokuno(killer), LootPackEntry.IsMondain(killer), LootPackEntry.IsStygian(killer));
                    RunicReforging.GenerateRandomArtifactItem(i, luck, Utility.RandomMinMax(800, 1200));
                    NegativeAttributes attrs = RunicReforging.GetNegativeAttributes(i);

                    if (attrs != null)
                    {
                        attrs.Prized = 1;
                    }
                }
                else
                {
                    Type[] list = m_RewardTable[ran];
                    Type t = list.Length == 1 ? list[0] : list[Utility.Random(list.Length)];

                    i = Activator.CreateInstance(t) as Item;
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

                    SetPoints(pm, 0);
                }
            }
        }

        public static Type[] DoomArtifacts => m_DoomArtifact;
        private static readonly Type[] m_DoomArtifact = new Type[]
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

        public static Type[][] RewardTable => m_RewardTable;
        private static readonly Type[][] m_RewardTable = new Type[][]
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

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt();
        }
    }
}
