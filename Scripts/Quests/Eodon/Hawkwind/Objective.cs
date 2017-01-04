using System;
using Server;
using Server.Items;
using Server.Mobiles;
 
namespace Server.Engines.Quests.TimeLord
{
	public class TimeForLegendsObjective : QuestObjective
	{
		public override object Message { get { return 1156341; } } // Prove yourself to Hawkwind, defeat thy foe in order to begin your journey among the Legendary.

        public override int MaxProgress { get { return 1; } }

		public TimeForLegendsObjective()
		{
		}
		
		public override void RenderProgress( BaseQuestGump gump )
		{
            if (System is TimeForLegendsQuest)
            {
                TimeForLegendsQuest q = System as TimeForLegendsQuest;

                if (q.ToSlay == null)
                    gump.AddHtmlObject(70, 260, 270, 100, this.Completed ? 1049077 : 1049078, BaseQuestGump.Blue, false, false);
                else
                {
                    int index = Array.IndexOf(TimeForLegendsQuest.Targets, q.ToSlay);

                    gump.AddHtmlObject(70, 260, 150, 100, index <= 13 ? 1156324 + index : 1156354 + (index - 14), BaseQuestGump.Blue, false, false);
                    gump.AddHtmlObject(230, 260, 10, 100, ":", BaseQuestGump.Blue, false, false);
                    gump.AddHtmlObject(235, 260, 150, 100, this.Completed ? 1046033 : 1046034, BaseQuestGump.Blue, false, false);
                }
            }
		}
		
		public override void OnKill(BaseCreature creature, Container corpse)
		{
			if(System is TimeForLegendsQuest && creature.GetType() == ((TimeForLegendsQuest)System).ToSlay)
			{
				Complete();
			}
		}
		
		public override void Complete()
		{
			base.Complete();
			System.Complete();
		}
	}
}