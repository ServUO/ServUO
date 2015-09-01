namespace Ultima
{
	public class WindowProcessStream : ProcessStream
	{
		private ClientWindowHandle m_Window;
		private ClientProcessHandle m_ProcessID;

		public ClientWindowHandle Window { get { return m_Window; } set { m_Window = value; } }

		public WindowProcessStream(ClientWindowHandle window)
		{
			m_Window = window;
			m_ProcessID = ClientProcessHandle.Invalid;
		}

		public override ClientProcessHandle ProcessID
		{
			get
			{
				if (NativeMethods.IsWindow(m_Window) != 0 && !m_ProcessID.IsInvalid)
				{
					return m_ProcessID;
				}

				NativeMethods.GetWindowThreadProcessId(m_Window, ref m_ProcessID);

				return m_ProcessID;
			}
		}
	}
}