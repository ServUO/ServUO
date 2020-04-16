using Server.Gumps;
using Server.Items;
using Server.Network;

namespace Server.Engines.Plants
{
    public class ReproductionGump : Gump
    {
        private readonly PlantItem m_Plant;
        public ReproductionGump(PlantItem plant)
            : base(20, 20)
        {
            m_Plant = plant;

            DrawBackground();

            AddButton(70, 67, 0xD4, 0xD4, 1, GumpButtonType.Reply, 0); // Main menu
            AddItem(57, 65, 0x1600);

            AddLabel(108, 67, 0x835, "Reproduction");

            if (m_Plant.PlantStatus == PlantStatus.Stage9)
            {
                AddButton(212, 67, 0xD4, 0xD4, 2, GumpButtonType.Reply, 0); // Set to decorative
                AddItem(202, 68, 0xC61);
                AddLabel(216, 66, 0x21, "/");
            }

            AddButton(80, 116, 0xD4, 0xD4, 3, GumpButtonType.Reply, 0); // Pollination
            AddItem(66, 117, 0x1AA2);
            AddPollinationState(106, 116);

            AddButton(128, 116, 0xD4, 0xD4, 4, GumpButtonType.Reply, 0); // Resources
            AddItem(113, 120, 0x1021);
            AddResourcesState(149, 116);

            AddButton(177, 116, 0xD4, 0xD4, 5, GumpButtonType.Reply, 0); // Seeds
            AddItem(160, 121, 0xDCF);
            AddSeedsState(199, 116);

            AddButton(70, 163, 0xD2, 0xD2, 6, GumpButtonType.Reply, 0); // Gather pollen
            AddItem(56, 164, 0x1AA2);

            AddButton(138, 163, 0xD2, 0xD2, 7, GumpButtonType.Reply, 0); // Gather resources
            AddItem(123, 167, 0x1021);

            AddButton(212, 163, 0xD2, 0xD2, 8, GumpButtonType.Reply, 0); // Gather seeds
            AddItem(195, 168, 0xDCF);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            if (info.ButtonID == 0 || m_Plant.Deleted || m_Plant.PlantStatus >= PlantStatus.DecorativePlant || m_Plant.PlantStatus == PlantStatus.BowlOfDirt)
                return;

            if ((info.ButtonID >= 6 && info.ButtonID <= 8) && !from.InRange(m_Plant.GetWorldLocation(), 3))
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3E9, 500446); // That is too far away.
                return;
            }

            if (!m_Plant.IsUsableBy(from))
            {
                m_Plant.LabelTo(from, 1061856); // You must have the item in your backpack or locked down in order to use it.
                return;
            }

            switch (info.ButtonID)
            {
                case 1: // Main menu
                    {
                        from.SendGump(new MainPlantGump(m_Plant));

                        break;
                    }
                case 2: // Set to decorative
                    {
                        if (m_Plant.PlantStatus == PlantStatus.Stage9)
                        {
                            from.SendGump(new SetToDecorativeGump(m_Plant));
                        }

                        break;
                    }
                case 3: // Pollination
                    {
                        from.Send(new DisplayHelpTopic(67, true)); // POLLINATION STATE

                        from.SendGump(new ReproductionGump(m_Plant));

                        break;
                    }
                case 4: // Resources
                    {
                        from.Send(new DisplayHelpTopic(69, true)); // RESOURCE PRODUCTION

                        from.SendGump(new ReproductionGump(m_Plant));

                        break;
                    }
                case 5: // Seeds
                    {
                        from.Send(new DisplayHelpTopic(68, true)); // SEED PRODUCTION

                        from.SendGump(new ReproductionGump(m_Plant));

                        break;
                    }
                case 6: // Gather pollen
                    {
                        if (!m_Plant.IsCrossable)
                        {
                            m_Plant.LabelTo(from, 1053050); // You cannot gather pollen from a mutated plant!
                        }
                        else if (!m_Plant.PlantSystem.PollenProducing)
                        {
                            m_Plant.LabelTo(from, 1053051); // You cannot gather pollen from a plant in this stage of development!
                        }
                        else if (m_Plant.PlantSystem.Health < PlantHealth.Healthy)
                        {
                            m_Plant.LabelTo(from, 1053052); // You cannot gather pollen from an unhealthy plant!
                        }
                        else
                        {
                            from.Target = new PollinateTarget(m_Plant);
                            from.SendLocalizedMessage(1053054); // Target the plant you wish to cross-pollinate to.

                            break;
                        }

                        from.SendGump(new ReproductionGump(m_Plant));

                        break;
                    }
                case 7: // Gather resources
                    {
                        PlantResourceInfo resInfo = PlantResourceInfo.GetInfo(m_Plant.PlantType, m_Plant.PlantHue);
                        PlantSystem system = m_Plant.PlantSystem;

                        if (resInfo == null)
                        {
                            if (m_Plant.IsCrossable)
                                m_Plant.LabelTo(from, 1053056); // This plant has no resources to gather!
                            else
                                m_Plant.LabelTo(from, 1053055); // Mutated plants do not produce resources!
                        }
                        else if (system.AvailableResources == 0)
                        {
                            m_Plant.LabelTo(from, 1053056); // This plant has no resources to gather!
                        }
                        else
                        {
                            Item resource = resInfo.CreateResource();

                            if (from.PlaceInBackpack(resource))
                            {
                                system.AvailableResources--;
                                m_Plant.LabelTo(from, 1053059); // You gather resources from the plant.
                            }
                            else
                            {
                                resource.Delete();
                                m_Plant.LabelTo(from, 1053058); // You attempt to gather as many resources as you can hold, but your backpack is full.
                            }
                        }

                        from.SendGump(new ReproductionGump(m_Plant));

                        break;
                    }
                case 8: // Gather seeds
                    {
                        PlantSystem system = m_Plant.PlantSystem;

                        if (!m_Plant.IsCrossable)
                        {
                            m_Plant.LabelTo(from, 1053060); // Mutated plants do not produce seeds!
                        }
                        else if (system.AvailableSeeds == 0)
                        {
                            m_Plant.LabelTo(from, 1053061); // This plant has no seeds to gather!
                        }                //Seed of Renewal Edit
                        else
                        {
                            if (Utility.RandomDouble() < 0.05)
                            {
                                Item Rseed = new SeedOfRenewal();

                                if (from.PlaceInBackpack(Rseed))
                                {
                                    system.AvailableSeeds--;
                                    m_Plant.LabelTo(from, 1053063); // You gather seeds from the plant.
                                }
                                else
                                {
                                    Rseed.Delete();
                                    m_Plant.LabelTo(from, 1053062); // You attempt to gather as many seeds as you can hold, but your backpack is full.
                                }
                            }
                            else
                            {
                                Seed seed = new Seed(system.SeedType, system.SeedHue, true);

                                if (from.PlaceInBackpack(seed))
                                {
                                    system.AvailableSeeds--;
                                    m_Plant.LabelTo(from, 1053063); // You gather seeds from the plant.
                                }
                                else
                                {
                                    seed.Delete();
                                    m_Plant.LabelTo(from, 1053062); // You attempt to gather as many seeds as you can hold, but your backpack is full.
                                }
                            }
                        }

                        from.SendGump(new ReproductionGump(m_Plant));

                        break;
                    }
            }
        }

        private void DrawBackground()
        {
            AddBackground(50, 50, 200, 150, 0xE10);

            AddImage(60, 90, 0xE17);
            AddImage(120, 90, 0xE17);

            AddImage(60, 145, 0xE17);
            AddImage(120, 145, 0xE17);

            AddItem(45, 45, 0xCEF);
            AddItem(45, 118, 0xCF0);

            AddItem(211, 45, 0xCEB);
            AddItem(211, 118, 0xCEC);
        }

        private void AddPollinationState(int x, int y)
        {
            PlantSystem system = m_Plant.PlantSystem;

            if (!system.PollenProducing)
                AddLabel(x, y, 0x35, "-");
            else if (!system.Pollinated)
                AddLabel(x, y, 0x21, "!");
            else
                AddLabel(x, y, 0x3F, "+");
        }

        private void AddResourcesState(int x, int y)
        {
            PlantResourceInfo resInfo = PlantResourceInfo.GetInfo(m_Plant.PlantType, m_Plant.PlantHue);

            PlantSystem system = m_Plant.PlantSystem;
            int totalResources = system.AvailableResources + system.LeftResources;

            if (resInfo == null || totalResources == 0)
            {
                AddLabel(x + 5, y, 0x21, "X");
            }
            else
            {
                AddLabel(x, y, PlantHueInfo.GetInfo(m_Plant.PlantHue).GumpHue,
                    string.Format("{0}/{1}", system.AvailableResources, totalResources));
            }
        }

        private void AddSeedsState(int x, int y)
        {
            PlantSystem system = m_Plant.PlantSystem;
            int totalSeeds = system.AvailableSeeds + system.LeftSeeds;

            if (!m_Plant.IsCrossable || totalSeeds == 0)
            {
                AddLabel(x + 5, y, 0x21, "X");
            }
            else
            {
                AddLabel(x, y, PlantHueInfo.GetInfo(system.SeedHue).GumpHue,
                    string.Format("{0}/{1}", system.AvailableSeeds, totalSeeds));
            }
        }
    }
}