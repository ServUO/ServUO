namespace Server.Engines.Quests.Hag
{
    public class DontOfferConversation : QuestConversation
    {
        public override object Message =>
                /* <I>The ancient, wrinkled hag looks up from her vile-smelling cauldron.
* Her single, unblinking eye attempts to focus in on you, but to
* little avail.</I><BR><BR>
* 
* What's that?  Who's there?  What do you want with me?  I don't have
* time for the likes of you.  I have stews to spice and brews to boil.
* Too many things to complete to be helping out a stranger.<BR><BR>
* 
* Besides, it looks as if you've already got yourself a quest that needs
* doing.  Perhaps if you finish the task you're on, you can return to me
* and I'll help you out.  But until then, leave an old witch alone to her
* magics!  Shoo!  Away with ye!<BR><BR>
* 
* <I>The witch rushes you off with a wave of her decrepit hand and returns
* to tending the noxious brew boiling in her cauldron.</I>
*/
                1055000;
        public override bool Logged => false;
    }

    public class AcceptConversation : QuestConversation
    {
        public override object Message =>
                /* <I>Somewhat out of character for the vile old witch, she actually seems
* delighted that you've accepted her offer.</I><BR><BR>
* 
* Ah! That's the spirit! You're not a useless bag of bones after all, are ye?
* Well then, best get your hind quarters in gear and head towards the road!
* Remember, my young Apprentice could be anywhere along the road heading towards
* the Yew Graveyard, so be sure to run the whole course of it, and stay
* on track!<BR><BR>
* 
* And for Gashnak's sake, come back here when you've found something! And remember,
* I don't have all day!  And watch out for the imp Zeefzorpul!  And don't return
* empty handed!  And pack a warm sweater!  And don't trample my lawn on the
* way out!<BR><BR>
* 
* What are you still doing here?  Get to it!  Shoo!
*/
                1055002;
        public override void OnRead()
        {
            System.AddObjective(new FindApprenticeObjective(true));
        }
    }

    public class HagDuringCorpseSearchConversation : QuestConversation
    {
        public override object Message =>
                /* <I>The wrinkled hag looks up at you with venom in her eyes.</I><BR><BR>
* 
* What're you doing back here?  I thought I told you to go find my lost
* Apprentice!  I don't have time for your laziness, you wretched little worm!
* Shoo! Away with ye! And don't come back until you've found out what's
* happened to my Apprentice!
*/
                1055003;
        public override bool Logged => false;
    }

    public class ApprenticeCorpseConversation : QuestConversation
    {
        public override object Message =>
                /* You inspect the charred and bloodied corpse, recognizing it from the
* Hag's description as the lost Apprentice you were tasked to
* bring back.<BR><BR>
* 
* It appears as if he has been scorched by fire and magic, and scratched
* at with vicious claws.<BR><BR>
* 
* You wonder if this horrific act is the work of the vile imp Zeefzorpul
* of which the Hag spoke.  You decide you'd best return to the Hag and
* report your findings.
*/
                1055004;
        public override void OnRead()
        {
            System.AddObjective(new FindGrizeldaAboutMurderObjective());
        }
    }

    public class MurderConversation : QuestConversation
    {
        public override object Message =>
                /* <I>The wrinkled old Hag looks up from her cauldron of boiling
* innards.</I><BR><BR>
* 
* Bah!  Back already?  Can't you see I'm busy with my cooking?  You
* wouldn't like to have a little taste of my delicious dragon gizzard soup,
* would you?  Haw! I thought as much.<BR><BR>
* 
* Enough of this jibber-jabber then - what news of my Apprentice?<BR><BR>
* 
* What's that?  You say that horrible little imp Zeefzorpul was behind his
* disappearance!?  What would Zeefzorpul want with my Apprentice?  Probably
* just wants to make life more miserable for me than it already is.<BR><BR>
* 
* Wait! Bah! That must be it! Zeefzorpul must have found out that I sent
* my Apprentices out with various Magic Brew Recipes - lists of tasks and
* ingredients that needed completing.<BR><BR>
* 
* That despicable Zeefzorpul knows I need the list of ingredients I gave to
* that Apprentice.  I've recipes to mix, stews to boil, magics to cast, and
* fortunes to meddle!  I won't let that wretched felchscum spoil my day.
* You then, I need you to go find Zeefzorpul and get that scrap of
* parchment back!<BR><BR>
* 
* I'm not sure where he bides his time, but I'm sure if you go find his imp
* friends and rough them up, they'll squeal on him in no time!  They all
* know each others' secret hiding places.  Go on!  Shoo! Go slay a few imps
* until they cough up their secrets!  No mercy for those little nasties!
*/
                1055005;
        public override void OnRead()
        {
            System.AddObjective(new KillImpsObjective(true));
        }
    }

    public class HagDuringImpSearchConversation : QuestConversation
    {
        public override object Message =>
                /* <I>The sickly old hag looks up from her boiling cauldron.</I><BR><BR>
* 
* Have you found that vile little Zeefzorpul yet?  What!?  You've come
* back here without finding out where Zeefzorpul is, and what he's done
* with my Magic Brew Recipe?<BR><BR>
* 
* I told you what needs to be done, you little whelp!  Now away with ye!
* And don't you return until you've found my list of ingredients!
*/
                1055006;
        public override bool Logged => false;
    }

    public class ImpDeathConversation : QuestConversation
    {
        private Point3D m_ImpLocation;
        public ImpDeathConversation(Point3D impLocation)
        {
            m_ImpLocation = impLocation;
        }

        public ImpDeathConversation()
        {
        }

        public override object Message =>
                /* <I>The wretched imp cries out for mercy.</I><BR><BR>
* 
* Forgive me! You master! You great warrior, great hooman, great greatest!
* Forgive! Forgive! I give up Zeef! He no good any way!  He always smack me
* head and hurt me good!  He say I ugly too, even with me pretty teef!<BR><BR>
* 
* But I knows where he hide!  I follow him flapping to his hidey hole.
* He think he so smart but he so wrong!  I make scribble drawing of where he
* like to hide!  But you need the whistle blower to make him come!  He no come
* without it!  Make with the whistle at his hidey place, and Zeef must come,
* he cannot resist!<BR><BR>
* 
* <I>The frightened imp hands you a crumpled map and a strange flute.</I><BR><BR>
* 
* You go to where the picture shows and then you play that whistle!  Zeef come,
* me promise!  But you make promise that you smack Zeef head good!
* Pweese?<BR><BR>
* 
* <I>With this last request, the miserable little imp falls and breathes no more.</I>
*/
                1055007;
        public override void OnRead()
        {
            System.AddObjective(new FindZeefzorpulObjective(m_ImpLocation));
        }

        public override void ChildDeserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            m_ImpLocation = reader.ReadPoint3D();
        }

        public override void ChildSerialize(GenericWriter writer)
        {
            writer.WriteEncodedInt(0); // version

            writer.Write(m_ImpLocation);
        }
    }

    public class ZeefzorpulConversation : QuestConversation
    {
        public override object Message =>
                /* <I>In a puff of smoke that smells of brimstone, the imp Zeefzorpul
* appears.</I><BR><BR>
* 
* Wuh-whut!? How did stupid hooman find mighty Zeefzorpul?  This crazy
* many times!  This crazy not possible! This big crazy with crazy on top!
* But it happening!  How can it be true!?<BR><BR>
* 
* GAH! Even mighty Zeefzorpul can no resist that crazy music!  Mighty
* Zeefzorpul do what you want!  Have you stupid paper back!  Mighty Zeefzorpul
* no want it any way.  It dumb.  It super dumb.  Big dumb like stupid dumb
* tree with dumb things on it!  So stupid!  So dumb that mighty Zeefzorpul
* not even care!  You see me not caring?  You better cause it certainly
* happening!  Me not caring one bit!<BR><BR>
* 
* <I>The strange little imp tosses the piece of parchment at you.  Much
* to your surprise, however, he swoops down in a flash of flapping wings
* and steals the Magic Flute from your grasp.</I><BR><BR>
* 
* Hah! So stupid like a hooman!  Mighty Zeefzorpul has defeated stupid
* hooman and is greatest ever imp in world!  You serious stupid, mister
* hooman.  Big stupid with stupid on top.  Now you no can make trick on me
* again with crazy dance music!  Mighty Zeefzorpul fly away to his other
* secret home where you never find him again!<BR><BR>
* 
* Me hope you get eated by a troll!<BR><BR>
* 
* <I>With that, the imp Zeefzorpul disappears in another puff of rancid smoke.</I>
*/
                1055008;
        public override void OnRead()
        {
            System.AddObjective(new ReturnRecipeObjective());
        }
    }

    public class RecipeConversation : QuestConversation
    {
        public override object Message =>
                /* <I>The wart-covered witch looks up from pouring fetid scraps of meat
* into her cauldron.</I><BR><BR>
* 
* You've dealt with that troublesome imp Zeefzorpul?  Good for you, little
* one!  You're not as useless as you appear, even to a daft old wench such
* as myself!<BR><BR>
* 
* Now then, I see you've recovered my precious Magic Brew Recipe.  I suppose
* you expect a reward?  Well, you can go on expecting, and I can go on being
* ugly.  What good is it to me that I have the list, if I don't have an
* apprentice to go gather the ingredients and perform the tasks
* themselves!<BR><BR>
* 
* If you want your precious little reward, you'll have to complete the task
* I gave to my previous Apprentice.  Now away with you!  Shoo! Shimmy! Skedattle!
* I've heads to boil and stews to spice!  Don't you return until you've completed
* every item on that list!
*/
                1055009;
        public override void OnRead()
        {
            System.AddObjective(new FindIngredientObjective(new Ingredient[0]));
        }
    }

    public class HagDuringIngredientsConversation : QuestConversation
    {
        public override object Message =>
                /* <I>The ancient crone looks up from her bubbling brew, staring you down
* with her one good eye.</I><BR><BR>
* 
* You've returned already have you?  And what of your task?  Have you gathered
* all the needed ingredients?<BR><BR>
* 
* What's that!?  You still haven't finished the simple little task I've set before
* you?  Then why come back here and bother me?  I can't get a single brew
* concocted if you keep bugging me with your whimpering little diatribes!  Why,
* you're worse than my last apprentice - and he was the very king of fools!<BR><BR>
* 
* Go on with ye!  Away and begone!  I don't want to see hide nor hair of your
* whining little face until you've gathered each and every last one of the ingredients
* on that list!<BR><BR>
* 
* <I>With a disgusting hacking noise, the vile witch spits upon the ground and
* brushes you off with a wave of her wrinkled old hand.</I>
*/
                1055012;
        public override bool Logged => false;
    }

    public class BlackheartFirstConversation : QuestConversation
    {
        public override object Message =>
                /* <I>The bawdy old pirate captain looks up from his bottle of Wild Harpy
* whiskey, as drunk as any man you've ever seen.<BR><BR>
* 
* With an excruciatingly slow movement, he pushes back his tricorne hat
* and stares you down with red-rimmed eyes.</I><BR><BR>
* 
* Whut tha blazes do ye want, landlubber?  Some've Captain Blackheart's
* fine Whiskey?  Well ye can drown in the seven seas, ya barnacle-covered
* bilge rat!<BR><BR>
* 
* I've cut down pasty-faced runts like yerself for lesser insults!  I've
* sailed the seas've this world fer fifty years, and never seen a more
* milk-soaked pansy lass than ye come in here for a favor.  Give ye some
* of my special Whiskey?  I'd sooner wrestle a sea serpent naked - and I've
* done that some twenty times!<BR><BR>
* 
* Ye see, ol' Captain Blackheart's Whiskey is only for pirate folk.  And ye
* don't look like no pirate I've ever seen.  Ye have te have the right cut
* of cloth and the right amount of liquor in yer belly te sail on my crew!
* And without that, ye might as well go home and cry to yer mommy.  Cause
* ye ain't ever gonna share no drink with me!<BR><BR>
* 
* Now off with ye!<BR><BR>
* 
* <I>With that, Captain Blackheart goes back to singing his bawdy songs
* and drinking his whiskey.  It seems as if you'll have to find some way to
* change his mind about your worthiness.</I>
*/
                1055010;
        public override void OnRead()
        {
            FindIngredientObjective obj = System.FindObjective(typeof(FindIngredientObjective)) as FindIngredientObjective;

            if (obj != null)
                System.AddObjective(new FindIngredientObjective(obj.Ingredients, true));
        }
    }

    public class BlackheartNoPirateConversation : QuestConversation
    {
        private bool m_Tricorne;
        private bool m_Drunken;
        public BlackheartNoPirateConversation(bool tricorne, bool drunken)
        {
            m_Tricorne = tricorne;
            m_Drunken = drunken;
        }

        public BlackheartNoPirateConversation()
        {
        }

        public override object Message
        {
            get
            {
                if (m_Tricorne)
                {
                    if (m_Drunken)
                    {
                        /* <I>The filthy Captain flashes a pleased grin at you as he looks you up
                        * and down.</I><BR><BR>Well that's more like it, me little deck swabber!
                        * Ye almost look like ye fit in around here, ready te sail the great seas
                        * of Britannia, sinking boats and slaying sea serpents!<BR><BR>
                        * 
                        * But can ye truly handle yerself?  Ye might think ye can test me meddle
                        * with a sip or two of yer dandy wine, but a real pirate walks the decks
                        * with a belly full of it.  Lookit that, yer not even wobblin'!<BR><BR>
                        * 
                        * Ye've impressed me a bit, ye wee tyke, but it'll take more'n that te
                        * join me crew!<BR><BR><I>Captain Blackheart tips his mug in your direction,
                        * offering up a jolly laugh, but it seems you still haven't impressed him
                        * enough.</I>
                        */
                        return 1055059;
                    }
                    else
                    {
                        /* <I>Captain Blackheart looks up from polishing his cutlass, glaring at
                        * you with red-rimmed eyes.</I><BR><BR>
                        * 
                        * Well, well.  Lookit the wee little deck swabby.  Aren't ye a cute lil'
                        * lassy?  Don't ye look just fancy?  Ye think yer ready te join me pirate
                        * crew?  Ye think I should offer ye some've me special Blackheart brew?<BR><BR>
                        * 
                        * I'll make ye walk the plank, I will!  We'll see how sweet n' darlin' ye
                        * look when the sea serpents get at ye and rip ye te threads!  Won't that be
                        * a pretty picture, eh?<BR><BR>
                        * 
                        * Ye don't have the stomach fer the pirate life, that's plain enough te me.  Ye
                        * prance around here like a wee lil' princess, ye do.  If ye want to join my
                        * crew ye can't just look tha part - ye have to have the stomach fer it, filled
                        * up with rotgut until ye can't see straight.  I don't drink with just any ol'
                        * landlubber!  Ye'd best prove yer mettle before ye talk te me again!<BR><BR>
                        * 
                        * <I>The drunken pirate captain leans back in his chair, taking another gulp of
                        * his drink before he starts in on another bawdy pirate song.</I>
                        */
                        return 1055057;
                    }
                }
                else
                {
                    if (m_Drunken)
                    {
                        /* <I>The inebriated pirate looks up at you with a wry grin.</I><BR><BR>
                        * 
                        * Well hello again, me little matey.  I see ye have a belly full of rotgut
                        * in ye.  I bet ye think you're a right hero, ready te face the world.  But
                        * as I told ye before, bein' a member of my pirate crew means more'n just
                        * being able to hold yer drink.  Ye have te look the part - and frankly, me
                        * little barnacle, ye don't have the cut of cloth te fit in with the crowd I
                        * like te hang around.<BR><BR>
                        * 
                        * So scurry off, ye wee sewer rat, and don't come back round these parts all
                        * liquored up an' three sheets te tha wind, unless yer truly ready te join
                        * me pirate crew!<BR><BR>
                        * 
                        * <I>Captain Blackheart shoves you aside, banging his cutlass against the
                        * table as he calls to the waitress for another round.</I>
                        */
                        return 1055056;
                    }
                    else
                    {
                        /* <I>Captain Blackheart looks up from his drink, almost tipping over
                        * his chair as he looks you up and down.</I><BR><BR>
                        * 
                        * You again?  I thought I told ye te get lost?  Go on with ye!  Ye ain't
                        * no pirate - yer not even fit te clean the barnacles off me rear end!
                        * Don't ye come back babbling te me for any of me Blackheart Whiskey until
                        * ye look and act like a true pirate!<BR><BR>
                        * 
                        * Now shove off, sewer rat - I've got drinkin' te do!<BR><BR>
                        * 
                        * <I>The inebriated pirate bolts back another mug of ale and brushes you
                        * off with a wave of his hand.</I>
                        */
                        return 1055058;
                    }
                }
            }
        }
        public override bool Logged => false;
        public override void ChildDeserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            m_Tricorne = reader.ReadBool();
            m_Drunken = reader.ReadBool();
        }

        public override void ChildSerialize(GenericWriter writer)
        {
            writer.WriteEncodedInt(0); // version

            writer.Write(m_Tricorne);
            writer.Write(m_Drunken);
        }
    }

    public class BlackheartPirateConversation : QuestConversation
    {
        private bool m_FirstMet;
        public BlackheartPirateConversation(bool firstMet)
        {
            m_FirstMet = firstMet;
        }

        public BlackheartPirateConversation()
        {
        }

        public override object Message
        {
            get
            {
                if (m_FirstMet)
                {
                    /* <I>The bawdy old pirate captain looks up from his bottle of Wild Harpy
                    * whiskey, as drunk as any man you've ever seen.</I><BR><BR>
                    * 
                    * Avast ye, ye loveable pirate!  Just in from sailin' the glorious sea?  Ye
                    * look right ready te fall down on the spot, ye do!<BR><BR>
                    * 
                    * I tell ye what, from the look've ye, ye deserve a belt of better brew than
                    * the slop ye've been drinking, and I've just the thing.<BR><BR>
                    * 
                    * I call it Captain Blackheart's Whiskey, and it'll give ye hairs on yer chest,
                    * that's for sure.  Why, a keg of this stuff once spilled on my ship, and it
                    * ate a hole right through the deck!<BR><BR>Go on, drink up, or use it to clean
                    * the rust off your cutlass - it's the best brew, either way!<BR><BR>
                    * 
                    * <I>Captain Blackheart hands you a jug of his famous Whiskey. You think it best
                    * to return it to the Hag, rather than drink any of the noxious swill.</I>
                    */
                    return 1055054;
                }
                else
                {
                    /* <I>The drunken pirate, Captain Blackheart, looks up from his bottle
                    * of whiskey with a pleased expression.</I><BR><BR>
                    * 
                    * Well looky here!  I didn't think a landlubber like yourself had the pirate
                    * blood in ye!  But look at that!  You certainly look the part now!  Sure
                    * you can still keep on your feet?  Har!<BR><BR>
                    * 
                    * Avast ye, ye loveable pirate!  Ye deserve a belt of better brew than the slop
                    * ye've been drinking, and I've just the thing.<BR><BR>
                    * 
                    * I call it Captain Blackheart's Whiskey, and it'll give ye hairs on yer chest,
                    * that's for sure.  Why, a keg of this stuff once spilled on my ship, and it ate
                    * a hole right through the deck!<BR><BR>
                    * 
                    * Go on, drink up, or use it to clean the rust off your cutlass - it's the best
                    * brew, either way!<BR><BR>
                    * 
                    * <I>Captain Blackheart hands you a jug of his famous Whiskey. You think it best
                    * to return it to the Hag, rather than drink any of the noxious swill.</I>
                    */
                    return 1055011;
                }
            }
        }
        public override void OnRead()
        {
            FindIngredientObjective obj = System.FindObjective(typeof(FindIngredientObjective)) as FindIngredientObjective;

            if (obj != null)
                obj.NextStep();
        }

        public override void ChildDeserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            m_FirstMet = reader.ReadBool();
        }

        public override void ChildSerialize(GenericWriter writer)
        {
            writer.WriteEncodedInt(0); // version

            writer.Write(m_FirstMet);
        }
    }

    public class EndConversation : QuestConversation
    {
        public override object Message =>
                /* <I>The horrible wretch of a witch looks up from her vile experiments
* and focuses her one good eye on you.</I><BR><BR>
* 
* Eh?  What's that?  You say you've gathered the ingredients for my delicious
* Magic Brew?<BR><BR>
* 
* Well, well, I don't know exactly what to say.  I thought for sure you'd
* end up dead!  Haw!  Can't blame a lady for wishing, can you?  Even if she
* is a bit old and wrinkled.<BR><BR>
* 
* Well, I promised you a reward for your efforts, and I never lie - leastways
* not to someone like you, after the great sacrifices you've made.  You know,
* I could use a new Apprentice, in an official capacity as it were.  I couldn't
* convince you to stay around and help me out some more could I?  There's always
* cauldrons that need cleaning, dung that needs shoveling, newts eye that
* needs a proper chewing, and fires that need stoking.<BR><BR>
* 
* What's that? Not interested?  Well, I suppose you have great things ahead of
* you and all that. Feh! Like a puckish little puke like you could ever make
* something of themselves in this cold old world!<BR><BR>
* 
* Nevertheless, I'll give you your blasted reward, and you'd better be happy
* with it because it's all you're getting.  Caused me enough trouble as it is.
* Here, take it, and be off with you!  It'll be a pleasure to my eye if I
* never have to squint to see you again!  And the stench!  Smells like you
* washed this very morning!  A great fancy folk you are, with your soaps and
* water!  Think you're so great...why, I remember when we didn't even have
* soap, and water was made by tiny little fairies and cost a gold piece for
* a thimbleful...I could tell you some stories, I could...<BR><BR>
* 
* <I>Your reward in hand, you decide to leave the old Hag to her mumblings
* before she realizes you're still around and puts you back to work.</I>
*/
                1055013;
        public override void OnRead()
        {
            System.Complete();
        }
    }

    public class RecentlyFinishedConversation : QuestConversation
    {
        public override object Message =>
                /* <I>The wrinkled old crone stops stirring her noxious stew, looking up at
* you with an annoyed expression on her face.</I><BR><BR>
* 
* You again? Listen, you little wretch, I'm in no mood for any of your meddlesome
* requests. I've work to do, and no time for your whining.<BR><BR>
* 
* Come back later, and maybe I'll have something for you to do. In the meantime,
* get out of my sight - and don't touch anything on your way out!<BR><BR>
* 
* <I>The vile hag hacks up a gob of phlegm, spitting it on the ground before
* returning to her work.</I>
*/
                1055064;
        public override bool Logged => false;
    }
}
