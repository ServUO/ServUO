using System;
using Server.Items;

namespace Server.Engines.Quests
{
    public class FriendsOfTheLibraryQuest : BaseQuest
    { 
        public FriendsOfTheLibraryQuest()
            : base()
        { 
            this.AddObjective(new DeliverObjective(typeof(LibraryApplication), "friends of the library application", 1, typeof(Sarakki), "Sarakki (Britain)"));		
							
            this.AddReward(new BaseReward(1072749)); // Friends of the Library Membership Token.
        }

        public override QuestChain ChainID
        {
            get
            {
                return QuestChain.LibraryFriends;
            }
        }
        public override Type NextQuest
        {
            get
            {
                return typeof(BureaucraticDelayQuest);
            }
        }
        /* Friends of the Library */
        public override object Title
        {
            get
            {
                return 1072716;
            }
        }
        /* Shhh!  *angry whisper*  This is a library, not some bawdy house!  If you wish to become a friend of the library 
        you'll learn to moderate your volume.  And, of course, you'll take this application and have it notarized by Sarakki 
        the Notary.  Until you've become an official friend of the library, I can't allow you to make donations.  It wouldn't 
        be proper. */
        public override object Description
        {
            get
            {
                return 1072720;
            }
        }
        /* Hrmph! */
        public override object Refuse
        {
            get
            {
                return 1072721;
            }
        }
        /* *glare*  Shhhh!  You need to visit the notary.  She can be found near the castle. */
        public override object Uncomplete
        {
            get
            {
                return 1072722;
            }
        }
        /* Greetings! */
        public override object Complete
        {
            get
            {
                return 1073985;
            }
        }
        public override bool CanOffer()
        {
            return MondainsLegacy.PublicDonations;
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

    public class BureaucraticDelayQuest : BaseQuest
    { 
        public BureaucraticDelayQuest()
            : base()
        { 
            this.AddObjective(new DeliverObjective(typeof(SealingWaxOrder), "sealing wax order", 1, typeof(Petrus), "Petrus (Ilshenar)"));		
							
            this.AddReward(new BaseReward(1074871)); // A step closer to having sealing wax.
        }

        public override QuestChain ChainID
        {
            get
            {
                return QuestChain.LibraryFriends;
            }
        }
        public override Type NextQuest
        {
            get
            {
                return typeof(TheSecretIngredientQuest);
            }
        }
        /* Bureaucratic Delay */
        public override object Title
        {
            get
            {
                return 1072717;
            }
        }
        /* What do you have there?  Oh.  *look of dismay*  It seems everyone is interested in helping the library -- but 
        no one warned me to stock up on sealing wax.  I'm afraid I'm out of the mixture we use to notarize offical documents.  
        There will be a delay ... unless you'd like to take matters into your own hands and retrieve more for me from Petrus? */
        public override object Description
        {
            get
            {
                return 1072724;
            }
        }
        /* I do apologize for being unprepared. Perhaps when you return later I'll have more wax in stock. */
        public override object Refuse
        {
            get
            {
                return 1072725;
            }
        }
        /* Petrus lives in Ilshenar, past the Compassion gate and beyond the gypsy camp. */
        public override object Uncomplete
        {
            get
            {
                return 1072726;
            }
        }
        /* Hello, hello. */
        public override object Complete
        {
            get
            {
                return 1073986;
            }
        }
        public override bool CanOffer()
        {
            return MondainsLegacy.PublicDonations;
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

    public class TheSecretIngredientQuest : BaseQuest
    { 
        public TheSecretIngredientQuest()
            : base()
        { 
            this.AddObjective(new ObtainObjective(typeof(SpeckledPoisonSac), "speckled poison sacs", 5, 3600));		
							
            this.AddReward(new BaseReward(1074871)); // A step closer to having sealing wax.
        }

        public override QuestChain ChainID
        {
            get
            {
                return QuestChain.LibraryFriends;
            }
        }
        public override Type NextQuest
        {
            get
            {
                return typeof(SpecialDeliveryQuest);
            }
        }
        /* The Secret Ingredient */
        public override object Title
        {
            get
            {
                return 1072718;
            }
        }
        /* What's this?  Sealing wax ... Sarakki ... official documents ... Oh, I see.  Can do, can do.  You will need to get me the poison 
        sacs of course.  They are so volatile they aren't viable after an hour or so ... yes, yes.  Right, well off you go, speckled scorpions 
        are the only critters that have the right poison.  They live in the desert near here. */
        public override object Description
        {
            get
            {
                return 1072727;
            }
        }
        /* I see. I see.  Well good luck to you. */
        public override object Refuse
        {
            get
            {
                return 1072728;
            }
        }
        /* Hello, hello!  Speckled, that's what they are.  All covered with spots and speckles when you look very closely.  Speckled scorpion 
        poison sacs will do the trick. */
        public override object Uncomplete
        {
            get
            {
                return 1072729;
            }
        }
        /* Fine, fine.  Do you have them? */
        public override object Complete
        {
            get
            {
                return 1073987;
            }
        }
        public override bool CanOffer()
        {
            return MondainsLegacy.PublicDonations;
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

    public class SpecialDeliveryQuest : BaseQuest
    { 
        public SpecialDeliveryQuest()
            : base()
        { 
            this.AddObjective(new DeliverObjective(typeof(OfficialSealingWax), "sealing wax", 1, typeof(Sarakki), "Sarakki (Britain)"));		
							
            this.AddReward(new BaseReward(1072749)); // Friends of the Library Membership Token.
        }

        public override QuestChain ChainID
        {
            get
            {
                return QuestChain.LibraryFriends;
            }
        }
        public override Type NextQuest
        {
            get
            {
                return typeof(AccessToTheStacksQuest);
            }
        }
        /* Special Delivery */
        public override object Title
        {
            get
            {
                return 1072719;
            }
        }
        /* Good good!  The wax is just cooling now and will be ready by the time you get it back to Sarakki.  You still want the wax, right? */
        public override object Description
        {
            get
            {
                return 1072741;
            }
        }
        /* Sorry, sorry.  I'll hold onto your order in case you change your mind. */
        public override object Refuse
        {
            get
            {
                return 1072746;
            }
        }
        /* Hello, hello.  No rush of course.  I'm sure Sarakki is patient and doesn't mind waiting for the wax. */
        public override object Uncomplete
        {
            get
            {
                return 1072747;
            }
        }
        /* Oh welcome back!  Do you have my wax? */
        public override object Complete
        {
            get
            {
                return 1073988;
            }
        }
        public override bool CanOffer()
        {
            return MondainsLegacy.PublicDonations;
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

    public class AccessToTheStacksQuest : BaseQuest
    { 
        public AccessToTheStacksQuest()
            : base()
        { 
            this.AddObjective(new DeliverObjective(typeof(NotarizedApplication), "notarized application", 1, typeof(Verity), "Verity (Britain)"));		
							
            this.AddReward(new BaseReward(typeof(FriendOfTheLibraryToken), 1072749)); // Friends of the Library Membership Token.
        }

        public override QuestChain ChainID
        {
            get
            {
                return QuestChain.LibraryFriends;
            }
        }
        /* Access to the Stacks */
        public override object Title
        {
            get
            {
                return 1072723;
            }
        }
        /* There you are!  *pleased smile*  Don't you just love when a form is all filled in like that?  All of the sections are complete, 
        everything is neat and tidy and the official seal is perfectly formed and in exactly the right spot.  It's comforting.  Here you are.  
        All you need to do now is return the form to Librarian Verity.  Have a nice day! */
        public override object Description
        {
            get
            {
                return 1072748;
            }
        }
        /* Oh dear!  You've changed your mind?  *looks flustered*  I'll file your notarized application then, in case you decide at a future 
        date to become a friend of the library. */
        public override object Refuse
        {
            get
            {
                return 1072750;
            }
        }
        /* The librarian can always be found in the Library.  *admiring tone*  She's got a really strong work ethic. */
        public override object Uncomplete
        {
            get
            {
                return 1072751;
            }
        }
        /* As an official friend of the library you can make contributions at a donation area. */
        public override object Complete
        {
            get
            {
                return 1074811;
            }
        }
        public override bool CanOffer()
        {
            return MondainsLegacy.PublicDonations;
        }

        public override void GiveRewards()
        {
            base.GiveRewards();
			
            this.Owner.LibraryFriend = true;
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