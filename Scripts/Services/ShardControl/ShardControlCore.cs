/*using System;
using Server;
using Server.Gumps;

namespace CustomsFramework.Systems.ShardControl
{
public partial class ShardControlCore : BaseCore
{
#region Core Stuff
public static void Initialize()
{
ShardControlCore core = World.GetCore(typeof(ShardControlCore)) as ShardControlCore;

if (core == null)
{
core = new ShardControlCore();
core.Prep();
}
}

public const string SystemVersion = @"1.0";

public override string Name
{
get
{
return @"Shard Control";
}
}

public override string Description
{
get
{
return @"System that makes controlling a shard so easy, a child could do it.";
}
}

public override string Version
{
get
{
return SystemVersion;
}
}

public override AccessLevel EditLevel
{
get
{
return AccessLevel.Owner;
}
}

public override Gump SettingsGump
{
get
{
return null;
}
}
#endregion
}
}*/