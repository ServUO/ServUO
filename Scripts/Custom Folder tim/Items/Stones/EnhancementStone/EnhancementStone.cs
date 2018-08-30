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
using Server.Targeting;

namespace Server.Items
{
    public class EnhancementStone : BaseAddon
	{
        public override BaseAddonDeed Deed { get { return new EnhancementStoneDeed(); } }

		[Constructable]
		public EnhancementStone()
		{
            AddComponent(new EnhancementComponent(), 0, 0, 0);
		}

        public EnhancementStone(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

    public class EnhancementComponent : AddonComponent
    {
        [Constructable]
        public EnhancementComponent()
            : base(0xED6)
        {
            Name = "Equipment Enhancement Stone";
            Hue = 0x481;
        }

        public EnhancementComponent(Serial serial)
            : base(serial)
		{
		}

        public override void OnDoubleClick(Mobile from)
        {
            from.SendMessage("Select the equipment you would like to enhance...");
            from.BeginTarget(-1, false, TargetFlags.None, new TargetCallback(OnTarget));
        }

        public void OnTarget(Mobile from, object obj)
        {
            if (obj is Item)
            {
                if (((Item)obj).RootParent != from)
                {
                    from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
                }
                else
                {
                    EnhancementStoneProcess process = new EnhancementStoneProcess(from, (Item)obj);
                    process.BeginProcess();
                }
            }
        }
        
        public override void Serialize(GenericWriter writer)
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
    }

    public class EnhancementStoneDeed : BaseAddonDeed
	{
        public override BaseAddon Addon { get { return new EnhancementStone(); } }

		[Constructable]
		public EnhancementStoneDeed()
		{
            Name = "Equipment Enhancement Stone";
        }

        public EnhancementStoneDeed(Serial serial) : base(serial)
		{
            Name = "Equipment Enhancement Stone";
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}