using Server;
using Server.Commands;
using Server.Mobiles;
using System;

public class HordeSpawner
{
    public static void Initialize()
    {
        CommandSystem.Register("SpawnHorde", AccessLevel.GameMaster, new CommandEventHandler(SpawnHorde_OnCommand));
    }

    [Usage("SpawnHorde")]
    [Description("Spawns a horde of custom minions that attack players and flee when damaged.")]
    public static void SpawnHorde_OnCommand(CommandEventArgs e)
    {

        //Point3D initialLocation = e.Mobile.Location;
        //Map map = e.Mobile.Map;
        //Type[] https://www.nvidia.com/en-us/geforce/drivers/monsters = new Type[] { typeof(CustomHordeMinion) };
        //int range = 10;
        //int size = 20;

        //HordeManager hordeManager = new HordeManager(initialLocation, map, monsters, range, size);
        //e.Mobile.SendMessage("Horde of custom minions spawned.");


        // Show the custom gump
        //e.Mobile.SendGump(new SkillGoalGump(e.Mobile));
    }
}
