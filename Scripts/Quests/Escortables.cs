using System;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests
{
    public class EscortToYewQuest : BaseQuest
    { 
        public EscortToYewQuest()
            : base()
        { 
            this.AddObjective(new EscortObjective("Yew"));		  
            this.AddReward(new BaseReward(typeof(Gold), 500, 1062577)); 
        }

        /* An escort to Yew */
        public override object Title
        {
            get
            {
                return 1072275;
            }
        }
        /* I seek a worthy escort.  I can offer some small pay to any able bodied adventurer who can assist me.  
        * It is imperative that I reach my destination. */
        public override object Description
        {
            get
            {
                return 1072287;
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
        /* We have not yet arrived in Yew.  Let's keep going. */
        public override object Uncomplete
        {
            get
            {
                return 1072289;
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

    public class EscortToVesperQuest : BaseQuest
    { 
        public EscortToVesperQuest()
            : base()
        { 
            this.AddObjective(new EscortObjective("Vesper"));		  
            this.AddReward(new BaseReward(typeof(Gold), 500, 1062577)); 
        }

        /* An escort to Vesper */
        public override object Title
        {
            get
            {
                return 1072276;
            }
        }
        /* I seek a worthy escort.  I can offer some small pay to any able bodied adventurer who can assist me.  
        * It is imperative that I reach my destination. */
        public override object Description
        {
            get
            {
                return 1072287;
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
        /* We have not yet arrived in Vesper.  Let's keep going. */
        public override object Uncomplete
        {
            get
            {
                return 1072290;
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

    public class EscortToTrinsicQuest : BaseQuest
    { 
        public EscortToTrinsicQuest()
            : base()
        { 
            this.AddObjective(new EscortObjective("Trinsic"));		  
            this.AddReward(new BaseReward(typeof(Gold), 500, 1062577)); 
        }

        /* An escort to Trinsic */
        public override object Title
        {
            get
            {
                return 1072277;
            }
        }
        /* I seek a worthy escort.  I can offer some small pay to any able bodied adventurer who can assist me.  
        * It is imperative that I reach my destination. */
        public override object Description
        {
            get
            {
                return 1072287;
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
        /* We have not yet arrived in Trinsic.  Let's keep going. */
        public override object Uncomplete
        {
            get
            {
                return 1072291;
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

    public class EscortToSkaraQuest : BaseQuest
    { 
        public EscortToSkaraQuest()
            : base()
        { 
            this.AddObjective(new EscortObjective("Skara Brae"));		  
            this.AddReward(new BaseReward(typeof(Gold), 500, 1062577)); 
        }

        /* An escort to Skara */
        public override object Title
        {
            get
            {
                return 1072278;
            }
        }
        /* I seek a worthy escort.  I can offer some small pay to any able bodied adventurer who can assist me.  
        * It is imperative that I reach my destination. */
        public override object Description
        {
            get
            {
                return 1072287;
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
        /* We have not yet arrived in Skara.  Let's keep going. */
        public override object Uncomplete
        {
            get
            {
                return 1072292;
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

    public class EscortToSerpentsHoldQuest : BaseQuest
    { 
        public EscortToSerpentsHoldQuest()
            : base()
        { 
            this.AddObjective(new EscortObjective("Serpent's Hold"));		  
            this.AddReward(new BaseReward(typeof(Gold), 500, 1062577)); 
        }

        /* An escort to Serpent's Hold */
        public override object Title
        {
            get
            {
                return 1072279;
            }
        }
        /* I seek a worthy escort.  I can offer some small pay to any able bodied adventurer who can assist me.  
        * It is imperative that I reach my destination. */
        public override object Description
        {
            get
            {
                return 1072287;
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
        /* We have not yet arrived in Serpent's Hold.  Let's keep going. */
        public override object Uncomplete
        {
            get
            {
                return 1072293;
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

    public class EscortToNujelmQuest : BaseQuest
    { 
        public EscortToNujelmQuest()
            : base()
        { 
            this.AddObjective(new EscortObjective("Nujel'm"));		  
            this.AddReward(new BaseReward(typeof(Gold), 500, 1062577)); 
        }

        /* An escort to Nujel'm */
        public override object Title
        {
            get
            {
                return 1072280;
            }
        }
        /* I seek a worthy escort.  I can offer some small pay to any able bodied adventurer who can assist me.  
        * It is imperative that I reach my destination. */
        public override object Description
        {
            get
            {
                return 1072287;
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
        /* We have not yet arrived in Nujel'm.  Let's keep going. */
        public override object Uncomplete
        {
            get
            {
                return 1072294;
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

    public class EscortToMoonglowQuest : BaseQuest
    { 
        public EscortToMoonglowQuest()
            : base()
        { 
            this.AddObjective(new EscortObjective("Moonglow"));		  
            this.AddReward(new BaseReward(typeof(Gold), 500, 1062577)); 
        }

        /* An escort to Moonglow */
        public override object Title
        {
            get
            {
                return 1072281;
            }
        }
        /* I seek a worthy escort.  I can offer some small pay to any able bodied adventurer who can assist me.  
        * It is imperative that I reach my destination. */
        public override object Description
        {
            get
            {
                return 1072287;
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
        /* We have not yet arrived in Moonglow.  Let's keep going. */
        public override object Uncomplete
        {
            get
            {
                return 1072295;
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

    public class EscortToMinocQuest : BaseQuest
    { 
        public EscortToMinocQuest()
            : base()
        { 
            this.AddObjective(new EscortObjective("Minoc"));		  
            this.AddReward(new BaseReward(typeof(Gold), 500, 1062577)); 
        }

        /* An escort to Minoc */
        public override object Title
        {
            get
            {
                return 1072282;
            }
        }
        /* I seek a worthy escort.  I can offer some small pay to any able bodied adventurer who can assist me.  
        * It is imperative that I reach my destination. */
        public override object Description
        {
            get
            {
                return 1072287;
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
        /* We have not yet arrived in Minoc.  Let's keep going. */
        public override object Uncomplete
        {
            get
            {
                return 1072296;
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

    public class EscortToMaginciaQuest : BaseQuest
    { 
        public EscortToMaginciaQuest()
            : base()
        { 
            this.AddObjective(new EscortObjective("Magincia"));		  
            this.AddReward(new BaseReward(typeof(Gold), 500, 1062577)); 
        }

        /* An escort to Magincia */
        public override object Title
        {
            get
            {
                return 1072283;
            }
        }
        /* I seek a worthy escort.  I can offer some small pay to any able bodied adventurer who can assist me.  
        * It is imperative that I reach my destination. */
        public override object Description
        {
            get
            {
                return 1072287;
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
        /* We have not yet arrived in Magincia.  Let's keep going. */
        public override object Uncomplete
        {
            get
            {
                return 1072297;
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

    public class EscortToJhelomQuest : BaseQuest
    { 
        public EscortToJhelomQuest()
            : base()
        { 
            this.AddObjective(new EscortObjective("Jhelom"));		  
            this.AddReward(new BaseReward(typeof(Gold), 500, 1062577)); 
        }

        /* An escort to Jhelom */
        public override object Title
        {
            get
            {
                return 1072284;
            }
        }
        /* I seek a worthy escort.  I can offer some small pay to any able bodied adventurer who can assist me.  
        * It is imperative that I reach my destination. */
        public override object Description
        {
            get
            {
                return 1072287;
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
        /* We have not yet arrived in Jhelom.  Let's keep going. */
        public override object Uncomplete
        {
            get
            {
                return 1072298;
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

    public class EscortToCoveQuest : BaseQuest
    { 
        public EscortToCoveQuest()
            : base()
        { 
            this.AddObjective(new EscortObjective("Cove"));		  
            this.AddReward(new BaseReward(typeof(Gold), 500, 1062577)); 
        }

        /* An escort to Cove */
        public override object Title
        {
            get
            {
                return 1072285;
            }
        }
        /* I seek a worthy escort.  I can offer some small pay to any able bodied adventurer who can assist me.  
        * It is imperative that I reach my destination. */
        public override object Description
        {
            get
            {
                return 1072287;
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
        /* We have not yet arrived in Cove.  Let's keep going. */
        public override object Uncomplete
        {
            get
            {
                return 1072299;
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

    public class EscortToBritainQuest : BaseQuest
    { 
        public EscortToBritainQuest()
            : base()
        { 
            this.AddObjective(new EscortObjective("Britain"));		  
            this.AddReward(new BaseReward(typeof(Gold), 500, 1062577)); 
        }

        /* An escort to Britain */
        public override object Title
        {
            get
            {
                return 1072286;
            }
        }
        /* I seek a worthy escort.  I can offer some small pay to any able bodied adventurer who can assist me.  
        * It is imperative that I reach my destination. */
        public override object Description
        {
            get
            {
                return 1072287;
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
        /* We have not yet arrived in Britain.  Let's keep going. */
        public override object Uncomplete
        {
            get
            {
                return 1072300;
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

    public class TownEscortable : BaseEscort
    {
        private static readonly Type[] m_Quests = new Type[]
        {
            typeof(EscortToYewQuest),
            typeof(EscortToVesperQuest),
            typeof(EscortToTrinsicQuest),
            typeof(EscortToSkaraQuest),
            typeof(EscortToSerpentsHoldQuest),
            typeof(EscortToNujelmQuest),
            typeof(EscortToMoonglowQuest),
            typeof(EscortToMinocQuest),
            typeof(EscortToMaginciaQuest),
            typeof(EscortToJhelomQuest),
            typeof(EscortToCoveQuest),
            typeof(EscortToBritainQuest)
        };
        private static readonly string[] m_Destinations = new string[]
        {
            "Yew",
            "Vesper",
            "Trinsic",
            "Skara Brae",
            "Serpent's Hold",
            "Nujel'm",
            "Moonglow",
            "Minoc",
            "Magincia",
            "Jhelom",
            "Cove",
            "Britain"
        };
        private int m_Quest;
        public TownEscortable()
            : base()
        {
            this.m_Quest = Utility.Random(m_Quests.Length);
        }

        public TownEscortable(Serial serial)
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

    public class EscortableMerchant : TownEscortable
    {
        [Constructable]
        public EscortableMerchant()
        {
            this.Title = "the merchant";
            this.SetSkill(SkillName.ItemID, 55.0, 78.0);
            this.SetSkill(SkillName.ArmsLore, 55, 78);
        }

        public EscortableMerchant(Serial serial)
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

    public class EscortableMage : TownEscortable
    {
        [Constructable]
        public EscortableMage()
        {
            this.Title = "the mage";

            this.SetSkill(SkillName.EvalInt, 80.0, 100.0);
            this.SetSkill(SkillName.Inscribe, 80.0, 100.0);
            this.SetSkill(SkillName.Magery, 80.0, 100.0);
            this.SetSkill(SkillName.Meditation, 80.0, 100.0);
            this.SetSkill(SkillName.MagicResist, 80.0, 100.0);
        }

        public EscortableMage(Serial serial)
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

    public class EscortableMessenger : TownEscortable
    {
        [Constructable]
        public EscortableMessenger()
        {
            this.Title = "the messenger";
        }

        public EscortableMessenger(Serial serial)
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

    public class EscortableSeekerOfAdventure : TownEscortable
    {
        [Constructable]
        public EscortableSeekerOfAdventure()
        {
            this.Title = "the seeker of adventure";
        }

        public EscortableSeekerOfAdventure(Serial serial)
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

    public class EscortableNoble : TownEscortable
    {
        [Constructable]
        public EscortableNoble()
        {
            this.Title = "the noble";

            this.SetSkill(SkillName.Parry, 80.0, 100.0);
            this.SetSkill(SkillName.Swords, 80.0, 100.0);
            this.SetSkill(SkillName.Tactics, 80.0, 100.0);
        }

        public EscortableNoble(Serial serial)
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

    public class EscortableBrideGroom : TownEscortable
    {
        [Constructable]
        public EscortableBrideGroom()
        {
            if (this.Female)
                this.Title = "the bride";
            else
                this.Title = "the groom";	
        }

        public EscortableBrideGroom(Serial serial)
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

    public class EscortablePeasant : NewHavenEscortable
    {
        [Constructable]
        public EscortablePeasant()
        {
            this.Title = "the peasant";
        }

        public EscortablePeasant(Serial serial)
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

    public class EscortableHealer : TownEscortable
    {
        private static readonly TimeSpan ResurrectDelay = TimeSpan.FromSeconds(2.0);
        private DateTime m_NextResurrect;
        [Constructable]
        public EscortableHealer()
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

        public EscortableHealer(Serial serial)
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
            else if (m.Murderer)
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