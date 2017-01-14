using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests
{
    public class TheRightToolForTheJobQuest : BaseQuest
    { 
        public override bool DoneOnce
        {
            get
            {
                return true;
            }
        }
		
        /* The Right Tool for the Job */
        public override object Title
        {
            get
            {
                return 1077741;
            }
        }
		
        /* Create new scissors and hammers while inside Amelia's workshop. Try making scissors up to 45 skill, the switch 
        to making hammers until 50 skill.<br><center>-----</center><br>Hello! I guess you're here to learn something about 
        Tinkering, eh? You've come to the right place, as Tinkering is what I've dedicated my life to. <br><br>You'll need 
        two things to get started: a supply of ingots and the right tools for the job. You can either buy ingots from the 
        market, or go mine them yourself. As for tools, you can try making your own set of Tinker's Tools, or if you'd prefer 
        to buy them, I have some for sale.<br><br>Working here in my shop will let me give you pointers as you go, so you'll 
        be able to learn faster than anywhere else. Start off making scissors until you reach 45 tinkering skill, then switch 
        to hammers until you've achieved 50. Once you've done that, come talk to me and I'll give you something for your hard 
        work. */ 
        public override object Description
        {
            get
            {
                return 1077744;
            }
        }
		
        /* I’m disappointed that you aren’t interested in learning more about Tinkering. It’s really such a useful skill!<br><br>
        *Amelia smiles*<br><br>At least you know where to find me if you change your mind, since I rarely spend time outside 
        of this shop. */
        public override object Refuse
        {
            get
            {
                return 1077745;
            }
        }
		
        /* Nice going! You're not quite at Apprentice Tinkering yet, though, so you better get back to work. Remember that the 
        quickest way to learn is to make scissors up until 45 skill, and then switch to hammers. Also, don't forget that working 
        here in my shop will let me give you tips so you can learn faster. */
        public override object Uncomplete
        {
            get
            {
                return 1077746;
            }
        }
		
        /* You've done it! Look at our brand new Apprentice Tinker! You've still got quite a lot to learn if you want to be a 
        Grandmaster Tinker, but I believe you can do it! Just keep in mind that if you're tinkering just to practice and improve 
        your skill, make items that are moderately difficult (60-80% success chance), and try to stick to ones that use less 
        ingots.  <br><br>Come here, my brand new Apprentice Tinker, I want to give you something special. I created this just 
        for you, so I hope you like it. It's a set of Tinker's Tools that contains a bit of magic. These tools have more charges 
        than any Tinker's Tools a Tinker can make. You can even use them to make a normal set of tools, so that way you won't 
        ever find yourself stuck somewhere with no tools! */
        public override object Complete
        {
            get
            {
                return 1077748;
            }
        }
		
        public TheRightToolForTheJobQuest()
            : base()
        { 
            this.AddObjective(new ApprenticeObjective(SkillName.Tinkering, 50, "Springs And Things Workshop", 1077742, 1077743));
			
            // 1077742 By tinkering inside of Amelia’s workshop, she is able to give you advice. This helps you hone your Tinkering skill faster than normal.
            // 1077743 Since you’ve left Amelia’s workshop, she cannot give you advice. Your Tinkering learning potential is no longer enhanced.
			
            this.AddReward(new BaseReward(typeof(AmeliasToolbox), 1077749));
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
                return this.Owner.Skills.Tinkering.Base < 50;
        }
		
        public override void OnCompleted()
        { 
            this.Owner.SendLocalizedMessage(1077747, null, 0x23); // You have achieved the rank of Apprentice Tinker. Talk to Amelia Youngstone in New Haven to see what kind of reward she has waiting for you.
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
	
    public class Amelia : MondainQuester
    {
        public override Type[] Quests
        { 
            get
            {
                return new Type[] 
                {
                    typeof(TheRightToolForTheJobQuest)
                };
            }
        }
		
        public override void InitSBInfo()
        { 
            this.SBInfos.Add(new SBTinker(this));
        }
		
        [Constructable]
        public Amelia()
            : base("Amelia Youngstone", "The Tinkering Instructor")
        { 
            this.SetSkill(SkillName.ArmsLore, 120.0, 120.0);
            this.SetSkill(SkillName.Blacksmith, 120.0, 120.0);
            this.SetSkill(SkillName.Magery, 120.0, 120.0);
            this.SetSkill(SkillName.Tactics, 120.0, 120.0);
            this.SetSkill(SkillName.Swords, 120.0, 120.0);
            this.SetSkill(SkillName.Tinkering, 120.0, 120.0);
            this.SetSkill(SkillName.Mining, 120.0, 120.0);
        }
		
        public Amelia(Serial serial)
            : base(serial)
        {
        }
		
        public override void Advertise()
        {
            this.Say(1078123); // Tinkering is very useful for a blacksmith. You can make your own tools.
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
            this.AddItem(new Sandals());
            this.AddItem(new ShortPants());
            this.AddItem(new HalfApron(0x8AB));
            this.AddItem(new Doublet());
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