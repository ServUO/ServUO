using System;
using Server.Gumps;
using Server.Network;

namespace Server.Engines.Plants
{
    public class ReproductionGump : Gump
    {
        private readonly PlantItem m_Plant;
        public ReproductionGump(PlantItem plant)
            : base(20, 20)
        {
            this.m_Plant = plant;

            this.DrawBackground();

            this.AddButton(70, 67, 0xD4, 0xD4, 1, GumpButtonType.Reply, 0); // Main menu
            this.AddItem(57, 65, 0x1600);

            this.AddLabel(108, 67, 0x835, "Reproduction");

            if (this.m_Plant.PlantStatus == PlantStatus.Stage9)
            {
                this.AddButton(212, 67, 0xD4, 0xD4, 2, GumpButtonType.Reply, 0); // Set to decorative
                this.AddItem(202, 68, 0xC61);
                this.AddLabel(216, 66, 0x21, "/");
            }

            this.AddButton(80, 116, 0xD4, 0xD4, 3, GumpButtonType.Reply, 0); // Pollination
            this.AddItem(66, 117, 0x1AA2);
            this.AddPollinationState(106, 116);

            this.AddButton(128, 116, 0xD4, 0xD4, 4, GumpButtonType.Reply, 0); // Resources
            this.AddItem(113, 120, 0x1021);
            this.AddResourcesState(149, 116);

            this.AddButton(177, 116, 0xD4, 0xD4, 5, GumpButtonType.Reply, 0); // Seeds
            this.AddItem(160, 121, 0xDCF);
            this.AddSeedsState(199, 116);

            this.AddButton(70, 163, 0xD2, 0xD2, 6, GumpButtonType.Reply, 0); // Gather pollen
            this.AddItem(56, 164, 0x1AA2);

            this.AddButton(138, 163, 0xD2, 0xD2, 7, GumpButtonType.Reply, 0); // Gather resources
            this.AddItem(123, 167, 0x1021);

            this.AddButton(212, 163, 0xD2, 0xD2, 8, GumpButtonType.Reply, 0); // Gather seeds
            this.AddItem(195, 168, 0xDCF);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            if (info.ButtonID == 0 || this.m_Plant.Deleted || this.m_Plant.PlantStatus >= PlantStatus.DecorativePlant || this.m_Plant.PlantStatus == PlantStatus.BowlOfDirt)
                return;

            if ((info.ButtonID >= 6 && info.ButtonID <= 8) && !from.InRange(this.m_Plant.GetWorldLocation(), 3))
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3E9, 500446); // That is too far away.
                return;
            }
			
            if (!this.m_Plant.IsUsableBy(from))
            {
                this.m_Plant.LabelTo(from, 1061856); // You must have the item in your backpack or locked down in order to use it.
                return;
            }

            switch ( info.ButtonID )
            {
                case 1: // Main menu
                    {
                        from.SendGump(new MainPlantGump(this.m_Plant));

                        break;
                    }
                case 2: // Set to decorative
                    {
                        if (this.m_Plant.PlantStatus == PlantStatus.Stage9)
                        {
                            from.SendGump(new SetToDecorativeGump(this.m_Plant));
                        }

                        break;
                    }
                case 3: // Pollination
                    {
                        from.Send(new DisplayHelpTopic(67, true)); // POLLINATION STATE

                        from.SendGump(new ReproductionGump(this.m_Plant));

                        break;
                    }
                case 4: // Resources
                    {
                        from.Send(new DisplayHelpTopic(69, true)); // RESOURCE PRODUCTION

                        from.SendGump(new ReproductionGump(this.m_Plant));

                        break;
                    }
                case 5: // Seeds
                    {
                        from.Send(new DisplayHelpTopic(68, true)); // SEED PRODUCTION

                        from.SendGump(new ReproductionGump(this.m_Plant));

                        break;
                    }
                case 6: // Gather pollen
                    {
                        if (!this.m_Plant.IsCrossable)
                        {
                            this.m_Plant.LabelTo(from, 1053050); // You cannot gather pollen from a mutated plant!
                        }
                        else if (!this.m_Plant.PlantSystem.PollenProducing)
                        {
                            this.m_Plant.LabelTo(from, 1053051); // You cannot gather pollen from a plant in this stage of development!
                        }
                        else if (this.m_Plant.PlantSystem.Health < PlantHealth.Healthy)
                        {
                            this.m_Plant.LabelTo(from, 1053052); // You cannot gather pollen from an unhealthy plant!
                        }
                        else
                        {
                            from.Target = new PollinateTarget(this.m_Plant);
                            from.SendLocalizedMessage(1053054); // Target the plant you wish to cross-pollinate to.

                            break;
                        }

                        from.SendGump(new ReproductionGump(this.m_Plant));

                        break;
                    }
                case 7: // Gather resources
                    {
                        PlantResourceInfo resInfo = PlantResourceInfo.GetInfo(this.m_Plant.PlantType, this.m_Plant.PlantHue);
                        PlantSystem system = this.m_Plant.PlantSystem;

                        if (resInfo == null)
                        {
                            if (this.m_Plant.IsCrossable)
                                this.m_Plant.LabelTo(from, 1053056); // This plant has no resources to gather!
                            else
                                this.m_Plant.LabelTo(from, 1053055); // Mutated plants do not produce resources!
                        }
                        else if (system.AvailableResources == 0)
                        {
                            this.m_Plant.LabelTo(from, 1053056); // This plant has no resources to gather!
                        }
                        else
                        {
                            Item resource = resInfo.CreateResource();

                            if (from.PlaceInBackpack(resource))
                            {
                                system.AvailableResources--;
                                this.m_Plant.LabelTo(from, 1053059); // You gather resources from the plant.
                            }
                            else
                            {
                                resource.Delete();
                                this.m_Plant.LabelTo(from, 1053058); // You attempt to gather as many resources as you can hold, but your backpack is full.
                            }
                        }

                        from.SendGump(new ReproductionGump(this.m_Plant));

                        break;
                    }
                case 8: // Gather seeds
                    {
                        PlantSystem system = this.m_Plant.PlantSystem;

                        if (!this.m_Plant.IsCrossable)
                        {
                            this.m_Plant.LabelTo(from, 1053060); // Mutated plants do not produce seeds!
                        }
                        else if (system.AvailableSeeds == 0)
                        {
                            this.m_Plant.LabelTo(from, 1053061); // This plant has no seeds to gather!
                        }
                        else
                        {
                            Seed seed = new Seed(system.SeedType, system.SeedHue, true);

                            if (from.PlaceInBackpack(seed))
                            {
                                system.AvailableSeeds--;
                                this.m_Plant.LabelTo(from, 1053063); // You gather seeds from the plant.
                            }
                            else
                            {
                                seed.Delete();
                                this.m_Plant.LabelTo(from, 1053062); // You attempt to gather as many seeds as you can hold, but your backpack is full.
                            }
                        }

                        from.SendGump(new ReproductionGump(this.m_Plant));

                        break;
                    }
            }
        }

        private void DrawBackground()
        {
            this.AddBackground(50, 50, 200, 150, 0xE10);

            this.AddImage(60, 90, 0xE17);
            this.AddImage(120, 90, 0xE17);

            this.AddImage(60, 145, 0xE17);
            this.AddImage(120, 145, 0xE17);

            this.AddItem(45, 45, 0xCEF);
            this.AddItem(45, 118, 0xCF0);

            this.AddItem(211, 45, 0xCEB);
            this.AddItem(211, 118, 0xCEC);
        }

        private void AddPollinationState(int x, int y)
        {
            PlantSystem system = this.m_Plant.PlantSystem;

            if (!system.PollenProducing)
                this.AddLabel(x, y, 0x35, "-");
            else if (!system.Pollinated)
                this.AddLabel(x, y, 0x21, "!");
            else
                this.AddLabel(x, y, 0x3F, "+");
        }

        private void AddResourcesState(int x, int y)
        {
            PlantResourceInfo resInfo = PlantResourceInfo.GetInfo(this.m_Plant.PlantType, this.m_Plant.PlantHue);

            PlantSystem system = this.m_Plant.PlantSystem;
            int totalResources = system.AvailableResources + system.LeftResources;

            if (resInfo == null || totalResources == 0)
            {
                this.AddLabel(x + 5, y, 0x21, "X");
            }
            else
            {
                this.AddLabel(x, y, PlantHueInfo.GetInfo(this.m_Plant.PlantHue).GumpHue,
                    string.Format("{0}/{1}", system.AvailableResources, totalResources));
            }
        }

        private void AddSeedsState(int x, int y)
        {
            PlantSystem system = this.m_Plant.PlantSystem;
            int totalSeeds = system.AvailableSeeds + system.LeftSeeds;

            if (!this.m_Plant.IsCrossable || totalSeeds == 0)
            {
                this.AddLabel(x + 5, y, 0x21, "X");
            }
            else
            {
                this.AddLabel(x, y, PlantHueInfo.GetInfo(system.SeedHue).GumpHue,
                    string.Format("{0}/{1}", system.AvailableSeeds, totalSeeds));
            }
        }
    }
}