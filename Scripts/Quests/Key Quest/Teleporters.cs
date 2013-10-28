//By: Monolith - 10/10/2011

using System;
using Server.Mobiles;

namespace Server.Items
{ 
    public class UnderworldTele : Teleporter
    { 
        [Constructable]
        public UnderworldTele()
        {
        }

        public UnderworldTele(Serial serial)
            : base(serial)
        {
        }

        public override bool OnMoveOver(Mobile m)
        {
            if (m is PlayerMobile)
            {
                PlayerMobile player = (PlayerMobile)m;
				
                if (player.AbyssEntry)
                {
                    m.SendMessage("You Enter the Stygian Abyss");
                    return base.OnMoveOver(m);
                }
                else
                    m.SendMessage("You have not obtained entry to the Abyss");				
            }
            return true;
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