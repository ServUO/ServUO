using System;
using System.Xml;
using Server.Mobiles;
using Server.Regions;

namespace Server.Engines.Quests
{
    public class QuestOfferRegion : BaseRegion
    {
        private readonly Type m_Quest;
        public QuestOfferRegion(XmlElement xml, Map map, Region parent)
            : base(xml, map, parent)
        {
            ReadType(xml["quest"], "type", ref this.m_Quest);
        }

        public Type Quest
        {
            get
            {
                return this.m_Quest ;
            }
        }
        public override void OnEnter(Mobile m)
        {
            base.OnEnter(m);

            if (this.m_Quest == null)
                return;

            PlayerMobile player = m as PlayerMobile;

            if (player != null && player.Quest == null && QuestSystem.CanOfferQuest(m, this.m_Quest))
            {
                try
                {
                    QuestSystem qs = (QuestSystem)Activator.CreateInstance(this.m_Quest, new object[] { player });
                    qs.SendOffer();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error creating quest {0}: {1}", this.m_Quest, ex);
                }
            }
        }
    }
}