using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests
{
    public class TheDeluciansLostMineQuest : BaseQuest
    { 
        public override bool DoneOnce
        {
            get
            {
                return true;
            }
        }
		
        /* The Delucian’s Lost Mine */
        public override object Title
        {
            get
            {
                return 1077750;
            }
        }
		
        /* Find Jacob's Lost Mine and mine iron ore there, using a pickaxe or shovel. Bring it back to Jacob's forge and 
        smelt the ore into ingots, until you have raised your Mining skill to 50. You may find a packhorse useful for 
        hauling the ore around. The animal trainer in New Haven has packhorses for sale.<br><center>-----</center><br>Howdy! 
        Welcome to my camp. It's not much, I know, but it's all I'll be needin' up here. I don't need them fancy things those 
        townspeople have down there in New Haven. Nope, not one bit. Just me, Bessie, my pick and a thick vein 'o valorite.
        <br><br>Anyhows, I'm guessin' that you're up here to ask me about minin', aren't ya? Well, don't be expectin' me to 
        tell you where the valorite's at, cause I ain't gonna tell the King of Britannia, much less the likes of you. But I 
        will show ya how to mine and smelt iron, cause there certainly is a 'nough of up in these hills.<br><br>*Jacob looks 
        around, with a perplexed look on his face*<br><br>Problem is, I can't remember where my iron mine's at, so you'll 
        have to find it yourself. Once you're there, have at it with a pickaxe or shovel, then haul it back to camp and I'll 
        show ya how to smelt it. Ya look a bit wimpy, so you might wanna go buy yourself a packhorse in town from the animal 
        trainer to help you haul around all that ore.<br><br>When you're an Apprentice Miner, talk to me and I'll give ya a 
        little somethin' I've got layin' around here... somewhere. */ 
        public override object Description
        {
            get
            {
                return 1077753;
            }
        }
		
        /* Couldn’t find my iron mine, could ya? Well, neither can I!<br><br>*Jacob laughs*<br><br>Oh, ya don’t wanna find it? 
        Well, allrighty then, ya might as well head on back down to town then and stop cluttering up my camp. Come back and 
        talk to me if you’re interested in learnin’ ‘bout minin’. */
        public override object Refuse
        {
            get
            {
                return 1077754;
            }
        }
		
        /* Where ya been off a gallivantin’ all day, pilgrim? You ain’t seen no hard work yet! Get yer arse back out there to 
        my mine and dig up some more iron. Don’t forget to take a pickaxe or shovel, and if you’re so inclined, a packhorse too. */
        public override object Uncomplete
        {
            get
            {
                return 1077755;
            }
        }
		
        /* Dang gun it! If that don't beat all! Ya went and did it, didn’t ya? What we got ourselves here is a mighty fine brand 
        spankin’ new Apprentice Miner!<br><br>I can see ya put some meat on them bones too while you were at it!<br><br>Here’s 
        that little somethin’ I told ya I had for ya. It’s a pickaxe with some high falutin’ magic inside that’ll help you find 
        the good stuff when you’re off minin’. It wears out fast, though, so you can only use it a few times a day.<br><br>Welp, 
        I’ve got some smeltin’ to do, so off with ya. Good luck, pilgrim! */
        public override object Complete
        {
            get
            {
                return 1077757;
            }
        }
		
        public TheDeluciansLostMineQuest()
            : base()
        { 
            this.AddObjective(new ApprenticeObjective(SkillName.Mining, 50, "Haven Mountains", 1077751, 1077752));
			
            // 1077751 You can almost smell the ore in the rocks here! Your ability to improve your Mining skill is enhanced in this area.
            // 1077752 So many rocks, so little ore… Your potential to increase your Mining skill is no longer enhanced.
		
            this.AddReward(new BaseReward(typeof(JacobsPickaxe), 1077758));
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
                return this.Owner.Skills.Mining.Base < 50;
        }
		
        public override void OnCompleted()
        { 
            this.Owner.SendLocalizedMessage(1077756, null, 0x23); // You have achieved the rank of Apprentice Miner. Return to Jacob Waltz in at his camp in the hills above New Haven as soon as you can to claim your reward.
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
	
    public class Jacob : MondainQuester
    {
        public override Type[] Quests
        { 
            get
            {
                return new Type[] 
                {
                    typeof(TheDeluciansLostMineQuest)
                };
            }
        }
		
        [Constructable]
        public Jacob()
            : base("Jacob Waltzt", "The Miner Instructor")
        { 
            this.SetSkill(SkillName.ArmsLore, 120.0, 120.0);
            this.SetSkill(SkillName.Blacksmith, 120.0, 120.0);
            this.SetSkill(SkillName.Magery, 120.0, 120.0);
            this.SetSkill(SkillName.Tactics, 120.0, 120.0);
            this.SetSkill(SkillName.Tinkering, 120.0, 120.0);
            this.SetSkill(SkillName.Swords, 120.0, 120.0);
            this.SetSkill(SkillName.Mining, 120.0, 120.0);
        }
		
        public Jacob(Serial serial)
            : base(serial)
        {
        }
		
        public override void Advertise()
        {
            this.Say(1078124); // You there! I can use some help mining these rocks!
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
            this.AddItem(new Pickaxe());
            this.AddItem(new Boots());
            this.AddItem(new ShortPants(0x370));
            this.AddItem(new Shirt(0x966));
            this.AddItem(new WideBrimHat(0x966));
            this.AddItem(new HalfApron(0x1BB));
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