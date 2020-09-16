using Server.Items;
using System;

namespace Server.Mobiles
{
    public class ThiefGuildmaster : BaseGuildmaster
    {
        [Constructable]
        public ThiefGuildmaster()
            : base("thief")
        {
            SetSkill(SkillName.DetectHidden, 75.0, 98.0);
            SetSkill(SkillName.Hiding, 65.0, 88.0);
            SetSkill(SkillName.Lockpicking, 85.0, 100.0);
            SetSkill(SkillName.Snooping, 90.0, 100.0);
            SetSkill(SkillName.Poisoning, 60.0, 83.0);
            SetSkill(SkillName.Stealing, 90.0, 100.0);
            SetSkill(SkillName.Fencing, 75.0, 98.0);
            SetSkill(SkillName.Stealth, 85.0, 100.0);
            SetSkill(SkillName.RemoveTrap, 85.0, 100.0);
        }

        public ThiefGuildmaster(Serial serial)
            : base(serial)
        {
        }

        public override NpcGuild NpcGuild => NpcGuild.ThievesGuild;
        public override TimeSpan JoinAge => Siege.SiegeShard ? TimeSpan.FromDays(0.0) : TimeSpan.FromDays(7.0);
        public override TimeSpan JoinGameAge => Siege.SiegeShard ? TimeSpan.FromDays(0.0) : TimeSpan.FromDays(2.0);
        public override void InitOutfit()
        {
            base.InitOutfit();

            if (Utility.RandomBool())
                AddItem(new Kryss());
            else
                AddItem(new Dagger());
        }

        public override bool CheckCustomReqs(PlayerMobile pm)
        {
            if (pm.Young && !Siege.SiegeShard)
            {
                SayTo(pm, 502089); // You cannot be a member of the Thieves' Guild while you are Young.
                return false;
            }
            else if (pm.Kills > 0)
            {
                SayTo(pm, 501050); // This guild is for cunning thieves, not oafish cutthroats.
                return false;
            }
            else if (pm.Skills[SkillName.Stealing].Base < 60.0 && !Siege.SiegeShard)
            {
                SayTo(pm, 501051); // You must be at least a journeyman pickpocket to join this elite organization.
                return false;
            }

            return true;
        }

        public override void SayWelcomeTo(Mobile m)
        {
            SayTo(m, 1008053); // Welcome to the guild! Stay to the shadows, friend.
        }

        public override bool HandlesOnSpeech(Mobile from)
        {
            if (from.InRange(Location, 2))
                return true;

            return base.HandlesOnSpeech(from);
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
            Mobile from = e.Mobile;

            if (!e.Handled && from is PlayerMobile && from.InRange(Location, 2) && e.HasKeyword(0x1F)) // *disguise*
            {
                PlayerMobile pm = (PlayerMobile)from;

                if (pm.NpcGuild == NpcGuild.ThievesGuild)
                    SayTo(from, 501839); // That particular item costs 700 gold pieces.
                else
                    SayTo(from, 501838); // I don't know what you're talking about.

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

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}