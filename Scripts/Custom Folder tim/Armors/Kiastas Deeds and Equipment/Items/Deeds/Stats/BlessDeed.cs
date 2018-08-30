using System;
using Server.Network;
using Server.Prompts;
using Server.Items;
using Server.Targeting;
using Server;

namespace Server.Kiasta.Deeds
{
    public class BlessTarget : BaseDeedTarget
    {
        private BlessDeed m_Deed;

        public BlessTarget(BlessDeed deed)
        {
            m_Deed = deed;
            AttributeName = Settings.AttributeName.Bless;
            Attribute = new object[] { Settings.AttributeModifierValue.Bless };
        }

        protected override void OnTarget(Mobile from, object target)
        {
            if (m_Deed.Deleted || m_Deed.RootParent != from)
            {
                from.SendMessage("You cannot apply {0} to that.", AttributeName);
                return;
            }
            else
            {
                ModifyItem modify = new ModifyItem(from, target, Attribute, AttributeName, Max, Modifier);
                if (modify.IsApplied)
                {
                    m_Deed.Delete();
                }
            }
        }
    }

	public class BlessDeed : BaseDeed
	{
		[Constructable]
		public BlessDeed() : base( Settings.Misc.DeedItemID )
		{
            AttributeName = Settings.AttributeName.Bless;
            Name = "a " + AttributeName + " deed";
		}

		public BlessDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			LootType = LootType.Blessed;

			int version = reader.ReadInt();
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) )
			{
				 from.SendMessage("That must be in your pack for you to use it.");
			}
			else
			{
                from.SendMessage("What would you like {0}?", Settings.AttributeName.Bless);
				from.Target = new BlessTarget( this ); 
			}
		}	
	}
}