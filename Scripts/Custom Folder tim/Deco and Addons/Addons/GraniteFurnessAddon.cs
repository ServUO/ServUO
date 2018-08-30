using System;
using System;
using Server;
using Server.Network;
using Server.Items;
using System.Collections;

namespace Server.Items
{
    public class GraniteFurnessAddonDeed : BaseAddonDeed
    {
        public override BaseAddon Addon
        { get { return new GraniteFurnessAddon(); } }

        [Constructable]
        public GraniteFurnessAddonDeed()
        {
            Name = "A Granite Furness";
        }

        public GraniteFurnessAddonDeed(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class GraniteFurnessAddon : BaseAddon
    {
        [Server.Engines.Craft.Forge]
        public override BaseAddonDeed Deed
        { get { return new GraniteFurnessAddonDeed(); } }

        [Constructable]
        public GraniteFurnessAddon()
        {
            // Furness Walls 1st Layer
            AddComponent(new AddonComponent(1822), -3, -3, 0);
            AddComponent(new AddonComponent(1822), -2, -3, 0);
            AddComponent(new AddonComponent(1822), -1, -3, 0);

            AddComponent(new AddonComponent(1822), -3, -2, 0);
            AddComponent(new AddonComponent(1822), -3, -1, 0);

            AddComponent(new AddonComponent(1822), -2, -1, 0);
            AddComponent(new AddonComponent(1822), -1, -1, 0);
            AddComponent(new AddonComponent(1822), -1, -2, 0);

            // Furness Walls 2nd Layer
            AddComponent(new AddonComponent(1822), -3, -3, 5);
            AddComponent(new AddonComponent(1822), -2, -3, 5);
            AddComponent(new AddonComponent(1822), -1, -3, 5);

            AddComponent(new AddonComponent(1822), -3, -2, 5);
            AddComponent(new AddonComponent(1822), -3, -1, 5);

            // Front Pillar
            AddComponent(new AddonComponent(1822), -1, -1, 5);
            AddComponent(new AddonComponent(1822), -1, -1, 10);

            // Furness Wall 3rd Layer
            AddComponent(new AddonComponent(1822), -3, -3, 10);
            AddComponent(new AddonComponent(1822), -2, -3, 10);
            AddComponent(new AddonComponent(1822), -1, -3, 10);

            AddComponent(new AddonComponent(1822), -3, -2, 10);
            AddComponent(new AddonComponent(1822), -3, -1, 10);

            // Stone Top
            AddComponent(new AddonComponent(2325), -3, -3, 15);
            AddComponent(new AddonComponent(2325), -2, -3, 15);
            AddComponent(new AddonComponent(2325), -1, -3, 15);

            AddComponent(new AddonComponent(2325), -3, -2, 15);
            AddComponent(new AddonComponent(2325), -1, -2, 15);

            AddComponent(new AddonComponent(2325), -3, -1, 15);
            AddComponent(new AddonComponent(2325), -2, -1, 15);
            AddComponent(new AddonComponent(2325), -1, -1, 15);

            // Top Parts
            AddComponent(new AddonComponent(8705), -3, -3, 16);
            AddComponent(new AddonComponent(8705), -1, -3, 16);
            AddComponent(new AddonComponent(8705), -3, -1, 16);
            AddComponent(new AddonComponent(8706), -1, -1, 16);

            // Stone Floor Front Parts
            AddComponent(new AddonComponent(2328), -1, 0, 0);
            AddComponent(new AddonComponent(2328), -2, 0, 0);
            AddComponent(new AddonComponent(2328), -3, 0, 0);
            AddComponent(new AddonComponent(2328), 0, -1, 0);
            AddComponent(new AddonComponent(2328), 0, -2, 0);
            AddComponent(new AddonComponent(2328), 0, -3, 0);

            AddComponent(new AddonComponent(1981), 0, -2, 1);

            // Animated Parts in Center
            AddComponent(new AddonComponent(14089), -2, -2, 0);
            AddComponent(new AddonComponent(2339), -2, -2, 13);

            // Ladder
            AddComponent(new AddonComponent(2214), 0, -3, 0);

            // Winch
            AddComponent(new AddonComponent(7849), -2, -1, 16);

            // Sign On Top
            AddComponent(new GraniteFurnessSign(), -1, -2, 17);

            // Pulley
            AddComponent(new GraniteFurnessPulley(), -3, 0, 10);

            // Furness Fire South
            AddComponent(new GraniteFurnessFlamesSouth(), -2, -1, 5);

            // Furness Fire East
            AddComponent(new GraniteFurnessFlamesEast(), -1, -2, 5);

            // Furness Crate
            AddComponent(new GraniteFurnessCrate(), -2, 0, 0);
        }

        public GraniteFurnessAddon(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class GraniteFurnessSign : AddonComponent
    {
        private string m_SafetyWarning = "Safety Warning";
        private string m_Resource = "Normal Granite";

        [Constructable]
        public GraniteFurnessSign() : base(7976)
        {
            Name = "Granite Furness";
            Weight = 2500;
            Hue = 33;
        }

        public override void OnDoubleClick(Mobile from)
        {
            from.SendMessage(89, "Keep Hands Away For The Furness.");
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060661, "This Furness Uses \t{0}", m_Resource.ToString());
            list.Add(1060658, "{0} \tKeep Hands Away From The Furness Fire At All Times.", m_SafetyWarning.ToString());
            list.Add(1060659, "{0} \tAlways Use The Correct Protective Equipment.", m_SafetyWarning.ToString());
        }

        public GraniteFurnessSign(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class GraniteFurnessPulley : AddonComponent
    {
        [Constructable]
        public GraniteFurnessPulley() : base(7838)
        {
            Name = "Granite Furness Pulley";
        }

        public override void OnDoubleClick(Mobile from)
        {
            from.SendMessage(89, "This part of the machine looks too complicated to you.");
        }

        public GraniteFurnessPulley(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class GraniteFurnessFlamesSouth : AddonComponent
    {
        [Constructable]
        public GraniteFurnessFlamesSouth() : base(6732)
        {
            Name = "Intensly Hot Flames";
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(this.GetWorldLocation(), 1))
            {
                from.SendMessage(89, "Your not close enough to the flames.");
            }
            else
            {
                from.SendMessage(89, "You play with the fire... and get burnt !!!");
                Effects.SendLocationEffect(new Point3D(from.X, from.Y, from.Z + 1), from.Map, 0x3709, 15);
                Effects.PlaySound(new Point3D(from.X, from.Y, from.Z), from.Map, 0x208);
            }
        }

        public GraniteFurnessFlamesSouth(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class GraniteFurnessFlamesEast : AddonComponent
    {
        [Constructable]
        public GraniteFurnessFlamesEast() : base(6686)
        {
            Name = "Intensly Hot Flames";
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(this.GetWorldLocation(), 1))
            {
                from.SendMessage(89, "Your not close enough to the flames.");
            }
            else
            {
                from.SendMessage(89, "You play with the fire... and get burnt !!!");
                Effects.SendLocationEffect(new Point3D(from.X, from.Y, from.Z + 1), from.Map, 0x3709, 15);
                Effects.PlaySound(new Point3D(from.X, from.Y, from.Z), from.Map, 0x208);
            }
        }

        public GraniteFurnessFlamesEast(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class GraniteFurnessCrate : AddonComponent
    {
        [Constructable]
        public GraniteFurnessCrate() : base(3645)
        {
            Name = "Granite Crate";
        }

        public override void OnDoubleClick(Mobile from)
        {
            from.SendMessage(89, "Place your Normal Granite into the crate");
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (!from.InRange(this.GetWorldLocation(), 1))
            {
                from.SendMessage(89, "You are too far from the crate, step closer.");
                return false;
            }
            else
            {
                if (dropped is BaseGranite)
                {
                    if (dropped is Granite)
                    {
                        // NORMAL GRANITE
                        Granite yourgranite = (Granite)dropped;
                        dropped.Delete();
                        from.SendMessage(89, "The machine processes your granite.");
                        Effects.PlaySound(from.Location, from.Map, 0x2F3);  // Plays the earthquake sound
                        Item spawn = new Sand();
                        spawn.MoveToWorld(new Point3D(this.X + 2, this.Y - 2, this.Z + 2), this.Map);
                        return true;
                    }
                    else
                    {
                        // Wrong Granite Type (Not Normal)
                        if (dropped is BaseGranite)
                        {
                            BaseGranite yourgranite = (BaseGranite)dropped;
                            dropped.Delete();
                            from.SendMessage(89, "The machine tries to processes the granite but fail's.");
                            Effects.PlaySound(from.Location, from.Map, 0x208);  // Plays the sound
                            from.SendMessage(89, "You loose your piece of granite!!!");
                            return true;
                        }

                        return false;
                    }
                }
                from.SendMessage(89, "This machine can only process normal granite.");
                return false;
            }
            return false;
        }

        public GraniteFurnessCrate(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}