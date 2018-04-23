using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Auction
{
    class VirtualItem : Item
    {
        public Item item;

        public VirtualItem( Item i )
            : base( i.ItemID )
        {
            item = i;
            Hue = i.Hue;
            Name = i.Name;
            Movable = false;
        }

        public override void GetProperties( ObjectPropertyList list )
        {
            item.GetProperties( list );
        }

        public override void OnSingleClick( Mobile from )
        {
            item.OnSingleClick( from );
        }

        public VirtualItem( Serial s ) : base( s ) { }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int ver = reader.ReadInt();
        }
    }
}
