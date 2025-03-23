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
		public static IEnumerable<CoreModuleInfo> Modules
		{
			get
			{
				var idx = _Plugins.Count;

				while (--idx >= 0)
				{
					if (_Plugins.InBounds(idx) && _Plugins[idx] is CoreModuleInfo)
					{
						yield return (CoreModuleInfo)_Plugins[idx];
					}
				}
			}
		}

		public static int ModuleCount => Modules.Count();

		public static Dictionary<Type, CoreModuleAttribute> CoreModuleTypeCache { get; private set; }
		public static Assembly[] ModuleAssemblies { get; private set; }

		public static event Action<CoreModuleInfo> OnModuleEnabled;
		public static event Action<CoreModuleInfo> OnModuleDisabled;
		public static event Action<CoreModuleInfo> OnModuleConfigured;
		public static event Action<CoreModuleInfo> OnModuleInvoked;
		public static event Action<CoreModuleInfo> OnModuleSaved;
		public static event Action<CoreModuleInfo> OnModuleLoaded;
		public static event Action<CoreModuleInfo> OnModuleDisposed;

		public static CoreModuleInfo GetModule(Type t)
		{
			return Modules.FirstOrDefault(cmi => cmi.TypeOf.IsEqualOrChildOf(t));
		}

		public static CoreModuleInfo[] GetModules(string name, bool ignoreCase = true)
		{
			return FindModules(name, ignoreCase).ToArray();
		}

		public static IEnumerable<CoreModuleInfo> FindModules(string name, bool ignoreCase = true)
		{
			var c = ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture;

			var idx = _Plugins.Count;

			while (--idx >= 0)
			{
				var cp = _Plugins[idx] as CoreModuleInfo;

				if (cp != null && String.Equals(cp.Name, name, c))
				{
					yield return cp;
				}
			}
		}

		private static void CompileModules()
		{
			if (Compiled)
			{
				return;
			}

			ToConsole("Compiling Modules...");

			TryCatch(
				() =>
				{
					var files = ModulesDirectory.GetFiles("*.dll", SearchOption.AllDirectories);
					var asm = new List<Assembly>(files.Length);

					foreach (var file in files)
					{
						TryCatch(() => asm.Update(Assembly.LoadFrom(file.FullName)), ToConsole);
					}

					ModuleAssemblies = asm.ToArray();

					asm.AddRange(ScriptCompiler.Assemblies);
					asm.Prune();

					ScriptCompiler.Assemblies = asm.FreeToArray(true);
				},
				ToConsole);
		}

		public static void ConfigureModules()
		{
			ToConsole("Configuring Modules...");

			var types = GetCoreModuleTypes();

			foreach (var cmi in types.Select(kvp => new CoreModuleInfo(kvp.Key, kvp.Value)).OrderBy(cmi => cmi.Priority))
			{
				ConfigureModule(cmi);
			}
		}

		public static void ConfigureModule(CoreModuleInfo cmi)
		{
			if (cmi == null || !cmi.Enabled)
			{
				return;
			}

			cmi.ToConsole("Configuring...");

			if (!cmi.Configured)
			{
				if (cmi.ConfigSupported)
				{
					TryCatch(cmi.GetConfigHandler(), cmi.ToConsole);
				}

				TryCatch(cmi.OnConfigured, cmi.ToConsole);

				if (OnModuleConfigured != null)
				{
					TryCatch(() => OnModuleConfigured(cmi), cmi.ToConsole);
				}

				cmi.ToConsole("Done.");
			}
			else
			{
				cmi.ToConsole("Already configured, no action taken.");
			}
		}

		public static void InvokeModules()
		{
			ToConsole("Invoking Modules...");

			foreach (var cmi in Modules.OrderBy(cmi => cmi.Priority))
			{
				InvokeModule(cmi);
			}
		}

		public static void InvokeModule(CoreModuleInfo cmi)
		{
			if (cmi == null || !cmi.Enabled)
			{
				return;
			}

			cmi.ToConsole("Invoking...");

			if (!cmi.Invoked)
			{
				if (cmi.InvokeSupported)
				{
					TryCatch(cmi.GetInvokeHandler(), cmi.ToConsole);
				}

				TryCatch(cmi.OnInvoked, cmi.ToConsole);

				if (OnModuleInvoked != null)
				{
					TryCatch(() => OnModuleInvoked(cmi), cmi.ToConsole);
				}

				cmi.ToConsole("Done.");
			}
			else
			{
				cmi.ToConsole("Already invoked, no action taken.");
			}
		}

		public static void SaveModules()
		{
			ToConsole("Saving Modules...");

			foreach (var cmi in Modules.OrderBy(cmi => cmi.Priority))
			{
				SaveModule(cmi);
			}
		}

		public static void SaveModule(CoreModuleInfo cmi)
		{
			if (cmi == null || !cmi.Enabled)
			{
				return;
			}

			cmi.ToConsole("Saving...");

			TryCatch(cmi.SaveOptions, cmi.ToConsole);

			if (cmi.SaveSupported)
			{
				TryCatch(cmi.GetSaveHandler(), cmi.ToConsole);
			}

			TryCatch(cmi.OnSaved, cmi.ToConsole);

			if (OnModuleSaved != null)
			{
				TryCatch(() => OnModuleSaved(cmi), cmi.ToConsole);
			}

			cmi.ToConsole("Done.");
		}

		public static void LoadModules()
		{
			ToConsole("Loading Modules...");

			foreach (var cmi in Modules.OrderBy(cmi => cmi.Priority))
			{
				LoadModule(cmi);
			}
		}

		public static void LoadModule(CoreModuleInfo cmi)
		{
			if (cmi == null || !cmi.Enabled)
			{
				return;
			}

			cmi.ToConsole("Loading...");

			TryCatch(cmi.LoadOptions, cmi.ToConsole);

			if (cmi.LoadSupported)
			{
				TryCatch(cmi.GetLoadHandler(), cmi.ToConsole);
			}

			TryCatch(cmi.OnLoaded, cmi.ToConsole);

			if (OnModuleLoaded != null)
			{
				TryCatch(() => OnModuleLoaded(cmi), cmi.ToConsole);
			}

			cmi.ToConsole("Done.");
		}

		public static void DisposeModules()
		{
			ToConsole("Disposing Modules...");

			foreach (var cmi in Modules.OrderByDescending(cmi => cmi.Priority))
			{
				DisposeModule(cmi);
			}
		}

		public static void DisposeModule(CoreModuleInfo cmi)
		{
			if (cmi == null)
			{
				return;
			}

			cmi.ToConsole("Disposing...");

			if (!cmi.Disposed)
			{
				if (cmi.DisposeSupported)
				{
					TryCatch(cmi.GetDisposeHandler(), cmi.ToConsole);
				}

				TryCatch(cmi.OnDisposed, cmi.ToConsole);

				if (OnModuleDisposed != null)
				{
					TryCatch(() => OnModuleDisposed(cmi), cmi.ToConsole);
				}

				cmi.ToConsole("Done.");
			}
			else
			{
				cmi.ToConsole("Already disposed, no action taken.");
			}
		}

		public static void InvokeModuleEnabled(CoreModuleInfo cmi)
		{
			if (cmi == null)
			{
				return;
			}

			if (OnModuleEnabled != null)
			{
				TryCatch(() => OnModuleEnabled(cmi), ToConsole);
			}
		}

		public static void InvokeModuleDisabled(CoreModuleInfo cmi)
		{
			if (cmi == null)
			{
				return;
			}

			if (OnModuleDisabled != null)
			{
				OnModuleDisabled(cmi);
			}
		}

		public static Dictionary<Type, CoreModuleAttribute> GetCoreModuleTypes()
		{
			if (CoreModuleTypeCache != null && CoreModuleTypeCache.Count > 0)
			{
				return CoreModuleTypeCache;
			}

			CoreModuleTypeCache = new Dictionary<Type, CoreModuleAttribute>();

			foreach (var kvp in ScriptCompiler.Assemblies.SelectMany(GetCoreModuleTypes))
			{
				CoreModuleTypeCache[kvp.Key] = kvp.Value;
			}

			return CoreModuleTypeCache;
		}

		private static IEnumerable<KeyValuePair<Type, CoreModuleAttribute>> GetCoreModuleTypes(Assembly asm)
		{
			CoreModuleAttribute[] attrs;

			foreach (var typeOf in asm.GetTypes().Where(t => t != null && t.IsClass && t.IsAbstract && t.IsSealed))
			{
				attrs = typeOf.GetCustomAttributes<CoreModuleAttribute>(false);

				if (attrs != null && attrs.Length > 0)
				{
					yield return new KeyValuePair<Type, CoreModuleAttribute>(typeOf, attrs[0]);
				}
			}
		}
	}

	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public sealed class CoreModuleAttribute : Attribute
	{
		public string Name { get; set; }
		public string Version { get; set; }
		public bool Enabled { get; set; }
		public int Priority { get; set; }
		public bool Debug { get; set; }
		public bool Quiet { get; set; }

		public CoreModuleAttribute(
			string name,
			string version,
			bool enabled = false,
			int priority = TaskPriority.Medium,
			bool debug = false,
			bool quiet = true)
		{
			Name = name;
			Version = version;
			Enabled = enabled;
			Priority = priority;
			Debug = debug;
			Quiet = quiet;
		}
	}

	[PropertyObject]
	public sealed class CoreModuleInfo : ICorePluginInfo
	{
		private const BindingFlags SearchFlags =
			BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;

		private readonly PropertyInfo _OptionsProperty;
		private readonly MethodInfo _ConfigMethod;
		private readonly MethodInfo _DisabledMethod;
		private readonly MethodInfo _DisposeMethod;
		private readonly MethodInfo _EnabledMethod;
		private readonly MethodInfo _InvokeMethod;
		private readonly MethodInfo _LoadMethod;
		private readonly MethodInfo _SaveMethod;

		private readonly Type _TypeOf;

		private int _Priority;
		private string _Name;

		private bool _Enabled;
		private bool _Debug;
		private bool _Quiet;

		private Action _EnabledHandler;
		private Action _DisabledHandler;
		private Action _ConfigHandler;
		private Action _InvokeHandler;
		private Action _LoadHandler;
		private Action _SaveHandler;
		private Action _DisposeHandler;

		private CoreModuleOptions _Options;
		private VersionInfo _Version;

		public bool OptionsSupported => _OptionsProperty != null;
		public bool EnabledSupported => _EnabledMethod != null;
		public bool DisabledSupported => _DisabledMethod != null;
		public bool ConfigSupported => _ConfigMethod != null;
		public bool InvokeSupported => _InvokeMethod != null;
		public bool LoadSupported => _LoadMethod != null;
		public bool SaveSupported => _SaveMethod != null;
		public bool DisposeSupported => _DisposeMethod != null;

		public bool Active
		{
			get => Enabled && !Disposed;
			set => Enabled = !Disposed && value;
		}

		public bool Configured { get; private set; }
		public bool Invoked { get; private set; }
		public bool Disposed { get; private set; }
		public bool Deferred { get; private set; }

		public Assembly DynamicAssembly { get; private set; }
		public FileInfo DynamicAssemblyFile { get; private set; }

		[CommandProperty(VitaNexCore.Access)]
		public bool Dynamic => (DynamicAssembly != null && DynamicAssemblyFile != null);

		[CommandProperty(VitaNexCore.Access)]
		public int Priority
		{
			get => _Priority;
			set
			{
				_Priority = value;
				SaveState();
			}
		}

		[CommandProperty(VitaNexCore.Access)]
		public Type TypeOf => _TypeOf;

		[CommandProperty(VitaNexCore.Access)]
		public VersionInfo Version => _Version ?? (_Version = new VersionInfo());

		[CommandProperty(VitaNexCore.Access)]
		public string Name => _Name ?? (_Name = _TypeOf.Name);

		[CommandProperty(VitaNexCore.Access)]
		public string FullName => String.Format("{0}/{1}", Name, Version);

		[CommandProperty(VitaNexCore.Access)]
		public bool Enabled
		{
			get => _Enabled;
			set
			{
				if (!_Enabled && value)
				{
					_Enabled = true;

					if (!Configured)
					{
						VitaNexCore.ConfigureModule(this);
					}

					if (Deferred)
					{
						VitaNexCore.LoadModule(this);
					}

					if (!Invoked)
					{
						VitaNexCore.InvokeModule(this);
					}

					if (EnabledSupported && !Deferred)
					{
						VitaNexCore.TryCatch(GetEnabledHandler(), Options.ToConsole);
					}

					VitaNexCore.TryCatch(OnEnabled, Options.ToConsole);
					VitaNexCore.InvokeModuleEnabled(this);

					Deferred = false;
					SaveState();
				}
				else if (_Enabled && !value)
				{
					if (!Deferred)
					{
						VitaNexCore.SaveModule(this);

						if (DisabledSupported)
						{
							VitaNexCore.TryCatch(GetDisabledHandler(), Options.ToConsole);
						}

						VitaNexCore.TryCatch(OnDisabled, Options.ToConsole);
						VitaNexCore.InvokeModuleDisabled(this);
					}

					_Enabled = false;
					SaveState();
				}
			}
		}

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
		public CoreModuleOptions Options
		{
			get => OptionsSupported && _OptionsProperty.CanRead
					? (CoreModuleOptions)_OptionsProperty.GetValue(_TypeOf, null)
					: (_Options ?? (_Options = new CoreModuleOptions(_TypeOf)));
			set
			{
				if (OptionsSupported && _OptionsProperty.CanWrite)
				{
					_OptionsProperty.SetValue(_TypeOf, value, null);
				}
				else
				{
					_Options = (value ?? new CoreModuleOptions(_TypeOf));
				}
			}
		}

		public CoreModuleInfo(Type t, CoreModuleAttribute attr)
			: this(t, attr.Version, attr.Name, attr.Enabled, attr.Priority, attr.Debug, attr.Quiet)
		{ }

		public CoreModuleInfo(Type t, string version, string name, bool enabled, int priority, bool debug, bool quiet)
		{
			_TypeOf = t;
			_Name = name;
			_Version = version;
			_Enabled = enabled;
			_Priority = priority;
			_Debug = debug;
			_Quiet = quiet;

			Deferred = !_Enabled;

			if (VitaNexCore.ModuleAssemblies.Contains(_TypeOf.Assembly))
			{
				DynamicAssembly = _TypeOf.Assembly;
				DynamicAssemblyFile = new FileInfo(DynamicAssembly.Location);
				_Version = DynamicAssembly.GetName().Version.ToString();
			}

			_OptionsProperty = _TypeOf.GetProperty("CMOptions", SearchFlags);
			_ConfigMethod = _TypeOf.GetMethod("CMConfig", SearchFlags);
			_InvokeMethod = _TypeOf.GetMethod("CMInvoke", SearchFlags);
			_DisposeMethod = _TypeOf.GetMethod("CMDispose", SearchFlags);
			_LoadMethod = _TypeOf.GetMethod("CMLoad", SearchFlags);
			_SaveMethod = _TypeOf.GetMethod("CMSave", SearchFlags);
			_EnabledMethod = _TypeOf.GetMethod("CMEnabled", SearchFlags);
			_DisabledMethod = _TypeOf.GetMethod("CMDisabled", SearchFlags);

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

		public void OnEnabled()
		{ }

		public void OnDisabled()
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
									 writer.Write(_Enabled);
									 writer.Write(_Name);
									 writer.Write(_Priority);
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
									 _Enabled = reader.ReadBool();
									 _Name = reader.ReadString();
									 _Priority = reader.ReadInt();
									 _Debug = reader.ReadBool();
									 _Quiet = reader.ReadBool();

									 Deferred = !_Enabled;
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
					"The 'CMConfig' method of '" + _TypeOf.FullName + "' does not exist or can not be accessed");
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
					"The 'CMInvoke' method of '" + _TypeOf.FullName + "' does not exist or can not be accessed");
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
					"The 'CMDispose' method of '" + _TypeOf.FullName + "' does not exist or can not be accessed");
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
					"The 'CMLoad' method of '" + _TypeOf.FullName + "' does not exist or can not be accessed");
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
					"The 'CMSave' method of '" + _TypeOf.FullName + "' does not exist or can not be accessed");
			}

			return null;
		}

		public Action GetEnabledHandler(bool throwException = false)
		{
			if (_EnabledHandler != null)
			{
				return _EnabledHandler;
			}

			if (EnabledSupported)
			{
				return (_EnabledHandler = () => _EnabledMethod.Invoke(_TypeOf, null));
			}

			if (throwException)
			{
				throw new NotSupportedException(
					"The 'CMEnabled' method of '" + _TypeOf.FullName + "' does not exist or can not be accessed");
			}

			return null;
		}

		public Action GetDisabledHandler(bool throwException = false)
		{
			if (_DisabledHandler != null)
			{
				return _DisabledHandler;
			}

			if (EnabledSupported)
			{
				return (_DisabledHandler = () => _DisabledMethod.Invoke(_TypeOf, null));
			}

			if (throwException)
			{
				throw new NotSupportedException(
					"The 'CMEnabled' method of '" + _TypeOf.FullName + "' does not exist or can not be accessed");
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
				Utility.PushColor(ConsoleColor.Green);
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
				Utility.PushColor(ConsoleColor.Green);
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

	public class CoreModuleOptions : PropertyObject
	{
		private CoreModuleInfo _Module;
		private Type _ModuleType;

		[CommandProperty(VitaNexCore.Access)]
		public CoreModuleInfo Module => _Module ?? (_Module = VitaNexCore.GetModule(_ModuleType));

		[CommandProperty(VitaNexCore.Access)]
		public Type ModuleType => _ModuleType;

		[CommandProperty(VitaNexCore.Access)]
		public string ModuleName => Module.Name;

		[CommandProperty(VitaNexCore.Access)]
		public string ModuleVersion => Module.Version;

		[CommandProperty(VitaNexCore.Access)]
		public bool ModuleEnabled { get => Module.Enabled; set => Module.Enabled = value; }

		[CommandProperty(VitaNexCore.Access)]
		public int ModulePriority { get => Module.Priority; set => Module.Priority = value; }

		[CommandProperty(VitaNexCore.Access)]
		public bool ModuleDebug { get => Module.Debug; set => Module.Debug = value; }

		[CommandProperty(VitaNexCore.Access)]
		public bool ModuleQuiet { get => Module.Quiet; set => Module.Quiet = value; }

		public CoreModuleOptions(Type moduleType)
		{
			_ModuleType = moduleType;
		}

		public CoreModuleOptions(GenericReader reader)
			: base(reader)
		{ }

		public override void Clear()
		{
			ModuleDebug = false;
			ModuleQuiet = false;
		}

		public override void Reset()
		{
			ModuleDebug = false;
			ModuleQuiet = false;
		}

		public virtual void ToConsole(string[] lines)
		{
			Module.ToConsole(lines);
		}

		public virtual void ToConsole(string format, params object[] args)
		{
			Module.ToConsole(format, args);
		}

		public virtual void ToConsole(Exception[] errors)
		{
			Module.ToConsole(errors);
		}

		public virtual void ToConsole(Exception e)
		{
			Module.ToConsole(e);
		}

		public virtual void CompileControlPanel(SuperGump g, int x, int y, int w, int h)
		{ }

		public override string ToString()
		{
			return "Module Options";
		}

		public override void Serialize(GenericWriter writer)
		{
			writer.Write(VitaNexCore.Version);

			base.Serialize(writer);

			var version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
					writer.WriteType(_ModuleType);
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
					_ModuleType = reader.ReadType();
					break;
			}
		}
	}
}