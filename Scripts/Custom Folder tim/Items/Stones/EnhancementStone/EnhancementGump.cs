/*
 * 
 * Equipment Enhancement System
 * Version 1.5
 * Designed for SVN 663 + ML
 * Modified for RunUO 2.2 SVN
 * 
 * Authored by Dougan Ironfist
 * Last Updated on 2/1/2012
 * 
 * The purpose of these scripts is to allow an easier means for shards with a smaller playerbase to be able to enhance their equipment
 * to be more able to handle tougher creatures and spawns.  For shards with a larger playerbase, these scripts can be used as means
 * to eliminate alot of excess gold from the player economy.
 * 
 * These scripts provide a deed for the Equipment Enhancement Stone.  This will allow players to put a stone in their house for easy
 * access and convenience.  The deed can be dispensed in whatever means the shard administrators feel is appropriate.
 * 
 * Alternately, shard administrators could simply place the actual Equipment Enhancement Stones within the cities on their shard
 * and eliminate the need to determine how to distribute deeds.  This could allow the administrators to promote PVP on their
 * shard by placing only a handful of stones in cities in Felucca (if PVP is a desired goal of the shard).
 * 
 */

using System;
using Server;
using Server.Items;

namespace Server.Gumps
{
    public class EnhancementGump : Gump
    {
        private EnhancementStoneProcess Process;

        public EnhancementGump(EnhancementStoneProcess process) : base(40, 40)
        {
            bool MoreAttributesAllowed = true;

            Process = process;

            if (Process.CurrentAttributeCount >= Process.MaxAttrCount)
                MoreAttributesAllowed = false;

            AddBackground(0, 0, 620, 390, 9200);
            AddImageTiled(8, 10, 604, 24, 2624);
            AddImageTiled(8, 38, 300, 345, 2624);
            AddImageTiled(312, 38, 300, 345, 2624);
            AddAlphaRegion(8, 10, 604, 373);

            AddLabel(224, 12, 0x481, "Equipment Enhancement");

            AddLabel(15, 40, 0x481, "Attributes");
            AddLabel(184, 40, 0x481, "Cost");
            AddLabel(273, 40, 0x481, "Buy");

            AddLabel(319, 40, 0x481, "Attributes");
            AddLabel(488, 40, 0x481, "Cost");
            AddLabel(577, 40, 0x481, "Buy");

            int column = 0;
            int row = 0;

            for ( int i = 0; i < AttributeHandler.Definitions.Count; i++)
            {
                AttributeHandler handler = AttributeHandler.Definitions[i];

                if (handler.IsUpgradable(Process.ItemToUpgrade))
                {
                    int currentValue = handler.Upgrade(Process.ItemToUpgrade, true);

                    if (currentValue > 0 || MoreAttributesAllowed)
                    {
                        if (row > 11)
                        {
                            row = 0;
                            column = 1;
                        }

                        AddLabel(15 + (304 * column), 65 + (25 * row), 0x481, handler.Description);
                        AddLabel(184 + (304 * column), 65 + (25 * row), 0x481, Process.GetCostToUpgrade(handler).ToString());
                        AddButton(270 + (304 * column), 62 + (25 * row), 4023, 4024, 1000 + i, GumpButtonType.Reply, 0);

                        row++;
                    }
                }
            }
        }

        public override void OnResponse(Network.NetState sender, RelayInfo info)
        {
            if (info.ButtonID >= 1000)
            {
                AttributeHandler handler = AttributeHandler.Definitions[info.ButtonID - 1000];
                Process.BeginUpgrade(handler);
            }
        }
    }
}
