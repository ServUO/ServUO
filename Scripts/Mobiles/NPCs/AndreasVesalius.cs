using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests
{
    public class KnowThineEnemyQuest : BaseQuest
    { 
        public override bool DoneOnce
        {
            get
            {
                return true;
            }
        }
		
        /* Knowing Thine Enemy */
        public override object Title
        {
            get
            {
                return 1077685;
            }
        }
		
        /* Head East out of town to Old Haven. Battle monsters there, or heal yourself and other players, until you 
        have raised your Anatomy skill to 50.<br><center>------</center><br>Hail and well met. You must be here to 
        improve your knowledge of Anatomy. Well, you've come to the right place because I can teach you what you need 
        to know. At least all you'll need to know for now. Haha!<br><br>Knowing about how living things work inside 
        can be a very useful skill. Not only can you learn where to strike an opponent to hurt him the most, but you 
        can use what you learn to heal wounds better as well. Just walking around town, you can even tell if someone 
        is strong or weak or if they happen to be particularly dexterous or not.<BR><BR>If you're interested in learning 
        more, I'd advise you to head out to Old Haven, just to the east, and jump into the fray. You'll learn best by 
        engaging in combat while keeping you and your fellow adventurers healed, or you can even try sizing up your 
        opponents.<br><br>While you're gone, I'll dig up something you may find useful. */ 
        public override object Description
        {
            get
            {
                return 1077688;
            }
        }
		
        /* It's your choice, but I wouldn't head out there without knowing what makes those things tick inside! If you 
        change your mind, you can find me right here dissecting frogs, cats or even the occasional unlucky adventurer. */
        public override object Refuse
        {
            get
            {
                return 1077689;
            }
        }
		
        /* I'm surprised to see you back so soon. You've still got a ways to go if you want to really understand the 
        science of Anatomy. Head out to Old Haven and practice combat and healing yourself or other adventurers. */
        public override object Uncomplete
        {
            get
            {
                return 1077690;
            }
        }
		
        /* By the Virtues, you've done it! Congratulations mate! You still have quite a ways to go if you want to perfect 
        your knowledge of Anatomy, but I know you'll get there someday. Just keep at it.<br><br>In the meantime, here's a 
        piece of armor that you might find useful. It's not fancy, but it'll serve you well if you choose to wear it.<br>
        <br>Happy adventuring, and remember to keep your cranium separate from your clavicle! */
        public override object Complete
        {
            get
            {
                return 1077692;
            }
        }
		
        public KnowThineEnemyQuest()
            : base()
        { 
            this.AddObjective(new ApprenticeObjective(SkillName.Anatomy, 50, "Old Haven Training", 1077686, 1077687));
			
            // 1077686 You feel very willing to learn more about the body. Your ability to hone your Anatomy skill is enhanced in this area.
            // 1077687 You lose your ambition to learn about the body. Your Anatomy skill learning potential is no longer enhanced.
		
            this.AddReward(new BaseReward(typeof(TunicOfGuarding), 1077693));
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
                return this.Owner.Skills.Anatomy.Base < 50;
        }
		
        public override void OnCompleted()
        { 
            this.Owner.SendLocalizedMessage(1077691, null, 0x23); // You have achieved the rank of Apprentice Healer (for Anatomy). Return to Andreas Vesalius in New Haven as soon as you can to claim your reward.
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
	
    public class AndreasVesalius : MondainQuester
    {
        public override Type[] Quests
        { 
            get
            {
                return new Type[] 
                {
                    typeof(KnowThineEnemyQuest)
                };
            }
        }
		
        [Constructable]
        public AndreasVesalius()
            : base("Andreas Vesalius", "The Anatomy Instructor")
        { 
            this.SetSkill(SkillName.Anatomy, 120.0, 120.0);
            this.SetSkill(SkillName.Parry, 120.0, 120.0);
            this.SetSkill(SkillName.Healing, 120.0, 120.0);
            this.SetSkill(SkillName.Tactics, 120.0, 120.0);
            this.SetSkill(SkillName.Swords, 120.0, 120.0);
            this.SetSkill(SkillName.Focus, 120.0, 120.0);
        }
		
        public AndreasVesalius(Serial serial)
            : base(serial)
        {
        }
		
        public override void Advertise()
        {
            this.Say(1078138); // Learning of the body will allow you to excel in combat.
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
            this.AddItem(new BlackStaff());	
            this.AddItem(new Boots());	
            this.AddItem(new LongPants());	
            this.AddItem(new Tunic(0x66D));	
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