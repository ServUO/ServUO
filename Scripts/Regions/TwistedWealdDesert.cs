using System.Xml;
using Server.Network;
using Server.Spells;
using Server.Spells.Ninjitsu;

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
            EventSink.Login += new LoginEventHandler(Desert_OnLogin);
        }

        public override void OnEnter(Mobile m)
        {
            if (m.NetState != null &&
                !TransformationSpellHelper.UnderTransformation(m, typeof(AnimalForm)) &&
                m.AccessLevel < AccessLevel.GameMaster)
                m.Send(SpeedControl.WalkSpeed);
        }

        public override void OnExit(Mobile m)
        {
            if (m.NetState != null &&
                !TransformationSpellHelper.UnderTransformation(m, typeof(AnimalForm)) &&
                !TransformationSpellHelper.UnderTransformation(m, typeof(Server.Spells.Spellweaving.ReaperFormSpell)))
                m.Send(SpeedControl.Disable);
        }

        private static void Desert_OnLogin(LoginEventArgs e) 
        {
            Mobile m = e.Mobile;
            if (m.Region.IsPartOf(typeof(TwistedWealdDesert)) && m.AccessLevel < AccessLevel.GameMaster)
                m.Send(SpeedControl.WalkSpeed);
        }
    }
}