using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Regions;

public class HordeManager
{
    private List<BaseCreature> horde;
    private Timer moveTimer;
    private Point3D currentLocation;
    private Map currentMap;
    private Type[] monsterTypes;
    private int spawnRange;
    private int hordeSize;

    public HordeManager(Point3D initialLocation, Map map, Type[] monsters, int range, int size)
    {
        horde = new List<BaseCreature>();
        currentLocation = initialLocation;
        currentMap = map;
        monsterTypes = monsters;
        spawnRange = range;
        hordeSize = size;

        SpawnHorde();
        StartMoveTimer();
    }

    private void SpawnHorde()
    {
        for (int i = 0; i < hordeSize; i++)
        {
            BaseCreature monster = (BaseCreature)Activator.CreateInstance(monsterTypes[Utility.Random(monsterTypes.Length)]);
            Point3D spawnLocation = GetRandomSpawnLocation();
            monster.MoveToWorld(spawnLocation, currentMap);
            horde.Add(monster);
        }
    }

    private Point3D GetRandomSpawnLocation()
    {
        Point3D spawnLocation;
        do
        {
            int x = currentLocation.X + Utility.RandomMinMax(-spawnRange, spawnRange);
            int y = currentLocation.Y + Utility.RandomMinMax(-spawnRange, spawnRange);
            int z = currentMap.GetAverageZ(x, y);
            spawnLocation = new Point3D(x, y, z);
        } while (!IsLocationValid(spawnLocation));

        return spawnLocation;
    }

    private bool IsLocationValid(Point3D location)
    {
        return currentMap.CanFit(location, 16, true, false) && currentMap.LineOfSight(currentLocation, location);
    }

    private void StartMoveTimer()
    {
        moveTimer = Timer.DelayCall(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1), MoveHorde);
    }

    private void MoveHorde()
    {
        Point3D newLocation = GetNewLocation();
        foreach (BaseCreature monster in horde)
        {
            if (monster != null && !monster.Deleted)
            {
                Point3D moveToLocation = GetNearbyLocation(monster.Location, newLocation);
                if (IsLocationValid(moveToLocation))
                {
                    monster.MoveToWorld(moveToLocation, currentMap);
                }
            }
        }
        currentLocation = newLocation;
    }

    private Point3D GetNearbyLocation(Point3D currentLocation, Point3D targetLocation)
    {
        int x = targetLocation.X + Utility.RandomMinMax(-5, 5);
        int y = targetLocation.Y + Utility.RandomMinMax(-5, 5);
        int z = currentMap.GetAverageZ(x, y);
        return new Point3D(x, y, z);
    }

    private Point3D GetNewLocation()
    {
        Point3D spawnLocation;
        do
        {
            int x = currentLocation.X + Utility.RandomMinMax(-spawnRange, spawnRange);
            int y = currentLocation.Y + Utility.RandomMinMax(-spawnRange, spawnRange);
            int z = currentMap.GetAverageZ(x, y);
            spawnLocation = new Point3D(x, y, z);
        } while (!IsLocationValid(spawnLocation));

        return spawnLocation;
    }

    public void Stop()
    {
        if (moveTimer != null)
        {
            moveTimer.Stop();
            moveTimer = null;
        }

        foreach (BaseCreature monster in horde)
        {
            if (monster != null && !monster.Deleted)
            {
                monster.Delete();
            }
        }

        horde.Clear();
    }
}
