namespace Server.Items
{
	public interface ISiegeWeapon : IEntity
	{
		int Facing { get; set;}
		bool FixedFacing { get; set;}
		Item Projectile { get; set;}
		void LoadWeapon(Mobile from, Item projectile);
		void PlaceWeapon(Mobile from, Point3D location, Map map);
		void StoreWeapon(Mobile from);
		bool IsPackable { get; set;}
		bool IsDraggable { get; set;}
		double WeaponDamageFactor { get; }
	}

	public interface ISiegeProjectile
	{
		int Range { get; set; }
		int FiringSpeed { get; set; }
		int AccuracyBonus { get; set; }
		int Area { get; set; }
		int FireDamage { get; set; }
		int PhysicalDamage { get; set; }
		int AnimationID { get; }
		int AnimationHue { get; }
		void OnHit(Mobile from, ISiegeWeapon weapon, IEntity target, Point3D targetloc);
	}
}