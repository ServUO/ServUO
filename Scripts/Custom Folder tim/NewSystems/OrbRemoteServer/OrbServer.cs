using System;
using System.Collections;
using OrbServerSDK;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using Server.Accounting;
using Server;
using Server.Network;
using System.Threading;

namespace Server.Engines.OrbRemoteServer
{
	class OrbServer
	{
		private static readonly int SERVER_PORT = 2594;
		private static readonly AccessLevel REQUIRED_ACCESS_LEVEL = AccessLevel.GameMaster;
		private static readonly string SERVER_VERSION = "";
		private static readonly string SERVER_NAME = "UO Architect Server for RunUO 2.0";

		private static Hashtable m_Registry = System.Collections.Specialized.CollectionsUtil.CreateCaseInsensitiveHashtable(0);
		private static Hashtable m_Clients = new Hashtable();

		private static bool m_IsRunning = false;

		public static bool IsRunning
		{
			get{ return m_IsRunning; }
		}

		public static void Initialize()
		{
		}

		static OrbServer()
		{
			StartServer();

			OrbConnection.OnLogin += new OrbConnection.LoginEvent(OnLogin);
			OrbConnection.OnExecuteCommand += new OrbConnection.ExecuteCommandEvent(OnExecuteCommand);
			OrbConnection.OnExecuteRequest += new OrbConnection.ExecuteRequestEvent(OnExecuteRequest);
		}

		private static void OnExecuteCommand(string alias, OrbClientInfo clientInfo, OrbCommandArgs args)
		{
			OrbClientState client = GetClientState(clientInfo);

			if(client == null)
				return;

			try
			{
				OrbCommand command = GetCommand(alias, client);

				if(command != null)
				{
					new CommandSyncTimer(client, command, args).Start();
				}
			}
			catch(Exception e)
			{
				Console.WriteLine("Exception occurred for OrbServer command {0}\nMessage: {1}", alias, e.Message);
			}
		}

		private static OrbClientState GetClientState(OrbClientInfo clientInfo)
		{
			return (OrbClientState)m_Clients[clientInfo.ClientID];
		}

		private static OrbResponse OnExecuteRequest(string alias, OrbClientInfo clientInfo, OrbRequestArgs args)
		{
			OrbClientState client = GetClientState(clientInfo);
			
			if(client == null)
				return null;

			OrbResponse response = null;

			try
			{
				OrbRequest request = GetRequest(alias, client);

				if(request != null)
				{
					ManualResetEvent reset = new ManualResetEvent(false);
					request.ResetEvent = reset;

					new RequestSyncTimer(client, request, args).Start();
					reset.WaitOne();

					response = request.Response;
				}
			}
			catch(Exception e)
			{
				Console.WriteLine("Exception occurred for OrbServer request {0}\nMessage: {1}", alias, e.Message);
			}

			return response;
		}

		private static OrbRequest GetRequest(string alias, OrbClientState client)
		{
			OrbRegistryEntry entry = (OrbRegistryEntry)m_Registry[alias];
			OrbRequest request = null;

			if(entry != null)
			{
				if(CanConnectionAccess(client, entry))
				{
					try
					{
						request = (OrbRequest)Activator.CreateInstance(entry.Type);
					}
					catch(Exception e)
					{
						Console.WriteLine("OrbServer Exception: " + e.Message);
					}
				}
			}

			return request;
		}

		private static OrbCommand GetCommand(string alias, OrbClientState client)
		{
			OrbRegistryEntry entry = (OrbRegistryEntry)m_Registry[alias];
			OrbCommand command = null;

			if(entry != null)
			{
				if(CanConnectionAccess(client, entry))
				{
					try
					{
						command = (OrbCommand)Activator.CreateInstance(entry.Type);
					}
					catch(Exception e)
					{
						Console.WriteLine("OrbServer Exception: " + e.Message);
					}
				}
			}

			return command;
		}

		private static bool CanConnectionAccess(OrbClientState client, OrbRegistryEntry entry)
		{
			bool authorized = false;

			if(entry.RequiresLogin)
			{
				if(client.OnlineMobile != null)
				{
					authorized = IsAccessAllowed(client.OnlineMobile, entry.RequiredLevel);
				}
			}
			else
			{
				authorized = IsAccessAllowed(client.Account, entry.RequiredLevel);
			}

			return authorized;
		}

		private static void StartServer()
		{
			// Run the OrbServer in a worked thread
			ThreadPool.QueueUserWorkItem(new WaitCallback(OrbServer.Run), null);
		}

		public static void Register(string alias, Type type, AccessLevel requiredLevel, bool requiresLogin)
		{
			if(!type.IsSubclassOf(typeof(OrbRequest)) && !type.IsSubclassOf(typeof(OrbCommand)) )
			{
				Console.WriteLine("OrbRemoteServer Error: The type {0} isn't a subclass of the OrbCommand or OrbRequest classes.", type.FullName); 
			}
			else if(m_Registry.ContainsKey(alias))
			{	
				Console.WriteLine("OrbRemoteServer Error: The type {0} has been assigned a duplicate alias.", type.FullName); 
			}
			else
			{
				m_Registry.Add(alias, new OrbRegistryEntry(alias, type, requiredLevel, requiresLogin));
			}
			
		}

		//////////////////////////////////////////////////////////////////////////////
		// main method - this method registers the TCP/IP channel and the OrbConnection Type
		// for remote instanciation.
		public static void Run(object o)
		{
			try{
			Console.WriteLine("\n{0} {1} is listening on port {2}", SERVER_NAME, SERVER_VERSION, SERVER_PORT);
			// Create Tcp channel using the selected Port number
			TcpChannel chan = new TcpChannel(SERVER_PORT);
			ChannelServices.RegisterChannel(chan);	//register channel

			// register the remote object
			RemotingConfiguration.RegisterWellKnownServiceType(
				typeof(OrbConnection),
				"OrbConnection",
				WellKnownObjectMode.Singleton);
			
			while(true)
			{
				// Keep sleeping in order to keep this tread alive without hogging cpu cycles
				Thread.Sleep(30000);
			}
			
			}catch{}
		}

		// This function callback validates an account and returns true if it has access
		// rights to use this tool.
		private static LoginCodes OnLogin(OrbClientInfo clientInfo, string password)
		{
			LoginCodes code = LoginCodes.Success;

			//Console.WriteLine("OnValidateAccount");
			IAccount account = Accounts.GetAccount(clientInfo.UserName);

			if(account == null || account.CheckPassword(password) == false)
			{
				code = LoginCodes.InvalidAccount;
			}
			else
			{
				if(!IsAccessAllowed(account, REQUIRED_ACCESS_LEVEL))
				{
					Mobile player = GetOnlineMobile(account);

					if(player == null || !IsAccessAllowed(player, REQUIRED_ACCESS_LEVEL))
					{
						// Neither the account or the char the account is logged in with has
						// the required accesslevel to make this connection.
						code = LoginCodes.NotAuthorized;
					}
				}
				
				Console.WriteLine("{0} connected to the Orb Script Server", account.Username);
			}

			if(code == LoginCodes.Success)
			{
				if(m_Clients.ContainsKey(clientInfo.ClientID))
					m_Clients.Remove(clientInfo.ClientID);

				m_Clients.Add(clientInfo.ClientID, new OrbClientState(clientInfo, account, DateTime.Now));
			}

			return code;
		}

        private static bool IsAccessAllowed(IAccount acct, AccessLevel accessLevel)
		{
			bool AccessAllowed = false;

			if(acct != null)
			{
				if((int)acct.AccessLevel >= (int)accessLevel)
				{
					AccessAllowed = true;
				}
			}
			
			return AccessAllowed;
		}

		private static bool IsAccessAllowed(Mobile mobile, AccessLevel accessLevel)
		{
			bool AccessAllowed = false;

			if(mobile != null)
			{
				if((int)mobile.AccessLevel >= (int)accessLevel)
				{
					AccessAllowed = true;
				}
			}
			
			return AccessAllowed;
		}

		// get logged in char for an account
        internal static Mobile GetOnlineMobile(IAccount acct)
		{
			if(acct == null)
				return null;

			Mobile mobile = null;

			// syncronize the account object to keep this access thread safe
			lock(acct)
			{
				for(int i=0; i < 5; ++i)
				{
					Mobile mob = acct[i];

					if(mob == null)
						continue;

					if(mob.NetState != null)
					{
						mobile = mob;
						break;
					}
				}
			}

			return mobile;
		}

		// Stores info regarding the registered OrbCommand or OrbRequest
		class OrbRegistryEntry
		{
			public string Alias;
			public Type Type;
			public AccessLevel RequiredLevel;
			public bool RequiresLogin;

			public OrbRegistryEntry(string alias, Type type, AccessLevel requiredLevel, bool requiresLogin)
			{
				Alias = alias;
				Type = type;
				RequiredLevel = requiredLevel;
				RequiresLogin = requiresLogin;
			}

			public bool IsCommand
			{
				get{ return Type.IsSubclassOf(typeof(OrbCommand)); }
			}

			public bool IsRequest
			{
				get{ return Type.IsSubclassOf(typeof(OrbRequest)); }
			}
		}

		class RequestSyncTimer : Timer
		{
			private OrbClientState m_Client;
			private OrbRequest m_Request;
			private OrbRequestArgs m_Args;

			public RequestSyncTimer(OrbClientState client, OrbRequest request, OrbRequestArgs args) : base(TimeSpan.FromMilliseconds(20.0), TimeSpan.FromMilliseconds(20.0))
			{
				m_Client = client;
				m_Request = request;
				m_Args = args;
			}

			protected override void OnTick()
			{
				if(m_Request != null)
					m_Request.OnRequest(m_Client, m_Args);

				Stop();
			}
		}

		class CommandSyncTimer : Timer
		{
			private OrbClientState m_Client;
			private OrbCommand m_Command;
			private OrbCommandArgs m_Args;

			public CommandSyncTimer(OrbClientState client, OrbCommand command, OrbCommandArgs args) : base(TimeSpan.FromMilliseconds(20.0), TimeSpan.FromMilliseconds(20.0))
			{
				m_Client = client;
				m_Command = command;
				m_Args = args;
			}

			protected override void OnTick()
			{
				if(m_Command != null)
					m_Command.OnCommand(m_Client, m_Args);

				Stop();
			}
		}
	}

	internal class OrbClientState : OrbClientInfo 
	{
        private IAccount m_Account = null;
		private DateTime m_LoginTime;

        internal OrbClientState(OrbClientInfo clientInfo, IAccount account, DateTime loginTime)
            : base(clientInfo.ClientID, clientInfo.UserName)
		{
			m_Account = account;
			m_LoginTime = loginTime;
		}

        internal IAccount Account
		{
			get{ return m_Account; }
		}

		internal Mobile OnlineMobile
		{
			get{ return OrbServer.GetOnlineMobile(m_Account); }
		}

		internal DateTime LoginTime
		{
			get{ return m_LoginTime; }
		}
	}
}
