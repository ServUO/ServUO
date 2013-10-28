using System;
using System.IO;
using System.Xml;
using Server.Commands;
using Server.Engines.Quests;
using Server.Items;
using Server.Mobiles;

namespace Server
{
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
            CommandSystem.Register("DecorateML", AccessLevel.Administrator, new CommandEventHandler(DecorateML_OnCommand));
            CommandSystem.Register("SettingsML", AccessLevel.Administrator, new CommandEventHandler(SettingsML_OnCommand));
            CommandSystem.Register("Quests", AccessLevel.GameMaster, new CommandEventHandler(Quests_OnCommand));

            LoadSettings();
        }

        public static bool FindItem(int x, int y, int z, Map map, Item test)
        {
            return FindItem(new Point3D(x, y, z), map, test);
        }

        public static bool FindItem(Point3D p, Map map, Item test)
        {
            IPooledEnumerable eable = map.GetItemsInRange(p);

            foreach (Item item in eable)
            {
                if (item.Z == p.Z && item.ItemID == test.ItemID)
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

        public static bool CheckArtifactChance(Mobile m, BaseCreature bc)
        {
            if (!Core.ML)
                return false;

            return Paragon.CheckArtifactChance(m, bc);
        }

        public static void GiveArtifactTo(Mobile m)
        {
            Item item = Activator.CreateInstance(m_Artifacts[Utility.Random(m_Artifacts.Length)]) as Item;

            if (item == null)
                return;

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
                   region.IsPartOf("Citadel") ||
                   region.IsPartOf("Bedlam") ||
                   region.IsPartOf("Blighted Grove") ||
                   region.IsPartOf("Painted Caves") ||
                   region.IsPartOf("Palace of Paroxysmus") ||
                   region.IsPartOf("Labyrinth");
        }

        [Usage("DecorateML")]
        [Description("Generates Mondain's Legacy world decoration.")]
        private static void DecorateML_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendMessage("Generating Mondain's Legacy world decoration, please wait.");

            Decorate.Generate("Data/Mondain's Legacy/Trammel", Map.Trammel);
            Decorate.Generate("Data/Mondain's Legacy/Felucca", Map.Felucca);
            Decorate.Generate("Data/Mondain's Legacy/Ilshenar", Map.Ilshenar);
            Decorate.Generate("Data/Mondain's Legacy/Malas", Map.Malas);
            Decorate.Generate("Data/Mondain's Legacy/Tokuno", Map.Tokuno);
            Decorate.Generate("Data/Mondain's Legacy/TerMur", Map.TerMur);

            PeerlessAltar altar;
            PeerlessTeleporter tele;
            PrismOfLightPillar pillar;
            StygianDragonBrazier brazier;
            MedusaNest nest;

            // Bedlam - Malas
            altar = new BedlamAltar();

            if (!FindItem(86, 1627, 0, Map.Malas, altar))
            {
                altar = new BedlamAltar();
                altar.MoveToWorld(new Point3D(86, 1627, 0), Map.Malas);
                tele = new PeerlessTeleporter(altar);
                tele.PointDest = altar.ExitDest;
                tele.MoveToWorld(new Point3D(99, 1617, 50), Map.Malas);
            }

            // Blighted Grove - Trammel
            altar = new BlightedGroveAltar();

            if (!FindItem(6502, 875, 0, Map.Trammel, altar))
            {
                altar.MoveToWorld(new Point3D(6502, 875, 0), Map.Trammel);
                tele = new PeerlessTeleporter(altar);
                tele.PointDest = altar.ExitDest;
                tele.MoveToWorld(new Point3D(6511, 949, 26), Map.Trammel);
            }

            // Blighted Grove - Felucca
            altar = new BlightedGroveAltar();

            if (!FindItem(6502, 875, 0, Map.Felucca, altar))
            {
                altar.MoveToWorld(new Point3D(6502, 875, 0), Map.Felucca);
                tele = new PeerlessTeleporter(altar);
                tele.PointDest = altar.ExitDest;
                tele.MoveToWorld(new Point3D(6511, 949, 26), Map.Felucca);
            }

            // Palace of Paroxysmus - Trammel
            altar = new ParoxysmusAltar();

            if (!FindItem(6511, 506, -34, Map.Trammel, altar))
            {
                altar.MoveToWorld(new Point3D(6511, 506, -34), Map.Trammel);
                tele = new PeerlessTeleporter(altar);
                tele.PointDest = altar.ExitDest;
                tele.MoveToWorld(new Point3D(6518, 365, 46), Map.Trammel);
            }

            // Palace of Paroxysmus - Felucca
            altar = new ParoxysmusAltar();

            if (!FindItem(6511, 506, -34, Map.Felucca, altar))
            {
                altar.MoveToWorld(new Point3D(6511, 506, -34), Map.Felucca);
                tele = new PeerlessTeleporter(altar);
                tele.PointDest = altar.ExitDest;
                tele.MoveToWorld(new Point3D(6518, 365, 46), Map.Felucca);
            }

            // Prism of Light - Trammel
            altar = new PrismOfLightAltar();

            if (!FindItem(6509, 167, 6, Map.Trammel, altar))
            {
                altar.MoveToWorld(new Point3D(6509, 167, 6), Map.Trammel);
                tele = new PeerlessTeleporter(altar);
                tele.PointDest = altar.ExitDest;
                tele.Visible = true;
                tele.ItemID = 0xDDA;
                tele.MoveToWorld(new Point3D(6501, 137, -20), Map.Trammel);

                pillar = new PrismOfLightPillar((PrismOfLightAltar)altar, 0x581);
                pillar.MoveToWorld(new Point3D(6506, 167, 0), Map.Trammel);

                pillar = new PrismOfLightPillar((PrismOfLightAltar)altar, 0x581);
                pillar.MoveToWorld(new Point3D(6509, 164, 0), Map.Trammel);

                pillar = new PrismOfLightPillar((PrismOfLightAltar)altar, 0x581);
                pillar.MoveToWorld(new Point3D(6506, 164, 0), Map.Trammel);

                pillar = new PrismOfLightPillar((PrismOfLightAltar)altar, 0x481);
                pillar.MoveToWorld(new Point3D(6512, 167, 0), Map.Trammel);

                pillar = new PrismOfLightPillar((PrismOfLightAltar)altar, 0x481);
                pillar.MoveToWorld(new Point3D(6509, 170, 0), Map.Trammel);

                pillar = new PrismOfLightPillar((PrismOfLightAltar)altar, 0x481);
                pillar.MoveToWorld(new Point3D(6512, 170, 0), Map.Trammel);
            }

            // Prism of Light - Felucca
            altar = new PrismOfLightAltar();

            if (!FindItem(6509, 167, 6, Map.Felucca, altar))
            {
                altar.MoveToWorld(new Point3D(6509, 167, 6), Map.Felucca);
                tele = new PeerlessTeleporter(altar);
                tele.PointDest = altar.ExitDest;
                tele.Visible = true;
                tele.ItemID = 0xDDA;
                tele.MoveToWorld(new Point3D(6501, 137, -20), Map.Felucca);

                pillar = new PrismOfLightPillar((PrismOfLightAltar)altar, 0x581);
                pillar.MoveToWorld(new Point3D(6506, 167, 0), Map.Felucca);

                pillar = new PrismOfLightPillar((PrismOfLightAltar)altar, 0x581);
                pillar.MoveToWorld(new Point3D(6509, 164, 0), Map.Felucca);

                pillar = new PrismOfLightPillar((PrismOfLightAltar)altar, 0x581);
                pillar.MoveToWorld(new Point3D(6506, 164, 0), Map.Felucca);

                pillar = new PrismOfLightPillar((PrismOfLightAltar)altar, 0x481);
                pillar.MoveToWorld(new Point3D(6512, 167, 0), Map.Felucca);

                pillar = new PrismOfLightPillar((PrismOfLightAltar)altar, 0x481);
                pillar.MoveToWorld(new Point3D(6509, 170, 0), Map.Felucca);

                pillar = new PrismOfLightPillar((PrismOfLightAltar)altar, 0x481);
                pillar.MoveToWorld(new Point3D(6512, 170, 0), Map.Felucca);
            }

            // The Citadel - Malas
            altar = new CitadelAltar();

            if (!FindItem(89, 1885, 0, Map.Malas, altar))
            {
                altar.MoveToWorld(new Point3D(89, 1885, 0), Map.Malas);
                tele = new PeerlessTeleporter(altar);
                tele.PointDest = altar.ExitDest;
                tele.MoveToWorld(new Point3D(111, 1955, 0), Map.Malas);
            }

            // Twisted Weald - Ilshenar
            altar = new TwistedWealdAltar();

            if (!FindItem(2170, 1255, -60, Map.Ilshenar, altar))
            {
                altar.MoveToWorld(new Point3D(2170, 1255, -60), Map.Ilshenar);
                tele = new PeerlessTeleporter(altar);
                tele.PointDest = altar.ExitDest;
                tele.MoveToWorld(new Point3D(2139, 1271, -57), Map.Ilshenar);
            }

            // Stygian Dragon Lair - Abyss
            altar = new StygianDragonAltar();

            if (!FindItem(363, 157, 5, Map.TerMur, altar))
            {
                altar.MoveToWorld(new Point3D(363, 157, 0), Map.TerMur);
                tele = new PeerlessTeleporter(altar);
                tele.PointDest = altar.ExitDest;

                tele.MoveToWorld(new Point3D(305, 159, 105), Map.TerMur);

                brazier = new StygianDragonBrazier((StygianDragonAltar)altar, 0x207B);
                brazier.MoveToWorld(new Point3D(362, 156, 5), Map.TerMur);

                brazier = new StygianDragonBrazier((StygianDragonAltar)altar, 0x207B);
                brazier.MoveToWorld(new Point3D(364, 156, 7), Map.TerMur);

                brazier = new StygianDragonBrazier((StygianDragonAltar)altar, 0x207B);
                brazier.MoveToWorld(new Point3D(364, 158, 7), Map.TerMur);

                brazier = new StygianDragonBrazier((StygianDragonAltar)altar, 0x207B);
                brazier.MoveToWorld(new Point3D(362, 158, 7), Map.TerMur);
            }

            //Medusa Lair - Abyss
            altar = new MedusaAltar();

            if (!FindItem(822, 756, 56, Map.TerMur, altar))
            {
                altar.MoveToWorld(new Point3D(822, 756, 56), Map.TerMur);
                tele = new PeerlessTeleporter(altar);
                tele.PointDest = altar.ExitDest;

                tele.MoveToWorld(new Point3D(840, 926, -5), Map.TerMur);

                nest = new MedusaNest((MedusaAltar)altar, 0x207B);
                nest.MoveToWorld(new Point3D(821, 755, 56), Map.TerMur);

                nest = new MedusaNest((MedusaAltar)altar, 0x207B);
                nest.MoveToWorld(new Point3D(823, 755, 56), Map.TerMur);

                nest = new MedusaNest((MedusaAltar)altar, 0x207B);
                nest.MoveToWorld(new Point3D(821, 757, 56), Map.TerMur);

                nest = new MedusaNest((MedusaAltar)altar, 0x207B);
                nest.MoveToWorld(new Point3D(823, 757, 56), Map.TerMur);
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
}