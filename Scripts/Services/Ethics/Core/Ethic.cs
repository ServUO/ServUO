using System;
using Server.Mobiles;

namespace Server.Ethics
{
    public abstract class Ethic
    {
        public static readonly bool Enabled = false;
        public static readonly Ethic Hero = new Hero.HeroEthic();
        public static readonly Ethic Evil = new Evil.EvilEthic();
        public static readonly Ethic[] Ethics = new Ethic[]
        {
            Hero,
            Evil
        };
        protected EthicDefinition m_Definition;
        protected PlayerCollection m_Players;
        public Ethic()
        {
            this.m_Players = new PlayerCollection();
        }

        public EthicDefinition Definition
        {
            get
            {
                return this.m_Definition;
            }
        }
        public PlayerCollection Players
        {
            get
            {
                return this.m_Players;
            }
        }
        public static Ethic Find(Item item)
        {
            if ((item.SavedFlags & 0x100) != 0)
            {
                if (item.Hue == Hero.Definition.PrimaryHue)
                    return Hero;

                item.SavedFlags &= ~0x100;
            }

            if ((item.SavedFlags & 0x200) != 0)
            {
                if (item.Hue == Evil.Definition.PrimaryHue)
                    return Evil;

                item.SavedFlags &= ~0x200;
            }

            return null;
        }

        public static bool CheckTrade(Mobile from, Mobile to, Mobile newOwner, Item item)
        {
            Ethic itemEthic = Find(item);

            if (itemEthic == null || Find(newOwner) == itemEthic)
                return true;

            if (itemEthic == Hero)
                (from == newOwner ? to : from).SendMessage("Only heros may receive this item.");
            else if (itemEthic == Evil)
                (from == newOwner ? to : from).SendMessage("Only the evil may receive this item.");

            return false;
        }

        public static bool CheckEquip(Mobile from, Item item)
        {
            Ethic itemEthic = Find(item);

            if (itemEthic == null || Find(from) == itemEthic)
                return true;

            if (itemEthic == Hero)
                from.SendMessage("Only heros may wear this item.");
            else if (itemEthic == Evil)
                from.SendMessage("Only the evil may wear this item.");

            return false;
        }

        public static bool IsImbued(Item item)
        {
            return IsImbued(item, false);
        }

        public static bool IsImbued(Item item, bool recurse)
        {
            if (Find(item) != null)
                return true;

            if (recurse)
            {
                foreach (Item child in item.Items)
                {
                    if (IsImbued(child, true))
                        return true;
                }
            }

            return false;
        }

        public static void Initialize()
        {
            if (Enabled)
                EventSink.Speech += new SpeechEventHandler(EventSink_Speech);
        }

        public static void EventSink_Speech(SpeechEventArgs e)
        {
            if (e.Blocked || e.Handled)
                return;

            Player pl = Player.Find(e.Mobile);

            if (pl == null)
            {
                for (int i = 0; i < Ethics.Length; ++i)
                {
                    Ethic ethic = Ethics[i];

                    if (!ethic.IsEligible(e.Mobile))
                        continue;

                    if (!Insensitive.Equals(ethic.Definition.JoinPhrase.String, e.Speech))
                        continue;

                    bool isNearAnkh = false;

                    foreach (Item item in e.Mobile.GetItemsInRange(2))
                    {
                        if (item is Items.AnkhNorth || item is Items.AnkhWest)
                        {
                            isNearAnkh = true;
                            break;
                        }
                    }

                    if (!isNearAnkh)
                        continue;

                    pl = new Player(ethic, e.Mobile);

                    pl.Attach();

                    e.Mobile.FixedEffect(0x373A, 10, 30);
                    e.Mobile.PlaySound(0x209);

                    e.Handled = true;
                    break;
                }
            }
            else
            {
                if (e.Mobile is PlayerMobile && (e.Mobile as PlayerMobile).DuelContext != null)
                    return;

                Ethic ethic = pl.Ethic;

                for (int i = 0; i < ethic.Definition.Powers.Length; ++i)
                {
                    Power power = ethic.Definition.Powers[i];

                    if (!Insensitive.Equals(power.Definition.Phrase.String, e.Speech))
                        continue;

                    if (!power.CheckInvoke(pl))
                        continue;

                    power.BeginInvoke(pl);
                    e.Handled = true;

                    break;
                }
            }
        }

        public static Ethic Find(Mobile mob)
        {
            return Find(mob, false, false);
        }

        public static Ethic Find(Mobile mob, bool inherit)
        {
            return Find(mob, inherit, false);
        }

        public static Ethic Find(Mobile mob, bool inherit, bool allegiance)
        {
            Player pl = Player.Find(mob);

            if (pl != null)
                return pl.Ethic;

            if (inherit && mob is BaseCreature)
            {
                BaseCreature bc = (BaseCreature)mob;

                if (bc.Controlled)
                    return Find(bc.ControlMaster, false);
                else if (bc.Summoned)
                    return Find(bc.SummonMaster, false);
                else if (allegiance)
                    return bc.EthicAllegiance;
            }

            return null;
        }

        public abstract bool IsEligible(Mobile mob);

        public virtual void Deserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            switch ( version )
            {
                case 0:
                    {
                        int playerCount = reader.ReadEncodedInt();

                        for (int i = 0; i < playerCount; ++i)
                        {
                            Player pl = new Player(this, reader);

                            if (pl.Mobile != null)
                                Timer.DelayCall(TimeSpan.Zero, new TimerCallback(pl.CheckAttach));
                        }

                        break;
                    }
            }
        }

        public virtual void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt(0); // version

            writer.WriteEncodedInt(this.m_Players.Count);

            for (int i = 0; i < this.m_Players.Count; ++i)
                this.m_Players[i].Serialize(writer);
        }
    }
}