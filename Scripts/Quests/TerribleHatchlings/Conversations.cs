namespace Server.Engines.Quests.Zento
{
    public class AcceptConversation : QuestConversation
    {
        public override object Message =>
                /* <I><U>Important Quest Information</U></I><BR><BR>
* 
* During your quest, any important information that a
* <a href = "?ForceTopic31">NPC</a> gives you, will appear in a window
* such as this one.  You can review the information at any time during your
* quest.<BR><BR>
* 
* <U>Getting Help</U><BR><BR>
* 
* Some of the text you will come across during your quest,
* will be underlined <a href = "?ForceTopic73">links to the codex of wisdom</a>,
* or online help system.  You can click on the text to get detailed information
* on the underlined subject.  You may also access the Codex Of Wisdom by
* pressing "F1" or by clicking on the "?" on the toolbar at the top of
* your screen.<BR><BR><U>Context Menus</U><BR><BR>
* 
* Context menus can be called up by single left-clicking (or Shift + single
* left-click, if you changed it) on most objects or NPCs in the world.
* Nearly everything, including your own avatar will have context menus available.
* Bringing up your avatar's context menu will give you options to cancel your quest
* and review various quest information.<BR><BR>
*/
                1049092;
        public override void OnRead()
        {
            System.AddObjective(new FirstKillObjective());
        }
    }

    public class DirectionConversation : QuestConversation
    {
        public override object Message =>
                // The Deathwatch Beetle Hatchlings live in The Waste - the desert close to this city.
                1063323;
        public override bool Logged => false;
    }

    public class TakeCareConversation : QuestConversation
    {
        public override object Message =>
                // I know you can take care of those nasty Deathwatch Beetle Hatchlings! No get to it!
                1063324;
        public override bool Logged => false;
    }

    public class EndConversation : QuestConversation
    {
        public override object Message =>
                /* Thank you for helping me get rid of these vile beasts!
* You have been rewarded for your good deeds. If you wish to
* help me in the future, visit me again.<br><br>
* 
* Farewell.
*/
                1063321;
        public override void OnRead()
        {
            System.Complete();
        }
    }
}
