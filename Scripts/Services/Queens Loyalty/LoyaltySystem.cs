using System;
using Server;
using Server.Mobiles;
using Server.Items;
using System.Collections.Generic;
using Server.Engines.Quests;

namespace Server.Engines.QueensLoyalty
{
	public enum QueenLoyalty
	{
		Enemy,
		Friend,
		Citizen,
		Noble
	}
	
	public static class LoyaltySystem
	{
        public static readonly int MinForHousing = 10000;
		
		public static void HandleKill(BaseCreature victim, Mobile damager, bool highestDamager)
		{
            if (victim == null || damager == null || damager.Map != Map.TerMur)
                return;

			if(PointsEntry.Entries.ContainsKey(victim.GetType()))
				ProcessKill(victim, damager, highestDamager);
		}
		
		public static void ProcessKill(BaseCreature victim, Mobile damager, bool highestDamager)
		{
            Type type = victim.GetType();

            if (damager is BaseCreature && (((BaseCreature)damager).Controlled || ((BaseCreature)damager).Summoned))
                damager = ((BaseCreature)damager).GetMaster();

            if (damager == null)
                return;

			if(highestDamager)
			{
				if(PointsEntry.Entries.ContainsKey(type))
					AwardPoints(damager, PointsEntry.Entries[type].TopAttackerPoints, false);
			}
			else
			{
				if(PointsEntry.Entries.ContainsKey(type))
					AwardPoints(damager, PointsEntry.Entries[type].RightsPoints, false);
			}
		}
		
		public static void HandleQuest(Mobile from, BaseQuest quest)
		{
			if(PointsEntry.Entries.ContainsKey(quest.GetType()))
				ProcessQuest(from, quest);
		}
		
		public static void ProcessQuest(Mobile from, BaseQuest quest)
		{
			if(from == null || quest == null)
				return;
				
			Type type = quest.GetType();
				
			if(PointsEntry.Entries.ContainsKey(type))
				AwardPoints(from, PointsEntry.Entries[type].TopAttackerPoints, true);
		}
		
		public static void AwardPoints(Mobile from, double points, bool quest)
		{
            if (from is PlayerMobile)
            {
                PlayerMobile pm = from as PlayerMobile;

                if (pm.Level >= 100)
                {
                    from.SendMessage(0x26, "You already have maximum loyalty.");
                }
                else
                {
                    if (from is PlayerMobile)
                        ((PlayerMobile)from).Exp += (int)points * 10;

                    if (quest)
                        from.SendLocalizedMessage(1113719, ((int)points).ToString(), 0x26); //You have received ~1_val~ loyalty points as a reward for completing the quest. 
                    else
                        from.SendMessage(0x26, String.Format("You have recieved {0} loyalty points.", ((int)points).ToString()));
                }
            }
		}
	}
}