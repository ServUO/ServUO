using System;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests
{
    public class NewHavenAlchemistEscortQuest : BaseQuest
    { 
        public NewHavenAlchemistEscortQuest()
            : base()
        { 
            this.AddObjective(new EscortObjective("the New Haven Alchemist"));		  
            this.AddReward(new BaseReward(typeof(Gold), 500, 1062577)); 
        }

        /* An escort to the New Haven Alchemist in The bottled Imp */
        public override object Title
        {
            get
            {
                return 1072314;
            }
        }
        /* I need some potions before I set out for a long journey. Can you take me to the alchemist in The Bottled Imp? */
        public override object Description
        {
            get
            {
                return 1042769;
            }
        }
        /* I wish you would reconsider my offer.  I'll be waiting right here for someone brave enough to assist me. */
        public override object Refuse
        {
            get
            {
                return 1072288;
            }
        }
        /* We have not yet arrived at the New Haven Alchemist in The Bottled Imp. Let's keep going. */
        public override object Uncomplete
        {
            get
            {
                return 1072326;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
			
            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
			
            int version = reader.ReadEncodedInt();
        }
    }

    public class NewHavenBardEscortQuest : BaseQuest
    { 
        public NewHavenBardEscortQuest()
            : base()
        { 
            this.AddObjective(new EscortObjective("the New Haven Bard"));		  
            this.AddReward(new BaseReward(typeof(Gold), 500, 1062577)); 
        }

        /* An escort to the New Haven Bard */
        public override object Title
        {
            get
            {
                return 1072315;
            }
        }
        /* I fear my talent for music is less than my desire to learn, yet still I would like to try. Can you take me 
        * to the local music shop? */
        public override object Description
        {
            get
            {
                return 1042772;
            }
        }
        /* I wish you would reconsider my offer.  I'll be waiting right here for someone brave enough to assist me. */
        public override object Refuse
        {
            get
            {
                return 1072288;
            }
        }
        /* We have not yet arrived at the New Haven Bard.  Let's keep going. */
        public override object Uncomplete
        {
            get
            {
                return 1072327;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
			
            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
			
            int version = reader.ReadEncodedInt();
        }
    }

    public class NewHavenWarriorEscortQuest : BaseQuest
    { 
        public NewHavenWarriorEscortQuest()
            : base()
        { 
            this.AddObjective(new EscortObjective("the New Haven Warrior"));		  
            this.AddReward(new BaseReward(typeof(Gold), 500, 1062577)); 
        }

        /* An escort to the New Haven Warrior */
        public override object Title
        {
            get
            {
                return 1072316;
            }
        }
        /* I need someone to help me rid my home of mongbats. Please take me to the local swordfighter. */
        public override object Description
        {
            get
            {
                return 1042787;
            }
        }
        /* I wish you would reconsider my offer.  I'll be waiting right here for someone brave enough to assist me. */
        public override object Refuse
        {
            get
            {
                return 1072288;
            }
        }
        /* We have not yet arrived at the New Haven Warrior.  Let's keep going. */
        public override object Uncomplete
        {
            get
            {
                return 1072328;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
			
            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
			
            int version = reader.ReadEncodedInt();
        }
    }

    public class NewHavenTailorEscortQuest : BaseQuest
    { 
        public NewHavenTailorEscortQuest()
            : base()
        { 
            this.AddObjective(new EscortObjective("the New Haven Tailor"));		  
            this.AddReward(new BaseReward(typeof(Gold), 500, 1062577)); 
        }

        /* An escort to the New Haven Tailor */
        public override object Title
        {
            get
            {
                return 1072317;
            }
        }
        /* I need new clothes for a party, and I was wondering if you could take me to the tailor? */
        public override object Description
        {
            get
            {
                return 1042781;
            }
        }
        /* I wish you would reconsider my offer.  I'll be waiting right here for someone brave enough to assist me. */
        public override object Refuse
        {
            get
            {
                return 1072288;
            }
        }
        /* We have not yet arrived at the New Haven Tailor.  Let's keep going. */
        public override object Uncomplete
        {
            get
            {
                return 1072329;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
			
            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
			
            int version = reader.ReadEncodedInt();
        }
    }

    public class NewHavenCarpenterEscortQuest : BaseQuest
    { 
        public NewHavenCarpenterEscortQuest()
            : base()
        { 
            this.AddObjective(new EscortObjective("the New Haven Carpenter"));		  
            this.AddReward(new BaseReward(typeof(Gold), 500, 1062577)); 
        }

        /* An escort to the New Haven Carpenter */
        public override object Title
        {
            get
            {
                return 1072318;
            }
        }
        /* I need a hammer and nails. Never mind for what. Take me to the local carpenter or leave me be. */
        public override object Description
        {
            get
            {
                return 1042775;
            }
        }
        /* I wish you would reconsider my offer.  I'll be waiting right here for someone brave enough to assist me. */
        public override object Refuse
        {
            get
            {
                return 1072288;
            }
        }
        /* We have not yet arrived at the New Haven Carpenter.  Let's keep going. */
        public override object Uncomplete
        {
            get
            {
                return 1072330;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
			
            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
			
            int version = reader.ReadEncodedInt();
        }
    }

    public class NewHavenMapmakerEscortQuest : BaseQuest
    { 
        public NewHavenMapmakerEscortQuest()
            : base()
        { 
            this.AddObjective(new EscortObjective("the New Haven Mapmaker"));		  
            this.AddReward(new BaseReward(typeof(Gold), 500, 1062577)); 
        }

        /* An escort to the New Haven Mapmaker */
        public override object Title
        {
            get
            {
                return 1072319;
            }
        }
        /* Where am I? Who am I? Do you know me? Hmmm - on second thought, I think I best stick with where I am first. 
        * Do you know where I can get a map? */
        public override object Description
        {
            get
            {
                return 1042793;
            }
        }
        /* I wish you would reconsider my offer.  I'll be waiting right here for someone brave enough to assist me. */
        public override object Refuse
        {
            get
            {
                return 1072288;
            }
        }
        /* We have not yet arrived at the New Haven Mapmaker.  Let's keep going. */
        public override object Uncomplete
        {
            get
            {
                return 1072331;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
			
            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
			
            int version = reader.ReadEncodedInt();
        }
    }

    public class NewHavenMageEscortQuest : BaseQuest
    { 
        public NewHavenMageEscortQuest()
            : base()
        { 
            this.AddObjective(new EscortObjective("the New Haven Mage"));		  
            this.AddReward(new BaseReward(typeof(Gold), 500, 1062577)); 
        }

        /* An escort to the New Haven Mage */
        public override object Title
        {
            get
            {
                return 1072320;
            }
        }
        /* You there. Take me to see a sorcerer so I can turn a friend back in to a human. He is currently a cat 
        * and keeps demanding milk. */
        public override object Description
        {
            get
            {
                return 1042790;
            }
        }
        /* I wish you would reconsider my offer.  I'll be waiting right here for someone brave enough to assist me. */
        public override object Refuse
        {
            get
            {
                return 1072288;
            }
        }
        /* We have not yet arrived at the New Haven Mage.  Let's keep going. */
        public override object Uncomplete
        {
            get
            {
                return 1072332;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
			
            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
			
            int version = reader.ReadEncodedInt();
        }
    }

    public class NewHavenInnEscortQuest : BaseQuest
    { 
        public NewHavenInnEscortQuest()
            : base()
        { 
            this.AddObjective(new EscortObjective("the New Haven Inn"));		  
            this.AddReward(new BaseReward(typeof(Gold), 500, 1062577)); 
        }

        /* An escort to the New Haven Inn */
        public override object Title
        {
            get
            {
                return 1072321;
            }
        }
        /* I need something to eat. I am starving. Can you take me to the inn? */
        public override object Description
        {
            get
            {
                return 1042796;
            }
        }
        /* I wish you would reconsider my offer.  I'll be waiting right here for someone brave enough to assist me. */
        public override object Refuse
        {
            get
            {
                return 1072288;
            }
        }
        /* We have not yet arrived at the New Haven Inn.  Let's keep going. */
        public override object Uncomplete
        {
            get
            {
                return 1072333;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
			
            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
			
            int version = reader.ReadEncodedInt();
        }
    }

    public class NewHavenFarmEscortQuest : BaseQuest
    { 
        public NewHavenFarmEscortQuest()
            : base()
        { 
            this.AddObjective(new EscortObjective("the New Haven Farm"));		  
            this.AddReward(new BaseReward(typeof(Gold), 500, 1062577)); 
        }

        /* An escort to the New Haven Farm */
        public override object Title
        {
            get
            {
                return 1072322;
            }
        }
        /* I am hoping to swap soil stories with a local farmer, but I cannot find one. Can you take me to one? */
        public override object Description
        {
            get
            {
                return 1042799;
            }
        }
        /* I wish you would reconsider my offer.  I'll be waiting right here for someone brave enough to assist me. */
        public override object Refuse
        {
            get
            {
                return 1072288;
            }
        }
        /* We have not yet arrived at the New Haven Farm.  Let's keep going. */
        public override object Uncomplete
        {
            get
            {
                return 1072334;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
			
            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
			
            int version = reader.ReadEncodedInt();
        }
    }

    public class NewHavenDocksEscortQuest : BaseQuest
    { 
        public NewHavenDocksEscortQuest()
            : base()
        { 
            this.AddObjective(new EscortObjective("the New Haven Docks"));		  
            this.AddReward(new BaseReward(typeof(Gold), 500, 1062577)); 
        }

        /* An escort to the New Haven Docks */
        public override object Title
        {
            get
            {
                return 1072323;
            }
        }
        /* I have heard of a magical fish that grants wishes. I bet THAT fisherman knows where the fish is. Please take me to him. */
        public override object Description
        {
            get
            {
                return 1042802;
            }
        }
        /* I wish you would reconsider my offer.  I'll be waiting right here for someone brave enough to assist me. */
        public override object Refuse
        {
            get
            {
                return 1072288;
            }
        }
        /* We have not yet arrived at the New Haven Docks.  Let's keep going. */
        public override object Uncomplete
        {
            get
            {
                return 1072335;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
			
            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
			
            int version = reader.ReadEncodedInt();
        }
    }

    public class NewHavenBowyerEscortQuest : BaseQuest
    { 
        public NewHavenBowyerEscortQuest()
            : base()
        { 
            this.AddObjective(new EscortObjective("the New Haven Bowyer"));		  
            this.AddReward(new BaseReward(typeof(Gold), 500, 1062577)); 
        }

        /* An escort to the New Haven Bowyer */
        public override object Title
        {
            get
            {
                return 1072324;
            }
        }
        /* You there. Do you know the way to the local archer? */
        public override object Description
        {
            get
            {
                return 1042805;
            }
        }
        /* I wish you would reconsider my offer.  I'll be waiting right here for someone brave enough to assist me. */
        public override object Refuse
        {
            get
            {
                return 1072288;
            }
        }
        /* We have not yet arrived at the New Haven Bowyer.  Let's keep going. */
        public override object Uncomplete
        {
            get
            {
                return 1072336;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
			
            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
			
            int version = reader.ReadEncodedInt();
        }
    }

    public class NewHavenBankEscortQuest : BaseQuest
    { 
        public NewHavenBankEscortQuest()
            : base()
        { 
            this.AddObjective(new EscortObjective("the New Haven Bank"));		  
            this.AddReward(new BaseReward(typeof(Gold), 500, 1062577)); 
        }

        /* An escort to the New Haven Bank */
        public override object Title
        {
            get
            {
                return 1072325;
            }
        }
        /* I have a debt I need to pay off at the bank. Do you know the way there? */
        public override object Description
        {
            get
            {
                return 1042784;
            }
        }
        /* I wish you would reconsider my offer.  I'll be waiting right here for someone brave enough to assist me. */
        public override object Refuse
        {
            get
            {
                return 1072288;
            }
        }
        /* We have not yet arrived at the New Haven Bank.  Let's keep going. */
        public override object Uncomplete
        {
            get
            {
                return 1072337;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
			
            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
			
            int version = reader.ReadEncodedInt();
        }
    }

    public class NewHavenEscortable : BaseEscort
    {
        private static readonly Type[] m_Quests = new Type[]
        {
            typeof(NewHavenAlchemistEscortQuest),
            typeof(NewHavenBardEscortQuest),
            typeof(NewHavenWarriorEscortQuest),
            typeof(NewHavenTailorEscortQuest),
            typeof(NewHavenCarpenterEscortQuest),
            typeof(NewHavenMapmakerEscortQuest),
            typeof(NewHavenMageEscortQuest),
            typeof(NewHavenInnEscortQuest),
            typeof(NewHavenFarmEscortQuest),
            typeof(NewHavenDocksEscortQuest),
            typeof(NewHavenBowyerEscortQuest),
            typeof(NewHavenBankEscortQuest)
        };
        private static readonly string[] m_Destinations = new string[]
        {
            "the New Haven Alchemist",
            "the New Haven Bard",
            "the New Haven Warrior",
            "the New Haven Tailor",
            "the New Haven Carpenter",
            "the New Haven Mapmaker",
            "the New Haven Mage",
            "the New Haven Inn",
            "the New Haven Farm",
            "the New Haven Docks",
            "the New Haven Bowyer",
            "the New Haven Bank"
        };
        private int m_Quest;
        public NewHavenEscortable()
            : base()
        { 
        }

        public NewHavenEscortable(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        {
            get
            {
                return new Type[] { m_Quests[this.m_Quest] };
            }
        }
        public override void Advertise()
        {
            this.Say(Utility.RandomMinMax(1072301, 1072303));
        }

        public override Region GetDestination()
        {
            return QuestHelper.FindRegion(m_Destinations[this.m_Quest]);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version

            writer.Write(this.m_Quest);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            this.m_Quest = reader.ReadInt();
        }
    }

    public class NewHavenMerchant : NewHavenEscortable
    {
        [Constructable]
        public NewHavenMerchant()
        {
            this.Title = "the merchant";
            this.SetSkill(SkillName.ItemID, 55.0, 78.0);
            this.SetSkill(SkillName.ArmsLore, 55, 78);
        }

        public NewHavenMerchant(Serial serial)
            : base(serial)
        {
        }

        public override bool CanTeach
        {
            get
            {
                return true;
            }
        }
        public override bool ClickTitle
        {
            get
            {
                return false;
            }
        }
        public override void InitOutfit()
        {
            if (this.Female)
                this.AddItem(new PlainDress());
            else
                this.AddItem(new Shirt(this.GetRandomHue()));

            int lowHue = this.GetRandomHue();

            this.AddItem(new ThighBoots());

            if (this.Female)
                this.AddItem(new FancyDress(lowHue));
            else
                this.AddItem(new FancyShirt(lowHue));
            this.AddItem(new LongPants(lowHue));

            if (!this.Female)
                this.AddItem(new BodySash(lowHue));

            this.PackGold(200, 250);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class NewHavenMage : NewHavenEscortable
    {
        [Constructable]
        public NewHavenMage()
        {
            this.Title = "the mage";

            this.SetSkill(SkillName.EvalInt, 80.0, 100.0);
            this.SetSkill(SkillName.Inscribe, 80.0, 100.0);
            this.SetSkill(SkillName.Magery, 80.0, 100.0);
            this.SetSkill(SkillName.Meditation, 80.0, 100.0);
            this.SetSkill(SkillName.MagicResist, 80.0, 100.0);
        }

        public NewHavenMage(Serial serial)
            : base(serial)
        {
        }

        public override bool CanTeach
        {
            get
            {
                return true;
            }
        }
        public override bool ClickTitle
        {
            get
            {
                return false;
            }
        }
        public override void InitOutfit()
        {
            this.AddItem(new Robe(this.GetRandomHue()));

            int lowHue = this.GetRandomHue();

            this.AddItem(new ShortPants(lowHue));

            if (this.Female)
                this.AddItem(new ThighBoots(lowHue));
            else
                this.AddItem(new Boots(lowHue));

            this.PackGold(200, 250);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class NewHavenMessenger : NewHavenEscortable
    {
        [Constructable]
        public NewHavenMessenger()
        {
            this.Title = "the messenger";
        }

        public NewHavenMessenger(Serial serial)
            : base(serial)
        {
        }

        public override bool ClickTitle
        {
            get
            {
                return false;
            }
        }
        public override void InitOutfit()
        {
            if (this.Female)
                this.AddItem(new PlainDress());
            else
                this.AddItem(new Shirt(this.GetRandomHue()));

            int lowHue = this.GetRandomHue();

            this.AddItem(new ShortPants(lowHue));

            if (this.Female)
                this.AddItem(new Boots(lowHue));
            else
                this.AddItem(new Shoes(lowHue));

            switch ( Utility.Random(4) )
            {
                case 0:
                    this.AddItem(new ShortHair(Utility.RandomHairHue()));
                    break;
                case 1:
                    this.AddItem(new TwoPigTails(Utility.RandomHairHue()));
                    break;
                case 2:
                    this.AddItem(new ReceedingHair(Utility.RandomHairHue()));
                    break;
                case 3:
                    this.AddItem(new KrisnaHair(Utility.RandomHairHue()));
                    break;
            }

            this.PackGold(200, 250);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class NewHavenSeekerOfAdventure : NewHavenEscortable
    {
        [Constructable]
        public NewHavenSeekerOfAdventure()
        {
            this.Title = "the seeker of adventure";
        }

        public NewHavenSeekerOfAdventure(Serial serial)
            : base(serial)
        {
        }

        public override bool ClickTitle
        {
            get
            {
                return false;
            }
        }
        public override void InitOutfit()
        {
            if (this.Female)
                this.AddItem(new FancyDress(this.GetRandomHue()));
            else
                this.AddItem(new FancyShirt(this.GetRandomHue()));

            int lowHue = this.GetRandomHue();

            this.AddItem(new ShortPants(lowHue));

            if (this.Female)
                this.AddItem(new ThighBoots(lowHue));
            else
                this.AddItem(new Boots(lowHue));

            if (!this.Female)
                this.AddItem(new BodySash(lowHue));

            this.AddItem(new Cloak(this.GetRandomHue()));

            this.AddItem(new Longsword());

            this.PackGold(100, 150);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class NewHavenNoble : NewHavenEscortable
    {
        [Constructable]
        public NewHavenNoble()
        {
            this.Title = "the noble";

            this.SetSkill(SkillName.Parry, 80.0, 100.0);
            this.SetSkill(SkillName.Swords, 80.0, 100.0);
            this.SetSkill(SkillName.Tactics, 80.0, 100.0);
        }

        public NewHavenNoble(Serial serial)
            : base(serial)
        {
        }

        public override bool CanTeach
        {
            get
            {
                return true;
            }
        }
        public override bool ClickTitle
        {
            get
            {
                return false;
            }
        }
        public override void InitOutfit()
        {
            if (this.Female)
                this.AddItem(new FancyDress());
            else
                this.AddItem(new FancyShirt(this.GetRandomHue()));

            int lowHue = this.GetRandomHue();

            this.AddItem(new ShortPants(lowHue));

            if (this.Female)
                this.AddItem(new ThighBoots(lowHue));
            else
                this.AddItem(new Boots(lowHue));

            if (!this.Female)
                this.AddItem(new BodySash(lowHue));

            this.AddItem(new Cloak(this.GetRandomHue()));

            if (!this.Female)
                this.AddItem(new Longsword());

            this.PackGold(200, 250);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class NewHavenBrideGroom : NewHavenEscortable
    {
        [Constructable]
        public NewHavenBrideGroom()
        {
            if (this.Female)
                this.Title = "the bride";
            else
                this.Title = "the groom";
        }

        public NewHavenBrideGroom(Serial serial)
            : base(serial)
        {
        }

        public override bool ClickTitle
        {
            get
            {
                return false;
            }
        }
        public override void InitOutfit()
        {
            if (this.Female)
                this.AddItem(new FancyDress());
            else
                this.AddItem(new FancyShirt());

            int lowHue = this.GetRandomHue();

            this.AddItem(new LongPants(lowHue));

            if (this.Female)
                this.AddItem(new Shoes(lowHue));
            else
                this.AddItem(new Boots(lowHue));

            if (Utility.RandomBool())
                this.HairItemID = 0x203B;
            else
                this.HairItemID = 0x203C;

            this.HairHue = this.Race.RandomHairHue();

            this.PackGold(200, 250);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class NewHavenPeasant : NewHavenEscortable
    {
        [Constructable]
        public NewHavenPeasant()
        {
            this.Title = "the peasant";
        }

        public NewHavenPeasant(Serial serial)
            : base(serial)
        {
        }

        public override bool ClickTitle
        {
            get
            {
                return false;
            }
        }
        public override void InitOutfit()
        {
            if (this.Female)
                this.AddItem(new PlainDress());
            else
                this.AddItem(new Shirt(this.GetRandomHue()));

            int lowHue = this.GetRandomHue();

            this.AddItem(new ShortPants(lowHue));

            if (this.Female)
                this.AddItem(new Boots(lowHue));
            else
                this.AddItem(new Shoes(lowHue));

            Utility.AssignRandomHair(this);

            this.PackGold(200, 250);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class NewHavenHealer : NewHavenEscortable
    {
        private static readonly TimeSpan ResurrectDelay = TimeSpan.FromSeconds(2.0);
        private DateTime m_NextResurrect;
        [Constructable]
        public NewHavenHealer()
            : base()
        {
            this.Title = "the wandering healer";

            this.AI = AIType.AI_Mage;
            this.ActiveSpeed = 0.2;
            this.PassiveSpeed = 0.8;
            this.RangePerception = BaseCreature.DefaultRangePerception;
            this.FightMode = FightMode.Aggressor;

            this.SpeechHue = 0;

            this.SetStr(304, 400);
            this.SetDex(102, 150);
            this.SetInt(204, 300);

            this.SetDamage(10, 23);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 40, 50);
            this.SetResistance(ResistanceType.Fire, 40, 50);
            this.SetResistance(ResistanceType.Cold, 40, 50);
            this.SetResistance(ResistanceType.Poison, 40, 50);
            this.SetResistance(ResistanceType.Energy, 40, 50);

            this.SetSkill(SkillName.Anatomy, 75.0, 97.5);
            this.SetSkill(SkillName.EvalInt, 82.0, 100.0);
            this.SetSkill(SkillName.Healing, 75.0, 97.5);
            this.SetSkill(SkillName.Magery, 82.0, 100.0);
            this.SetSkill(SkillName.MagicResist, 82.0, 100.0);
            this.SetSkill(SkillName.Tactics, 82.0, 100.0);
            this.SetSkill(SkillName.Camping, 80.0, 100.0);
            this.SetSkill(SkillName.Forensics, 80.0, 100.0);
            this.SetSkill(SkillName.SpiritSpeak, 80.0, 100.0);

            this.Fame = 1000;
            this.Karma = 10000;

            this.PackItem(new Bandage(Utility.RandomMinMax(5, 10)));
            this.PackItem(new HealPotion());
            this.PackItem(new CurePotion());
        }

        public NewHavenHealer(Serial serial)
            : base(serial)
        {
        }

        public override bool ClickTitle
        {
            get
            {
                return false;
            }
        }
        public override bool CanTeach
        {
            get
            {
                return true;
            }
        }
        public virtual bool HealsYoungPlayers
        {
            get
            {
                return true;
            }
        }
        public override bool CheckTeach(SkillName skill, Mobile from)
        {
            if (!base.CheckTeach(skill, from))
                return false;

            return (skill == SkillName.Anatomy) ||
                   (skill == SkillName.Camping) ||
                   (skill == SkillName.Forensics) ||
                   (skill == SkillName.Healing) ||
                   (skill == SkillName.SpiritSpeak);
        }

        public override void InitOutfit()
        {
            this.AddItem(new Sandals(this.GetShoeHue()));
            this.AddItem(new Robe(Utility.RandomYellowHue()));
            this.AddItem(new GnarledStaff());
        }

        public virtual bool CheckResurrect(Mobile m)
        {
            if (m.Criminal)
            {
                this.Say(501222); // Thou art a criminal.  I shall not resurrect thee.
                return false;
            }
            else if (m.Kills >= 5)
            {
                this.Say(501223); // Thou'rt not a decent and good person. I shall not resurrect thee.
                return false;
            }

            return true;
        }

        public virtual void OfferResurrection(Mobile m)
        {
            this.Direction = this.GetDirectionTo(m);
            this.Say(501224); // Thou hast strayed from the path of virtue, but thou still deservest a second chance.

            m.PlaySound(0x214);
            m.FixedEffect(0x376A, 10, 16);

            m.CloseGump(typeof(ResurrectGump));
            m.SendGump(new ResurrectGump(m, ResurrectMessage.Healer));
        }

        public virtual void OfferHeal(PlayerMobile m)
        {
            this.Direction = this.GetDirectionTo(m);

            if (m.CheckYoungHealTime())
            {
                this.Say(501229); // You look like you need some healing my child.

                m.PlaySound(0x1F2);
                m.FixedEffect(0x376A, 9, 32);

                m.Hits = m.HitsMax;
            }
            else
            {
                this.Say(501228); // I can do no more for you at this time.
            }
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (!m.Frozen && DateTime.UtcNow >= this.m_NextResurrect && this.InRange(m, 4) && !this.InRange(oldLocation, 4) && this.InLOS(m))
            {
                if (!m.Alive)
                {
                    this.m_NextResurrect = DateTime.UtcNow + ResurrectDelay;

                    if (m.Map == null || !m.Map.CanFit(m.Location, 16, false, false))
                    {
                        m.SendLocalizedMessage(502391); // Thou can not be resurrected there!
                    }
                    else if (this.CheckResurrect(m))
                    {
                        this.OfferResurrection(m);
                    }
                }
                else if (this.HealsYoungPlayers && m.Hits < m.HitsMax && m is PlayerMobile && ((PlayerMobile)m).Young)
                {
                    this.OfferHeal((PlayerMobile)m);
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}