using Server.Gumps;
using Server.Network;
using System;

namespace Server.Engines.Plants
{
    public class SetToDecorativeGump : Gump
    {
        private readonly PlantItem m_Plant;

        public SetToDecorativeGump(PlantItem plant) : base(20, 20)
        {
            m_Plant = plant;

            DrawBackground();

            AddLabel(115, 85, 0x44, "Set plant");
            AddLabel(82, 105, 0x44, "to decorative mode?");

            AddButton(98, 140, 0x47E, 0x480, 1, GumpButtonType.Reply, 0); // Cancel

            AddButton(138, 141, 0xD2, 0xD2, 2, GumpButtonType.Reply, 0); // Help
            AddLabel(143, 141, 0x835, "?");

            AddButton(168, 140, 0x481, 0x483, 3, GumpButtonType.Reply, 0); // Ok
        }

        private void DrawBackground()
        {
            AddBackground(50, 50, 200, 150, 0xE10);

            AddItem(25, 45, 0xCEB);
            AddItem(25, 118, 0xCEC);

            AddItem(227, 45, 0xCEF);
            AddItem(227, 118, 0xCF0);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            if (info.ButtonID == 0 || m_Plant.Deleted || m_Plant.PlantStatus != PlantStatus.Stage9)
                return;

            if (info.ButtonID == 3 && !from.InRange(m_Plant.GetWorldLocation(), 3))
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
                case 1: // Cancel
                    {
                        from.SendGump(new ReproductionGump(m_Plant));

                        break;
                    }
                case 2: // Help
                    {
                        from.Send(new DisplayHelpTopic(70, true)); // DECORATIVE MODE

                        from.SendGump(new SetToDecorativeGump(m_Plant));

                        break;
                    }
                case 3: // Ok
                    {
                        m_Plant.PlantStatus = PlantStatus.DecorativePlant;
                        m_Plant.LabelTo(from, 1053077); // You prune the plant. This plant will no longer produce resources or seeds, but will require no upkeep.

                        if (!m_Plant.RequiresUpkeep || m_Plant.MaginciaPlant)
                        {
                            m_Plant.Movable = true;

                            if (m_Plant is MaginciaPlantItem)
                                ((MaginciaPlantItem)m_Plant).SetToDecorative = DateTime.Now;

                            if (from.Backpack != null)
                                from.Backpack.TryDropItem(from, m_Plant, false);

                            if (m_Plant is GardenBedPlantItem rp && rp.Component != null)
                            {
                                rp.Component.Plant = null;
                                rp.Component = null;
                            }
                        }

                        break;
                    }
            }
        }
    }
}
