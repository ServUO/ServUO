using Server.Commands;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using System;
using System.Text;
using Server.Engines.Points;
using Server.SkillHandlers;
using Server.Spells.SkillMasteries;
using Server.Engines.Craft;
using Server.Engines.Plants;
using Server.Accounting;

namespace Server.Misc
{
    public class TestCenter
    {
        public static bool Enabled { get; set; }

        static TestCenter()
        {
            Enabled = Config.Get("TestCenter.Enabled", false);
        }

        public static void Initialize()
        {
            if (Enabled)
                EventSink.Speech += EventSink_Speech;
        }

        private static void EventSink_Speech(SpeechEventArgs args)
        {
            if (!args.Handled)
            {
                if (Insensitive.StartsWith(args.Speech, "set"))
                {
                    Mobile from = args.Mobile;

                    string[] split = args.Speech.Split(' ');

                    if (split.Length == 3)
                    {
                        try
                        {
                            string name = split[1];
                            double value = Convert.ToDouble(split[2]);

                            if (Insensitive.Equals(name, "str"))
                                ChangeStrength(from, (int)value);
                            else if (Insensitive.Equals(name, "dex"))
                                ChangeDexterity(from, (int)value);
                            else if (Insensitive.Equals(name, "int"))
                                ChangeIntelligence(from, (int)value);
                            else
                                ChangeSkill(from, name, value);
                        }
                        catch (Exception e)
                        {
                            Diagnostics.ExceptionLogging.LogException(e);
                        }
                    }
                }
                else if(Insensitive.StartsWith(args.Speech, "give"))
                {
                    Mobile from = args.Mobile;

                    string[] split = args.Speech.Split(' ');

                    if (split.Length == 2)
                    {
                        string name = split[1];

                        if (Insensitive.Equals(name, "resources"))
                        {
                            if (CanGive(from, "Resources"))
                            {
                                GiveResources(from);
                                from.SendMessage("Resources have been added to your bank");
                            }
                        }
                        else if (Insensitive.Equals(name, "arties"))
                        {
                            if (CanGive(from, "Artifacts"))
                            {
                                GiveArtifacts(from);
                                from.SendMessage("Artifacts have been added to your bank");
                            }
                        }
                        else if (Insensitive.Equals(name, "air"))
                        {
                            if (CanGive(from, "Air"))
                            {
                                GiveAirFreshner(from);
                                from.SendMessage("Air Freshner has been added to your bank.");
                            }
                        }
                        else if (Insensitive.Equals(name, "seeds"))
                        {
                            if (CanGive(from, "Seeds"))
                            {
                                GiveSeeds(from);
                                from.SendMessage("Seeds have been added to your bank.");
                            }
                        }
                        else if (Insensitive.Equals(name, "tokens"))
                        {
                            if (CanGive(from, "Tokens"))
                            {
                                GiveTokens(from);
                            }
                        }
                        else if (Insensitive.Equals(name, "masteries"))
                        {
                            if (CanGive(from, "Masteries"))
                            {
                                GiveMasteries(from);
                                from.SendMessage("Masteries have been added to your bank.");
                            }
                        }
                    }
                }
                else if (Insensitive.Equals(args.Speech, "help"))
                {
                    args.Mobile.SendGump(new TCHelpGump());

                    args.Handled = true;
                }
            }
        }

        private static bool CanGive(Mobile m, string tagName)
        {
            Account a = m.Account as Account;

            if (a != null)
            {
                var tag = a.GetTag(m.Serial.ToString() + ' ' + tagName);

                if (tag == null)
                {
                    a.AddTag(m.Serial.ToString() + ' ' + tagName, DateTime.Now.ToString());
                    return true;
                }
            }

            return false;
        }

        private static void ChangeStrength(Mobile from, int value)
        {
            if (value < 10 || value > 125)
            {
                from.SendLocalizedMessage(1005628); // Stats range between 10 and 125.
            }
            else
            {
                if ((value + from.RawDex + from.RawInt) > from.StatCap)
                {
                    from.SendLocalizedMessage(1005629); // You can not exceed the stat cap.  Try setting another stat lower first.
                }
                else
                {
                    from.RawStr = value;
                    from.SendLocalizedMessage(1005630); // Your stats have been adjusted.
                }
            }
        }

        private static void ChangeDexterity(Mobile from, int value)
        {
            if (value < 10 || value > 125)
            {
                from.SendLocalizedMessage(1005628); // Stats range between 10 and 125.
            }
            else
            {
                if ((from.RawStr + value + from.RawInt) > from.StatCap)
                {
                    from.SendLocalizedMessage(1005629); // You can not exceed the stat cap.  Try setting another stat lower first.
                }
                else
                {
                    from.RawDex = value;
                    from.SendLocalizedMessage(1005630); // Your stats have been adjusted.
                }
            }
        }

        private static void ChangeIntelligence(Mobile from, int value)
        {
            if (value < 10 || value > 125)
            {
                from.SendLocalizedMessage(1005628); // Stats range between 10 and 125.
            }
            else
            {
                if ((from.RawStr + from.RawDex + value) > from.StatCap)
                {
                    from.SendLocalizedMessage(1005629); // You can not exceed the stat cap.  Try setting another stat lower first.
                }
                else
                {
                    from.RawInt = value;
                    from.SendLocalizedMessage(1005630); // Your stats have been adjusted.
                }
            }
        }

        private static void ChangeSkill(Mobile from, string name, double value)
        {
            SkillName index;

            if (!Enum.TryParse(name, true, out index))
            {
                from.SendLocalizedMessage(1005631); // You have specified an invalid skill to set.
                return;
            }

            Skill skill = from.Skills[index];

            if (skill != null)
            {
                if (value < 0 || value > skill.Cap)
                {
                    from.SendMessage(string.Format("Your skill in {0} is capped at {1:F1}.", skill.Info.Name, skill.Cap));
                }
                else
                {
                    int newFixedPoint = (int)(value * 10.0);
                    int oldFixedPoint = skill.BaseFixedPoint;

                    if (((skill.Owner.Total - oldFixedPoint) + newFixedPoint) > skill.Owner.Cap)
                    {
                        from.SendMessage("You can not exceed the skill cap.  Try setting another skill lower first.");
                    }
                    else
                    {
                        skill.BaseFixedPoint = newFixedPoint;
                    }
                }
            }
            else
            {
                from.SendLocalizedMessage(1005631); // You have specified an invalid skill to set.
            }
        }

        public static void GiveResources(Mobile from)
        {
            var box = new WoodenBox
            {
                Hue = 1193,
                Name = "General Resources"
            };

            PlaceItemIn(box, 115, 63, new PowderOfTemperament(30000));

            Container bag = new Bag
            {
                Hue = 75,
                Name = "Bag of Imbuing Materials"
            };

            for (int i = 0; i < Imbuing.IngredTypes.Length; i++)
            {
                var item = Loot.Construct(Imbuing.IngredTypes[i]);

                if (item != null)
                {
                    if (item.Stackable)
                    {
                        item.Amount = 1000;
                        bag.DropItem(item);
                    }
                    else
                    {
                        bag.DropItem(item);

                        for (int j = 0; j < 10; j++)
                        {
                            bag.DropItem(Loot.Construct(Imbuing.IngredTypes[j]));
                        }
                    }
                }
            }

            PlaceItemIn(box, 17, 67, bag);

            bag = new Bag
            {
                Hue = 1195,
                Name = "Bag of Elven Materials"
            };

            for (int i = 0; i < Loot.RareGemTypes.Length; i++)
            {
                var item = Loot.Construct(Loot.RareGemTypes[i]);
                item.Amount = 200;

                bag.DropItem(item);
            }

            bag.DropItem(new LardOfParoxysmus(200));
            bag.DropItem(new CapturedEssence(200));
            bag.DropItem(new LuminescentFungi(200));
            bag.DropItem(new Putrefaction(200));
            bag.DropItem(new Blight(200));
            bag.DropItem(new LardOfParoxysmus(200));
            bag.DropItem(new Taint(200));
            bag.DropItem(new Corruption(200));
            bag.DropItem(new BarkFragment(200));
            bag.DropItem(new Corruption(200));
            bag.DropItem(new ParasiticPlant(200));
            bag.DropItem(new Muculent(200));
            bag.DropItem(new PristineDreadHorn(200));
            bag.DropItem(new EyeOfTheTravesty(200));
            bag.DropItem(new GrizzledBones(200));
            bag.DropItem(new Scourge(200));

            PlaceItemIn(box, 40, 67, bag);

            bag = new Backpack
            {
                Name = "Runic Tool Bag"
            };

            PlaceItemIn(bag, 54, 74, new RunicHammer(CraftResource.DullCopper, 30000));
            PlaceItemIn(bag, 64, 74, new RunicHammer(CraftResource.ShadowIron, 30000));
            PlaceItemIn(bag, 74, 74, new RunicHammer(CraftResource.Copper, 30000));
            PlaceItemIn(bag, 84, 74, new RunicHammer(CraftResource.Bronze, 30000));
            PlaceItemIn(bag, 94, 74, new RunicHammer(CraftResource.Gold, 30000));
            PlaceItemIn(bag, 104, 74, new RunicHammer(CraftResource.Agapite, 30000));
            PlaceItemIn(bag, 114, 74, new RunicHammer(CraftResource.Verite, 30000));
            PlaceItemIn(bag, 124, 74, new RunicHammer(CraftResource.Valorite, 30000));

            PlaceItemIn(bag, 54, 90, new RunicSewingKit(CraftResource.SpinedLeather, 30000));
            PlaceItemIn(bag, 64, 90, new RunicSewingKit(CraftResource.HornedLeather, 30000));
            PlaceItemIn(bag, 64, 90, new RunicSewingKit(CraftResource.BarbedLeather, 30000));

            PlaceItemIn(bag, 74, 90, new RunicDovetailSaw(CraftResource.OakWood, 30000));
            PlaceItemIn(bag, 84, 90, new RunicDovetailSaw(CraftResource.AshWood, 30000));
            PlaceItemIn(bag, 94, 90, new RunicDovetailSaw(CraftResource.YewWood, 30000));
            PlaceItemIn(bag, 104, 90, new RunicDovetailSaw(CraftResource.Heartwood, 30000));

            PlaceItemIn(bag, 54, 107, new RunicFletcherTool(CraftResource.OakWood, 30000));
            PlaceItemIn(bag, 64, 107, new RunicFletcherTool(CraftResource.AshWood, 30000));
            PlaceItemIn(bag, 74, 107, new RunicFletcherTool(CraftResource.YewWood, 30000));
            PlaceItemIn(bag, 84, 107, new RunicFletcherTool(CraftResource.Heartwood, 30000));

            PlaceItemIn(box, 65, 67, bag);

            bag = new Bag
            {
                Name = "Raw Materials Bag"
            };

            PlaceItemIn(bag, 92, 59, new BarbedLeather(5000));
            PlaceItemIn(bag, 92, 68, new HornedLeather(5000));
            PlaceItemIn(bag, 92, 76, new SpinedLeather(5000));
            PlaceItemIn(bag, 92, 84, new Leather(5000));

            PlaceItemIn(bag, 30, 118, new Cloth(5000));
            PlaceItemIn(bag, 30, 84, new Board(5000));
            PlaceItemIn(bag, 57, 80, new BlankScroll(500));

            PlaceItemIn(bag, 30, 35, new DullCopperIngot(5000));
            PlaceItemIn(bag, 37, 35, new ShadowIronIngot(5000));
            PlaceItemIn(bag, 44, 35, new CopperIngot(5000));
            PlaceItemIn(bag, 51, 35, new BronzeIngot(5000));
            PlaceItemIn(bag, 58, 35, new GoldIngot(5000));
            PlaceItemIn(bag, 65, 35, new AgapiteIngot(5000));
            PlaceItemIn(bag, 72, 35, new VeriteIngot(5000));
            PlaceItemIn(bag, 79, 35, new ValoriteIngot(5000));
            PlaceItemIn(bag, 86, 35, new IronIngot(5000));

            PlaceItemIn(bag, 30, 59, new RedScales(5000));
            PlaceItemIn(bag, 36, 59, new YellowScales(5000));
            PlaceItemIn(bag, 42, 59, new BlackScales(5000));
            PlaceItemIn(bag, 48, 59, new GreenScales(5000));
            PlaceItemIn(bag, 54, 59, new WhiteScales(5000));
            PlaceItemIn(bag, 60, 59, new BlueScales(5000));

            PlaceItemIn(box, 40, 93, bag);

            bag = new Bag
            {
                Name = "Bag of Archery Ammo"
            };

            PlaceItemIn(bag, 48, 76, new Arrow(5000));
            PlaceItemIn(bag, 72, 76, new Bolt(5000));

            PlaceItemIn(box, 65, 93, bag);

            bag = new Bag
            {
                Name = "Tool Bag"
            };

            PlaceItemIn(bag, 30, 35, new TinkerTools(30000));
            PlaceItemIn(bag, 60, 35, new HousePlacementTool());
            PlaceItemIn(bag, 90, 35, new DovetailSaw(30000));
            PlaceItemIn(bag, 30, 68, new Scissors());
            PlaceItemIn(bag, 45, 68, new MortarPestle(30000));
            PlaceItemIn(bag, 75, 68, new ScribesPen(30000));
            PlaceItemIn(bag, 90, 68, new SmithHammer(30000));
            PlaceItemIn(bag, 30, 118, new TwoHandedAxe());
            PlaceItemIn(bag, 60, 118, new FletcherTools(30000));
            PlaceItemIn(bag, 90, 118, new SewingKit(30000));
            PlaceItemIn(bag, 70, 85, new Clippers(30000));
            PlaceItemIn(bag, 61, 76, new MalletAndChisel(30000));

            PlaceItemIn(box, 90, 93, bag);

            bag = new Bag
            {
                Name = "Bag of Recipes",
                Hue = 2301
            };

            foreach (var recipe in Recipe.Recipes.Values)
            {
                bag.DropItem(new RecipeScroll(recipe));
            }

            PlaceItemIn(box, 115, 93, bag);

            bag = new Bag
            {
                Name = "Bag of Wood",
                Hue = 1321
            };

            bag.DropItem(new Board(5000));
            bag.DropItem(new OakBoard(5000));
            bag.DropItem(new AshBoard(5000));
            bag.DropItem(new YewBoard(5000));
            bag.DropItem(new OakBoard(5000));
            bag.DropItem(new BloodwoodBoard(5000));
            bag.DropItem(new HeartwoodBoard(5000));
            bag.DropItem(new FrostwoodBoard(5000));

            PlaceItemIn(box, 139, 93, bag);

            PlaceItemIn(from.BankBox, 88, 142, box);
        }

        public static void GiveArtifacts(Mobile from)
        {
            var box = new WoodenBox
            {
                Hue = 1170,
                Name = "Artifacts"
            };

            Container bag = new Bag
            {
                Hue = 2075,
                Name = "SA Major Artifacts Human"
            };

            bag.DropItem(new AnimatedLegsoftheInsaneTinker());
            bag.DropItem(new ResonantStaffofEnlightenment());
            bag.DropItem(new JadeWarAxe());
            bag.DropItem(new DemonHuntersStandard());
            bag.DropItem(new WallOfHungryMouths());
            bag.DropItem(new HumanSignOfChaos());
            bag.DropItem(new GargishSignOfChaos());
            bag.DropItem(new IronwoodCompositeBow());
            bag.DropItem(new ClawsOfTheBerserker());
            bag.DropItem(new StandardOfChaos());
            bag.DropItem(new DefenderOfTheMagus());
            bag.DropItem(new TheImpalersPick());
            bag.DropItem(new CavalrysFolly());
            bag.DropItem(new AxeOfAbandon());
            bag.DropItem(new ProtectoroftheBattleMage());
            bag.DropItem(new FallenMysticsSpellbook());
            bag.DropItem(new CrownOfArcaneTemperament());
            bag.DropItem(new VampiricEssence());

            PlaceItemIn(box, 17, 57, bag);

            bag = new Bag
            {
                Hue = 1159,
                Name = "Eodon Artifacts"
            };

            bag.DropItem(new AnonsBoots());
            bag.DropItem(new AnonsBootsGargoyle());
            bag.DropItem(new AnonsSpellbook());
            bag.DropItem(new BalakaisShamanStaff());
            bag.DropItem(new BalakaisShamanStaffGargoyle());
            bag.DropItem(new EnchantressCameo());
            bag.DropItem(new GrugorsShield());
            bag.DropItem(new GrugorsShieldGargoyle());
            bag.DropItem(new HalawasHuntingBow());
            bag.DropItem(new HalawasHuntingBowGargoyle());
            bag.DropItem(new HawkwindsRobe());
            bag.DropItem(new JumusSacredHide());
            bag.DropItem(new JumusSacredHideGargoyle());
            bag.DropItem(new JuonarsGrimoire());
            bag.DropItem(new LereisHuntingSpear());
            bag.DropItem(new LereisHuntingSpearGargoyle());
            bag.DropItem(new MinaxsSandles());
            bag.DropItem(new MinaxsSandlesGargoyle());
            bag.DropItem(new OzymandiasObi());
            bag.DropItem(new OzymandiasObiGargoyle());
            bag.DropItem(new ShantysWaders());
            bag.DropItem(new ShantysWadersGargoyle());
            bag.DropItem(new TotemOfTheTribe());
            bag.DropItem(new WamapsBoneEarrings());
            bag.DropItem(new WamapsBoneEarringsGargoyle());
            bag.DropItem(new UnstableTimeRift());
            bag.DropItem(new MocapotlsObsidianSword());

            PlaceItemIn(box, 40, 57, bag);

            bag = new Bag
            {
                Hue = 1266,
                Name = "Major Artifacts"
            };

            bag.DropItem(new TitansHammer());
            bag.DropItem(new InquisitorsResolution());
            bag.DropItem(new BladeOfTheRighteous());
            bag.DropItem(new ZyronicClaw());

            for (int i = 0; i < DoomGauntlet.DoomArtifacts.Length; i++)
            {
                bag.DropItem(Loot.Construct(DoomGauntlet.DoomArtifacts[i]));
            }

            PlaceItemIn(box, 65, 57, bag);

            bag = new Bag
            {
                Hue = 1281,
                Name = "Tokuno Major Artifacts"
            };

            bag.DropItem(new SwordsOfProsperity());
            bag.DropItem(new SwordOfTheStampede());
            bag.DropItem(new WindsEdge());
            bag.DropItem(new DarkenedSky());
            bag.DropItem(new RuneBeetleCarapace());
            bag.DropItem(new KasaOfTheRajin());
            bag.DropItem(new Stormgrip());
            bag.DropItem(new TomeOfLostKnowledge());
            bag.DropItem(new PigmentsOfTokuno(PigmentType.ParagonGold));
            bag.DropItem(new PigmentsOfTokuno(PigmentType.VioletCouragePurple));
            bag.DropItem(new PigmentsOfTokuno(PigmentType.InvulnerabilityBlue));
            bag.DropItem(new PigmentsOfTokuno(PigmentType.LunaWhite));
            bag.DropItem(new PigmentsOfTokuno(PigmentType.DryadGreen));
            bag.DropItem(new PigmentsOfTokuno(PigmentType.ShadowDancerBlack));
            bag.DropItem(new PigmentsOfTokuno(PigmentType.BerserkerRed));
            bag.DropItem(new PigmentsOfTokuno(PigmentType.NoxGreen));
            bag.DropItem(new PigmentsOfTokuno(PigmentType.RumRed));
            bag.DropItem(new PigmentsOfTokuno(PigmentType.FireOrange));
            bag.DropItem(new PigmentsOfTokuno(PigmentType.FadedCoal));
            bag.DropItem(new PigmentsOfTokuno(PigmentType.Coal));
            bag.DropItem(new PigmentsOfTokuno(PigmentType.FadedGold));
            bag.DropItem(new PigmentsOfTokuno(PigmentType.StormBronze));
            bag.DropItem(new PigmentsOfTokuno(PigmentType.Rose));
            bag.DropItem(new PigmentsOfTokuno(PigmentType.MidnightCoal));
            bag.DropItem(new PigmentsOfTokuno(PigmentType.FadedBronze));
            bag.DropItem(new PigmentsOfTokuno(PigmentType.FadedRose));
            bag.DropItem(new PigmentsOfTokuno(PigmentType.DeepRose));

            PlaceItemIn(box, 115, 57, bag);

            bag = new Bag
            {
                Hue = 1167,
                Name = "Minor Artifacts"
            };

            for (int i = 0; i < MondainsLegacy.Artifacts.Length; i++)
            {
                bag.DropItem(Loot.Construct(MondainsLegacy.Artifacts[i]));
            }

            PlaceItemIn(box, 90, 57, bag);

            bag = new Bag
            {
                Hue = 55,
                Name = "Replicas"
            };

            bag.DropItem(new RoyalGuardInvestigatorsCloak());
            bag.DropItem(new TongueOfTheBeast());
            bag.DropItem(new TheMostKnowledgePerson());
            bag.DropItem(new ShroudOfDeceit());
            bag.DropItem(new ANecromancerShroud());
            bag.DropItem(new LightsRampart());
            bag.DropItem(new AcidProofRobe());
            bag.DropItem(new ObiDiEnse());
            bag.DropItem(new TheRobeOfBritanniaAri());
            bag.DropItem(new GauntletsOfAnger());
            bag.DropItem(new JadeArmband());
            bag.DropItem(new Subdue());
            bag.DropItem(new CrownOfTalKeesh());
            bag.DropItem(new DjinnisRing());
            bag.DropItem(new EmbroideredOakLeafCloak());
            bag.DropItem(new GladiatorsCollar());
            bag.DropItem(new LieutenantOfTheBritannianRoyalGuard());
            bag.DropItem(new CaptainJohnsHat());
            bag.DropItem(new BraveKnightOfTheBritannia());
            bag.DropItem(new Pacify());
            bag.DropItem(new OblivionsNeedle());
            bag.DropItem(new RoyalGuardSurvivalKnife());
            bag.DropItem(new Quell());
            bag.DropItem(new Calm());
            bag.DropItem(new OrcChieftainHelm());
            bag.DropItem(new FangOfRactus());
            bag.DropItem(new DetectiveBoots());

            PlaceItemIn(box, 90, 139, bag);

            bag = new Bag
            {
                Hue = 2731,
                Name = "Doom Upgrade Arties"
            };

            bag.DropItem(new BritchesOfWarding());
            bag.DropItem(new BowOfTheInfiniteSwarm());
            bag.DropItem(new GlovesOfFeudalGrip());
            bag.DropItem(new Glenda());
            bag.DropItem(new CuffsOfTheArchmage());
            bag.DropItem(new TheScholarsHalo());
            bag.DropItem(new TheDeceiver());
            bag.DropItem(new BraceletOfPrimalConsumption());

            PlaceItemIn(box, 17, 83, bag);
            PlaceItemIn(from.BankBox, 63, 106, box);
        }

        public static void GiveAirFreshner(Mobile from)
        {
        }

        public static void GiveSeeds(Mobile from)
        {
            var box = new WoodenBox
            {
                Hue = 578,
                Name = "Box of Seeds"
            };

            box.DropItem(new FertileDirt(5000));
            box.DropItem(new GreenThorns(15));

            Container bag = new Bag();

            for (int i = 0; i < 10; i++)
            {
                bag.DropItem(new Seed(PlantType.CocoaTree, PlantHue.Plain, false));
            }

            PlaceItemIn(box, 47, 83, bag);

            bag = new Bag();

            for (int i = 0; i < 10; i++)
            {
                bag.DropItem(Seed.RandomPeculiarSeed(Utility.Random(3)));
            }

            PlaceItemIn(box, 78, 83, bag);

            bag = new Bag();

            for (int i = 0; i < 5; i++)
            {
                bag.DropItem(new Seed(PlantTypeInfo.RandomFirstGeneration(), PlantHue.White, false));
            }

            for (int i = 0; i < 5; i++)
            {
                bag.DropItem(new Seed(PlantTypeInfo.RandomFirstGeneration(), PlantHue.Black, false));
            }

            for (int i = 0; i < 5; i++)
            {
                bag.DropItem(new Seed(PlantTypeInfo.RandomFirstGeneration(), PlantHue.FireRed, false));
            }

            // TODO: Plant Spawners

            PlaceItemIn(box, 109, 83, bag);

            PlaceItemIn(from.BankBox, 83, 106, box);
        }

        public static void GiveTokens(Mobile from)
        {
            from.SendLocalizedMessage(1075549); // A token has been placed in your backpack. Double-click it to redeem your promotion.

            for (int i = 0; i < 10; i++)
            {
                from.AddToBackpack(new HeritageToken());
            }

            from.AddToBackpack(new AnniversaryPromotionalToken(AnniversaryType.ShadowItems));
            from.AddToBackpack(new AnniversaryPromotionalToken(AnniversaryType.CrystalItems));
            from.AddToBackpack(new PersonalAttendantToken());
        }

        public static void GiveMasteries(Mobile from)
        {
            var backpack = new Backpack
            {
                Hue = 1154,
                Name = "Skill Masteries"
            };

            Bag bag = null;

            for (int i = 0; i < MasteryInfo.Skills.Length; i++)
            {
                var skill = MasteryInfo.Skills[i];

                bag = new Bag
                {
                    Name = string.Format("{0} Mastery", SkillInfo.Table[(int)skill].Name)
                };

                for (int j = 1; j <= 3; j++)
                {
                    bag.DropItem(new SkillMasteryPrimer(skill, j));
                }

                backpack.DropItem(bag);
            }

            PlaceItemIn(backpack, 83, 106, new BookOfMasteries());
            PlaceItemIn(from.BankBox, 103, 106, backpack);
        }

        private static void PlaceItemIn(Container parent, int x, int y, Item item)
        {
            parent.AddItem(item);
            item.Location = new Point3D(x, y, 0);
        }

        private static Item MakePotionKeg(PotionEffect type, int hue)
        {
            PotionKeg keg = new PotionKeg
            {
                Held = 100,
                Type = type,
                Hue = hue
            };

            return keg;
        }

        public static void FillBank(Mobile m)
        {
            BankBox bank = m.BankBox;

            for (int i = 0; i < PowerScroll.Skills.Count; ++i)
                m.Skills[PowerScroll.Skills[i]].Cap = 120.0;

            m.StatCap = 250;

            var book = new Runebook(9999);
            book.CurCharges = book.MaxCharges;
            book.Entries.Add(new RunebookEntry(new Point3D(1438, 1695, 0), Map.Trammel, "Britain Bank", null));
            book.Entries.Add(new RunebookEntry(new Point3D(1821, 2821, 0), Map.Trammel, "Trinsic Bank", null));
            book.Entries.Add(new RunebookEntry(new Point3D(1492, 1628, 13), Map.Trammel, "Britain Sweet Dreams", null));
            book.Entries.Add(new RunebookEntry(new Point3D(1388, 1507, 10), Map.Trammel, "Britain Graveyard", null));
            book.Entries.Add(new RunebookEntry(new Point3D(1300, 1080, 0), Map.Trammel, "Dungeon Despise", null));
            book.Entries.Add(new RunebookEntry(new Point3D(1171, 2639, 0), Map.Trammel, "Dungeon Destard", null));
            book.Entries.Add(new RunebookEntry(new Point3D(1260, 2296, 0), Map.Trammel, "Hedge Maze", null));

            m.AddToBackpack(book);

            #region Gold
            var account = m.Account as Account;

            if (account != null && account.GetTag("TCGold") == null)
            {
                account.AddTag("TCGold", "Gold Given");

                Banker.Deposit(m, 30000000, false);
            }
            #endregion

            #region Bank Level Items
            bank.DropItem(new Robe(443));
            bank.DropItem(new Dagger());
            bank.DropItem(new Candle());
            bank.DropItem(new FireworksWand() { Name = "Mininova Fireworks Wand" });
            #endregion

            Container cont;

            #region TMaps
            cont = new Bag
            {
                Name = "Bag of Treasure Maps"
            };

            for (int i = 0; i < 10; i++)
            {
                PlaceItemIn(cont, 30 + (i * 6), 35, new TreasureMap(Utility.Random(3), Map.Trammel));
            }

            for (int i = 0; i < 10; i++)
            {
                PlaceItemIn(cont, 30 + (i * 6), 51, new TreasureMap(Utility.Random(3), Map.Trammel));
            }

            for (int i = 0; i < 20; i++)
            {
                PlaceItemIn(cont, 76, 91, new MessageInABottle());
            }

            PlaceItemIn(cont, 22, 77, new Shovel(30000));
            PlaceItemIn(cont, 57, 97, new Lockpick(3));

            PlaceItemIn(bank, 98, 124, cont);
            #endregion

            #region Trans Powder
            cont = new Bag();

            PlaceItemIn(cont, 117, 147, new PowderOfTranslocation(100));
            PlaceItemIn(bank, 117, 147, cont);
            #endregion

            #region Magery Items
            cont = new WoodenBox
            {
                Hue = 1195,
                Name = "Magery Items"
            };

            PlaceItemIn(cont, 78, 88, new CrimsonCincture() { Hue = 232 });
            PlaceItemIn(cont, 102, 90, new CrystallineRing());

            var brac = new GoldBracelet
            {
                Name = "Farmer's Bank of Mastery"
            };
            brac.Attributes.CastRecovery = 3;
            brac.Attributes.CastSpeed = 1;
            PlaceItemIn(cont, 139, 30, brac);

            Container bag = new Backpack
            {
                Hue = 1152,
                Name = "Spell Casting Stuff"
            };

            PlaceItemIn(bag, 45, 107, new Spellbook(ulong.MaxValue));
            PlaceItemIn(bag, 65, 107, new NecromancerSpellbook((ulong)0xFFFF));
            PlaceItemIn(bag, 85, 107, new BookOfChivalry((ulong)0x3FF));
            PlaceItemIn(bag, 105, 107, new BookOfBushido());	//Default ctor = full
            PlaceItemIn(bag, 125, 107, new BookOfNinjitsu()); //Default ctor = full

            PlaceItemIn(bag, 102, 122, new SpellweavingBook((1ul << 16) - 1));
            PlaceItemIn(bag, 122, 122, new MysticBook((1ul << 16) - 1));

            Runebook runebook = new Runebook(20);
            runebook.CurCharges = runebook.MaxCharges;
            PlaceItemIn(bag, 145, 105, runebook);

            Item toHue = new BagOfReagents(5000)
            {
                Hue = 0x2D
            };
            PlaceItemIn(bag, 45, 128, toHue);

            toHue = new BagOfNecroReagents(3000)
            {
                Hue = 0x488
            };
            PlaceItemIn(bag, 64, 125, toHue);

            toHue = new BagOfMysticReagents(3000)
            {
                Hue = 1167
            };
            PlaceItemIn(bag, 141, 128, toHue);

            for (int i = 0; i < 9; ++i)
                PlaceItemIn(bag, 45 + (i * 10), 74, new RecallRune());

            PlaceItemIn(cont, 47, 91, bag);

            bag = new Backpack
            {
                Name = "Various Potion Kegs"
            };

            PlaceItemIn(bag, 45, 149, MakePotionKeg(PotionEffect.CureGreater, 0x2D));
            PlaceItemIn(bag, 69, 149, MakePotionKeg(PotionEffect.HealGreater, 0x499));
            PlaceItemIn(bag, 93, 149, MakePotionKeg(PotionEffect.PoisonDeadly, 0x46));
            PlaceItemIn(bag, 117, 149, MakePotionKeg(PotionEffect.RefreshTotal, 0x21));
            PlaceItemIn(bag, 141, 149, MakePotionKeg(PotionEffect.ExplosionGreater, 0x74));

            PlaceItemIn(bag, 93, 82, new Bottle(1000));

            PlaceItemIn(cont, 53, 169, bag);

            PlaceItemIn(bank, 63, 142, cont);
            #endregion

            #region Silver - No Mas Silver
            /*cont = new WoodenBox();
            cont.Hue = 1161;

            PlaceItemIn(cont, 47, 91, new Silver(9000));
            PlaceItemIn(bank, 38, 142, cont);*/
            #endregion

            #region Ethys
            cont = new Backpack
            {
                Hue = 0x490,
                Name = "Bag Of Ethy's!"
            };

            PlaceItemIn(cont, 45, 66, new EtherealHorse());
            PlaceItemIn(cont, 93, 99, new EtherealLlama());
            PlaceItemIn(cont, 45, 132, new EtherealUnicorn());
            PlaceItemIn(cont, 117, 99, new EtherealBeetle());

            PlaceItemIn(bank, 38, 124, cont);
            #endregion
        }

        public static void FillBankbox(Mobile m)
        {
            FillBank(m);
        }

        public class TCHelpGump : Gump
        {
            public TCHelpGump()
                : base(40, 40)
            {
                AddPage(0);
                AddBackground(0, 0, 160, 120, 5054);

                AddButton(10, 10, 0xFB7, 0xFB9, 1, GumpButtonType.Reply, 0);
                AddLabel(45, 10, 0x34, "ServUO");

                AddButton(10, 35, 0xFB7, 0xFB9, 2, GumpButtonType.Reply, 0);
                AddLabel(45, 35, 0x34, "List of skills");

                AddButton(10, 60, 0xFB7, 0xFB9, 3, GumpButtonType.Reply, 0);
                AddLabel(45, 60, 0x34, "Command list");

                AddButton(10, 85, 0xFB1, 0xFB3, 0, GumpButtonType.Reply, 0);
                AddLabel(45, 85, 0x34, "Close");
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                switch (info.ButtonID)
                {
                    case 1:
                        {
                            sender.LaunchBrowser("https://servuo.com");
                            break;
                        }
                    case 2: // List of skills
                        {
                            string[] strings = Enum.GetNames(typeof(SkillName));

                            Array.Sort(strings);

                            StringBuilder sb = new StringBuilder();

                            if (strings.Length > 0)
                                sb.Append(strings[0]);

                            for (int i = 1; i < strings.Length; ++i)
                            {
                                string v = strings[i];

                                if ((sb.Length + 1 + v.Length) >= 256)
                                {
                                    sender.Send(new AsciiMessage(Server.Serial.MinusOne, -1, MessageType.Label, 0x35, 3, "System", sb.ToString()));
                                    sb = new StringBuilder();
                                    sb.Append(v);
                                }
                                else
                                {
                                    sb.Append(' ');
                                    sb.Append(v);
                                }
                            }

                            if (sb.Length > 0)
                            {
                                sender.Send(new AsciiMessage(Server.Serial.MinusOne, -1, MessageType.Label, 0x35, 3, "System", sb.ToString()));
                            }

                            break;
                        }
                    case 3: // Command list
                        {
                            sender.Mobile.SendAsciiMessage(0x482, "The command prefix is \"{0}\"", CommandSystem.Prefix);
                            CommandHandlers.Help_OnCommand(new CommandEventArgs(sender.Mobile, "help", "", new string[0]));

                            break;
                        }
                }
            }
        }
    }
}
