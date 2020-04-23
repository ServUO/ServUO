namespace Server.Engines.Quests.Collector
{
    public class DontOfferConversation : QuestConversation
    {
        public override object Message =>
                /* <I>Elwood looks up from his ale as you greet him.</I><BR><BR>
* 
* What's that? Who me? No, no. You must be looking for someone else.
*/
                1055080;
        public override bool Logged => false;
    }

    public class DeclineConversation : QuestConversation
    {
        public override object Message =>
                /* <I>Elwood looks a bit flustered and nearly knocks over his
* bottle of ale.</I><BR><BR>
* 
* Well, I see. It's like that, is it? Yes. Well then. Okay.
* You've changed. Yes. Yes, you have. Something's changed. I know
* I haven't. Not me. Not good ole Elwood.<BR><BR>
* 
* <I>Elwood trails off, though you can still hear him muttering softly.</I>
*/
                1055082;
        public override bool Logged => false;
    }

    public class AcceptConversation : QuestConversation
    {
        public override object Message =>
                /* <I>Elwood slaps his knee and grins at you.</I><BR><BR>
* 
* Yes. Yes. That's the spirit. I knew it. Knew it when I
* first saw you. You remind me so much of your dear departed
* father. Or someone. Not sure. Maybe no one. But that's okay.
* Ah, good times.<BR><BR>
* 
* Anyway, so as you know, I'm a collector. Got all kinds of
* interesting things laying around back at my warehouse.
* You know. You've seen it. Haven't you? Yes? No? Nevermind.
* Not important.<BR><BR>
* 
* Right. So, always trying to add new things to my collection.
* Or sometimes just get more of something. Can't have too many.
* Right? Yeah? Sure.<BR><BR>
* 
* Let's see. Where to start. Oh, I know. Pearls. Yes. But not
* just any pearls. Rainbow pearls. Yes. From the lake here in
* Haven. Seems all that magic Uzeraan was throwing around when
* he transformed the island had an interesting effect on some
* of the shellfish down there. Exactly, rainbow pearls. Useless
* for magic, but an item worth collecting. Trust me on this.
* Trust me.<BR><BR>
* 
* Need you to go fish some up for me. Down at the lake. Lake Haven.
* Off ya go. Happy fishing!<BR><BR>
* 
* <I>Elwood turns back to his ale and now seems oblivious to you.</I>
*/
                1055083;
        public override void OnRead()
        {
            System.AddObjective(new FishPearlsObjective());
        }
    }

    public class ElwoodDuringFishConversation : QuestConversation
    {
        public override object Message =>
                /* <I>Elwood looks up as you tap him on the shoulder.</I><BR><BR>
* 
* Good. Good. You're back. Wait. You don't have the rainbow pearls I
* need. Taking a break? Yeah. Sure. There's no hurry. Let me know when
* you've got all those pearls, though. I'll be here.
*/
                1055089;
        public override bool Logged => false;
    }

    public class ReturnPearlsConversation : QuestConversation
    {
        public override object Message =>
                /* <I>Elwood jumps slightly when you call his name.</I><BR><BR>
* 
* What. I'm awake. Oh, It's you. Hey, look at those pearls.
* Beautiful. Wow. Where'd you get those... oh right. I sent you to get
* them. From the lake. Lake Haven. Great job. Gotta love rainbow
* pearls. Oooh, Colors.<BR><BR>
* 
* Okay, let's see. Next. Need a painting. Go to the Colored Canvas and
* speak to Alberta. Alberta Giacco. Best painter I've ever met. Ask
* her to do a painting of you. Yes. Of you. A portrait. Never know if
* you might up and become famous one day. Need to have a painting of you
* in my collection. From now. Before all the fame. Go. Alberta awaits.
* She's in Vesper.<BR><BR>
* 
* <I>Elwood starts playing with the pearls you brought him and seems
* to have forgotten you're there.</I>
*/
                1055090;
        public override void OnRead()
        {
            System.AddObjective(new FindAlbertaObjective());
        }
    }

    public class AlbertaPaintingConversation : QuestConversation
    {
        public override object Message =>
                /* <I>Alberta looks up from the painting she is working on and
* faces you.</I><BR><BR>
* 
* Excuse me. I don't mean to be rude, but I'm in the middle
* of something, and can't... oh wait, I see. You must be the one
* Elwood sent over.<BR><BR>
* 
* Very well. If you'll have a seat on the stool over there, we'll
* get started. This will just take a few seconds. I paint quite
* quickly, you see. I'll start once you are seated.<BR><BR>
* 
* <I>Alberta exchanges the painting she was working on for a blank canvas.</I>
*/
                1055092;
        public override void OnRead()
        {
            System.AddObjective(new SitOnTheStoolObjective());
        }
    }

    public class AlbertaStoolConversation : QuestConversation
    {
        public override object Message =>
                /* <I>Alberta looks at you sympathetically.</I><BR><BR>
* 
* Don't worry, this will only take a few seconds. I realize
* that stool can be uncomfortable, and I apologize for that.
* Perhaps I should replace it with a more comfortable chair.
* But then again, it's that very discomfort that helps produce
* such wonderful facial expressions for my paintings. Ah well.
*/
                1055096;
        public override bool Logged => false;
    }

    public class AlbertaEndPaintingConversation : QuestConversation
    {
        public override object Message =>
                /* <I>Alberta stands back from the canvas and surveys her
* work.</I><BR><BR>
* 
* Not too bad. Quite good even, if I do say so myself. As always,
* of course.<BR><BR>
* 
* Oh, you're still here. Please let Elwood know that the painting
* has been completed. I'll have it sent to him once it dries.<BR><BR>
* 
* <I>Alberta removes the portrait from her easel and sets
* it aside to dry.</I>
*/
                1055098;
        public override void OnRead()
        {
            System.AddObjective(new ReturnPaintingObjective());
        }
    }

    public class AlbertaAfterPaintingConversation : QuestConversation
    {
        public override object Message =>
                /* <I>Alberta stops cleaning her brushes and looks your way.</I><BR><BR>
* 
* Don't worry, I'll send the painting to Elwood once it's dry. Please
* let him know that the painting has been finished.
*/
                1055102;
        public override bool Logged => false;
    }

    public class ElwoodDuringPainting1Conversation : QuestConversation
    {
        public override object Message =>
                /* <I>Elwood yawns and stretches, then focuses his gaze
* on you.</I><BR><BR>
* 
* Hello. Do I know you? Hold on a second. Yes. Yes, I do.
* You were going to bring me some rainbow pearls. Wait. No.
* You already did that. I remember now. Right. So go get that
* portrait painted. Alberta is in Vesper. Go to her. Alberta
* Giacco. Come back when she's done.
*/
                1055094;
        public override bool Logged => false;
    }

    public class ElwoodDuringPainting2Conversation : QuestConversation
    {
        public override object Message =>
                /* <I>Elwood drums his fingers on the counter then looks
* up at you expectantly.</I><BR><BR>
* 
* Ah, finally. I'm famished. This so-called tavern doesn't
* even serve pizza, so one must have it delivered. It's an
* outrage. Er... wait. Where's my pizza? Yes, my pizza.
* What kind of delivery is this? Didn't even bring my pizza.
* This will severely impact your tip, I'm afraid.<BR><BR>
* 
* Hold on a moment. You were helping me with something else,
* weren't you? Ah. Yes. I've got it now. You were to have a
* portrait done. Well. Good. Yes. But let's not dawdle.
* Off you go. If you happen to see anyone with my pizza,
* please insist they hurry.<BR><BR>
*/
                1055097;
        public override bool Logged => false;
    }

    public class ReturnPaintingConversation : QuestConversation
    {
        public override object Message =>
                /* <I>Elwood notices you immediately and waves you over.</I><BR><BR>
* 
* You're back. Good. That's good. Hmm. You don't seem to have that
* painting yet. Don't tell me Alberta refused. That's no good. I made
* her what she is! She was living in the gutter when I found her. The
* gutter. And she has the... what's that? Oh, she finished the portrait.
* Right. Good. Can always count on Alberta. Always. Best painter in
* the land.<BR><BR>
* 
* Just have a couple... er... wait... make that... two more task for you.
* Then our business will be concluded. For now. Old chums like us will
* always work together again. Yes. We go so far back. Right. I think.<BR><BR>
* 
* Anyway. There's a musician. A minstrel. His name's Gabriel Piete. Quite
* the good singer. One of my favorites. Absolute favorite. What I'd like
* is his autograph. Simple. Just his autograph. You're likely to find him
* at the Conservatory of Music in Britain. He's often there. Often. Between
* performances. Hurry now. There ya go.<BR><BR>
* 
* <I>Elwood falls silent though his lips are still  moving. It looks like
* he's quietly repeating the word, "autograph."</I>
*/
                1055100;
        public override void OnRead()
        {
            System.AddObjective(new FindGabrielObjective());
        }
    }

    public class GabrielAutographConversation : QuestConversation
    {
        public override object Message =>
                /* <I>Gabriel sighs loudly as you address him and stops whatever
* it was that he was doing.</I><BR><BR>
* 
* WHAT?!? Can you not see that I'm working here?  Ugh, sometimes
* I wish they'd just lock the outer doors to anyone who doesn't
* belong here. So go ahead and fawn. Get it over with, and then
* leave. Sooner you're out of here, the sooner I'm back to work.<BR><BR>
* 
* I see. So you want an autograph. Fine. If it'll get rid of you,
* I'll give you my autograph. But I'll only sign some sheet music
* for one of my songs. Until then, please let me get back to work.<BR><BR>
* 
* You can probably find some of my sheet music at one of the theaters
* in the land. When I perform there, they often sell it as a souvenir.
* Speak to the impresario... the theater manager. My last three
* performances were in Nujel'm, Jhelom, and here in Britian.
*/
                1055103;
        public override void OnRead()
        {
            System.AddObjective(new FindSheetMusicObjective(true));
        }
    }

    public class GabrielNoSheetMusicConversation : QuestConversation
    {
        public override object Message =>
                /* <I>Gabriel does not look happy to see you.</I><BR><BR>
* 
* Do you have any sheet music? No. Please return when you do,
* but until then please leave me to my work.
*/
                1055111;
        public override bool Logged => false;
    }

    public class NoSheetMusicConversation : QuestConversation
    {
        public override object Message =>
                /* Sheet music for a Gabriel Piete song? No, I'm sorry, but we've run out.
* We might get some more after he performs here again, but right now
* we don't have any. My apologies.
*/
                1055106;
        public override bool Logged => false;
    }

    public class GetSheetMusicConversation : QuestConversation
    {
        public override object Message =>
                // The theater impresario hands you some sheet of music for one of Gabriel Piete's songs.
                1055109;
        public override void OnRead()
        {
            System.AddObjective(new ReturnSheetMusicObjective());
        }
    }

    public class GabrielSheetMusicConversation : QuestConversation
    {
        public override object Message =>
                /* <I>Gabriel looks up impatiently as you approach.</I><BR><BR>
* 
* Good. We can finally be done with one another. Here, let me
* sign that and have this business completed.<BR><BR>
* 
* <I>Gabriel takes the sheet music, autographs it, and then hands it back to you.</I>
*/
                1055113;
        public override void OnRead()
        {
            System.AddObjective(new ReturnAutographObjective());
        }
    }

    public class GabrielIgnoreConversation : QuestConversation
    {
        public override object Message =>
                // <I>Gabriel ignores you.</I>
                1055118;
        public override bool Logged => false;
    }

    public class ElwoodDuringAutograph1Conversation : QuestConversation
    {
        public override object Message =>
                /* <I>Elwood jumps and almost falls from his stool when you
* tap his shoulder.</I><BR><BR>
* 
* Oh my. Don't do that. Scared me half to death. Sneaking up on
* people like that. I don't even know you. Yes, I do. I do know you.
* Ordered a pizza from you and never received it. I'm rather miffed
* about that.<BR><BR>
* 
* No. No. Wait. Of course. Not the pizza. You're here with the
* moonfire brew I wanted. Magical moonfire brew. Not sure what it is.
* Just want some.<BR><BR>
* 
* What's that? Oh. The autograph. Gabriel Piete. Yes. Of course.
* Do you have it? No. Well. Hmm. Don't sneak up on people like that. Not polite.
*/
                1055105;
        public override bool Logged => false;
    }

    public class ElwoodDuringAutograph2Conversation : QuestConversation
    {
        public override object Message =>
                /* <I>Elwood seems to be studying the bottom of his bottle of ale
* as you approach.</I><BR><BR>
* 
* What's this? Strange. Quite strange. I could have sworn I was
* drinking wine.<BR><BR>
* 
* Oh, hello. Yes. Good to see you. Any luck with that autograph?
* Bet you thought I'd forgotten. No. Not me. Mind like a steel trap.
* Can't get it open no matter how hard you try. Or something. No luck
* yet? Ah well. keep trying. I have faith in you. Whoever you are.
*/
                1055112;
        public override bool Logged => false;
    }

    public class ElwoodDuringAutograph3Conversation : QuestConversation
    {
        public override object Message =>
                /* <I>Elwood scratches his ear then notices you.</I><BR><BR>
* 
* Good day. What brings you to the Albatross? Me? An autograph?
* You want my autograph? Well, I suppose. What's that? Oh. Yes.
* Gabriel Piete. Yes. Get his autograph and return to me. Good day.
*/
                1055115;
        public override bool Logged => false;
    }

    public class ReturnAutographConversation : QuestConversation
    {
        public override object Message =>
                /* <I>Elwood looks up eagerly as you tell him about the autographed
* sheet music.</I><BR><BR>
* 
* Quite good work. Not an easy one to deal with, that one. Gabriel Piete.
* Yes. Well done. Nice autograph.<BR><BR>
* 
* One last task. I would like a set of toy monster figurines made by the
* famous toymaker, Tomas O'Neerlan. You'll find him in Trinsic. He's often
* at the Tinker's Guild. Try looking there.<BR><BR>
* 
* You're doing quite well. Quite well indeed. Knew you would. Just like old
* times. Yes. Quite good.
*/
                1055116;
        public override void OnRead()
        {
            System.AddObjective(new FindTomasObjective());
        }
    }

    public class TomasToysConversation : QuestConversation
    {
        public override object Message =>
                /* <I>Tomas smiles freely as you speak to him.</I><BR><BR>
* 
* Ah, to be sure I can make you some toy monster figurines.
* That's my work, making toys. Worry not, we'll put together
* a good set of monsters for you and your figurines.<BR><BR>
* 
* But I'll be needing something from you before I can begin.
* Here, take these enchanted paints. Using them, you can capture
* a set of images of the creatures you wish me to make into toys.
* It'll only work on the group of creatures I select for your
* set of figurines. Oh, and I'll be needing those enchanted paints
* back when all is said and done.<BR><BR>
*/
                1055119;
        public override void OnRead()
        {
            System.AddObjective(new CaptureImagesObjective(true));
        }
    }

    public class TomasDuringCollectingConversation : QuestConversation
    {
        public override object Message =>
                /* <I>Tomas greets you warmly as you approach.</I><BR><BR>
* 
* 'Tis good to see you. I see that you have not yet collected
* all of the images we need. 'Tis fine, but I'll be needing
* those before I can make the toy figurines. Return when you have
* the complete set of images.<BR><BR>
*/
                1055129;
        public override bool Logged => false;
    }

    public class ReturnImagesConversation : QuestConversation
    {
        public override object Message =>
                /* <I>Tomas grins as you walk toward him.</I><BR><BR>
* 
* I see that you have collected all of the images we need.
* 'Tis good. I'll begin straight away on the toy figurines.
* I'll have them delivered to you when ready. Where shall I send
* them? To Elwood in Haven? Yes, I know him. We've done business
* in the past. Odd fellow.<BR><BR>Tomas smiles as you return his
* enchanted paints back to him.<BR><BR>
*/
                1055131;
        public override void OnRead()
        {
            System.AddObjective(new ReturnToysObjective());
        }
    }

    public class ElwoodDuringToys1Conversation : QuestConversation
    {
        public override object Message =>
                /* <I>You watch as Elwood spins around blissfully on his stool.</I><BR><BR>
* 
* Oh. Forgive me. Didn't see you there. Whoo. Dizzy. Can't see straight.
* Have you gotten those figurines yet? No. Ah. Not to worry. Keep at it.
* No doubt you'll come through with those.<BR><BR>
* 
* Excuse me a moment. I think I need to sit down. Wait. I am sitting down.
* Good then. Yes. Sitting down.<BR><BR><I>Elwood reaches out and takes hold
* of the counter as if to steady himself.</I>
*/
                1055123;
        public override bool Logged => false;
    }

    public class ElwoodDuringToys2Conversation : QuestConversation
    {
        public override object Message =>
                /* <I>Elwood suddenly stops and beckons you over to him.</I><BR><BR>
* 
* Over here. Come here. Don't be alarmed, but I think one of the
* tavernkeepers used to be a wandering healer. Said something about
* the good ole days when people would just walk up and hand gold to
* them. Piles of gold. Out of the blue. Can you imagine? Got so much
* gold, this ex-healer decided to buy a tavern and settle down. Kind
* of sad really. Can't even cure my hangover now. Nice tavern though.<BR><BR>
* 
* Right. Anyway. Let me know when those toy figurines are ready.
* I'll be here. As always.
*/
                1055130;
        public override bool Logged => false;
    }

    public class ElwoodDuringToys3Conversation : QuestConversation
    {
        public override object Message =>
                /* <I>Elwood is singing as you greet him.</I><BR><BR>
* 
* Come. Join in. It's a Gabriel Piete song. I have the sheet music
* for it. It's even autographed by Gabriel Piete himself. Yes.
* One of his songs. Brilliant.<BR><BR>
* 
* So let me see the toys. Figurines. Let's see them. Oh. You don't
* have them yet. I see. Well. Okay. That's too bad.
*/
                1055133;
        public override bool Logged => false;
    }

    public class EndConversation : QuestConversation
    {
        public override object Message =>
                /* <I>Elwood takes a sip of his ale as you address him.</I><BR><BR>
* 
* Ah. That's the stuff. Ale. Nothing better. What's that? Toy figurines
* will be delivered. Right. Yes. Perhaps better than Ale. Tough one.<BR><BR>
* 
* Very good work. Quite. Think we're all done now. You completed everything
* I asked. Would work with you again. Yes. We should.<BR><BR>
* 
* Ah. Yes. your payment. The usual. I think you'll be pleased.<BR><BR>
* 
* <I>With that Elwood rummages around in his backpack. He eventually
* pulls out a small bag and hands it to you.</I>
*/
                1055134;
        public override void OnRead()
        {
            System.Complete();
        }
    }

    public class FullEndConversation : QuestConversation
    {
        private readonly bool m_Logged;
        public FullEndConversation(bool logged)
        {
            m_Logged = logged;
        }

        public FullEndConversation()
        {
            m_Logged = true;
        }

        public override object Message =>
                /* <I>Elwood stares at you as you approach.</I><BR><BR>
* 
* I know you. Oh yes. You've been running some errands for me.
* We are about done. Noticed that your backpack is a bit full.
* Might want to make some room. Won't be able to hold your payment.
* Come back when you have more room, and we'll conclude our business.
*/
                1055135;
        public override bool Logged => m_Logged;
        public override void OnRead()
        {
            if (m_Logged)
                System.AddObjective(new MakeRoomObjective());
        }
    }
}
