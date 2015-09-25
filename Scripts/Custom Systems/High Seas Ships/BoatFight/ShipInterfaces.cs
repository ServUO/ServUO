namespace Server.Items
{
    public interface IShipWeapon : IEntity
    {
        int Facing { get; set; }
        bool FixedFacing { get; set; }
        Item Projectile { get; set; }
        bool IsPackable { get; set; }
        bool IsDraggable { get; set; }
        double WeaponDamageFactor { get; }
        void LoadWeapon(Mobile from, Item projectile);

        void PlaceWeapon(Mobile from, Point3D location, Map map);

        void StoreWeapon(Mobile from);
    }

    public interface IShipProjectile
    {
        int Range { get; set; }
        int FiringSpeed { get; set; }
        int AccuracyBonus { get; set; }
        int Area { get; set; }
        int FireDamage { get; set; }
        int PhysicalDamage { get; set; }
        int AnimationID { get; }
        int AnimationHue { get; }
        void OnHit(Mobile from, IShipWeapon weapon, IEntity target, Point3D targetloc);
    }
}