/* 
 * This is a 3 piece weapon add on (a magic gem, and 2 broken pieces of a Lich staff).  When you have each of the 3 parts you double click the gem and you will 
 * get the Lich Staff.  You just drop these files wherever you put your custom scripts. Introduce them into the game in any way you want. They make a nice quest 
 * addition. I simply added them as a possible loot drop on the Lich.  
 * 
3 pieces to assemble staff:
BrokenLichStaffPart1
BrokenLichStaffPart2
LichGem	
Together they create the Staff - LichStaff

**Tukaram June2017**

To make them a possible loot drop on the Lich I added this region just under "loot pack" and above "serialization" in lich.cs.

#region loot test 

 public override void OnDeath(Container c) // (random chance) 

 

    #region Lich Staff 

        public override void OnDeath(Container c) // (random chance) 
                {

            base.OnDeath(c);

            if (0.10 > Utility.RandomDouble()) // 0.1=10% chance to drop 

                switch (Utility.Random(3))

                {

                    case 0: c.DropItem(new BrokenLichStaffPart1()); break;

                    case 1: c.DropItem(new BrokenLichStaffPart2()); break;

                    case 2: c.DropItem(new LichGem()); break;
                }

        }

        #endregion

 * 
 */ 
using System;
using Server;
using Server.Gumps;
using Server.Network;
using System.Collections;
using Server.Multis;
using Server.Mobiles;


namespace Server.Items
{

    public class LichGem : Item
    {
        [Constructable]
        public LichGem() : this(null)
        {
        }

        [Constructable]
        public LichGem(string name) : base(0x1EA7)
        {
            Name = "A Magical Lich Gem";
            Hue = 1152;

        }

        public LichGem(Serial serial) : base(serial)
        {
        }


        
                public override void OnDoubleClick( Mobile m )

                {
                    Item a = m.Backpack.FindItemByType( typeof(BrokenLichStaffPart1) );
                    if ( a != null )
                    {	
                    Item b = m.Backpack.FindItemByType( typeof(BrokenLichStaffPart2) );
                    if ( b != null )
                    {						
                        m.AddToBackpack( new LichStaff () );
                        a.Delete();
                        b.Delete();

                        m.SendMessage( "You repair the Lich Staff" );
                        this.Delete();

                    }
                        else
                    {
                        m.SendMessage( "Are You Forgetting Something?" );
                }
                }
                }


        
        public override void Serialize ( GenericWriter writer)
		{
			base.Serialize ( writer );

			writer.Write ( (int) 0);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize ( reader );

			int version = reader.ReadInt();
		}
	}
}