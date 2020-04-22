using Server.Network;
using Server.Spells;
using Server.Spells.Ninjitsu;
using System.Xml;

namespace Server.Regions
{
    public class TwistedWealdDesert : MondainRegion
    {
        public TwistedWealdDesert(XmlElement xml, Map map, Region parent)
            : base(xml, map, parent)
        {
        }

        public static void Initialize()
        {
            EventSink.Login += Desert_OnLogin;
        }

        public override void OnEnter(Mobile m)
        {
            if (m.NetState != null && !TransformationSpellHelper.UnderTransformation(m, typeof(AnimalForm)) && m.AccessLevel < AccessLevel.GameMaster)
            {
                m.SendSpeedControl(SpeedControlType.WalkSpeed);
            }
        }

        public override void OnExit(Mobile m)
        {
            if (m.NetState != null && !TransformationSpellHelper.UnderTransformation(m, typeof(AnimalForm)))
            {
                m.SendSpeedControl(SpeedControlType.Disable);
            }
        }

        private static void Desert_OnLogin(LoginEventArgs e)
        {
            Mobile m = e.Mobile;

            if (m.Region.IsPartOf<TwistedWealdDesert>() && m.AccessLevel < AccessLevel.GameMaster)
                m.SendSpeedControl(SpeedControlType.WalkSpeed);
        }
    }
}
