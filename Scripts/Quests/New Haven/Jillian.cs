using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests
{
    public class ScribingArcaneKnowledgeQuest : BaseQuest
    { 
        public override bool DoneOnce
        {
            get
            {
                return true;
            }
        }
		
        /* Scribing Arcane Knowledge */
        public override object Title
        {
            get
            {
                return 1077615;
            }
        }
		
        /* While Here ar the New Haven Magery Library, use scribe's pen and scribe 3rd and 4th circle Magery scrolls that 
        you have in your spellbook. Remeber, you will need blank scrolls aswell. Do this until you have raised your Inscription 
        skill to 50. Greetings and welcome to the New Haven Magery Library! You wish to learn how to scribe spell scrolls? You 
        have come to the right place! Inscribeed in a steady hand and imbued with te power of reagents, a scroll can mean the 
        difference between life and death in a perilous situation. Those knowledgeable in Inscription man transcribe spells to 
        create useful and valuale magical scrolls. Before you can inscribe a spell, you must first be able to cast the spell 
        without the aid of a scroll. This means that you need the appropriate level of proficiency as a mage, the required mana, 
        and the required reagents. Second, you will need a blank scroll to write on and a scribe's pen. Then, you will need to 
        decide which particular spell you wish to scribe. It may sound easy, but there is a bit more to it. As with the development 
        of all skills, you need to practice Inscription of lower level spells before you can move onto the more difficult ones. The 
        most important aspect of Inscription is mana. Inscribing a scroll with a magic spell drains your mana. When inscribing 3rd 
        or lower spells this is will not be much of a problem for these spells consume a small amount of mana. However, when you are 
        inscribing higher circle spells, you may see your mana drain rapidly. When this happens, pause or meditate before continuing.
        I suggest you begin scribing any 3rd and 4th circle spells that you know. If you don't possess ant, you can alwayers barter 
        with one of the local mage merchants or a fellow adventurer that is a seasoned Scribe. Come back to me once you feel that 
        you are the worthy rankof Apprentice Scribe and i will reward you with an arcane prize. */ 
        public override object Description
        {
            get
            {
                return 1077616;
            }
        }
		
        /* I understand. When you are ready, feel free to return to me for Inscription training. Thanks for stopping by! */
        public override object Refuse
        {
            get
            {
                return 1077617;
            }
        }
		
        /* You have not achived the rank of Apprentice Scribe. Come back to me once you feel that you are worthy of the rank 
        Apprentice Scribe and i will reward you with a arcane prize. */
        public override object Uncomplete
        {
            get
            {
                return 1077631;
            }
        }
		
        /* Scribing is a very fulfilling pursuit. I am please to see you embark on this journey. You sling a pen well! On behalf 
        of the New Haven Mage Council I wish to present you with this spellbook. When equipped, the Hallowed Spellbook greatly 
        enhanced the potency of your offensive soells when used against Undead. Be mindful, though. While this book is equiped 
        you invoke powerful spells and abilities vs Humanoids, such as other humans, orcs, ettins, and trolls. Your offensive 
        spells will diminish in effectiveness. I suggest unequipping the Hallowed Spellbook when battling Humanoids. I hope this 
        spellbook serves you well. */
        public override object Complete
        {
            get
            {
                return 1077619;
            }
        }
		
        public ScribingArcaneKnowledgeQuest()
            : base()
        { 
            this.AddObjective(new ApprenticeObjective(SkillName.Inscribe, 50, "Haven Library", 1077493, 1077587));
			
            // 1077493 Your Inscription potential is greatly enhanced while questing in this area.
            // 1077587 You are not in the quest area for Apprentice Scribe. Your Inscription potential is not enhanced here.
		  
            this.AddReward(new BaseReward(typeof(HallowedSpellbook), 1077620));
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
                return this.Owner.Skills.Inscribe.Base < 50;
        }
		
        public override void OnCompleted()
        { 
            this.Owner.SendLocalizedMessage(1077618, null, 0x23); // You have achieved the rank of Apprentice Scribe. Return to Jillian in New Haven to receive your arcane prize.
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
	
    public class Jillian : MondainQuester
    {
        public override Type[] Quests
        { 
            get
            {
                return new Type[] 
                {
                    typeof(ScribingArcaneKnowledgeQuest)
                };
            }
        }
		
        [Constructable]
        public Jillian()
            : base("Jillian", "The Inscription Instructor")
        { 
            this.SetSkill(SkillName.EvalInt, 120.0, 120.0);
            this.SetSkill(SkillName.Inscribe, 120.0, 120.0);
            this.SetSkill(SkillName.Magery, 120.0, 120.0);
            this.SetSkill(SkillName.MagicResist, 120.0, 120.0);
            this.SetSkill(SkillName.Wrestling, 120.0, 120.0);
            this.SetSkill(SkillName.Meditation, 120.0, 120.0);
        }
		
        public Jillian(Serial serial)
            : base(serial)
        {
        }
		
        public override void Advertise()
        {
            this.Say(1078129); // I can teach you how to scribe magic scrolls.
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
            this.AddItem(new Robe(0x479));
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