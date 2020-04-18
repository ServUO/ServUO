using Server.Items;
using System;

namespace Server.Mobiles
{
    /// <summary>
    /// This is a test creature
    /// You can set its value in game
    /// It die after 5 minutes, so your test server stay clean
    /// Create a macro to help your creation "[add Dummy 1 15 7 -1 0.5 2"
    /// 
    /// A iTeam of negative will set a faction at random
    /// 
    /// Say Kill if you want them to die
    /// 
    /// </summary>
    public class Dummy : BaseCreature
    {
        public Timer m_Timer;
        [Constructable]
        public Dummy(AIType iAI, FightMode iFightMode, int iRangePerception, int iRangeFight, double dActiveSpeed, double dPassiveSpeed)
            : base(iAI, iFightMode, iRangePerception, iRangeFight, dActiveSpeed, dPassiveSpeed)
        {
            Body = 400 + Utility.Random(2);
            Hue = Utility.RandomSkinHue();

            Skills[SkillName.DetectHidden].Base = 100;
            Skills[SkillName.MagicResist].Base = 120;

            Team = Utility.Random(3);

            int iHue = 20 + Team * 40;
            int jHue = 25 + Team * 40;

            Utility.AssignRandomHair(this, iHue);

            LeatherGloves glv = new LeatherGloves();
            glv.Hue = iHue;
            glv.LootType = LootType.Newbied;
            AddItem(glv);

            Container pack = new Backpack();

            pack.Movable = false;

            AddItem(pack);

            m_Timer = new AutokillTimer(this);
            m_Timer.Start();
        }

        public Dummy(Serial serial)
            : base(serial)
        {
            m_Timer = new AutokillTimer(this);
            m_Timer.Start();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public override bool HandlesOnSpeech(Mobile from)
        {
            if (from.AccessLevel >= AccessLevel.GameMaster)
                return true;

            return base.HandlesOnSpeech(from);
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
            base.OnSpeech(e);

            if (e.Mobile.AccessLevel >= AccessLevel.GameMaster)
            {
                if (e.Speech == "kill")
                {
                    m_Timer.Stop();
                    m_Timer.Delay = TimeSpan.FromSeconds(Utility.Random(1, 5));
                    m_Timer.Start();
                }
            }
        }

        public override void OnTeamChange()
        {
            int iHue = 20 + Team * 40;
            int jHue = 25 + Team * 40;

            Item item = FindItemOnLayer(Layer.OuterTorso);

            if (item != null)
                item.Hue = jHue;

            item = FindItemOnLayer(Layer.Helm);

            if (item != null)
                item.Hue = iHue;

            item = FindItemOnLayer(Layer.Gloves);

            if (item != null)
                item.Hue = iHue;

            item = FindItemOnLayer(Layer.Shoes);

            if (item != null)
                item.Hue = iHue;

            HairHue = iHue;

            item = FindItemOnLayer(Layer.MiddleTorso);

            if (item != null)
                item.Hue = iHue;

            item = FindItemOnLayer(Layer.OuterLegs);

            if (item != null)
                item.Hue = iHue;
        }

        private class AutokillTimer : Timer
        {
            private readonly Dummy m_Owner;
            public AutokillTimer(Dummy owner)
                : base(TimeSpan.FromMinutes(5.0))
            {
                m_Owner = owner;
                Priority = TimerPriority.FiveSeconds;
            }

            protected override void OnTick()
            {
                m_Owner.Kill();
                Stop();
            }
        }
    }
}