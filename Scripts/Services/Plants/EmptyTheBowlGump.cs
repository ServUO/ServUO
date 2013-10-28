using System;
using Server.Gumps;
using Server.Network;

namespace Server.Engines.Plants
{
    public class EmptyTheBowlGump : Gump
    {
        private readonly PlantItem m_Plant;
        public EmptyTheBowlGump(PlantItem plant)
            : base(20, 20)
        {
            this.m_Plant = plant;

            this.DrawBackground();

            this.AddLabel(90, 70, 0x44, "Empty the bowl?");

            this.DrawPicture();

            this.AddButton(98, 150, 0x47E, 0x480, 1, GumpButtonType.Reply, 0); // Cancel

            this.AddButton(138, 151, 0xD2, 0xD2, 2, GumpButtonType.Reply, 0); // Help
            this.AddLabel(143, 151, 0x835, "?");

            this.AddButton(168, 150, 0x481, 0x483, 3, GumpButtonType.Reply, 0); // Ok
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            if (info.ButtonID == 0 || this.m_Plant.Deleted || this.m_Plant.PlantStatus >= PlantStatus.DecorativePlant)
                return;
			
            if (info.ButtonID == 3 && !from.InRange(this.m_Plant.GetWorldLocation(), 3))
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
                case 1: // Cancel
                    {
                        from.SendGump(new MainPlantGump(this.m_Plant));

                        break;
                    }
                case 2: // Help
                    {
                        from.Send(new DisplayHelpTopic(71, true)); // EMPTYING THE BOWL

                        from.SendGump(new EmptyTheBowlGump(this.m_Plant));

                        break;
                    }
                case 3: // Ok
                    {
                        PlantBowl bowl = new PlantBowl();

                        if (!from.PlaceInBackpack(bowl))
                        {
                            bowl.Delete();

                            this.m_Plant.LabelTo(from, 1053047); // You cannot empty a bowl with a full pack!
                            from.SendGump(new MainPlantGump(this.m_Plant));

                            break;
                        }

                        if (this.m_Plant.PlantStatus != PlantStatus.BowlOfDirt && this.m_Plant.PlantStatus < PlantStatus.Plant)
                        {
                            Seed seed = new Seed(this.m_Plant.PlantType, this.m_Plant.PlantHue, this.m_Plant.ShowType);

                            if (!from.PlaceInBackpack(seed))
                            {
                                bowl.Delete();
                                seed.Delete();

                                this.m_Plant.LabelTo(from, 1053047); // You cannot empty a bowl with a full pack!
                                from.SendGump(new MainPlantGump(this.m_Plant));

                                break;
                            }
                        }

                        this.m_Plant.Delete();

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

        private void DrawPicture()
        {
            this.AddItem(90, 100, 0x1602);
            this.AddImage(140, 102, 0x15E1);
            this.AddItem(160, 100, 0x15FD);

            if (this.m_Plant.PlantStatus != PlantStatus.BowlOfDirt && this.m_Plant.PlantStatus < PlantStatus.Plant)
                this.AddItem(156, 130, 0xDCF); // Seed
        }
    }
}