using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests
{
    public class StoppingTheWorldQuest : BaseQuest
    { 
        public override bool DoneOnce
        {
            get
            {
                return true;
            }
        }
		
        /* Stopping the World */
        public override object Title
        {
            get
            {
                return 1077597;
            }
        }
		
        /* Head East out of town and go to Old Haven. Use spells and abilites to deplete your mana and meditate there until you have 
        raised your Meditation skill to 50.	Well met! I can teach you how to 'Stop the World' around you and focus your inner energies 
        on replenishing you mana. What is mana? Mana is the life force for everyone who practices arcane arts. When a practitioner 
        of magic invokes a spell or scribes a scroll. It consumes mana. Having a abundant supply of mana is vital to excelling as a 
        practitioner of the arcane. Those of us who study the art of Meditation are also known as stotics. The Meditation skill allows 
        stoics to increase the rate at which they regenerate mana A Stoic needs to perform abilities or cast spells to deplete mana 
        before he can meditate to replenish it. Meditation can occur passively or actively. Actively Meditation is more difficult to 
        master but allows for the stoic to replenish mana at a significantly faster rate. Metal armor inerferes with the regenerative 
        properties of Meditation. It is wise to wear leather or cloth protection when meditating. Head east out of town and go to Old 
        Haven. Use spells and abilities to deplete your mana and actively meditate to replenish it.	Come back once you feel you are at 
        the worthy rank of Apprentice Stoic and i will reward you with a arcane prize. */ 
        public override object Description
        {
            get
            {
                return 1077598;
            }
        }
		
        /* Seek me out if you ever wish to study the art of Meditation. Good journey. */
        public override object Refuse
        {
            get
            {
                return 1077599;
            }
        }
		
        /* You have not achived the rank of Apprentice Stoic. Come back to me once you feel that you are worthy of the rank Apprentice 
        Stoic and i will reward you with a arcane prize. */
        public override object Uncomplete
        {
            get
            {
                return 1077601;
            }
        }
		
        /* Splendid! On behalf of the New Haven Mage Council i wish to present you with this hat. When worn the Philosopher's Hat will 
        protect you somewhat from physical attacks. The Philosopher's Hat also enhances the potency of your offensive spells, lowers 
        the mana cost of your arcane soekks abd abilities, and passively increases your mana regeneration rate. Ah yes almost forgot. 
        The philosopher's Hat also grants one other special ability to its wearer. It allows a chance for the wearer to cast spells 
        without using any reagents. I hope the Philosopher's Hate serves you well. */
        public override object Complete
        {
            get
            {
                return 1077601;
            }
        }
		
        public StoppingTheWorldQuest()
            : base()
        { 
            this.AddObjective(new ApprenticeObjective(SkillName.Meditation, 50, "Old Haven Training", 1077490, 1077584));
			
            // 1077490 Your Meditation potential is greatly enhanced while questing in this area.
            // 1077584 You are not in the quest area for Apprentice Stoic. Your Meditation potential is not enhanced here.
		  
            this.AddReward(new BaseReward(typeof(PhilosophersHat), 1077602));
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
                return this.Owner.Skills.Meditation.Base < 50;
        }
		
        public override void OnCompleted()
        { 
            this.Owner.SendLocalizedMessage(1077600, null, 0x23); // You have achieved the rank of Apprentice Stoic (for Meditation). Return to Gustar in New Haven to receive your arcane prize.
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
	
    public class Gustar : MondainQuester
    {
        public override Type[] Quests
        { 
            get
            {
                return new Type[] 
                {
                    typeof(StoppingTheWorldQuest)
                };
            }
        }
		
        [Constructable]
        public Gustar()
            : base("Gustar", "The Meditation Instructor")
        { 
            this.SetSkill(SkillName.EvalInt, 120.0, 120.0);
            this.SetSkill(SkillName.Inscribe, 120.0, 120.0);
            this.SetSkill(SkillName.Magery, 120.0, 120.0);
            this.SetSkill(SkillName.MagicResist, 120.0, 120.0);
            this.SetSkill(SkillName.Wrestling, 120.0, 120.0);
            this.SetSkill(SkillName.Meditation, 120.0, 120.0);
        }
		
        public Gustar(Serial serial)
            : base(serial)
        {
        }
		
        public override void Advertise()
        {
            this.Say(1078126); // Meditation allows a mage to replenish mana quickly. I can teach you.
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
            this.AddItem(new HoodedShroudOfShadows(0x479));
            this.AddItem(new Sandals());
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