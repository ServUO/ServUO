using System;
using Server;
using Server.Mobiles;
using Server.Berserk;

namespace Server.Items
{
	public class BestialGloves : LeatherGloves
	{
        public override bool IsArtifact { get { return true; } }
        public override int LabelNumber{ get{ return 1151198; } } // Bestial Gloves

        #region ISetItem Members
        public override SetItem SetID{ get{ return SetItem.Bestial; } }
		public override int Pieces{ get{ return 4; } }
        public override int Berserk { get { return 1; } }
        #endregion

        public override int BasePhysicalResistance{ get{ return 4; } }
		public override int BaseFireResistance{ get{ return 19; } }
		public override int BaseColdResistance{ get{ return 5; } }
		public override int BasePoisonResistance{ get{ return 5; } }
		public override int BaseEnergyResistance{ get{ return 5; } }
		public override int InitMinHits{ get{ return 125; } }
		public override int InitMaxHits{ get{ return 125; } }

		[Constructable]
		public BestialGloves() : base()
		{
            this.Hue = 2010;
            this.Weight = 1;
            this.StrRequirement = 20;
        }

		public BestialGloves( Serial serial ) : base( serial )
		{
		}

        public override void OnAdded(object parent)
        {
            base.OnAdded(parent);

            if (parent is PlayerMobile)
            {
                PlayerMobile pm = parent as PlayerMobile;

                if (pm.BestialBerserk)
                    this.Hue = BeastialSetHelper.AddBestialHueParent(pm);
            }
        }

        public override void OnRemoved(object parent)
        {
            base.OnRemoved(parent);

            if (parent is PlayerMobile && !Deleted)
            {
                PlayerMobile pm = parent as PlayerMobile;

                if (pm.BestialBerserk)
                {
                    this.Hue = 2010;
                    BeastialSetHelper.DropBestialHueParent(pm);
                }
            }
        }

        public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );			
			writer.Write( (int) 0 ); // version
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );			
			int version = reader.ReadInt();

            this.Hue = 2010;
        }
	}
}