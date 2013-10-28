using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests
{
    public class DefyingTheArcaneQuest : BaseQuest
    { 
        public override bool DoneOnce
        {
            get
            {
                return true;
            }
        }
		
        /* Defying the Arcane */
        public override object Title
        {
            get
            {
                return 1077621;
            }
        }
		
        /* Head East out of town and go to Old Haven. Battle spell casting monsters there until you have raised your 
        Resisting Spells skill to 50.<br><center>------</center><br>Hail and well met! To become a true master of 
        the arcane art of Magery, I suggest learning the complementary skill known as Resisting Spells. While the 
        name of this skill may suggest that it helps with resisting all spells, this is not the case. This skill 
        helps you lessen the severity of spells that lower your stats or ones that last for a specific duration of 
        time. It does not lessen damage from spells such as Energy Bolt or Flamestrike.<BR><BR>The Magery spells that 
        can be resisted are Clumsy, Curse, Feeblemind, Mana Drain, Mana Vampire, Paralyze, Paralyze Field, Poison, 
        Poison Field, and Weaken.<BR><BR>The Necromancy spells that can be resisted are Blood Oath, Corpse Skin, Mind 
        Rot, and Pain Spike.<BR><BR>At higher ranks, the Resisting Spells skill also benefits you by adding a bonus to 
        your minimum elemental resists. This bonus is only applied after all other resist modifications - such as from 
        equipment - has been calculated. It's also not cumulative. It compares the number of your minimum resists to 
        the calculated value of your modifications and uses the higher of the two values.<BR><BR>As you can see, 
        Resisting Spells is a difficult skill to understand, and even more difficult to master. This is because in 
        order to improve it, you will have to put yourself in harm's way - as in the path of one of the above spells.
        <BR><BR>Undead have plagued the town of Old Haven. We need your assistance in cleansing the town of this evil 
        influence. Old Haven is located east of here. Battle the undead spell casters that inhabit there.<BR><BR>Come
        back to me once you feel that you are worthy of the rank of Apprentice Mage and I will reward you with an 
        arcane prize. */ 
        public override object Description
        {
            get
            {
                return 1077623;
            }
        }
		
        /* The ability to resist powerful spells is a taxing experience. I understand your resistance in wanting to 
        pursue it. If you wish to reconsider, feel free to return to me for Resisting Spells training. Good journey 
        to you! */
        public override object Refuse
        {
            get
            {
                return 1077624;
            }
        }
		
        /* You have not achieved the rank of Apprentice Mage. Come back to me once you feel that you are worthy of the 
        rank of Apprentice Mage and I will reward you with an arcane prize. */
        public override object Uncomplete
        {
            get
            {
                return 1077632;
            }
        }
		
        /* You have successfully begun your journey in becoming a true master of Magery. On behalf of the New Haven Mage 
        Council I wish to present you with this bracelet. When worn, the Bracelet of Resilience will enhance your resistances 
        vs. the elements, physical, and poison harm. The Bracelet of Resilience also magically enhances your ability fend 
        off ranged and melee attacks. I hope it serves you well.*/
        public override object Complete
        {
            get
            {
                return 1077626;
            }
        }
		
        public DefyingTheArcaneQuest()
            : base()
        { 
            this.AddObjective(new ApprenticeObjective(SkillName.MagicResist, 50, "Old Haven Training", 1077494, 1077588));
			
            // 1077494 Your Resisting Spells potential is greatly enhanced while questing in this area.
            // 1077588 You are not in the quest area for Apprentice Mage. Your Resisting Spells potential is not enhanced here.
		
            this.AddReward(new BaseReward(typeof(BraceletOfResilience), 1077627));
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
                return this.Owner.Skills.MagicResist.Base < 50;
        }
		
        public override void OnCompleted()
        { 
            this.Owner.SendLocalizedMessage(1077625, null, 0x23); // You have achieved the rank of Apprentice Mage (for Resisting Spells). Return to Alefian in New Haven to receive your arcane prize.
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
	
    public class Alefian : MondainQuester
    {
        public override Type[] Quests
        { 
            get
            {
                return new Type[] 
                {
                    typeof(DefyingTheArcaneQuest)
                };
            }
        }
		
        [Constructable]
        public Alefian()
            : base("Alefian", "The Resisting Spells Instructor")
        { 
            this.SetSkill(SkillName.EvalInt, 120.0, 120.0);
            this.SetSkill(SkillName.Inscribe, 120.0, 120.0);
            this.SetSkill(SkillName.Magery, 120.0, 120.0);
            this.SetSkill(SkillName.MagicResist, 120.0, 120.0);
            this.SetSkill(SkillName.Wrestling, 120.0, 120.0);
            this.SetSkill(SkillName.Meditation, 120.0, 120.0);
        }
		
        public Alefian(Serial serial)
            : base(serial)
        {
        }
		
        public override void Advertise()
        {
            this.Say(1078130); // A mage should learn how to resist spells.
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
            this.AddItem(new Robe());
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