using Server.Items;
using System;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class GBBigglesby : BaseVendor
    {
        public override bool IsActiveVendor => false;
        protected override List<SBInfo> SBInfos => new List<SBInfo>();

        public override void InitSBInfo()
        {
        }

        [Constructable]
        public GBBigglesby()
            : base("the proprietor")
        {
            Name = "G.B. Bigglesby";
        }

        public override void InitBody()
        {
            InitStats(100, 100, 25);
            Female = false;
            Race = Race.Human;

            Hue = Race.RandomSkinHue();
            Race.RandomHair(this);
            HairHue = Race.RandomHairHue();

            Item fancyShirt = new FancyShirt();
            Item shirt = new Shirt(PirateCaptain.GetRandomShirtHue())
            {
                Layer = Layer.OuterTorso
            };

            AddItem(new Cloak(5));
            AddItem(new Cutlass());
            AddItem(shirt);
            AddItem(fancyShirt);
            AddItem(new LongPants());
            AddItem(new Boots());

            m_NextSay = DateTime.UtcNow;
        }

        private int m_LastSay;
        private DateTime m_NextSay;

        public override void OnDoubleClick(Mobile m)
        {
            if (m_NextSay < DateTime.UtcNow)
            {
                if (m_LastSay == 0)
                {
                    Say(1152651);  //I'm G.B. Bigglesby, proprietor of the G.B. Bigglesby Free Trade Floating Emporium.

                    m_LastSay = 1;
                }
                else
                {
                    Say(1152652);  //This sea market be me life's work and 'tis me pride and joy..
                    m_LastSay = 0;
                }

                m_NextSay = DateTime.UtcNow + TimeSpan.FromSeconds(5);
            }
        }

        public GBBigglesby(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_NextSay = DateTime.UtcNow;
        }
    }
}