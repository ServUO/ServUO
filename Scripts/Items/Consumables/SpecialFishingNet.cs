using System;
using Server.Mobiles;
using Server.Multis;
using Server.Spells;
using Server.Targeting;

namespace Server.Items
{
    public class SpecialFishingNet : Item
    {
        private static readonly int[] m_Hues = new int[]
        {
            0x09B,
            0x0CD,
            0x0D3,
            0x14D,
            0x1DD,
            0x1E9,
            0x1F4,
            0x373,
            0x451,
            0x47F,
            0x489,
            0x492,
            0x4B5,
            0x8AA
        };
        private static readonly int[] m_WaterTiles = new int[]
        {
            0x00A8, 0x00AB,
            0x0136, 0x0137
        };
        private static readonly int[] m_UndeepWaterTiles = new int[]
        {
            0x1797, 0x179C
        };
        private bool m_InUse;
        [Constructable]
        public SpecialFishingNet()
            : base(0x0DCA)
        {
            this.Weight = 15.0;

            if (0.01 > Utility.RandomDouble())
                this.Hue = Utility.RandomList(m_Hues);
            else
                this.Hue = 0x8A0;
        }

        public SpecialFishingNet(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1041079;
            }
        }// a special fishing net
        [CommandProperty(AccessLevel.GameMaster)]
        public bool InUse
        {
            get
            {
                return this.m_InUse;
            }
            set
            {
                this.m_InUse = value;
            }
        }
        public virtual bool RequireDeepWater
        {
            get
            {
                return true;
            }
        }
        public static bool FullValidation(Map map, int x, int y)
        {
            bool valid = ValidateDeepWater(map, x, y);

            for (int j = 1, offset = 5; valid && j <= 5; ++j, offset += 5)
            {
                if (!ValidateDeepWater(map, x + offset, y + offset))
                    valid = false;
                else if (!ValidateDeepWater(map, x + offset, y - offset))
                    valid = false;
                else if (!ValidateDeepWater(map, x - offset, y + offset))
                    valid = false;
                else if (!ValidateDeepWater(map, x - offset, y - offset))
                    valid = false;
            }

            return valid;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            this.AddNetProperties(list);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            writer.Write(this.m_InUse);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 1:
                    {
                        this.m_InUse = reader.ReadBool();

                        if (this.m_InUse)
                            this.Delete();

                        break;
                    }
            }

            this.Stackable = false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (this.m_InUse)
            {
                from.SendLocalizedMessage(1010483); // Someone is already using that net!
            }
            else if (this.IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1010484); // Where do you wish to use the net?
                from.BeginTarget(-1, true, TargetFlags.None, new TargetCallback(OnTarget));
            }
            else
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
        }

        public void OnTarget(Mobile from, object obj)
        {
            if (this.Deleted || this.m_InUse)
                return;

            IPoint3D p3D = obj as IPoint3D;

            if (p3D == null)
                return;

            Map map = from.Map;

            if (map == null || map == Map.Internal)
                return;

            int x = p3D.X, y = p3D.Y, z = map.GetAverageZ(x, y); // OSI just takes the targeted Z

            if (!from.InRange(p3D, 6))
            {
                from.SendLocalizedMessage(500976); // You need to be closer to the water to fish!
            }
            else if (!from.InLOS(obj))
            {
                from.SendLocalizedMessage(500979); // You cannot see that location.
            }
            else if (this.RequireDeepWater ? FullValidation(map, x, y) : (ValidateDeepWater(map, x, y) || ValidateUndeepWater(map, obj, ref z)))
            {
                Point3D p = new Point3D(x, y, z);

                if (this.GetType() == typeof(SpecialFishingNet))
                {
                    for (int i = 1; i < this.Amount; ++i) // these were stackable before, doh
                        from.AddToBackpack(new SpecialFishingNet());
                }

                this.m_InUse = true;
                this.Movable = false;
                this.MoveToWorld(p, map);

                SpellHelper.Turn(from, p);
                from.Animate(12, 5, 1, true, false, 0);

                Effects.SendLocationEffect(p, map, 0x352D, 16, 4);
                Effects.PlaySound(p, map, 0x364);

                Timer.DelayCall(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.25), 14, new TimerStateCallback(DoEffect), new object[] { p, 0, from });

                from.SendLocalizedMessage(this.RequireDeepWater ? 1010487 : 1074492); // You plunge the net into the sea... / You plunge the net into the water...
            }
            else
            {
                from.SendLocalizedMessage(this.RequireDeepWater ? 1010485 : 1074491); // You can only use this net in deep water! / You can only use this net in water!
            }
        }

        protected virtual void AddNetProperties(ObjectPropertyList list)
        {
            // as if the name wasn't enough..
            list.Add(1017410); // Special Fishing Net
        }

        protected virtual int GetSpawnCount()
        {
            int count = Utility.RandomMinMax(3, 6);

            if (this.Hue != 0x8A0)
                count += Utility.RandomMinMax(1, 2);

            return count;
        }

        protected void Spawn(Point3D p, Map map, BaseCreature spawn)
        {
            if (map == null)
            {
                spawn.Delete();
                return;
            }

            int x = p.X, y = p.Y;

            for (int j = 0; j < 20; ++j)
            {
                int tx = p.X - 2 + Utility.Random(5);
                int ty = p.Y - 2 + Utility.Random(5);

                LandTile t = map.Tiles.GetLandTile(tx, ty);

                if (t.Z == p.Z && ((t.ID >= 0xA8 && t.ID <= 0xAB) || (t.ID >= 0x136 && t.ID <= 0x137)) && !Spells.SpellHelper.CheckMulti(new Point3D(tx, ty, p.Z), map))
                {
                    x = tx;
                    y = ty;
                    break;
                }
            }

            spawn.MoveToWorld(new Point3D(x, y, p.Z), map);

            if (spawn is Kraken && 0.35 < Utility.RandomDouble())
                spawn.PackItem(new MessageInABottle(map == Map.Felucca ? Map.Felucca : Map.Trammel));
            
        }

        protected virtual void FinishEffect(Point3D p, Map map, Mobile from)
        {
            from.RevealingAction();

            int count = this.GetSpawnCount();

            for (int i = 0; map != null && i < count; ++i)
            {
                BaseCreature spawn;

                switch ( Utility.Random(4) )
                {
                    default:
                    case 0:
                        spawn = new SeaSerpent();
                        break;
                    case 1:
                        spawn = new DeepSeaSerpent();
                        break;
                    case 2:
                        spawn = new WaterElemental();
                        break;
                    case 3:
                        spawn = new Kraken();
                        break;
                }

                this.Spawn(p, map, spawn);

                spawn.Combatant = from;
            }

            this.Delete();
        }

        private static bool ValidateDeepWater(Map map, int x, int y)
        {
            int tileID = map.Tiles.GetLandTile(x, y).ID;
            bool water = false;

            for (int i = 0; !water && i < m_WaterTiles.Length; i += 2)
                water = (tileID >= m_WaterTiles[i] && tileID <= m_WaterTiles[i + 1]);

            return water;
        }

        private static bool ValidateUndeepWater(Map map, object obj, ref int z)
        {
            if (!(obj is StaticTarget))
                return false;

            StaticTarget target = (StaticTarget)obj;

            if (BaseHouse.FindHouseAt(target.Location, map, 0) != null)
                return false;

            int itemID = target.ItemID;

            for (int i = 0; i < m_UndeepWaterTiles.Length; i += 2)
            {
                if (itemID >= m_UndeepWaterTiles[i] && itemID <= m_UndeepWaterTiles[i + 1])
                {
                    z = target.Z;
                    return true;
                }
            }

            return false;
        }

        private void DoEffect(object state)
        {
            if (this.Deleted)
                return;

            object[] states = (object[])state;

            Point3D p = (Point3D)states[0];
            int index = (int)states[1];
            Mobile from = (Mobile)states[2];

            states[1] = ++index;

            if (index == 1)
            {
                Effects.SendLocationEffect(p, this.Map, 0x352D, 16, 4);
                Effects.PlaySound(p, this.Map, 0x364);
            }
            else if (index <= 7 || index == 14)
            {
                if (this.RequireDeepWater)
                {
                    for (int i = 0; i < 3; ++i)
                    {
                        int x, y;

                        switch ( Utility.Random(8) )
                        {
                            default:
                            case 0:
                                x = -1;
                                y = -1;
                                break;
                            case 1:
                                x = -1;
                                y = 0;
                                break;
                            case 2:
                                x = -1;
                                y = +1;
                                break;
                            case 3:
                                x = 0;
                                y = -1;
                                break;
                            case 4:
                                x = 0;
                                y = +1;
                                break;
                            case 5:
                                x = +1;
                                y = -1;
                                break;
                            case 6:
                                x = +1;
                                y = 0;
                                break;
                            case 7:
                                x = +1;
                                y = +1;
                                break;
                        }

                        Effects.SendLocationEffect(new Point3D(p.X + x, p.Y + y, p.Z), this.Map, 0x352D, 16, 4);
                    }
                }
                else
                {
                    Effects.SendLocationEffect(p, this.Map, 0x352D, 16, 4);
                }

                if (Utility.RandomBool())
                    Effects.PlaySound(p, this.Map, 0x364);

                if (index == 14)
                    this.FinishEffect(p, this.Map, from);
                else
                    this.Z -= 1;
            }
        }
    }

    public class FabledFishingNet : SpecialFishingNet
    {
        [Constructable]
        public FabledFishingNet()
        {
            this.Hue = 0x481;
        }

        public FabledFishingNet(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1063451;
            }
        }// a fabled fishing net
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (this.Weight != 15.0)
                this.Weight = 15.0;
        }

        protected override void AddNetProperties(ObjectPropertyList list)
        {
        }

        protected override int GetSpawnCount()
        {
            return base.GetSpawnCount() + 4;
        }

        protected override void FinishEffect(Point3D p, Map map, Mobile from)
        {
            this.Spawn(p, map, new Leviathan(from));

            base.FinishEffect(p, map, from);
        }
    }
}