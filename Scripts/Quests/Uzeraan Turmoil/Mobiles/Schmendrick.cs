using System;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests.Haven
{
    public class Schmendrick : BaseQuester
    {
        [Constructable]
        public Schmendrick()
            : base("the High Mage")
        {
        }

        public Schmendrick(Serial serial)
            : base(serial)
        {
        }

        public override void InitBody()
        {
            this.InitStats(100, 100, 25);

            this.Hue = 0x83F3;

            this.Female = false;
            this.Body = 0x190;
            this.Name = "Schmendrick";
        }

        public override void InitOutfit()
        {
            this.AddItem(new Robe(0x4DD));
            this.AddItem(new WizardsHat(0x482));
            this.AddItem(new Shoes(0x482));

            this.HairItemID = 0x203C;
            this.HairHue = 0x455;

            this.FacialHairItemID = 0x203E;
            this.FacialHairHue = 0x455;

            GlacialStaff staff = new GlacialStaff();
            staff.Movable = false;
            this.AddItem(staff);

            Backpack pack = new Backpack();
            pack.Movable = false;
            this.AddItem(pack);
        }

        public override int GetAutoTalkRange(PlayerMobile pm)
        {
            return 7;
        }

        public override bool CanTalkTo(PlayerMobile to)
        {
            UzeraanTurmoilQuest qs = to.Quest as UzeraanTurmoilQuest;

            return (qs != null && qs.FindObjective(typeof(FindSchmendrickObjective)) != null);
        }

        public override void OnTalk(PlayerMobile player, bool contextMenu)
        {
            QuestSystem qs = player.Quest;

            if (qs is UzeraanTurmoilQuest)
            {
                if (UzeraanTurmoilQuest.HasLostScrollOfPower(player))
                {
                    this.FocusTo(player);
                    qs.AddConversation(new LostScrollOfPowerConversation(false));
                }
                else
                {
                    QuestObjective obj = qs.FindObjective(typeof(FindSchmendrickObjective));

                    if (obj != null && !obj.Completed)
                    {
                        this.FocusTo(player);
                        obj.Complete();
                    }
                    else if (contextMenu)
                    {
                        this.FocusTo(player);
                        this.SayTo(player, 1049357); // I have nothing more for you at this time.
                    }
                }
            }
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (dropped is BlankScroll && UzeraanTurmoilQuest.HasLostScrollOfPower(from))
            {
                this.FocusTo(from);

                Item scroll = new SchmendrickScrollOfPower();

                if (!from.PlaceInBackpack(scroll))
                {
                    scroll.Delete();
                    from.SendLocalizedMessage(1046260); // You need to clear some space in your inventory to continue with the quest.  Come back here when you have more space in your inventory.
                    return false;
                }
                else
                {
                    dropped.Consume();
                    from.SendLocalizedMessage(1049346); // Schmendrick scribbles on the scroll for a few moments and hands you the finished product.
                    return dropped.Deleted;
                }
            }

            return base.OnDragDrop(from, dropped);
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            base.OnMovement(m, oldLocation);

            if (m is PlayerMobile && !m.Frozen && !m.Alive && this.InRange(m, 4) && !this.InRange(oldLocation, 4) && this.InLOS(m))
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