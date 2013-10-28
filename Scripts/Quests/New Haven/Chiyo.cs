using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests
{
    public class BecomingOneWithTheShadowsQuest : BaseQuest
    { 
        public override bool DoneOnce
        {
            get
            {
                return true;
            }
        }
		
        /* Becoming One With The Shadows */
        public override object Title
        {
            get
            {
                return 1078164;
            }
        }
		
        /* Practice hiding in the Ninja Dojo until you reach 50 Hiding skill.<br><center>------</center><br>Come closer. 
        Don't be afraid. The shadows will not harm you. To be a successful Ninja, you must learn to become one with the 
        shadows. The Ninja Dojo is the ideal place to learn the art of concealment. Practice hiding here.<br><br>Talk to 
        me once you have achieved the rank of Apprentice Rogue (for Hiding), and I shall reward you. */ 
        public override object Description
        {
            get
            {
                return 1078168;
            }
        }
		
        /* If you wish to become one with the shadows, come back and talk to me. */
        public override object Refuse
        {
            get
            {
                return 1078169;
            }
        }
		
        /* You have not achieved the rank of Apprentice Rogue (for Hiding). Talk to me when you feel you have accomplished 
        this. */
        public override object Uncomplete
        {
            get
            {
                return 10778170;
            }
        }
		
        /* Not bad at all. You have learned to control your fear of the dark and you are becoming one with the shadows. If 
        you haven't already talked to Jun, I advise you do so. Jun can teach you how to stealth undetected. Hiding and 
        Stealth are essential skills to master when becoming a Ninja.<br><br>As promised, I have a reward for you. Here are 
        some smokebombs. As long as you are an Apprentice Ninja and have mana available you will be able to use them. They 
        will allow you to hide while in the middle of combat. I hope these serve you well. */
        public override object Complete
        {
            get
            {
                return 1078172;
            }
        }
		
        public BecomingOneWithTheShadowsQuest()
            : base()
        { 
            this.AddObjective(new ApprenticeObjective(SkillName.Hiding, 50, "Haven Dojo", 1078166, 1078167));
			
            // 1078166 You feel you can easily slip into the shadows here. Your ability to hide is enhanced in this area.
            // 1078167 You feel it is more difficult to hide here. Your ability to hide is no longer enhanced.
		  
            this.AddReward(new BaseReward(typeof(BagOfSmokeBombs), 1078173));
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
                return this.Owner.Skills.Hiding.Base < 50;
        }
		
        public override void OnCompleted()
        { 
            this.Owner.SendLocalizedMessage(1078171, null, 0x23); // You have achieved the rank of Apprentice Rogue (for Hiding). Return to Chiyo in New Haven to claim your reward.
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
	
    public class Chiyo : MondainQuester
    {
        public override Type[] Quests
        { 
            get
            {
                return new Type[] 
                {
                    typeof(BecomingOneWithTheShadowsQuest)
                };
            }
        }
		
        [Constructable]
        public Chiyo()
            : base("Chiyo", "The Hiding Instructor")
        { 
            this.SetSkill(SkillName.Hiding, 120.0, 120.0);
            this.SetSkill(SkillName.Tracking, 120.0, 120.0);
            this.SetSkill(SkillName.Healing, 120.0, 120.0);
            this.SetSkill(SkillName.Tactics, 120.0, 120.0);
            this.SetSkill(SkillName.Fencing, 120.0, 120.0);
            this.SetSkill(SkillName.Stealth, 120.0, 120.0);
            this.SetSkill(SkillName.Ninjitsu, 120.0, 120.0);
        }
		
        public Chiyo(Serial serial)
            : base(serial)
        {
        }
		
        public override void Advertise()
        {
            this.Say(1078165); // To be undetected means you cannot be harmed.
        }
		
        public override void OnOfferFailed()
        { 
            this.Say(1077772); // I cannot teach you, for you know all I can teach!
        }
		
        public override void InitBody()
        { 
            this.Female = false;
            this.CantWalk = true;
            this.Body = 247;			
        }
		
        public override void InitOutfit()
        { 
            this.AddItem(new Backpack());	
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