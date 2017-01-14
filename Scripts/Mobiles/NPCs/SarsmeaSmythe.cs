using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests
{
    public class TheInnerWarriorQuest : BaseQuest
    { 
        public override bool DoneOnce
        {
            get
            {
                return true;
            }
        }
		
        /* The Inner Warrior */
        public override object Title
        {
            get
            {
                return 1077696;
            }
        }
		
        /* Head East out of town to Old Haven. Expend stamina and mana until you have raised your Focus skill to 50. Well, hello there. 
        Don't you like quite the adventureer! You want to learn more about Focus, do you? I can teach you something about that, but 
        first you should know that not everyone can be a disciplined enough to excel at it. Focus is the ability to achive inner balance 
        in both body and spirit, so that you recover from physical and mental exertion faster than you other wise would. If you want to 
        practice Focus, the best place to do that is east of here, in Old Haven, where you'll find an undead infestation, Exert yourself 
        physically by engaging in combat and moving quickly. For testing your mental balance, expend mana in whatever way you find most 
        suitable to your abilites. Casting spells and using abilites work well for consuming your mana.	Go. Train hard, and you will find 
        that your concentration will imporve naturally. When you've improved your ability to focus yourself at an Apprentice level, come 
        back to me and i shall give you something worthy of your new ability. */ 
        public override object Description
        {
            get
            {
                return 1077699;
            }
        }
		
        /* I'm disappointed. You have alot of inner potential, and it would pain me greatly to see you waste that. Oh well. If you change 
        your mind ill be right here */
        public override object Refuse
        {
            get
            {
                return 1077700;
            }
        }
		
        /* Hell again. I see you've returned, but it seems that your Focus skill hasn't improved as much as it could have. Just head east, 
        to Old Haven, and exert yourself physically and mentally as much as possible. To do this physically engage in combat and move as 
        quickly as you can. For exerting yourself mentally, expend mana in whatever way you find most suitable to your abilites. Casting 
        spells and using abilites work well for consuming your mana. */
        public override object Uncomplete
        {
            get
            {
                return 1077701;
            }
        }
		
        /* Look Who it is! I knew you could do it if you just had the discipline to apply yourself. It feels good to recover from battle 
        so quickly, doesn't it? Just wait until you become a Grandmaster, It's amazing!	Please take this gift, as you've more than earned 
        it with your hard work. It will help you recover even faster during battle, and provides a bit of protection aswell. You have so 
        much more potential, so don't stop trying to improve your Focus now! Safe travels! */
        public override object Complete
        {
            get
            {
                return 1077703;
            }
        }
		
        public TheInnerWarriorQuest()
            : base()
        { 
            this.AddObjective(new ApprenticeObjective(SkillName.Focus, 50, "Old Haven Training", 1077697, 1077698));
			
            // 1077697 You feel much more attuned to yourself. Your ability to improve Focus skill is enhanced in this area.
            // 1077698 You feel like you don't even know yourself anymore! Your Focus learning potential is no longer enhanced.
			
            this.AddReward(new BaseReward(typeof(ClaspOfConcentration), 1077695)); 
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
                return this.Owner.Skills.Focus.Base < 50;
        }
		
        public override void OnCompleted()
        { 
            this.Owner.SendLocalizedMessage(1077702, null, 0x23); // You have achieved the rank of Apprentice Stoic (for Focus). Return to Sarsmea Smythe in New Haven to see what kind of reward she has waiting for you. 
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
	
    public class SarsmeaSmythe : MondainQuester
    {
        public override Type[] Quests
        { 
            get
            {
                return new Type[] 
                {
                    typeof(TheInnerWarriorQuest)
                };
            }
        }
		
        [Constructable]
        public SarsmeaSmythe()
            : base("Sarsmea Smythe", "The Focus Instructor")
        { 
            this.SetSkill(SkillName.Anatomy, 120.0, 120.0);
            this.SetSkill(SkillName.Parry, 120.0, 120.0);
            this.SetSkill(SkillName.Healing, 120.0, 120.0);
            this.SetSkill(SkillName.Tactics, 120.0, 120.0);
            this.SetSkill(SkillName.Swords, 120.0, 120.0);
            this.SetSkill(SkillName.Focus, 120.0, 120.0);
        }
		
        public SarsmeaSmythe(Serial serial)
            : base(serial)
        {
        }
		
        public override void Advertise()
        {
            this.Say(1078139);  // Know yourself, and you will become a true warrior.
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
            this.AddItem(new LeatherLegs());
            this.AddItem(new ThighBoots());
            this.AddItem(new FemaleLeatherChest());
            this.AddItem(new StuddedGloves());
            this.AddItem(new LeatherNinjaBelt());
            this.AddItem(new StuddedGorget());
            this.AddItem(new LightPlateJingasa());
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