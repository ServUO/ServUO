using System;
using Server;

namespace Server.ACC.CM
{
	public abstract class Module
	{
		/*
		 * Append( Module mod, bool negatively )
		 * This method MUST be inherited.
		 * This method is used to take what is already in CM
		 * and add/subtract from it what is on the Module mod.
		 * if( negatively ) means you want to remove stuff.
		 */
		public abstract void Append( Module mod, bool negatively );

		public abstract string Name();

		internal int m_TypeRef;
		private Serial m_Owner;
		public  Serial Owner{ get{ return m_Owner; } }

		public Module( Serial ser )
		{
			m_Owner = ser;

			Type type = this.GetType();
			m_TypeRef = CentralMemory.m_Types.IndexOf( type );

            if (m_TypeRef == -1)
            {
                CentralMemory.m_Types.Add(type);
                m_TypeRef = CentralMemory.m_Types.Count - 1;
            }
		}

		public virtual void Serialize( GenericWriter writer )
		{
			writer.Write( (int)0 ); //version
		}

		public virtual void Deserialize( GenericReader reader )
		{
			int version = reader.ReadInt();
		}
	}
}