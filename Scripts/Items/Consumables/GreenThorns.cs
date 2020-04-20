using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using System;

namespace Server.Items
{
    public class GreenThorns : Item, ICommodity
    {
        [Constructable]
        public GreenThorns()
            : this(1)
        {
        }

        [Constructable]
        public GreenThorns(int amount)
            : base(0xF42)
        {
            Stackable = true;
            Weight = 1.0;
            Hue = 0x42;
            Amount = amount;
        }

        public GreenThorns(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

        public override int LabelNumber => 1060837;// green thorns
        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042664); // You must have the object in your backpack to use it.
                return;
            }

            if (!from.CanBeginAction(typeof(GreenThorns)))
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1061908); // * You must wait a while before planting another thorn. *
                return;
            }

            from.Target = new InternalTarget(this);
            from.SendLocalizedMessage(1061906); // Choose a spot to plant the thorn.
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        private class InternalTarget : Target
        {
            private readonly GreenThorns m_Thorn;
            public InternalTarget(GreenThorns thorn)
                : base(3, true, TargetFlags.None)
            {
                m_Thorn = thorn;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Thorn.Deleted)
                    return;

                if (!m_Thorn.IsChildOf(from.Backpack))
                {
                    from.SendLocalizedMessage(1042664); // You must have the object in your backpack to use it.
                    return;
                }

                if (!from.CanBeginAction(typeof(GreenThorns)))
                {
                    from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1061908); // * You must wait a while before planting another thorn. *
                    return;
                }

                if (from.Map != Map.Trammel && from.Map != Map.Felucca)
                {
                    from.LocalOverheadMessage(MessageType.Regular, 0x2B2, true, "No solen lairs exist on this facet.  Try again in Trammel or Felucca.");
                    return;
                }

                LandTarget land = targeted as LandTarget;

                if (land == null)
                {
                    from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1061912); // * You cannot plant a green thorn there! *
                }
                else
                {
                    GreenThornsEffect effect = GreenThornsEffect.Create(from, land);

                    if (effect == null)
                    {
                        from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1061913); // * You sense it would be useless to plant a green thorn there. *
                    }
                    else
                    {
                        m_Thorn.Consume();

                        from.LocalOverheadMessage(MessageType.Emote, 0x961, 1061914); // * You push the strange green thorn into the ground *
                        from.NonlocalOverheadMessage(MessageType.Emote, 0x961, 1061915, from.Name); // * ~1_PLAYER_NAME~ pushes a strange green thorn into the ground. *

                        from.BeginAction(typeof(GreenThorns));
                        new EndActionTimer(from).Start();

                        effect.Start();
                    }
                }
            }

            protected override void OnTargetOutOfRange(Mobile from, object targeted)
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 502825); // That location is too far away
            }
        }

        private class EndActionTimer : Timer
        {
            private readonly Mobile m_From;
            public EndActionTimer(Mobile from)
                : base(TimeSpan.FromMinutes(3.0))
            {
                m_From = from;
                Priority = TimerPriority.FiveSeconds;
            }

            protected override void OnTick()
            {
                m_From.EndAction(typeof(GreenThorns));
            }
        }
    }

    public abstract class GreenThornsEffect : Timer
    {
        private static readonly TilesAndEffect[] m_Table = new TilesAndEffect[]
        {
            new TilesAndEffect(new int[]
            {
                0x71, 0x7C,
                0x82, 0xA7,
                0xDC, 0xE3,
                0xE8, 0xEB,
                0x141, 0x144,
                0x14C, 0x14F,
                0x169, 0x174,
                0x1DC, 0x1E7,
                0x1EC, 0x1EF,
                0x272, 0x275,
                0x27E, 0x281,
                0x2D0, 0x2D7,
                0x2E5, 0x2FF,
                0x303, 0x31F,
                0x32C, 0x32F,
                0x33D, 0x340,
                0x345, 0x34C,
                0x355, 0x358,
                0x367, 0x36E,
                0x377, 0x37A,
                0x38D, 0x390,
                0x395, 0x39C,
                0x3A5, 0x3A8,
                0x3F6, 0x405,
                0x547, 0x54E,
                0x553, 0x556,
                0x597, 0x59E,
                0x623, 0x63A,
                0x6F3, 0x6FA,
                0x777, 0x791,
                0x79A, 0x7A9,
                0x7AE, 0x7B1,
            },
                typeof(DirtGreenThornsEffect)),
            new TilesAndEffect(new int[]
            {
                0x9, 0x15,
                0x150, 0x15C
            },
                typeof(FurrowsGreenThornsEffect)),
            new TilesAndEffect(new int[]
            {
                0x9C4, 0x9EB,
                0x3D65, 0x3D65,
                0x3DC0, 0x3DD9,
                0x3DDB, 0x3DDC,
                0x3DDE, 0x3EF0,
                0x3FF6, 0x3FF6,
                0x3FFC, 0x3FFE,
            },
                typeof(SwampGreenThornsEffect)),
            new TilesAndEffect(new int[]
            {
                0x10C, 0x10F,
                0x114, 0x117,
                0x119, 0x11D,
                0x179, 0x18A,
                0x385, 0x38C,
                0x391, 0x394,
                0x39D, 0x3A4,
                0x3A9, 0x3AC,
                0x5BF, 0x5D6,
                0x5DF, 0x5E2,
                0x745, 0x748,
                0x751, 0x758,
                0x75D, 0x760,
                0x76D, 0x773
            },
                typeof(SnowGreenThornsEffect)),
            new TilesAndEffect(new int[]
            {
                0x16, 0x3A,
                0x44, 0x4B,
                0x11E, 0x121,
                0x126, 0x12D,
                0x192, 0x192,
                0x1A8, 0x1AB,
                0x1B9, 0x1D1,
                0x282, 0x285,
                0x28A, 0x291,
                0x335, 0x33C,
                0x341, 0x344,
                0x34D, 0x354,
                0x359, 0x35C,
                0x3B7, 0x3BE,
                0x3C7, 0x3CA,
                0x5A7, 0x5B2,
                0x64B, 0x652,
                0x657, 0x65A,
                0x663, 0x66A,
                0x66F, 0x672,
                0x7BD, 0x7D0
            },
                typeof(SandGreenThornsEffect))
        };
        private readonly Point3D m_Location;
        private readonly Map m_Map;
        private readonly Mobile m_From;
        private int m_Step;
        public GreenThornsEffect(Point3D location, Map map, Mobile from)
            : base(TimeSpan.FromSeconds(2.5))
        {
            m_Location = location;
            m_Map = map;
            m_From = from;

            Priority = TimerPriority.TwoFiftyMS;
        }

        public Point3D Location => m_Location;
        public Map Map => m_Map;
        public Mobile From => m_From;
        public static GreenThornsEffect Create(Mobile from, LandTarget land)
        {
            if (!from.Map.CanSpawnMobile(land.Location))
                return null;

            int tileID = land.TileID;

            foreach (TilesAndEffect taep in m_Table)
            {
                bool contains = false;

                for (int i = 0; !contains && i < taep.Tiles.Length; i += 2)
                    contains = (tileID >= taep.Tiles[i] && tileID <= taep.Tiles[i + 1]);

                if (contains)
                {
                    GreenThornsEffect effect = (GreenThornsEffect)Activator.CreateInstance(taep.Effect, new object[] { land.Location, from.Map, from });
                    return effect;
                }
            }

            return null;
        }

        protected override void OnTick()
        {
            TimeSpan nextDelay = Play(m_Step++);

            if (nextDelay > TimeSpan.Zero)
            {
                Delay = nextDelay;

                Start();
            }
        }

        protected abstract TimeSpan Play(int step);

        protected bool SpawnItem(Item item)
        {
            for (int i = 0; i < 5; i++) // Try 5 times
            {
                int x = Location.X + Utility.RandomMinMax(-1, 1);
                int y = Location.Y + Utility.RandomMinMax(-1, 1);
                int z = Map.GetAverageZ(x, y);

                if (Map.CanFit(x, y, Location.Z, 1))
                {
                    item.MoveToWorld(new Point3D(x, y, Location.Z), Map);
                    return true;
                }
                else if (Map.CanFit(x, y, z, 1))
                {
                    item.MoveToWorld(new Point3D(x, y, z), Map);
                    return true;
                }
            }

            return false;
        }

        protected bool SpawnCreature(BaseCreature creature)
        {
            for (int i = 0; i < 5; i++) // Try 5 times
            {
                int x = Location.X + Utility.RandomMinMax(-1, 1);
                int y = Location.Y + Utility.RandomMinMax(-1, 1);
                int z = Map.GetAverageZ(x, y);

                if (Map.CanSpawnMobile(x, y, Location.Z))
                {
                    creature.MoveToWorld(new Point3D(x, y, Location.Z), Map);
                    creature.Combatant = From;
                    return true;
                }
                else if (Map.CanSpawnMobile(x, y, z))
                {
                    creature.MoveToWorld(new Point3D(x, y, z), Map);
                    creature.Combatant = From;
                    return true;
                }
            }

            return false;
        }

        private class TilesAndEffect
        {
            private readonly int[] m_Tiles;
            private readonly Type m_Effect;
            public TilesAndEffect(int[] tiles, Type effect)
            {
                m_Tiles = tiles;
                m_Effect = effect;
            }

            public int[] Tiles => m_Tiles;
            public Type Effect => m_Effect;
        }
    }

    public class DirtGreenThornsEffect : GreenThornsEffect
    {
        public DirtGreenThornsEffect(Point3D location, Map map, Mobile from)
            : base(location, map, from)
        {
        }

        protected override TimeSpan Play(int step)
        {
            switch (step)
            {
                case 0:
                    {
                        Effects.PlaySound(Location, Map, 0x106);
                        Effects.SendLocationParticles(EffectItem.Create(Location, Map, EffectItem.DefaultDuration), 0x3735, 1, 182, 0xBE3);

                        return TimeSpan.FromSeconds(4.0);
                    }
                case 1:
                    {
                        Effects.PlaySound(Location, Map, 0x222);

                        return TimeSpan.FromSeconds(4.0);
                    }
                case 2:
                    {
                        Effects.PlaySound(Location, Map, 0x21F);

                        return TimeSpan.FromSeconds(5.0);
                    }
                case 3:
                    {
                        EffectItem dummy = EffectItem.Create(Location, Map, TimeSpan.FromSeconds(20.0));
                        dummy.PublicOverheadMessage(MessageType.Regular, 0x3B2, true, "* The ground erupts with chaotic growth! *");

                        Effects.PlaySound(Location, Map, 0x12D);

                        SpawnReagents();
                        SpawnReagents();

                        return TimeSpan.FromSeconds(2.0);
                    }
                case 4:
                    {
                        Effects.PlaySound(Location, Map, 0x12D);

                        SpawnReagents();
                        SpawnReagents();

                        return TimeSpan.FromSeconds(2.0);
                    }
                case 5:
                    {
                        Effects.PlaySound(Location, Map, 0x12D);

                        SpawnReagents();
                        SpawnReagents();

                        return TimeSpan.FromSeconds(3.0);
                    }
                default:
                    {
                        Effects.PlaySound(Location, Map, 0x12D);

                        SpawnReagents();
                        SpawnReagents();

                        return TimeSpan.Zero;
                    }
            }
        }

        private void SpawnReagents()
        {
            Item reagents;
            int amount = Utility.RandomMinMax(10, 25);

            switch (Utility.Random(9))
            {
                case 0:
                    reagents = new BlackPearl(amount);
                    break;
                case 1:
                    reagents = new Bloodmoss(amount);
                    break;
                case 2:
                    reagents = new Garlic(amount);
                    break;
                case 3:
                    reagents = new Ginseng(amount);
                    break;
                case 4:
                    reagents = new MandrakeRoot(amount);
                    break;
                case 5:
                    reagents = new Nightshade(amount);
                    break;
                case 6:
                    reagents = new SulfurousAsh(amount);
                    break;
                case 7:
                    reagents = new SpidersSilk(amount);
                    break;
                default:
                    reagents = new FertileDirt(amount);
                    break;
            }

            if (!SpawnItem(reagents))
                reagents.Delete();
        }
    }

    public class FurrowsGreenThornsEffect : GreenThornsEffect
    {
        public FurrowsGreenThornsEffect(Point3D location, Map map, Mobile from)
            : base(location, map, from)
        {
        }

        protected override TimeSpan Play(int step)
        {
            switch (step)
            {
                case 0:
                    {
                        Effects.PlaySound(Location, Map, 0x106);
                        Effects.SendLocationParticles(EffectItem.Create(Location, Map, EffectItem.DefaultDuration), 0x3735, 1, 182, 0xBE3);

                        return TimeSpan.FromSeconds(4.0);
                    }
                case 1:
                    {
                        EffectItem hole = EffectItem.Create(Location, Map, TimeSpan.FromSeconds(10.0));
                        hole.ItemID = 0x913;

                        Effects.PlaySound(Location, Map, 0x222);

                        return TimeSpan.FromSeconds(4.0);
                    }
                case 2:
                    {
                        Effects.PlaySound(Location, Map, 0x21F);

                        return TimeSpan.FromSeconds(4.0);
                    }
                default:
                    {
                        EffectItem dummy = EffectItem.Create(Location, Map, TimeSpan.FromSeconds(20.0));
                        dummy.PublicOverheadMessage(MessageType.Regular, 0x3B2, true, "* A magical bunny leaps out of its hole, disturbed by the thorn's effect! *");

                        BaseCreature spawn = new VorpalBunny();
                        if (!SpawnCreature(spawn))
                            spawn.Delete();

                        return TimeSpan.Zero;
                    }
            }
        }
    }

    public class SwampGreenThornsEffect : GreenThornsEffect
    {
        public SwampGreenThornsEffect(Point3D location, Map map, Mobile from)
            : base(location, map, from)
        {
        }

        protected override TimeSpan Play(int step)
        {
            switch (step)
            {
                case 0:
                    {
                        Effects.PlaySound(Location, Map, 0x106);
                        Effects.SendLocationParticles(EffectItem.Create(Location, Map, EffectItem.DefaultDuration), 0x3735, 1, 182, 0xBE3);

                        return TimeSpan.FromSeconds(4.0);
                    }
                case 1:
                    {
                        Effects.PlaySound(Location, Map, 0x222);

                        return TimeSpan.FromSeconds(4.0);
                    }
                case 2:
                    {
                        Effects.PlaySound(Location, Map, 0x21F);

                        return TimeSpan.FromSeconds(1.0);
                    }
                default:
                    {
                        EffectItem dummy = EffectItem.Create(Location, Map, TimeSpan.FromSeconds(20.0));
                        dummy.PublicOverheadMessage(MessageType.Regular, 0x3B2, true, "* Strange green tendrils rise from the ground, whipping wildly! *");
                        Effects.PlaySound(Location, Map, 0x2B0);

                        BaseCreature spawn = new WhippingVine();
                        if (!SpawnCreature(spawn))
                            spawn.Delete();

                        return TimeSpan.Zero;
                    }
            }
        }
    }

    public class SnowGreenThornsEffect : GreenThornsEffect
    {
        public SnowGreenThornsEffect(Point3D location, Map map, Mobile from)
            : base(location, map, from)
        {
        }

        protected override TimeSpan Play(int step)
        {
            switch (step)
            {
                case 0:
                    {
                        Effects.PlaySound(Location, Map, 0x106);
                        Effects.SendLocationParticles(EffectItem.Create(Location, Map, EffectItem.DefaultDuration), 0x3735, 1, 182, 0xBE3);

                        return TimeSpan.FromSeconds(4.0);
                    }
                case 1:
                    {
                        Effects.PlaySound(Location, Map, 0x222);

                        return TimeSpan.FromSeconds(4.0);
                    }
                case 2:
                    {
                        Effects.PlaySound(Location, Map, 0x21F);

                        return TimeSpan.FromSeconds(4.0);
                    }
                default:
                    {
                        EffectItem dummy = EffectItem.Create(Location, Map, TimeSpan.FromSeconds(20.0));
                        dummy.PublicOverheadMessage(MessageType.Regular, 0x3B2, true, "* Slithering ice serpents rise to the surface to investigate the disturbance! *");

                        BaseCreature spawn = new GiantIceWorm();
                        if (!SpawnCreature(spawn))
                            spawn.Delete();

                        for (int i = 0; i < 3; i++)
                        {
                            BaseCreature snake = new IceSnake();
                            if (!SpawnCreature(snake))
                                snake.Delete();
                        }

                        return TimeSpan.Zero;
                    }
            }
        }
    }

    public class SandGreenThornsEffect : GreenThornsEffect
    {
        public SandGreenThornsEffect(Point3D location, Map map, Mobile from)
            : base(location, map, from)
        {
        }

        protected override TimeSpan Play(int step)
        {
            switch (step)
            {
                case 0:
                    {
                        Effects.PlaySound(Location, Map, 0x106);
                        Effects.SendLocationParticles(EffectItem.Create(Location, Map, EffectItem.DefaultDuration), 0x3735, 1, 182, 0xBE3);

                        return TimeSpan.FromSeconds(4.0);
                    }
                case 1:
                    {
                        Effects.PlaySound(Location, Map, 0x222);

                        return TimeSpan.FromSeconds(4.0);
                    }
                case 2:
                    {
                        Effects.PlaySound(Location, Map, 0x21F);

                        return TimeSpan.FromSeconds(5.0);
                    }
                default:
                    {
                        EffectItem dummy = EffectItem.Create(Location, Map, TimeSpan.FromSeconds(20.0));
                        dummy.PublicOverheadMessage(MessageType.Regular, 0x3B2, true, "* The sand collapses, revealing a dark hole. *");

                        GreenThornsSHTeleporter.Create(Location, Map);

                        return TimeSpan.Zero;
                    }
            }
        }
    }

    public class GreenThornsSHTeleporter : Item
    {
        public static readonly Point3D Destination = new Point3D(5738, 1856, 0);
        public GreenThornsSHTeleporter(Serial serial)
            : base(serial)
        {
        }

        private GreenThornsSHTeleporter()
            : base(0x913)
        {
            Movable = false;
            Hue = 0x1;
        }

        public override string DefaultName => "a hole";
        public static void Create(Point3D location, Map map)
        {
            GreenThornsSHTeleporter tele = new GreenThornsSHTeleporter();

            tele.MoveToWorld(location, map);

            new InternalTimer(tele).Start();
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(this, 3))
            {
                BaseCreature.TeleportPets(from, Destination, Map);

                from.Location = Destination;
            }
            else
            {
                from.SendLocalizedMessage(1019045); // I can't reach that.
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            Delete();
        }

        private class InternalTimer : Timer
        {
            private readonly GreenThornsSHTeleporter m_Teleporter;
            public InternalTimer(GreenThornsSHTeleporter teleporter)
                : base(TimeSpan.FromMinutes(1.0))
            {
                m_Teleporter = teleporter;
                Priority = TimerPriority.FiveSeconds;
            }

            protected override void OnTick()
            {
                m_Teleporter.Delete();
            }
        }
    }
}
