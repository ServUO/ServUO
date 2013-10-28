using System;
using Server.Network;
using Server.Spells;

namespace Server.Items
{
    [Flipable(0x125E, 0x1230)]
    public class GuillotineComponent : AddonComponent
    {
        public GuillotineComponent()
            : base(0x125E)
        {
        }

        public GuillotineComponent(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1024656;
            }
        }// Guillotine
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class GuillotineAddon : BaseAddon
    {
        [Constructable]
        public GuillotineAddon()
            : base()
        {
            this.AddComponent(new GuillotineComponent(), 0, 0, 0);
        }

        public GuillotineAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new GuillotineDeed();
            }
        }
        public override void OnComponentUsed(AddonComponent c, Mobile from)
        {
            if (from.InRange(this.Location, 2))
            {
                if (Utility.RandomBool())
                {
                    from.Location = this.Location;

                    Timer.DelayCall(TimeSpan.FromSeconds(0.5), new TimerStateCallback(Activate), new object[] { c, from });
                }
                else
                    from.LocalOverheadMessage(MessageType.Regular, 0, 501777); // Hmm... you suspect that if you used this again, it might hurt.
            }
            else
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }

        public virtual void Activate(AddonComponent c, Mobile from)
        {
            if (c.ItemID == 0x125E || c.ItemID == 0x1269 || c.ItemID == 0x1260)
                c.ItemID = 0x1269;
            else
                c.ItemID = 0x1247;

            // blood
            int amount = Utility.RandomMinMax(3, 7);

            for (int i = 0; i < amount; i++)
            {
                int x = c.X + Utility.RandomMinMax(-1, 1);
                int y = c.Y + Utility.RandomMinMax(-1, 1);
                int z = c.Z;

                if (!c.Map.CanFit(x, y, z, 1, false, false, true))
                {
                    z = c.Map.GetAverageZ(x, y);

                    if (!c.Map.CanFit(x, y, z, 1, false, false, true))
                        continue;
                }

                Blood blood = new Blood(Utility.RandomMinMax(0x122C, 0x122F));
                blood.MoveToWorld(new Point3D(x, y, z), c.Map);
            }

            if (from.Female)
                from.PlaySound(Utility.RandomMinMax(0x150, 0x153));
            else
                from.PlaySound(Utility.RandomMinMax(0x15A, 0x15D));

            from.LocalOverheadMessage(MessageType.Regular, 0, 501777); // Hmm... you suspect that if you used this again, it might hurt.
            SpellHelper.Damage(TimeSpan.Zero, from, Utility.Dice(2, 10, 5));

            Timer.DelayCall(TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(0.5), 2, new TimerStateCallback(Deactivate), c);
        }

        private void Activate(object obj)
        {
            object[] param = (object[])obj;

            if (param[0] is AddonComponent && param[1] is Mobile)
                this.Activate((AddonComponent)param[0], (Mobile)param[1]);
        }

        private void Deactivate(object obj)
        {
            if (obj is AddonComponent)
            {
                AddonComponent c = (AddonComponent)obj;

                if (c.ItemID == 0x1269)
                    c.ItemID = 0x1260;
                else if (c.ItemID == 0x1260)
                    c.ItemID = 0x125E;
                else if (c.ItemID == 0x1247)
                    c.ItemID = 0x1246;
                else if (c.ItemID == 0x1246)
                    c.ItemID = 0x1230;
            }
        }
    }

    public class GuillotineDeed : BaseAddonDeed
    {
        [Constructable]
        public GuillotineDeed()
            : base()
        {
            this.LootType = LootType.Blessed;
        }

        public GuillotineDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new GuillotineAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1024656;
            }
        }// Guillotine
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}