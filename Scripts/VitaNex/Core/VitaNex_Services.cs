#region Header
//               _,-'/-'/
//   .      __,-; ,'( '/
//    \.    `-.__`-._`:_,-._       _ , . ``
//     `:-._,------' ` _,`--` -: `_ , ` ,' :
//        `---..__,,--'  (C) 2023  ` -'. -'
//        #  Vita-Nex [http://core.vita-nex.com]  #
//  {o)xxx|===============-   #   -===============|xxx(o}
//        #                                       #
#endregion

#region References
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Server;

using VitaNex.IO;
using VitaNex.SuperGumps;
#endregion

namespace VitaNex
{
	public static partial class VitaNexCore
	{
		public static IEnumerable<CoreServiceInfo> Services
		{
			get
			{
				var idx = _Plugins.Count;

				while (--idx >= 0)
				{
					if (_Plugins.InBounds(idx) && _Plugins[idx] is CoreServiceInfo)
					{
						yield return (CoreServiceInfo)_Plugins[idx];
					}
				}
			}
		}

		public static int ServiceCount => Services.Count();

		public static Dictionary<Type, CoreServiceAttribute> ServiceTypeCache { get; private set; }
		public static Assembly[] ServiceAssemblies { get; private set; }

		public static event Action<CoreServiceInfo> OnServiceConfigured;
		public static event Action<CoreServiceInfo> OnServiceInvoked;
		public static event Action<CoreServiceInfo> OnServiceSaved;
		public static event Action<CoreServiceInfo> OnServiceLoaded;
		public static event Action<CoreServiceInfo> OnServiceDisposed;

		public static CoreServiceInfo GetService(Type t)
		{
			return Services.FirstOrDefault(csi => csi.TypeOf.IsEqualOrChildOf(t));
		}

		public static CoreServiceInfo[] GetServices(string name, bool ignoreCase = true)
		{
			return FindServices(name, ignoreCase).ToArray();
		}

		public static IEnumerable<CoreServiceInfo> FindServices(string name, bool ignoreCase = true)
		{
			var c = ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture;

			var idx = _Plugins.Count;

			while (--idx >= 0)
			{
				var cp = _Plugins[idx] as CoreServiceInfo;

				if (cp != null && String.Equals(cp.Name, name, c))
				{
					yield return cp;
				}
			}
		}

		private static void CompileServices()
		{
			if (Compiled)
			{
				return;
			}

			ToConsole("Compiling Services...");

			TryCatch(
				() =>
				{
					var files = ServicesDirectory.GetFiles("*.dll", SearchOption.AllDirectories);
					var asm = new List<Assembly>(files.Length);

					foreach (var file in files)
					{
						TryCatch(() => asm.Update(Assembly.LoadFrom(file.FullName)), ToConsole);
					}

					ServiceAssemblies = asm.ToArray();

					asm.AddRange(ScriptCompiler.Assemblies);
					asm.Prune();

					ScriptCompiler.Assemblies = asm.FreeToArray(true);
				},
				ToConsole);
		}

		public static void ConfigureServices()
		{
			ToConsole("Configuring Services...");

			var types = GetCoreServiceTypes();

			foreach (var csi in types.Select(kvp => new CoreServiceInfo(kvp.Key, kvp.Value)))
			{
				ConfigureService(csi);
			}
		}

		public static void ConfigureService(CoreServiceInfo csi)
		{
			if (csi == null)
			{
				return;
			}

			csi.ToConsole("Configuring...");

			if (!csi.Configured)
			{
				if (csi.ConfigSupported)
				{
					TryCatch(csi.GetConfigHandler(), csi.ToConsole);
				}

				TryCatch(csi.OnConfigured, csi.ToConsole);

				if (OnServiceConfigured != null)
				{
					TryCatch(() => OnServiceConfigured(csi), csi.ToConsole);
				}

				csi.ToConsole("Done.");
			}
			else
			{
				csi.ToConsole("Already configured, no action taken.");
			}
		}

		public static void InvokeServices()
		{
			ToConsole("Invoking Services...");

			foreach (var csi in Services.OrderBy(csi => csi.Priority))
			{
				InvokeService(csi);
			}
		}

		public static void InvokeService(CoreServiceInfo csi)
		{
			if (csi == null)
			{
				return;
			}

			csi.ToConsole("Invoking...");

			if (!csi.Invoked)
			{
				if (csi.InvokeSupported)
				{
					TryCatch(csi.GetInvokeHandler(), csi.ToConsole);
				}

				TryCatch(csi.OnInvoked, csi.ToConsole);

				if (OnServiceInvoked != null)
				{
					TryCatch(() => OnServiceInvoked(csi), csi.ToConsole);
				}

				csi.ToConsole("Done.");
			}
			else
			{
				csi.ToConsole("Already invoked, no action taken.");
			}
		}

		public static void SaveServices()
		{
			ToConsole("Saving Services...");

			foreach (var csi in Services.OrderBy(csi => csi.Priority))
			{
				SaveService(csi);
			}
		}

		public static void SaveService(CoreServiceInfo csi)
		{
			if (csi == null)
			{
				return;
			}

			csi.ToConsole("Saving...");

			TryCatch(csi.SaveOptions, csi.ToConsole);

			if (csi.SaveSupported)
			{
				TryCatch(csi.GetSaveHandler(), csi.ToConsole);
			}

			TryCatch(csi.OnSaved, csi.ToConsole);

			if (OnServiceSaved != null)
			{
				TryCatch(() => OnServiceSaved(csi), csi.ToConsole);
			}

			csi.ToConsole("Done.");
		}

		public static void LoadServices()
		{
			ToConsole("Loading Services...");

			foreach (var csi in Services.OrderBy(csi => csi.Priority))
			{
				LoadService(csi);
			}
		}

		public static void LoadService(CoreServiceInfo csi)
		{
			if (csi == null)
			{
				return;
			}

			csi.ToConsole("Loading...");

			TryCatch(csi.LoadOptions, csi.ToConsole);

			if (csi.LoadSupported)
			{
				TryCatch(csi.GetLoadHandler(), csi.ToConsole);
			}

			TryCatch(csi.OnLoaded, csi.ToConsole);

			if (OnServiceLoaded != null)
			{
				TryCatch(() => OnServiceLoaded(csi), csi.ToConsole);
			}

			csi.ToConsole("Done.");
		}

		public static void DisposeServices()
		{
			ToConsole("Disposing Services...");

			foreach (var csi in Services.OrderByDescending(csi => csi.Priority))
			{
				DisposeService(csi);
			}
		}

		public static void DisposeService(CoreServiceInfo csi)
		{
			if (csi == null)
			{
				return;
			}

			csi.ToConsole("Disposing...");

			if (!csi.Disposed)
			{
				if (csi.DisposeSupported)
				{
					TryCatch(csi.GetDisposeHandler(), csi.ToConsole);
				}

				TryCatch(csi.OnDisposed, csi.ToConsole);

				if (OnServiceDisposed != null)
				{
					TryCatch(() => OnServiceDisposed(csi), csi.ToConsole);
				}

				csi.ToConsole("Done.");
			}
			else
			{
				csi.ToConsole("Already disposed, no action taken.");
			}
		}

		/// <summary>
		///     Gets a collection of [cached] Types representing all CoreServices in this assembly
		/// </summary>
		public static Dictionary<Type, CoreServiceAttribute> GetCoreServiceTypes()
		{
			if (ServiceTypeCache != null && ServiceTypeCache.Count > 0)
			{
				return ServiceTypeCache;
			}

			ServiceTypeCache = new Dictionary<Type, CoreServiceAttribute>();

			foreach (var kvp in ScriptCompiler.Assemblies.SelectMany(
				asm => GetCoreServiceTypes(asm).Where(kvp => !ServiceTypeCache.ContainsKey(kvp.Key))))
			{
				ServiceTypeCache.Add(kvp.Key, kvp.Value);
			}

			return ServiceTypeCache;
		}

		private static IEnumerable<KeyValuePair<Type, CoreServiceAttribute>> GetCoreServiceTypes(Assembly asm)
		{
			CoreServiceAttribute[] attrs;

			foreach (var typeOf in asm.GetTypes().Where(t => t != null && t.IsClass && t.IsAbstract && t.IsSealed))
			{
				attrs = typeOf.GetCustomAttributes<CoreServiceAttribute>(false);

				if (attrs != null && attrs.Length > 0)
				{
					yield return new KeyValuePair<Type, CoreServiceAttribute>(typeOf, attrs[0]);
				}
			}
		}
	}

	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public sealed class CoreServiceAttribute : Attribute
	{
		public string Name { get; set; }
		public string Version { get; set; }
		public int Priority { get; set; }
		public bool Debug { get; set; }
		public bool Quiet { get; set; }

		public CoreServiceAttribute(
			string name,
			string version,
			int priority = TaskPriority.Medium,
			bool debug = false,
			bool quiet = true)
		{
			Name = name;
			Version = version;
			Priority = priority;
			Debug = debug;
			Quiet = quiet;
		}
	}

	[PropertyObject]
	public sealed class CoreServiceInfo : ICorePluginInfo
	{
		private const BindingFlags SearchFlags =
			BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;

		private readonly PropertyInfo _OptionsProperty;
		private readonly MethodInfo _ConfigMethod;
		private readonly MethodInfo _InvokeMethod;
		private readonly MethodInfo _LoadMethod;
		private readonly MethodInfo _SaveMethod;
		private readonly MethodInfo _DisposeMethod;

		private readonly Type _TypeOf;

		private readonly int _Priority;

		private string _Name;

		private bool _Debug;
		private bool _Quiet;

		private Action _ConfigHandler;
		private Action _InvokeHandler;
		private Action _LoadHandler;
		private Action _SaveHandler;
		private Action _DisposeHandler;

		private CoreServiceOptions _Options;
		private VersionInfo _Version;

		public bool OptionsSupported => _OptionsProperty != null;
		public bool ConfigSupported => _ConfigMethod != null;
		public bool InvokeSupported => _InvokeMethod != null;
		public bool LoadSupported => _LoadMethod != null;
		public bool SaveSupported => _SaveMethod != null;
		public bool DisposeSupported => _DisposeMethod != null;

		public bool Active { get => !Disposed; set { } }

		public bool Configured { get; private set; }
		public bool Invoked { get; private set; }
		public bool Disposed { get; private set; }

		public Assembly DynamicAssembly { get; private set; }
		public FileInfo DynamicAssemblyFile { get; private set; }

		[CommandProperty(VitaNexCore.Access)]
		public bool Dynamic => (DynamicAssembly != null && DynamicAssemblyFile != null);

		[CommandProperty(VitaNexCore.Access)]
		public int Priority => _Priority;

		[CommandProperty(VitaNexCore.Access)]
		public Type TypeOf => _TypeOf;

		[CommandProperty(VitaNexCore.Access)]
		public VersionInfo Version => _Version ?? (_Version = new VersionInfo());

		[CommandProperty(VitaNexCore.Access)]
		public string Name => _Name ?? (_Name = _TypeOf.Name);

		[CommandProperty(VitaNexCore.Access)]
		public string FullName => String.Format("{0}/{1}", Name, Version);

		[CommandProperty(VitaNexCore.Access)]
		public bool Debug
		{
			get => _Debug;
			set
			{
				_Debug = value;
				SaveState();
			}
		}

		[CommandProperty(VitaNexCore.Access)]
		public bool Quiet
		{
			get => _Quiet;
			set
			{
				_Quiet = value;
				SaveState();
			}
		}

		[CommandProperty(VitaNexCore.Access)]
		public CoreServiceOptions Options
		{
			get => OptionsSupported && _OptionsProperty.CanRead
					? (CoreServiceOptions)_OptionsProperty.GetValue(_TypeOf, null)
					: (_Options ?? (_Options = new CoreServiceOptions(_TypeOf)));
			set
			{
				if (OptionsSupported && _OptionsProperty.CanWrite)
				{
					_OptionsProperty.SetValue(_TypeOf, value, null);
				}
				else
				{
					_Options = (value ?? new CoreServiceOptions(_TypeOf));
				}
			}
		}

		public CoreServiceInfo(Type t, CoreServiceAttribute attr)
			: this(t, attr.Version, attr.Name, attr.Priority, attr.Debug, attr.Quiet)
		{ }

		public CoreServiceInfo(Type t, string version, string name, int priority, bool debug, bool quiet)
		{
			_TypeOf = t;
			_Name = name;
			_Version = version;
			_Priority = priority;
			_Debug = debug;
			_Quiet = quiet;

			if (VitaNexCore.ServiceAssemblies.Contains(_TypeOf.Assembly))
			{
				DynamicAssembly = _TypeOf.Assembly;
				DynamicAssemblyFile = new FileInfo(DynamicAssembly.Location);
				_Version = DynamicAssembly.GetName().Version.ToString();
			}

			_OptionsProperty = _TypeOf.GetProperty("CSOptions", SearchFlags);
			_ConfigMethod = _TypeOf.GetMethod("CSConfig", SearchFlags);
			_InvokeMethod = _TypeOf.GetMethod("CSInvoke", SearchFlags);
			_DisposeMethod = _TypeOf.GetMethod("CSDispose", SearchFlags);
			_LoadMethod = _TypeOf.GetMethod("CSLoad", SearchFlags);
			_SaveMethod = _TypeOf.GetMethod("CSSave", SearchFlags);

			VitaNexCore.RegisterPlugin(this);
		}

		public void OnRegistered()
		{
			LoadState();
		}

		public void OnConfigured()
		{
			Configured = true;
		}

		public void OnInvoked()
		{
			Invoked = true;
		}

		public void OnDisposed()
		{
			Disposed = true;
		}

		public void OnSaved()
		{ }

		public void OnLoaded()
		{ }

		public void SaveState()
		{
			IOUtility.EnsureFile(VitaNexCore.CacheDirectory + "/States/" + _TypeOf.FullName + ".state", true)
					 .Serialize(
						 writer =>
						 {
							 var version = writer.SetVersion(0);

							 switch (version)
							 {
								 case 0:
								 {
									 writer.Write(_Name);
									 writer.Write(_Debug);
									 writer.Write(_Quiet);
								 }
								 break;
							 }
						 });
		}

		public void LoadState()
		{
			IOUtility.EnsureFile(VitaNexCore.CacheDirectory + "/States/" + _TypeOf.FullName + ".state")
					 .Deserialize(
						 reader =>
						 {
							 if (reader.End())
							 {
								 return;
							 }

							 var version = reader.GetVersion();

							 switch (version)
							 {
								 case 0:
								 {
									 _Name = reader.ReadString();
									 _Debug = reader.ReadBool();
									 _Quiet = reader.ReadBool();
								 }
								 break;
							 }
						 });
		}

		public void SaveOptions()
		{
			IOUtility.EnsureFile(VitaNexCore.CacheDirectory + "/Options/" + _TypeOf.FullName + ".opt", true)
					 .Serialize(
						 writer =>
						 {
							 var version = writer.SetVersion(0);

							 switch (version)
							 {
								 case 0:
								 {
									 writer.WriteType(
										 Options,
										 t =>
										 {
											 if (t != null)
											 {
												 Options.Serialize(writer);
											 }
										 });
								 }
								 break;
							 }
						 });
		}

		public void LoadOptions()
		{
			IOUtility.EnsureFile(VitaNexCore.CacheDirectory + "/Options/" + _TypeOf.FullName + ".opt")
					 .Deserialize(
						 reader =>
						 {
							 if (reader.End())
							 {
								 return;
							 }

							 var version = reader.GetVersion();

							 switch (version)
							 {
								 case 0:
								 {
									 if (reader.ReadType() != null)
									 {
										 Options.Deserialize(reader);
									 }
								 }
								 break;
							 }
						 });
		}

		public Action GetConfigHandler(bool throwException = false)
		{
			if (_ConfigHandler != null)
			{
				return _ConfigHandler;
			}

			if (ConfigSupported)
			{
				return (_ConfigHandler = () => _ConfigMethod.Invoke(_TypeOf, null));
			}

			if (throwException)
			{
				throw new NotSupportedException(
					"The 'CSConfig' method of '" + _TypeOf.FullName + "' does not exist or can not be accessed");
			}

			return null;
		}

		public Action GetInvokeHandler(bool throwException = false)
		{
			if (_InvokeHandler != null)
			{
				return _InvokeHandler;
			}

			if (InvokeSupported)
			{
				return (_InvokeHandler = () => _InvokeMethod.Invoke(_TypeOf, null));
			}

			if (throwException)
			{
				throw new NotSupportedException(
					"The 'CSInvoke' method of '" + _TypeOf.FullName + "' does not exist or can not be accessed");
			}

			return null;
		}

		public Action GetDisposeHandler(bool throwException = false)
		{
			if (_DisposeHandler != null)
			{
				return _DisposeHandler;
			}

			if (DisposeSupported)
			{
				return (_DisposeHandler = () => _DisposeMethod.Invoke(_TypeOf, null));
			}

			if (throwException)
			{
				throw new NotSupportedException(
					"The 'CSDispose' method of '" + _TypeOf.FullName + "' does not exist or can not be accessed");
			}

			return null;
		}

		public Action GetLoadHandler(bool throwException = false)
		{
			if (_LoadHandler != null)
			{
				return _LoadHandler;
			}

			if (LoadSupported)
			{
				return (_LoadHandler = () => _LoadMethod.Invoke(_TypeOf, null));
			}

			if (throwException)
			{
				throw new NotSupportedException(
					"The 'CSLoad' method of '" + _TypeOf.FullName + "' does not exist or can not be accessed");
			}

			return null;
		}

		public Action GetSaveHandler(bool throwException = false)
		{
			if (_SaveHandler != null)
			{
				return _SaveHandler;
			}

			if (SaveSupported)
			{
				return (_SaveHandler = () => _SaveMethod.Invoke(_TypeOf, null));
			}

			if (throwException)
			{
				throw new NotSupportedException(
					"The 'CSSave' method of '" + _TypeOf.FullName + "' does not exist or can not be accessed");
			}

			return null;
		}

		public void ToConsole(string[] lines)
		{
			if (Quiet || lines == null || lines.Length == 0)
			{
				return;
			}

			foreach (var line in lines)
			{
				ToConsole(line);
			}
		}

		public void ToConsole(string format, params object[] args)
		{
			if (Quiet)
			{
				return;
			}

			lock (VitaNexCore.ConsoleLock)
			{
				Console.Write('[');
				Utility.PushColor(ConsoleColor.Cyan);
				Console.Write(Name);
				Utility.PopColor();
				Console.Write("]: ");
				Utility.PushColor(ConsoleColor.DarkCyan);
				Console.WriteLine(format, args);
				Utility.PopColor();
			}
		}

		public void ToConsole(Exception[] errors)
		{
			if (errors == null || errors.Length == 0)
			{
				return;
			}

			foreach (var e in errors)
			{
				ToConsole(e);
			}
		}

		public void ToConsole(Exception e)
		{
			if (e == null)
			{
				return;
			}

			lock (VitaNexCore.ConsoleLock)
			{
				Console.Write('[');
				Utility.PushColor(ConsoleColor.Cyan);
				Console.Write(Name);
				Utility.PopColor();
				Console.Write("]: ");
				Utility.PushColor(ConsoleColor.Red);
				Console.WriteLine((Quiet && !Debug) ? e.Message : e.ToString());
				Utility.PopColor();
			}

			if (Debug)
			{
				e.Log(IOUtility.EnsureFile(VitaNexCore.LogsDirectory + "/Debug/" + TypeOf.FullName + ".log"));
			}
		}

		public void CompileControlPanel(SuperGump g, int x, int y, int w, int h)
		{
			if (Options != null)
			{
				Options.CompileControlPanel(g, x, y, w, h);
			}
		}

		public override int GetHashCode()
		{
			return _TypeOf.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj is ICorePluginInfo && Equals((ICorePluginInfo)obj);
		}

		public bool Equals(ICorePluginInfo cp)
		{
			return !ReferenceEquals(cp, null) && (ReferenceEquals(cp, this) || cp.TypeOf == _TypeOf);
		}

		public int CompareTo(ICorePluginInfo cp)
		{
			return cp == null || cp.Disposed ? -1 : _Priority.CompareTo(cp.Priority);
		}

		public override string ToString()
		{
			return FullName;
		}
	}

	public class CoreServiceOptions : PropertyObject
	{
		private CoreServiceInfo _Service;
		private Type _ServiceType;

		[CommandProperty(VitaNexCore.Access)]
		public CoreServiceInfo Service => _Service ?? (_Service = VitaNexCore.GetService(_ServiceType));

		[CommandProperty(VitaNexCore.Access)]
		public Type ServiceType => _ServiceType;

		[CommandProperty(VitaNexCore.Access)]
		public string ServiceName => Service.Name;

		[CommandProperty(VitaNexCore.Access)]
		public string ServiceVersion => Service.Version;

		[CommandProperty(VitaNexCore.Access)]
		public bool ServiceDebug { get => Service.Debug; set => Service.Debug = value; }

		[CommandProperty(VitaNexCore.Access)]
		public bool ServiceQuiet { get => Service.Quiet; set => Service.Quiet = value; }

		public CoreServiceOptions(Type serviceType)
		{
			_ServiceType = serviceType;
		}

		public CoreServiceOptions(GenericReader reader)
			: base(reader)
		{ }

		public override void Clear()
		{
			ServiceDebug = false;
			ServiceQuiet = false;
		}

		public override void Reset()
		{
			ServiceDebug = false;
			ServiceQuiet = false;
		}

		public virtual void ToConsole(string[] lines)
		{
			Service.ToConsole(lines);
		}

		public virtual void ToConsole(string format, params object[] args)
		{
			Service.ToConsole(format, args);
		}

		public virtual void ToConsole(Exception[] errors)
		{
			Service.ToConsole(errors);
		}

		public virtual void ToConsole(Exception e)
		{
			Service.ToConsole(e);
		}

		public virtual void CompileControlPanel(SuperGump g, int x, int y, int w, int h)
		{ }

		public override string ToString()
		{
			return "Service Options";
		}

		public override void Serialize(GenericWriter writer)
		{
			writer.Write(VitaNexCore.Version);

			base.Serialize(writer);

			var version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
					writer.WriteType(_ServiceType);
					break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			/*string vncVersion = reader.ReadString();*/
			reader.ReadString();

			base.Deserialize(reader);

			var version = reader.GetVersion();

			switch (version)
			{
				case 0:
					_ServiceType = reader.ReadType();
					break;
			}
		}
	}
}
