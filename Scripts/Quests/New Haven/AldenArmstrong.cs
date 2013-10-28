using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests
{
    public class TheArtOfWarQuest : BaseQuest
    { 
        public override bool DoneOnce
        {
            get
            {
                return true;
            }
        }
		
        /* The Art of War */
        public override object Title
        {
            get
            {
                return 1077667;
            }
        }
		
        /* Head East out of town to Old Haven. Battle monsters there until you have raised your Tactics skill 
        to 50.<br><center>------</center><br>Knowing how to hold a weapon is only half of the battle. The other 
        half is knowing how to use it against an opponent. It's one thing to kill a few bunnies now and then 
        for fun, but a true warrior knows that the right moves to use against a lich will pretty much get your 
        arse fried by a dragon.<br><br>I'll help teach you how to fight so that when you do come up against that 
        dragon, maybe you won't have to walk out of there "OooOOooOOOooOO'ing" and looking for a healer.<br><br>
        There are some undead that need cleaning out in Old Haven towards the east. Why don't you head on over 
        there and practice killing things?<br><br>When you feel like you've got the basics down, come back to me 
        and I'll see if I can scrounge up an item to help you in your adventures later on. */ 
        public override object Description
        {
            get
            {
                return 1077670;
            }
        }
		
        /* That's too bad. I really thought you had it in you. Well, I'm sure those undead will still be there 
        later, so if you change your mind, feel free to stop on by and I'll help you the best I can. */
        public override object Refuse
        {
            get
            {
                return 1077671;
            }
        }
		
        /* You're making some progress, that i can tell, but you're not quite good enough to last for very long 
        out there by yourself. Head back to Old Haven, to the east, and kill some more undead. */
        public override object Uncomplete
        {
            get
            {
                return 1077672;
            }
        }
		
        /* Hey, good job killing those undead! Hopefully someone will come along and clean up the mess. All that 
        blood and guts tends to stink after a few days, and when the wind blows in from the east, it can raise a 
        mighty stink!<br><br>Since you performed valiantly, please take these arms and use them well. I've seen 
        a few too many harvests to be running around out there myself, so you might as well take it.<br><br>There 
        is a lot left for you to learn, but I think you'll do fine. Remember to keep your elbows in and stick'em 
        where it hurts the most! */
        public override object Complete
        {
            get
            {
                return 1077674;
            }
        }
		
        public TheArtOfWarQuest()
            : base()
        { 
            this.AddObjective(new ApprenticeObjective(SkillName.Tactics, 50, "Old Haven Training", 1077668, 1077669));
			
            // 1077668 You feel like practicing combat here would really help you learn to fight better. Your ability to raise your Tactics skill is enhanced in this area.
            // 1077669 You feel less able to absorb the lessons of combat. Your Tactics learning potential is no longer enhanced.
			
            this.AddReward(new BaseReward(typeof(ArmsOfArmstrong), 1077675));
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
                return this.Owner.Skills.Tactics.Base < 50;
        }
		
        public override void OnCompleted()
        { 
            this.Owner.SendLocalizedMessage(1077673, null, 0x23); // You have achieved the rank of Apprentice Warrior. Return to Alden Armstrong in New Haven to claim your reward.
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
	
    public class AldenArmstrong : MondainQuester
    {
        public override Type[] Quests
        { 
            get
            {
                return new Type[] 
                {
                    typeof(TheArtOfWarQuest)
                };
            }
        }
		
        [Constructable]
        public AldenArmstrong()
            : base("Alden Armstrong", "The Tactics Instructor")
        { 
            this.SetSkill(SkillName.Anatomy, 120.0, 120.0);
            this.SetSkill(SkillName.Parry, 120.0, 120.0);
            this.SetSkill(SkillName.Healing, 120.0, 120.0);
            this.SetSkill(SkillName.Tactics, 120.0, 120.0);
            this.SetSkill(SkillName.Swords, 120.0, 120.0);
            this.SetSkill(SkillName.Focus, 120.0, 120.0);
        }
		
        public AldenArmstrong(Serial serial)
            : base(serial)
        {
        }
		
        public override void Advertise()
        {
            this.Say(1078136); // There is an art to slaying your enemies swiftly. It's called tactics, and I can teach it to you.
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
            this.AddItem(new Shoes());
            this.AddItem(new StuddedLegs());
            this.AddItem(new StuddedGloves());
            this.AddItem(new StuddedGorget());
            this.AddItem(new StuddedChest());
            this.AddItem(new StuddedArms());
            this.AddItem(new Katana());			
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