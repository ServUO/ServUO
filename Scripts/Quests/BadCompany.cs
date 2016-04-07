/*                                                             .---.
/  .  \
|\_/|   |
|   |  /|
.----------------------------------------------------------------' |
/  .-.                                                              |
|  /   \         Contribute To The Orbsydia SA Project               |
| |\_.  |                                                            |
|\|  | /|                        By Lotar84                          |
| `---' |                                                            |
|       |       (Orbanised by Orb SA Core Development Team)          | 
|       |                                                           /
|       |----------------------------------------------------------'
\       |
\     /
`---'
*/
using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests
{
    public class BadCompany : BaseQuest
    {
        public BadCompany()
            : base()
        {
            this.AddObjective(new SlayObjective(typeof(GreenGoblin), "GreenGoblin", 10));

            this.AddReward(new BaseReward(typeof(JaacarBox), "Bowl of Rotworm Stew Recipe"));
        }

        public override Type NextQuest
        {
            get
            {
                return typeof(ATangledWeb);
            }
        }
        /*Bad Company*/
        public override object Title
        {
            get
            {
                return 1095022;
            }
        }
        /*Travel to the Green Goblin area and kill Green Goblins until they fear you.  
        Return to Jaacar for your reward.Jaacar make friends with your kind.  
        Not want violence... not eat each other!  Jaacar eat rotworms... Rotworm stew good!  
        Make Jaacar smart!<br><br>We can be friends, yes?  Outside kind and inside kind be friends?  This is good, yes?  
        Jaacar knows who hates your kind; Gray Goblins not hate you. We want to be friends!  Jaacar want to warn you!
        Yes, friends give good warnings!<br><br>Green Goblins bad, very bad.  Green Goblins building up piles of weapons!  
        When green goblins get enough weapons, they make war with the outside kind... Your kind!  They come in the night and stab my new friend with own sword!  
        They will!  I swear!<br><br>Gray Goblins know this, that is why we fight them!  We protect our friend!  You, our friend!  Will you help stop Green Goblins?  
        If you help, Jaacar give some of smart knowledge! Help each other, yes?*/
        public override object Description
        {
            get
            {
                return 1095024;
            }
        }
        /*Oh poor friend.  Not believe Jaacar.  You will see.  Maybe too late, but you will see.*/
        public override object Refuse
        {
            get
            {
                return 1095025;
            }
        }
        /*Friend make Green Goblins dead yet?  Make them go squish?  If no green squish, they kill you when you sleep!  They will!*/
        public override object Uncomplete
        {
            get
            {
                return 1095026;
            }
        }
        /*Oh, have mercy on us!  Have you come to kill every one of us?  Take what you will and go!
        Your kind is more terrible than the master!  Woe are we, the green goblins, we serve the master's plan and yet he... *gasp**/
        public override object Complete
        {
            get
            {
                return 1095030;
            }
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