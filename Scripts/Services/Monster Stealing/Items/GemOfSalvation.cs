using System;
using System.Collections.Generic;
using Server.Gumps; 
using Server.Mobiles;
using Server.Network;
using Server.Items;
using Server;

namespace Server.Items
{
    [TypeAlias("drNO.ThieveItems.GemOfSalvation")]
    public class GemOfSalvation : Item
    {
        public override int LabelNumber { get { return 1094939; } } // Gem of Salvation

        private static readonly TimeSpan Cooldown = TimeSpan.FromHours(6);

        [Constructable]
        public GemOfSalvation()
            : base(0x1F13)
        {
            Hue = 0xba;
            LootType = LootType.Blessed;
        }

        public static Dictionary<PlayerMobile, DateTime> SalvationUsage = new Dictionary<PlayerMobile, DateTime>();

        public static void Initialize()
        {
            EventSink.PlayerDeath += new PlayerDeathEventHandler(OnDeath);
        }

        public static void OnDeath(PlayerDeathEventArgs args)
        {
            PlayerMobile pm = (PlayerMobile)args.Mobile;

            if (pm != null)
            {
                HandleDeath(pm);
            }
        }

        public static void DoCleanup()
        {
            List<PlayerMobile> toRemove = new List<PlayerMobile>();

            foreach (PlayerMobile pm in SalvationUsage.Keys)
            {
                if (SalvationUsage[pm] != null)
                {
                    if (SalvationUsage[pm] < DateTime.Now + Cooldown)
                    {
                        toRemove.Add(pm);
                    }
                }
            }

            foreach (PlayerMobile pm in toRemove)
            {
                SalvationUsage.Remove(pm);
            }

            toRemove.Clear();
        }

        private static bool CheckUse(PlayerMobile pm)
        {
            if (SalvationUsage.ContainsKey(pm))
            {
                if (SalvationUsage[pm] + Cooldown >= DateTime.Now)
                    return false;
                else
                    return true;
            }
            else
            {
                return true;
            }
        }

        public static void HandleDeath(PlayerMobile pm)
        {
            if (!CheckUse(pm))
            {
                return;
            }

            if (pm.Backpack != null)
            {
                foreach (Item itm in pm.Backpack.Items)
                {
                    if (itm is GemOfSalvation)
                    {

                        Timer t1 = new GemOfSalvationGumpTimer(pm);
                        t1.Start();

                        Timer t2 = new GemOfSalvationGumpCloseTimer(pm);
                        t2.Start();

                        break;
                    }
                }
            }

        }
        public GemOfSalvation(Serial serial)
            : base(serial)
        {
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

    public class GemOfSalvationGump : Gump
    {
        public GemOfSalvationGump()
            : base(0, 0)
        {

            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;
            this.AddPage(0);
            this.AddBackground(55, 82, 343, 112, 9200);
            this.AddBackground(61, 89, 329, 100, 9350);
            this.AddLabel(178, 95, 0, @"Gem of Salvation ");
            this.AddButton(70, 120, 4005, 4006, (int)Buttons.btnYes, GumpButtonType.Reply, 0);
            this.AddLabel(110, 120, 371, @"Yes, i want to be ressurected");
            this.AddButton(70, 150, 4005, 4006, (int)Buttons.btnNO, GumpButtonType.Reply, 0);
            this.AddLabel(110, 150, 337, @"No, I'dont want to be ressurected");
        }

        public enum Buttons
        {
            btnYes = 1,
            btnNO = 2,
        }


        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile mob = sender.Mobile;
            if (info.ButtonID == (int)Buttons.btnYes)
            {
                if (mob != null && !mob.Alive)
                {
                    if (mob.Backpack != null && mob.Backpack.Items != null)
                    {

                        foreach (Item itm in mob.Backpack.Items)
                        {
                            if (itm is GemOfSalvation)
                            {
                                PlayerMobile pm = mob as PlayerMobile;

                                if (!GemOfSalvation.SalvationUsage.ContainsKey(pm))
                                {
                                    GemOfSalvation.SalvationUsage.Add(pm, DateTime.Now);
                                }
                                else
                                {
                                    GemOfSalvation.SalvationUsage[pm] = DateTime.Now;
                                }
                                mob.Resurrect();
                                itm.Consume();
                                break;

                            }
                        }

                    }
                }
            }
            else
            {
                mob.SendMessage("Canceled");
            }
        }

    }

    class GemOfSalvationGumpCloseTimer : Timer
    {
        Mobile m_Owner;

        public GemOfSalvationGumpCloseTimer(Mobile owner)
            : base(TimeSpan.FromSeconds(15))
        {
            m_Owner = owner;
        }

        protected override void OnTick()
        {
            m_Owner.CloseGump(typeof(GemOfSalvationGump));
        }
    }

    class GemOfSalvationGumpTimer : Timer
    {
        Mobile m_Owner;

        public GemOfSalvationGumpTimer(Mobile owner)
            : base(TimeSpan.FromSeconds(5))
        {
            m_Owner = owner;
        }

        protected override void OnTick()
        {
            m_Owner.CloseGump(typeof(GemOfSalvationGump));
            m_Owner.SendGump(new GemOfSalvationGump());

        }
    }
}

