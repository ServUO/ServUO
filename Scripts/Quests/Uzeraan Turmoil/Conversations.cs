using System;

namespace Server.Engines.Quests.Haven
{
    public class AcceptConversation : QuestConversation
    {
        public AcceptConversation()
        {
        }

        public override object Message
        {
            get
            {
                /* <I><U>Important Quest Information</U></I><BR><BR>
                * 
                * During your quest, any important information that a
                * <a href = "?ForceTopic31">NPC</a> gives you, will appear
                * in a window such as this one.  You can review the information
                * at any time during your quest.<BR><BR>
                * 
                * <U>Getting Help</U><BR><BR>
                * 
                * Some of the text you will come across during your quest,
                * will be underlined <a href = "?ForceTopic73">links to the codex of wisdom</a>,
                * or online help system.  You can click on the text to get detailed
                * information on the underlined subject.  You may also access the
                * Codex Of Wisdom by pressing "F1" or by clicking on the "?" on the toolbar
                * at the top of your screen.<BR><BR>
                * 
                * <U>Context Menus</U><BR><BR>
                * 
                * Context menus can be called up by single left-clicking
                * (or Shift + single left-click, if you changed it) on most objects
                * or NPCs in the world.  Nearly everything, including your own avatar
                * will have context menus available.  Bringing up your avatar's
                * context menu will give you options to cancel your quest and review
                * various quest information.<BR><BR>
                */
                return 1049092;
            }
        }
        public override void OnRead()
        {
            this.System.AddObjective(new FindUzeraanBeginObjective());
        }
    }

    public class UzeraanTitheConversation : QuestConversation
    {
        public UzeraanTitheConversation()
        {
        }

        public override object Message
        {
            get
            {
                /* <I>Uzeraan greets you as you approach...</I><BR><BR>
                * 
                * Greetings traveler!<BR><BR>
                * 
                * I am Uzeraan, the lord of this house and overseer of this fine city, Haven.
                * I know we have just met, but time is short and we need to reinforce
                * the troops in the mountain pass, so I will not waste your time
                * with pleasantries.<BR><BR>
                * 
                * We have been trying to fight back the wicked <I>Horde Minions</I> which
                * have recently begun attacking our cities - but to no avail.
                * We desperately need help!<BR><BR>
                * 
                * Your first task will be to assess the situation in the mountain pass,
                * and help our troops defeat the Horde Minions there.<BR><BR>
                * 
                * Before I send you into battle however, it is time that I teach you a thing
                * or two about the way of the Paladin.<BR><BR>
                * 
                * <U>The Paladin</U><BR><BR>
                * 
                * Paladins are the holy warriors of the realm who have dedicated themselves
                * as protectors of the virtues and vanquishers of all that is evil.<BR><BR>
                * 
                * Paladins have several <a href = "?ForceTopic111">special abilities</a> that
                * are not available to the ordinary warrior. Due to the spiritual nature of
                * these abilities, the Paladin requires some amount of mana to activate them.
                * In addition to mana, the Paladin is also required to spend a certain amount
                * of <a href = "?ForceTopic109">tithing points</a> each time a special ability
                * is used.<BR><BR>
                * 
                * Tithing points and mana are automatically consumed when a special Paladin
                * ability is activated. All Paladin abilities are activated through the
                * <a href = "?ForceTopic114">Book of Chivalry</a><BR><BR>
                * 
                * Go now, to the shrine just East of here, just before the doors and
                * <a href = "?ForceTopic109">tithe</a> at least 500 gold.<BR><BR>
                * 
                * Return here when you are done.
                */
                return 1060209;
            }
        }
        public override void OnRead()
        {
            this.System.AddObjective(new TitheGoldObjective());
        }
    }

    public class UzeraanFirstTaskConversation : QuestConversation
    {
        private static readonly QuestItemInfo[] m_Info = new QuestItemInfo[]
        {
            new QuestItemInfo(1023676, 0xE68)// glowing rune
        };
        public UzeraanFirstTaskConversation()
        {
        }

        public override object Message
        {
            get
            {
                if (this.System.From.Profession == 1) // warrior
                {
                    /* <I>Uzeraan greets you as you approach...</I><BR><BR>
                    * 
                    * Greetings traveler!<BR><BR>
                    * 
                    * I am Uzeraan, the lord of this house and overseer of this fine city, Haven.
                    * I know we have just met, but time is short and we need to reinforce the troops
                    * in the mountain pass, so I will not waste your time with pleasantries.<BR><BR>
                    * 
                    * We have been trying to fight back the wicked <I>Horde Minions</I> which have
                    * recently begun attacking our cities - but to no avail.  We desperately need
                    * help!<BR><BR>
                    * 
                    * Your first task will be to assess the situation in the mountain pass, and
                    * help our troops defeat the Horde Minions there.<BR><BR>
                    * 
                    * Take the road marked with glowing runes, that starts just outside of this
                    * mansion. Before you go into battle, it would be prudent to
                    * <a href="?ForceTopic27">review combat techniques</a> as well as
                    * <a href = "?ForceTopic29">information on healing yourself</a>.<BR><BR>
                    * 
                    * To aid you in your fight, you may also wish to
                    * <a href = "?ForceTopic33">purchase equipment</a> from Frank the Blacksmith,
                    * who is standing just <a href = "?ForceTopic13">South</a> of here.<BR><BR>
                    * 
                    * Good luck young warrior.
                    */
                    return 1049088;
                }
                else if (this.System.From.Profession == 2) // magician
                {
                    /* <I>Uzeraan greets you as you approach...</I><BR><BR>
                    * 
                    * Greetings traveler!<BR><BR>
                    * 
                    * I am Uzeraan, the lord of this house and overseer of this fine city, Haven.
                    * I know we have just met, but time is short and we need to reinforce
                    * the troops in the mountain pass, so I will not waste your time with
                    * pleasantries.<BR><BR>
                    * 
                    * We have been trying to fight back the wicked <I>Horde Minions</I> which have
                    * recently begun attacking our cities - but to no avail.  We desperately
                    * need help!<BR><BR>
                    * 
                    * Your first task will be to assess the situation in the mountain pass,
                    * and help our troops defeat the Horde Minions there.<BR><BR>
                    * 
                    * Take the road marked with glowing runes, that starts just outside of this
                    * mansion. Before you go into battle, it would be prudent to
                    * <a href="?ForceTopic35">review your magic skills</a> as well as
                    * <a href = "?ForceTopic29">information on healing yourself</a>.<BR><BR>
                    * 
                    * To aid you in your fight, you may also wish to
                    * <a href = "?ForceTopic33">purchase equipment</a> from Frank the Blacksmith,
                    * who is standing just <a href = "?ForceTopic13">South</a> of here.<BR><BR>
                    * 
                    * Good luck young mage.
                    */
                    return 1049386;
                }
                else
                {
                    /* <I>Uzeraan nods at you with approval and begins to speak...</I><BR><BR>
                    * 
                    * Now that you are ready, let me give you your first task.<BR><BR>
                    * 
                    * As I mentioned earlier, we have been trying to fight back the wicked
                    * <I>Horde Minions</I> which have recently begun attacking our cities
                    * - but to no avail. Our need is great!<BR><BR>
                    * 
                    * Your first task will be to assess the situation in the mountain pass,
                    * and help our troops defeat the Horde Minions there.<BR><BR>
                    * 
                    * Take the road marked with glowing runes, that starts just outside of this mansion.
                    * Before you go into battle, it would be prudent to
                    * <a href="?ForceTopic27">review combat techniques</a> as well as
                    * <a href = "?ForceTopic29">information on healing yourself,
                    * using your Paladin ability 'Close Wounds'</a>.<BR><BR>
                    * 
                    * To aid you in your fight, you may also wish to
                    * <a href = "?ForceTopic33">purchase equipment</a> from Frank the Blacksmith,
                    * who is standing just <a href = "?ForceTopic13">South</a> of here.<BR><BR>
                    * 
                    * Good luck young Paladin!
                    */
                    return 1060388;
                }
            }
        }
        public override QuestItemInfo[] Info
        {
            get
            {
                return m_Info;
            }
        }
        public override void OnRead()
        {
            this.System.AddObjective(new KillHordeMinionsObjective());
        }
    }

    public class UzeraanReportConversation : QuestConversation
    {
        private static readonly QuestItemInfo[] m_Info = new QuestItemInfo[]
        {
            new QuestItemInfo(1026153, 0x1822), // teleporter
            new QuestItemInfo(1048032, 0xE76)// a bag
        };
        public UzeraanReportConversation()
        {
        }

        public override object Message
        {
            get
            {
                if (this.System.From.Profession == 2) // magician
                {
                    /* <I>You give your report to Uzeraan and after a while, he begins to
                    * speak...</I><BR><BR>
                    * 
                    * Your report is grim,  but all hope is not lost!  It has become apparent
                    * that our swords and spells will not drive the evil from Haven.<BR><BR>
                    * 
                    * The head of my order, the High Mage Schmendrick, arrived here shortly after
                    * you went into battle with the <I>Horde Minions</I>.  He has brought with him a
                    * scroll of great power, that should aid us greatly in our battle.<BR><BR>
                    * 
                    * Unfortunately, the entrance to one of our mining caves collapsed recently,
                    * trapping our miners inside.<BR><BR>
                    * 
                    * Schmendrick went to install magical teleporters inside the mines so that
                    * the miners would have a way out.  The miners have since returned, but Schmendrick has not.
                    * Those who have returned, all seem to have lost their minds to madness;
                    * mumbling strange things of "the souls of the dead seeking revenge".<BR><BR>
                    * 
                    * No matter. We must find Schmendrick.<BR><BR>
                    * 
                    * Step onto the teleporter, located against the wall, and seek Schmendrick in the mines.<BR><BR>
                    * 
                    * I've given you a bag with some <a href="?ForceTopic93">Travel Spells</a>,
                    * in case you need to make a quick escape. In addition, you may wish to cast
                    * the <a href="?ForceTopic92">Night Sight</a> spell on yourself before going
                    * into the cave, as it it's pretty dark in there.<BR><BR>
                    * 
                    * Now please go. Good luck, friend.
                    */
                    return 1049387;
                }
                else
                {
                    /* <I>You give your report to Uzeraan and after a while,
                    * he begins to speak...</I><BR><BR>
                    * 
                    * Your report is grim, but all hope is not lost!  It has become apparent
                    * that our swords and spells will not drive the evil from Haven.<BR><BR>
                    * 
                    * The head of my order, the High Mage Schmendrick, arrived here shortly after
                    * you went into battle with the <I>Horde Minions</I>.  He has brought with him a
                    * scroll of great power, that should aid us greatly in our battle.<BR><BR>
                    * 
                    * Unfortunately, the entrance to one of our mining caves collapsed recently,
                    * trapping our miners inside.<BR><BR>
                    * 
                    * Schmendrick went to install magical teleporters inside the mines so that
                    * the miners would have a way out.  The miners have since returned, but Schmendrick has not.
                    * Those who have returned, all seem to have lost their minds to madness;
                    * mumbling strange things of "the souls of the dead seeking revenge".<BR><BR>
                    * 
                    * No matter. We must find Schmendrick.<BR><BR>
                    * 
                    * Step onto the teleporter, located against the wall, and seek Schmendrick in the mines.<BR><BR>
                    * 
                    * I've given you a bag with some <a href="?ForceTopic75">Night Sight</a>
                    * and <a href="?ForceTopic76">Healing</a> <a href="?ForceTopic74">potions</a>
                    * to help you out along the way.  Good luck.
                    */
                    return 1049119;
                }
            }
        }
        public override QuestItemInfo[] Info
        {
            get
            {
                return m_Info;
            }
        }
        public override void OnRead()
        {
            this.System.AddObjective(new FindSchmendrickObjective());
        }
    }

    public class SchmendrickConversation : QuestConversation
    {
        private static readonly QuestItemInfo[] m_Info = new QuestItemInfo[]
        {
            new QuestItemInfo(1023637, 0xE34)// scroll
        };
        public SchmendrickConversation()
        {
        }

        public override object Message
        {
            get
            {
                if (this.System.From.Profession == 5) // paladin
                {
                    /* <I>Schmendrick barely pays you any attention as you approach him.
                    * His mind seems to be occupied with something else. You explain to him that
                    * you came for the scroll of power and after a long while he begins to speak,
                    * but apparently still not giving you his full attention...</I><BR><BR>
                    * 
                    * Hmmm.. peculiar indeed. Very strange activity here indeed... I wonder...<BR><BR>
                    * 
                    * Hmmm. Oh yes! Scroll, you say? I don't have it, sorry. My apprentice
                    * was carrying it, and he ran off to somewhere in this cave. Find him and
                    * you will find the scroll.<BR><BR>
                    * 
                    * Be sure to bring the scroll to Uzeraan once you have it. He's the only person
                    * aside from myself who can read the ancient markings on the scroll.
                    * I need to figure out what's going on down here before I can leave.
                    * Strange activity indeed...<BR><BR>
                    * 
                    * One more thing...<BR><BR>
                    * 
                    * Be careful of the restless souls wandering about. They seem to be in the habit
                    * of spontaneously attacking people.  Perhaps using your paladin ability
                    * <a href="?ForceTopic104">Enemy of One</a> might help you overcome the perils
                    * of these halls.<BR><BR>
                    * 
                    * <I>Schmendrick goes back to his work and you seem to completely fade from his awareness...
                    */
                    return 1060749;
                }
                else
                {
                    /* <I>Schmendrick barely pays you any attention as you approach him.  His
                    * mind seems to be occupied with something else.  You explain to him that
                    * you came for the scroll of power and after a long while he begins to speak,
                    * but apparently still not giving you his full attention...</I><BR><BR>
                    * 
                    * Hmmm.. peculiar indeed.  Very strange activity here indeed... I wonder...<BR><BR>
                    * 
                    * Hmmm.  Oh yes! Scroll, you say?  I don't have it, sorry. My apprentice was
                    * carrying it, and he ran off to somewhere in this cave.  Find him and you will
                    * find the scroll.<BR><BR>Be sure to bring the scroll to Uzeraan once you
                    * have it. He's the only person aside from myself who can read the ancient
                    * markings on the scroll.  I need to figure out what's going on down here before
                    * I can leave.  Strange activity indeed...<BR><BR>
                    * 
                    * <I>Schmendrick goes back to his work and you seem to completely fade from his
                    * awareness...
                    */
                    return 1049322;
                }
            }
        }
        public override QuestItemInfo[] Info
        {
            get
            {
                return m_Info;
            }
        }
        public override void OnRead()
        {
            this.System.AddObjective(new FindApprenticeObjective());
        }
    }

    public class UzeraanScrollOfPowerConversation : QuestConversation
    {
        private static readonly QuestItemInfo[] m_Info = new QuestItemInfo[]
        {
            new QuestItemInfo(1048030, 0x14EB), // a Treasure Map
            new QuestItemInfo(1023969, 0xF81), // Fertile Dirt
            new QuestItemInfo(1049117, 0xFC4)// Horn of Retreat
        };
        public UzeraanScrollOfPowerConversation()
        {
        }

        public override object Message
        {
            get
            {
                /* <I>Uzeraan carefully unravels the scroll and begins to read...
                * after a short while his face lights up with a smile and he speaks to you...</I><BR><BR>
                * 
                * This is wonderful, friend!  For your troubles I've given you a treasure map
                * I had laying about, along with a shovel to <a href = "?ForceTopic91">dig up the treasure</a>.
                * Feel free to find the treasure at your leisure.<BR><BR>
                * 
                * Now let us get back to the business of this scroll. The only trouble,
                * is that this scroll calls for some special ingredients that I do
                * not have on hand.<BR><BR>
                * 
                * Though it may involve some danger, I will ask of you to find
                * these reagents for me.  <BR><BR>
                * 
                * There are three reagents I need to complete the spell.<BR><BR>
                * 
                * The first thing I need is some <I>Fertile Dirt</I>.<BR><BR>
                * 
                * There lives a Dryad on this island who I know would have such a thing on hand.
                * I have recalibrated the teleporter to transport you to the Dryad's grove,
                * which lies <a href = "?ForceTopic13">South-West</a> of this mansion.<BR><BR>
                * 
                * Tell her Uzeraan sent you, and she should cooperate.<BR><BR>
                * 
                * Should you get into trouble out there or should you lose your way, do not worry.
                * I have also given you a magical horn - a <I>Horn of Retreat</I>.
                * Play the horn at any time to open a magical gateway that leads back to this mansion.<BR><BR>
                * 
                * Should your horn run out of <a href = "?ForceTopic83">charges</a>,
                * simply hand me or any of my mansion guards the horn to have it recharged.<BR><BR>
                * 
                * Good luck friend.
                */
                return 1049325;
            }
        }
        public override QuestItemInfo[] Info
        {
            get
            {
                return m_Info;
            }
        }
        public override void OnRead()
        {
            this.System.AddObjective(new FindDryadObjective());
        }
    }

    public class DryadConversation : QuestConversation
    {
        public DryadConversation()
        {
        }

        public override object Message
        {
            get
            {
                /* <I>The Dryad watches hungrily as you approach, giving you an
                * uneasy feeling in the pit of your stomach.  You explain that
                * Uzeraan has sent you for a quantity of fertile dirt.  With a wide grin
                * and in a slightly hoarse voice she replies...</I><BR><BR>
                * 
                * <I>Fertile Dirt</I>, eh?  Well, I have a few patches here...but what have
                * you brought me in return?  Came empty-handed did you?  That's unfortunate
                * indeed... but since you were sent by my dear friend Uzeraan, I supposed
                * I could oblige you.<BR><BR>
                * 
                * <I>The Dryad digs around in the ground and hands you a patch of Fertile Dirt.<BR><BR>
                * 
                * With a smile she goes back to her work...</I>
                */
                return 1049326;
            }
        }
        public override void OnRead()
        {
            this.System.AddObjective(new ReturnFertileDirtObjective());
        }
    }

    public class UzeraanFertileDirtConversation : QuestConversation
    {
        private static readonly QuestItemInfo[] m_Info = new QuestItemInfo[]
        {
            new QuestItemInfo(1023965, 0xF7D), // Daemon Blood
            new QuestItemInfo(1022581, 0xA22), // lantern
        };
        public UzeraanFertileDirtConversation()
        {
        }

        public override object Message
        {
            get
            {
                if (this.System.From.Profession == 2) // magician
                {
                    /* <I>Uzeraan takes the dirt from you and smiles...<BR><BR></I>
                    * 
                    * Wonderful!  I knew I could count on you.  As a token of my appreciation
                    * I've given you a bag with some <a href = "?ForceTopic37">reagents</a>
                    * as well as some <a href="?ForceTopic35">spell scrolls</a>.  They should
                    * help out a bit.<BR><BR>
                    * 
                    * The next item I need is a <I>Vial of Blood</I>.  I know it seems strange,
                    * but that's what the formula asks for.  I have some locked away in a chest
                    * not far from here.  It's only a short distance from the mansion.  Let me
                    * give you directions...<BR><BR>Exit the front door to the East.  Then follow
                    * the path to the North.  You will pass by several pedestals with lanterns on
                    * them.  Continue on this path until you run into a small hut.  Walk up the
                    * stairs and through the door.  Inside you will find a chest.  Open it and
                    * bring me a <I>Vial of Blood</I> from inside the chest.  It's very easy to find.
                    * Just follow the road and you can't miss it.<BR><BR>
                    * 
                    * Good luck!
                    */
                    return 1049388;
                }
                else
                {
                    /* <I>Uzeraan takes the dirt from you and smiles...<BR><BR></I>
                    * 
                    * Wonderful!  I knew I could count on you.  As a token of my appreciation
                    * I've given you a bag with some bandages as well as some healing potions.
                    * They should help out a bit.<BR><BR>
                    * 
                    * The next item I need is a <I>Vial of Blood</I>.  I know it seems strange,
                    * but that's what the formula asks for.  I have some locked away in a chest
                    * not far from here.  It's only a short distance from the mansion.  Let me give
                    * you directions...<BR><BR>
                    * 
                    * Exit the front door to the East.  Then follow the path to the North.
                    * You will pass by several pedestals with lanterns on them.  Continue on this
                    * path until you run into a small hut.  Walk up the stairs and through the door.
                    * Inside you will find a chest.  Open it and bring me a <I>Vial of Blood</I>
                    * from inside the chest.  It's very easy to find.  Just follow the road and you
                    * can't miss it.<BR><BR>
                    * 
                    * Good luck!
                    */
                    return 1049329;
                }
            }
        }
        public override QuestItemInfo[] Info
        {
            get
            {
                return m_Info;
            }
        }
        public override void OnRead()
        {
            this.System.AddObjective(new GetDaemonBloodObjective());
        }
    }

    public class UzeraanDaemonBloodConversation : QuestConversation
    {
        private static readonly QuestItemInfo[] m_Info = new QuestItemInfo[]
        {
            new QuestItemInfo(1017412, 0xF80), // Daemon Bone
        };
        private static readonly QuestItemInfo[] m_InfoPaladin = new QuestItemInfo[]
        {
            new QuestItemInfo(1017412, 0xF80), // Daemon Bone
            new QuestItemInfo(1060577, 0x1F14), // Recall Rune
        };
        public UzeraanDaemonBloodConversation()
        {
        }

        public override object Message
        {
            get
            {
                if (this.System.From.Profession == 2) // magician
                {
                    //return 1049389; // localized message is bugged
                    return "<I>You hand Uzeraan the Vial of Blood, which he hastily accepts...</I><BR>" +
                           "<BR>" +
                           "Excellent work!  Only one reagent remains and the spell is complete!  The final " +
                           "requirement is a <I>Daemon Bone</I>, which will not be as easily acquired as the " +
                           "previous two components.<BR>" +
                           "<BR>" +
                           "There is a haunted graveyard on this island, which is the home to many undead " +
                           "creatures.   Dispose of the undead as you see fit.  Be sure to search their remains " +
                           "after you have smitten them, to check for a <I>Daemon Bone</I>.  I'm quite sure " +
                           "that you will find what we seek, if you are thorough enough with your " +
                           "extermination.<BR>" +
                           "<BR>" +
                           "Take these explosion spell scrolls and  magical wizard's hat to aid you in your " +
                           "battle.  The scrolls should help you make short work of the undead.<BR>" +
                           "<BR>" +
                           "Return here when you have found a <I>Daemon Bone</I>.";
                }
                else
                {
                    /* <I>You hand Uzeraan the Vial of Blood, which he hastily accepts...</I><BR><BR>
                    * 
                    * Excellent work!  Only one reagent remains and the spell is complete!
                    * The final requirement is a <I>Daemon Bone</I>, which will not be as easily
                    * acquired as the previous two components.<BR><BR>
                    * 
                    * There is a haunted graveyard on this island, which is the home to many
                    * undead creatures.   Dispose of the undead as you see fit.  Be sure to search
                    * their remains after you have smitten them, to check for a <I>Daemon Bone</I>.
                    * I'm quite sure that you will find what we seek, if you are thorough enough
                    * with your extermination.<BR><BR>
                    * 
                    * Take this magical silver sword to aid you in your battle.  Silver weapons
                    * will damage the undead twice as much as your regular weapon.<BR><BR>
                    * 
                    * Return here when you have found a <I>Daemon Bone</I>.
                    */
                    return 1049333;
                }
            }
        }
        public override QuestItemInfo[] Info
        {
            get
            {
                if (this.System.From.Profession == 5) // paladin
                    return m_InfoPaladin;
                else
                    return m_Info;
            }
        }
        public override void OnRead()
        {
            this.System.AddObjective(new GetDaemonBoneObjective());
        }
    }

    public class UzeraanDaemonBoneConversation : QuestConversation
    {
        public UzeraanDaemonBoneConversation()
        {
        }

        public override object Message
        {
            get
            {
                /* <I>As you hand Uzeraan the final reagent, he nods at you with approval
                * and starts searching through the pockets of his robe...<BR><BR>
                * 
                * After a short while he hands you a small pouch...</I><BR><BR>
                * 
                * There you are.  Your contract of employment with me has expired and so here
                * is your pay.  2000 gold in the form of a check and a magical sextant that
                * will help you find <a href = "?ForceTopic47">Moongates</a> and Banks.<BR><BR>
                * 
                * Before you can actually spend the money I have given you, however, you must
                * <a href="?ForceTopic86">cash the check</a>.<BR><BR>
                * 
                * I have recalibrated the teleporter to take you to the Haven
                * <a href="?ForceTopic38">Bank</a>.  Step onto the teleporter to be taken
                * to the bank, which lies <a href = "?ForceTopic13">South-East</a> of here.<BR><BR>
                * 
                * Thank you for all your help friend.  I hope we shall meet
                * each other again in the future.<BR><BR>
                * 
                * Farewell.
                */
                return 1049335;
            }
        }
        public override void OnRead()
        {
            this.System.AddObjective(new CashBankCheckObjective());
        }
    }

    public class BankerConversation : QuestConversation
    {
        public BankerConversation()
        {
        }

        public override object Message
        {
            get
            {
                /* <I>The banker smiles at you and greets you in a loud and robust voice...</I><BR><BR>
                * 
                * Well hello there adventurer! I see you've learned how to cash checks. Wonderful!
                * Let me tell you a bit about the banks in this world...<BR><BR>
                * 
                * Anything that you place into any bank box, can be retrieved from any other
                * bank box in the land. For instance, if you place an item into a bank box in
                * Britain, it can be retrieved from your bank box in Moonglow or any other city.<BR><BR>
                * 
                * Bank boxes are very secure. So secure, in fact, that no one can ever get into
                * your bank box except for yourself. Security is hard to come by these days,
                * but you can trust in the banking system of Britannia! We shall not let you down!<BR><BR>
                * 
                * I hope to be seeing much more of you as your riches grow! May your bank box overflow
                * with the spoils of your adventures.<BR><BR>Farewell adventurer, you are now free to
                * explore the world on your own.
                */
                return 1060137;
            }
        }
        public override void OnRead()
        {
            this.System.Complete();
        }
    }

    public class RadarConversation : QuestConversation
    {
        public RadarConversation()
        {
        }

        public override object Message
        {
            get
            {
                /* If you are leaving the mansion, you should learn about the Radar Map.<BR><BR>
                * 
                * The Radar Map (or Overhead View) can be opened by pressing 'ALT-R' on your
                * keyboard. It shows your immediate surroundings from a bird's eye view.<BR><BR>
                * 
                * Pressing ALT-R twice, will enlarge the Radar Map a little.  Use the Radar Map
                * often as you travel throughout the world to familiarize yourself with your surroundings.
                */
                return 1049660;
            }
        }
        public override bool Logged
        {
            get
            {
                return false;
            }
        }
    }

    public class LostScrollOfPowerConversation : QuestConversation
    {
        private bool m_FromUzeraan;
        public LostScrollOfPowerConversation(bool fromUzeraan)
        {
            this.m_FromUzeraan = fromUzeraan;
        }

        public LostScrollOfPowerConversation()
        {
        }

        public override object Message
        {
            get
            {
                if (this.m_FromUzeraan)
                {
                    /* You return without the scroll???<BR><BR>
                    * 
                    * All hope is lost without it, friend.  Return to the mines and talk to
                    * Schmendrick to see if he can help us out of this predicament.
                    */
                    return 1049377;
                }
                else
                {
                    /* You've lost the scroll?  Argh!  I will have to try and re-construct
                    * the scroll from memory.  Bring me a blank scroll, which you can
                    * <a href = "?ForceTopic33">purchase from the mage shop</a> just
                    * <a href = "?ForceTopic13">East</a> of Uzeraan's mansion in Haven.<BR><BR>
                    * 
                    * Return the scroll to me and I will try to make another scroll for you.<BR><BR>
                    * 
                    * When you return, be sure to hand me the scroll (drag and drop).
                    */
                    return 1049345;
                }
            }
        }
        public override bool Logged
        {
            get
            {
                return false;
            }
        }
        public override void ChildDeserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            this.m_FromUzeraan = reader.ReadBool();
        }

        public override void ChildSerialize(GenericWriter writer)
        {
            writer.WriteEncodedInt((int)0); // version

            writer.Write((bool)this.m_FromUzeraan);
        }
    }

    public class LostFertileDirtConversation : QuestConversation
    {
        private bool m_FromUzeraan;
        public LostFertileDirtConversation(bool fromUzeraan)
        {
            this.m_FromUzeraan = fromUzeraan;
        }

        public LostFertileDirtConversation()
        {
        }

        public override object Message
        {
            get
            {
                if (this.m_FromUzeraan)
                {
                    /* You return without <I>Fertile Dirt</I>?  It is imperative that we
                    * get all of the ingredients friend.<BR><BR>
                    * 
                    * Seek out the Dryad and ask her to help you again.
                    */
                    return 1049374;
                }
                else
                {
                    /* You've lost the dirt I gave you?<BR><BR>
                    * 
                    * My, my, my... What ever shall we do now?<BR><BR>
                    * 
                    * I can try to make you some more, but I will need something
                    * that I can transform.  Bring me an <I>apple</I>, and I shall
                    * see what I can do.<BR><BR>
                    * 
                    * You can <a href = "?ForceTopic33">buy</a> apples from the
                    * Provisioner's Shop, which is located a ways <a href = "?ForceTopic13">East</a>
                    * of Uzeraan's mansion.<BR><BR>
                    * 
                    * Hand me the apple when you have it, and I shall see about transforming
                    * it for you.<BR><BR>
                    * 
                    * Good luck.<BR><BR>
                    */
                    return 1049359;
                }
            }
        }
        public override bool Logged
        {
            get
            {
                return false;
            }
        }
        public override void ChildDeserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            this.m_FromUzeraan = reader.ReadBool();
        }

        public override void ChildSerialize(GenericWriter writer)
        {
            writer.WriteEncodedInt((int)0); // version

            writer.Write((bool)this.m_FromUzeraan);
        }
    }

    public class DryadAppleConversation : QuestConversation
    {
        public DryadAppleConversation()
        {
        }

        public override object Message
        {
            get
            {
                /* <I>The Dryad sticks the apple into the ground and you watch it
                * rot before your eyes.<BR><BR>
                * 
                * She pulls the now fertile dirt out of the ground and hands
                * it to you.</I><BR><BR>
                * 
                * There you go friend.  Try not to lose it again this time, eh?
                */
                return 1049360;
            }
        }
        public override bool Logged
        {
            get
            {
                return false;
            }
        }
    }

    public class LostDaemonBloodConversation : QuestConversation
    {
        public LostDaemonBloodConversation()
        {
        }

        public override object Message
        {
            get
            {
                /* You return without <I>a Vial of Blood</I>?  It is imperative that we
                * get all of the ingredients friend.<BR><BR>
                * 
                * Go back to the chest and fetch another vial.  Please hurry.
                */
                return 1049375;
            }
        }
        public override bool Logged
        {
            get
            {
                return false;
            }
        }
    }

    public class LostDaemonBoneConversation : QuestConversation
    {
        public LostDaemonBoneConversation()
        {
        }

        public override object Message
        {
            get
            {
                /* You return without <I>a Daemon Bone</I>?  It is imperative that we
                * get all of the ingredients friend.<BR><BR>
                * 
                * Go back to the graveyard and continue hunting the undead until you
                * find another one.  Please hurry.
                */
                return 1049376;
            }
        }
        public override bool Logged
        {
            get
            {
                return false;
            }
        }
    }

    public class FewReagentsConversation : QuestConversation
    {
        public FewReagentsConversation()
        {
        }

        public override object Message
        {
            get
            {
                /* I don't feel comfortable sending you into a potentially dangerous situation
                * with as few <a href = "?ForceTopic37">reagents</a> as you have in your pack.<BR><BR>
                * 
                * Before going on, please acquire at least 30 of each reagent.  You can
                * <a href ="?ForceTopic33">purchase</a> reagents from the Mage shop, which is
                * located just <a href ="?ForceTopic13">East</a> this mansion.<BR><BR>
                * 
                * Remember that there are eight (8) different reagents: Black Pearl, Mandrake Root,
                * Sulfurous Ash, Garlic, Ginseng, Blood Moss, Nightshade and Spider's Silk.<BR><BR>
                * 
                * Come back here when you are ready to go.
                */
                return 1049390;
            }
        }
        public override bool Logged
        {
            get
            {
                return false;
            }
        }
    }
}