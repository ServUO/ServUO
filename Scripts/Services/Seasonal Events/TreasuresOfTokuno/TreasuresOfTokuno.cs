using Server.Engines.Points;
using Server.Gumps;
using Server.Items;
using Server.Misc;
using Server.Mobiles;
using Server.Network;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Server.Misc
{
    public enum TreasuresOfTokunoEra
    {
        None,
        ToTOne,
        ToTTwo,
        ToTThree
    }

    public class TreasuresOfTokuno : PointsSystem
    {
        public override PointsType Loyalty => PointsType.TOT;
        public override TextDefinition Name => m_Name;
        public override bool AutoAdd => true;
        public override double MaxPoints => double.MaxValue;
        public override bool ShowOnLoyaltyGump => false;

        private readonly TextDefinition m_Name = new TextDefinition("Treasures of Tokuno");

        public const int ItemsPerReward = 10;

        private static readonly Type[] m_LesserArtifactsTotal = new Type[]
        {
            typeof(AncientFarmersKasa), typeof(AncientSamuraiDo), typeof(ArmsOfTacticalExcellence), typeof(BlackLotusHood),
            typeof(DaimyosHelm), typeof(DemonForks), typeof(DragonNunchaku), typeof(Exiler), typeof(GlovesOfTheSun),
            typeof(HanzosBow), typeof(LegsOfStability), typeof(PeasantsBokuto), typeof(PilferedDancerFans), typeof(TheDestroyer),
            typeof(TomeOfEnlightenment), typeof(AncientUrn), typeof(HonorableSwords), typeof(PigmentsOfTokuno), typeof(FluteOfRenewal),
            typeof(LeurociansMempoOfFortune), typeof(LesserPigmentsOfTokuno), typeof(MetalPigmentsOfTokuno), typeof(ChestOfHeirlooms)
        };

        public static Type[] LesserArtifactsTotal => m_LesserArtifactsTotal;

        private static TreasuresOfTokunoEra _DropEra = TreasuresOfTokunoEra.None;
        private static TreasuresOfTokunoEra _RewardEra = TreasuresOfTokunoEra.ToTOne;

        public static TreasuresOfTokunoEra DropEra
        {
            get
            {
                return _DropEra;
            }
            set
            {
                _DropEra = value;
            }
        }

        public static TreasuresOfTokunoEra RewardEra
        {
            get
            {
                return _RewardEra;
            }
            set
            {
                _RewardEra = value;
            }
        }

        private static readonly Type[][] m_LesserArtifacts = new Type[][]
        {
            // ToT One Rewards
            new Type[]
            {
                typeof(AncientFarmersKasa), typeof(AncientSamuraiDo), typeof(ArmsOfTacticalExcellence), typeof(BlackLotusHood),
                typeof(DaimyosHelm), typeof(DemonForks), typeof(DragonNunchaku), typeof(Exiler), typeof(GlovesOfTheSun),
                typeof(HanzosBow), typeof(LegsOfStability), typeof(PeasantsBokuto), typeof(PilferedDancerFans), typeof(TheDestroyer),
                typeof(TomeOfEnlightenment), typeof(AncientUrn), typeof(HonorableSwords), typeof(PigmentsOfTokuno),
                typeof(FluteOfRenewal), typeof(ChestOfHeirlooms)
            },
            // ToT Two Rewards
            new Type[]
            {
                typeof(MetalPigmentsOfTokuno), typeof(AncientFarmersKasa), typeof(AncientSamuraiDo), typeof(ArmsOfTacticalExcellence),
                typeof(MetalPigmentsOfTokuno), typeof(BlackLotusHood), typeof(DaimyosHelm), typeof(DemonForks),
                typeof(MetalPigmentsOfTokuno), typeof(DragonNunchaku), typeof(Exiler), typeof(GlovesOfTheSun), typeof(HanzosBow),
                typeof(MetalPigmentsOfTokuno), typeof(LegsOfStability), typeof(PeasantsBokuto), typeof(PilferedDancerFans), typeof(TheDestroyer),
                typeof(MetalPigmentsOfTokuno), typeof(TomeOfEnlightenment), typeof(AncientUrn), typeof(HonorableSwords),
                typeof(MetalPigmentsOfTokuno), typeof(FluteOfRenewal), typeof(ChestOfHeirlooms)
            },
            // ToT Three Rewards
            new Type[]
            {
                typeof(LesserPigmentsOfTokuno), typeof(AncientFarmersKasa), typeof(AncientSamuraiDo), typeof(ArmsOfTacticalExcellence),
                typeof(LesserPigmentsOfTokuno), typeof(BlackLotusHood), typeof(DaimyosHelm), typeof(HanzosBow),
                typeof(LesserPigmentsOfTokuno), typeof(DemonForks), typeof(DragonNunchaku), typeof(Exiler), typeof(GlovesOfTheSun),
                typeof(LesserPigmentsOfTokuno), typeof(LegsOfStability), typeof(PeasantsBokuto), typeof(PilferedDancerFans), typeof(TheDestroyer),
                typeof(LesserPigmentsOfTokuno), typeof(TomeOfEnlightenment), typeof(AncientUrn), typeof(HonorableSwords), typeof(FluteOfRenewal),
                typeof(LesserPigmentsOfTokuno), typeof(LeurociansMempoOfFortune), typeof(ChestOfHeirlooms)
            }
        };

        public static Type[] LesserArtifacts => m_LesserArtifacts[(int)RewardEra - 1];

        private static Type[][] m_GreaterArtifacts = null;

        public static Type[] GreaterArtifacts
        {
            get
            {
                if (m_GreaterArtifacts == null)
                {
                    m_GreaterArtifacts = new Type[ToTRedeemGump.NormalRewards.Length][];

                    for (int i = 0; i < m_GreaterArtifacts.Length; i++)
                    {
                        m_GreaterArtifacts[i] = new Type[ToTRedeemGump.NormalRewards[i].Length];

                        for (int j = 0; j < m_GreaterArtifacts[i].Length; j++)
                        {
                            m_GreaterArtifacts[i][j] = ToTRedeemGump.NormalRewards[i][j].Type;
                        }
                    }
                }

                return m_GreaterArtifacts[(int)RewardEra - 1];
            }
        }

        private bool CheckLocation(Mobile m)
        {
            Region r = m.Region;

            if (r.IsPartOf<Regions.HouseRegion>() || Multis.BaseBoat.FindBoatAt(m, m.Map) != null)
                return false;

            if (r.IsPartOf("Yomotsu Mines") || r.IsPartOf("Fan Dancer's Dojo"))
                return true;

            return (m.Map == Map.Tokuno);
        }

        public override void SendMessage(PlayerMobile from, double old, double points, bool quest)
        {
        }

        public override TextDefinition GetTitle(PlayerMobile from)
        {
            return new TextDefinition("Treasures of Tokuno");
        }

        public override PointsEntry GetSystemEntry(PlayerMobile pm)
        {
            return new TOTEntry(pm);
        }

        public int GetTurnIns(PlayerMobile pm)
        {
            if (pm == null)
            {
                return 0;
            }

            return GetPlayerEntry<TOTEntry>(pm).TurnIns;
        }

        public int TurnIn(PlayerMobile pm)
        {
            if (pm == null)
            {
                return 0;
            }

            return GetPlayerEntry<TOTEntry>(pm).TurnIns++;
        }

        public void RemoveTurnIns(PlayerMobile pm, int amount)
        {
            TOTEntry entry = GetPlayerEntry<TOTEntry>(pm);

            entry.TurnIns = Math.Max(0, entry.TurnIns - amount);
        }

        public override void ProcessKill(Mobile victim, Mobile killer)
        {
            PlayerMobile pm = killer as PlayerMobile;
            BaseCreature bc = victim as BaseCreature;

            if (DropEra == TreasuresOfTokunoEra.None || pm == null || bc == null || !CheckLocation(bc) || !CheckLocation(pm) || !killer.InRange(victim, 18) || !pm.Alive || bc.GivenSpecialArtifact)
                return;

            if (bc.Controlled || bc.Owners.Count > 0 || bc.Fame <= 0)
                return;

            //25000 for 1/100 chance, 10 hyrus
            //1500, 1/1000 chance, 20 lizard men for that chance.
            int luck = Math.Max(0, pm.RealLuck);
            AwardPoints(pm, (int)Math.Max(0, (bc.Fame * (1 + Math.Sqrt(luck) / 100))));

            //This is the Exponentional regression with only 2 datapoints.
            //A log. func would also work, but it didn't make as much sense.
            //This function isn't OSI exact beign that I don't know OSI's func they used ;p
            double x = GetPoints(pm);

            //const double A = 8.63316841 * Math.Pow( 10, -4 );
            const double A = 0.000863316841;
            //const double B = 4.25531915 * Math.Pow( 10, -6 );
            const double B = 0.00000425531915;

            double chance = A * Math.Pow(10, B * x);

            if (chance > Utility.RandomDouble())
            {
                Item i = null;

                try
                {
                    i = Activator.CreateInstance(m_LesserArtifacts[(int)DropEra - 1][Utility.Random(m_LesserArtifacts[(int)DropEra - 1].Length)]) as Item;
                }
                catch (Exception e)
                {
                    Diagnostics.ExceptionLogging.LogException(e);
                }

                if (i != null)
                {
                    killer.PlaySound(0x5B4);
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

                    bc.GivenSpecialArtifact = true;
                    SetPoints(pm, 0);
                }
            }
        }

        /// <summary>
        /// PlayerMobile.cs version 40 deserializationn
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="turnIns"></param>
        /// <param name="points"></param>
        public void Convert(PlayerMobile pm, int turnIns, int points)
        {
            TOTEntry entry = GetPlayerEntry<TOTEntry>(pm);

            entry.TurnIns = turnIns;
            entry.Points = points;
        }

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

        public class TOTEntry : PointsEntry
        {
            public int TurnIns { get; set; }

            public TOTEntry(PlayerMobile pm)
                : base(pm)
            {
            }

            public override void Serialize(GenericWriter writer)
            {
                writer.Write(1);
                base.Serialize(writer);

                writer.Write(TurnIns);
            }

            public override void Deserialize(GenericReader reader)
            {
                int version = reader.ReadInt();

                if (version > 0)
                {
                    base.Deserialize(reader);
                }

                TurnIns = reader.ReadInt();
            }
        }
    }
}

namespace Server.Mobiles
{
    public class IharaSoko : BaseVendor
    {
        public override bool IsActiveVendor => false;
        public override bool IsInvulnerable => true;
        public override bool DisallowAllMoves => true;
        public override bool ClickTitle => true;
        public override bool CanTeach => false;

        protected List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos => m_SBInfos;

        public override void InitSBInfo()
        {
        }

        public override void InitOutfit()
        {
            AddItem(new Waraji(0x711));
            AddItem(new Backpack());
            AddItem(new Kamishimo(0x483));

            Item item = new LightPlateJingasa
            {
                Hue = 0x711
            };

            AddItem(item);
        }

        [Constructable]
        public IharaSoko()
            : base("the Imperial Minister of Trade")
        {
            Name = "Ihara Soko";
            Female = false;
            Body = 0x190;
            Hue = 0x8403;
        }

        public IharaSoko(Serial serial)
            : base(serial)
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

        public override bool CanBeDamaged()
        {
            return false;
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (m.Alive && m is PlayerMobile)
            {
                PlayerMobile pm = (PlayerMobile)m;

                int range = 3;
                int turnIns = PointsSystem.TreasuresOfTokuno.GetTurnIns(pm);

                if (m.Alive && Math.Abs(Z - m.Z) < 16 && InRange(m, range) && !InRange(oldLocation, range))
                {
                    if (turnIns >= TreasuresOfTokuno.ItemsPerReward)
                    {
                        SayTo(pm, 1070980); // Congratulations! You have turned in enough minor treasures to earn a greater reward.

                        pm.CloseGump(typeof(ToTTurnInGump));    //Sanity

                        if (!pm.HasGump(typeof(ToTRedeemGump)))
                            pm.SendGump(new ToTRedeemGump(this, false));
                    }
                    else
                    {
                        if (turnIns == 0)
                            SayTo(pm, 1071013); // Bring me 10 of the lost treasures of Tokuno and I will reward you with a valuable item.
                        else
                            SayTo(pm, 1070981, string.Format("{0}\t{1}", turnIns, TreasuresOfTokuno.ItemsPerReward)); // You have turned in ~1_COUNT~ minor artifacts. Turn in ~2_NUM~ to receive a reward.

                        ArrayList buttons = ToTTurnInGump.FindRedeemableItems(pm);

                        if (buttons.Count > 0 && !pm.HasGump(typeof(ToTTurnInGump)))
                            pm.SendGump(new ToTTurnInGump(this, buttons));
                    }
                }

                int leaveRange = 7;

                if (!InRange(m, leaveRange) && InRange(oldLocation, leaveRange))
                {
                    pm.CloseGump(typeof(ToTRedeemGump));
                    pm.CloseGump(typeof(ToTTurnInGump));
                }
            }
        }

        public override void TurnToTokuno()
        {
        }
    }
}

namespace Server.Gumps
{
    public class ItemTileButtonInfo : ImageTileButtonInfo
    {
        private Item m_Item;

        public Item Item
        {
            get
            {
                return m_Item;
            }
            set
            {
                m_Item = value;
            }
        }

        public ItemTileButtonInfo(Item i)
            : base(i.ItemID, i.Hue, ((i.Name == null || i.Name.Length <= 0) ? (TextDefinition)i.LabelNumber : (TextDefinition)i.Name))
        {
            m_Item = i;
        }
    }

    public class ToTTurnInGump : BaseImageTileButtonsGump
    {
        public static ArrayList FindRedeemableItems(Mobile m)
        {
            Backpack pack = (Backpack)m.Backpack;
            if (pack == null)
                return new ArrayList();

            ArrayList items = new ArrayList(pack.FindItemsByType(TreasuresOfTokuno.LesserArtifactsTotal));
            ArrayList buttons = new ArrayList();

            for (int i = 0; i < items.Count; i++)
            {
                Item item = (Item)items[i];
                if (item is ChestOfHeirlooms && !((ChestOfHeirlooms)item).Locked)
                    continue;

                if (item is ChestOfHeirlooms && ((ChestOfHeirlooms)item).TrapLevel != 10)
                    continue;

                if (item is PigmentsOfTokuno && ((PigmentsOfTokuno)item).Type != PigmentType.None)
                    continue;

                buttons.Add(new ItemTileButtonInfo(item));
            }

            return buttons;
        }

        readonly Mobile m_Collector;

        public ToTTurnInGump(Mobile collector, ArrayList buttons)
            : base(1071012, buttons)// Click a minor artifact to give it to Ihara Soko.
        {
            m_Collector = collector;
        }

        public ToTTurnInGump(Mobile collector, ItemTileButtonInfo[] buttons)
            : base(1071012, buttons)// Click a minor artifact to give it to Ihara Soko.
        {
            m_Collector = collector;
        }

        public override void HandleButtonResponse(NetState sender, int adjustedButton, ImageTileButtonInfo buttonInfo)
        {
            PlayerMobile pm = sender.Mobile as PlayerMobile;

            Item item = ((ItemTileButtonInfo)buttonInfo).Item;

            if (!(pm != null && item.IsChildOf(pm.Backpack) && pm.InRange(m_Collector.Location, 7)))
                return;

            item.Delete();

            PointsSystem.TreasuresOfTokuno.TurnIn(pm);
            int turnIns = PointsSystem.TreasuresOfTokuno.GetTurnIns(pm);

            if (turnIns >= TreasuresOfTokuno.ItemsPerReward)
            {
                m_Collector.SayTo(pm, 1070980); // Congratulations! You have turned in enough minor treasures to earn a greater reward.

                pm.CloseGump(typeof(ToTTurnInGump));	//Sanity

                if (!pm.HasGump(typeof(ToTRedeemGump)))
                    pm.SendGump(new ToTRedeemGump(m_Collector, false));
            }
            else
            {
                m_Collector.SayTo(pm, 1070981, string.Format("{0}\t{1}", turnIns, TreasuresOfTokuno.ItemsPerReward)); // You have turned in ~1_COUNT~ minor artifacts. Turn in ~2_NUM~ to receive a reward.

                ArrayList buttons = FindRedeemableItems(pm);

                pm.CloseGump(typeof(ToTTurnInGump)); //Sanity

                if (buttons.Count > 0)
                    pm.SendGump(new ToTTurnInGump(m_Collector, buttons));
            }
        }

        public override void HandleCancel(NetState sender)
        {
            PlayerMobile pm = sender.Mobile as PlayerMobile;

            if (pm == null || !pm.InRange(m_Collector.Location, 7))
                return;

            int turnIns = PointsSystem.TreasuresOfTokuno.GetTurnIns(pm);

            if (turnIns == 0)
                m_Collector.SayTo(pm, 1071013); // Bring me 10 of the lost treasures of Tokuno and I will reward you with a valuable item.
            else if (turnIns < TreasuresOfTokuno.ItemsPerReward)	//This case should ALWAYS be true with this gump, jsut a sanity check
                m_Collector.SayTo(pm, 1070981, string.Format("{0}\t{1}", turnIns, TreasuresOfTokuno.ItemsPerReward)); // You have turned in ~1_COUNT~ minor artifacts. Turn in ~2_NUM~ to receive a reward.
            else
                m_Collector.SayTo(pm, 1070982); // When you wish to choose your reward, you have but to approach me again.
        }
    }
}

namespace Server.Gumps
{
    public class ToTRedeemGump : BaseImageTileButtonsGump
    {
        public class TypeTileButtonInfo : ImageTileButtonInfo
        {
            private readonly Type m_Type;

            public Type Type => m_Type;

            public TypeTileButtonInfo(Type type, int itemID, int hue, TextDefinition label, int localizedToolTip)
                : base(itemID, hue, label, localizedToolTip)
            {
                m_Type = type;
            }

            public TypeTileButtonInfo(Type type, int itemID, TextDefinition label)
                : this(type, itemID, 0, label, -1)
            {
            }

            public TypeTileButtonInfo(Type type, int itemID, TextDefinition label, int localizedToolTip)
                : this(type, itemID, 0, label, localizedToolTip)
            {
            }
        }

        public class PigmentsTileButtonInfo : ImageTileButtonInfo
        {
            private PigmentType m_Pigment;

            public PigmentType Pigment
            {
                get
                {
                    return m_Pigment;
                }

                set
                {
                    m_Pigment = value;
                }
            }

            public PigmentsTileButtonInfo(PigmentType p)
                : base(0xEFF, PigmentsOfTokuno.GetInfo(p)[0], PigmentsOfTokuno.GetInfo(p)[1])
            {
                m_Pigment = p;
            }
        }

        #region ToT Normal Rewards Table
        private static readonly TypeTileButtonInfo[][] m_NormalRewards = new TypeTileButtonInfo[][]
        {
            // ToT One Rewards
            new TypeTileButtonInfo[]
            {
                new TypeTileButtonInfo(typeof(SwordsOfProsperity), 0x27A9, 1070963, 1071002),
                new TypeTileButtonInfo(typeof(SwordOfTheStampede), 0x27A2, 1070964, 1070978),
                new TypeTileButtonInfo(typeof(WindsEdge), 0x27A3, 1070965, 1071003),
                new TypeTileButtonInfo(typeof(DarkenedSky), 0x27AD, 1070966, 1071004),
                new TypeTileButtonInfo(typeof(TheHorselord), 0x27A5, 1070967, 1071005),
                new TypeTileButtonInfo(typeof(RuneBeetleCarapace), 0x277D, 1070968, 1071006),
                new TypeTileButtonInfo(typeof(KasaOfTheRajin), 0x2798, 1070969, 1071007),
                new TypeTileButtonInfo(typeof(Stormgrip), 0x2792, 1070970, 1071008),
                new TypeTileButtonInfo(typeof(TomeOfLostKnowledge), 0x0EFA, 0x530, 1070971, 1071009),
                new TypeTileButtonInfo(typeof(PigmentsOfTokuno), 0x0EFF, 1070933, 1071011)
            },
            // ToT Two Rewards
            new TypeTileButtonInfo[]
            {
                new TypeTileButtonInfo(typeof(SwordsOfProsperity), 0x27A9, 1070963, 1071002),
                new TypeTileButtonInfo(typeof(SwordOfTheStampede), 0x27A2, 1070964, 1070978),
                new TypeTileButtonInfo(typeof(WindsEdge), 0x27A3, 1070965, 1071003),
                new TypeTileButtonInfo(typeof(DarkenedSky), 0x27AD, 1070966, 1071004),
                new TypeTileButtonInfo(typeof(TheHorselord), 0x27A5, 1070967, 1071005),
                new TypeTileButtonInfo(typeof(RuneBeetleCarapace), 0x277D, 1070968, 1071006),
                new TypeTileButtonInfo(typeof(KasaOfTheRajin), 0x2798, 1070969, 1071007),
                new TypeTileButtonInfo(typeof(Stormgrip), 0x2792, 1070970, 1071008),
                new TypeTileButtonInfo(typeof(TomeOfLostKnowledge), 0x0EFA, 0x530, 1070971, 1071009),
                new TypeTileButtonInfo(typeof(PigmentsOfTokuno), 0x0EFF, 1070933, 1071011)
            },
            // ToT Three Rewards
            new TypeTileButtonInfo[]
            {
                new TypeTileButtonInfo(typeof(SwordsOfProsperity), 0x27A9, 1070963, 1071002),
                new TypeTileButtonInfo(typeof(SwordOfTheStampede), 0x27A2, 1070964, 1070978),
                new TypeTileButtonInfo(typeof(WindsEdge), 0x27A3, 1070965, 1071003),
                new TypeTileButtonInfo(typeof(DarkenedSky), 0x27AD, 1070966, 1071004),
                new TypeTileButtonInfo(typeof(TheHorselord), 0x27A5, 1070967, 1071005),
                new TypeTileButtonInfo(typeof(RuneBeetleCarapace), 0x277D, 1070968, 1071006),
                new TypeTileButtonInfo(typeof(KasaOfTheRajin), 0x2798, 1070969, 1071007),
                new TypeTileButtonInfo(typeof(Stormgrip), 0x2792, 1070970, 1071008),
                new TypeTileButtonInfo(typeof(TomeOfLostKnowledge), 0x0EFA, 0x530, 1070971, 1071009)
            }
        };
        #endregion

        public static TypeTileButtonInfo[][] NormalRewards => m_NormalRewards;

        #region ToT Pigment Rewards Table
        private static readonly PigmentsTileButtonInfo[][] m_PigmentRewards = new PigmentsTileButtonInfo[][]
        {
            // ToT One Pigment Rewards
            new PigmentsTileButtonInfo[]
            {
                new PigmentsTileButtonInfo(PigmentType.ParagonGold),
                new PigmentsTileButtonInfo(PigmentType.VioletCouragePurple),
                new PigmentsTileButtonInfo(PigmentType.InvulnerabilityBlue),
                new PigmentsTileButtonInfo(PigmentType.LunaWhite),
                new PigmentsTileButtonInfo(PigmentType.DryadGreen),
                new PigmentsTileButtonInfo(PigmentType.ShadowDancerBlack),
                new PigmentsTileButtonInfo(PigmentType.BerserkerRed),
                new PigmentsTileButtonInfo(PigmentType.NoxGreen),
                new PigmentsTileButtonInfo(PigmentType.RumRed),
                new PigmentsTileButtonInfo(PigmentType.FireOrange)
            },
            // ToT Two Pigment Rewards
            new PigmentsTileButtonInfo[]
            {
                new PigmentsTileButtonInfo(PigmentType.FadedCoal),
                new PigmentsTileButtonInfo(PigmentType.Coal),
                new PigmentsTileButtonInfo(PigmentType.FadedGold),
                new PigmentsTileButtonInfo(PigmentType.StormBronze),
                new PigmentsTileButtonInfo(PigmentType.Rose),
                new PigmentsTileButtonInfo(PigmentType.MidnightCoal),
                new PigmentsTileButtonInfo(PigmentType.FadedBronze),
                new PigmentsTileButtonInfo(PigmentType.FadedRose),
                new PigmentsTileButtonInfo(PigmentType.DeepRose)
            },
            // ToT Three Pigment Rewards
            new PigmentsTileButtonInfo[]
            {
                new PigmentsTileButtonInfo(PigmentType.ParagonGold),
                new PigmentsTileButtonInfo(PigmentType.VioletCouragePurple),
                new PigmentsTileButtonInfo(PigmentType.InvulnerabilityBlue),
                new PigmentsTileButtonInfo(PigmentType.LunaWhite),
                new PigmentsTileButtonInfo(PigmentType.DryadGreen),
                new PigmentsTileButtonInfo(PigmentType.ShadowDancerBlack),
                new PigmentsTileButtonInfo(PigmentType.BerserkerRed),
                new PigmentsTileButtonInfo(PigmentType.NoxGreen),
                new PigmentsTileButtonInfo(PigmentType.RumRed),
                new PigmentsTileButtonInfo(PigmentType.FireOrange)
            }
        };
        #endregion

        public static PigmentsTileButtonInfo[][] PigmentRewards => m_PigmentRewards;

        private readonly Mobile m_Collector;

        public ToTRedeemGump(Mobile collector, bool pigments)
            : base(pigments ? 1070986 : 1070985, pigments ? m_PigmentRewards[(int)TreasuresOfTokuno.RewardEra - 1] : (ImageTileButtonInfo[])m_NormalRewards[(int)TreasuresOfTokuno.RewardEra - 1])
        {
            m_Collector = collector;
        }

        public override void HandleButtonResponse(NetState sender, int adjustedButton, ImageTileButtonInfo buttonInfo)
        {
            PlayerMobile pm = sender.Mobile as PlayerMobile;
            int turnIns = PointsSystem.TreasuresOfTokuno.GetTurnIns(pm);

            if (pm == null || !pm.InRange(m_Collector.Location, 7) || !(turnIns >= TreasuresOfTokuno.ItemsPerReward))
                return;

            bool pigments = (buttonInfo is PigmentsTileButtonInfo);

            Item item = null;

            if (pigments)
            {
                PigmentsTileButtonInfo p = buttonInfo as PigmentsTileButtonInfo;

                item = new PigmentsOfTokuno(p.Pigment);
            }
            else
            {
                TypeTileButtonInfo t = buttonInfo as TypeTileButtonInfo;

                if (t.Type == typeof(PigmentsOfTokuno))	//Special case of course.
                {
                    pm.CloseGump(typeof(ToTTurnInGump));	//Sanity
                    pm.CloseGump(typeof(ToTRedeemGump));

                    pm.SendGump(new ToTRedeemGump(m_Collector, true));

                    return;
                }

                try
                {
                    item = (Item)Activator.CreateInstance(t.Type);
                }
                catch
                {
                }
            }

            if (item == null)
                return; //Sanity

            if (pm.AddToBackpack(item))
            {
                PointsSystem.TreasuresOfTokuno.RemoveTurnIns(pm, TreasuresOfTokuno.ItemsPerReward);
                m_Collector.SayTo(pm, 1070984, (item.Name == null || item.Name.Length <= 0) ? string.Format("#{0}", item.LabelNumber) : item.Name); // You have earned the gratitude of the Empire. I have placed the ~1_OBJTYPE~ in your backpack.
            }
            else
            {
                item.Delete();
                m_Collector.SayTo(pm, 500722); // You don't have enough room in your backpack!
                m_Collector.SayTo(pm, 1070982); // When you wish to choose your reward, you have but to approach me again.
            }
        }

        public override void HandleCancel(NetState sender)
        {
            PlayerMobile pm = sender.Mobile as PlayerMobile;

            if (pm == null || !pm.InRange(m_Collector.Location, 7))
                return;

            int turnIns = PointsSystem.TreasuresOfTokuno.GetTurnIns(pm);

            if (turnIns == 0)
                m_Collector.SayTo(pm, 1071013); // Bring me 10 of the lost treasures of Tokuno and I will reward you with a valuable item.
            else if (turnIns < TreasuresOfTokuno.ItemsPerReward)	//This and above case should ALWAYS be FALSE with this gump, jsut a sanity check
                m_Collector.SayTo(pm, 1070981, string.Format("{0}\t{1}", turnIns, TreasuresOfTokuno.ItemsPerReward)); // You have turned in ~1_COUNT~ minor artifacts. Turn in ~2_NUM~ to receive a reward.
            else
                m_Collector.SayTo(pm, 1070982); // When you wish to choose your reward, you have but to approach me again.
        }
    }
}

/* Notes

Pigments of tokuno do NOT check for if item is already hued 0;  APPARENTLY he still accepts it if it's < 10 charges.

Chest of Heirlooms don't show if unlocked.

Chest of heirlooms, locked, HARD to pick at 100 lock picking but not impossible.  had 95 health to 0, cause it's trapped >< (explosion i think)
*/
