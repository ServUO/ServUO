using System;
using Server;
using System.Collections.Generic;
using System.Linq;
using Server.Mobiles;
using Server.Items;
using Server.Engines.CityLoyalty;

namespace Server.Engines.Blackthorn
{
    public class InvasionBeacon : DamageableItem
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public InvasionController Controller { get; set; }

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
                if (value)
                    DoEffects();
            }
        }

        public List<Item> Rubble { get; set; }

        public override bool DeleteOnDestroy { get { return false; } }
        public override double IDChange { get { return 0.50; } }
        public override bool CanDamage { get { return Controller == null || Controller.BeaconVulnerable; } }

        public InvasionBeacon(InvasionController controller)
            : base(18212, 39299, 1)
        {
            Controller = controller;
            Component = new BeaconItem(this);

            Name = "lighthouse";

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
                Component.Location = new Point3D(this.X - 1, this.Y, this.Z);
        }

        public override void OnMapChange()
        {
            base.OnMapChange();

            if (Component != null)
                Component.Map = this.Map;
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (Component != null && !Component.Deleted)
                Component.Delete();
        }

        public override bool OnBeforeDestroyed()
        {
            if (Controller != null)
                Controller.OnBeaconDestroyed();

            List<Item> delete = new List<Item>();

            if (Rubble != null)
            {
                foreach (Item i in Rubble.Where(item => item.Z > this.Z))
                {
                    i.Delete();
                    delete.Add(i);
                }

                delete.ForEach(i => Rubble.Remove(i));
            }

            DoEffects();

            if (Component != null)
            {
                Component.ItemID = 1;
                Component.Visible = false;
            }

            Visible = false;

            delete.Clear();
            delete.TrimExcess();

            AddRubble(new Static(634), new Point3D(this.X - 2, this.Y, this.Z));
            AddRubble(new Static(633), new Point3D(this.X - 2, this.Y + 1, this.Z));

            AddRubble(new Static(635), new Point3D(this.X + 2, this.Y - 2, this.Z));
            AddRubble(new Static(632), new Point3D(this.X + 3, this.Y - 2, this.Z));

            AddRubble(new Static(634), new Point3D(this.X + 2, this.Y, this.X));
            AddRubble(new Static(633), new Point3D(this.X + 2, this.Y + 1, this.Z));
            return true;
        }

        private void DoEffects()
        {
            int range = 8;

            //Flamestrikes
            for (int i = 0; i < range; i++)
            {
                Timer.DelayCall(TimeSpan.FromMilliseconds(i * 50), o =>
                    {
                        Server.Misc.Geometry.Circle2D(this.Location, this.Map, (int)o, (pnt, map) =>
                        {
                            Effects.SendLocationEffect(pnt, map, 0x3709, 30, 20, 0, 2);
                        });
                    }, i);
            }

            //Explosions
            Timer.DelayCall(TimeSpan.FromMilliseconds(1000), () =>
            {
                for (int i = 0; i < range + 3; i++)
                {
                    Server.Misc.Geometry.Circle2D(this.Location, this.Map, i, (pnt, map) =>
                    {
                        Effects.SendLocationEffect(pnt, map, 0x36CB, 14, 10, 2498, 2);
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
                        Server.Misc.Geometry.Circle2D(this.Location, this.Map, (int)o, (pnt, map) =>
                        {
                            Effects.SendLocationEffect(pnt, map, Utility.RandomBool() ? 14000 : 14013, 14, 20, 2018, 0);
                        });
                    }, i);
                }
            });
        }

        public override void OnIDChange(int oldID)
        {
            if (ItemID == IDHalfHits && oldID == IDStart)
            {
                AddRubble(new Static(6571), new Point3D(this.X, this.Y + 1, this.Z + 42));
                AddRubble(new Static(3118), new Point3D(this.X - 1, this.Y + 1, this.Z));
                AddRubble(new Static(3118), new Point3D(this.X + 1, this.Y - 1, this.Z));
            }
        }

        public override void OnDamage(int amount, Mobile from, bool willkill)
        {
            base.OnDamage(amount, from, willkill);

            if (this.ItemID == IDHalfHits && this.Hits <= (HitsMax * .25))
            {
                IPooledEnumerable eable = this.Map.GetMobilesInRange(this.Location, 20);

                foreach (Mobile m in eable)
                {
                    if (m.NetState != null)
                        m.PrivateOverheadMessage(Server.Network.MessageType.Regular, 1154, 1154551, m.NetState); // *Minax's Beacon surges with energy into an invulnerable state! Defeat her Captains to weaken the Beacon's defenses!*
                }

                eable.Free();

                if (Controller != null)
                    Controller.SpawnWave();

                AddRubble(new Static(14732), new Point3D(this.X - 1, this.Y + 1, this.Z));
                AddRubble(new Static(14742), new Point3D(this.X + 1, this.Y - 1, this.Z));
                AddRubble(new Static(14742), new Point3D(this.X, this.Y, this.Z + 63));

                AddRubble(new Static(6571), new Point3D(this.X + 1, this.Y + 1, this.Z + 42));
                AddRubble(new Static(6571), new Point3D(this.X + 1, this.Y, this.Z + 59));

                this.ItemID = 39300;
            }

            if (0.02 > Utility.RandomDouble())
            {
                DoAreaAttack();
            }
        }

        private void AddRubble(Item i, Point3D p)
        {
            i.MoveToWorld(p, this.Map);

            if (Rubble == null)
                Rubble = new List<Item>();

            Rubble.Add(i);
        }

        public override void Delete()
        {
            base.Delete();

            if (Rubble != null)
            {
                List<Item> rubble = new List<Item>(Rubble);

                rubble.ForEach(i => i.Delete());
                rubble.ForEach(i => Rubble.Remove(i));

                rubble.Clear();
                rubble.TrimExcess();
            }
        }

        private void DoAreaAttack()
        {
            List<Mobile> list = new List<Mobile>();
            IPooledEnumerable eable = this.Map.GetMobilesInRange(this.Location, 8);

            foreach (Mobile m in eable)
            {
                if (m.AccessLevel > AccessLevel.Player)
                    continue;

                if (m is PlayerMobile || (m is BaseCreature && ((BaseCreature)m).GetMaster() is PlayerMobile))
                    list.Add(m);
            }

            list.ForEach(m =>
            {
                m.BoltEffect(0);
                AOS.Damage(m, null, Utility.RandomMinMax(80, 90), 0, 0, 0, 0, 100);

                if(m.NetState != null)
                    m.PrivateOverheadMessage(Server.Network.MessageType.Regular, 1154, 1154552, m.NetState); // *The beacon blasts a surge of energy at you!"
            });

            list.Clear();
            list.TrimExcess();
        }

        public InvasionBeacon(Serial serial)
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
                Item item = reader.ReadItem();
                if (item != null)
                {
                    if (Rubble == null)
                        Rubble = new List<Item>();

                    Rubble.Add(item);
                }
            }
		}
    }

    public class BeaconItem : Item
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public InvasionBeacon Beacon { get; set; }

        public override bool ForceShowProperties { get { return true; } }

        public BeaconItem(InvasionBeacon beacon)
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
}