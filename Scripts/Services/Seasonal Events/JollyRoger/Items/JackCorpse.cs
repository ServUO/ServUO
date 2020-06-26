using Server.Gumps;
using Server.Network;
using Server.SkillHandlers;

namespace Server.Items
{
    public class JackCorpse : Item, IForensicTarget
    {
        public static JackCorpse InstanceTram { get; set; }
        public static JackCorpse InstanceFel { get; set; }

        [Constructable]
        public JackCorpse()
            : base(0xA52B)
        {
            Movable = false;
        }

        public JackCorpse(Serial serial)
            : base(serial)
        {
        }

        public void OnForensicEval(Mobile m)
        {
            if (!m.Player)
                return;

            m.PrivateOverheadMessage(MessageType.Regular, 0x47E, 1157722, "Forensics", m.NetState); // *Your proficiency in ~1_SKILL~ reveals more about the item*
            m.SendSound(m.Female ? 0x30B : 0x41A);

            m.CloseGump(typeof(JackCorpseGump));
            m.SendGump(new JackCorpseGump());
        }

        public class JackCorpseGump : Gump
        {
            public JackCorpseGump()
                : base(100, 100)
            {
                AddPage(0);

                AddBackground(0, 0, 454, 400, 0x24A4);
                AddItem(30, 120, 0xA52B);
                AddHtmlLocalized(177, 50, 250, 18, 1114513, "#1126303", 0x3442, false, false); // <DIV ALIGN=CENTER>~1_TOKEN~</DIV>
                AddHtmlLocalized(177, 77, 250, 273, 1159356, 0xC63, true, true); // The corpse is frozen solid and appears to have suffered a blow to the back of the head. Bits of what appear to be a large pumpkin lay frozen in the snow. The corpse clutches an amulet that is firmly worn around the neck. The cause of death was most likely exposure following a blow to the head when the ship wrecked.
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();

            if (Map == Map.Trammel)
            {
                InstanceTram = this;
            }

            if (Map == Map.Felucca)
            {
                InstanceFel = this;
            }
        }
    }
}
