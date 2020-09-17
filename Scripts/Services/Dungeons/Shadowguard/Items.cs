using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using System;
using System.Linq;

namespace Server.Engines.Shadowguard
{
    public class ShadowguardBottleOfLiquor : BaseDecayingItem
    {
        public override int Lifespan => 60;
        public override int LabelNumber => 1042961;  // a bottle of liquor

        public BarEncounter Encounter { get; set; }

        [Constructable]
        public ShadowguardBottleOfLiquor(BarEncounter encounter) : base(0x99B)
        {
            Encounter = encounter;
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (m.InRange(GetWorldLocation(), 2))
            {
                if (0.1 > Utility.RandomDouble())
                {
                    m.BAC = Math.Min(60, m.BAC + 10);
                    m.PlaySound(Utility.RandomList(0x30, 0x2D6));
                    BaseBeverage.CheckHeaveTimer(m);

                    m.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1156270, m.NetState); // *You ready the bottle to throw but it's enchanting label persuades you to drink it instead!*

                    Delete();
                }
                else
                {
                    m.SendLocalizedMessage(1010086); // What do you want to use this on?
                    m.BeginTarget(10, false, Targeting.TargetFlags.None, (from, targeted) =>
                    {
                        if (0.25 > Utility.RandomDouble() && m.BAC > 0)
                        {
                            AOS.Damage(m, Utility.RandomMinMax(25, 50), 100, 0, 0, 0, 0);
                            m.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1156271, m.NetState); // *You wind up to throw but in your inebriated state you manage to hit yourself!*
                            m.FixedParticles(0x3728, 20, 10, 5044, EffectLayer.Head);

                            Delete();
                        }
                        else if (targeted is ShadowguardPirate)
                        {
                            ShadowguardPirate pirate = targeted as ShadowguardPirate;

                            m.DoHarmful(pirate);
                            m.MovingParticles(pirate, 0x99B, 10, 0, false, true, 0, 0, 9502, 6014, 0x11D, EffectLayer.Waist, 0);

                            Timer.DelayCall(TimeSpan.FromSeconds(.5), () =>
                            {
                                if (pirate.Alive && !pirate.BlockReflect)
                                {
                                    // this is gay, but can't figure out a better way to do!
                                    pirate.BlockReflect = true;
                                    AOS.Damage(pirate, m, 300, 0, 0, 0, 0, 0, 0, 100);
                                    pirate.BlockReflect = false;
                                    pirate.FixedParticles(0x3728, 20, 10, 5044, EffectLayer.Head);

                                    pirate.PlaySound(Utility.Random(0x3E, 3));
                                }
                            });

                            Delete();
                        }
                        else
                            m.SendLocalizedMessage(1156211); // You cannot throw this there!
                    });
                }
            }
        }

        public override void OnAfterDelete()
        {
            if (Encounter != null)
                Encounter.CheckEncounter();
        }

        public ShadowguardBottleOfLiquor(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public enum VirtueType
    {
        Honesty,
        Compassion,
        Valor,
        Justice,
        Sacrafice,
        Honor,
        Spirituality,
        Humility,
        Deceit,
        Despise,
        Destard,
        Wrong,
        Covetous,
        Shame,
        Hythloth,
        Pride
    }

    public class ShadowguardApple : Apple
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public ShadowguardCypress Tree { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public OrchardEncounter Encounter { get; set; }

        private bool _Thrown;

        public ShadowguardApple(OrchardEncounter encounter, ShadowguardCypress tree)
        {
            Encounter = encounter;
            Tree = tree;

            AttachSocket(new DecayingItemSocket(30, true));
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            if (Tree != null)
                list.Add(1156210, Tree.VirtueType.ToString()); // An Enchanted Apple of ~1_TYPE~
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (IsChildOf(m.Backpack) && Tree != null)
            {
                m.SendLocalizedMessage(1010086); // What do you want to use this on?
                m.BeginTarget(10, false, Targeting.TargetFlags.None, (from, targeted) =>
                {
                    _Thrown = true;

                    if (targeted is ShadowguardCypress || targeted is ShadowguardCypress.ShadowguardCypressFoilage)
                    {
                        ShadowguardCypress tree = null;

                        if (targeted is ShadowguardCypress)
                            tree = targeted as ShadowguardCypress;
                        else if (targeted is ShadowguardCypress.ShadowguardCypressFoilage)
                            tree = ((ShadowguardCypress.ShadowguardCypressFoilage)targeted).Tree;

                        if (tree != null)
                        {
                            Point3D p = tree.Location;
                            Map map = tree.Map;

                            from.Animate(31, 7, 1, true, false, 0);
                            m.MovingParticles(tree, ItemID, 10, 0, false, true, 0, 0, 9502, 6014, 0x11D, EffectLayer.Waist, 0);

                            Timer.DelayCall(TimeSpan.FromSeconds(.7), () =>
                                {
                                    if (tree.IsOppositeVirtue(Tree.VirtueType))
                                    {
                                        tree.Delete();

                                        Effects.SendLocationParticles(EffectItem.Create(p, map, EffectItem.DefaultDuration), 0x3709, 10, 30, 5052);
                                        Effects.PlaySound(p, map, 0x243);

                                        m.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1156213, m.NetState); // *Your throw releases powerful magics and destroys the tree!*

                                        if (Tree != null)
                                        {
                                            p = Tree.Location;
                                            Tree.Delete();

                                            Effects.SendLocationParticles(EffectItem.Create(p, map, EffectItem.DefaultDuration), 0x3709, 10, 30, 5052);
                                            Effects.PlaySound(p, map, 0x243); //TODO: Get sound
                                        }

                                        tree.Encounter.CheckEncounter();
                                        Delete();
                                    }
                                    else if (Encounter != null)
                                    {
                                        foreach (PlayerMobile pm in Encounter.Region.GetEnumeratedMobiles().OfType<PlayerMobile>())
                                        {
                                            if (!pm.Alive)
                                                continue;

                                            p = pm.Location;
                                            VileTreefellow creature = new VileTreefellow();

                                            for (int i = 0; i < 10; i++)
                                            {
                                                int x = Utility.RandomMinMax(p.X - 1, p.X + 1);
                                                int y = Utility.RandomMinMax(p.Y - 1, p.Y + 1);
                                                int z = p.Z;

                                                if (map.CanSpawnMobile(x, y, z))
                                                {
                                                    p = new Point3D(x, y, z);
                                                    break;
                                                }
                                            }

                                            creature.MoveToWorld(p, map);
                                            Timer.DelayCall(() => creature.Combatant = pm);

                                            Encounter.AddSpawn(creature);
                                        }

                                        m.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1156212, m.NetState); // *Your throw seems to have summoned an ambush!*
                                        Delete();
                                    }
                                });
                        }
                    }
                });
            }
        }

        public override void OnDelete()
        {
            base.OnDelete();

            if (!_Thrown && Encounter != null)
            {
                foreach (PlayerMobile pm in Encounter.Region.GetEnumeratedMobiles().OfType<PlayerMobile>())
                {
                    if (!pm.Alive)
                        continue;

                    Point3D p = pm.Location;
                    Map map = pm.Map;
                    VileTreefellow creature = new VileTreefellow();

                    for (int i = 0; i < 10; i++)
                    {
                        int x = Utility.RandomMinMax(p.X - 1, p.X + 1);
                        int y = Utility.RandomMinMax(p.Y - 1, p.Y + 1);
                        int z = p.Z;

                        if (map.CanSpawnMobile(x, y, z))
                        {
                            p = new Point3D(x, y, z);
                            break;
                        }
                    }

                    creature.MoveToWorld(p, map);
                    Timer.DelayCall(() => creature.Combatant = pm);

                    Encounter.AddSpawn(creature);
                }
            }
        }

        public override void Delete()
        {
            base.Delete();

            if (Encounter != null)
            {
                Encounter.OnAppleDeleted();
            }
        }

        public ShadowguardApple(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version == 0)
            {
                reader.ReadItem();
            }
        }
    }

    public class ShadowguardCypress : Item
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public OrchardEncounter Encounter { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public VirtueType VirtueType { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public ShadowguardCypressFoilage Foilage { get; set; }

        // 0xD96, 0xD9A, 

        public ShadowguardCypress(OrchardEncounter encounter, VirtueType type)
            : base(3329)
        {
            VirtueType = type;
            Encounter = encounter;

            Foilage = new ShadowguardCypressFoilage(Utility.RandomBool() ? 0xD96 : 0xD9A, this);

            Movable = false;
        }

        public override void OnLocationChange(Point3D oldLocation)
        {
            base.OnLocationChange(oldLocation);

            if (Foilage != null)
                Foilage.Location = new Point3D(X, Y, Z + 6);
        }

        public override void OnMapChange()
        {
            if (Foilage != null)
                Foilage.Map = Map;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.Backpack != null && from.InRange(Location, 3))
            {
                if (Encounter.Apple == null || Encounter.Apple.Deleted)
                {
                    Encounter.Apple = new ShadowguardApple(Encounter, this);
                    from.Backpack.DropItem(Encounter.Apple);

                    Encounter.OnApplePicked();
                }
            }
        }

        public bool IsOppositeVirtue(VirtueType type)
        {
            switch (type)
            {
                default:
                case VirtueType.Honesty: return VirtueType == VirtueType.Deceit;
                case VirtueType.Compassion: return VirtueType == VirtueType.Despise;
                case VirtueType.Valor: return VirtueType == VirtueType.Destard;
                case VirtueType.Justice: return VirtueType == VirtueType.Wrong;
                case VirtueType.Sacrafice: return VirtueType == VirtueType.Covetous;
                case VirtueType.Honor: return VirtueType == VirtueType.Shame;
                case VirtueType.Spirituality: return VirtueType == VirtueType.Hythloth;
                case VirtueType.Humility: return VirtueType == VirtueType.Pride;
                case VirtueType.Deceit: return VirtueType == VirtueType.Honesty;
                case VirtueType.Despise: return VirtueType == VirtueType.Compassion;
                case VirtueType.Destard: return VirtueType == VirtueType.Valor;
                case VirtueType.Wrong: return VirtueType == VirtueType.Justice;
                case VirtueType.Covetous: return VirtueType == VirtueType.Sacrafice;
                case VirtueType.Shame: return VirtueType == VirtueType.Honor;
                case VirtueType.Hythloth: return VirtueType == VirtueType.Spirituality;
                case VirtueType.Pride: return VirtueType == VirtueType.Humility;
            }
        }

        public override void Delete()
        {
            base.Delete();

            if (Foilage != null)
                Foilage.Delete();

            if (Encounter != null)
                Encounter.CheckEncounter();
        }

        public class ShadowguardCypressFoilage : Item
        {
            public ShadowguardCypress Tree { get; set; }

            public ShadowguardCypressFoilage(int id, ShadowguardCypress cypress)
                : base(id)
            {
                Movable = false;
                Tree = cypress;
            }

            public override void OnDoubleClick(Mobile m)
            {
                if (Tree != null)
                    Tree.OnDoubleClick(m);
            }

            public ShadowguardCypressFoilage(Serial serial)
                : base(serial)
            {
            }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);
                writer.Write(0);
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);
                int version = reader.ReadInt();
            }
        }


        public ShadowguardCypress(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(Foilage);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Foilage = reader.ReadItem() as ShadowguardCypressFoilage;

            if (Foilage != null)
            {
                Foilage.Tree = this;
            }
        }
    }

    public class Phylactery : BaseDecayingItem
    {
        public override int Lifespan => 60;
        public override int LabelNumber => _Purified ? 1156221 : 1156220;  // Purified Phylactery : Corrupt Phylactery

        private bool _Purified;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Purified
        {
            get { return _Purified; }
            set
            {
                _Purified = value;

                if (value)
                {
                    Hue = 0;
                }

                InvalidateProperties();
            }
        }

        [Constructable]
        public Phylactery()
            : base(0x4686)
        {
            Hue = 2075;
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (IsChildOf(m.Backpack))
            {
                m.SendLocalizedMessage(1010086); // What do you want to use this on?
                m.BeginTarget(3, false, Targeting.TargetFlags.None, (from, targeted) =>
                {
                    if (targeted is PurifyingFlames)
                    {
                        PurifyingFlames flames = targeted as PurifyingFlames;

                        if (!from.InLOS(flames))
                            from.SendLocalizedMessage(500237); // Target cannot be seen.
                        else if (!Purified)
                        {
                            m.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1156225, m.NetState); // *You purify the phylactery!*

                            Effects.SendLocationParticles(EffectItem.Create(flames.Location, flames.Map, EffectItem.DefaultDuration), 0x3709, 10, 30, 5052);
                            Effects.PlaySound(flames.Location, flames.Map, 0x225); //TODO: Get sound

                            Purified = true;
                            InvalidateProperties();
                        }
                    }
                    else if (targeted is CursedSuitOfArmor)
                    {
                        CursedSuitOfArmor armor = targeted as CursedSuitOfArmor;

                        if (!from.InLOS(armor))
                            from.SendLocalizedMessage(500237); // Target cannot be seen.
                        else if (!_Purified)
                            m.SendLocalizedMessage(1156224); // *The cursed armor rejects the phylactery!*
                        else
                        {
                            m.SendLocalizedMessage(1156222); // *You throw the phylactery at the armor causing it to disintegrate!*

                            Map map = armor.Map;
                            Point3D p;

                            if (armor.ItemID == 5402)
                                p = new Point3D(armor.X - 1, armor.Y, armor.Z);
                            else
                                p = new Point3D(armor.X, armor.Y - 1, armor.Z);

                            armor.Delete();
                            Delete();

                            Effects.SendLocationParticles(EffectItem.Create(p, map, EffectItem.DefaultDuration), 0x3709, 10, 30, 2720, 7, 5052, 0);
                            Effects.PlaySound(p, map, 0x225); //TODO: Get sound

                            Timer.DelayCall(TimeSpan.FromSeconds(1), () =>
                            {
                                Item item = new Static(Utility.Random(8762, 16))
                                {
                                    Hue = 1111,
                                    Name = "Broken Armor"
                                };
                                item.MoveToWorld(p, Map.TerMur);

                                ArmoryEncounter encounter = ShadowguardController.GetEncounter(p, Map.TerMur) as ArmoryEncounter;

                                if (encounter != null)
                                    encounter.AddDestroyedArmor(item);

                                int ticks = 1;
                                Timer.DelayCall(TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(50), 2, () =>
                                {
                                    Misc.Geometry.Circle2D(p, map, ticks, (pnt, mob) =>
                                    {
                                        Effects.PlaySound(pnt, mob, 0x307);
                                        Effects.SendLocationEffect(pnt, mob, Utility.RandomBool() ? 14000 : 14013, 20, 2018, 0);

                                    });

                                    ticks++;
                                });
                            });
                        }
                    }
                });
            }
        }

        public Phylactery(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(_Purified);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            _Purified = reader.ReadBool();
        }
    }

    public class CursedSuitOfArmor : Item
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public ShadowguardEncounter Encounter { get; set; }

        public override int LabelNumber => 1156218;  // Cursed Suit of Armor

        public CursedSuitOfArmor(ShadowguardEncounter encounter) : base(0x151A)
        {
            Encounter = encounter;
            Movable = false;
        }

        public override void Delete()
        {
            base.Delete();

            if (Encounter != null)
                Encounter.CheckEncounter();
        }

        public CursedSuitOfArmor(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class PurifyingFlames : Item
    {
        public override int LabelNumber => 1156217;  // Purifying Flames

        [Constructable]
        public PurifyingFlames() : base(0x19AB)
        {
            Movable = false;
        }

        public PurifyingFlames(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public enum Flow
    {
        EastWest,
        NorthSouth,
        NorthWestCorner,
        NorthEastCorner,
        SouthWestCorner,
        SouthEastCorner
    }

    public class ShadowguardCanal : BaseDecayingItem, IChopable
    {
        public override int Lifespan => 1800;
        public override int LabelNumber => 1156228;  // Canal

        private Flow _Flow;

        [CommandProperty(AccessLevel.GameMaster)]
        public Flow Flow { get { return _Flow; } set { _Flow = value; InvalideIDfromFlow(); } }

        [Constructable]
        public ShadowguardCanal() : base(Utility.RandomList(39911, 39915, 39919, 39924, 39928, 39932))
        {
            InvalidateID();
            Hue = 2500;
        }

        [Constructable]
        public ShadowguardCanal(Flow flow) : base(0)
        {
            Flow = flow;
            Hue = 2500;
        }

        public void OnChop(Mobile from)
        {
            if (Movable)
            {
                Effects.PlaySound(Location, Map, 0x3B3);
                from.SendLocalizedMessage(500461); // You destroy the item.
                Delete();
            }
        }

        public void Fill()
        {
            ItemID--;
            Hue = 0;
        }

        private void InvalideIDfromFlow()
        {
            switch (_Flow)
            {
                case Flow.NorthSouth: ItemID = 39911; break;
                case Flow.SouthEastCorner: ItemID = 39915; break;
                case Flow.SouthWestCorner: ItemID = 39919; break;
                case Flow.EastWest: ItemID = 39924; break;
                case Flow.NorthEastCorner: ItemID = 39928; break;
                case Flow.NorthWestCorner: ItemID = 39932; break;
            }
        }

        private void InvalidateID()
        {
            switch (ItemID)
            {
                default: ItemID = 39911; InvalidateID(); break;
                case 39911: _Flow = Flow.NorthSouth; break;
                case 39915: _Flow = Flow.SouthEastCorner; break;
                case 39919: _Flow = Flow.SouthWestCorner; break;
                case 39924: _Flow = Flow.EastWest; break;
                case 39928: _Flow = Flow.NorthEastCorner; break;
                case 39932: _Flow = Flow.NorthWestCorner; break;
            }
        }

        public bool Connects(ShadowguardCanal next)
        {
            Direction d = Utility.GetDirection(this, next);
            Flow f = next.Flow;

            switch (d)
            {
                case Direction.North:
                    switch (_Flow)
                    {
                        case Flow.EastWest: return false; //
                        case Flow.NorthSouth: return f == Flow.NorthSouth || f == Flow.NorthEastCorner || f == Flow.NorthWestCorner; //
                        case Flow.NorthWestCorner: return false; //
                        case Flow.NorthEastCorner: return false; //
                        case Flow.SouthWestCorner: return f == Flow.NorthSouth || f == Flow.NorthEastCorner || f == Flow.NorthWestCorner; //
                        case Flow.SouthEastCorner: return f == Flow.NorthSouth || f == Flow.NorthEastCorner || f == Flow.NorthWestCorner; //
                    }
                    break;
                case Direction.South:
                    switch (_Flow)
                    {
                        case Flow.EastWest: return false; // 
                        case Flow.NorthSouth: return f == Flow.NorthSouth || f == Flow.SouthEastCorner || f == Flow.SouthWestCorner; //
                        case Flow.NorthWestCorner: return f == Flow.NorthSouth || f == Flow.SouthEastCorner || f == Flow.SouthWestCorner; //
                        case Flow.NorthEastCorner: return f == Flow.NorthSouth || f == Flow.SouthEastCorner || f == Flow.SouthWestCorner; //
                        case Flow.SouthWestCorner: return false; //
                        case Flow.SouthEastCorner: return false; //
                    }
                    break;
                case Direction.East:
                    switch (_Flow)
                    {
                        case Flow.EastWest: return f == Flow.EastWest || f == Flow.NorthEastCorner || f == Flow.SouthEastCorner; //
                        case Flow.NorthSouth: return false; //
                        case Flow.NorthWestCorner: return f == Flow.EastWest || f == Flow.NorthEastCorner || f == Flow.SouthEastCorner; // 
                        case Flow.NorthEastCorner: return false; //
                        case Flow.SouthWestCorner: return f == Flow.EastWest || f == Flow.NorthEastCorner || f == Flow.SouthEastCorner; //
                        case Flow.SouthEastCorner: return false; //
                    }
                    break;
                case Direction.West:
                    switch (_Flow)
                    {
                        case Flow.EastWest: return f == Flow.EastWest || f == Flow.NorthWestCorner || f == Flow.SouthWestCorner; //
                        case Flow.NorthSouth: return false; //
                        case Flow.NorthWestCorner: return false; //
                        case Flow.NorthEastCorner: return f == Flow.EastWest || f == Flow.NorthWestCorner || f == Flow.SouthWestCorner; //
                        case Flow.SouthWestCorner: return false; // 
                        case Flow.SouthEastCorner: return f == Flow.EastWest || f == Flow.NorthWestCorner || f == Flow.SouthWestCorner; //
                    }
                    break;
            }

            return false;
        }

        public ShadowguardCanal(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write((int)_Flow);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            _Flow = (Flow)reader.ReadInt();
        }
    }

    public class ShadowguardSpigot : Item
    {
        public override int LabelNumber => 1156275;  // A Spigot

        public ShadowguardSpigot(int id) : base(id)
        {
            Movable = false;
        }

        public override void OnDoubleClick(Mobile m)
        {
            FountainEncounter encounter = ShadowguardController.GetEncounter(Location, Map) as FountainEncounter;

            if (m.InRange(Location, 2) && encounter != null && ItemID != 17294 && ItemID != 17278)
            {
                encounter.UseSpigot(this, m);
            }
        }

        public ShadowguardSpigot(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class ShadowguardDrain : Item
    {
        public override int LabelNumber => 1156272;  // A Drain

        public ShadowguardDrain() : base(0x9BFF)
        {
            Movable = false;
            Hue = 2500;
        }

        public ShadowguardDrain(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class MagicDrakeWing : BaseDecayingItem
    {
        public override int Lifespan => 90;
        public override int LabelNumber => 1156233;  // Magic Drake Wing

        [Constructable]
        public MagicDrakeWing() : base(0x1E85)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            BelfryEncounter encounter = ShadowguardController.GetEncounter(from.Location, from.Map) as BelfryEncounter;

            if (encounter != null && IsChildOf(from.Backpack))
            {
                Point3D p = encounter.SpawnPoints[1];
                encounter.ConvertOffset(ref p);
                BaseCreature.TeleportPets(from, p, from.Map);
                from.MoveToWorld(p, Map.TerMur);
            }
        }

        public MagicDrakeWing(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class FeedingBell : BaseAddon
    {
        public override int LabelNumber => 1156232;   // Feeding Bell

        [Constructable]
        public FeedingBell()
        {
            AddComponent(new AddonComponent(38955), 0, 0, 0);
            AddComponent(new AddonComponent(38951), 1, 0, 0);
            AddComponent(new LocalizedAddonComponent(19548, 1156232), 0, 0, 0);

            AddComponent(new AddonComponent(3892), 0, 0, 0);
            AddComponent(new AddonComponent(3892), 1, 0, 0);
            AddComponent(new AddonComponent(3893), 0, 1, 0);
            AddComponent(new AddonComponent(3893), 0, 1, 0);
        }

        public override void OnComponentUsed(AddonComponent c, Mobile from)
        {
            if (from.InRange(c.Location, 2) && c.ItemID == 19548)
            {
                BelfryEncounter encounter = ShadowguardController.GetEncounter(c.Location, c.Map) as BelfryEncounter;

                if (encounter != null && encounter.Drakes != null && encounter.Drakes.Count == 0)
                {
                    int toSpawn = 2 + (encounter.PartySize() * 3);

                    for (int i = 0; i < toSpawn; i++)
                    {
                        encounter.SpawnDrake(Location, from);
                        Effects.PlaySound(Location, Map, 0x66C);
                    }
                }
            }
        }

        public FeedingBell(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class WitheringBones : Container
    {
        public override int LabelNumber => 1156214;  // The Withered Bones of an Adventurer
        public override bool IsDecoContainer => false;

        [Constructable]
        public WitheringBones()
            : base(0xECF)
        {
            Movable = false;

            DropItem(new TatteredBook());
        }

        public WitheringBones(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class TatteredBook : Item
    {
        public override int LabelNumber => 1156215;  // a tattered book

        public TatteredBook()
            : base(7712)
        {
            Movable = false;
        }

        public override void OnDoubleClick(Mobile m)
        {
            m.SendGump(new InternalGump());
        }

        private class InternalGump : Gump
        {
            public InternalGump()
                : base(100, 150)
            {
                AddBackground(0, 0, 400, 300, 9380);

                AddHtmlLocalized(40, 45, 330, 200, 1156216, 1, false, true);
                /*I've finally found...this vile orchard is the key to Minax's enchantments!...
                 * days I've spent trapped within this tower, I dare not pick the fruit from 
                 * the foliage for I know not what consequences may...Hunger is building...so 
                 * hungry I must...Blech! Vile fruit!...What's this? For when I tossed this 
                 * vile apple from whence it came a horrific beast appeared!...for I hope I 
                 * can fight it...<br>*/
            }
        }

        public TatteredBook(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
