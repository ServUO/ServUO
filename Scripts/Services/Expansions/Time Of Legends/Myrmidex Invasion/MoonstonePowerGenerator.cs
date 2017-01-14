using Server;
using System;
using Server.Mobiles;
using System.Collections.Generic;
using System.Linq;
using Server.Commands;

namespace Server.Items
{
    public class MoonstonePowerGeneratorAddon : BaseAddon
    {
        public override bool ShareHue { get { return false; } }

        public Timer ActiveTimer { get; set; }
        public InternalComponent Activator1 { get; set; }
        public InternalComponent Activator2 { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Activated { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool SetActive
        {
            get
            {
                return Activated;
            }
            set
            {
                if (value)
                {
                    if (ActiveTimer != null)
                    {
                        ActiveTimer.Stop();
                        ActiveTimer = null;
                    }

                    Activated = true;
                    CheckNetwork();
                }
            }
        }

        public bool Link { get; set; }

        public MoonstonePowerGenerator Generator { get; set; }

        [Constructable]
        public MoonstonePowerGeneratorAddon(bool link)
        {
            AddonComponent c = new AddonComponent(39759);
            c.Hue = 2955;
            AddComponent(c, 0, 0, 0);

            c = new AddonComponent(39759);
            c.Hue = 2955;
            AddComponent(c, -1, 0, 0);

            c = new AddonComponent(39759);
            c.Hue = 2955;
            AddComponent(c, 1, 0, 0);

            c = new AddonComponent(39759);
            c.Hue = 2955;
            AddComponent(c, 0, -1, 0);

            c = new AddonComponent(39759);
            c.Hue = 2955;
            AddComponent(c, 0, 1, 0);

            c = new AddonComponent(39818);
            c.Hue = 2955;
            AddComponent(c, -1, -1, 0);

            c = new AddonComponent(39818);
            c.Hue = 2955;
            AddComponent(c, -1, 1, 0);

            c = new AddonComponent(39818);
            c.Hue = 2955;
            AddComponent(c, 1, -1, 0);

            c = new AddonComponent(39818);
            c.Hue = 2955;
            AddComponent(c, 1, 1, 0);

            Activator1 = new InternalComponent(40158);
            Activator2 = new InternalComponent(40203);

            AddComponent(Activator1, 0, -1, 5);
            AddComponent(Activator2, 0, 1, 5);

            AddComponent(new LocalizedAddonComponent(40155, 1156623), 1, 0, 5);
            AddComponent(new LocalizedAddonComponent(40155, 1156623), -1, 0, 5);

            AddComponent(new LocalizedAddonComponent(40156, 1156628), -1, 0, 10);
            AddComponent(new LocalizedAddonComponent(40156, 1156628), 1, 0, 10);

            //AddComponent(new LocalizedAddonComponent(40147, 1124171), 0, 0, 5);
            AddComponent(new LocalizedAddonComponent(40157, 1124171), 0, 0, 20);

            Generator = new MoonstonePowerGenerator(this);

            Link = link;

            if (link)
                Generators.Add(this);
        }

        public override void OnLocationChange(Point3D oldlocation)
        {
            base.OnLocationChange(oldlocation);

            if (Generator != null && !Generator.Deleted)
            {
                Generator.MoveToWorld(new Point3D(this.X, this.Y, this.Z + 5), this.Map);
            }
        }

        public override void OnMapChange()
        {
            base.OnMapChange();

            if (Generator != null && !Generator.Deleted)
            {
                Generator.Map = this.Map;
            }
        }

        public override void OnComponentUsed(AddonComponent component, Mobile from)
        {
            if (!Activated && component != null && component is InternalComponent && from.InRange(component.Location, 2))
            {
                InternalComponent comp = component as InternalComponent;

                if (!comp.Active)
                {
                    comp.Active = true;
                    comp.WhoActivated = from;

                    if (Activator1.Active && Activator2.Active && Activator1.WhoActivated != Activator2.WhoActivated)
                    {
                        if (ActiveTimer != null)
                        {
                            ActiveTimer.Stop();
                            ActiveTimer = null;
                        }

                        Activated = true;
                        CheckNetwork();
                    }
                    else if (ActiveTimer == null)
                        ActiveTimer = Timer.DelayCall(TimeSpan.FromSeconds(1), Reset);
                }
            }
        }

        private void Reset()
        {
            if (Activator1 != null)
                Activator1.Active = false;

            if (Activator2 != null)
                Activator2.Active = false;

            ActiveTimer = null;
        }

        public class InternalComponent : LocalizedAddonComponent
        {
            private bool _Active;

            [CommandProperty(AccessLevel.GameMaster)]
            public bool Active
            {
                get { return _Active; }
                set
                {
                    if (!_Active && value)
                    {
                        ItemID = ActiveID;
                        Effects.PlaySound(this.Location, this.Map, 0x051);
                    }
                    else if (_Active && !value)
                    {
                        ItemID = InactiveID;
                        WhoActivated = null;
                        Effects.PlaySound(this.Location, this.Map, 0x051);
                    }

                    _Active = value;
                }
            }

            public Mobile WhoActivated { get; set; }

            public int ActiveID { get; set; }
            public int InactiveID { get; set; }

            public InternalComponent(int itemid)
                : base(itemid, 1156624)
            {
                InactiveID = itemid;

                if (itemid == 40203)
                    ActiveID = 40158;
                else
                    ActiveID = 40203;
            }

            public InternalComponent(Serial serial)
                : base(serial)
            {
            }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);
                writer.Write(0);

                writer.Write(ActiveID);
                writer.Write(InactiveID);

                writer.Write(_Active);
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);
                int v = reader.ReadInt();

                ActiveID = reader.ReadInt();
                InactiveID = reader.ReadInt();

                Active = reader.ReadBool();

                MoonstonePowerGeneratorAddon chamber = Addon as MoonstonePowerGeneratorAddon;

                if (chamber != null)
                {
                    if (chamber.Activator1 == null)
                        chamber.Activator1 = this;
                    else
                        chamber.Activator2 = this;
                }
            }
        }

        public override void Delete()
        {
            base.Delete();

            if (Generators.Contains(this))
                Generators.Remove(this);
        }

        public static void Configure()
        {
            Generators = new List<MoonstonePowerGeneratorAddon>();
        }

        public static void Initialize()
        {
            bool active = true;

            foreach (MoonstonePowerGeneratorAddon c in Generators)
            {
                if (!c.Activated)
                {
                    active = false;
                    break;
                }
            }

            if (!active)
            {
                ResetGenerators(true);
            }
            else if (Boss == null)
            {
                ResetGenerators();
            }

            CommandSystem.Register("ActivateChambers", AccessLevel.Administrator, e =>
                {
                    if (Boss == null)
                    {
                        Generators.ForEach(c => c.Activated = true);
                        CheckNetwork();
                    }
                });

            CommandSystem.Register("MorphChamberItems", AccessLevel.Administrator, e =>
                {
                    MorphItems();
                });
        }

        public static List<MoonstonePowerGeneratorAddon> Generators { get; set; }
        public static Zipactriotl Boss { get; set; }

        public static readonly Point3D GroundZero = new Point3D(896, 2304, -19);

        public static void CheckNetwork()
        {
            bool allactive = true;

            foreach (MoonstonePowerGeneratorAddon c in Generators)
            {
                if (!c.Activated)
                {
                    allactive = false;
                    break;
                }
            }

            if (allactive)
            {
                Boss = new Zipactriotl(true);
                Boss.MoveToWorld(new Point3D(899, 2303, -20), Map.TerMur);

                ColUtility.ForEach(Generators.Where(c => c.Generator != null), c =>
                {
                    c.Generator.CanSpawn = true;
                });

                MorphItems();
            }
        }

        public static void MorphItems()
        {
            IPooledEnumerable eable = Map.TerMur.GetItemsInRange(GroundZero, 15);

            foreach (Item item in eable)
            {
                if (item.ItemID == 40161)
                    item.ItemID = 40159;
                else if (item.ItemID == 40142)
                    item.ItemID = 40173;
                else if (item.ItemID == 40169)
                    item.ItemID = 40174;
                else if (item.ItemID == 40165)
                    item.ItemID = 40160;

                else if (item.ItemID == 40159)
                    item.ItemID = 40161;
                else if (item.ItemID == 40173)
                    item.ItemID = 40142;
                else if (item.ItemID == 40174)
                    item.ItemID = 40169;
                else if (item.ItemID == 40160)
                    item.ItemID = 40165;
            }
            eable.Free();
        }

        public static void ResetGenerators(bool startup = false)
        {
            Generators.ForEach(c =>
                {
                    c.Activated = false;
                    c.Reset();

                    if (c.Generator == null || c.Generator.Deleted)
                    {
                        c.Generator = new MoonstonePowerGenerator(c);
                        c.Generator.MoveToWorld(new Point3D(c.X, c.Y, c.Z + 5), c.Map);
                    }

                    c.Generator.CanSpawn = false;

                    c.Components.ForEach(comp =>
                    {
                        if (!comp.Visible)
                            comp.Visible = true;
                    });
                });

            if(!startup)
                MorphItems();

            if (Boss != null)
                Boss = null;
        }

        public MoonstonePowerGeneratorAddon(Serial serial)
            : base(serial)
        {
        }


        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(Activated);
            writer.Write(Link);
            writer.Write(Generator);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();

            Activated = reader.ReadBool();

            if (reader.ReadBool())
            {
                Generators.Add(this);
                Link = true;
            }

            Generator = reader.ReadItem() as MoonstonePowerGenerator;

            if (Generator != null)
                Generator.Addon = this;
        }
    }

    public class MoonstonePowerGenerator : DamageableItem
    {
        public override int LabelNumber { get { return 1156854; } } // Moonstone Power Generator

        public List<BaseCreature> Spawn;
        public Timer Timer { get; set; }

        private bool _CanSpawn;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool CanSpawn
        {
            get { return _CanSpawn; }
            set
            {
                _CanSpawn = value;

                if (_CanSpawn)
                {
                    Spawn = new List<BaseCreature>();

                    if (Timer != null)
                    {
                        Timer.Stop();
                        Timer = null;
                    }

                    Timer = Timer.DelayCall(TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30), OnTick);
                    Timer.Start();
                }
                else
                {
                    if (Timer != null)
                    {
                        Timer.Stop();
                        Timer = null;
                    }
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public MoonstonePowerGeneratorAddon Addon
        {
            get;
            set;
        }

        [Constructable]
        public MoonstonePowerGenerator(MoonstonePowerGeneratorAddon addon = null)
            : base(40147, 40153)
        {
            Addon = addon;

            Level = ItemLevel.Average;
            Name = "Moonstone Power Generator";
        }

        public void OnTick()
        {
            if (Spawn.Count >= 7 || this.Deleted || this.Map == null)
                return;

            IPooledEnumerable eable = this.Map.GetMobilesInRange(this.Location, 8);

            foreach (Mobile m in eable)
            {
                if (m is PlayerMobile || (m is BaseCreature && ((BaseCreature)m).GetMaster() is PlayerMobile))
                {
                    DoSpawn();
                    break;
                }
            }
            eable.Free();
        }

        private void DoSpawn()
        {
            if (Spawn.Count >= 7 || this.Deleted || this.Map == null)
                return;

            BaseCreature bc = new IgnisFatalis();

            int x = Utility.RandomBool() ? 2 : -2;
            int y = Utility.RandomBool() ? 2 : -2;

            bc.MoveToWorld(new Point3D(this.X + x, this.Y + y, this.Map.GetAverageZ(x, y)), this.Map);
            Spawn.Add(bc);
        }

        public override void OnDamage(int amount, Mobile from, bool willkill)
        {
            base.OnDamage(amount, from, willkill);

            int oldhits = Hits;

            if (this.ItemID == IDHalfHits && this.Hits <= (HitsMax * .10))
            {
                ItemID = 40154;
            }

            if (0.033 > Utility.RandomDouble())
            {
                from.PrivateOverheadMessage(Server.Network.MessageType.Regular, 0x23, 1156855, from.NetState); // *Arcing energy from the generator zaps you!*
                AOS.Damage(from, Utility.RandomMinMax(50, 100), 0, 0, 0, 0, 100);
                from.FixedParticles(0x3818, 1, 11, 0x13A8, 0, 0, EffectLayer.Waist);
                Effects.PlaySound(this.Location, this.Map, 0x1DC);
            }
        }

        public override void OnAfterDestroyed()
        {
            Effects.PlaySound(this.Location, this.Map, 0x665);

            if (Spawn != null)
            {
                Spawn.ForEach(bc =>
                    {
                        if(bc != null && bc.Alive)
                            bc.Kill();
                    });

                Spawn.Clear();
                Spawn.TrimExcess();
                Spawn = null;
            }

            if (Timer != null)
            {
                Timer.Stop();
                Timer = null;
            }

            if (Addon != null)
            {
                AddonComponent comp = Addon.Components.FirstOrDefault(c => c.ItemID == 40157);

                if (comp != null)
                    comp.Visible = false;
            }

            base.OnAfterDestroyed();
        }

        public MoonstonePowerGenerator(Serial serial)
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
            int v = reader.ReadInt();
        }
    }
}