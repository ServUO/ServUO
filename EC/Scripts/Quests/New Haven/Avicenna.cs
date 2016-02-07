using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests
{
    public class BruisesBandagesAndBloodQuest : BaseQuest
    { 
        public override bool DoneOnce
        {
            get
            {
                return true;
            }
        }
		
        /* Bruises, Bandages and Blood */
        public override object Title
        {
            get
            {
                return 1077676;
            }
        }
		
        /* Head East out of town and go to Old Haven. Heal yourself and other players until you have raised your 
        Healing skill to 50.<br><center>------</center><br>Ah, welcome to my humble practice. I am Avicenna, New 
        Haven's resident Healer. A lot of adventurers head out into the wild from here, so I keep rather busy when 
        they come back bruised, bleeding, or worse.<br><br>I can teach you how to bandage a wound, sure, but it's 
        not a job for the queasy! For some folks, the mere sight of blood is too much for them, but it's something 
        you'll get used to over time. It is one thing to cut open a living thing, but it's quite another to sew it 
        back up and save it from sure death. 'Tis noble work, healing.<br><br>Best way for you to practice fixing 
        up wounds is to head east out to Old Haven and either practice binding up your own wounds, or practice on 
        someone else. Surely they'll be grateful for the assistance.<br><br>Make sure to take enough bandages with 
        you! You don't want to run out in the middle of a tough fight. */ 
        public override object Description
        {
            get
            {
                return 1077679;
            }
        }
		
        /* No? Are you sure? Well, when you feel that you're ready to practice your healing, come back to me. I'll 
        be right here, fixing up adventurers and curing the occasional cold! */
        public override object Refuse
        {
            get
            {
                return 1077680;
            }
        }
		
        /*Hail! 'Tis good to see you again. Unfortunately, you're not quite ready to call yourself an Apprentice 
        Healer quite yet. Head back out to Old Haven, due east from here, and bandage up some wounds. Yours or 
        someone else's, it doesn't much matter.*/
        public override object Uncomplete
        {
            get
            {
                return 1077681;
            }
        }
		
        /*Hello there, friend. I see you've returned in one piece, and you're an Apprentice Healer to boot! You 
        should be proud of your accomplishment, as not everyone has "the touch" when it comes to healing.<br><br>
        I can't stand to see such good work go unrewarded, so I have something I'd like you to have. It's not much, 
        but it'll help you heal just a little faster, and maybe keep you alive.<br><br>Good luck out there, friend,
        and don't forget to help your fellow adventurer whenever possible!*/
        public override object Complete
        {
            get
            {
                return 1077683;
            }
        }
		
        public BruisesBandagesAndBloodQuest()
            : base()
        { 
            this.AddObjective(new ApprenticeObjective(SkillName.Healing, 50, "Old Haven Training", 1077677, 1077678));
			
            // 1077677 You feel fresh and are eager to heal wounds. Your ability to improve your Healing skill is enhanced in this area.
            // 1077678 You feel as if you've seen enough blood to last a lifetime. Your Healing learning potential is no longer enhanced.
		
            this.AddReward(new BaseReward(typeof(HealersTouch), 1077684));
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
                return this.Owner.Skills.Healing.Base < 50;
        }
		
        public override void OnCompleted()
        { 
            this.Owner.SendLocalizedMessage(1077682, null, 0x23); // You have achieved the rank of Apprentice Healer. Return to Avicenna in New Haven as soon as you can to claim your reward.
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
	
    public class Avicenna : MondainQuester
    {
        public override Type[] Quests
        { 
            get
            {
                return new Type[] 
                {
                    typeof(BruisesBandagesAndBloodQuest)
                };
            }
        }
		
        [Constructable]
        public Avicenna()
            : base("Avicenna", "The Healing Instructor")
        { 
            this.SetSkill(SkillName.Anatomy, 120.0, 120.0);
            this.SetSkill(SkillName.Parry, 120.0, 120.0);
            this.SetSkill(SkillName.Healing, 120.0, 120.0);
            this.SetSkill(SkillName.Tactics, 120.0, 120.0);
            this.SetSkill(SkillName.Swords, 120.0, 120.0);
            this.SetSkill(SkillName.Focus, 120.0, 120.0);
        }
		
        public Avicenna(Serial serial)
            : base(serial)
        {
        }
		
        public override void Advertise()
        {
            this.Say(1078137); // A warrior needs to learn how to apply bandages to wounds.
        }
		
        public override void OnOfferFailed()
        { 
            this.Say(1077772); // I cannot teach you, for you know all I can teach!
        }
		
        public override void InitBody()
        { 
            this.Female = true;
            this.CantWalk = true;
            this.Race = Race.Human;		
		
            base.InitBody();
        }
		
        public override void InitOutfit()
        {
            this.AddItem(new Backpack());
            this.AddItem(new Robe(0x66D));
            this.AddItem(new Boots());
            this.AddItem(new GnarledStaff());
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