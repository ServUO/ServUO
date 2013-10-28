using System;
using Server.Items;

namespace Server.Engines.Quests.Necro
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
                * <a href = "?ForceTopic31">NPC</a> gives you, will appear in a
                * window such as this one.  You can review the information at
                * any time during your quest.<BR><BR><U>Getting Help</U><BR><BR>
                * 
                * Some of the text you will come across during your quest, will
                * be underlined <a href = "?ForceTopic73">links to the codex of
                * wisdom</a>, or online help system.  You can click on the text
                * to get detailed information on the underlined subject.  You
                * may also access the Codex Of Wisdom by pressing "F1" or by
                * clicking on the "?" on the toolbar at the top of your screen.<BR><BR>
                * 
                * <U>Context Menus</U><BR><BR>Context menus can be called up by
                * single left-clicking (or Shift + single left-click, if you
                * changed it) on most objects or NPCs in the world.  Nearly
                * everything, including your own avatar will have context menus
                * available.  Bringing up your avatar's context menu will give
                * you options to cancel your quest and review various quest
                * information.<BR><BR>
                */
                return 1049092;
            }
        }
        public override void OnRead()
        {
            Container bag = Mardoth.GetNewContainer();

            bag.DropItem(new DarkTidesHorn());

            this.System.From.AddToBackpack(bag);

            this.System.AddConversation(new ReanimateMaabusConversation());
        }
    }

    public class ReanimateMaabusConversation : QuestConversation
    {
        private static readonly QuestItemInfo[] m_Info = new QuestItemInfo[]
        {
            new QuestItemInfo(1026153, 6178), // teleporter
            new QuestItemInfo(1049117, 4036), // Horn of Retreat
            new QuestItemInfo(1048032, 3702)// a bag
        };
        public ReanimateMaabusConversation()
        {
        }

        public override object Message
        {
            get
            {
                /* Excellent choice, young apprentice of evil!<BR><BR>
                * 
                * I will not waste our time with pleasantries.  There is much work
                * to be done – especially in light of the recent Paladin ambushes
                * that we have suffered.  The necromantic brotherhood is working
                * towards the summoning of the elder daemon Kronus, who will rise
                * from the Well of Tears to help us finally crush the Paladin forces
                * that have plagued our lands for so long now.<BR><BR>
                * 
                * To summon Kronus, we must energize the Well of Tears with a series
                * of dark rituals.  Unfortunately the rituals needed to sufficiently
                * energize the Well of Tears have been lost to us.  Your task will be
                * to recover one of the ritual scrolls needed for the summoning.<BR><BR>
                *  
                * You will need to find the corpse of the Arch Necromancer Maabus, which
                * has been laid to rest in the tomb of elders.  We believe his spirit may
                * have memory of where we may find the scrolls needed for the summoning.
                * You will need to awaken him from the slumber of death, using your
                * Animate Dead spell, of course.<BR><BR>
                * 
                * To reach the tomb, step onto the magical teleporter just to the
                * <a href = "?ForceTopic13">West</a> of where I am standing.<BR><BR>
                * 
                * Once you have been teleported, follow the path, which will lead you to
                * the tomb of Maabus.<BR><BR>One more thing before you go:<BR><BR>
                * 
                * Should you get into trouble out there or should you lose your way, do
                * not worry. I have also given you a magical horn - a <I>Horn of Retreat</I>.
                * Play the horn at any time to open a magical gateway that leads back to this
                * tower.<BR><BR>
                * 
                * Should your horn run out of <a href = "?ForceTopic83">charges</a>, simply
                * hand me the horn to have it recharged.<BR><BR>
                * 
                * Good luck friend.
                */
                return 1060099;
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
            this.System.AddObjective(new FindMaabusTombObjective());
        }
    }

    public class MaabasConversation : QuestConversation
    {
        private static readonly QuestItemInfo[] m_Info = new QuestItemInfo[]
        {
            new QuestItemInfo(1026153, 6178)// teleporter
        };
        public MaabasConversation()
        {
        }

        public override object Message
        {
            get
            {
                /* <I>Maabus emits an ear-crawling screech as his body reanimates.
                * He turns and angrily screams at you</I>:<BR><BR>
                * 
                * YOU INFIDEL!  HOW DARE YOU AWAKEN MAABUS!?!<BR><BR>
                * 
                * <I>Maabus continues to scream at you angrily for some time.
                * As he settles down, you explain to him the purpose of your visit.
                * Once you explain that you are on a quest to summon the elder daemon
                * Kronus, Maabus begins to cooperate, and begins to speak in a more
                * reasonable tone</I>:<BR><BR>
                * 
                * Well, why didn’t you say so?  If you’re going to raise Kronus from
                * the Well of Tears, you must first complete a long series of dark
                * rituals.  I once owned one of the scrolls needed for the summoning,
                * but alas it was lost to me when I lost my life to a cowardly Paladin
                * ambush near the Paladin city of Light.  They would have probably
                * hidden the scroll in their precious crystal cave near the city.<BR><BR>
                * 
                * There is a teleporter in the corner of this tomb.  It will transport
                * you near the crystal cave at which I believe one of the calling scrolls
                * is hidden.  Good luck.<BR><BR>
                * 
                * <I>Maabus' body slumps back into the coffinas your magic expires</I>.
                */
                return 1060103;
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
            this.System.AddObjective(new FindCrystalCaveObjective());
        }
    }

    public class HorusConversation : QuestConversation
    {
        public HorusConversation()
        {
        }

        public override object Message
        {
            get
            {
                /* <I>An old man, dressed in slightly tattered armor, whom you recognize
                * to be a Paladin stands before the Crystal Cave staring blankly into
                * the space in front of him.  As he begins to speak to you, you realize
                * this man is blind.  You attempt to persuade the blind man that you are
                * a Paladin seeking to inspect the scroll of calling...</I><BR><BR>
                * 
                * Greetings traveler!<BR><BR>
                * 
                * You seek entrance to the Crystal Cave, home of the Calling Scroll?  Hmm.
                * You reak of death and decay, brother.  You reak of death like a Necromancer,
                * but yet you claim to be a Paladin in hopes that I will grant thee passage
                * into the cave?<BR><BR>
                * 
                * Please don’t think ill of me for this, but I’m just a blind, old man looking
                * to keep the brotherhood of Paladins safe from the clutches of the elder daemon
                * Kronus.  The Necromancers have been after this particular scroll for quite some
                * time, so we must take all the security precautions we can.<BR><BR>
                * 
                * Before I can let you pass into the Crystal Cave, you must speak to me the secret
                * word that is kept in the Scroll of Abraxus in the Vault of Secrets at the Paladin
                * city of Light.  It’s the only way that I can be sure you are who you claim to be,
                * since Necromancers cannot enter the Vault due to powerful protective magic that
                * the brotherhood has blessed the vault with.
                */
                return 1060105;
            }
        }
        public override void OnRead()
        {
            this.System.AddObjective(new FindMardothAboutVaultObjective());
        }
    }

    public class MardothVaultConversation : QuestConversation
    {
        public MardothVaultConversation()
        {
        }

        public override object Message
        {
            get
            {
                /* <I>Mardoth looks at you expectantly until you tell him that you failed
                * to retrieve the scroll...</I><BR><BR>
                * 
                * You failed?  Very unfortunate...  So now you must find your way into
                * the paladin’s Vault of Secrets, eh?  Well, you won't be able to get in
                * – there is a powerful magic aura that protects the Vault from all
                * Necromancers.  We simply cannot enter.  However, that's not to say you
                * familiar spirit can't.<BR><BR>
                * 
                * <I>Mardoth grins with obvious satisfaction
                * as he explains the details of the <a href="?ForceTopic127">Summon
                * Familiar</a> spell to you...</I>, which will allow you to summon a
                * scavenging Horde Minion to steal the scroll.<BR><BR>
                * 
                * Very well.  You are prepared to go.  Take the teleporter just to the
                * <a href = "?ForceTopic13">West</a> of where I am standing to transport
                * to the Paladin city of Light.  Once you have arrived in the city, follow
                * the road of glowing runes to the Vault of Secrets.  You know what to do.
                */
                return 1060107;
            }
        }
        public override void OnRead()
        {
            this.System.AddObjective(new FindCityOfLightObjective());
        }
    }

    public class VaultOfSecretsConversation : QuestConversation
    {
        private static readonly QuestItemInfo[] m_Info = new QuestItemInfo[]
        {
            new QuestItemInfo(1023643, 8787)// spellbook
        };
        public VaultOfSecretsConversation()
        {
        }

        public override object Message
        {
            get
            {
                /* You have arrived in the Vault of Secrets.  You can feel the
                * protective magic in this place restricting you, making you
                * feel nearly claustrophobic.<BR><BR>
                * 
                * Just ahead of you and out of your reach, you see a collection
                * of scrolls and books, one of them being entitled
                * 'Scroll of Abraxus' .  You can only assume that this scroll
                * holds the current password required to enter the Crystal Cave.<BR><BR>
                * 
                * This would be a good opportunity to <a href="?ForceTopic127">summon
                * your familiar</a>.  Since your familiar is not a Necromancer, it
                * will not be affected by the anti-magic aura that surrounds the Vault.<BR><BR>
                * 
                * Summon your familiar with the <a href="?ForceTopic127">Summon Familiar</a> spell.
                */
                return 1060110;
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
            this.System.AddObjective(new FetchAbraxusScrollObjective());
        }
    }

    public class ReadAbraxusScrollConversation : QuestConversation
    {
        public ReadAbraxusScrollConversation()
        {
        }

        public override object Message
        {
            get
            {
                /* You have obtained the Scroll of Abraxus, which contains the secret
                * password needed to gain passage into the Crystal Cave where the
                * Scroll of Calling is kept.  Read the scroll (double click) and
                * figure out the password.<BR><BR>
                * 
                * Once you have the password, return to the Crystal Cave and speak
                * the password to the guard.<BR><BR>
                * 
                * If you do not know the way to the Crystal Cave from the Paladin City,
                * you can use the magic teleporter located just outside of the vault.
                */
                return 1060114;
            }
        }
        public override void OnRead()
        {
            this.System.AddObjective(new ReadAbraxusScrollObjective());
        }
    }

    public class SecondHorusConversation : QuestConversation
    {
        public SecondHorusConversation()
        {
        }

        public override object Message
        {
            get
            {
                /* Very well Paladin, you have proven to me your identity.
                * I grant thee passage.<BR><BR>
                * 
                * Be careful, however – I’ve heard that the cave has been
                * infested with a vermin of some sort.  Our High Lord
                * Melkeer was supposed to send some troops to clear the
                * vermin out of the cave, but that was last week already.
                * I fear that he forgot.<BR><BR>
                * 
                * If you can find it in your goodness to dispose of at
                * least 5 of those vermin in there, I shall reward your
                * efforts.  If however you are too busy, and I would
                * understand if you were, don’t bother with the vermin.<BR><BR>
                * 
                * You may now pass through the energy barrier to enter the
                * Crystal Cave.   Take care honorable Paladin soul.
                * Walk in the light my friend.
                */
                return 1060118;
            }
        }
        public override void OnRead()
        {
            this.System.AddObjective(new FindCallingScrollObjective());
        }
    }

    public class HealConversation : QuestConversation
    {
        public HealConversation()
        {
        }

        public override object Message
        {
            get
            {
                /* You've just slain a creature.  Now is a good time to learn how
                * to heal yourself as a Necromancer.<BR><BR>
                * 
                * As a follower of the dark path, you are able to recover lost
                * hitpoints by communing with the spirit world via the skill
                * <a href="?ForceTopic133">Spirit Speak</a>.  Learn more about it now,
                * <a href="?ForceTopic73">in the codex of Wisdom</a>.
                */
                return 1061610;
            }
        }
    }

    public class HorusRewardConversation : QuestConversation
    {
        public HorusRewardConversation()
        {
        }

        public override object Message
        {
            get
            {
                /* I thank you for going out of your way to clean out some
                * of the vermin in that cave – here is your reward: a bag
                * containing 500 gold coins plus a strange and magical artifact
                * that should come in handy in your travels.<BR><BR>
                * 
                * Take care young Paladin!
                */
                return 1060717;
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

    public class LostCallingScrollConversation : QuestConversation
    {
        private bool m_FromMardoth;
        public LostCallingScrollConversation(bool fromMardoth)
        {
            this.m_FromMardoth = fromMardoth;
        }

        // Serialization
        public LostCallingScrollConversation()
        {
        }

        public override object Message
        {
            get
            {
                if (this.m_FromMardoth)
                {
                    /* You return without the scroll of Calling?  I'm afraid that
                    * won't do.  You must return to the Crystal Cave and fetch
                    * another scroll.  Use the teleporter to the West of me to
                    * get there.  Return here when you have the scroll.  Do not 
                    * fail me this time, young apprentice of evil.
                    */
                    return 1062058;
                }
                else // from well of tears
                {
                    /* You have arrived at the well, but no longer have the scroll
                    * of calling.  Use Mardoth's teleporter to return to the
                    * Crystal Cave and fetch another scroll from the box.
                    */
                    return 1060129;
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

            this.m_FromMardoth = reader.ReadBool();
        }

        public override void ChildSerialize(GenericWriter writer)
        {
            writer.WriteEncodedInt((int)0); // version

            writer.Write((bool)this.m_FromMardoth);
        }
    }

    public class MardothKronusConversation : QuestConversation
    {
        public MardothKronusConversation()
        {
        }

        public override object Message
        {
            get
            {
                /* You have returned with the scroll!  I knew I could count on you.
                * You can now perform the rite of calling at the Well of Tears.
                * This ritual will help charge the Well to prepare for the coming
                * of Kronus.   You are prepared to do your part young Necromancer!<BR><BR>
                * 
                * Just outside of this tower, you will find a path lined with red
                * lanterns.  Follow this path to get to the Well of Tears.  Once
                * you have arrived at the Well, use the scroll to perform the
                * ritual of calling.  Performing the rite will empower the well
                * and bring us that much closer to the arrival of Kronus.<BR><BR>
                * 
                * Once you have completed the ritual, return here for your
                * promised reward.
                */
                return 1060121;
            }
        }
        public override void OnRead()
        {
            this.System.AddObjective(new FindWellOfTearsObjective());
        }
    }

    public class MardothEndConversation : QuestConversation
    {
        public MardothEndConversation()
        {
        }

        public override object Message
        {
            get
            {
                /* You have done as I asked... I knew I could count on you from
                * the moment you walked in here!<BR><BR>
                * 
                * The forces of evil are strong within you.  You will become
                * a great Necromancer in this life - perhaps even the greatest.<BR><BR>
                * 
                * My work for you is done here.  I release you from my service
                * to go into the world and fight for our cause...<BR><BR>
                * 
                * Oh...I almost forgot - your reward.  Here is a magical
                * weapon and 2000 gold for you, in the form of a check. Don't
                * spend it all in one place though, eh?<BR><BR>
                * 
                * Actually, before you can spend any of it at all, you will
                * have to <a href="?ForceTopic86">cash the check</a> at the
                * nearest bank.  Shopkeepers never accept checks for payment,
                * they require cash.<BR><BR>
                * 
                * In your pack, you will find an enchanted sextant.  Use this
                * sextant to guide you to the nearest bank.<BR><BR>
                * 
                * Farewell, and stay true to the ways of the shadow...
                */
                return 1060133;
            }
        }
        public override void OnRead()
        {
            this.System.AddObjective(new FindBankObjective());
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
                /* If you are leaving the tower, you should learn about the Radar Map.<BR><BR>
                * 
                * The Radar Map (or Overhead View) can be opened by pressing 'ALT-R'
                * on your keyboard. It shows your immediate surroundings from a bird's
                * eye view.<BR><BR>Pressing ALT-R twice, will enlarge the Radar Map a
                * little. Use the Radar Map often as you travel throughout the world
                * to familiarize yourself with your surroundings.
                */
                return 1061692;
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