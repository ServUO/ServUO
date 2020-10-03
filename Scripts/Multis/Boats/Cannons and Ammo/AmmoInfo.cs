using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Items
{
    public interface ICannonAmmo
    {
        AmmunitionType AmmoType { get; }
    }

    public enum AmmunitionType
    {
        Empty,
        Cannonball,
        Grapeshot,
        FrostCannonball,
        FlameCannonball
    }

    public class AmmoInfo
    {
        private static Dictionary<Type, AmmoInfo> Infos { get; set; } = new Dictionary<Type, AmmoInfo>();

        public static void Initialize()
        {
            Infos[typeof(Cannonball)] = new AmmoInfo(typeof(HeavyCannonball), AmmunitionType.Cannonball, 1095804, 5000, 5000, 3);
            Infos[typeof(Grapeshot)] = new AmmoInfo(typeof(HeavyGrapeshot), AmmunitionType.Grapeshot, 1095741, 100, 150, 3);

            Infos[typeof(FlameCannonball)] = new AmmoInfo(typeof(HeavyFlameCannonball), AmmunitionType.FlameCannonball, 1149633, 5000, 5000, 3, true, 50, 50, 0, 0, 0, false);
            Infos[typeof(FrostCannonball)] = new AmmoInfo(typeof(HeavyFrostCannonball), AmmunitionType.FrostCannonball, 1149634, 30, 50, 3, true, 50, 0, 50, 0, 0, false);
        }

        public Type Type { get; set; }
        public AmmunitionType AmmoType { get; set; }
        public TextDefinition Name { get; set; }
        public int MinDamage { get; set; }
        public int MaxDamage { get; set; }
        public int LateralOffset { get; set; }
        public int PhysicalDamage { get; set; }
        public int FireDamage { get; set; }
        public int ColdDamage { get; set; }
        public int PoisonDamage { get; set; }
        public int EnergyDamage { get; set; }
        public bool SingleTarget { get; set; }
        public bool RequiresSurface { get; set; }

        public AmmoInfo(Type type, AmmunitionType ammoType, TextDefinition name, int minDamage, int maxDamage, int lateralOffset)
            : this(type, ammoType, name, minDamage, maxDamage, lateralOffset, true, 100, 0, 0, 0, 0, false)
        {
        }

        public AmmoInfo(Type type, AmmunitionType ammoType, TextDefinition name, int minDamage, int maxDamage, int lateralOffset, bool singleOnly)
            : this(type, ammoType, name, minDamage, maxDamage, lateralOffset, singleOnly, 100, 0, 0, 0, 0, false)
        {
        }

        public AmmoInfo(Type type, AmmunitionType ammoType, TextDefinition name, int minDamage, int maxDamage, int lateralOffset, bool singleOnly, int phys, int fire, int cold, int poison, int energy, bool requiresSurface)
        {
            Type = type;
            AmmoType = ammoType;
            Name = name;
            MinDamage = minDamage;
            MaxDamage = maxDamage;
            LateralOffset = lateralOffset;
            PhysicalDamage = phys;
            FireDamage = fire;
            ColdDamage = cold;
            PoisonDamage = poison;
            EnergyDamage = energy;
            SingleTarget = singleOnly;
            RequiresSurface = requiresSurface;
        }


        /// <summary>
        /// Gets damage for Rising Tides Cannons. This differentiates between the cannon sizes.
        /// </summary>
        /// <param name="cannon"></param>
        /// <returns></returns>
        public int GetDamage(BaseShipCannon cannon)
        {
            return Utility.RandomMinMax(MinDamage, MaxDamage);

            // Fucking EA, after rising tide, made all cannons deal the same amount of damage. I don't get it. If they ever pull their head out of their asses, or you
            // want, use the code below...

            /*if (AmmoType == AmmunitionType.Grapeshot)
            {
                return Utility.RandomMinMax(MinDamage, MaxDamage);
            }

            int baseDamage = Utility.RandomMinMax(info.MinDamage, info.MaxDamage);

            switch (cannon.Power)
            {
                default:
                case CannonPower.Light: return baseDamage;
                case CannonPower.Heavy: return baseDamage + 1500;
                case CannonPower.Massive: return baseDamage + 3000;
            }*/
        }

        public static AmmoInfo GetAmmoInfo(Type ammoType)
        {
            if (ammoType != null && Infos.ContainsKey(ammoType))
            {
                return Infos[ammoType];
            }

            return null;
        }

        public static AmmoInfo GetAmmoInfo(AmmunitionType ammoType)
        {
            return Infos.Values.FirstOrDefault(i => i.AmmoType == ammoType);
        }

        public static void GetSurfaceTop(ref Point3D p, Map map)
        {
            StaticTile[] tiles = map.Tiles.GetStaticTiles(p.X, p.Y, true);
            int z = p.Z;

            foreach (StaticTile tile in tiles)
            {
                ItemData id = TileData.ItemTable[tile.ID & TileData.MaxItemValue];

                if (id.Surface && (z == p.Z || tile.Z + id.CalcHeight > z))
                {
                    z = tile.Z + id.CalcHeight;
                }
            }

            if (z != p.Z)
                p.Z = z;
        }

        public static TextDefinition GetAmmoName(IShipCannon cannon)
        {
            AmmoInfo info = Infos.Values.FirstOrDefault(i => i.AmmoType == cannon.AmmoType);

            if (info == null)
            {
                return 1116033; // None 
            }

            return info.Name;
        }
    }
}
