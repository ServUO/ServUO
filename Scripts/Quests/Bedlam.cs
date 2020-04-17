using Server.Items;
using Server.Mobiles;
using System;

namespace Server.Engines.Quests
{
    public class MistakenIdentityQuest : BaseQuest
    {
        public MistakenIdentityQuest()
            : base()
        {
            AddObjective(new DeliverObjective(typeof(TuitionReimbursementForm), "tuition reimbursement form", 1, typeof(Gorrow), "Gorrow (Luna)"));

            AddReward(new BaseReward(1074634)); // Tuition Reimbursement
        }

        public override QuestChain ChainID => QuestChain.TuitionReimbursement;
        public override Type NextQuest => typeof(YouScratchMyBackQuest);
        /* Mistaken Identity */
        public override object Title => 1074573;
        /* What do you want?  Wonderful, another whining request for a refund on tuition.  You know, experiences 
        like that are invaluable ... and infrequent.  Having the opportunity to test yourself under such realistic 
        situations isn't something the college offers all students.  Fine. Fine.  You'll need to submit a refund 
        request form in triplicate before I can return your 1,000,000 gold tuition.  You'll need to get some 
        signatures and a few other odds and ends. */
        public override object Description => 1074574;
        /* If you're not willing to follow the proper process then go away. */
        public override object Refuse => 1074606;
        /* You're not getting a refund without the proper forms and signatures. */
        public override object Uncomplete => 1074605;
        /* Oh blast!  Not another of those forms.  I'm so sick of this endless paperwork. */
        public override object Complete => 1074607;
        public override bool CanOffer()
        {
            return MondainsLegacy.Bedlam;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class YouScratchMyBackQuest : BaseQuest
    {
        public YouScratchMyBackQuest()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(UnicornRibs), "unicorn ribs", 1));
            AddObjective(new ObtainObjective(typeof(KirinBrains), "ki-rin brains", 2));
            AddObjective(new ObtainObjective(typeof(PixieLeg), "pixie legs", 5));

            AddReward(new BaseReward(1074634)); // Tuition Reimbursement
        }

        public override QuestChain ChainID => QuestChain.TuitionReimbursement;
        public override Type NextQuest => typeof(FoolingAernyaQuest);
        /* You Scratch My Back */
        public override object Title => 1074608;
        /* Heh.  Heheheh.  Good one.  You're not a Bedlam student and you're definitely not eligible for a tuition refund.  
        Heheheh. That old witch Aernya doesn't see as well as she used to you know.  Otherwise, she would have ... hmmm, 
        wait a minute.  I sense a certain 'opportunity' here.  I'll sign your forms in return for a little help with a 
        project of my own.  What do you say? */
        public override object Description => 1074609;
        /* Hehehe.  Your choice. */
        public override object Refuse => 1074615;
        /* I'm something of a gourmet, you see.  It's tough getting some of the ingredients, though.  Bring me back some 
        pixie legs, unicorn ribs and ki-rin brains and I'll sign your form. */
        public override object Uncomplete => 1074616;
        /* Oh excellent, you're back.  I'll get the oven going.  That thing about pixie legs, you see, is that they burn 
        and dry out if you're not really careful.  Taste just like chicken too! */
        public override object Complete => 1074617;
        public override bool CanOffer()
        {
            return MondainsLegacy.Bedlam;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class FoolingAernyaQuest : BaseQuest
    {
        public FoolingAernyaQuest()
            : base()
        {
            AddObjective(new DeliverObjective(typeof(SignedTuitionReimbursementForm), "signed tuition reimbursement form", 1, typeof(Aernya), "Aernya (Umbra)"));

            AddReward(new BaseReward(1074634)); // Tuition Reimbursement
        }

        public override QuestChain ChainID => QuestChain.TuitionReimbursement;
        public override Type NextQuest => typeof(NotQuiteThatEasyQuest);
        /* Fooling Aernya */
        public override object Title => 1074618;
        /* Now that I've signed your papers you'd better get back to that witch Aernya.  Mmmm mmm smell those ribs! */
        public override object Description => 1074619;
        /* Giving up on your scheme eh?  Suit yourself. */
        public override object Refuse => 1074620;
        /* You better hurry back to Mistress Aernya with that signed form.  The college only has so much money and with 
        enough claims you may find yourself unable to get your tuition refunded.  *wink* */
        public override object Uncomplete => 1074621;
        /* What?  Hrmph.  Gorrow signed your form did he?  Let me see that.  *squint* */
        public override object Complete => 1074622;
        public override bool CanOffer()
        {
            return MondainsLegacy.Bedlam;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class NotQuiteThatEasyQuest : BaseQuest
    {
        public NotQuiteThatEasyQuest()
            : base()
        {
            AddObjective(new DeliverObjective(typeof(SignedTuitionReimbursementForm), "signed tuition reimbursement form", 1, typeof(Gnosos), "Master Gnosos (Bedlam)"));

            AddReward(new BaseReward(1074634)); // Tuition Reimbursement
        }

        public override QuestChain ChainID => QuestChain.TuitionReimbursement;
        public override Type NextQuest => typeof(ConvinceMeQuest);
        /* Not Quite That Easy */
        public override object Title => 1074623;
        /* I wouldn't be too smug just yet, whiner.  You still need Master Gnosos' signature before I can cut your refund.  
        Last I heard, he's coordinating the recovery of the portions of the college that are currently overrun.  *nasty 
        smile*  Off with you. */
        public override object Description => 1074624;
        /* Coward. */
        public override object Refuse => 1074626;
        /* What are you waiting for?  The iron maiden is still the portal to Bedlam. */
        public override object Uncomplete => 1074627;
        /* Made it through did you?  Did you happen to see Red Death out there?  Big horse, skeletal ... burning eyes?  No?  
        What's this?  Forms?  FORMS?  I'm up to my eyebrows in ravenous out-of-control undead and you want a signature? */
        public override object Complete => 1074628;
        public override bool CanOffer()
        {
            return MondainsLegacy.Bedlam;
        }

        public override void OnAccept()
        {
            base.OnAccept();

            Owner.Bedlam = true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class ConvinceMeQuest : BaseQuest
    {
        public ConvinceMeQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(RedDeath), "red death", 1, "Bedlam"));
            AddObjective(new SlayObjective(typeof(GoreFiend), "gore fiends", 10, "Bedlam"));
            AddObjective(new SlayObjective(typeof(RottingCorpse), "rotting corpses", 8, "Bedlam"));

            AddReward(new BaseReward(1074634)); // Tuition Reimbursement
        }

        public override QuestChain ChainID => QuestChain.TuitionReimbursement;
        public override Type NextQuest => typeof(TuitionReimbursementQuest);
        /* Convince Me */
        public override object Title => 1074629;
        /* I'm not signing any forms until the situation here is under control.  So, you can either help out or you 
        can forget getting your tuition refund.  Which will it be?  Help control the shambling dead? */
        public override object Description => 1074630;
        /* No signature for you. */
        public override object Refuse => 1074631;
        /* No signature for you until you kill off some of the shambling dead out there and destroy that blasted horse. */
        public override object Uncomplete => 1074632;
        /* Pulled it off huh?  Well then you've earned this signature! */
        public override object Complete => 1074633;
        public override bool CanOffer()
        {
            return MondainsLegacy.Bedlam;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class TuitionReimbursementQuest : BaseQuest
    {
        public TuitionReimbursementQuest()
            : base()
        {
            AddObjective(new DeliverObjective(typeof(CompletedTuitionReimbursementForm), "completed tuition reimbursement form", 1, typeof(Aernya), "Aernya (Umbra)"));

            AddReward(new BaseReward(typeof(RewardBox), 1072584));
        }

        public override QuestChain ChainID => QuestChain.TuitionReimbursement;
        public override Type NextQuest => typeof(TheGoldenHornQuest);
        /* Tuition Reimbursement */
        public override object Title => 1074634;
        /* Well, there you are.  I've added my signature to that of Gorrow, so you should be set to return to Mistress 
        Aernya and get your tuition refunded. */
        public override object Description => 1074635;
        /* Great! If you're going to stick around here, I know we have more tasks for you to perform. */
        public override object Refuse => 1074636;
        /* Just head out the main gates there and you'll find yourself embracing the iron maiden in the Bloodletter's Guild. */
        public override object Uncomplete => 1074637;
        /* *disinterested stare*  What?  Oh, you've gotten your form filled in.  How nice.  *glare*  And I'd hoped you'd 
        drop this charade before I was forced to rub your nose in it.  *nasty smile*  You're not even a student and as 
        such, you're not eligible for a refund -- you've never paid tuition.  For your services, Master Gnosos has 
        recommended you receive pay.  So here.  Now go away. */
        public override object Complete => 1074638;
        public override bool CanOffer()
        {
            return MondainsLegacy.Bedlam;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}