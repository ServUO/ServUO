using System;
using System.Collections.Generic;
using Server.Items;
using Server.Network;

namespace Server.Mobiles
{
    public abstract class BaseGuildmaster : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        public BaseGuildmaster(string title)
            : base(title)
        {
            this.Title = String.Format("the {0} {1}", title, this.Female ? "guildmistress" : "guildmaster");
        }

        public BaseGuildmaster(Serial serial)
            : base(serial)
        {
        }

        public override bool IsActiveVendor
        {
            get
            {
                return false;
            }
        }
        public override bool ClickTitle
        {
            get
            {
                return false;
            }
        }
        public virtual int JoinCost
        {
            get
            {
                return 500;
            }
        }
        public virtual TimeSpan JoinAge
        {
            get
            {
                return TimeSpan.FromDays(0.0);
            }
        }
        public virtual TimeSpan JoinGameAge
        {
            get
            {
                return TimeSpan.FromDays(2.0);
            }
        }
        public virtual TimeSpan QuitAge
        {
            get
            {
                return TimeSpan.FromDays(7.0);
            }
        }
        public virtual TimeSpan QuitGameAge
        {
            get
            {
                return TimeSpan.FromDays(4.0);
            }
        }
        protected override List<SBInfo> SBInfos
        {
            get
            {
                return this.m_SBInfos;
            }
        }
        public override void InitSBInfo()
        {
        }

        public virtual bool CheckCustomReqs(PlayerMobile pm)
        {
            return true;
        }

        public virtual void SayGuildTo(Mobile m)
        {
            this.SayTo(m, 1008055 + (int)this.NpcGuild);
        }

        public virtual void SayWelcomeTo(Mobile m)
        {
            this.SayTo(m, 1008054); // Welcome to the guild! Thou shalt find that fellow members shall grant thee lower prices in shops.
        }

        public virtual void SayPriceTo(Mobile m)
        {
            m.Send(new MessageLocalizedAffix(this.Serial, this.Body, MessageType.Regular, this.SpeechHue, 3, 1008052, this.Name, AffixType.Append, this.JoinCost.ToString(), ""));
        }

        public virtual bool WasNamed(string speech)
        {
            string name = this.Name;

            return (name != null && Insensitive.StartsWith(speech, name));
        }

        public override bool HandlesOnSpeech(Mobile from)
        {
            if (from.InRange(this.Location, 2))
                return true;

            return base.HandlesOnSpeech(from);
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
            Mobile from = e.Mobile;

            if (!e.Handled && from is PlayerMobile && from.InRange(this.Location, 2) && this.WasNamed(e.Speech))
            {
                PlayerMobile pm = (PlayerMobile)from;

                if (e.HasKeyword(0x0004)) // *join* | *member*
                {
                    if (pm.NpcGuild == this.NpcGuild)
                        this.SayTo(from, 501047); // Thou art already a member of our guild.
                    else if (pm.NpcGuild != NpcGuild.None)
                        this.SayTo(from, 501046); // Thou must resign from thy other guild first.
                    else if (pm.GameTime < this.JoinGameAge || (pm.CreationTime + this.JoinAge) > DateTime.UtcNow)
                        this.SayTo(from, 501048); // You are too young to join my guild...
                    else if (this.CheckCustomReqs(pm))
                        this.SayPriceTo(from);

                    e.Handled = true;
                }
                else if (e.HasKeyword(0x0005)) // *resign* | *quit*
                {
                    if (pm.NpcGuild != this.NpcGuild)
                    {
                        this.SayTo(from, 501052); // Thou dost not belong to my guild!
                    }
                    else if ((pm.NpcGuildJoinTime + this.QuitAge) > DateTime.UtcNow || (pm.NpcGuildGameTime + this.QuitGameAge) > pm.GameTime)
                    {
                        this.SayTo(from, 501053); // You just joined my guild! You must wait a week to resign.
                    }
                    else
                    {
                        this.SayTo(from, 501054); // I accept thy resignation.
                        pm.NpcGuild = NpcGuild.None;
                    }

                    e.Handled = true;
                }
            }

            base.OnSpeech(e);
        }

        public override bool OnGoldGiven(Mobile from, Gold dropped)
        {
            if (from is PlayerMobile && dropped.Amount == this.JoinCost)
            {
                PlayerMobile pm = (PlayerMobile)from;

                if (pm.NpcGuild == this.NpcGuild)
                {
                    this.SayTo(from, 501047); // Thou art already a member of our guild.
                }
                else if (pm.NpcGuild != NpcGuild.None)
                {
                    this.SayTo(from, 501046); // Thou must resign from thy other guild first.
                }
                else if (pm.GameTime < this.JoinGameAge || (pm.CreationTime + this.JoinAge) > DateTime.UtcNow)
                {
                    this.SayTo(from, 501048); // You are too young to join my guild...
                }
                else if (this.CheckCustomReqs(pm))
                {
                    this.SayWelcomeTo(from);

                    pm.NpcGuild = this.NpcGuild;
                    pm.NpcGuildJoinTime = DateTime.UtcNow;
                    pm.NpcGuildGameTime = pm.GameTime;

                    dropped.Delete();
                    return true;
                }

                return false;
            }

            return base.OnGoldGiven(from, dropped);
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