using System;
using Server;
using Server.Network;
using Server.Items;
using System.Collections;

namespace Server.Items
{
    public class SawmillBlade : AddonComponent
    {
        [Constructable]
        public SawmillBlade() : base(4533)
        {
            Name = "An dangerous looking saw blade";
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(this.GetWorldLocation(), 1))
            {
                from.SendMessage(89, "Your too far away from the spinning blade to do any damage.");
            }
            else
            {
                from.SendMessage(89, "You Almost loose a finger !!!");
                Effects.PlaySound(from.Location, from.Map, 0x218);    // Plays the saw sound
                BeginBleed(from);
                // BLOOD
            }
        }

        // ****************************
        // *** BLOOD CODE FROM HERE ***
        // ****************************
        private static Hashtable m_BloodTable = new Hashtable();

        public static bool IsBleeding(Mobile m)
        {
            return m_BloodTable.Contains(m);
        }

        public static void BeginBleed(Mobile m)
        {
            Timer t = (Timer)m_BloodTable[m];

            if (t != null)
                t.Stop();

            t = new InternalTimer(m);
            m_BloodTable[m] = t;

            t.Start();
        }

        public static void DoBleed(Mobile m)
        {
            if (m.Alive)
            {
                m.PlaySound(0x133);

                Blood blood = new Blood();

                blood.ItemID = Utility.Random(0x122A, 5);

                blood.MoveToWorld(m.Location, m.Map);
            }
            else
            {
                EndBleed(m, false);
            }
        }

        public static void EndBleed(Mobile m, bool message)
        {
            Timer t = (Timer)m_BloodTable[m];

            if (t == null)
                return;

            t.Stop();
            m_BloodTable.Remove(m);

            if (message)
                m.SendLocalizedMessage(1060167); // The bleeding wounds have healed, you are no longer bleeding!
        }

        private class InternalTimer : Timer
        {
            private Mobile m_Mobile;
            private int m_Count;

            public InternalTimer(Mobile m) : base(TimeSpan.FromSeconds(2.0), TimeSpan.FromSeconds(2.0))
            {
                m_Mobile = m;
                Priority = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                DoBleed(m_Mobile);

                if (++m_Count == 5)
                    EndBleed(m_Mobile, true);
            }
        }

        // ***************************

        public SawmillBlade(Serial serial) : base(serial)
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

    public class SawmillSign : AddonComponent
    {
        private string m_SafetyWarning = "Safety Warning";
        private string m_Resource = "ALL LOG TYPES";

        [Constructable]
        public SawmillSign() : base(3026)
        {
            Name = "The 'Cut-O-Matic' Lumber Mill";
            Hue = 33;
        }

        public override void OnDoubleClick(Mobile from)
        {
            from.SendMessage(89, "Don't play with the blade");
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060661, "This Sawmill Cuts \t{0}", m_Resource.ToString());
            list.Add(1060658, "{0} \tNever Put Your Fingers Near The Blade.", m_SafetyWarning.ToString());
            list.Add(1060659, "{0} \tAlways Use Correct Protective Equipment.", m_SafetyWarning.ToString());
        }

        public SawmillSign(Serial serial) : base(serial)
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

    public class SawmillCrate : AddonComponent
    {
        [Constructable]
        public SawmillCrate() : base(3645)
		{
            Name = "Sawmill Crate";
		}

        public override void OnDoubleClick(Mobile from)
        {
            from.SendMessage(89, "Place your logs into the crate");
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            int amounttocut = 0;

            if (!from.InRange(this.GetWorldLocation(), 1))
            {
                from.SendMessage(89, "You are too far from the machine, step closer.");
                return false;
            }
            else
            {
                if (dropped is Log) // Log is now BaseLog
                {
                    if (dropped is OakLog)
                    {
                        // It's an OAK log
                        OakLog yourlogs = (OakLog)dropped;
                        dropped.Delete();
                        amounttocut = yourlogs.Amount;
                        from.SendMessage(89, "You recieve {0} boards", amounttocut);
                        Effects.PlaySound(from.Location, from.Map, 0x218);  // Plays the saw sound
                        Item spawn = new OakBoard(amounttocut);
                        spawn.MoveToWorld(new Point3D(this.X, this.Y - 3, this.Z + 3), this.Map);
                        return true;
                    }

                    else if (dropped is AshLog)
                    {
                        // It's an ASH log
                        AshLog yourlogs = (AshLog)dropped;
                        dropped.Delete();
                        amounttocut = yourlogs.Amount;
                        from.SendMessage(89, "You recieve {0} boards", amounttocut);
                        Effects.PlaySound(from.Location, from.Map, 0x218);  // Plays the saw sound
                        Item spawn = new AshBoard(amounttocut);
                        spawn.MoveToWorld(new Point3D(this.X, this.Y - 3, this.Z + 3), this.Map);
                        return true;
                    }

                    else if (dropped is YewLog)
                    {
                        // It's a YEW log
                        YewLog yourlogs = (YewLog)dropped;
                        dropped.Delete();
                        amounttocut = yourlogs.Amount;
                        from.SendMessage(89, "You recieve {0} boards", amounttocut);
                        Effects.PlaySound(from.Location, from.Map, 0x218);  // Plays the saw sound
                        Item spawn = new YewBoard(amounttocut);
                        spawn.MoveToWorld(new Point3D(this.X, this.Y - 3, this.Z + 3), this.Map);
                        return true;
                    }

                    else if (dropped is HeartwoodLog)
                    {
                        // It's a HEARTWOOD log
                        HeartwoodLog yourlogs = (HeartwoodLog)dropped;
                        dropped.Delete();
                        amounttocut = yourlogs.Amount;
                        from.SendMessage(89, "You recieve {0} boards", amounttocut);
                        Effects.PlaySound(from.Location, from.Map, 0x218);  // Plays the saw sound
                        Item spawn = new HeartwoodBoard(amounttocut);
                        spawn.MoveToWorld(new Point3D(this.X, this.Y - 3, this.Z + 3), this.Map);
                        return true;
                    }

                    else if (dropped is BloodwoodLog)
                    {
                        // It's a BLOODWOOD log
                        BloodwoodLog yourlogs = (BloodwoodLog)dropped;
                        dropped.Delete();
                        amounttocut = yourlogs.Amount;
                        from.SendMessage(89, "You recieve {0} boards", amounttocut);
                        Effects.PlaySound(from.Location, from.Map, 0x218);  // Plays the saw sound
                        Item spawn = new BloodwoodBoard(amounttocut);
                        spawn.MoveToWorld(new Point3D(this.X, this.Y - 3, this.Z + 3), this.Map);
                        return true;
                    }

                    else if (dropped is FrostwoodLog)
                    {
                        // It's a FROSTWOOD log
                        FrostwoodLog yourlogs = (FrostwoodLog)dropped;
                        dropped.Delete();
                        amounttocut = yourlogs.Amount;
                        from.SendMessage(89, "You recieve {0} boards", amounttocut);
                        Effects.PlaySound(from.Location, from.Map, 0x218);  // Plays the saw sound
                        Item spawn = new FrostwoodBoard(amounttocut);
                        spawn.MoveToWorld(new Point3D(this.X, this.Y - 3, this.Z + 3), this.Map);
                        return true;
                    }

                    else if (dropped is Log)
                    {
                        // It's a NORMAL log
                        Log yourlogs = (Log)dropped;
                        dropped.Delete();
                        amounttocut = yourlogs.Amount;
                        from.SendMessage(89, "You recieve {0} boards", amounttocut);
                        Effects.PlaySound(from.Location, from.Map, 0x218);  // Plays the saw sound
                        Item spawn = new Board(amounttocut);
                        spawn.MoveToWorld(new Point3D(this.X, this.Y - 3, this.Z + 3), this.Map);
                        return true;
                    }
                }

                from.SendMessage(89, "This can only cut logs.");
                return false;
            }
            return false;
        }
        public SawmillCrate(Serial serial) : base(serial)
		{
		}

        public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
    }

	public class SawmillAddon : BaseAddon
	{
        public override BaseAddonDeed Deed
        { get { return new SawmillAddonDeed(); } }

        [Constructable]
        public SawmillAddon()
        {
            // base
            AddComponent(new AddonComponent(2328), 0, 0, 0);
            AddComponent(new AddonComponent(2328), 0, -3, 0);
            AddComponent(new AddonComponent(1872), 0, -1, 0);
            AddComponent(new AddonComponent(1872), 0, -2, 0);

            // platforms
            AddComponent(new AddonComponent(1981), 0, -1, 5);
            AddComponent(new AddonComponent(1981), 0, -2, 5);
            AddComponent(new AddonComponent(1981), 0, -3, 1);

            // blade
            AddComponent(new SawmillBlade(), 0, -1, 4);

            // sign
            AddComponent(new SawmillSign(), 1, -1, -4);

            // crate
            AddComponent(new SawmillCrate(), 0, 0, 0);
        }

        public SawmillAddon(Serial serial) : base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}	
	}

	public class SawmillAddonDeed : BaseAddonDeed
	{
        public override BaseAddon Addon
        { get { return new SawmillAddon(); } }

        [Constructable]
		public SawmillAddonDeed()
		{
			Name = "Cut-o-Matic Sawmill Addon [ALL LOGS]";
		}

        public SawmillAddonDeed(Serial serial) : base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}