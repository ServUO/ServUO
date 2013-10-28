#region Header
// **********
// ServUO - Events.cs
// **********
#endregion

#region References
using System;
#endregion

namespace CustomsFramework
{
	public delegate void BaseCoreEventHandler(BaseCoreEventArgs e);

	public delegate void BaseModuleEventHandler(BaseModuleEventArgs e);

	public class BaseCoreEventArgs : EventArgs
	{
		private readonly BaseCore m_Core;

		public BaseCoreEventArgs(BaseCore core)
		{
			m_Core = core;
		}

		public BaseCore Core { get { return m_Core; } }
	}

	public class BaseModuleEventArgs : EventArgs
	{
		private readonly BaseModule m_Module;

		public BaseModuleEventArgs(BaseModule module)
		{
			m_Module = module;
		}

		public BaseModule Module { get { return m_Module; } }
	}
}