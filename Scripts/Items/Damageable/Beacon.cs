using Server.Mobiles;
using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class Beacon : DamageableItem
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public BeaconItem Component { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool DoBlast
        {
            get
            {
                return false;
            }
            set
            {
                if (value && !Deleted)
                    DoEffects(Location, Map);
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool DoArea
        {
            get
            {
                return false;
            }
            set
            {
                if (value)
                    DoAreaAttack();
            }
        }

        public List<BeaconRubble> Rubble { get; set; }
        public override double IDChange => 0.50;

        public Beacon()
            : base(18212, 39299)
        {
            Component = new BeaconItem(this);

            ResistBasePhys = 50;
            ResistBaseFire = 85;
            ResistBaseCold = 99;
            ResistBasePoison = 99;
            ResistBaseEnergy = 70;

            Level = ItemLevel.Easy; // Hard
        }

        public override void OnLocationChange(Point3D oldlocation)
        {
            base.OnLocationChange(oldlocation);

            if (Component != null)
                Component.Location = new Point3D(X - 1, Y, Z);
        }

        public override void OnMapChange()
        {
            base.OnMapChange();

            if (Component != null)
                Component.Map = Map;
        }

        public override bool OnBeforeDestroyed()
        {
            List<BeaconRubble> delete = new List<BeaconRubble>();

            if (Rubble != null)
            {
                foreach (var i in Rubble)
                {
                    if (i.Z > Z)
                    {
                        i.Delete();
                        delete.Add(i);
                    }
                    else
                    {
                        i.SetDelete(TimeSpan.FromMinutes(2));
                    }
                }

                delete.ForEach(i => Rubble.Remove(i));
            }

            DoEffects(Location, Map);

            if (Component != null)
            {
                Component.Delete();
            }

            ColUtility.Free(delete);

            AddRubble(new BeaconRubble(this, 634, TimeSpan.FromMinutes(2)), new Point3D(X - 2, Y, Z));
            AddRubble(new BeaconRubble(this, 633, TimeSpan.FromMinutes(2)), new Point3D(X - 2, Y + 1, Z));

            AddRubble(new BeaconRubble(this, 635, TimeSpan.FromMinutes(2)), new Point3D(X + 2, Y - 2, Z));
            AddRubble(new BeaconRubble(this, 632, TimeSpan.FromMinutes(2)), new Point3D(X + 3, Y - 2, Z));

            AddRubble(new BeaconRubble(this, 634, TimeSpan.FromMinutes(2)), new Point3D(X + 2, Y, X));
            AddRubble(new BeaconRubble(this, 633, TimeSpan.FromMinutes(2)), new Point3D(X + 2, Y + 1, Z));
            return true;
        }

        private void DoEffects(Point3D location, Map map)
        {
            int range = 8;

            //Flamestrikes
            for (int i = 0; i < range; i++)
            {
                Timer.DelayCall(TimeSpan.FromMilliseconds(i * 50), o =>
                    {
                        Misc.Geometry.Circle2D(location, map, o, (pnt, m) =>
                        {
                            Effects.SendLocationEffect(pnt, m, 0x3709, 30, 20, 0, 2);
                        });
                    }, i);
            }

            //Explosions
            Timer.DelayCall(TimeSpan.FromMilliseconds(1000), () =>
            {
                for (int i = 0; i < range + 3; i++)
                {
                    Misc.Geometry.Circle2D(location, map, i, (pnt, m) =>
                    {
                        Effects.SendLocationEffect(pnt, m, 0x36CB, 14, 10, 2498, 2);
                    });
                }
            });

            // Black explosions
            Timer.DelayCall(TimeSpan.FromMilliseconds(1400), () =>
            {
                for (int i = 0; i < range - 3; i++)
                {
                    Timer.DelayCall(TimeSpan.FromMilliseconds(i * 50), o =>
                    {
                        Misc.Geometry.Circle2D(location, map, o, (pnt, m) =>
                        {
                            Effects.SendLocationEffect(pnt, m, Utility.RandomBool() ? 14000 : 14013, 14, 20, 2018, 0);
                        });
                    }, i);
                }
            });
        }

        public override void OnIDChange(int oldID)
        {
            if (ItemID == IDHalfHits && oldID == IDStart)
            {
                AddRubble(new BeaconRubble(this, 6571), new Point3D(X, Y + 1, Z + 42));
                AddRubble(new BeaconRubble(this, 3118), new Point3D(X - 1, Y + 1, Z));
                AddRubble(new BeaconRubble(this, 3118), new Point3D(X + 1, Y - 1, Z));
            }
        }

        public override void OnDamage(int amount, Mobile from, bool willkill)
        {
            base.OnDamage(amount, from, willkill);

            if (ItemID == IDHalfHits && Hits <= (HitsMax * .25))
            {
                AddRubble(new BeaconRubble(this, 14732), new Point3D(X - 1, Y + 1, Z));
                AddRubble(new BeaconRubble(this, 14742), new Point3D(X + 1, Y - 1, Z));
                AddRubble(new BeaconRubble(this, 14742), new Point3D(X, Y, Z + 63));

                AddRubble(new BeaconRubble(this, 6571), new Point3D(X + 1, Y + 1, Z + 42));
                AddRubble(new BeaconRubble(this, 6571), new Point3D(X + 1, Y, Z + 59));

                OnHalfDamage();

                ItemID = 39300;
            }
            else if (CheckAreaDamage(from, amount))
            {
                DoAreaAttack();
            }
        }

        public virtual bool CheckAreaDamage(Mobile from, int amount)
        {
            return 0.02 > Utility.RandomDouble();
        }

        public virtual void OnHalfDamage()
        {
        }

        private void AddRubble(BeaconRubble i, Point3D p)
        {
            i.MoveToWorld(p, Map);

            if (Rubble == null)
                Rubble = new List<BeaconRubble>();

            Rubble.Add(i);
        }

        public virtual void DoAreaAttack()
        {
            if (Map == null)
            {
                return;
            }

            List<Mobile> list = new List<Mobile>();
            IPooledEnumerable eable = Map.GetMobilesInRange(Location, 8);

            foreach (Mobile m in eable)
            {
                if (m.AccessLevel > AccessLevel.Player)
                    continue;

                if (m is PlayerMobile || (m is BaseCreature && ((BaseCreature)m).GetMaster() is PlayerMobile))
                    list.Add(m);
            }

            eable.Free();

            list.ForEach(m =>
            {
                m.BoltEffect(0);
                AOS.Damage(m, null, Utility.RandomMinMax(80, 90), 0, 0, 0, 0, 100);

                if (m.NetState != null)
                    m.PrivateOverheadMessage(Network.MessageType.Regular, 1154, 1154552, m.NetState); // *The beacon blasts a surge of energy at you!"
            });

            ColUtility.Free(list);
        }

        public Beacon(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(Component);

            writer.Write(Rubble == null ? 0 : Rubble.Count);

            if (Rubble != null)
                Rubble.ForEach(i => writer.Write(i));
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Component = reader.ReadItem() as BeaconItem;

            if (Component != null)
                Component.Beacon = this;

            int count = reader.ReadInt();

            for (int i = 0; i < count; i++)
            {
                BeaconRubble item = reader.ReadItem() as BeaconRubble;

                if (item != null)
                {
                    if (Rubble == null)
                        Rubble = new List<BeaconRubble>();

                    Rubble.Add(item);
                }
            }
        }
    }

    public class BeaconItem : Item
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public Beacon Beacon { get; set; }

        public override bool ForceShowProperties => true;

        public BeaconItem(Beacon beacon)
            : base(18223)
        {
            Movable = false;
            Beacon = beacon;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (Beacon != null)
                Beacon.OnDoubleClick(from);
        }

        public BeaconItem(Serial serial)
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

    public class BeaconRubble : Static
    {
        public Beacon Beacon { get; set; }

        public BeaconRubble(Beacon beacon, int itemID)
            : this(beacon, itemID, TimeSpan.Zero)
        {
        }

        public BeaconRubble(Beacon beacon, int itemID, TimeSpan duration)
            : base(itemID)
        {
            Beacon = beacon;

            SetDelete(duration);
        }

        public void SetDelete(TimeSpan ts)
        {
            if (ts != TimeSpan.Zero)
            {
                Timer.DelayCall(ts, Delete);
            }
        }

        public BeaconRubble(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.WriteItem(Beacon);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Beacon = reader.ReadItem<Beacon>();

            if (Beacon == null)
            {
                Delete();
            }
        }
    }
}
