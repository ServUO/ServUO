using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests
{
    public class TheWayOfTheBladeQuest : BaseQuest
    { 
        public override bool DoneOnce
        {
            get
            {
                return true;
            }
        }
		
        /* The way of The Blade */
        public override object Title
        {
            get
            {
                return 1077658;
            }
        }
		
        /* Head East out of town and go to Old Haven. While wielding your sword, battle monster there until you have raised your 
        Swordsmanship skill to 50. *as you approach, you notice Jockles sizing you up with a skeptical look on his face* i can see 
        you want to learn how to handle a blade. It's a lot harder than it looks, and you're going to have to put alot of time and 
        effort if you ever want to be half as good as i am. I'll tell you what, kid, I'll help you get started, but you're going 
        to have to do all the work if you want to learn something. East of here, outside of town, is Old Haven. It's been overrun 
        with the nastiest of undead you've seen, which makes it a perfect place for you to turn that sloppy grin on your face into 
        actual skill at handling a sword. Make sure you have a sturdy Swordsmanship weapon in good repair before you leave. 'tis 
        no fun to travel all the way down there just to find out you forgot your blade! When you feel that you've cut down enough 
        of those foul smelling things to learn how to handle a blade without hurting yourself, come back to me. If i think you've 
        improved enough, I'll give you something suited for a real warrior. */ 
        public override object Description
        {
            get
            {
                return 1077661;
            }
        }
		
        /* Ha! I had a feeling you were a lily-livered pansy. You might have potential, but you're scared by a few smelly undead, 
        maybe it's better that you stay away from sharp objects. After all, you wouldn't want to hurt yourself swinging a sword. 
        If you change your mind, I might give you another chance...maybe. */
        public override object Refuse
        {
            get
            {
                return 1077662;
            }
        }
		
        /* *Jockles looks you up and down* Come on! You've got to work harder than that to get better. Now get out of here, go 
        kill some more of those undead to the east in Old Haven, and don't come back till you've got real skill. */
        public override object Uncomplete
        {
            get
            {
                return 1077663;
            }
        }
		
        /* Well, well, look at what we have here! You managed to do it after all. I have to say, I'm a little surprised that you 
        came back in one piece, but since you did. I've got a little something for you. This is a fine blade that served me well 
        in my younger days. Of course I've got much better swords at my disposal now, so I'll let you go ahead and use it under 
        one condition. Take goodcare of it and treat it with the respect that a fine sword deserves. You're one of the quickers 
        learners I've seen, but you still have a long way to go. Keep at it, and you'll get there someday. Happy hunting, kid. */
        public override object Complete
        {
            get
            {
                return 1077665;
            }
        }
		
        public TheWayOfTheBladeQuest()
            : base()
        { 
            this.AddObjective(new ApprenticeObjective(SkillName.Swords, 50, "Old Haven Training", 1077659, 1077660));
			
            // 1077659 You feel much more attuned to your blade. Your ability to hone your Swordsmanship skill is enhanced in this area.
            // 1077660 You feel less attuned to your blade. Your Swordsmanship learning potential is no longer enhanced.
			
            this.AddReward(new BaseReward(typeof(JocklesQuicksword), 1077666));
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
                return this.Owner.Skills.Swords.Base < 50;
        }
		
        public override void OnCompleted()
        { 
            this.Owner.SendLocalizedMessage(1077664, null, 0x23); // You have achieved the rank of Apprentice Swordsman. Return to Jockles in New Haven to see what kind of reward he has waiting for you. Hopefully he'll be a little nicer this time!
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
	
    public class Jockles : MondainQuester
    {
        public override Type[] Quests
        { 
            get
            {
                return new Type[] 
                {
                    typeof(TheWayOfTheBladeQuest)
                };
            }
        }
		
        [Constructable]
        public Jockles()
            : base("Jockles", "The Swordsmanship Instructor")
        { 
            this.SetSkill(SkillName.Anatomy, 120.0, 120.0);
            this.SetSkill(SkillName.Parry, 120.0, 120.0);
            this.SetSkill(SkillName.Healing, 120.0, 120.0);
            this.SetSkill(SkillName.Tactics, 120.0, 120.0);
            this.SetSkill(SkillName.Swords, 120.0, 120.0);
            this.SetSkill(SkillName.Focus, 120.0, 120.0);
        }
		
        public Jockles(Serial serial)
            : base(serial)
        {
        }
		
        public override void Advertise()
        {
            this.Say(1078135); // Talk to me to learn the way of the blade.
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
            this.AddItem(new Broadsword());
            this.AddItem(new PlateChest());
            this.AddItem(new PlateLegs());
            this.AddItem(new PlateGloves());
            this.AddItem(new PlateArms());
            this.AddItem(new PlateGorget());
            this.AddItem(new OrderShield());
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