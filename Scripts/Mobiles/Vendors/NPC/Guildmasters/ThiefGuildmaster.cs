using System;
using Server.Items;

namespace Server.Mobiles
{
    public class ThiefGuildmaster : BaseGuildmaster
    {
        [Constructable]
        public ThiefGuildmaster()
            : base("thief")
        {
            this.SetSkill(SkillName.DetectHidden, 75.0, 98.0);
            this.SetSkill(SkillName.Hiding, 65.0, 88.0);
            this.SetSkill(SkillName.Lockpicking, 85.0, 100.0);
            this.SetSkill(SkillName.Snooping, 90.0, 100.0);
            this.SetSkill(SkillName.Poisoning, 60.0, 83.0);
            this.SetSkill(SkillName.Stealing, 90.0, 100.0);
            this.SetSkill(SkillName.Fencing, 75.0, 98.0);
            this.SetSkill(SkillName.Stealth, 85.0, 100.0);
            this.SetSkill(SkillName.RemoveTrap, 85.0, 100.0);
        }

        public ThiefGuildmaster(Serial serial)
            : base(serial)
        {
        }

        public override NpcGuild NpcGuild
        {
            get
            {
                return NpcGuild.ThievesGuild;
            }
        }
        public override TimeSpan JoinAge
        {
            get
            {
                return TimeSpan.FromDays(7.0);
            }
        }
        public override void InitOutfit()
        {
            base.InitOutfit();

            if (Utility.RandomBool())
                this.AddItem(new Server.Items.Kryss());
            else
                this.AddItem(new Server.Items.Dagger());
        }

        public override bool CheckCustomReqs(PlayerMobile pm)
        {
            if (pm.Young)
            {
                this.SayTo(pm, 502089); // You cannot be a member of the Thieves' Guild while you are Young.
                return false;
            }
            else if (pm.Kills > 0)
            {
                this.SayTo(pm, 501050); // This guild is for cunning thieves, not oafish cutthroats.
                return false;
            }
            else if (pm.Skills[SkillName.Stealing].Base < 60.0)
            {
                this.SayTo(pm, 501051); // You must be at least a journeyman pickpocket to join this elite organization.
                return false;
            }

            return true;
        }

        public override void SayWelcomeTo(Mobile m)
        {
            this.SayTo(m, 1008053); // Welcome to the guild! Stay to the shadows, friend.
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

            if (!e.Handled && from is PlayerMobile && from.InRange(this.Location, 2) && e.HasKeyword(0x1F)) // *disguise*
            {
                PlayerMobile pm = (PlayerMobile)from;

                if (pm.NpcGuild == NpcGuild.ThievesGuild)
                    this.SayTo(from, 501839); // That particular item costs 700 gold pieces.
                else
                    this.SayTo(from, 501838); // I don't know what you're talking about.

                e.Handled = true;
            }

            base.OnSpeech(e);
        }

        public override bool OnGoldGiven(Mobile from, Gold dropped)
        {
            if (from is PlayerMobile && dropped.Amount == 700)
            {
                PlayerMobile pm = (PlayerMobile)from;

                if (pm.NpcGuild == NpcGuild.ThievesGuild)
                {
                    from.AddToBackpack(new DisguiseKit());

                    dropped.Delete();
                    return true;
                }
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