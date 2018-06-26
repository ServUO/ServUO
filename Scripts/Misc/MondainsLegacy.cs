using System;
using System.IO;
using System.Xml;
using Server.Commands;
using Server.Engines.InstancedPeerless;
using Server.Engines.Quests;
using Server.Items;
using Server.Mobiles;
using Server.Gumps;
using Server.Network;

namespace Server
{
    public interface ICanBeElfOrHuman
    {
        bool ElfOnly { get; set; }
    }

    public static class MondainsLegacy
    {
        private static readonly Type[] m_PigmentList = new Type[]
        {
            typeof(RoyalZooLeatherLegs), typeof(RoyalZooLeatherGloves), typeof(RoyalZooLeatherGorget), typeof(RoyalZooLeatherArms),
            typeof(RoyalZooLeatherChest), typeof(RoyalZooLeatherFemaleChest), typeof(RoyalZooStuddedLegs), typeof(RoyalZooStuddedGloves),
            typeof(RoyalZooStuddedGorget), typeof(RoyalZooStuddedArms), typeof(RoyalZooStuddedChest), typeof(RoyalZooStuddedFemaleChest),
            typeof(RoyalZooPlateHelm), typeof(RoyalZooPlateFemaleChest), typeof(RoyalZooPlateChest), typeof(RoyalZooPlateArms),
            typeof(RoyalZooPlateGorget), typeof(RoyalZooPlateGloves), typeof(RoyalZooPlateLegs), typeof(MinaxsArmor),
            typeof(KeeoneansChainMail), typeof(VesperOrderShield), typeof(VesperChaosShield), typeof(ClaininsSpellbook),
            typeof(BlackthornsKryss), typeof(SwordOfJustice), typeof(GeoffreysAxe), typeof(TreatiseonAlchemyTalisman),
            typeof(PrimerOnArmsTalisman), typeof(MyBookTalisman), typeof(TalkingtoWispsTalisman), typeof(GrammarOfOrchishTalisman),
            typeof(BirdsofBritanniaTalisman), typeof(TheLifeOfTravelingMinstrelTalisman), typeof(MaceAndShieldGlasses), typeof(GlassesOfTheArts),
            typeof(FoldedSteelGlasses), typeof(TradesGlasses), typeof(LyricalGlasses), typeof(AnthropomorphistGlasses),
            typeof(LightOfWayGlasses), typeof(NecromanticGlasses), typeof(WizardsCrystalGlasses), typeof(MaritimeGlasses),
            typeof(TreasuresAndTrinketsGlasses), typeof(PoisonedGlasses), typeof(GypsyHeaddress), typeof(NystulsWizardsHat),
            typeof(JesterHatOfChuckles)
        };
        private static readonly Type[] m_Artifacts = new Type[]
        {
            typeof(AegisOfGrace), typeof(BladeDance), typeof(BloodwoodSpirit), typeof(Bonesmasher),
            typeof(Boomstick), typeof(BrightsightLenses), typeof(FeyLeggings), typeof(FleshRipper),
            typeof(HelmOfSwiftness), typeof(PadsOfTheCuSidhe), typeof(QuiverOfRage), typeof(QuiverOfElements),
            typeof(RaedsGlory), typeof(RighteousAnger), typeof(RobeOfTheEclipse), typeof(RobeOfTheEquinox),
            typeof(SoulSeeker), typeof(TalonBite), typeof(TotemOfVoid), typeof(WildfireBow),
            typeof(Windsong)
        };
        // true - dungeon is enabled, false - dungeon is disabled
        private static bool m_PalaceOfParoxysmus;
        private static bool m_TwistedWeald;
        private static bool m_BlightedGrove;
        private static bool m_Bedlam;
        private static bool m_PrismOfLight;
        private static bool m_Citadel;
        private static bool m_PaintedCaves;
        private static bool m_Labyrinth;
        private static bool m_Sanctuary;
        private static bool m_StygianDragonLair;
        private static bool m_MedusasLair;
        private static bool m_Spellweaving;
        private static bool m_PublicDonations;
        public static bool PalaceOfParoxysmus
        {
            get
            {
                return m_PalaceOfParoxysmus;
            }
            set
            {
                m_PalaceOfParoxysmus = value;
            }
        }
        public static bool TwistedWeald
        {
            get
            {
                return m_TwistedWeald;
            }
            set
            {
                m_TwistedWeald = value;
            }
        }
        public static bool BlightedGrove
        {
            get
            {
                return m_BlightedGrove;
            }
            set
            {
                m_BlightedGrove = value;
            }
        }
        public static bool Bedlam
        {
            get
            {
                return m_Bedlam;
            }
            set
            {
                m_Bedlam = value;
            }
        }
        public static bool PrismOfLight
        {
            get
            {
                return m_PrismOfLight;
            }
            set
            {
                m_PrismOfLight = value;
            }
        }
        public static bool Citadel
        {
            get
            {
                return m_Citadel;
            }
            set
            {
                m_Citadel = value;
            }
        }
        public static bool PaintedCaves
        {
            get
            {
                return m_PaintedCaves;
            }
            set
            {
                m_PaintedCaves = value;
            }
        }
        public static bool Labyrinth
        {
            get
            {
                return m_Labyrinth;
            }
            set
            {
                m_Labyrinth = value;
            }
        }
        public static bool Sanctuary
        {
            get
            {
                return m_Sanctuary;
            }
            set
            {
                m_Sanctuary = value;
            }
        }
        public static bool StygianDragonLair
        {
            get
            {
                return m_StygianDragonLair;
            }
            set
            {
                m_StygianDragonLair = value;
            }
        }
        public static bool MedusasLair
        {
            get
            {
                return m_MedusasLair;
            }
            set
            {
                m_MedusasLair = value;
            }
        }
        public static bool Spellweaving
        {
            get
            {
                return m_Spellweaving;
            }
            set
            {
                m_Spellweaving = value;
            }
        }
        public static bool PublicDonations
        {
            get
            {
                return m_PublicDonations;
            }
            set
            {
                m_PublicDonations = value;
            }
        }
        public static Type[] PigmentList
        {
            get
            {
                return m_PigmentList;
            }
        }
        public static Type[] Artifacts
        {
            get
            {
                return m_Artifacts;
            }
        }
        public static void Initialize()
        {
            EventSink.OnKilledBy += OnKilledBy;

			CommandSystem.Register("DecorateML", AccessLevel.Administrator, new CommandEventHandler(DecorateML_OnCommand));
			CommandSystem.Register("DecorateMLDelete", AccessLevel.Administrator, new CommandEventHandler(DecorateMLDelete_OnCommand));
			CommandSystem.Register("SettingsML", AccessLevel.Administrator, new CommandEventHandler(SettingsML_OnCommand));
            CommandSystem.Register("Quests", AccessLevel.GameMaster, new CommandEventHandler(Quests_OnCommand));

            LoadSettings();
        }

        public static bool FindItem(int x, int y, int z, Map map, int itemID)
        {
            return FindItem(new Point3D(x, y, z), map, itemID);
        }

        public static bool FindItem(int x, int y, int z, Map map, Item test)
        {
            return FindItem(new Point3D(x, y, z), map, test.ItemID);
        }

        public static bool FindItem(Point3D p, Map map, int itemID)
        {
            IPooledEnumerable eable = map.GetItemsInRange(p);

            foreach (Item item in eable)
            {
                if (item.Z == p.Z && item.ItemID == itemID)
                {
                    eable.Free();
                    return true;
                }
            }

            eable.Free();
            return false;
        }

        public static void LoadSettings()
        {
            if (!Directory.Exists("Data/Mondain's Legacy"))
                Directory.CreateDirectory("Data/Mondain's Legacy");

            if (!File.Exists("Data/Mondain's Legacy/Settings.xml"))
                File.Create("Data/Mondain's Legacy/Settings.xml");

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(Path.Combine(Core.BaseDirectory, "Data/Mondain's Legacy/Settings.xml"));

                XmlElement root = doc["Settings"];

                if (root == null)
                    return;

                ReadNode(root, "PalaceOfParoxysmus", ref m_PalaceOfParoxysmus);
                ReadNode(root, "TwistedWeald", ref m_TwistedWeald);
                ReadNode(root, "BlightedGrove", ref m_BlightedGrove);
                ReadNode(root, "Bedlam", ref m_Bedlam);
                ReadNode(root, "PrismOfLight", ref m_PrismOfLight);
                ReadNode(root, "Citadel", ref m_Citadel);
                ReadNode(root, "PaintedCaves", ref m_PaintedCaves);
                ReadNode(root, "Labyrinth", ref m_Labyrinth);
                ReadNode(root, "Sanctuary", ref m_Sanctuary);
                ReadNode(root, "StygianDragonLair", ref m_StygianDragonLair);
                ReadNode(root, "MedusasLair", ref m_MedusasLair);
                ReadNode(root, "Spellweaving", ref m_Spellweaving);
                ReadNode(root, "PublicDonations", ref m_PublicDonations);
            }
            catch
            {
            }

            if (Core.ML)
            {
                if (!FindItem(new Point3D(1431, 1696, 0), Map.Trammel, 0x307F))
                {
                    var addon = new ArcaneCircleAddon();
                    addon.MoveToWorld(new Point3D(1431, 1696, 0), Map.Trammel);
                }

                if (!FindItem(new Point3D(1431, 1696, 0), Map.Felucca, 0x307F))
                {
                    var addon = new ArcaneCircleAddon();
                    addon.MoveToWorld(new Point3D(1431, 1696, 0), Map.Felucca);
                }
            }
        }

        public static void SaveSetings()
        {
            if (!Directory.Exists("Data/Mondain's Legacy"))
                Directory.CreateDirectory("Data/Mondain's Legacy");

            if (!File.Exists("Data/Mondain's Legacy/Settings.xml"))
                File.Create("Data/Mondain's Legacy/Settings.xml");

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(Path.Combine(Core.BaseDirectory, "Data/Mondain's Legacy/Settings.xml"));

                XmlElement root = doc["Settings"];

                if (root == null)
                    return;

                UpdateNode(root, "PalaceOfParoxysmus", m_PalaceOfParoxysmus);
                UpdateNode(root, "TwistedWeald", m_TwistedWeald);
                UpdateNode(root, "BlightedGrove", m_BlightedGrove);
                UpdateNode(root, "Bedlam", m_Bedlam);
                UpdateNode(root, "PrismOfLight", m_PrismOfLight);
                UpdateNode(root, "Citadel", m_Citadel);
                UpdateNode(root, "PaintedCaves", m_PaintedCaves);
                UpdateNode(root, "Labyrinth", m_Labyrinth);
                UpdateNode(root, "Sanctuary", m_Sanctuary);
                UpdateNode(root, "StygianDragonLair", m_StygianDragonLair);
                UpdateNode(root, "MedusasLair", m_MedusasLair);
                UpdateNode(root, "Spellweaving", m_Spellweaving);
                UpdateNode(root, "PublicDonations", m_PublicDonations);

                doc.Save("Data/Mondain's Legacy/Settings.xml");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while updating 'Settings.xml': {0}", e);
            }
        }

        public static void ReadNode(XmlElement root, string dungeon, ref bool val)
        {
            if (root == null)
                return;

            foreach (XmlElement element in root.SelectNodes(dungeon))
            {
                if (element.HasAttribute("active"))
                    val = XmlConvert.ToBoolean(element.GetAttribute("active"));
            }
        }

        public static void UpdateNode(XmlElement root, string dungeon, bool val)
        {
            if (root == null)
                return;

            foreach (XmlElement element in root.SelectNodes(dungeon))
            {
                if (element.HasAttribute("active"))
                    element.SetAttribute("active", XmlConvert.ToString(val));
            }
        }

        public static void OnKilledBy(OnKilledByEventArgs e)
        {
            BaseCreature killed = e.Killed as BaseCreature;
            Mobile killer = e.KilledBy;

            if (killed != null && killed.GivesMLMinorArtifact && CheckArtifactChance(killer, killed))
            {
                MondainsLegacy.GiveArtifactTo(killer);
            }
        }

        public static bool CheckArtifactChance(Mobile m, BaseCreature bc)
        {
            if (!Core.ML || bc is BasePeerless) // Peerless drops to the corpse, this is handled elsewhere
                return false;

            return Paragon.CheckArtifactChance(m, bc);
        }

        public static void GiveArtifactTo(Mobile m)
        {
            Item item = Activator.CreateInstance(m_Artifacts[Utility.Random(m_Artifacts.Length)]) as Item;

            if (item == null)
                return;

			m.PlaySound(0x5B4);

            if (m.AddToBackpack(item))
            {
                m.SendLocalizedMessage(1072223); // An item has been placed in your backpack.
                m.SendLocalizedMessage(1062317); // For your valor in combating the fallen beast, a special artifact has been bestowed on you.
            }
            else if (m.BankBox.TryDropItem(m, item, false))
            {
                m.SendLocalizedMessage(1072224); // An item has been placed in your bank box.
                m.SendLocalizedMessage(1062317); // For your valor in combating the fallen beast, a special artifact has been bestowed on you.
            }
            else
            {
                // Item was placed at feet by m.AddToBackpack
                m.SendLocalizedMessage(1072523); // You find an artifact, but your backpack and bank are too full to hold it.
            }
        }

        public static void DropPeerlessMinor(Container peerlessCorpse)
        {
            Item item = Activator.CreateInstance(m_Artifacts[Utility.Random(m_Artifacts.Length)]) as Item;

            if (item is ICanBeElfOrHuman)
                ((ICanBeElfOrHuman)item).ElfOnly = false;

            peerlessCorpse.DropItem(item);
        }

        public static bool CheckML(Mobile from)
        {
            return CheckML(from, true);
        }

        public static bool CheckML(Mobile from, bool message)
        {
            if (from == null || from.NetState == null)
                return false;

            if (from.NetState.SupportsExpansion(Expansion.ML))
                return true;

            if (message)
                from.SendLocalizedMessage(1072791); // You must upgrade to Mondain's Legacy in order to use that item.

            return false;
        }

        public static bool IsMLRegion(Region region)
        {
            return region.IsPartOf("Twisted Weald") ||
                   region.IsPartOf("Sanctuary") ||
                   region.IsPartOf("Prism of Light") ||
                   region.IsPartOf("TheCitadel") ||
                   region.IsPartOf("Bedlam") ||
                   region.IsPartOf("Blighted Grove") ||
                   region.IsPartOf("Painted Caves") ||
                   region.IsPartOf("Palace of Paroxysmus") ||
                   region.IsPartOf("Labyrinth");
        }

		[Usage("DecorateMLDelete")]
		[Description("Deletes Mondain's Legacy world decoration.")]
		private static void DecorateMLDelete_OnCommand(CommandEventArgs e)
		{
			WeakEntityCollection.Delete("ml");
		}

		[Usage("DecorateML")]
        [Description("Generates Mondain's Legacy world decoration.")]
        private static void DecorateML_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendMessage("Generating Mondain's Legacy world decoration, please wait.");

            Decorate.Generate("ml", "Data/Mondain's Legacy/Trammel", Map.Trammel);
            Decorate.Generate("ml", "Data/Mondain's Legacy/Felucca", Map.Felucca);
            Decorate.Generate("ml", "Data/Mondain's Legacy/Ilshenar", Map.Ilshenar);
            Decorate.Generate("ml", "Data/Mondain's Legacy/Malas", Map.Malas);
            Decorate.Generate("ml", "Data/Mondain's Legacy/Tokuno", Map.Tokuno);
            Decorate.Generate("ml", "Data/Mondain's Legacy/TerMur", Map.TerMur);

            PeerlessAltar altar;
            PeerlessTeleporter tele;
            PrismOfLightPillar pillar;

            // Bedlam - Malas
            altar = new BedlamAltar();

            if (!FindItem(86, 1627, 0, Map.Malas, altar))
            {
				WeakEntityCollection.Add("ml", altar);
				altar.MoveToWorld(new Point3D(86, 1627, 0), Map.Malas);
                tele = new PeerlessTeleporter(altar);
				WeakEntityCollection.Add("ml", tele);
				tele.PointDest = altar.ExitDest;
                tele.MoveToWorld(new Point3D(99, 1617, 50), Map.Malas);
            }

            // Blighted Grove - Trammel
            altar = new BlightedGroveAltar();

            if (!FindItem(6502, 875, 0, Map.Trammel, altar))
            {
				WeakEntityCollection.Add("ml", altar);
				altar.MoveToWorld(new Point3D(6502, 875, 0), Map.Trammel);
                tele = new PeerlessTeleporter(altar);
				WeakEntityCollection.Add("ml", tele);
				tele.PointDest = altar.ExitDest;
                tele.MoveToWorld(new Point3D(6511, 949, 26), Map.Trammel);
            }

            // Blighted Grove - Felucca
            altar = new BlightedGroveAltar();

            if (!FindItem(6502, 875, 0, Map.Felucca, altar))
            {
				WeakEntityCollection.Add("ml", altar);
				altar.MoveToWorld(new Point3D(6502, 875, 0), Map.Felucca);
                tele = new PeerlessTeleporter(altar);
				WeakEntityCollection.Add("ml", tele);
				tele.PointDest = altar.ExitDest;
                tele.MoveToWorld(new Point3D(6511, 949, 26), Map.Felucca);
            }

            // Palace of Paroxysmus - Trammel
            altar = new ParoxysmusAltar();

            if (!FindItem(6511, 506, -34, Map.Trammel, altar))
            {
				WeakEntityCollection.Add("ml", altar);
				altar.MoveToWorld(new Point3D(6511, 506, -34), Map.Trammel);
                tele = new PeerlessTeleporter(altar);
				WeakEntityCollection.Add("ml", tele);
				tele.PointDest = altar.ExitDest;
                tele.MoveToWorld(new Point3D(6518, 365, 46), Map.Trammel);
            }

            // Palace of Paroxysmus - Felucca
            altar = new ParoxysmusAltar();

            if (!FindItem(6511, 506, -34, Map.Felucca, altar))
            {
				WeakEntityCollection.Add("ml", altar);
				altar.MoveToWorld(new Point3D(6511, 506, -34), Map.Felucca);
                tele = new PeerlessTeleporter(altar);
				WeakEntityCollection.Add("ml", tele);
				tele.PointDest = altar.ExitDest;
                tele.MoveToWorld(new Point3D(6518, 365, 46), Map.Felucca);
            }

            // Prism of Light - Trammel
            altar = new PrismOfLightAltar();

            if (!FindItem(6509, 167, 6, Map.Trammel, altar))
            {
				WeakEntityCollection.Add("ml", altar);
				altar.MoveToWorld(new Point3D(6509, 167, 6), Map.Trammel);
                tele = new PeerlessTeleporter(altar);
				WeakEntityCollection.Add("ml", tele);
				tele.PointDest = altar.ExitDest;
                tele.Visible = true;
                tele.ItemID = 0xDDA;
                tele.MoveToWorld(new Point3D(6501, 137, -20), Map.Trammel);

                pillar = new PrismOfLightPillar((PrismOfLightAltar)altar, 0x581);
				WeakEntityCollection.Add("ml", pillar);
				pillar.MoveToWorld(new Point3D(6506, 167, 0), Map.Trammel);

                pillar = new PrismOfLightPillar((PrismOfLightAltar)altar, 0x581);
				WeakEntityCollection.Add("ml", pillar);
				pillar.MoveToWorld(new Point3D(6509, 164, 0), Map.Trammel);

                pillar = new PrismOfLightPillar((PrismOfLightAltar)altar, 0x581);
				WeakEntityCollection.Add("ml", pillar);
				pillar.MoveToWorld(new Point3D(6506, 164, 0), Map.Trammel);

                pillar = new PrismOfLightPillar((PrismOfLightAltar)altar, 0x481);
				WeakEntityCollection.Add("ml", pillar);
				pillar.MoveToWorld(new Point3D(6512, 167, 0), Map.Trammel);

                pillar = new PrismOfLightPillar((PrismOfLightAltar)altar, 0x481);
				WeakEntityCollection.Add("ml", pillar);
				pillar.MoveToWorld(new Point3D(6509, 170, 0), Map.Trammel);

                pillar = new PrismOfLightPillar((PrismOfLightAltar)altar, 0x481);
				WeakEntityCollection.Add("ml", pillar);
				pillar.MoveToWorld(new Point3D(6512, 170, 0), Map.Trammel);
            }

            // Prism of Light - Felucca
            altar = new PrismOfLightAltar();

            if (!FindItem(6509, 167, 6, Map.Felucca, altar))
            {
				WeakEntityCollection.Add("ml", altar);
				altar.MoveToWorld(new Point3D(6509, 167, 6), Map.Felucca);
                tele = new PeerlessTeleporter(altar);
				WeakEntityCollection.Add("ml", tele);
				tele.PointDest = altar.ExitDest;
                tele.Visible = true;
                tele.ItemID = 0xDDA;
                tele.MoveToWorld(new Point3D(6501, 137, -20), Map.Felucca);

                pillar = new PrismOfLightPillar((PrismOfLightAltar)altar, 0x581);
				WeakEntityCollection.Add("ml", pillar);
				pillar.MoveToWorld(new Point3D(6506, 167, 0), Map.Felucca);

                pillar = new PrismOfLightPillar((PrismOfLightAltar)altar, 0x581);
				WeakEntityCollection.Add("ml", pillar);
				pillar.MoveToWorld(new Point3D(6509, 164, 0), Map.Felucca);

                pillar = new PrismOfLightPillar((PrismOfLightAltar)altar, 0x581);
				WeakEntityCollection.Add("ml", pillar);
				pillar.MoveToWorld(new Point3D(6506, 164, 0), Map.Felucca);

                pillar = new PrismOfLightPillar((PrismOfLightAltar)altar, 0x481);
				WeakEntityCollection.Add("ml", pillar);
				pillar.MoveToWorld(new Point3D(6512, 167, 0), Map.Felucca);

                pillar = new PrismOfLightPillar((PrismOfLightAltar)altar, 0x481);
				WeakEntityCollection.Add("ml", pillar);
				pillar.MoveToWorld(new Point3D(6509, 170, 0), Map.Felucca);

                pillar = new PrismOfLightPillar((PrismOfLightAltar)altar, 0x481);
				WeakEntityCollection.Add("ml", pillar);
				pillar.MoveToWorld(new Point3D(6512, 170, 0), Map.Felucca);
            }

            // The Citadel - Malas
            altar = new CitadelAltar();

            if (!FindItem(89, 1885, 0, Map.Malas, altar))
            {
				WeakEntityCollection.Add("ml", altar);
				altar.MoveToWorld(new Point3D(89, 1885, 0), Map.Malas);
                tele = new PeerlessTeleporter(altar);
				WeakEntityCollection.Add("ml", tele);
				tele.PointDest = altar.ExitDest;
                tele.MoveToWorld(new Point3D(111, 1955, 0), Map.Malas);
            }

            // Twisted Weald - Ilshenar
            altar = new TwistedWealdAltar();

            if (!FindItem(2170, 1255, -60, Map.Ilshenar, altar))
            {
				WeakEntityCollection.Add("ml", altar);
				altar.MoveToWorld(new Point3D(2170, 1255, -60), Map.Ilshenar);
                tele = new PeerlessTeleporter(altar);
				WeakEntityCollection.Add("ml", tele);
				tele.PointDest = altar.ExitDest;
                tele.MoveToWorld(new Point3D(2139, 1271, -57), Map.Ilshenar);
            }

            // Stygian Dragon Lair - Abyss
            StygianDragonPlatform sAltar = new StygianDragonPlatform();

            if (!FindItem(363, 157, 5, Map.TerMur, sAltar))
            {
				WeakEntityCollection.Add("ml", sAltar);
				sAltar.MoveToWorld(new Point3D(363, 157, 0), Map.TerMur);

            }

            //Medusa Lair - Abyss
            MedusaPlatform mAltar = new MedusaPlatform();

            if (!FindItem(822, 756, 56, Map.TerMur, mAltar))
            {
				WeakEntityCollection.Add("ml", sAltar);
				mAltar.MoveToWorld(new Point3D(822, 756, 56), Map.TerMur);
            }

            e.Mobile.SendMessage("Mondain's Legacy world generating complete.");
        }

        [Usage("SettingsML")]
        [Description("Mondain's Legacy Settings.")]
        private static void SettingsML_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendGump(new MondainsLegacyGump());
        }

        [Usage("Quests")]
        [Description("Pops up a quest list from targeted player.")]
        private static void Quests_OnCommand(CommandEventArgs e)
        {
            Mobile m = e.Mobile;
            m.SendMessage("Target a player to view their quests.");

            m.BeginTarget(-1, false, Server.Targeting.TargetFlags.None, new TargetCallback(
                delegate(Mobile from, object targeted)
                {
                    if (targeted is PlayerMobile)
                        m.SendGump(new MondainQuestGump((PlayerMobile)targeted));
                    else
                        m.SendMessage("That is not a player!");
                }));
        }
    }

	public class MondainsLegacyGump : Gump
	{
		public MondainsLegacyGump()
			: base(50, 50)
		{
			this.Closable = true;
			this.Disposable = true;
			this.Dragable = true;
			this.Resizable = false;

			this.AddPage(0);
			this.AddBackground(0, 0, 308, 390, 0x2454);

			// title
			this.AddLabel(125, 10, 150, "Settings");
			this.AddImage(256, 5, 0x9E1);

			// dungeons			
			this.AddButton(20, 60, MondainsLegacy.PalaceOfParoxysmus ? 0x939 : 0x938, MondainsLegacy.PalaceOfParoxysmus ? 0x939 : 0x938, 1, GumpButtonType.Reply, 0);
			this.AddButton(20, 85, MondainsLegacy.TwistedWeald ? 0x939 : 0x938, MondainsLegacy.TwistedWeald ? 0x939 : 0x938, 2, GumpButtonType.Reply, 0);
			this.AddButton(20, 110, MondainsLegacy.BlightedGrove ? 0x939 : 0x938, MondainsLegacy.BlightedGrove ? 0x939 : 0x938, 3, GumpButtonType.Reply, 0);
			this.AddButton(20, 135, MondainsLegacy.Bedlam ? 0x939 : 0x938, MondainsLegacy.Bedlam ? 0x939 : 0x938, 4, GumpButtonType.Reply, 0);
			this.AddButton(20, 160, MondainsLegacy.PrismOfLight ? 0x939 : 0x938, MondainsLegacy.PrismOfLight ? 0x939 : 0x938, 5, GumpButtonType.Reply, 0);
			this.AddButton(20, 185, MondainsLegacy.Citadel ? 0x939 : 0x938, MondainsLegacy.Citadel ? 0x939 : 0x938, 6, GumpButtonType.Reply, 0);
			this.AddButton(20, 210, MondainsLegacy.PaintedCaves ? 0x939 : 0x938, MondainsLegacy.PaintedCaves ? 0x939 : 0x938, 7, GumpButtonType.Reply, 0);
			this.AddButton(20, 235, MondainsLegacy.Labyrinth ? 0x939 : 0x938, MondainsLegacy.Labyrinth ? 0x939 : 0x938, 8, GumpButtonType.Reply, 0);
			this.AddButton(20, 260, MondainsLegacy.Sanctuary ? 0x939 : 0x938, MondainsLegacy.Sanctuary ? 0x939 : 0x938, 9, GumpButtonType.Reply, 0);
			this.AddButton(20, 285, MondainsLegacy.StygianDragonLair ? 0x939 : 0x938, MondainsLegacy.StygianDragonLair ? 0x939 : 0x938, 10, GumpButtonType.Reply, 0);
			this.AddButton(20, 310, MondainsLegacy.MedusasLair ? 0x939 : 0x938, MondainsLegacy.MedusasLair ? 0x939 : 0x938, 11, GumpButtonType.Reply, 0);
			this.AddButton(20, 335, MondainsLegacy.Spellweaving ? 0x939 : 0x938, MondainsLegacy.Spellweaving ? 0x939 : 0x938, 12, GumpButtonType.Reply, 0);
			this.AddButton(20, 360, MondainsLegacy.PublicDonations ? 0x939 : 0x938, MondainsLegacy.PublicDonations ? 0x939 : 0x938, 13, GumpButtonType.Reply, 0);

			this.AddLabel(45, 56, 0x226, "Palace of Paroxysmus");
			this.AddLabel(45, 81, 0x226, "Twisted Weald");
			this.AddLabel(45, 106, 0x226, "Blighted Grove");
			this.AddLabel(45, 131, 0x226, "Bedlam");
			this.AddLabel(45, 156, 0x226, "Prism of Light");
			this.AddLabel(45, 181, 0x226, "The Citadel");
			this.AddLabel(45, 206, 0x226, "Painted Caves");
			this.AddLabel(45, 231, 0x226, "Labyrinth");
			this.AddLabel(45, 256, 0x226, "Sanctuary");
			this.AddLabel(45, 281, 0x226, "StygianDragonLair");
			this.AddLabel(45, 306, 0x226, "MedusasLair");
			this.AddLabel(45, 331, 0x226, "Spellweaving");
			this.AddLabel(45, 356, 0x226, "PublicDonations");

			// legend
			this.AddLabel(243, 205, 0x226, "Legend:");

			this.AddImage(218, 235, 0x938);
			this.AddLabel(243, 231, 0x226, "disabled");
			this.AddImage(218, 260, 0x939);
			this.AddLabel(243, 256, 0x226, "enabled");
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			switch (info.ButtonID)
			{
				case 0:
					MondainsLegacy.SaveSetings();
					break;
				case 1:
					MondainsLegacy.PalaceOfParoxysmus = !MondainsLegacy.PalaceOfParoxysmus;
					break;
				case 2:
					MondainsLegacy.TwistedWeald = !MondainsLegacy.TwistedWeald;
					break;
				case 3:
					MondainsLegacy.BlightedGrove = !MondainsLegacy.BlightedGrove;
					break;
				case 4:
					MondainsLegacy.Bedlam = !MondainsLegacy.Bedlam;
					break;
				case 5:
					MondainsLegacy.PrismOfLight = !MondainsLegacy.PrismOfLight;
					break;
				case 6:
					MondainsLegacy.Citadel = !MondainsLegacy.Citadel;
					break;
				case 7:
					MondainsLegacy.PaintedCaves = !MondainsLegacy.PaintedCaves;
					break;
				case 8:
					MondainsLegacy.Labyrinth = !MondainsLegacy.Labyrinth;
					break;
				case 9:
					MondainsLegacy.Sanctuary = !MondainsLegacy.Sanctuary;
					break;
				case 10:
					MondainsLegacy.StygianDragonLair = !MondainsLegacy.StygianDragonLair;
					break;
				case 11:
					MondainsLegacy.MedusasLair = !MondainsLegacy.MedusasLair;
					break;
				case 12:
					MondainsLegacy.Spellweaving = !MondainsLegacy.Spellweaving;
					break;
				case 13:
					MondainsLegacy.PublicDonations = !MondainsLegacy.PublicDonations;
					break;
			}

			if (info.ButtonID > 0)
				sender.Mobile.SendGump(new MondainsLegacyGump());
		}
	}
}