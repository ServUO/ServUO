using System;
using Server.Accounting;
using Server.Engines.XmlSpawner2;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Misc
{
    public class CharacterCreation
    {
        private static readonly CityInfo m_NewHavenInfo = new CityInfo("New Haven", "The Bountiful Harvest Inn", 3503, 2574, 14, Map.Trammel);
        private static Mobile m_Mobile;
        public static void Initialize()
        {
            // Register our event handler
            EventSink.CharacterCreated += new CharacterCreatedEventHandler(EventSink_CharacterCreated);
        }

        public static bool VerifyProfession(int profession)
        {
            if (profession < 0)
                return false;
            else if (profession < 4)
                return true;
            else if (Core.AOS && profession < 6)
                return true;
            else if (Core.SE && profession < 8)
                return true;
            else
                return false;
        }

        private static void AddBackpack(Mobile m)
        {
            Container pack = m.Backpack;

            if (pack == null)
            {
                pack = new Backpack();
                pack.Movable = false;

                m.AddItem(pack);
            }

            PackItem(new RedBook("a book", m.Name, 20, true));
            PackItem(new Gold(1000)); // Starting gold can be customized here
            PackItem(new Candle());

            if (m.Race != Race.Gargoyle)
                PackItem(new Dagger());
            else
                PackItem(new GargishDagger());
        }

        private static Item MakeNewbie(Item item)
        {
            if (!Core.AOS)
                item.LootType = LootType.Newbied;

            return item;
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

        private static void FillBankAOS(Mobile m)
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

            PlaceItemIn(cont, 16, 115, new Factions.Silver(9000));
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

        private static void FillBankbox(Mobile m)
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

            book.Content = ulong.MaxValue;

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

        private static void AddPowerScrolls(BankBox bank)
        {
            Bag bag = new Bag();

            for (int i = 0; i < PowerScroll.Skills.Count; ++i)
                bag.DropItem(new PowerScroll(PowerScroll.Skills[i], 120.0));

            bag.DropItem(new StatCapScroll(250));

            bank.DropItem(bag);
        }

        private static void AddShirt(Mobile m, int shirtHue)
        {
            int hue = Utility.ClipDyedHue(shirtHue & 0x3FFF);

            if (m.Race == Race.Elf)
            {
                EquipItem(new ElvenShirt(hue), true);
            }
            else if (m.Race == Race.Human)
            {
                switch ( Utility.Random(3) )
                {
                    case 0:
                        EquipItem(new Shirt(hue), true);
                        break;
                    case 1:
                        EquipItem(new FancyShirt(hue), true);
                        break;
                    case 2:
                        EquipItem(new Doublet(hue), true);
                        break;
                }
            }
            else if (m.Race == Race.Gargoyle)
            {
                EquipItem(new GargishClothChest(hue));
            }
        }

        private static void AddPants(Mobile m, int pantsHue)
        {
            int hue = Utility.ClipDyedHue(pantsHue & 0x3FFF);

            if (m.Race == Race.Elf)
            {
                EquipItem(new ElvenPants(hue), true);
            }
            else if (m.Race == Race.Human)
            {
                if (m.Female)
                {
                    switch ( Utility.Random(2) )
                    {
                        case 0:
                            EquipItem(new Skirt(hue), true);
                            break;
                        case 1:
                            EquipItem(new Kilt(hue), true);
                            break;
                    }
                }
                else
                {
                    switch ( Utility.Random(2) )
                    {
                        case 0:
                            EquipItem(new LongPants(hue), true);
                            break;
                        case 1:
                            EquipItem(new ShortPants(hue), true);
                            break;
                    }
                }
            }
            else if (m.Race == Race.Gargoyle)
            {
                EquipItem(new GargishClothKilt(hue));
            }
        }

        private static void AddShoes(Mobile m)
        {
            if (m.Race == Race.Elf)
                EquipItem(new ElvenBoots(), true);
            else if (m.Race == Race.Human)
                EquipItem(new Shoes(Utility.RandomYellowHue()), true);
        }

        private static Mobile CreateMobile(Account a)
        {
            if (a.Count >= a.Limit)
                return null;

            for (int i = 0; i < a.Length; ++i)
            {
                if (a[i] == null)
                    return (a[i] = new PlayerMobile());
            }

            return null;
        }

        private static void EventSink_CharacterCreated(CharacterCreatedEventArgs args)
        {
            if (!VerifyProfession(args.Profession))
                args.Profession = 0;

            NetState state = args.State;

            if (state == null)
                return;

            Mobile newChar = CreateMobile(args.Account as Account);

            if (newChar == null)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine("Login: {0}: Character creation failed, account full", state);
                Utility.PopColor();
                return;
            }

            args.Mobile = newChar;
            m_Mobile = newChar;

            newChar.Player = true;
            newChar.AccessLevel = args.Account.AccessLevel;
            newChar.Female = args.Female;
            //newChar.Body = newChar.Female ? 0x191 : 0x190;

            if (Core.Expansion >= args.Race.RequiredExpansion)
                newChar.Race = args.Race;	//Sets body
            else
                newChar.Race = Race.DefaultRace;

            //newChar.Hue = Utility.ClipSkinHue( args.Hue & 0x3FFF ) | 0x8000;
            newChar.Hue = newChar.Race.ClipSkinHue(args.Hue & 0x3FFF) | 0x8000;

            newChar.Hunger = 20;

            bool young = false;

            if (newChar is PlayerMobile)
            {
                PlayerMobile pm = (PlayerMobile)newChar;

                pm.Profession = args.Profession;

                if (pm.IsPlayer() && ((Account)pm.Account).Young)
                    young = pm.Young = true;
            }

            SetName(newChar, args.Name);

            AddBackpack(newChar);

            SetStats(newChar, state, args.Str, args.Dex, args.Int);
            SetSkills(newChar, args.Skills, args.Profession);

            Race race = newChar.Race;

            if (race.ValidateHair(newChar, args.HairID))
            {
                newChar.HairItemID = args.HairID;
                newChar.HairHue = race.ClipHairHue(args.HairHue & 0x3FFF);
            }

            if (race.ValidateFacialHair(newChar, args.BeardID))
            {
                newChar.FacialHairItemID = args.BeardID;
                newChar.FacialHairHue = race.ClipHairHue(args.BeardHue & 0x3FFF);
            }

            if (args.Profession <= 3)
            {
                AddShirt(newChar, args.ShirtHue);
                AddPants(newChar, args.PantsHue);
                AddShoes(newChar);
            }

            if (TestCenter.Enabled)
                FillBankbox(newChar);

            if (young)
            {
                NewPlayerTicket ticket = new NewPlayerTicket();
                ticket.Owner = newChar;
                newChar.BankBox.DropItem(ticket);
            }

            CityInfo city = GetStartLocation(args, young);

            newChar.MoveToWorld(city.Location, city.Map);

            Utility.PushColor(ConsoleColor.Green);
            Console.WriteLine("Login: {0}: New character being created (account={1})", state, args.Account.Username);
            Utility.PopColor();
            Utility.PushColor(ConsoleColor.DarkGreen);
            Console.WriteLine(" - Character: {0} (serial={1})", newChar.Name, newChar.Serial);
            Console.WriteLine(" - Started: {0} {1} in {2}", city.City, city.Location, city.Map.ToString());
            Utility.PopColor();

            new WelcomeTimer(newChar).Start();

            XmlAttach.AttachTo(newChar, new XmlPoints());
            XmlAttach.AttachTo(newChar, new XmlMobFactions());
        }

        private static CityInfo GetStartLocation(CharacterCreatedEventArgs args, bool isYoung)
        {
            if (Core.ML)
            {
                //if( args.State != null && args.State.NewHaven )
                return m_NewHavenInfo;	//We don't get the client Version until AFTER Character creation
                //return args.City;  TODO: Uncomment when the old quest system is actually phased out
            }

            bool useHaven = isYoung;

            ClientFlags flags = args.State == null ? ClientFlags.None : args.State.Flags;
            Mobile m = args.Mobile;

            switch ( args.Profession )
            {
                case 4: //Necro
                    {
                        if ((flags & ClientFlags.Malas) != 0)
                        {
                            return new CityInfo("Umbra", "Mardoth's Tower", 2114, 1301, -50, Map.Malas);
                        }
                        else
                        {
                            useHaven = true; 

                            new BadStartMessage(m, 1062205);
                            /*
                            * Unfortunately you are playing on a *NON-Age-Of-Shadows* game 
                            * installation and cannot be transported to Malas.  
                            * You will not be able to take your new player quest in Malas 
                            * without an AOS client.  You are now being taken to the city of 
                            * Haven on the Trammel facet.
                            * */
                        }

                        break;
                    }
                case 5:	//Paladin
                    {
                        return m_NewHavenInfo;
                    }
                case 6:	//Samurai
                    {
                        if ((flags & ClientFlags.Tokuno) != 0)
                        {
                            return new CityInfo("Samurai DE", "Haoti's Grounds", 368, 780, -1, Map.Malas);
                        }
                        else
                        {
                            useHaven = true;

                            new BadStartMessage(m, 1063487);
                            /*
                            * Unfortunately you are playing on a *NON-Samurai-Empire* game 
                            * installation and cannot be transported to Tokuno. 
                            * You will not be able to take your new player quest in Tokuno 
                            * without an SE client. You are now being taken to the city of 
                            * Haven on the Trammel facet.
                            * */
                        }

                        break;
                    }
                case 7:	//Ninja
                    {
                        if ((flags & ClientFlags.Tokuno) != 0)
                        {
                            return new CityInfo("Ninja DE", "Enimo's Residence", 414,	823, -1, Map.Malas);
                        }
                        else
                        {
                            useHaven = true;

                            new BadStartMessage(m, 1063487);
                            /*
                            * Unfortunately you are playing on a *NON-Samurai-Empire* game 
                            * installation and cannot be transported to Tokuno. 
                            * You will not be able to take your new player quest in Tokuno 
                            * without an SE client. You are now being taken to the city of 
                            * Haven on the Trammel facet.
                            * */
                        }

                        break;
                    }
            }

            if (useHaven)
                return m_NewHavenInfo;
            else
                return args.City;
        }

        private static void FixStats(ref int str, ref int dex, ref int intel, int max)
        {
            int vMax = max - 30;

            int vStr = str - 10;
            int vDex = dex - 10;
            int vInt = intel - 10;

            if (vStr < 0)
                vStr = 0;

            if (vDex < 0)
                vDex = 0;

            if (vInt < 0)
                vInt = 0;

            int total = vStr + vDex + vInt;

            if (total == 0 || total == vMax)
                return;

            double scalar = vMax / (double)total;

            vStr = (int)(vStr * scalar);
            vDex = (int)(vDex * scalar);
            vInt = (int)(vInt * scalar);

            FixStat(ref vStr, (vStr + vDex + vInt) - vMax, vMax);
            FixStat(ref vDex, (vStr + vDex + vInt) - vMax, vMax);
            FixStat(ref vInt, (vStr + vDex + vInt) - vMax, vMax);

            str = vStr + 10;
            dex = vDex + 10;
            intel = vInt + 10;
        }

        private static void FixStat(ref int stat, int diff, int max)
        {
            stat += diff;

            if (stat < 0)
                stat = 0;
            else if (stat > max)
                stat = max;
        }

        private static void SetStats(Mobile m, NetState state, int str, int dex, int intel)
        {
            int max = state.NewCharacterCreation ? 90 : 80;

            FixStats(ref str, ref dex, ref intel, max);

            if (str < 10 || str > 60 || dex < 10 || dex > 60 || intel < 10 || intel > 60 || (str + dex + intel) != max)
            {
                str = 10;
                dex = 10;
                intel = 10;
            }

            m.InitStats(str, dex, intel);
        }

        private static void SetName(Mobile m, string name)
        {
            name = name.Trim();

            if (!NameVerification.Validate(name, 2, 16, true, false, true, 1, NameVerification.SpaceDashPeriodQuote))
                name = "Generic Player";

            m.Name = name;
        }

        private static bool ValidSkills(SkillNameValue[] skills)
        {
            int total = 0;

            for (int i = 0; i < skills.Length; ++i)
            {
                if (skills[i].Value < 0 || skills[i].Value > 50)
                    return false;

                total += skills[i].Value;

                for (int j = i + 1; j < skills.Length; ++j)
                {
                    if (skills[j].Value > 0 && skills[j].Name == skills[i].Name)
                        return false;
                }
            }

            return (total == 100 || total == 120);
        }

        private static void SetSkills(Mobile m, SkillNameValue[] skills, int prof)
        {
            switch ( prof )
            {
                case 1: // Warrior
                    {
                        skills = new SkillNameValue[]
                        {
                            new SkillNameValue(SkillName.Anatomy, 30),
                            new SkillNameValue(SkillName.Healing, 45),
                            new SkillNameValue(SkillName.Swords, 35),
                            new SkillNameValue(SkillName.Tactics, 50)
                        };

                        break;
                    }
                case 2: // Magician
                    {
                        skills = new SkillNameValue[]
                        {
                            new SkillNameValue(SkillName.EvalInt, 30),
                            new SkillNameValue(SkillName.Wrestling, 30),
                            new SkillNameValue(SkillName.Magery, 50),
                            new SkillNameValue(SkillName.Meditation, 50)
                        };

                        break;
                    }
                case 3: // Blacksmith
                    {
                        skills = new SkillNameValue[]
                        {
                            new SkillNameValue(SkillName.Mining, 30),
                            new SkillNameValue(SkillName.ArmsLore, 30),
                            new SkillNameValue(SkillName.Blacksmith, 50),
                            new SkillNameValue(SkillName.Tinkering, 50)
                        };

                        break;
                    }
                case 4: // Necromancer
                    {
                        skills = new SkillNameValue[]
                        {
                            new SkillNameValue(SkillName.Necromancy, 50),
                            new SkillNameValue(SkillName.Focus, 30),
                            new SkillNameValue(SkillName.SpiritSpeak, 30),
                            new SkillNameValue(SkillName.Swords, 30),
                            new SkillNameValue(SkillName.Tactics, 20)
                        };

                        break;
                    }
                case 5: // Paladin
                    {
                        skills = new SkillNameValue[]
                        {
                            new SkillNameValue(SkillName.Chivalry, 51),
                            new SkillNameValue(SkillName.Swords, 49),
                            new SkillNameValue(SkillName.Focus, 30),
                            new SkillNameValue(SkillName.Tactics, 30)
                        };

                        break;
                    }
                case 6:	//Samurai
                    {
                        skills = new SkillNameValue[]
                        {
                            new SkillNameValue(SkillName.Bushido, 50),
                            new SkillNameValue(SkillName.Swords, 50),
                            new SkillNameValue(SkillName.Anatomy, 30),
                            new SkillNameValue(SkillName.Healing, 30)
                        };
                        break;
                    }
                case 7:	//Ninja
                    {
                        skills = new SkillNameValue[]
                        {
                            new SkillNameValue(SkillName.Ninjitsu, 50),
                            new SkillNameValue(SkillName.Hiding, 50),
                            new SkillNameValue(SkillName.Fencing, 30),
                            new SkillNameValue(SkillName.Stealth, 30)
                        };
                        break;
                    }
                default:
                    {
                        if (!ValidSkills(skills))
                            return;

                        break;
                    }
            }

            bool addSkillItems = true;
            bool elf = (m.Race == Race.Elf);
            bool human = (m.Race == Race.Human);
            bool gargoyle = (m.Race == Race.Gargoyle);

            switch ( prof )
            {
                case 1: // Warrior
                    {
                        if (elf)
                            EquipItem(new LeafChest());
                        else if (human)
                            EquipItem(new LeatherChest());
                        else if (gargoyle)
                        {
                            EquipItem(new GargishLeatherChest());
                        }

                        break;
                    }
                case 4: // Necromancer
                    {
                        Container regs = new BagOfNecroReagents(50);

                        if (!Core.AOS)
                        {
                            foreach (Item item in regs.Items)
                                item.LootType = LootType.Newbied;
                        }

                        PackItem(regs);

                        regs.LootType = LootType.Regular;

                        if (elf || human)
                            EquipItem(new BoneHelm());

                        if (elf)
                        {
                            EquipItem(new ElvenMachete());
                            EquipItem(NecroHue(new LeafChest()));
                            EquipItem(NecroHue(new LeafArms()));
                            EquipItem(NecroHue(new LeafGloves()));
                            EquipItem(NecroHue(new LeafGorget()));
                            EquipItem(NecroHue(new LeafGorget()));
                            EquipItem(NecroHue(new ElvenPants()));	//TODO: Verify the pants
                            EquipItem(new ElvenBoots());
                        }
                        else if (human)
                        {
                            EquipItem(new BoneHarvester());
                            EquipItem(NecroHue(new LeatherChest()));
                            EquipItem(NecroHue(new LeatherArms()));
                            EquipItem(NecroHue(new LeatherGloves()));
                            EquipItem(NecroHue(new LeatherGorget()));
                            EquipItem(NecroHue(new LeatherLegs()));
                            EquipItem(NecroHue(new Skirt()));
                            EquipItem(new Sandals(0x8FD));
                        }
                        else if (gargoyle)
                        {
                            EquipItem(new GlassSword());
                            EquipItem(NecroHue(new GargishLeatherChest()));
                            EquipItem(NecroHue(new GargishLeatherArms()));
                            EquipItem(NecroHue(new GargishLeatherLegs()));
                            EquipItem(NecroHue(new GargishLeatherKilt()));
                        }

                        Spellbook book = new NecromancerSpellbook((ulong)0x8981); // animate dead, evil omen, pain spike, summon familiar, wraith form

                        PackItem(book);

                        book.LootType = LootType.Blessed;

                        addSkillItems = false;
                        break;
                    }
                case 5: // Paladin
                    {
                        if (elf)
                        {
                            EquipItem(new ElvenMachete());
                            EquipItem(new WingedHelm());
                            EquipItem(new LeafGorget());
                            EquipItem(new LeafArms());
                            EquipItem(new LeafChest());
                            EquipItem(new LeafLegs());
                            EquipItem(new ElvenBoots());	//Verify hue
                        }
                        else if (human)
                        {
                            EquipItem(new Broadsword());
                            EquipItem(new Helmet());
                            EquipItem(new PlateGorget());
                            EquipItem(new RingmailArms());
                            EquipItem(new RingmailChest());
                            EquipItem(new RingmailLegs());
                            EquipItem(new ThighBoots(0x748));
                            EquipItem(new Cloak(0xCF));
                            EquipItem(new BodySash(0xCF));
                        }
                        else if (gargoyle)
                        {
                            EquipItem(new DreadSword());
                            EquipItem(new GargishPlateChest());
                            EquipItem(new GargishPlateArms());
                            EquipItem(new GargishPlateLegs());
                            EquipItem(new GargishPlateKilt());
                        }

                        Spellbook book = new BookOfChivalry((ulong)0x3FF);
                        book.LootType = LootType.Blessed;
                        PackItem(book);

                        addSkillItems = false;
                        break;
                    }
					
                case 6: // Samurai
                    {
                        if (elf || human)
                        {
                            EquipItem(new HakamaShita(0x2C3));
                            EquipItem(new Hakama(0x2C3));
                            EquipItem(new SamuraiTabi(0x2C3));
                            EquipItem(new TattsukeHakama(0x22D));
                            EquipItem(new Bokuto());

                            if (elf)
                                EquipItem(new RavenHelm());
                            else
                                EquipItem(new LeatherJingasa());
                        }
                        else if (gargoyle)
                        {
                            EquipItem(new GlassSword());
                            EquipItem(new GargishPlateChest());
                            EquipItem(new GargishPlateArms());
                            EquipItem(new GargishPlateLegs());
                            EquipItem(new GargishPlateKilt());
                        }

                        PackItem(new Scissors());
                        PackItem(new Bandage(50));

                        Spellbook book = new BookOfBushido();
                        PackItem(book);

                        addSkillItems = false;
                        break;
                    }
                case 7: // Ninja
                    {
                        int[] hues = new int[] { 0x1A8, 0xEC, 0x99, 0x90, 0xB5, 0x336, 0x89 };
                        //TODO: Verify that's ALL the hues for that above.

                        if (elf || human)
                        {
                            EquipItem(new Kasa());
                            EquipItem(new TattsukeHakama(hues[Utility.Random(hues.Length)]));
                            EquipItem(new HakamaShita(0x2C3));
                            EquipItem(new NinjaTabi(0x2C3));

                            if (elf)
                                EquipItem(new AssassinSpike());
                            else
                                EquipItem(new Tekagi());
                        }
                        else if (gargoyle)
                        {
                            EquipItem(new GargishDagger());

                            int hue = hues[Utility.Random(hues.Length)];

                            EquipItem(new GargishClothChest(hue));
                            EquipItem(new GargishClothArms(hue));
                            EquipItem(new GargishClothLegs(hue));
                            EquipItem(new GargishClothKilt(hue));
                        }

                        PackItem(new SmokeBomb());

                        Spellbook book = new BookOfNinjitsu();
                        PackItem(book);

                        addSkillItems = false;
                        break;
                    }
            }
            
			for (int i = 0; i < skills.Length; ++i)
			{
				SkillNameValue snv = skills[i];

				if (snv.Value > 0 && (snv.Name != SkillName.Stealth || prof == 7) && snv.Name != SkillName.RemoveTrap && snv.Name != SkillName.Spellweaving)
				{
					Skill skill = m.Skills[snv.Name];

					if (skill != null)
					{
						skill.BaseFixedPoint = snv.Value * 10;
						
						if ( addSkillItems )
							AddSkillItems(snv.Name, m);
					}
                }
            }
        }

        private static void EquipItem(Item item)
        {
            EquipItem(item, false);
        }

        private static void EquipItem(Item item, bool mustEquip)
        {
            if (!Core.AOS)
                item.LootType = LootType.Newbied;

            if (m_Mobile != null && m_Mobile.EquipItem(item))
                return;

            Container pack = m_Mobile.Backpack;

            if (!mustEquip && pack != null)
                pack.DropItem(item);
            else
                item.Delete();
        }

        private static void PackItem(Item item)
        {
            if (!Core.AOS)
                item.LootType = LootType.Newbied;

            Container pack = m_Mobile.Backpack;

            if (pack != null)
                pack.DropItem(item);
            else
                item.Delete();
        }

        private static void PackInstrument()
        {
            switch ( Utility.Random(6) )
            {
                case 0:
                    PackItem(new Drums());
                    break;
                case 1:
                    PackItem(new Harp());
                    break;
                case 2:
                    PackItem(new LapHarp());
                    break;
                case 3:
                    PackItem(new Lute());
                    break;
                case 4:
                    PackItem(new Tambourine());
                    break;
                case 5:
                    PackItem(new TambourineTassel());
                    break;
            }
        }

        private static void PackScroll(int circle)
        {
            switch ( Utility.Random(8) * (circle + 1) )
            {
                case 0:
                    PackItem(new ClumsyScroll());
                    break;
                case 1:
                    PackItem(new CreateFoodScroll());
                    break;
                case 2:
                    PackItem(new FeeblemindScroll());
                    break;
                case 3:
                    PackItem(new HealScroll());
                    break;
                case 4:
                    PackItem(new MagicArrowScroll());
                    break;
                case 5:
                    PackItem(new NightSightScroll());
                    break;
                case 6:
                    PackItem(new ReactiveArmorScroll());
                    break;
                case 7:
                    PackItem(new WeakenScroll());
                    break;
                case 8:
                    PackItem(new AgilityScroll());
                    break;
                case 9:
                    PackItem(new CunningScroll());
                    break;
                case 10:
                    PackItem(new CureScroll());
                    break;
                case 11:
                    PackItem(new HarmScroll());
                    break;
                case 12:
                    PackItem(new MagicTrapScroll());
                    break;
                case 13:
                    PackItem(new MagicUnTrapScroll());
                    break;
                case 14:
                    PackItem(new ProtectionScroll());
                    break;
                case 15:
                    PackItem(new StrengthScroll());
                    break;
                case 16:
                    PackItem(new BlessScroll());
                    break;
                case 17:
                    PackItem(new FireballScroll());
                    break;
                case 18:
                    PackItem(new MagicLockScroll());
                    break;
                case 19:
                    PackItem(new PoisonScroll());
                    break;
                case 20:
                    PackItem(new TelekinisisScroll());
                    break;
                case 21:
                    PackItem(new TeleportScroll());
                    break;
                case 22:
                    PackItem(new UnlockScroll());
                    break;
                case 23:
                    PackItem(new WallOfStoneScroll());
                    break;
            }
        }

        private static Item NecroHue(Item item)
        {
            item.Hue = 0x2C3;

            return item;
        }

        private static void	AddSkillItems(SkillName skill, Mobile m)
        {
            bool elf = (m.Race == Race.Elf);
            bool human = (m.Race == Race.Human);
            bool gargoyle = (m.Race == Race.Gargoyle);

            switch ( skill )
            {
                case SkillName.Alchemy:
                    {
                        PackItem(new Bottle(4));
                        PackItem(new MortarPestle());

                        int hue = Utility.RandomPinkHue();

                        if (elf)
                        {
                            if (m.Female)
                                EquipItem(new FemaleElvenRobe(hue));
                            else
                                EquipItem(new MaleElvenRobe(hue));
                        }
                        else
                        {
                            EquipItem(new Robe(Utility.RandomPinkHue()));
                        }
                        break;
                    }
                case SkillName.Anatomy:
                    {
                        PackItem(new Bandage(3));

                        int hue = Utility.RandomYellowHue();

                        if (elf)
                        {
                            if (m.Female)
                                EquipItem(new FemaleElvenRobe(hue));
                            else
                                EquipItem(new MaleElvenRobe(hue));
                        }
                        else
                        {
                            EquipItem(new Robe(hue));
                        }
                        break;
                    }
                case SkillName.AnimalLore:
                    {
                        int hue = Utility.RandomBlueHue();

                        if (elf)
                        {
                            EquipItem(new WildStaff());

                            if (m.Female)
                                EquipItem(new FemaleElvenRobe(hue));
                            else
                                EquipItem(new MaleElvenRobe(hue));
                        }
                        else
                        {
                            EquipItem(new ShepherdsCrook());
                            EquipItem(new Robe(hue));
                        }
                        break;
                    }
                case SkillName.Archery:
                    {
                        PackItem(new Arrow(25));

                        if (elf)
                            EquipItem(new ElvenCompositeLongbow());
                        else if (human)
                            EquipItem(new Bow());
					
                        break;
                    }
                case SkillName.ArmsLore:
                    {
                        if (elf)
                        {
                            switch ( Utility.Random(3) )
                            {
                                case 0:
                                    EquipItem(new Leafblade());
                                    break;
                                case 1:
                                    EquipItem(new RuneBlade());
                                    break;
                                case 2:
                                    EquipItem(new DiamondMace());
                                    break;
                            }
                        }
                        else if (human)
                        {
                            switch ( Utility.Random(3) )
                            {
                                case 0:
                                    EquipItem(new Kryss());
                                    break;
                                case 1:
                                    EquipItem(new Katana());
                                    break;
                                case 2:
                                    EquipItem(new Club());
                                    break;
                            }
                        }
                        else if (gargoyle)
                        {
                            switch( Utility.Random(3) )
                            {
                                case 0:
                                    EquipItem(new BloodBlade());
                                    break;
                                case 1:
                                    EquipItem(new GlassSword());
                                    break;
                                case 2:
                                    EquipItem(new DiscMace());
                                    break;
                            }
                        }

                        break;
                    }
                case SkillName.Begging:
                    {
                        if (elf)
                            EquipItem(new WildStaff());
                        else if (human)
                            EquipItem(new GnarledStaff());
                        else if (gargoyle)
                            EquipItem(new SerpentStoneStaff());

                        break;
                    }
                case SkillName.Blacksmith:
                    {
                        PackItem(new Tongs());
                        PackItem(new Pickaxe());
                        PackItem(new Pickaxe());
                        PackItem(new IronIngot(50));
					
                        if (human || elf)
                        {
                            EquipItem(new HalfApron(Utility.RandomYellowHue()));
                        }

                        break;
                    }
                case SkillName.Bushido:
                    {
                        if (human || elf)
                        {
                            EquipItem(new Hakama());
                            EquipItem(new Kasa());
                        }

                        EquipItem(new BookOfBushido());
                        break;
                    }
                case SkillName.Fletching:
                    {
                        PackItem(new Board(14));
                        PackItem(new Feather(5));
                        PackItem(new Shaft(5));
                        break;
                    }
                case SkillName.Camping:
                    {
                        PackItem(new Bedroll());
                        PackItem(new Kindling(5));
                        break;
                    }
                case SkillName.Carpentry:
                    {
                        PackItem(new Board(10));
                        PackItem(new Saw());

                        if (human || elf)
                        {
                            EquipItem(new HalfApron(Utility.RandomYellowHue()));
                        }

                        break;
                    }
                case SkillName.Cartography:
                    {
                        PackItem(new BlankMap());
                        PackItem(new BlankMap());
                        PackItem(new BlankMap());
                        PackItem(new BlankMap());
                        PackItem(new Sextant());
                        break;
                    }
                case SkillName.Cooking:
                    {
                        PackItem(new Kindling(2));
                        PackItem(new RawLambLeg());
                        PackItem(new RawChickenLeg());
                        PackItem(new RawFishSteak());
                        PackItem(new SackFlour());
                        PackItem(new Pitcher(BeverageType.Water));
                        break;
                    }
                case SkillName.Chivalry:
                    {
                        if (Core.ML)
                            PackItem(new BookOfChivalry((ulong)0x3FF));

                        break;
                    }
                case SkillName.DetectHidden:
                    {
                        if (human || elf)
                            EquipItem(new Cloak(0x455));

                        break;
                    }
                case SkillName.Discordance:
                    {
                        PackInstrument();
                        break;
                    }
                case SkillName.Fencing:
                    {
                        if (elf)
                            EquipItem(new Leafblade());
                        else if (human)
                            EquipItem(new Kryss());
                        else if (gargoyle)
                            EquipItem(new BloodBlade());

                        break;
                    }
                case SkillName.Fishing:
                    {
                        EquipItem(new FishingPole());

                        int hue = Utility.RandomYellowHue();

                        if (elf)
                        {
                            Item i = new Circlet();
                            i.Hue = hue;
                            EquipItem(i);
                        }
                        else if (human)
                        {
                            EquipItem(new FloppyHat(hue));
                        }

                        break;
                    }
                case SkillName.Healing:
                    {
                        PackItem(new Bandage(50));
                        PackItem(new Scissors());
                        break;
                    }
                case SkillName.Herding:
                    {
                        if (elf)
                            EquipItem(new WildStaff());
                        else
                            EquipItem(new ShepherdsCrook());

                        break;
                    }
                case SkillName.Hiding:
                    {
                        if (human || elf)
                            EquipItem(new Cloak(0x455));

                        break;
                    }
                case SkillName.Inscribe:
                    {
                        PackItem(new BlankScroll(2));
                        PackItem(new BlueBook());
                        break;
                    }
                case SkillName.ItemID:
                    {
                        if (elf)
                            EquipItem(new WildStaff());
                        else if (human)
                            EquipItem(new GnarledStaff());
                        else if (gargoyle)
                            EquipItem(new SerpentStoneStaff());

                        break;
                    }
                case SkillName.Lockpicking:
                    {
                        PackItem(new Lockpick(20));
                        break;
                    }
                case SkillName.Lumberjacking:
                    {
                        if (human || elf)
                            EquipItem(new Hatchet());
                        else if (gargoyle)
                            EquipItem(new DualShortAxes());

                        break;
                    }
                case SkillName.Macing:
                    {
                        if (elf)
                            EquipItem(new DiamondMace());
                        else if (human)
                            EquipItem(new Club());
                        else if (gargoyle)
                            EquipItem(new DiscMace());

                        break;
                    }
                case SkillName.Magery:
                    {
                        BagOfAllReagents regs = new BagOfAllReagents(30);

                        if (!Core.AOS)
                        {
                            foreach (Item item in regs.Items)
                                item.LootType = LootType.Newbied;
                        }

                        PackItem(regs);

                        regs.LootType = LootType.Regular;

                        PackScroll(0);
                        PackScroll(1);
                        PackScroll(2);

                        Spellbook book = new Spellbook((ulong)0x382A8C38);
                        book.LootType = LootType.Blessed;
                        EquipItem(book);

                        if (elf)
                        {
                            EquipItem(new Circlet());

                            if (m.Female)
                                EquipItem(new FemaleElvenRobe(Utility.RandomBlueHue()));
                            else
                                EquipItem(new MaleElvenRobe(Utility.RandomBlueHue()));
                        }
                        else
                        {
                            if (human)
                                EquipItem(new WizardsHat());

                            EquipItem(new Robe(Utility.RandomBlueHue()));
                        }

                        break;
                    }
                case SkillName.Mining:
                    {
                        PackItem(new Pickaxe());
                        break;
                    }
                case SkillName.Musicianship:
                    {
                        PackInstrument();
                        break;
                    }
                case SkillName.Necromancy:
                    {
                        if (Core.ML)
                        {
                            Container regs = new BagOfNecroReagents(50);

                            PackItem(regs);

                            regs.LootType = LootType.Regular;
                        }

                        // RunUO fix
                        Spellbook book = new NecromancerSpellbook((ulong)0x8981); // animate dead, evil omen, pain spike, summon familiar, wraith form
                        book.LootType = LootType.Blessed;
                        PackItem(book);

                        break;
                    }
                case SkillName.Ninjitsu:
                    {
                        if (human || elf)
                        {
                            EquipItem(new Hakama(0x2C3));	//Only ninjas get the hued one.
                            EquipItem(new Kasa());
                        }

                        EquipItem(new BookOfNinjitsu());
                        break;
                    }
                case SkillName.Parry:
                    {
                        if (human || elf)
                            EquipItem(new WoodenShield());
                        else if (gargoyle)
                            EquipItem(new GargishWoodenShield());

                        break;
                    }
                case SkillName.Peacemaking:
                    {
                        PackInstrument();
                        break;
                    }
                case SkillName.Poisoning:
                    {
                        PackItem(new LesserPoisonPotion());
                        PackItem(new LesserPoisonPotion());
                        break;
                    }
                case SkillName.Provocation:
                    {
                        PackInstrument();
                        break;
                    }
                case SkillName.Snooping:
                    {
                        PackItem(new Lockpick(20));
                        break;
                    }
                case SkillName.SpiritSpeak:
                    {
                        if (human || elf)
                        {
                            EquipItem(new Cloak(0x455));
                        }

                        break;
                    }
                case SkillName.Stealing:
                    {
                        PackItem(new Lockpick(20));
                        break;
                    }
                case SkillName.Swords:
                    {
                        if (elf)
                            EquipItem(new RuneBlade());
                        else if (human)
                            EquipItem(new Katana());
                        else if (gargoyle)
                            EquipItem(new GlassSword());

                        break;
                    }
                case SkillName.Tactics:
                    {
                        if (elf)
                            EquipItem(new RuneBlade());
                        else if (human)
                            EquipItem(new Katana());
                        else if (gargoyle)
                            EquipItem(new GlassSword());

                        break;
                    }
                case SkillName.Tailoring:
                    {
                        PackItem(new BoltOfCloth());
                        PackItem(new SewingKit());
                        break;
                    }
                case SkillName.Tracking:
                    {
                        if (human || elf)
                        {
                            if (m_Mobile != null)
                            {
                                Item shoes = m_Mobile.FindItemOnLayer(Layer.Shoes);

                                if (shoes != null)
                                    shoes.Delete();
                            }

                            int hue = Utility.RandomYellowHue();

                            if (elf)
                                EquipItem(new ElvenBoots(hue));
                            else
                                EquipItem(new Boots(hue));

                            EquipItem(new SkinningKnife());
                        }
                        else if (gargoyle)
                            PackItem(new SkinningKnife());

                        break;
                    }
                case SkillName.Veterinary:
                    {
                        PackItem(new Bandage(5));
                        PackItem(new Scissors());
                        break;
                    }
                case SkillName.Wrestling:
                    {
                        if (elf)
                            EquipItem(new LeafGloves());
                        else if (human)
                            EquipItem(new LeatherGloves());
                        else if (gargoyle)
                        {
                            // Why not give them arm armor?
                            EquipItem(new GargishLeatherArms());
                        }

                        break;
                    }
                case SkillName.Throwing:
                    {
                        if (gargoyle)
                            EquipItem(new Boomerang());

                        break;
                    }
            }
        }

        private class BadStartMessage : Timer
        {
            readonly Mobile m_Mobile;
            readonly int m_Message;
            public BadStartMessage(Mobile m, int message)
                : base(TimeSpan.FromSeconds(3.5))
            {
                this.m_Mobile = m;
                this.m_Message = message;
                this.Start();
            }

            protected override void OnTick()
            {
                this.m_Mobile.SendLocalizedMessage(this.m_Message);
            }
        }
    }
}