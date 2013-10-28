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

namespace Server.Engines.Quests
{
    public class PinkistheNewBlack : BaseQuest
    {
        public PinkistheNewBlack()
            : base()
        {
            this.AddObjective(new ObtainObjective(typeof(SearedFireAntGoo), "Seared Fire Ant Goo", 5, 0x2f5f));
            this.AddObjective(new ObtainObjective(typeof(PileofInspectedAgapiteIngots), "Pile of Inspected Agapite Ingots", 1, 0x1BEA));
            
            this.AddReward(new BaseReward(typeof(ElixirofAgapiteConversion), "Elixir of Agapite Conversion"));
        }

        /*Pink Is The New Black */
        public override object Title
        {
            get
            {
                return 1112777;
            }
        }
        public override TimeSpan RestartDelay
        {
            get
            {
                return TimeSpan.FromHours(2);
            }
        }
        /*It is good to see you. As one of our valued customers, Mistress Zosilem has given me permission to offer you another special elixir, 
        one able to convert the more common shadow iron into valuable agapite. I'll need twenty agapite ingots and some seared fire ant goo for the mixture. 
        Are you interested?<br><br>I will need to inspect the ingots before I accept them, so give those to me before we complete the transaction.*/
        public override object Description
        {
            get
            {
                return 1112956;
            }
        }
        /*As always, feel free to return to our shop when you find yourself in need. Farewell.*/
        public override object Refuse
        {
            get
            {
                return 1112957;
            }
        }
        /*I will need twenty agapite ingots and some seared fire ant goo which, unsurprisingly, can be found on fire ants.*/
        public override object Uncomplete
        {
            get
            {
                return 1112958;
            }
        }
        /*Good to see you again, have you come to bring me the ingredients for the elixir of agapite conversion? Good, 
        I'll take those in return for this elixir I made earlier. I'll be busy the rest of the day, but come back tomorrow if you need more.*/
        public override object Complete
        {
            get
            {
                return 1112959;
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