using Server;
using System;
using Server.Mobiles;
using Server.Items;

namespace Server.Engines.Quests
{
    public class ThievesBeAfootQuest : BaseQuest
    {
        public override object Title { get { return 1094958; } }        // Thieves Be Afoot!

        public override object Description { get { return 1094960; } }  /*Travel into the Underworld and search for the stolen
                                                                         * barrels of barley.  Return them to Quartermaster Flint
                                                                         * for your reward.<br><center>-----</center><br>Hail 
                                                                         * Traveler.  Trying to find the fabled Stygian Abyss are 
                                                                         * ye?  I say good luck, an' be weary for I believe there 
                                                                         * to be a den o' thieves hidden in them halls!  Aye, just
                                                                         * last night I lost four barrels o' Barley.  I know they
                                                                         * be sayin' that none but critters live in them halls, but
                                                                         * I've looked every place I dare and seen no sign o' me
                                                                         * barrels.<br><br>Hope them lazy Society folk got a good 
                                                                         * nap last night, cause they wan have any o' me fine Barley
                                                                         * based products unless we get those barrels back!  
                                                                         * I canne believe none of them loafers who was guarding 
                                                                         * the door saw nothin!  Oy, it makes me so mad, I must not
                                                                         * think of it and control me temper... It's a frickin' 
                                                                         * barrel of Barley!  How could they miss seeing it???
                                                                         * <br><br>Sorry... I don' mean ta be takin' it out on ye.
                                                                         * Tell you what friend.  You find those barrels and I will
                                                                         * pay you for bringin' them back.  There be some nasty 
                                                                         * stuff in thar, so if'n ye bring back all four, I have 
                                                                         * somethin' special I will share with ye!*/

        public override object Refuse { get { return 1094961; } }       /*Fine then, be on yar way but be warned!  Ol' Flint makes
                                                                         * the best refreshin' barley products in tha known world! 
                                                                         * That barley will not profit ye if'n ol' Flint ha' not
                                                                         * prepared it proper!*/

        public override object Uncomplete { get { return 1094962; } }   /*What?  Back so soon and narry a barrel in sight?  Be y
                                                                         * e advised that ye are not the only traveler ol' Flint 
                                                                         * has a lookin fer his barrels!  If'n ye are gonna profit
                                                                         * of me potions, ye best be about it!*/

        public override object Complete { get { return 1094965; } }     /*Ah, thar she is!  That's me barrel all right, I knew
                                                                         * someone had taken it in thar!  Goblins you say? Oy,
                                                                         * they be a nasty bit o' business, ain't they?  Well, a 
                                                                         * deal's a deal, here is yar potion as promised!*/

        public override QuestChain ChainID { get { return QuestChain.FlintTheQuartermaster; } }
        public override Type NextQuest { get { return typeof(BibliophileQuest); } }

        public ThievesBeAfootQuest()
        {
            AddObjective(new ObtainObjective(typeof(BarrelOfBarley), "Barrel of Barley", 4, 4014));
            AddReward(new BaseReward(typeof(BottleOfFlintsPungnentBrew), 1094967));
        }

        public override void OnObjectiveUpdate(Item item)
        {
            Owner.SendLocalizedMessage(1094964); // This barrel fits the description Flint gave you.  Boy is it heavy!
        }

        public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0);
		}

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();
        }
    }

    public class BibliophileQuest : BaseQuest
    {
        public override object Title { get { return 1094968; } }        // Bibliophile

        public override object Description { get { return 1094970; } }  /*Travel into the Underworld and find the Flint's stolen
                                                                         * log book.  Return to Flint with the Log book for your 
                                                                         * reward.<br><center>-----</center><br>Ye will not be 
                                                                         * believin' what misfortune has befallen me now!  No sooner
                                                                         * have I gotten me Barley back, those goblins have gone 
                                                                         * and taken me log book!  How in the two dimensions am I 
                                                                         * supposted to keep up with all of tha supplies with no 
                                                                         * log book?  Of course, those lay about guards dinna see 
                                                                         * anything!<br><br>Listen, ye 've been a blessin' to ol' 
                                                                         * Flint in tha past so I wanta make ye another offer.  
                                                                         * If'n ye will bring ol' Flint's book back ta 'im, I will
                                                                         * give ye a keg o' me special brew!*/

        public override object Refuse { get { return 1094971; } }       /*'Tis a fine thing to do to a friend in need!  But so be it.
                                                                         * 'Tis not the first time today ol' Flint has been let 
                                                                         * down. Won't be the last.*/

        public override object Uncomplete { get { return 1094972; } }   /*'Ave ye laid hold to ol' Flint's log book yet?  Oh, I'm sorry, 
                                                                         * I jes' figured ye would have it back by now... Carry on then!*/

        public override object Complete { get { return 1094975; } }     /*'Ave ye laid hold to ol' Flint's log book did ye?  Let me
                                                                         * 'ave a look here!  Bloomin' savages!  They dog eared one
                                                                         * o' the pages and bent the corner o' me cover! Blast! 
                                                                         * Well, that's not fer you to be worryin' about.  Here be
                                                                         * yer pay as promised a keg of me brew far yer own.  This 
                                                                         * keg is special made by me own design, ye can use it to 
                                                                         * refill that bottle I gave ye.  My brew is too strong for
                                                                         * normal bottles, so I hope ye listened to ol' Flint and 
                                                                         * kept that bottle!*/

        public override QuestChain ChainID { get { return QuestChain.FlintTheQuartermaster; } }

        public BibliophileQuest()
        {
            AddObjective(new ObtainObjective(typeof(FlintsLogbook), "Flint's Logbook", 1, 7185));
            AddReward(new BaseReward(typeof(KegOfFlintsPungnentBrew), 1113608));
        }

        public override void OnObjectiveUpdate(Item item)
        {
            Owner.SendLocalizedMessage(1094974); // This appears to be Flint's logbook.  It is not clear why the goblins were using it in a ritual.  Perhaps they were summoning a nefarious intention?
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();
        }
    }

    public class QuartermasterFlint : MondainQuester
    {
        [Constructable]
        public QuartermasterFlint()
            : base("Quartermaster Flint", "")
        {
        }

        public QuartermasterFlint(Serial serial)
            : base(serial)
        {
        }

        public override void Advertise()
        {
            Say(Utility.RandomBool() ? 1094959 : 1094969); // Keep an eye pealed, traveler, thieves be afoot!
        }

        public override Type[] Quests
        {
            get
            {
                return new Type[] 
                {
                    typeof(ThievesBeAfootQuest)
                };
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);

            this.Female = false;
            this.Race = Race.Human;

            this.Hue = 0x8418;
            this.HairItemID = 0x2046;
            this.HairHue = 0x466;
        }

        public override void InitOutfit()
        {
            this.AddItem(new Backpack());
            this.AddItem(new Shoes(0x743));
            this.AddItem(new LongPants());
            this.AddItem(new FancyShirt());
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