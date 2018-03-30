using System;
using Server;
using System.Collections.Generic;
using Server.Network;
using Server.ContextMenus;
using Server.Gumps;
using Server.Engines.SphynxFortune;

namespace Server.Mobiles
{
    public class Sphynx : BaseCreature
    {
        [Constructable]
        public Sphynx()
            : base(AIType.AI_Mage, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            Body = 788;
            Name = "The Sphynx";

            SetStr(1001, 1200);
            SetDex(176, 195);
            SetInt(301, 400);

            SetHits(1001, 1200);
            SetStam(176, 195);
            SetMana(301, 400);

            SetDamage(10, 15);

            SetDamageType(ResistanceType.Physical, 85);
            SetDamageType(ResistanceType.Energy, 15);

            SetResistance(ResistanceType.Physical, 60, 80);
            SetResistance(ResistanceType.Fire, 30, 50);
            SetResistance(ResistanceType.Cold, 40, 60);
            SetResistance(ResistanceType.Poison, 40, 50);
            SetResistance(ResistanceType.Energy, 40, 50);

            SetSkill(SkillName.Wrestling, 90.1, 100.0);
            SetSkill(SkillName.Tactics, 90.1, 100.0);
            SetSkill(SkillName.MagicResist, 100.5, 150.0);
            SetSkill(SkillName.Magery, 95.5, 100.0);
            SetSkill(SkillName.Anatomy, 25.1, 50.0);
            SetSkill(SkillName.EvalInt, 90.1, 100.0);
            SetSkill(SkillName.Meditation, 95.1, 120.0);
			SetSkill(SkillName.DetectHidden, 100.0);

            Fame = 15000;
            Karma = 0;

            PackGold(1000, 1200);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich, 2);
        }

        public override void AddCustomContextEntries(Mobile from, List<ContextMenuEntry> list)
        {
            if (from.Alive)
                list.Add(new AskAboutFutureEntry(from, this));

            base.AddCustomContextEntries(from, list);
        }

        public Sphynx(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        public class AskAboutFutureEntry : ContextMenuEntry
        {
            private Sphynx m_Sphynx;
            private Mobile m;

            public AskAboutFutureEntry(Mobile from, Sphynx Sphynx)
                : base(6199, 8)
            {
                m_Sphynx = Sphynx;
                m = from;

                Enabled = !SphynxFortune.UnderEffect(from);
            }

            public override void OnClick()
            {
                m.CloseGump(typeof(SphynxGump));
                m.SendGump(new SphynxGump(m_Sphynx));
            }
        }

        public class SphynxGump : Gump
        {
            private Sphynx Sphynx;

            public SphynxGump(Sphynx s)
                : base(150, 50)
            {
                Sphynx = s;

                AddPage(0);

                Closable = false;

                AddImage(0, 0, 0xE10);
                AddImageTiled(0, 14, 15, 200, 0xE13);
                AddImageTiled(380, 14, 14, 200, 0xE15);

                AddImage(0, 201, 0xE16);
                AddImageTiled(15, 201, 370, 16, 0xE17);
                AddImageTiled(15, 0, 370, 16, 0xE11);

                AddImage(380, 0, 0xE12);
                AddImage(380, 201, 0xE18);
                AddImageTiled(15, 15, 365, 190, 0xA40);

                AddRadio(30, 140, 0x25FF, 0x2602, false, 1);
                AddHtmlLocalized(65, 145, 300, 25, 1060863, 0xFFFFFF, false, false); // Pay for the reading.

                AddRadio(30, 175, 0x25FF, 0x2602, true, 0);
                AddHtmlLocalized(65, 178, 300, 25, 1060862, 0xFFFFFF, false, false); // No thanks. I decide my own destiny!

                AddHtmlLocalized(30, 20, 360, 35, 1060864, 0xFFFFFF, false, false); // Interested in your fortune, are you?  The ancient Sphynx can read the future for you - for a price of course...

                AddImage(65, 72, 0x15E5);
                AddImageTiled(80, 90, 200, 1, 0x2393);
                AddImageTiled(95, 92, 200, 1, 0x23C5);

                AddLabel(90, 70, 0x66D, "5000");

                AddHtmlLocalized(140, 70, 100, 25, 1023823, 0xFFFFFF, false, false); // gold coins

                AddButton(290, 175, 0xF7, 0xF8, 2, GumpButtonType.Reply, 0);

                AddImageTiled(15, 14, 365, 1, 0x2393);
                AddImageTiled(380, 14, 1, 190, 0x2391);
                AddImageTiled(15, 205, 365, 1, 0x2393);
                AddImageTiled(15, 14, 1, 190, 0x2391);
                AddImageTiled(0, 0, 395, 1, 0x23C5);
                AddImageTiled(394, 0, 1, 217, 0x23C3);
                AddImageTiled(0, 216, 395, 1, 0x23C5);
                AddImageTiled(0, 0, 1, 217, 0x23C3);

                AddHtmlLocalized(30, 105, 340, 40, 1060865, 0x1DB2D, false, false); // Do you accept this offer?  The funds will be withdrawn from your backpack.
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                Mobile from = sender.Mobile;

                if (info.ButtonID == 2)
                {
                    if (info.IsSwitched(1))
                    {
                        if (Banker.Deposit(from, 5000, true))
                        {
                            SphynxFortune.ApplyFortune(from, Sphynx);
                            SphynxFortune.ApplyFortune(from, Sphynx);

                            from.UpdateResistances();
                        }
                        else
                        {
                            from.SendLocalizedMessage(1061006); // You haven't got the coin to make the proper donation to the Sphynx.  Your fortune has not been read.
                        }
                    }
                    else
                    {
                        from.SendLocalizedMessage(1061007); // You decide against having your fortune told.
                    }
                }
            }
        }
    }
}
