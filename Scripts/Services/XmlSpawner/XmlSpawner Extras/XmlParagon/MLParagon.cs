using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.XmlSpawner2
{
    public class MLParagon : XmlParagon
    {
        // default artifact types
        private static Type[] m_MLArtifacts = new Type[]
		{
			typeof( AegisOfGrace ), typeof( BladeDance ), 
			typeof( Bonesmasher ), typeof( FeyLeggings ),
			typeof( FleshRipper ), typeof( MelisandesCorrodedHatchet ),
			typeof( PadsOfTheCuSidhe ), typeof( RaedsGlory ),
			typeof( RighteousAnger ), typeof( RobeOfTheEclipse ),
			typeof( RobeOfTheEquinox ), typeof( SoulSeeker ),
			typeof( TalonBite )
		};

        public override Type[] Artifacts { get { return m_MLArtifacts; } set { m_MLArtifacts = value; } }

        public override string OnIdentify(Mobile from)
        {
            return String.Format("ML {0}", base.OnIdentify(from));
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
            // version 0
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            // version 0
        }

        #region constructors
        public MLParagon(ASerial serial)
            : base(serial)
        {
        }

        [Attachable]
        public MLParagon()
            : base()
        {
        }
        #endregion
    }
}