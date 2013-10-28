using System;
using Server.Gumps;
using Server.Network;

namespace Server.Engines.Plants
{
    public class SetToDecorativeGump : Gump
    {
        private readonly PlantItem m_Plant;
        public SetToDecorativeGump(PlantItem plant)
            : base(20, 20)
        {
            this.m_Plant = plant;

            this.DrawBackground();

            this.AddLabel(115, 85, 0x44, "Set plant");
            this.AddLabel(82, 105, 0x44, "to decorative mode?");

            this.AddButton(98, 140, 0x47E, 0x480, 1, GumpButtonType.Reply, 0); // Cancel

            this.AddButton(138, 141, 0xD2, 0xD2, 2, GumpButtonType.Reply, 0); // Help
            this.AddLabel(143, 141, 0x835, "?");

            this.AddButton(168, 140, 0x481, 0x483, 3, GumpButtonType.Reply, 0); // Ok
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            if (info.ButtonID == 0 || this.m_Plant.Deleted || this.m_Plant.PlantStatus != PlantStatus.Stage9)
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
                        from.SendGump(new ReproductionGump(this.m_Plant));

                        break;
                    }
                case 2: // Help
                    {
                        from.Send(new DisplayHelpTopic(70, true)); // DECORATIVE MODE

                        from.SendGump(new SetToDecorativeGump(this.m_Plant));

                        break;
                    }
                case 3: // Ok
                    {
                        this.m_Plant.PlantStatus = PlantStatus.DecorativePlant;
                        this.m_Plant.LabelTo(from, 1053077); // You prune the plant. This plant will no longer produce resources or seeds, but will require no upkeep.

                        break;
                    }
            }
        }

        private void DrawBackground()
        {
            this.AddBackground(50, 50, 200, 150, 0xE10);

            this.AddItem(25, 45, 0xCEB);
            this.AddItem(25, 118, 0xCEC);

            this.AddItem(227, 45, 0xCEF);
            this.AddItem(227, 118, 0xCF0);
        }
    }
}