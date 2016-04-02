using System;
using System.Text;
using Server.Commands;
using Server.Factions;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Misc
{
    public class TestCenter
    {
        private static readonly bool m_Enabled = Config.Get("TestCenter.Enabled", false);

        public static bool Enabled
        {
            get
            {
                return m_Enabled;
            }
        }

        public static void Initialize()
        {
            // Register our speech handler
            if (Enabled)
                EventSink.Speech += new SpeechEventHandler(EventSink_Speech);
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
                                ChangeStrength(@from, (int)value);
                            else if (Insensitive.Equals(name, "dex"))
                                ChangeDexterity(@from, (int)value);
                            else if (Insensitive.Equals(name, "int"))
                                ChangeIntelligence(@from, (int)value);
                            else
                                ChangeSkill(@from, name, value);
                        }
                        catch
                        {
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

        private static void ChangeStrength(Mobile from, int value)
        {
            if (value < 10 || value > 125)
            {
                @from.SendLocalizedMessage(1005628); // Stats range between 10 and 125.
            }
            else
            {
                if ((value + @from.RawDex + @from.RawInt) > @from.StatCap)
                {
                    @from.SendLocalizedMessage(1005629); // You can not exceed the stat cap.  Try setting another stat lower first.
                }
                else
                {
                    @from.RawStr = value;
                    @from.SendLocalizedMessage(1005630); // Your stats have been adjusted.
                }
            }
        }

        private static void ChangeDexterity(Mobile from, int value)
        {
            if (value < 10 || value > 125)
            {
                @from.SendLocalizedMessage(1005628); // Stats range between 10 and 125.
            }
            else
            {
                if ((@from.RawStr + value + @from.RawInt) > @from.StatCap)
                {
                    @from.SendLocalizedMessage(1005629); // You can not exceed the stat cap.  Try setting another stat lower first.
                }
                else
                {
                    @from.RawDex = value;
                    @from.SendLocalizedMessage(1005630); // Your stats have been adjusted.
                }
            }
        }

        private static void ChangeIntelligence(Mobile from, int value)
        {
            if (value < 10 || value > 125)
            {
                @from.SendLocalizedMessage(1005628); // Stats range between 10 and 125.
            }
            else
            {
                if ((@from.RawStr + @from.RawDex + value) > @from.StatCap)
                {
                    @from.SendLocalizedMessage(1005629); // You can not exceed the stat cap.  Try setting another stat lower first.
                }
                else
                {
                    @from.RawInt = value;
                    @from.SendLocalizedMessage(1005630); // Your stats have been adjusted.
                }
            }
        }

        private static void ChangeSkill(Mobile from, string name, double value)
        {
            SkillName index;

            if (!Enum.TryParse(name, true, out index) || (!Core.SE && (int)index > 51) || (!Core.AOS && (int)index > 48))
            {
                @from.SendLocalizedMessage(1005631); // You have specified an invalid skill to set.
                return;
            }

            Skill skill = @from.Skills[index];

            if (skill != null)
            {
                if (value < 0 || value > skill.Cap)
                {
                    @from.SendMessage(String.Format("Your skill in {0} is capped at {1:F1}.", skill.Info.Name, skill.Cap));
                }
                else
                {
                    int newFixedPoint = (int)(value * 10.0);
                    int oldFixedPoint = skill.BaseFixedPoint;

                    if (((skill.Owner.Total - oldFixedPoint) + newFixedPoint) > skill.Owner.Cap)
                    {
                        @from.SendMessage("You can not exceed the skill cap.  Try setting another skill lower first.");
                    }
                    else
                    {
                        skill.BaseFixedPoint = newFixedPoint;
                    }
                }
            }
            else
            {
                @from.SendLocalizedMessage(1005631); // You have specified an invalid skill to set.
            }
        }

        private static void PlaceItemIn(Container parent, int x, int y, Item item)
        {
            parent.AddItem(item);
            item.Location = new Point3D(x, y, 0);
        }

        private static Item MakePotionKeg(PotionEffect type, int hue)
        {
            PotionKeg keg = new PotionKeg();

            keg.Held = 100;
            keg.Type = type;
            keg.Hue = hue;

            return MakeNewbie(keg);
        }

        private static Item MakeNewbie(Item item)
        {
            if (!Core.AOS)
                item.LootType = LootType.Newbied;

            return item;
        }

        public static void FillBankAOS(Mobile m)
        {
            BankBox bank = m.BankBox;

            // The new AOS bankboxes don't have powerscrolls, they are automatically 'applied':

            for (int i = 0; i < PowerScroll.Skills.Count; ++i)
                m.Skills[PowerScroll.Skills[i]].Cap = 120.0;

            m.StatCap = 250;

            Container cont;

            // Begin box of money
            cont = new WoodenBox();
            cont.ItemID = 0xE7D;
            cont.Hue = 0x489;

            PlaceItemIn(cont, 16, 51, new BankCheck(500000));
            PlaceItemIn(cont, 28, 51, new BankCheck(250000));
            PlaceItemIn(cont, 40, 51, new BankCheck(100000));
            PlaceItemIn(cont, 52, 51, new BankCheck(100000));
            PlaceItemIn(cont, 64, 51, new BankCheck(50000));

            PlaceItemIn(cont, 16, 115, new Silver(9000));
            PlaceItemIn(cont, 34, 115, new Gold(60000));

            PlaceItemIn(bank, 18, 169, cont);
            // End box of money

            // Begin bag of potion kegs
            cont = new Backpack();
            cont.Name = "Various Potion Kegs";

            PlaceItemIn(cont, 45, 149, MakePotionKeg(PotionEffect.CureGreater, 0x2D));
            PlaceItemIn(cont, 69, 149, MakePotionKeg(PotionEffect.HealGreater, 0x499));
            PlaceItemIn(cont, 93, 149, MakePotionKeg(PotionEffect.PoisonDeadly, 0x46));
            PlaceItemIn(cont, 117, 149, MakePotionKeg(PotionEffect.RefreshTotal, 0x21));
            PlaceItemIn(cont, 141, 149, MakePotionKeg(PotionEffect.ExplosionGreater, 0x74));

            PlaceItemIn(cont, 93, 82, new Bottle(1000));

            PlaceItemIn(bank, 53, 169, cont);
            // End bag of potion kegs

            // Begin bag of tools
            cont = new Bag();
            cont.Name = "Tool Bag";

            PlaceItemIn(cont, 30, 35, new TinkerTools(1000));
            PlaceItemIn(cont, 60, 35, new HousePlacementTool());
            PlaceItemIn(cont, 90, 35, new DovetailSaw(1000));
            PlaceItemIn(cont, 30, 68, new Scissors());
            PlaceItemIn(cont, 45, 68, new MortarPestle(1000));
            PlaceItemIn(cont, 75, 68, new ScribesPen(1000));
            PlaceItemIn(cont, 90, 68, new SmithHammer(1000));
            PlaceItemIn(cont, 30, 118, new TwoHandedAxe());
            PlaceItemIn(cont, 60, 118, new FletcherTools(1000));
            PlaceItemIn(cont, 90, 118, new SewingKit(1000));

            PlaceItemIn(cont, 36, 51, new RunicHammer(CraftResource.DullCopper, 1000));
            PlaceItemIn(cont, 42, 51, new RunicHammer(CraftResource.ShadowIron, 1000));
            PlaceItemIn(cont, 48, 51, new RunicHammer(CraftResource.Copper, 1000));
            PlaceItemIn(cont, 54, 51, new RunicHammer(CraftResource.Bronze, 1000));
            PlaceItemIn(cont, 61, 51, new RunicHammer(CraftResource.Gold, 1000));
            PlaceItemIn(cont, 67, 51, new RunicHammer(CraftResource.Agapite, 1000));
            PlaceItemIn(cont, 73, 51, new RunicHammer(CraftResource.Verite, 1000));
            PlaceItemIn(cont, 79, 51, new RunicHammer(CraftResource.Valorite, 1000));

            PlaceItemIn(cont, 36, 55, new RunicSewingKit(CraftResource.SpinedLeather, 1000));
            PlaceItemIn(cont, 42, 55, new RunicSewingKit(CraftResource.HornedLeather, 1000));
            PlaceItemIn(cont, 48, 55, new RunicSewingKit(CraftResource.BarbedLeather, 1000));

            PlaceItemIn(bank, 118, 169, cont);
            // End bag of tools

            // Begin bag of archery ammo
            cont = new Bag();
            cont.Name = "Bag Of Archery Ammo";

            PlaceItemIn(cont, 48, 76, new Arrow(5000));
            PlaceItemIn(cont, 72, 76, new Bolt(5000));

            PlaceItemIn(bank, 118, 124, cont);
            // End bag of archery ammo

            // Begin bag of treasure maps
            cont = new Bag();
            cont.Name = "Bag Of Treasure Maps";

            PlaceItemIn(cont, 30, 35, new TreasureMap(1, Map.Trammel));
            PlaceItemIn(cont, 45, 35, new TreasureMap(2, Map.Trammel));
            PlaceItemIn(cont, 60, 35, new TreasureMap(3, Map.Trammel));
            PlaceItemIn(cont, 75, 35, new TreasureMap(4, Map.Trammel));
            PlaceItemIn(cont, 90, 35, new TreasureMap(5, Map.Trammel));
            PlaceItemIn(cont, 90, 35, new TreasureMap(6, Map.Trammel));

            PlaceItemIn(cont, 30, 50, new TreasureMap(1, Map.Trammel));
            PlaceItemIn(cont, 45, 50, new TreasureMap(2, Map.Trammel));
            PlaceItemIn(cont, 60, 50, new TreasureMap(3, Map.Trammel));
            PlaceItemIn(cont, 75, 50, new TreasureMap(4, Map.Trammel));
            PlaceItemIn(cont, 90, 50, new TreasureMap(5, Map.Trammel));
            PlaceItemIn(cont, 90, 50, new TreasureMap(6, Map.Trammel));

            PlaceItemIn(cont, 55, 100, new Lockpick(30));
            PlaceItemIn(cont, 60, 100, new Pickaxe());

            PlaceItemIn(bank, 98, 124, cont);
            // End bag of treasure maps

            // Begin bag of raw materials
            cont = new Bag();
            cont.Hue = 0x835;
            cont.Name = "Raw Materials Bag";

            PlaceItemIn(cont, 92, 60, new BarbedLeather(5000));
            PlaceItemIn(cont, 92, 68, new HornedLeather(5000));
            PlaceItemIn(cont, 92, 76, new SpinedLeather(5000));
            PlaceItemIn(cont, 92, 84, new Leather(5000));

            PlaceItemIn(cont, 30, 118, new Cloth(5000));
            PlaceItemIn(cont, 30, 84, new Board(5000));
            PlaceItemIn(cont, 57, 80, new BlankScroll(500));

            PlaceItemIn(cont, 30, 35, new DullCopperIngot(5000));
            PlaceItemIn(cont, 37, 35, new ShadowIronIngot(5000));
            PlaceItemIn(cont, 44, 35, new CopperIngot(5000));
            PlaceItemIn(cont, 51, 35, new BronzeIngot(5000));
            PlaceItemIn(cont, 58, 35, new GoldIngot(5000));
            PlaceItemIn(cont, 65, 35, new AgapiteIngot(5000));
            PlaceItemIn(cont, 72, 35, new VeriteIngot(5000));
            PlaceItemIn(cont, 79, 35, new ValoriteIngot(5000));
            PlaceItemIn(cont, 86, 35, new IronIngot(5000));

            PlaceItemIn(cont, 30, 59, new RedScales(5000));
            PlaceItemIn(cont, 36, 59, new YellowScales(5000));
            PlaceItemIn(cont, 42, 59, new BlackScales(5000));
            PlaceItemIn(cont, 48, 59, new GreenScales(5000));
            PlaceItemIn(cont, 54, 59, new WhiteScales(5000));
            PlaceItemIn(cont, 60, 59, new BlueScales(5000));

            PlaceItemIn(bank, 98, 169, cont);
            // End bag of raw materials

            // Begin bag of spell casting stuff
            cont = new Backpack();
            cont.Hue = 0x480;
            cont.Name = "Spell Casting Stuff";

            PlaceItemIn(cont, 45, 105, new Spellbook(UInt64.MaxValue));
            PlaceItemIn(cont, 65, 105, new NecromancerSpellbook((UInt64)0xFFFF));
            PlaceItemIn(cont, 85, 105, new BookOfChivalry((UInt64)0x3FF));
            PlaceItemIn(cont, 105, 105, new BookOfBushido());	//Default ctor = full
            PlaceItemIn(cont, 125, 105, new BookOfNinjitsu()); //Default ctor = full

            Runebook runebook = new Runebook(10);
            runebook.CurCharges = runebook.MaxCharges;
            PlaceItemIn(cont, 145, 105, runebook);

            Item toHue = new BagOfAllReagents(150);
            toHue.Hue = 0x2D;
            PlaceItemIn(cont, 45, 150, toHue);

            toHue = new BagOfNecroReagents(150);
            toHue.Hue = 0x488;
            PlaceItemIn(cont, 65, 150, toHue);

            PlaceItemIn(cont, 140, 150, new BagOfAllReagents(500));

            for (int i = 0; i < 9; ++i)
                PlaceItemIn(cont, 45 + (i * 10), 75, new RecallRune());

            PlaceItemIn(cont, 141, 74, new FireHorn());

            PlaceItemIn(bank, 78, 169, cont);
            // End bag of spell casting stuff

            // Begin bag of ethereals
            cont = new Backpack();
            cont.Hue = 0x490;
            cont.Name = "Bag Of Ethy's!";

            PlaceItemIn(cont, 45, 66, new EtherealHorse());
            PlaceItemIn(cont, 69, 82, new EtherealOstard());
            PlaceItemIn(cont, 93, 99, new EtherealLlama());
            PlaceItemIn(cont, 117, 115, new EtherealKirin());
            PlaceItemIn(cont, 45, 132, new EtherealUnicorn());
            PlaceItemIn(cont, 69, 66, new EtherealRidgeback());
            PlaceItemIn(cont, 93, 82, new EtherealSwampDragon());
            PlaceItemIn(cont, 117, 99, new EtherealBeetle());

            PlaceItemIn(bank, 38, 124, cont);
            // End bag of ethereals

            // Begin first bag of artifacts
            cont = new Backpack();
            cont.Hue = 0x48F;
            cont.Name = "Bag of Artifacts";

            PlaceItemIn(cont, 45, 66, new TitansHammer());
            PlaceItemIn(cont, 69, 82, new InquisitorsResolution());
            PlaceItemIn(cont, 93, 99, new BladeOfTheRighteous());
            PlaceItemIn(cont, 117, 115, new ZyronicClaw());

            PlaceItemIn(bank, 58, 124, cont);
            // End first bag of artifacts

            // Begin second bag of artifacts
            cont = new Backpack();
            cont.Hue = 0x48F;
            cont.Name = "Bag of Artifacts";

            PlaceItemIn(cont, 45, 66, new GauntletsOfNobility());
            PlaceItemIn(cont, 69, 82, new MidnightBracers());
            PlaceItemIn(cont, 93, 99, new VoiceOfTheFallenKing());
            PlaceItemIn(cont, 117, 115, new OrnateCrownOfTheHarrower());
            PlaceItemIn(cont, 45, 132, new HelmOfInsight());
            PlaceItemIn(cont, 69, 66, new HolyKnightsBreastplate());
            PlaceItemIn(cont, 93, 82, new ArmorOfFortune());
            PlaceItemIn(cont, 117, 99, new TunicOfFire());
            PlaceItemIn(cont, 45, 115, new LeggingsOfBane());
            PlaceItemIn(cont, 69, 132, new ArcaneShield());
            PlaceItemIn(cont, 93, 66, new Aegis());
            PlaceItemIn(cont, 117, 82, new RingOfTheVile());
            PlaceItemIn(cont, 45, 99, new BraceletOfHealth());
            PlaceItemIn(cont, 69, 115, new RingOfTheElements());
            PlaceItemIn(cont, 93, 132, new OrnamentOfTheMagician());
            PlaceItemIn(cont, 117, 66, new DivineCountenance());
            PlaceItemIn(cont, 45, 82, new JackalsCollar());
            PlaceItemIn(cont, 69, 99, new HuntersHeaddress());
            PlaceItemIn(cont, 93, 115, new HatOfTheMagi());
            PlaceItemIn(cont, 117, 132, new ShadowDancerLeggings());
            PlaceItemIn(cont, 45, 66, new SpiritOfTheTotem());
            PlaceItemIn(cont, 69, 82, new BladeOfInsanity());
            PlaceItemIn(cont, 93, 99, new AxeOfTheHeavens());
            PlaceItemIn(cont, 117, 115, new TheBeserkersMaul());
            PlaceItemIn(cont, 45, 132, new Frostbringer());
            PlaceItemIn(cont, 69, 66, new BreathOfTheDead());
            PlaceItemIn(cont, 93, 82, new TheDragonSlayer());
            PlaceItemIn(cont, 117, 99, new BoneCrusher());
            PlaceItemIn(cont, 45, 115, new StaffOfTheMagi());
            PlaceItemIn(cont, 69, 132, new SerpentsFang());
            PlaceItemIn(cont, 93, 66, new LegacyOfTheDreadLord());
            PlaceItemIn(cont, 117, 82, new TheTaskmaster());
            PlaceItemIn(cont, 45, 99, new TheDryadBow());

            PlaceItemIn(bank, 78, 124, cont);
            // End second bag of artifacts

            // Begin bag of minor artifacts
            cont = new Backpack();
            cont.Hue = 0x48F;
            cont.Name = "Bag of Minor Artifacts";

            PlaceItemIn(cont, 45, 66, new LunaLance());
            PlaceItemIn(cont, 69, 82, new VioletCourage());
            PlaceItemIn(cont, 93, 99, new CavortingClub());
            PlaceItemIn(cont, 117, 115, new CaptainQuacklebushsCutlass());
            PlaceItemIn(cont, 45, 132, new NightsKiss());
            PlaceItemIn(cont, 69, 66, new ShipModelOfTheHMSCape());
            PlaceItemIn(cont, 93, 82, new AdmiralsHeartyRum());
            PlaceItemIn(cont, 117, 99, new CandelabraOfSouls());
            PlaceItemIn(cont, 45, 115, new IolosLute());
            PlaceItemIn(cont, 69, 132, new GwennosHarp());
            PlaceItemIn(cont, 93, 66, new ArcticDeathDealer());
            PlaceItemIn(cont, 117, 82, new EnchantedTitanLegBone());
            PlaceItemIn(cont, 45, 99, new NoxRangersHeavyCrossbow());
            PlaceItemIn(cont, 69, 115, new BlazeOfDeath());
            PlaceItemIn(cont, 93, 132, new DreadPirateHat());
            PlaceItemIn(cont, 117, 66, new BurglarsBandana());
            PlaceItemIn(cont, 45, 82, new GoldBricks());
            PlaceItemIn(cont, 69, 99, new AlchemistsBauble());
            PlaceItemIn(cont, 93, 115, new PhillipsWoodenSteed());
            PlaceItemIn(cont, 117, 132, new PolarBearMask());
            PlaceItemIn(cont, 45, 66, new BowOfTheJukaKing());
            PlaceItemIn(cont, 69, 82, new GlovesOfThePugilist());
            PlaceItemIn(cont, 93, 99, new OrcishVisage());
            PlaceItemIn(cont, 117, 115, new StaffOfPower());
            PlaceItemIn(cont, 45, 132, new ShieldOfInvulnerability());
            PlaceItemIn(cont, 69, 66, new HeartOfTheLion());
            PlaceItemIn(cont, 93, 82, new ColdBlood());
            PlaceItemIn(cont, 117, 99, new GhostShipAnchor());
            PlaceItemIn(cont, 45, 115, new SeahorseStatuette());
            PlaceItemIn(cont, 69, 132, new WrathOfTheDryad());
            PlaceItemIn(cont, 93, 66, new PixieSwatter());

            for (int i = 0; i < 10; i++)
                PlaceItemIn(cont, 117, 128, new MessageInABottle(Utility.RandomBool() ? Map.Trammel : Map.Felucca, 4));

            PlaceItemIn(bank, 18, 124, cont);

            if (Core.SE)
            {
                cont = new Bag();
                cont.Hue = 0x501;
                cont.Name = "Tokuno Minor Artifacts";

                PlaceItemIn(cont, 42, 70, new Exiler());
                PlaceItemIn(cont, 38, 53, new HanzosBow());
                PlaceItemIn(cont, 45, 40, new TheDestroyer());
                PlaceItemIn(cont, 92, 80, new DragonNunchaku());
                PlaceItemIn(cont, 42, 56, new PeasantsBokuto());
                PlaceItemIn(cont, 44, 71, new TomeOfEnlightenment());
                PlaceItemIn(cont, 35, 35, new ChestOfHeirlooms());
                PlaceItemIn(cont, 29, 0, new HonorableSwords());
                PlaceItemIn(cont, 49, 85, new AncientUrn());
                PlaceItemIn(cont, 51, 58, new FluteOfRenewal());
                PlaceItemIn(cont, 70, 51, new PigmentsOfTokuno());
                PlaceItemIn(cont, 40, 79, new AncientSamuraiDo());
                PlaceItemIn(cont, 51, 61, new LegsOfStability());
                PlaceItemIn(cont, 88, 78, new GlovesOfTheSun());
                PlaceItemIn(cont, 55, 62, new AncientFarmersKasa());
                PlaceItemIn(cont, 55, 83, new ArmsOfTacticalExcellence());
                PlaceItemIn(cont, 50, 85, new DaimyosHelm());
                PlaceItemIn(cont, 52, 78, new BlackLotusHood());
                PlaceItemIn(cont, 52, 79, new DemonForks());
                PlaceItemIn(cont, 33, 49, new PilferedDancerFans());

                PlaceItemIn(bank, 58, 124, cont);
            }

            if (Core.SE)	//This bag came only after SE.
            {
                cont = new Bag();
                cont.Name = "Bag of Bows";

                PlaceItemIn(cont, 31, 84, new Bow());
                PlaceItemIn(cont, 78, 74, new CompositeBow());
                PlaceItemIn(cont, 53, 71, new Crossbow());
                PlaceItemIn(cont, 56, 39, new HeavyCrossbow());
                PlaceItemIn(cont, 82, 72, new RepeatingCrossbow());
                PlaceItemIn(cont, 49, 45, new Yumi());

                for (int i = 0; i < cont.Items.Count; i++)
                {
                    BaseRanged bow = cont.Items[i] as BaseRanged;

                    if (bow != null)
                    {
                        bow.Attributes.WeaponSpeed = 35;
                        bow.Attributes.WeaponDamage = 35;
                    }
                }

                PlaceItemIn(bank, 108, 135, cont);
            }
        }

        public static void FillBankbox(Mobile m)
        {
            if (Core.AOS)
            {
                FillBankAOS(m);
                return;
            }

            BankBox bank = m.BankBox;

            bank.DropItem(new BankCheck(1000000));

            // Full spellbook
            Spellbook book = new Spellbook();

            book.Content = UInt64.MaxValue;

            bank.DropItem(book);

            Bag bag = new Bag();

            for (int i = 0; i < 5; ++i)
                bag.DropItem(new Moonstone(MoonstoneType.Felucca));

            // Felucca moonstones
            bank.DropItem(bag);

            bag = new Bag();

            for (int i = 0; i < 5; ++i)
                bag.DropItem(new Moonstone(MoonstoneType.Trammel));

            // Trammel moonstones
            bank.DropItem(bag);

            // Treasure maps
            bank.DropItem(new TreasureMap(1, Map.Trammel));
            bank.DropItem(new TreasureMap(2, Map.Trammel));
            bank.DropItem(new TreasureMap(3, Map.Trammel));
            bank.DropItem(new TreasureMap(4, Map.Trammel));
            bank.DropItem(new TreasureMap(5, Map.Trammel));

            // Bag containing 50 of each reagent
            bank.DropItem(new BagOfAllReagents(50));

            // Craft tools
            bank.DropItem(MakeNewbie(new Scissors()));
            bank.DropItem(MakeNewbie(new SewingKit(1000)));
            bank.DropItem(MakeNewbie(new SmithHammer(1000)));
            bank.DropItem(MakeNewbie(new FletcherTools(1000)));
            bank.DropItem(MakeNewbie(new DovetailSaw(1000)));
            bank.DropItem(MakeNewbie(new MortarPestle(1000)));
            bank.DropItem(MakeNewbie(new ScribesPen(1000)));
            bank.DropItem(MakeNewbie(new TinkerTools(1000)));

            // A few dye tubs
            bank.DropItem(new Dyes());
            bank.DropItem(new DyeTub());
            bank.DropItem(new DyeTub());
            bank.DropItem(new BlackDyeTub());

            DyeTub darkRedTub = new DyeTub();

            darkRedTub.DyedHue = 0x485;
            darkRedTub.Redyable = false;

            bank.DropItem(darkRedTub);

            // Some food
            bank.DropItem(MakeNewbie(new Apple(1000)));

            // Resources
            bank.DropItem(MakeNewbie(new Feather(1000)));
            bank.DropItem(MakeNewbie(new BoltOfCloth(1000)));
            bank.DropItem(MakeNewbie(new BlankScroll(1000)));
            bank.DropItem(MakeNewbie(new Hides(1000)));
            bank.DropItem(MakeNewbie(new Bandage(1000)));
            bank.DropItem(MakeNewbie(new Bottle(1000)));
            bank.DropItem(MakeNewbie(new Log(1000)));

            bank.DropItem(MakeNewbie(new IronIngot(5000)));
            bank.DropItem(MakeNewbie(new DullCopperIngot(5000)));
            bank.DropItem(MakeNewbie(new ShadowIronIngot(5000)));
            bank.DropItem(MakeNewbie(new CopperIngot(5000)));
            bank.DropItem(MakeNewbie(new BronzeIngot(5000)));
            bank.DropItem(MakeNewbie(new GoldIngot(5000)));
            bank.DropItem(MakeNewbie(new AgapiteIngot(5000)));
            bank.DropItem(MakeNewbie(new VeriteIngot(5000)));
            bank.DropItem(MakeNewbie(new ValoriteIngot(5000)));

            // Reagents
            bank.DropItem(MakeNewbie(new BlackPearl(1000)));
            bank.DropItem(MakeNewbie(new Bloodmoss(1000)));
            bank.DropItem(MakeNewbie(new Garlic(1000)));
            bank.DropItem(MakeNewbie(new Ginseng(1000)));
            bank.DropItem(MakeNewbie(new MandrakeRoot(1000)));
            bank.DropItem(MakeNewbie(new Nightshade(1000)));
            bank.DropItem(MakeNewbie(new SulfurousAsh(1000)));
            bank.DropItem(MakeNewbie(new SpidersSilk(1000)));

            // Some extra starting gold
            bank.DropItem(MakeNewbie(new Gold(9000)));

            // 5 blank recall runes
            for (int i = 0; i < 5; ++i)
                bank.DropItem(MakeNewbie(new RecallRune()));

            AddPowerScrolls(bank);
        }

        public static void AddPowerScrolls(BankBox bank)
        {
            Bag bag = new Bag();

            for (int i = 0; i < PowerScroll.Skills.Count; ++i)
                bag.DropItem(new PowerScroll(PowerScroll.Skills[i], 120.0));

            bag.DropItem(new StatCapScroll(250));

            bank.DropItem(bag);
        }

        public class TCHelpGump : Gump
        {
            public TCHelpGump()
                : base(40, 40)
            {
                this.AddPage(0);
                this.AddBackground(0, 0, 160, 120, 5054);

                this.AddButton(10, 10, 0xFB7, 0xFB9, 1, GumpButtonType.Reply, 0);
                this.AddLabel(45, 10, 0x34, "ServUO");

                this.AddButton(10, 35, 0xFB7, 0xFB9, 2, GumpButtonType.Reply, 0);
                this.AddLabel(45, 35, 0x34, "List of skills");

                this.AddButton(10, 60, 0xFB7, 0xFB9, 3, GumpButtonType.Reply, 0);
                this.AddLabel(45, 60, 0x34, "Command list");

                this.AddButton(10, 85, 0xFB1, 0xFB3, 0, GumpButtonType.Reply, 0);
                this.AddLabel(45, 85, 0x34, "Close");
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                switch ( info.ButtonID )
                {
                    case 1:
                        {
                            sender.LaunchBrowser("http://ServUO.craftuo.com/");
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
