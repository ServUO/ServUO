using Server;

namespace CustomsFramework
{
	public partial class SaveData
    {
        #region ISerializable Members

        public virtual void SerializeSW( GenericWriter writer )
        {
            //sollte als Basisklasse das Selbe sein:
            //writer.WriteVersionSW( this, 0 );
            writer.Write( 0 );
        }

        #endregion


        public virtual void DeserializeSW( GenericReader reader )
        {
            //sollte als Basisklasse das Selbe sein:
            //reader.ReadVersionSW( this );
            reader.ReadInt();
        }
    }
}
