using System;
using System.Collections.Generic;
using Server;

namespace Server.ACC.CM
{
	public class ModuleList : Dictionary<Type,Module>
	{
		private Serial m_Owner;

		public ModuleList( Serial serial )
		{
			m_Owner = serial;
		}

        public List<Module> GetListOfModules()
        {
            return new List<Module>(Values);
        }

		public bool Contains( Type type )
		{
			return ContainsKey( type );
		}

		public void Add( Module mod )
		{
			if( ContainsKey( mod.GetType() ) )
				return;

			Add( mod.GetType(), mod );
		}

		public void Add( Type type )
		{
			if( ContainsKey( type ) )
				return;

			object[] Params = new object[1]{ m_Owner };
			Module mod = Activator.CreateInstance( type, Params ) as Module;
			if( mod != null )
				Add( type, mod );
		}

		public void Change( Module mod )
		{
            if (ContainsValue(mod))
                this[mod.GetType()] = mod;
            else
                Add(mod);
		}

		public void Append( Module mod, bool negatively )
		{
			if( ContainsKey( mod.GetType() ) )
				((Module)this[ mod.GetType() ]).Append( mod, negatively );
		}

		public void Remove( Module mod )
		{
			Remove( mod.GetType() );

			if( Count == 0 )
				CentralMemory.Remove( m_Owner );
		}

		public new void Remove( Type type )
		{
			base.Remove( type );

			if( Count == 0 )
				CentralMemory.Remove( m_Owner );
		}

		public Module Get( Type type )
		{
            if (Contains(type))
                return this[type];
            else return null;
		}
	}
}