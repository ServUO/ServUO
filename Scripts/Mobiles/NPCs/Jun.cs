using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests
{
    public class WalkingSilentlyQuest : BaseQuest
    { 
        public override bool DoneOnce
        {
            get
            {
                return true;
            }
        }
		
        /* Walking Silently */
        public override object Title
        {
            get
            {
                return 1078174;
            }
        }
		
        /* Head East out of town and go to Old Haven. While wearing normal clothes, practice Stealth there until you 
        reach 50 Stealth skill. You there. You're not very quiet in your movements. I can help you with that. Not only 
        must you learn to become one with the shadows, but also you must learn to quiet your movements. Old Haven is 
        the ideal place to learn how to Stealth. Head East out of town and go to Old Haven, While wearing normal clothes, 
        practice Stealth tere. Stealth becomes more difficult as you wear heavier pieces of armor, so for now, only wear 
        clothes while practicing Stealth. You can only Stealth once you are hidden. If you become visable, use your Hiding 
        skill, and begin slowing walking. Come back to me once you have achived the rank of Apprentice Rought ( for Stealth), 
        and i will reward you with something useful. */ 
        public override object Description
        {
            get
            {
                return 1078178;
            }
        }
		
        /* If you want to learn to quiet your movements, talk to me and I will help you.*/
        public override object Refuse
        {
            get
            {
                return 1078179;
            }
        }
		
        /* You have not ahcived the rank of Apprentice Rouge (for Stealth). Talk to me when you feel you have accomplished this. */
        public override object Uncomplete
        {
            get
            {
                return 1078180;
            }
        }
		
        /* Good. You have learned to quiet your movements. If you haven't already talked to Chiyo, I advise you do so. Chiyo 
        can teach you how to become one with the shadows. Hiding and Stealth are essential skills to master when becoming a 
        Ninja. Here is your reward. This leather Ninja jacket is called " Twilight Jacket". It will offer greater protection 
        to you. I hope this will server you well. */
        public override object Complete
        {
            get
            {
                return 1078182;
            }
        }
		
        public WalkingSilentlyQuest()
            : base()
        { 
            this.AddObjective(new ApprenticeObjective(SkillName.Stealth, 50, "Old Haven Training", 1078176, 1078177));
			
            // 1078176 You feel you can easily slip into the shadows and walk silently here. Your ability to Stealth is enhanced in this area.
            // 1078177 You feel it is more difficult to Stealth here. Your ability to Stealth is no longer enhanced.
		
            this.AddReward(new BaseReward(typeof(TwilightJacket), 1078183));
        }
		
        public override bool CanOffer()
        {
            #region Scroll of Alacrity
            PlayerMobile pm = this.Owner as PlayerMobile;
            if (pm.AcceleratedStart > DateTime.UtcNow)
            {
                this.Owner.SendLocalizedMessage(1077951); // You are already under the effect of an accelerated skillgain scroll.
                return false;
            }
            #endregion
            else
                return this.Owner.Skills.Stealth.Base < 50;
        }
		
        public override void OnCompleted()
        { 
            this.Owner.SendLocalizedMessage(1078181, null, 0x23); // You have achieved the rank of Apprentice Rogue (for Stealth). Return to Jun in New Haven to claim your reward.
            this.Owner.PlaySound(this.CompleteSound);
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
	
    public class Jun : MondainQuester
    {
        public override Type[] Quests
        { 
            get
            {
                return new Type[] 
                {
                    typeof(WalkingSilentlyQuest)
                };
            }
        }
		
        [Constructable]
        public Jun()
            : base("Jun", "The Stealth Instructor")
        { 
            this.SetSkill(SkillName.Hiding, 120.0, 120.0);
            this.SetSkill(SkillName.Tactics, 120.0, 120.0);
            this.SetSkill(SkillName.Tracking, 120.0, 120.0);
            this.SetSkill(SkillName.Fencing, 120.0, 120.0);
            this.SetSkill(SkillName.Stealth, 120.0, 120.0);
            this.SetSkill(SkillName.Ninjitsu, 120.0, 120.0);
        }
		
        public Jun(Serial serial)
            : base(serial)
        {
        }
		
        public override void Advertise()
        {
            this.Say(1078175); // Walk Silently. Remain unseen. I can teach you.
        }
		
        public override void OnOfferFailed()
        { 
            this.Say(1077772); // I cannot teach you, for you know all I can teach!
        }
		
        public override void InitBody()
        { 
            this.Female = false;
            this.CantWalk = true;
            this.Race = Race.Human;		
		
            base.InitBody();
        }
		
        public override void InitOutfit()
        {
            this.AddItem(new Backpack());
            this.AddItem(new SamuraiTabi());
            this.AddItem(new LeatherNinjaPants());
            this.AddItem(new LeatherNinjaHood());
            this.AddItem(new LeatherNinjaBelt());
            this.AddItem(new LeatherNinjaMitts());
            this.AddItem(new LeatherNinjaJacket());
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