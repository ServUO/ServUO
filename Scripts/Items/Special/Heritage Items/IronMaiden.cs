using System;
using Server.Network;
using Server.Spells;

namespace Server.Items
{
    public class IronMaidenAddon : BaseAddon
    {
        public IronMaidenAddon()
            : base()
        {
            this.AddComponent(new LocalizedAddonComponent(0x1249, 1076288), 0, 0, 0);
        }

        public IronMaidenAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new IronMaidenDeed();
            }
        }
        public override void OnComponentUsed(AddonComponent c, Mobile from)
        {
            if (from.InRange(this.Location, 2))
            {
                if (Utility.RandomBool())
                {
                    from.Location = this.Location;
                    c.ItemID = 0x124A;

                    Timer.DelayCall(TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(0.5), 3, new TimerStateCallback(Activate), new object[] { c, from });
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
            c.ItemID += 1;

            if (c.ItemID >= 0x124D)
            {
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

                Timer.DelayCall(TimeSpan.FromSeconds(1), new TimerStateCallback(Deactivate), c);
            }
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
                ((AddonComponent)obj).ItemID = 0x1249;
        }
    }

    public class IronMaidenDeed : BaseAddonDeed
    {
        [Constructable]
        public IronMaidenDeed()
            : base()
        {
            this.LootType = LootType.Blessed;
        }

        public IronMaidenDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new IronMaidenAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1076288;
            }
        }// Iron Maiden
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