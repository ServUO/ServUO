using Server;
using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using Server.ContextMenus;
using Server.Gumps;
using System.Collections;
using Server.Network;
using Server.Engines.Points;

namespace Server.Engines.CleanUpBritannia
{
    public class TheCleanupOfficer : BaseVendor
    {
        public override bool IsActiveVendor { get { return false; } }
        public override bool IsInvulnerable { get { return true; } }
        public override bool DisallowAllMoves { get { return true; } }
        public override bool ClickTitle { get { return true; } }
        public override bool CanTeach { get { return false; } }

        protected List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos { get { return this.m_SBInfos; } }
        public override void InitSBInfo() { }

        [Constructable]
        public TheCleanupOfficer()
            : base("the Cleanup Officer")
        {
        }

        public override void InitBody()
        {
            base.InitBody();

            Name = NameList.RandomName("male");

            Hue = Utility.RandomSkinHue();
            Body = 0x190;
            HairItemID = 0x2044;
            HairHue = 1644;
            FacialHairItemID = 0x203F;
            FacialHairHue = 1644;
        }

        public override void InitOutfit()
        {
            SetWearable(new Cloak(), 337);
            SetWearable(new ThighBoots());
            SetWearable(new LongPants(), 1409);
            SetWearable(new Doublet(), 50);
            SetWearable(new FancyShirt(), 1644);
            SetWearable(new Necklace());

            if (Backpack == null)
            {
                Item backpack = new Backpack();
                backpack.Movable = false;
                AddItem(backpack);
            }    

            /*Item item = new Mailbox();
            item.Movable = false;
            PackItem(item);

            item = new HumansAndElvesRobe();
            item.Movable = false;
            PackItem(item);

            item = new GargoylesAreOurFriendsRobe();
            item.Movable = false;
            PackItem(item);

            item = new WeArePiratesRobe();
            item.Movable = false;
            PackItem(item);

            item = new FollowerOfBaneRobe();
            item.Movable = false;
            PackItem(item);

            item = new QueenDawnForeverRobe();
            item.Movable = false;
            PackItem(item);

            item = new LillyPad();
            item.Movable = false;
            PackItem(item);

            item = new LillyPads();
            item.Movable = false;
            PackItem(item);

            item = new Mushrooms1();
            item.Movable = false;
            PackItem(item);

            item = new Mushrooms2();
            item.Movable = false;
            PackItem(item);

            item = new Mushrooms3();
            item.Movable = false;
            PackItem(item);

            item = new Mushrooms4();
            item.Movable = false;
            PackItem(item);

            item = new NocturneEarrings();
            item.Movable = false;
            PackItem(item);

            item = new SherryTheMouseQuotes();
            item.Movable = false;
            PackItem(item);

            item = new ChaosTileDeed();
            item.Movable = false;
            PackItem(item);

            item = new HonestyVirtueTileDeed();
            item.Movable = false;
            PackItem(item);

            item = new CompassionVirtueTileDeed();
            item.Movable = false;
            PackItem(item);

            item = new ValorVirtueTileDeed();
            item.Movable = false;
            PackItem(item);

            item = new SpiritualityVirtueTileDeed();
            item.Movable = false;
            PackItem(item);

            item = new HonorVirtueTileDeed();
            item.Movable = false;
            PackItem(item);

            item = new HumilityVirtueTileDeed();
            item.Movable = false;
            PackItem(item);

            item = new SacrificeVirtueTileDeed();
            item.Movable = false;
            PackItem(item);

            item = new StewardDeed();
            item.Movable = false;
            PackItem(item);

            item = new KnightsBascinet();
            item.Movable = false;
            PackItem(item);

            item = new KnightsCloseHelm();
            item.Movable = false;
            PackItem(item);

            item = new KnightsFemalePlateChest();
            item.Movable = false;
            PackItem(item);

            item = new KnightsNorseHelm();
            item.Movable = false;
            PackItem(item);

            item = new KnightsPlateArms();
            item.Movable = false;
            PackItem(item);

            item = new KnightsPlateChest();
            item.Movable = false;
            PackItem(item);

            item = new KnightsPlateGloves();
            item.Movable = false;
            PackItem(item);

            item = new KnightsPlateGorget();
            item.Movable = false;
            PackItem(item);

            item = new KnightsPlateHelm();
            item.Movable = false;
            PackItem(item);

            item = new KnightsPlateLegs();
            item.Movable = false;
            PackItem(item);

            item = new ScoutArms();
            item.Movable = false;
            PackItem(item);

            item = new ScoutBustier();
            item.Movable = false;
            PackItem(item);

            item = new ScoutChest();
            item.Movable = false;
            PackItem(item);

            item = new ScoutCirclet();
            item.Movable = false;
            PackItem(item);

            item = new ScoutFemaleChest();
            item.Movable = false;
            PackItem(item);

            item = new ScoutGloves();
            item.Movable = false;
            PackItem(item);

            item = new ScoutGorget();
            item.Movable = false;
            PackItem(item);

            item = new ScoutLegs();
            item.Movable = false;
            PackItem(item);

            item = new ScoutSmallPlateJingasa();
            item.Movable = false;
            PackItem(item);

            item = new SorcererArms();
            item.Movable = false;
            PackItem(item);

            item = new SorcererChest();
            item.Movable = false;
            PackItem(item);

            item = new SorcererFemaleChest();
            item.Movable = false;
            PackItem(item);

            item = new SorcererGloves();
            item.Movable = false;
            PackItem(item);

            item = new SorcererGorget();
            item.Movable = false;
            PackItem(item);

            item = new SorcererHat();
            item.Movable = false;
            PackItem(item);

            item = new SorcererLegs();
            item.Movable = false;
            PackItem(item);

            item = new SorcererSkirt();
            item.Movable = false;
            PackItem(item);

            item = new YuccaTree();
            item.Movable = false;
            PackItem(item);

            item = new TableLamp();
            item.Movable = false;
            PackItem(item);

            item = new Bamboo();
            item.Movable = false;
            PackItem(item);

            item = new HorseBardingDeed();
            item.Movable = false;
            PackItem(item);

            item = new ScrollofAlacrity();
            item.Movable = false;
            PackItem(item);

            item = new SnakeSkinBoots();
            item.Movable = false;
            PackItem(item);

            item = new BootsOfTheLavaLizard();
            item.Movable = false;
            PackItem(item);

            item = new BootsOfTheIceWyrm();
            item.Movable = false;
            PackItem(item);

            item = new BootsOfTheCrystalHydra();
            item.Movable = false;
            PackItem(item);

            item = new BootsOfTheThrasher();
            item.Movable = false;
            PackItem(item);

            item = new NaturesTears();
            item.Movable = false;
            PackItem(item);

            item = new PrimordialDecay();
            item.Movable = false;
            PackItem(item);

            item = new ArachnidDoom();
            item.Movable = false;
            PackItem(item);

            item = new SophisticatedElvenTapestry();
            item.Movable = false;
            PackItem(item);

            item = new OrnateElvenTapestry();
            item.Movable = false;
            PackItem(item);

            item = new ChestOfDrawers();
            item.Movable = false;
            PackItem(item);

            item = new FootedChestOfDrawers();
            item.Movable = false;
            PackItem(item);

            item = new DragonHeadDeed();
            item.Movable = false;
            PackItem(item);

            item = new NestWithEggs();
            item.Movable = false;
            PackItem(item);

            item = new FishermansHat();
            item.Movable = false;
            PackItem(item);

            item = new FishermansTrousers();
            item.Movable = false;
            PackItem(item);

            item = new FishermansVest();
            item.Movable = false;
            PackItem(item);

            item = new FishermansEelskinGloves();
            item.Movable = false;
            PackItem(item);

            item = new FishermansChestguard();
            item.Movable = false;
            PackItem(item);

            item = new FishermansKilt();
            item.Movable = false;
            PackItem(item);

            item = new FishermansArms();
            item.Movable = false;
            PackItem(item);

            item = new FishermansEarrings();
            item.Movable = false;
            PackItem(item);

            item = new FirePitDeed();
            item.Movable = false;
            PackItem(item);

            item = new PresentationStone();
            item.Movable = false;
            PackItem(item);

            item = new Beehive();
            item.Movable = false;
            PackItem(item);

            item = new ArcheryButteAddon();
            item.Movable = false;
            PackItem(item);

            item = new NovoBleue();
            item.Movable = false;
            PackItem(item);

            item = new EtoileBleue();
            item.Movable = false;
            PackItem(item);

            item = new SoleilRouge();
            item.Movable = false;
            PackItem(item);

            item = new LuneRouge();
            item.Movable = false;
            PackItem(item);

            item = new IntenseTealPigment();
            item.Movable = false;
            PackItem(item);

            item = new TyrianPurplePigment();
            item.Movable = false;
            PackItem(item);

            item = new MottledSunsetBluePigment();
            item.Movable = false;
            PackItem(item);

            item = new MossyGreenPigment();
            item.Movable = false;
            PackItem(item);

            item = new VibrantOcherPigment();
            item.Movable = false;
            PackItem(item);

            item = new OliveGreenPigment();
            item.Movable = false;
            PackItem(item);

            item = new PolishedBronzePigment();
            item.Movable = false;
            PackItem(item);

            item = new GlossyBluePigment();
            item.Movable = false;
            PackItem(item);

            item = new BlackAndGreenPigment();
            item.Movable = false;
            PackItem(item);

            item = new DeepVioletPigment();
            item.Movable = false;
            PackItem(item);

            item = new AuraOfAmberPigment();
            item.Movable = false;
            PackItem(item);

            item = new MurkySeagreenPigment();
            item.Movable = false;
            PackItem(item);

            item = new ShadowyBluePigment();
            item.Movable = false;
            PackItem(item);

            item = new GleamingFuchsiaPigment();
            item.Movable = false;
            PackItem(item);

            item = new GlossyFuchsiaPigment();
            item.Movable = false;
            PackItem(item);

            item = new DeepBluePigment();
            item.Movable = false;
            PackItem(item);

            item = new VibranSeagreenPigment();
            item.Movable = false;
            PackItem(item);

            item = new MurkyAmberPigment();
            item.Movable = false;
            PackItem(item);

            item = new VibrantCrimsonPigment();
            item.Movable = false;
            PackItem(item);

            item = new ReflectiveShadowPigment();
            item.Movable = false;
            PackItem(item);

            item = new StarBluePigment();
            item.Movable = false;
            PackItem(item);

            item = new MotherOfPearlPigment();
            item.Movable = false;
            PackItem(item);

            item = new LiquidSunshinePigment();
            item.Movable = false;
            PackItem(item);

            item = new DarkVoidPigment();
            item.Movable = false;
            PackItem(item);

            item = new LuckyCharm();
            item.Movable = false;
            PackItem(item);

            item = new SoldiersMedal();
            item.Movable = false;
            PackItem(item);

            item = new DuelistsEdge();
            item.Movable = false;
            PackItem(item);

            item = new NecromancersPhylactery();
            item.Movable = false;
            PackItem(item);

            item = new WizardsCurio();
            item.Movable = false;
            PackItem(item);

            item = new MysticsMemento();
            item.Movable = false;
            PackItem(item);*/
        }        

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1151317); // Clean Up Britannia Reward Trader
        }
        
        public override void OnDoubleClick(Mobile from)
        {
            if (from is PlayerMobile && from.InRange(this.Location, 5))
                from.SendGump(new CleanUpBritanniaRewardGump(this, from as PlayerMobile));
        }

        public TheCleanupOfficer(Serial serial)
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
    }    
}
