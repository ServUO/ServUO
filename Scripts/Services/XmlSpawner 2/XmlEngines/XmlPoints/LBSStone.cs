using System;
using System.Collections;
using Server.Commands;

namespace Server.Engines.XmlSpawner2
{
    public class LBSStone : Item
    {
        [Constructable]
        public LBSStone()
            : base(0xED4)
        {
            this.Movable = false;
            this.Visible = false;
            this.Name = "LeaderboardSave Stone";
            
            // is there already another?
            ArrayList dlist = new ArrayList();
            foreach (Item i in World.Items.Values)
            {
                if (i is LBSStone && i != this)
                {
                    dlist.Add(i);
                }
            }
            foreach (Item d in dlist)
            {
                d.Delete();
            }
        }

        public LBSStone(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (m != null && m.AccessLevel >= AccessLevel.GameMaster)
            {
                CommandEventArgs e = new CommandEventArgs(m, "", "", new string[0]);
                XmlPoints.LeaderboardSave_OnCommand(e);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
			
            XmlPoints.LBSSerialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
			
            XmlPoints.LBSDeserialize(reader);

            int version = reader.ReadInt();
            // version 0
        }
    }
}