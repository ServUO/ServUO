using System;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests.Ninja
{
    public class Zoel : BaseQuester
    {
        [Constructable]
        public Zoel()
            : base("the Masterful Tactician")
        {
        }

        public Zoel(Serial serial)
            : base(serial)
        {
        }

        public override int TalkNumber
        {
            get
            {
                return -1;
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);

            this.Hue = 0x83FE;

            this.Female = false;
            this.Body = 0x190;
            this.Name = "Elite Ninja Zoel";
        }

        public override void InitOutfit()
        {
            this.HairItemID = 0x203B;
            this.HairHue = 0x901;

            this.AddItem(new HakamaShita(0x1));
            this.AddItem(new NinjaTabi());
            this.AddItem(new TattsukeHakama());
            this.AddItem(new Bandana());

            this.AddItem(new LeatherNinjaBelt());

            Tekagi tekagi = new Tekagi();
            tekagi.Movable = false;
            this.AddItem(tekagi);
        }

        public override int GetAutoTalkRange(PlayerMobile pm)
        {
            return 2;
        }

        public override bool CanTalkTo(PlayerMobile to)
        {
            return to.Quest is EminosUndertakingQuest;
        }

        public override void OnTalk(PlayerMobile player, bool contextMenu)
        {
            QuestSystem qs = player.Quest;

            if (qs is EminosUndertakingQuest)
            {
                QuestObjective obj = qs.FindObjective(typeof(FindZoelObjective));

                if (obj != null && !obj.Completed)
                    obj.Complete();
            }
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            PlayerMobile player = from as PlayerMobile;

            if (player != null)
            {
                QuestSystem qs = player.Quest;

                if (qs is EminosUndertakingQuest)
                {
                    if (dropped is NoteForZoel)
                    {
                        QuestObjective obj = qs.FindObjective(typeof(GiveZoelNoteObjective));

                        if (obj != null && !obj.Completed)
                        {
                            dropped.Delete();
                            obj.Complete();
                            return true;
                        }
                    }
                }
            }

            return base.OnDragDrop(from, dropped);
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            base.OnMovement(m, oldLocation);

            if (!m.Frozen && !m.Alive && this.InRange(m, 4) && !this.InRange(oldLocation, 4) && this.InLOS(m))
            {
                if (m.Map == null || !m.Map.CanFit(m.Location, 16, false, false))
                {
                    m.SendLocalizedMessage(502391); // Thou can not be resurrected there!
                }
                else
                {
                    this.Direction = this.GetDirectionTo(m);

                    m.PlaySound(0x214);
                    m.FixedEffect(0x376A, 10, 16);

                    m.CloseGump(typeof(ResurrectGump));
                    m.SendGump(new ResurrectGump(m, ResurrectMessage.Healer));
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}