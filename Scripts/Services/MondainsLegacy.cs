using Server.Commands;
using Server.Engines.InstancedPeerless;
using Server.Items;
using Server.Mobiles;
using System;
using Server.Misc;

namespace Server
{
    public static class MondainsLegacy
    {
        public static Type[] Artifacts => m_Artifacts;
        private static readonly Type[] m_Artifacts = new Type[]
        {
            typeof(AegisOfGrace), typeof(BladeDance), typeof(BloodwoodSpirit), typeof(Bonesmasher),
            typeof(Boomstick), typeof(BrightsightLenses), typeof(FeyLeggings), typeof(FleshRipper),
            typeof(HelmOfSwiftness), typeof(PadsOfTheCuSidhe), typeof(QuiverOfRage), typeof(QuiverOfElements),
            typeof(RaedsGlory), typeof(RighteousAnger), typeof(RobeOfTheEclipse), typeof(RobeOfTheEquinox),
            typeof(SoulSeeker), typeof(TalonBite), typeof(TotemOfVoid), typeof(WildfireBow),
            typeof(Windsong)
        };

        public static void Initialize()
        {
            EventSink.OnKilledBy += OnKilledBy;

            CommandSystem.Register("DecorateML", AccessLevel.Administrator, DecorateML_OnCommand);
            CommandSystem.Register("DecorateMLDelete", AccessLevel.Administrator, DecorateMLDelete_OnCommand);

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
            if (!FindItem(new Point3D(1431, 1696, 0), Map.Trammel, 0x307F))
            {
                ArcaneCircleAddon addon = new ArcaneCircleAddon();
                addon.MoveToWorld(new Point3D(1431, 1696, 0), Map.Trammel);
            }

            if (!FindItem(new Point3D(1431, 1696, 0), Map.Felucca, 0x307F))
            {
                ArcaneCircleAddon addon = new ArcaneCircleAddon();
                addon.MoveToWorld(new Point3D(1431, 1696, 0), Map.Felucca);
            }
        }

        public static void OnKilledBy(OnKilledByEventArgs e)
        {
            BaseCreature killed = e.Killed as BaseCreature;
            Mobile killer = e.KilledBy;

            if (killed != null && killer != null && killer.Alive && killed.GivesMLMinorArtifact && CheckArtifactChance(killer, killed))
            {
                GiveArtifactTo(killer);
            }
        }

        public static bool CheckArtifactChance(Mobile m, BaseCreature bc)
        {
            if (bc is BasePeerless) // Peerless drops to the corpse, this is handled elsewhere
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

        public static bool IsMLRegion(Region region)
        {
            return region.IsPartOf("Twisted Weald") || region.IsPartOf("Sanctuary") ||
                   region.IsPartOf("Prism of Light") || region.IsPartOf("TheCitadel") ||
                   region.IsPartOf("Bedlam") || region.IsPartOf("Blighted Grove") ||
                   region.IsPartOf("Painted Caves") || region.IsPartOf("Palace of Paroxysmus") ||
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
            ParoxysmusIronGate gate;

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
                gate = new ParoxysmusIronGate(altar);
                WeakEntityCollection.Add("ml", gate);
                gate.MoveToWorld(new Point3D(6518, 492, -50), Map.Trammel);
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
                gate = new ParoxysmusIronGate(altar);
                WeakEntityCollection.Add("ml", gate);
                gate.MoveToWorld(new Point3D(6518, 492, -50), Map.Felucca);
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
    }
}
