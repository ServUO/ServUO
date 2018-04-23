/* Created by M_0_h 
   Please do not redistribute this file without permission
   Feel free to modify the file for your own use as you please */
using System;
using Server;
using System.Collections;
using Server.Items;
using Server.Spells;
using Server.Misc;
using Server.Engines.CannedEvil;
using Server.Targeting;


namespace Server.Mobiles
{
	public class Aghor : BaseCreature
	{
        private DateTime m_NextAbility = DateTime.Now + TimeSpan.FromSeconds(5);
        private DateTime m_NextWall = DateTime.Now + TimeSpan.FromSeconds(20);

        [Constructable]
		public Aghor() : base( AIType.AI_Mage, FightMode.Weakest, 10, 1, 0.1, 0.4 )
		{
			Name = "Agho'r";
            Title = "the Cold-Hearted";
			Body = 43;
			BaseSoundID = 357;
            Hue = 1152;

			SetStr( 600, 700 );
			SetDex( 176, 195 );
			SetInt( 201, 225 );

			SetHits( 22226, 24322 );

			SetDamage( 13, 23 );

			SetSkill( SkillName.EvalInt, 140.1, 150.0 );
			SetSkill( SkillName.Magery, 110.1, 120.0 );
			SetSkill( SkillName.MagicResist, 175.1, 185.0 );
			SetSkill( SkillName.Tactics, 180.1, 190.0 );
			SetSkill( SkillName.Wrestling, 100.1, 120.0 );

            SetDamageType(ResistanceType.Physical, 0);
            SetDamageType(ResistanceType.Cold, 100);

			SetResistance( ResistanceType.Physical, 55, 65 );
			SetResistance( ResistanceType.Fire, 10, 20 );
			SetResistance( ResistanceType.Cold, 60, 70 );
			SetResistance( ResistanceType.Poison, 20, 30 );
			SetResistance( ResistanceType.Energy, 30, 40 );

			Fame = 18000;
			Karma = -18000;

			VirtualArmor = 60;

			//if ( 0.07 >= Utility.RandomDouble() )
				//PackItem( new DailyCurtian() );
		}

        public override void OnThink()
        {
            if (Alive && !Blessed && Map != Map.Internal && Map != null && Combatant != null)
            {
                if (m_NextAbility <= DateTime.Now /*&& Hits > HitsMax * 0.03 */)
                {
                    switch (Utility.Random(3))
                    {
                        case 1: DoBlizzard();
                            break;
                        case 2: DoSummon();
                            break;
                        case 0: DoIceNova();
                            break;
                    }
                    m_NextAbility = DateTime.Now + TimeSpan.FromSeconds(Utility.Random(11) + 12);
                }
                if (m_NextWall <= DateTime.Now)
                {
                    if (Utility.RandomBool())
                    {
                        DoCircleOfIce();
                        m_NextWall = DateTime.Now + TimeSpan.FromSeconds(40);
                    }
                    else
                        m_NextWall = DateTime.Now + TimeSpan.FromSeconds(5);
                }
            }
        }

        #region CircleOfIce

        public void DoCircleOfIce()
        {
            for (int x = -8; x <= 8; ++x)
            {
                for (int y = -8; y <= 8; ++y)
                {
                    double dist = Math.Sqrt(x * x + y * y);

                    if (dist > 7 && dist < 9)
                    {
                        Point3D loc = new Point3D(X + x, Y + y, Z);
                        if (Map.CanFit(loc, 0, true))
                        {
                            Item item = new InternalItem();
                            item.MoveToWorld(loc, Map);
                            Effects.SendLocationParticles(EffectItem.Create(loc, Map, EffectItem.DefaultDuration), 0x376A, 9, 10, 5029);
                        }
                    }
                }
            }
        }

        [DispellableField]
        private class InternalItem : Item
        {            
            public override bool BlocksFit { get { return true; } }
            private Timer m_Timer;

            public InternalItem()
                : base(0x08E2)
            {
                Movable = false;
                ItemID = Utility.RandomList(2274, 2275, 2272, 2273, 2279, 2280);
                Name = "Ice";
                Hue = 1152;
                m_Timer = new InternalTimer(this, TimeSpan.FromSeconds(30.0));
                m_Timer.Start();
            }

            public InternalItem(Serial serial)
                : base(serial)
            {
            }
            
            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);

                writer.Write((int)1); // version
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);

                int version = reader.ReadInt();

                switch (version)
                {
                    case 1:
                        {
                            break;
                        }
                    case 0:
                        {
                            break;
                        }
                }
            }

            public override void OnAfterDelete()
            {
                base.OnAfterDelete();

                if (m_Timer != null)
                    m_Timer.Stop();
            }
        }

        private class InternalTimer : Timer
        {
            private InternalItem m_Item;

            public InternalTimer(InternalItem item, TimeSpan duration)
                : base(duration)
            {
                m_Item = item;
            }

            protected override void OnTick()
            {
                m_Item.Delete();
            }
        }
        #endregion

        #region IceNova

        public void DoIceNova()
        {
            AbilityCollection.AreaEffect(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1), 0.1, Map, Location, 14000, 13, 1151, 4, 12, 3, false, true, false);
            Timer.DelayCall(TimeSpan.FromSeconds(1.5), new TimerStateCallback(IceNova_Callback), new object[]{this, Location});
        }

        public static void IceNova_Callback(object state)
        {
            object[] states = ((object[])state);
            Aghor ag = states[0] as Aghor;
            Point3D loc = ((Point3D)states[1]);
            if (ag == null || !ag.Alive || ag.Map == Map.Internal)
                return;

            Effects.PlaySound(ag.Location, ag.Map, 534);

            IPooledEnumerable eable = ag.Map.GetMobilesInRange(loc, 12);

            if (eable == null)
                return;

            foreach (Mobile m in eable)
            {
                if (m is BaseCreature)
                {
                    BaseCreature b = m as BaseCreature;

                    if (b == null)
                        return;

                    if (b.Alive && !b.Blessed && !b.IsDeadBondedPet && b.CanBeHarmful(ag) && ((BaseCreature)ag).IsEnemy(b) && b.Map != null && b.Map != Map.Internal && b != null)
                    {
                        AOS.Damage(b, ag, Utility.RandomMinMax(100, 150), 0, 0, 100, 0, 0);
                        ag.DoFreeze(b, 20);
                    }
                }
                else if (m is PlayerMobile)
                {
                    PlayerMobile p = m as PlayerMobile;

                    if (p == null)
                        return;

                    if (p.Alive && !p.Blessed && p.AccessLevel == AccessLevel.Player && p.Map != null && p.Map != Map.Internal && p != null)
                    {
                        AOS.Damage(p, ag, Utility.RandomMinMax(50, 100), 0, 0, 100, 0, 0);
                        ag.DoFreeze(p, 3);
                    }
                }
            }
            eable.Free();
        }

        #endregion

        #region Blizzard

        public void DoBlizzard()
        {
            for (int i = 0; i < 30; ++i)
            {
                int x = X + Utility.Random(25) - 12;
                int y = Y + Utility.Random(25) - 12;
                int z = Map.GetAverageZ(x, y);

                Point3D loc = new Point3D(x, y, z);

                if (Map.CanFit(loc, 0, true))
                {
                    double delay = 5 * Utility.RandomDouble();
                    Timer.DelayCall(TimeSpan.FromSeconds(delay), new TimerStateCallback(BlizzardEffect_Callback), new object[]{this, loc});
                    Timer.DelayCall(TimeSpan.FromSeconds(delay + 2.5), new TimerStateCallback(BlizzardDamage_Callback), new object[] { this, loc });
                }
            }
        }

        public static void BlizzardEffect_Callback(object state)
        {
            object[] states = ((object[])state);
            Mobile ag = states[0] as Mobile;
            Point3D loc = ((Point3D)states[1]);

            if (ag == null || !ag.Alive || ag.Map == Map.Internal)
                return;

            IEntity to = new Entity(Serial.Zero, new Point3D(loc.X, loc.Y, loc.Z), ag.Map);
            IEntity from = new Entity(Serial.Zero, new Point3D(loc.X, loc.Y, loc.Z + 50), ag.Map);
            Effects.SendMovingEffect(from, to, 14036, 2, 16, false, true, 1151, 4);
            Effects.PlaySound(loc, ag.Map, 0x1E5);
        }

        public static void BlizzardDamage_Callback(object state)
        {
            //Point3D loc = (Point3D)state;

            object[] states = ((object[])state);
            Mobile ag = states[0] as Mobile;
            Point3D loc = ((Point3D)states[1]);

            if (ag == null || !ag.Alive || ag.Map == Map.Internal)
                return;

            IPooledEnumerable eable = ag.Map.GetMobilesInRange(loc, 0);
            foreach (Mobile m in eable)
            {
                if (m.Blessed || m == null || m.Map == Map.Internal || m.Map == null || !m.Alive || !ag.CanBeHarmful(m))                
                    return;

                else if (m is PlayerMobile)
                {
                    PlayerMobile p = m as PlayerMobile;

                    if (p.AccessLevel == AccessLevel.Player)
                        AOS.Damage(p, ag, 100, 0, 0, 0, 100, 0);
                    else
                        p.SendMessage("With your godly powers you avoid the damage");
                }

                else if (m is BaseCreature)
                {
                    BaseCreature b = m as BaseCreature;

                    if (b.IsEnemy(ag))
                        AOS.Damage(b, ag, 300, 0, 0, 0, 100, 0);
                }
            }
	        eable.Free();
        }
        #endregion

        #region Summon

        public void DoSummon()
        {
            if (this.Map != null)
            {
                Map map = this.Map;
                int amount = Utility.RandomMinMax(6, 8);

                for (int l = 0; l < amount; ++l)
                {
                    for (int k = 0; k < 1; ++k)
                    {
                        bool validLocation = false;
                        Point3D loc = this.Location;
                        for (int j = 0; !validLocation && j < 10; ++j)
                        {
                            int x = X + Utility.Random(11) - 5;
                            int y = Y + Utility.Random(11) - 5;
                            int z = map.GetAverageZ(x, y);

                            if (validLocation = map.CanFit(x, y, this.Z, 16, false, false))
                                loc = new Point3D(x, y, Z);
                            else if (validLocation = map.CanFit(x, y, z, 16, false, false))
                                loc = new Point3D(x, y, z);
                        }
                        switch (Utility.Random(4))
                        {
                            case 0: IceSerpent serpent = new IceSerpent();
                                serpent.MoveToWorld(loc, map);
                                break;
                            case 1: IceFiend fiend = new IceFiend();
                                fiend.MoveToWorld(loc, map);
                                break;
                            case 2: IceElemental ice = new IceElemental();
                                ice.MoveToWorld(loc, map);
                                break;
                            case 3: SnowElemental snow = new SnowElemental();
                                snow.MoveToWorld(loc, map);
                                break;
                        }   
                    }
                }
            }
        }
        #endregion

        #region Teleport

        public void DoTeleport(Mobile target)
        {
            if ((GetDistanceToSqrt(target) <= 1 || GetDistanceToSqrt(target) >= 12) || !CanSee(target))
                return;

            bool validLocation = false;
            Point3D loc = target.Location;
            for (int j = 0; !validLocation && j < 10; ++j)
            {
                int x = target.X + Utility.Random(3) - 1;
                int y = target.Y + Utility.Random(3) - 1;
                int z = Map.GetAverageZ(x, y);

                if (validLocation = Map.CanFit(x, y, target.Z, 16, false, false))
                    loc = new Point3D(x, y, Z);
                else if (validLocation = Map.CanFit(x, y, z, 16, false, false))
                    loc = new Point3D(x, y, z);
            }
            Effects.SendLocationParticles(EffectItem.Create(Location, Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 2023);
            Effects.SendLocationParticles(EffectItem.Create(loc, Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 5023);
            MoveToWorld(loc, Map);

            Combatant = target;
        }
        #endregion

        public override void OnDamagedBySpell(Mobile from)
        {
            if (Utility.Random(5) == 1 && from != null && InRange(from, 12) && from.Map == Map)
            {
                DoTeleport(from);
            }
        }

        #region Freeze
        public override void OnGaveMeleeAttack(Mobile defender)
        {
            if (!AFrozen(defender) && Utility.RandomBool())
                DoFreeze(defender, 5);
            base.OnGaveMeleeAttack(defender);
        }

        public void DoFreeze(Mobile from, int duration)
        {
            ExpireTimer timer = (ExpireTimer)m_Table[from];

            if (timer != null )
            {
                timer.DoExpire();
                from.SendLocalizedMessage(1070831); // The freezing wind continues to blow!
            }
            else
                from.SendLocalizedMessage(1070832); // An icy wind surrounds you, freezing your lungs as you breathe!

            timer = new ExpireTimer(from, this, duration);
            timer.Start();
            m_Table[from] = timer;
        }

        public bool AFrozen(Mobile m)
        {
            ExpireTimer timer = (ExpireTimer)m_Table[m];
            return timer != null;
        }

        private static Hashtable m_Table = new Hashtable();

        private class FrozenItem : Item 
        {
            public override bool BlocksFit { get { return false; } }

            public FrozenItem()
                : base(Utility.RandomList(2279, 2281, 2276, 2272))
            {
                Hue = 1152;
                Name = "Ice";
            }

            public FrozenItem(Serial serial)
                : base(serial)
            {
            }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);

                writer.Write((int)1); // version
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);

                int version = reader.ReadInt();

                switch (version)
                {
                    case 1:
                        {
                            break;
                        }
                }
            }
        }

        private class ExpireTimer : Timer
        {
            private Mobile m_Mobile;
            private Mobile m_From;
            private int m_Count;
            private int m_MaxCount;
            private FrozenItem m_Item;

            public ExpireTimer(Mobile m, Mobile from, int maxCount)
                : base(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0))
            {
                m_Mobile = m;
                m_From = from;
                m_MaxCount = maxCount;
                m_Mobile.CantWalk = true;
                m_Item = new FrozenItem();
                m_Item.MoveToWorld(m_Mobile.Location, m_Mobile.Map);
                Priority = TimerPriority.TwoFiftyMS;
            }

            public void DoExpire()
            {
                Stop();
                m_Mobile.CantWalk = false;
                m_Item.Delete();
                m_Table.Remove(m_Mobile);
            }

            public void DrainLife()
            {
                if (m_Mobile.Alive)
                    m_Mobile.Damage(Utility.RandomMinMax(5,11), m_From);
                else
                    DoExpire();
            }

            protected override void OnTick()
            {
                DrainLife();

                if (++m_Count >= m_MaxCount)
                {
                    DoExpire();
                    m_Mobile.SendLocalizedMessage(1070830); // The icy wind dissipates.
                }
            }
        }
        #endregion

        public override void GenerateLoot()
		{
			AddLoot( LootPack.AosSuperBoss, 4);
		}

        public override bool AlwaysMurderer { get { return true; } }
        public override bool AutoDispel { get { return true; } }
        public override double AutoDispelChance { get { return 1.0; } }
        public override bool BardImmune { get { return !Core.SE; } }
        public override bool Unprovokable { get { return Core.SE; } }
        public override bool Uncalmable { get { return Core.SE; } }
        public override Poison PoisonImmune { get { return Poison.Deadly; } }

        public Aghor(Serial serial)
            : base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}