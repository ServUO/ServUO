using Server.ContextMenus;
using Server.Engines.VoidPool;
using Server.Mobiles;

namespace Server.Engines.Points
{
    public class VoidPool : PointsSystem
    {
        public override PointsType Loyalty => PointsType.VoidPool;
        public override TextDefinition Name => m_Name;
        public override bool AutoAdd => true;
        public override double MaxPoints => double.MaxValue;

        private readonly TextDefinition m_Name = new TextDefinition(1152733);

        public override void SendMessage(PlayerMobile from, double old, double points, bool quest)
        {
            from.SendLocalizedMessage(1152649, string.Format("{0}\t{1}", from.Map.ToString(), points));
            // For your participation in the Battle for the Void Pool on ~1_FACET~, you have received ~2_POINTS~ reward points. Any reward points you have accumulated may be redeemed by visiting Vela in Cove.
        }

        public override TextDefinition GetTitle(PlayerMobile from)
        {
            return new TextDefinition(1152531); // The Void Pool
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            if (Version >= 2)
            {
                int version = reader.ReadInt();

                // all deserialize code in here
            }
        }
    }

    public class VoidPoolInfo : ContextMenuEntry
    {
        private readonly Mobile m_From;
        private readonly VoidPoolController m_Controller;

        public VoidPoolInfo(Mobile from, VoidPoolController controller)
            : base(1152531, -1) // The Void Pool
        {
            m_From = from;
            m_Controller = controller;
        }

        public override void OnClick()
        {
            if (m_From is PlayerMobile && m_Controller != null)
            {
                m_From.SendGump(new VoidPoolGump(m_Controller, m_From as PlayerMobile));
            }
        }
    }
}
