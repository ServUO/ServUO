using Server.Items;
using Server.Mobiles;
using System;

namespace Server.Engines.Quests
{
    public class WalkingSilentlyQuest : BaseQuest
    {
        public override bool DoneOnce => true;

        /* Walking Silently */
        public override object Title => 1078174;

        /* Head East out of town and go to Old Haven. While wearing normal clothes, practice Stealth there until you 
        reach 50 Stealth skill. You there. You're not very quiet in your movements. I can help you with that. Not only 
        must you learn to become one with the shadows, but also you must learn to quiet your movements. Old Haven is 
        the ideal place to learn how to Stealth. Head East out of town and go to Old Haven, While wearing normal clothes, 
        practice Stealth tere. Stealth becomes more difficult as you wear heavier pieces of armor, so for now, only wear 
        clothes while practicing Stealth. You can only Stealth once you are hidden. If you become visable, use your Hiding 
        skill, and begin slowing walking. Come back to me once you have achived the rank of Apprentice Rought ( for Stealth), 
        and i will reward you with something useful. */
        public override object Description => 1078178;

        /* If you want to learn to quiet your movements, talk to me and I will help you.*/
        public override object Refuse => 1078179;

        /* You have not ahcived the rank of Apprentice Rouge (for Stealth). Talk to me when you feel you have accomplished this. */
        public override object Uncomplete => 1078180;

        /* Good. You have learned to quiet your movements. If you haven't already talked to Chiyo, I advise you do so. Chiyo 
        can teach you how to become one with the shadows. Hiding and Stealth are essential skills to master when becoming a 
        Ninja. Here is your reward. This leather Ninja jacket is called " Twilight Jacket". It will offer greater protection 
        to you. I hope this will server you well. */
        public override object Complete => 1078182;

        public WalkingSilentlyQuest()
            : base()
        {
            AddObjective(new ApprenticeObjective(SkillName.Stealth, 50, "Old Haven Training", 1078176, 1078177));

            // 1078176 You feel you can easily slip into the shadows and walk silently here. Your ability to Stealth is enhanced in this area.
            // 1078177 You feel it is more difficult to Stealth here. Your ability to Stealth is no longer enhanced.

            AddReward(new BaseReward(typeof(TwilightJacket), 1078183));
        }

        public override bool CanOffer()
        {
            #region Scroll of Alacrity
            PlayerMobile pm = Owner as PlayerMobile;
            if (pm.AcceleratedStart > DateTime.UtcNow)
            {
                Owner.SendLocalizedMessage(1077951); // You are already under the effect of an accelerated skillgain scroll.
                return false;
            }
            #endregion
            else
                return Owner.Skills.Stealth.Base < 50;
        }

        public override void OnCompleted()
        {
            Owner.SendLocalizedMessage(1078181, null, 0x23); // You have achieved the rank of Apprentice Rogue (for Stealth). Return to Jun in New Haven to claim your reward.
            Owner.PlaySound(CompleteSound);
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

    public class Jun : MondainQuester
    {
        public override Type[] Quests => new Type[]
                {
                    typeof(WalkingSilentlyQuest)
                };

        [Constructable]
        public Jun()
            : base("Jun", "The Stealth Instructor")
        {
            SetSkill(SkillName.Hiding, 120.0, 120.0);
            SetSkill(SkillName.Tactics, 120.0, 120.0);
            SetSkill(SkillName.Tracking, 120.0, 120.0);
            SetSkill(SkillName.Fencing, 120.0, 120.0);
            SetSkill(SkillName.Stealth, 120.0, 120.0);
            SetSkill(SkillName.Ninjitsu, 120.0, 120.0);
        }

        public Jun(Serial serial)
            : base(serial)
        {
        }

        public override void Advertise()
        {
            Say(1078175); // Walk Silently. Remain unseen. I can teach you.
        }

        public override void OnOfferFailed()
        {
            Say(1077772); // I cannot teach you, for you know all I can teach!
        }

        public override void InitBody()
        {
            Female = false;
            CantWalk = true;
            Race = Race.Human;

            base.InitBody();
        }

        public override void InitOutfit()
        {
            AddItem(new Backpack());
            AddItem(new SamuraiTabi());
            AddItem(new LeatherNinjaPants());
            AddItem(new LeatherNinjaHood());
            AddItem(new LeatherNinjaBelt());
            AddItem(new LeatherNinjaMitts());
            AddItem(new LeatherNinjaJacket());
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