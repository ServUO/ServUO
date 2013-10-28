using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests
{
    public class CrushingBonesAndTakingNamesQuest : BaseQuest
    { 
        public override bool DoneOnce
        {
            get
            {
                return true;
            }
        }
		
        /* Crushing Bones and Taking Names */
        public override object Title
        {
            get
            {
                return 1078070;
            }
        }
		
        /* Head East out of town and go to Old Haven. While wielding your mace,battle monster there until you have 
        raised your Mace Fighting skill to 50. I see you want to learn a real weapon skill and not that toothpick 
        training Jockles hasto offer. Real warriors are called Armsmen, and they wield mace weapons. No doubt about 
        it. Nothing is more satisfying than knocking the wind out of your enemies, smashing there armor, crushing 
        their bones, and taking there names. Want to learn how to wield a mace? Well i have an assignment for you. 
        Head East out of town and go to Old Haven. Undead have plagued the town, so there are plenty of bones for 
        you to smash there.	Come back to me after you have ahcived the rank of Apprentice Armsman, and i will 
        reward you with a real weapon.*/ 
        public override object Description
        {
            get
            {
                return 1078065;
            }
        }
		
        /* I thought you wanted to be an Armsman and really make something of yourself. You have potential, kid, 
        but if you want to play with toothpicks, run to Jockles and he will teach you how to clean your teeth 
        with a sword. If you change your mind, come back to me, and i will show you how to wield a real weapon. */
        public override object Refuse
        {
            get
            {
                return 1078068;
            }
        }
		
        /* Listen kid. There are a lot of undead in Old Haven, and you haven't smashed enough of them yet. So get 
        back there and do some more cleansing. */
        public override object Uncomplete
        {
            get
            {
                return 1078067;
            }
        }
		
        /* Now that's what I'm talking about! Well done! Don't you like crushing bones and taking names? As i promised, 
        here is a war mace for you. It hits hard. It swings fast. It hits often. What more do you need? Now get out of 
        here and crush some more enemies! */
        public override object Complete
        {
            get
            {
                return 1078069;
            }
        }
		
        public CrushingBonesAndTakingNamesQuest()
            : base()
        { 
            this.AddObjective(new ApprenticeObjective(SkillName.Macing, 50, "Old Haven Training", 1078063, 1078064));
			
            // 1078063 You feel much more attuned to your mace. Your ability to hone your Mace Fighting skill is enhanced in this area.
            // 1078064 You feel less attuned to your mace. Your Mace Fighting learning potential is no longer enhanced.
		
            this.AddReward(new BaseReward(typeof(ChurchillsWarMace), 1078062));
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
                return this.Owner.Skills.Macing.Base < 50;
        }
		
        public override void OnCompleted()
        { 
            this.Owner.SendLocalizedMessage(1078068, null, 0x23); // You have achieved the rank of Apprentice Armsman. Return to Churchill in New Haven to claim your reward.
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
	
    public class Churchill : MondainQuester
    {
        public override Type[] Quests
        { 
            get
            {
                return new Type[] 
                {
                    typeof(CrushingBonesAndTakingNamesQuest)
                };
            }
        }
		
        [Constructable]
        public Churchill()
            : base("Churchill", "The Mace Fighting Instructor")
        { 
            this.SetSkill(SkillName.Anatomy, 120.0, 120.0);
            this.SetSkill(SkillName.Parry, 120.0, 120.0);
            this.SetSkill(SkillName.Healing, 120.0, 120.0);
            this.SetSkill(SkillName.Tactics, 120.0, 120.0);
            this.SetSkill(SkillName.Macing, 120.0, 120.0);
            this.SetSkill(SkillName.Focus, 120.0, 120.0);
        }
		
        public Churchill(Serial serial)
            : base(serial)
        {
        }
		
        public override void Advertise()
        {
            this.Say(1078141); // Don't listen to Jockles. Real warriors wield mace weapons!
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
            this.Direction = Direction.South;
			
            base.InitBody();
        }
		
        public override void InitOutfit()
        {
            this.AddItem(new Backpack());
            this.AddItem(new OrderShield());
            this.AddItem(new WarMace());
			
            Item item;
			
            item = new PlateLegs();
            item.Hue = 0x966;
            this.AddItem(item);
			
            item = new PlateGloves();
            item.Hue = 0x966;
            this.AddItem(item);
			
            item = new PlateGorget();
            item.Hue = 0x966;
            this.AddItem(item);
			
            item = new PlateChest();
            item.Hue = 0x966;
            this.AddItem(item);
			
            item = new PlateArms();
            item.Hue = 0x966;
            this.AddItem(item);		
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