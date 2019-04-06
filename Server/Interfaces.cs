#region References
using System;
using System.Collections.Generic;

using Server.ContextMenus;
using Server.Mobiles;
#endregion

namespace Server.Mobiles
{
	public interface IMount
	{
		Mobile Rider { get; set; }
		void OnRiderDamaged(Mobile from, ref int amount, bool willKill);
	}

	public interface IMountItem
	{
		IMount Mount { get; }
	}
}

namespace Server
{
	public interface IVendor
	{
		bool OnBuyItems(Mobile from, List<BuyItemResponse> list);
		bool OnSellItems(Mobile from, List<SellItemResponse> list);

		DateTime LastRestock { get; set; }
		TimeSpan RestockDelay { get; }
		void Restock();
	}

	public interface IPoint2D
	{
		int X { get; }
		int Y { get; }
	}

	public interface IPoint3D : IPoint2D
	{
		int Z { get; }
	}

	public interface ICarvable
	{
		bool Carve(Mobile from, Item item);
	}

	public interface IWeapon
	{
		int MaxRange { get; }
		void OnBeforeSwing(Mobile attacker, IDamageable damageable);
        TimeSpan OnSwing(Mobile attacker, IDamageable damageable);
		void GetStatusDamage(Mobile from, out int min, out int max);
		TimeSpan GetDelay(Mobile attacker);
	}

	public interface IHued
	{
		int HuedItemID { get; }
	}

	public interface ISpell
	{
		int ID { get; }
		bool IsCasting { get; }
		void OnCasterHurt();
		void OnCasterKilled();
		void OnConnectionChanged();
		bool OnCasterMoving(Direction d);
        bool CheckMovement(Mobile caster);
		bool OnCasterEquiping(Item item);
		bool OnCasterUsingObject(object o);
		bool OnCastInTown(Region r);
	}

	public interface IParty
	{
		void OnStamChanged(Mobile m);
		void OnManaChanged(Mobile m);
		void OnStatsQuery(Mobile beholder, Mobile beheld);
	}

	public interface ISpawner
	{
		bool UnlinkOnTaming { get; }
		Point3D HomeLocation { get; }
		int HomeRange { get; }

		void Remove(ISpawnable spawn);

		void GetSpawnProperties(ISpawnable spawn, ObjectPropertyList list);
		void GetSpawnContextEntries(ISpawnable spawn, Mobile m, List<ContextMenuEntry> list);
	}

	public interface ISpawnable : IEntity
	{
		void OnBeforeSpawn(Point3D location, Map map);
		void MoveToWorld(Point3D location, Map map);
		void OnAfterSpawn();

		ISpawner Spawner { get; set; }
	}

    public interface IDamageable : IEntity
    {
        int Hits { get; set; }
        int HitsMax { get; }
        bool Alive { get; }

        int PhysicalResistance { get; }
        int FireResistance { get; }
        int ColdResistance { get; }
        int PoisonResistance { get; }
        int EnergyResistance { get; }

		int Damage(int amount, Mobile attacker);

        void PlaySound(int soundID);
		
        void MovingEffect(IEntity to, int itemID, int speed, int duration, bool fixedDirection, bool explodes, int hue, int renderMode);
        void MovingEffect(IEntity to, int itemID, int speed, int duration, bool fixedDirection, bool explodes);

        void MovingParticles(IEntity to, int itemID, int speed, int duration, bool fixedDirection, bool explodes, int hue, int renderMode, int effect, int explodeEffect, int explodeSound, EffectLayer layer, int unknown);
        void MovingParticles(IEntity to, int itemID, int speed, int duration, bool fixedDirection, bool explodes, int hue, int renderMode, int effect, int explodeEffect, int explodeSound, int unknown);
        void MovingParticles(IEntity to, int itemID, int speed, int duration, bool fixedDirection, bool explodes, int effect, int explodeEffect, int explodeSound, int unknown);
        void MovingParticles(IEntity to, int itemID, int speed, int duration, bool fixedDirection, bool explodes, int effect, int explodeEffect, int explodeSound);

        void FixedEffect(int itemID, int speed, int duration, int hue, int renderMode);
        void FixedEffect(int itemID, int speed, int duration);

        void FixedParticles(int itemID, int speed, int duration, int effect, int hue, int renderMode, EffectLayer layer, int unknown);
        void FixedParticles(int itemID, int speed, int duration, int effect, int hue, int renderMode, EffectLayer layer);
        void FixedParticles(int itemID, int speed, int duration, int effect, EffectLayer layer, int unknown);
        void FixedParticles(int itemID, int speed, int duration, int effect, EffectLayer layer);
        void BoltEffect(int hue);
    }

    public interface IArtifact
    {
        int ArtifactRarity { get; }
    }
}