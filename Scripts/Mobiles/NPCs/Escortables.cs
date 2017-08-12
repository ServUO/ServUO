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
            AddObjective(new EscortObjective("the New Haven Alchemist"));		  
            AddReward(new BaseReward(typeof(Gold), 500, 1062577)); 
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
            AddObjective(new EscortObjective("the New Haven Bard"));		  
            AddReward(new BaseReward(typeof(Gold), 500, 1062577)); 
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
            AddObjective(new EscortObjective("the New Haven Warrior"));		  
            AddReward(new BaseReward(typeof(Gold), 500, 1062577)); 
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
            AddObjective(new EscortObjective("the New Haven Tailor"));		  
            AddReward(new BaseReward(typeof(Gold), 500, 1062577)); 
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
            AddObjective(new EscortObjective("the New Haven Carpenter"));		  
            AddReward(new BaseReward(typeof(Gold), 500, 1062577)); 
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
            AddObjective(new EscortObjective("the New Haven Mapmaker"));		  
            AddReward(new BaseReward(typeof(Gold), 500, 1062577)); 
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
            AddObjective(new EscortObjective("the New Haven Mage"));		  
            AddReward(new BaseReward(typeof(Gold), 500, 1062577)); 
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
            AddObjective(new EscortObjective("the New Haven Inn"));		  
            AddReward(new BaseReward(typeof(Gold), 500, 1062577)); 
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
            AddObjective(new EscortObjective("the New Haven Farm"));		  
            AddReward(new BaseReward(typeof(Gold), 500, 1062577)); 
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
            AddObjective(new EscortObjective("the New Haven Docks"));		  
            AddReward(new BaseReward(typeof(Gold), 500, 1062577)); 
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
            AddObjective(new EscortObjective("the New Haven Bowyer"));		  
            AddReward(new BaseReward(typeof(Gold), 500, 1062577)); 
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
            AddObjective(new EscortObjective("the New Haven Bank"));		  
            AddReward(new BaseReward(typeof(Gold), 500, 1062577)); 
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
            : this(Utility.Random(12))
        { 
        }

        public NewHavenEscortable(int quest)
        {
            m_Quest = quest;
        }

        public NewHavenEscortable(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        {
            get
            {
                return new Type[] { m_Quests[m_Quest] };
            }
        }
        public override void Advertise()
        {
            Say(Utility.RandomMinMax(1072301, 1072303));
        }

        public override Region GetDestination()
        {
            return QuestHelper.FindRegion(m_Destinations[m_Quest]);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version

            writer.Write(m_Quest);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            m_Quest = reader.ReadInt();
        }
    }

    public class NewHavenMerchant : NewHavenEscortable
    {
        [Constructable]
        public NewHavenMerchant()
        {
            Title = "the merchant";
            SetSkill(SkillName.ItemID, 55.0, 78.0);
            SetSkill(SkillName.ArmsLore, 55, 78);
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
            if (Female)
                AddItem(new PlainDress());
            else
                AddItem(new Shirt(GetRandomHue()));

            int lowHue = GetRandomHue();

            AddItem(new ThighBoots());

            if (Female)
                AddItem(new FancyDress(lowHue));
            else
                AddItem(new FancyShirt(lowHue));
            AddItem(new LongPants(lowHue));

            if (!Female)
                AddItem(new BodySash(lowHue));

            PackGold(200, 250);
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
            Title = "the mage";

            SetSkill(SkillName.EvalInt, 80.0, 100.0);
            SetSkill(SkillName.Inscribe, 80.0, 100.0);
            SetSkill(SkillName.Magery, 80.0, 100.0);
            SetSkill(SkillName.Meditation, 80.0, 100.0);
            SetSkill(SkillName.MagicResist, 80.0, 100.0);
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
            AddItem(new Robe(GetRandomHue()));

            int lowHue = GetRandomHue();

            AddItem(new ShortPants(lowHue));

            if (Female)
                AddItem(new ThighBoots(lowHue));
            else
                AddItem(new Boots(lowHue));

            PackGold(200, 250);
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
            Title = "the messenger";
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
            if (Female)
                AddItem(new PlainDress());
            else
                AddItem(new Shirt(GetRandomHue()));

            int lowHue = GetRandomHue();

            AddItem(new ShortPants(lowHue));

            if (Female)
                AddItem(new Boots(lowHue));
            else
                AddItem(new Shoes(lowHue));

            switch ( Utility.Random(4) )
            {
                case 0:
                    AddItem(new ShortHair(Utility.RandomHairHue()));
                    break;
                case 1:
                    AddItem(new TwoPigTails(Utility.RandomHairHue()));
                    break;
                case 2:
                    AddItem(new ReceedingHair(Utility.RandomHairHue()));
                    break;
                case 3:
                    AddItem(new KrisnaHair(Utility.RandomHairHue()));
                    break;
            }

            PackGold(200, 250);
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
            Title = "the seeker of adventure";
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
            if (Female)
                AddItem(new FancyDress(GetRandomHue()));
            else
                AddItem(new FancyShirt(GetRandomHue()));

            int lowHue = GetRandomHue();

            AddItem(new ShortPants(lowHue));

            if (Female)
                AddItem(new ThighBoots(lowHue));
            else
                AddItem(new Boots(lowHue));

            if (!Female)
                AddItem(new BodySash(lowHue));

            AddItem(new Cloak(GetRandomHue()));

            AddItem(new Longsword());

            PackGold(100, 150);
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
            Title = "the noble";

            SetSkill(SkillName.Parry, 80.0, 100.0);
            SetSkill(SkillName.Swords, 80.0, 100.0);
            SetSkill(SkillName.Tactics, 80.0, 100.0);
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
            if (Female)
                AddItem(new FancyDress());
            else
                AddItem(new FancyShirt(GetRandomHue()));

            int lowHue = GetRandomHue();

            AddItem(new ShortPants(lowHue));

            if (Female)
                AddItem(new ThighBoots(lowHue));
            else
                AddItem(new Boots(lowHue));

            if (!Female)
                AddItem(new BodySash(lowHue));

            AddItem(new Cloak(GetRandomHue()));

            if (!Female)
                AddItem(new Longsword());

            PackGold(200, 250);
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
            if (Female)
                Title = "the bride";
            else
                Title = "the groom";
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
            if (Female)
                AddItem(new FancyDress());
            else
                AddItem(new FancyShirt());

            int lowHue = GetRandomHue();

            AddItem(new LongPants(lowHue));

            if (Female)
                AddItem(new Shoes(lowHue));
            else
                AddItem(new Boots(lowHue));

            if (Utility.RandomBool())
                HairItemID = 0x203B;
            else
                HairItemID = 0x203C;

            HairHue = Race.RandomHairHue();

            PackGold(200, 250);
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
            Title = "the peasant";
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
            if (Female)
                AddItem(new PlainDress());
            else
                AddItem(new Shirt(GetRandomHue()));

            int lowHue = GetRandomHue();

            AddItem(new ShortPants(lowHue));

            if (Female)
                AddItem(new Boots(lowHue));
            else
                AddItem(new Shoes(lowHue));

            Utility.AssignRandomHair(this);

            PackGold(200, 250);
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
            Title = "the wandering healer";

            AI = AIType.AI_Mage;
            ActiveSpeed = 0.2;
            PassiveSpeed = 0.8;
            RangePerception = BaseCreature.DefaultRangePerception;
            FightMode = FightMode.Aggressor;

            SpeechHue = 0;

            SetStr(304, 400);
            SetDex(102, 150);
            SetInt(204, 300);

            SetDamage(10, 23);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 40, 50);
            SetResistance(ResistanceType.Fire, 40, 50);
            SetResistance(ResistanceType.Cold, 40, 50);
            SetResistance(ResistanceType.Poison, 40, 50);
            SetResistance(ResistanceType.Energy, 40, 50);

            SetSkill(SkillName.Anatomy, 75.0, 97.5);
            SetSkill(SkillName.EvalInt, 82.0, 100.0);
            SetSkill(SkillName.Healing, 75.0, 97.5);
            SetSkill(SkillName.Magery, 82.0, 100.0);
            SetSkill(SkillName.MagicResist, 82.0, 100.0);
            SetSkill(SkillName.Tactics, 82.0, 100.0);
            SetSkill(SkillName.Camping, 80.0, 100.0);
            SetSkill(SkillName.Forensics, 80.0, 100.0);
            SetSkill(SkillName.SpiritSpeak, 80.0, 100.0);

            Fame = 1000;
            Karma = 10000;

            PackItem(new Bandage(Utility.RandomMinMax(5, 10)));
            PackItem(new HealPotion());
            PackItem(new CurePotion());
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
            AddItem(new Sandals(GetShoeHue()));
            AddItem(new Robe(Utility.RandomYellowHue()));
            AddItem(new GnarledStaff());
        }

        public virtual bool CheckResurrect(Mobile m)
        {
            if (m.Criminal)
            {
                Say(501222); // Thou art a criminal.  I shall not resurrect thee.
                return false;
            }
            else if (m.Murderer)
            {
                Say(501223); // Thou'rt not a decent and good person. I shall not resurrect thee.
                return false;
            }

            return true;
        }

        public virtual void OfferResurrection(Mobile m)
        {
            Direction = GetDirectionTo(m);
            Say(501224); // Thou hast strayed from the path of virtue, but thou still deservest a second chance.

            m.PlaySound(0x214);
            m.FixedEffect(0x376A, 10, 16);

            m.CloseGump(typeof(ResurrectGump));
            m.SendGump(new ResurrectGump(m, ResurrectMessage.Healer));
        }

        public virtual void OfferHeal(PlayerMobile m)
        {
            Direction = GetDirectionTo(m);

            if (m.CheckYoungHealTime())
            {
                Say(501229); // You look like you need some healing my child.

                m.PlaySound(0x1F2);
                m.FixedEffect(0x376A, 9, 32);

                m.Hits = m.HitsMax;
            }
            else
            {
                Say(501228); // I can do no more for you at this time.
            }
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (!m.Frozen && DateTime.UtcNow >= m_NextResurrect && InRange(m, 4) && !InRange(oldLocation, 4) && InLOS(m))
            {
                if (!m.Alive)
                {
                    m_NextResurrect = DateTime.UtcNow + ResurrectDelay;

                    if (m.Map == null || !m.Map.CanFit(m.Location, 16, false, false))
                    {
                        m.SendLocalizedMessage(502391); // Thou can not be resurrected there!
                    }
                    else if (CheckResurrect(m))
                    {
                        OfferResurrection(m);
                    }
                }
                else if (HealsYoungPlayers && m.Hits < m.HitsMax && m is PlayerMobile && ((PlayerMobile)m).Young)
                {
                    OfferHeal((PlayerMobile)m);
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