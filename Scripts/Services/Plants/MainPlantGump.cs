using System;
using Server.Gumps;
using Server.Items;
using Server.Network;

namespace Server.Engines.Plants
{
    public class MainPlantGump : Gump
    {
        private readonly PlantItem m_Plant;
        public MainPlantGump(PlantItem plant)
            : base(20, 20)
        {
            this.m_Plant = plant;

            this.DrawBackground();

            this.DrawPlant();

            this.AddButton(71, 67, 0xD4, 0xD4, 1, GumpButtonType.Reply, 0); // Reproduction menu
            this.AddItem(59, 68, 0xD08);

            PlantSystem system = plant.PlantSystem;

            this.AddButton(71, 91, 0xD4, 0xD4, 2, GumpButtonType.Reply, 0); // Infestation
            this.AddItem(8, 96, 0x372);
            this.AddPlus(95, 92, system.Infestation);

            this.AddButton(71, 115, 0xD4, 0xD4, 3, GumpButtonType.Reply, 0); // Fungus
            this.AddItem(58, 115, 0xD16);
            this.AddPlus(95, 116, system.Fungus);

            this.AddButton(71, 139, 0xD4, 0xD4, 4, GumpButtonType.Reply, 0); // Poison
            this.AddItem(59, 143, 0x1AE4);
            this.AddPlus(95, 140, system.Poison);

            this.AddButton(71, 163, 0xD4, 0xD4, 5, GumpButtonType.Reply, 0); // Disease
            this.AddItem(55, 167, 0x1727);
            this.AddPlus(95, 164, system.Disease);

            this.AddButton(209, 67, 0xD2, 0xD2, 6, GumpButtonType.Reply, 0); // Water
            this.AddItem(193, 67, 0x1F9D);
            this.AddPlusMinus(196, 67, system.Water);

            this.AddButton(209, 91, 0xD4, 0xD4, 7, GumpButtonType.Reply, 0); // Poison potion
            this.AddItem(201, 91, 0xF0A);
            this.AddLevel(196, 91, system.PoisonPotion);

            this.AddButton(209, 115, 0xD4, 0xD4, 8, GumpButtonType.Reply, 0); // Cure potion
            this.AddItem(201, 115, 0xF07);
            this.AddLevel(196, 115, system.CurePotion);

            this.AddButton(209, 139, 0xD4, 0xD4, 9, GumpButtonType.Reply, 0); // Heal potion
            this.AddItem(201, 139, 0xF0C);
            this.AddLevel(196, 139, system.HealPotion);

            this.AddButton(209, 163, 0xD4, 0xD4, 10, GumpButtonType.Reply, 0); // Strength potion
            this.AddItem(201, 163, 0xF09);
            this.AddLevel(196, 163, system.StrengthPotion);

            this.AddImage(48, 47, 0xD2);
            this.AddLevel(54, 47, (int)this.m_Plant.PlantStatus);

            this.AddImage(232, 47, 0xD2);
            this.AddGrowthIndicator(239, 47);

            this.AddButton(48, 183, 0xD2, 0xD2, 11, GumpButtonType.Reply, 0); // Help
            this.AddLabel(54, 183, 0x835, "?");

            this.AddButton(232, 183, 0xD4, 0xD4, 12, GumpButtonType.Reply, 0); // Empty the bowl
            this.AddItem(219, 180, 0x15FD);
        }

        public static Item GetPotion(Mobile from, PotionEffect[] effects)
        {
            if (from.Backpack == null)
                return null;

            Item[] items = from.Backpack.FindItemsByType(new Type[] { typeof(BasePotion), typeof(PotionKeg) });

            foreach (Item item in items)
            {
                if (item is BasePotion)
                {
                    BasePotion potion = (BasePotion)item;

                    if (Array.IndexOf(effects, potion.PotionEffect) >= 0)
                        return potion;
                }
                else
                {
                    PotionKeg keg = (PotionKeg)item;

                    if (keg.Held > 0 && Array.IndexOf(effects, keg.Type) >= 0)
                        return keg;
                }
            }

            return null;
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            if (info.ButtonID == 0 || this.m_Plant.Deleted || this.m_Plant.PlantStatus >= PlantStatus.DecorativePlant)
                return;
			
            if (((info.ButtonID >= 6 && info.ButtonID <= 10) || info.ButtonID == 12) && !from.InRange(this.m_Plant.GetWorldLocation(), 3))
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
                case 1: // Reproduction menu
                    {
                        if (this.m_Plant.PlantStatus > PlantStatus.BowlOfDirt)
                        {
                            from.SendGump(new ReproductionGump(this.m_Plant));
                        }
                        else
                        {
                            from.SendLocalizedMessage(1061885); // You need to plant a seed in the bowl first.

                            from.SendGump(new MainPlantGump(this.m_Plant));
                        }

                        break;
                    }
                case 2: // Infestation
                    {
                        from.Send(new DisplayHelpTopic(54, true)); // INFESTATION LEVEL

                        from.SendGump(new MainPlantGump(this.m_Plant));

                        break;
                    }
                case 3: // Fungus
                    {
                        from.Send(new DisplayHelpTopic(56, true)); // FUNGUS LEVEL

                        from.SendGump(new MainPlantGump(this.m_Plant));

                        break;
                    }
                case 4: // Poison
                    {
                        from.Send(new DisplayHelpTopic(58, true)); // POISON LEVEL

                        from.SendGump(new MainPlantGump(this.m_Plant));

                        break;
                    }
                case 5: // Disease
                    {
                        from.Send(new DisplayHelpTopic(60, true)); // DISEASE LEVEL

                        from.SendGump(new MainPlantGump(this.m_Plant));

                        break;
                    }
                case 6: // Water
                    {
                        Item[] item = from.Backpack.FindItemsByType(typeof(BaseBeverage));
					
                        bool foundUsableWater = false;
					
                        if (item != null && item.Length > 0)
                        {
                            for (int i = 0; i < item.Length; ++i)
                            {
                                BaseBeverage beverage = (BaseBeverage)item[i];
							
                                if (!beverage.IsEmpty && beverage.Pourable && beverage.Content == BeverageType.Water)
                                {
                                    foundUsableWater = true;
                                    this.m_Plant.Pour(from, beverage);
                                    break;
                                }
                            }
                        }
					
                        if (!foundUsableWater)
                        {
                            from.Target = new PlantPourTarget(this.m_Plant);
                            from.SendLocalizedMessage(1060808, "#" + this.m_Plant.GetLocalizedPlantStatus().ToString()); // Target the container you wish to use to water the ~1_val~.
                        }

                        from.SendGump(new MainPlantGump(this.m_Plant));

                        break;
                    }
                case 7: // Poison potion
                    {
                        this.AddPotion(from, PotionEffect.PoisonGreater, PotionEffect.PoisonDeadly);

                        break;
                    }
                case 8: // Cure potion
                    {
                        this.AddPotion(from, PotionEffect.CureGreater);

                        break;
                    }
                case 9: // Heal potion
                    {
                        this.AddPotion(from, PotionEffect.HealGreater);

                        break;
                    }
                case 10: // Strength potion
                    {
                        this.AddPotion(from, PotionEffect.StrengthGreater);

                        break;
                    }
                case 11: // Help
                    {
                        from.Send(new DisplayHelpTopic(48, true)); // PLANT GROWING

                        from.SendGump(new MainPlantGump(this.m_Plant));

                        break;
                    }
                case 12: // Empty the bowl
                    {
                        from.SendGump(new EmptyTheBowlGump(this.m_Plant));

                        break;
                    }
            }
        }

        private void DrawBackground()
        {
            this.AddBackground(50, 50, 200, 150, 0xE10);

            this.AddItem(45, 45, 0xCEF);
            this.AddItem(45, 118, 0xCF0);

            this.AddItem(211, 45, 0xCEB);
            this.AddItem(211, 118, 0xCEC);
        }

        private void DrawPlant()
        {
            PlantStatus status = this.m_Plant.PlantStatus;

            if (status < PlantStatus.FullGrownPlant)
            {
                this.AddImage(110, 85, 0x589);

                this.AddItem(122, 94, 0x914);
                this.AddItem(135, 94, 0x914);
                this.AddItem(120, 112, 0x914);
                this.AddItem(135, 112, 0x914);

                if (status >= PlantStatus.Stage2)
                {
                    this.AddItem(127, 112, 0xC62);
                }
                if (status == PlantStatus.Stage3 || status == PlantStatus.Stage4)
                {
                    this.AddItem(129, 85, 0xC7E);
                }
                if (status >= PlantStatus.Stage4)
                {
                    this.AddItem(121, 117, 0xC62);
                    this.AddItem(133, 117, 0xC62);
                }
                if (status >= PlantStatus.Stage5)
                {
                    this.AddItem(110, 100, 0xC62);
                    this.AddItem(140, 100, 0xC62);
                    this.AddItem(110, 130, 0xC62);
                    this.AddItem(140, 130, 0xC62);
                }
                if (status >= PlantStatus.Stage6)
                {
                    this.AddItem(105, 115, 0xC62);
                    this.AddItem(145, 115, 0xC62);
                    this.AddItem(125, 90, 0xC62);
                    this.AddItem(125, 135, 0xC62);
                }
            }
            else
            {
                PlantTypeInfo typeInfo = PlantTypeInfo.GetInfo(this.m_Plant.PlantType);
                PlantHueInfo hueInfo = PlantHueInfo.GetInfo(this.m_Plant.PlantHue);

                // The large images for these trees trigger a client crash, so use a smaller, generic tree.
                if (this.m_Plant.PlantType == PlantType.CypressTwisted || this.m_Plant.PlantType == PlantType.CypressStraight)
                    this.AddItem(130 + typeInfo.OffsetX, 96 + typeInfo.OffsetY, 0x0CCA, hueInfo.Hue);
                else
                    this.AddItem(130 + typeInfo.OffsetX, 96 + typeInfo.OffsetY, typeInfo.ItemID, hueInfo.Hue);
            }

            if (status != PlantStatus.BowlOfDirt)
            {
                int message = this.m_Plant.PlantSystem.GetLocalizedHealth();

                switch ( this.m_Plant.PlantSystem.Health )
                {
                    case PlantHealth.Dying:
                        {
                            this.AddItem(92, 167, 0x1B9D);
                            this.AddItem(161, 167, 0x1B9D);

                            this.AddHtmlLocalized(136, 167, 42, 20, message, 0x00FC00, false, false);

                            break;
                        }
                    case PlantHealth.Wilted:
                        {
                            this.AddItem(91, 164, 0x18E6);
                            this.AddItem(161, 164, 0x18E6);

                            this.AddHtmlLocalized(132, 167, 42, 20, message, 0x00C207, false, false);

                            break;
                        }
                    case PlantHealth.Healthy:
                        {
                            this.AddItem(96, 168, 0xC61);
                            this.AddItem(162, 168, 0xC61);

                            this.AddHtmlLocalized(129, 167, 42, 20, message, 0x008200, false, false);

                            break;
                        }
                    case PlantHealth.Vibrant:
                        {
                            this.AddItem(93, 162, 0x1A99);
                            this.AddItem(162, 162, 0x1A99);

                            this.AddHtmlLocalized(129, 167, 42, 20, message, 0x0083E0, false, false);

                            break;
                        }
                }
            }
        }

        private void AddPlus(int x, int y, int value)
        {
            switch ( value )
            {
                case 1:
                    this.AddLabel(x, y, 0x35, "+");
                    break;
                case 2:
                    this.AddLabel(x, y, 0x21, "+");
                    break;
            }
        }

        private void AddPlusMinus(int x, int y, int value)
        {
            switch ( value )
            {
                case 0:
                    this.AddLabel(x, y, 0x21, "-");
                    break;
                case 1:
                    this.AddLabel(x, y, 0x35, "-");
                    break;
                case 3:
                    this.AddLabel(x, y, 0x35, "+");
                    break;
                case 4:
                    this.AddLabel(x, y, 0x21, "+");
                    break;
            }
        }

        private void AddLevel(int x, int y, int value)
        {
            this.AddLabel(x, y, 0x835, value.ToString());
        }

        private void AddGrowthIndicator(int x, int y)
        {
            if (!this.m_Plant.IsGrowable)
                return;

            switch ( this.m_Plant.PlantSystem.GrowthIndicator )
            {
                case PlantGrowthIndicator.InvalidLocation :
                    this.AddLabel(x, y, 0x21, "!");
                    break;
                case PlantGrowthIndicator.NotHealthy :
                    this.AddLabel(x, y, 0x21, "-");
                    break;
                case PlantGrowthIndicator.Delay :
                    this.AddLabel(x, y, 0x35, "-");
                    break;
                case PlantGrowthIndicator.Grown :
                    this.AddLabel(x, y, 0x3, "+");
                    break;
                case PlantGrowthIndicator.DoubleGrown :
                    this.AddLabel(x, y, 0x3F, "+");
                    break;
            }
        }

        private void AddPotion(Mobile from, params PotionEffect[] effects)
        {
            Item item = GetPotion(from, effects);

            if (item != null)
            {
                this.m_Plant.Pour(from, item);
            }
            else
            {
                int message;
                if (this.m_Plant.ApplyPotion(effects[0], true, out message))
                {
                    from.SendLocalizedMessage(1061884); // You don't have any strong potions of that type in your pack.

                    from.Target = new PlantPourTarget(this.m_Plant);
                    from.SendLocalizedMessage(1060808, "#" + this.m_Plant.GetLocalizedPlantStatus().ToString()); // Target the container you wish to use to water the ~1_val~.

                    return;
                }
                else
                {
                    this.m_Plant.LabelTo(from, message);
                }
            }

            from.SendGump(new MainPlantGump(this.m_Plant));
        }
    }
}