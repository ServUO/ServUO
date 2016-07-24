using System;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Items
{
    public class AcidSac : Item
    {
        public override int LabelNumber { get { return 1111654; } } // acid sac

        [Constructable]
        public AcidSac()
            : base(0x0C67)
        {
            this.Stackable = true;
            this.Weight = 1.0;
            this.Hue = 0x464;
        }

        public AcidSac(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.Backpack == null || !IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1080063); // This must be in your backpack to use it.
                return;
            }

            from.SendLocalizedMessage(1111656); // What do you wish to use the acid on?
            from.Target = new InternalTarget(this);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        private class InternalTarget : Target
        {
            private readonly Item m_Item;
            private Item wall;
            private Item wallandvine;
            public InternalTarget(Item item)
                : base(2, false, TargetFlags.None)
            {
                this.m_Item = item;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                PlayerMobile pm = from as PlayerMobile;

                if (this.m_Item.Deleted)
                    return;

                if (targeted is AddonComponent)
                {
                    AddonComponent addoncomponent = (AddonComponent)targeted;

                    if (addoncomponent is MagicVinesComponent || addoncomponent is StoneWallComponent || addoncomponent is DungeonWallComponent)
                    {
                        int Xs = addoncomponent.X;

                        if (addoncomponent is MagicVinesComponent)
                            Xs += -1;

                        if (addoncomponent.Addon is StoneWallAndVineAddon)
                        {
                            this.wall = new SecretStoneWallNS();
                            this.wallandvine = new StoneWallAndVineAddon();
                        }
                        else if (addoncomponent.Addon is DungeonWallAndVineAddon)
                        {
                            this.wall = new SecretDungeonWallNS();
                            this.wallandvine = new DungeonWallAndVineAddon();
                        }

                        this.wall.MoveToWorld(new Point3D(Xs, addoncomponent.Y, addoncomponent.Z), addoncomponent.Map);

                        addoncomponent.Delete();

                        this.m_Item.Consume();

                        this.wall.PublicOverheadMessage(0, 1358, 1111662); // The acid quickly burns through the writhing wallvines, revealing the strange wall.

                        Timer.DelayCall(TimeSpan.FromSeconds(15.0), delegate ()
                        {
                            this.wallandvine.MoveToWorld(this.wall.Location, this.wall.Map);

                            this.wall.Delete();
                            this.wallandvine.PublicOverheadMessage(0, 1358, 1111663); // The vines recover from the acid and, spreading like tentacles, reclaim their grip over the wall.
                        });
                    }
                }
                else
                {
                    from.SendLocalizedMessage(1111657); // The acid swiftly burn through it.
                    this.m_Item.Consume();
                    return; // Exit the method, because addoncomponent is null
                }
            }
        }
    }
}