using System;
using Server;
using System.Collections;
using Server.Commands;
using Server.Commands.Generic;

namespace Server.Engines.XmlSpawner2
{
    public class QuestLeadersStone: Item
	{

	   [Constructable]
		public QuestLeadersStone() : base( 0xED4)
		{
            Movable = false;
            Visible = false;
            Name = "Quest LeaderboardSave Stone";
            
            // is there already another?
            ArrayList dlist = new ArrayList();
            foreach( Item i in World.Items.Values)
            {
                if(i is QuestLeadersStone && i != this)
                {
                    dlist.Add(i);
                }
            }
            foreach(Item d in dlist)
            {
                d.Delete();
            }
		}

        public QuestLeadersStone( Serial serial ) : base( serial )
		{
		}
		
		public override void OnDoubleClick( Mobile m )
		{
			if( m != null && m.AccessLevel >= AccessLevel.Administrator)
			{
                CommandEventArgs e = new CommandEventArgs(m, "", "", new string[0]);
                XmlQuestLeaders.QuestLeaderboardSave_OnCommand(e);
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			
			XmlQuestLeaders.QuestLBSSerialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			
			XmlQuestLeaders.QuestLBSDeserialize( reader );

			int version = reader.ReadInt();
			
			// version 0
		}
	}
}
