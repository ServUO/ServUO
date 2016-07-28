using Server;
using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Multis;

namespace Server.Items
{
    public class TemporaryForge : BaseAddon
    {
        public const int DecayPeriod = 4;
        private Timer m_Timer;

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime Expires { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Owner { get; set; }

        public TemporaryForge()
        {
            AddComponent(new InternalComponent(), 0, 0, 0);

            Expires = DateTime.UtcNow + TimeSpan.FromHours(DecayPeriod);
            m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10), CheckDecay);
        }

        public override void OnChop(Mobile from)
        {
            if (BaseHouse.FindHouseAt(this) == null && Owner == from)
            {
                Effects.PlaySound(GetWorldLocation(), Map, 0x3B3);
                from.SendLocalizedMessage(500461); // You destroy the item.
                Delete();
                return;
            }

            base.OnChop(from);
        }

        public void CheckDecay()
        {
            if (Expires < DateTime.UtcNow)
                Decay();
            else
                InvalidateProperties();
        }

        public void Decay()
        {
            if (RootParent is Mobile)
            {
                Mobile parent = (Mobile)RootParent;

                if (Name == null)
                    parent.SendLocalizedMessage(1072515, "#" + LabelNumber); // The ~1_name~ expired...
                else
                    parent.SendLocalizedMessage(1072515, Name); // The ~1_name~ expired...

                Effects.SendLocationParticles(EffectItem.Create(parent.Location, parent.Map, EffectItem.DefaultDuration), 0x3728, 8, 20, 5042);
                Effects.PlaySound(parent.Location, parent.Map, 0x201);
            }
            else
            {
                Effects.SendLocationParticles(EffectItem.Create(this.Location, this.Map, EffectItem.DefaultDuration), 0x3728, 8, 20, 5042);
                Effects.PlaySound(this.Location, this.Map, 0x201);
            }

            Delete();
        }

        public override void Delete()
        {
            base.Delete();

            if (m_Timer != null)
            {
                m_Timer.Stop();
                m_Timer = null;
            }
        }

        private class InternalComponent : ForgeComponent
        {
            public override bool ForceShowProperties { get { return true; } }
            public override int LabelNumber { get { return 1152601; } } //Temporary forge

            public InternalComponent() : base(0xFB1)
            {
            }

            public override void GetProperties(ObjectPropertyList list)
            {
                base.GetProperties(list);

                if (Addon == null || !(Addon is TemporaryForge))
                    return;

                int left = 0;
                if (DateTime.UtcNow < ((TemporaryForge)Addon).Expires)
                    left = (int)(((TemporaryForge)Addon).Expires - DateTime.UtcNow).TotalSeconds;

                list.Add(1072517, left.ToString()); // Lifespan: ~1_val~ seconds
            }

            public InternalComponent(Serial serial)
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

        public TemporaryForge(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(Expires);
            writer.Write(Owner);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Expires = reader.ReadDateTime();
            Owner = reader.ReadMobile();

            if (Expires < DateTime.UtcNow)
                Decay();
            else
                m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10), CheckDecay);
        }
    }

    public class TemporaryForgeDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new TemporaryForge(); } }

        public const int DecayPeriod = 24;

        private Timer m_Timer;

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime Expires { get; set; }

        [Constructable]
        public TemporaryForgeDeed()
        {
            Expires = DateTime.UtcNow + TimeSpan.FromHours(DecayPeriod);
            m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10), CheckDecay);
        }

        public void CheckDecay()
        {
            if (Expires < DateTime.UtcNow)
                Decay();
            else
                InvalidateProperties();
        }

        public void Decay()
        {
            if (RootParent is Mobile)
            {
                Mobile parent = (Mobile)RootParent;

                if (Name == null)
                    parent.SendLocalizedMessage(1072515, "#" + LabelNumber); // The ~1_name~ expired...
                else
                    parent.SendLocalizedMessage(1072515, Name); // The ~1_name~ expired...

                Effects.SendLocationParticles(EffectItem.Create(parent.Location, parent.Map, EffectItem.DefaultDuration), 0x3728, 8, 20, 5042);
                Effects.PlaySound(parent.Location, parent.Map, 0x201);
            }
            else
            {
                Effects.SendLocationParticles(EffectItem.Create(this.Location, this.Map, EffectItem.DefaultDuration), 0x3728, 8, 20, 5042);
                Effects.PlaySound(this.Location, this.Map, 0x201);
            }

            Delete();
        }

        public override void Delete()
        {
            base.Delete();

            if (m_Timer != null)
            {
                m_Timer.Stop();
                m_Timer = null;
            }
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            list.Add(1152601); // Temporary Forge
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            int left = 0;
            if (DateTime.UtcNow < Expires)
                left = (int)(Expires - DateTime.UtcNow).TotalSeconds;

            list.Add(1072517, left.ToString()); // Lifespan: ~1_val~ seconds
            list.Add(1152627); // no house required
            list.Add(1152628); // cannot re-deed
            list.Add(1152629, TemporaryForge.DecayPeriod.ToString()); // lasts ~1_count~ hours
        }

        public override void OnDoubleClick(Mobile from)
        {
            //TODO: Finish the ontaret stuff and clilocs.
            if (!IsChildOf(from.Backpack))
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            else
            {
                BaseHouse house = BaseHouse.FindHouseAt(from);

                if (house != null)
                    base.OnDoubleClick(from);
                else
                {
                    from.BeginTarget(10, true, Server.Targeting.TargetFlags.None, (m, targeted) =>
                    {
                        if (targeted is IPoint3D)
                        {
                            Point3D p = new Point3D((IPoint3D)targeted);
                            int dist = (int)from.GetDistanceToSqrt(p);

                            if (dist < 2 || dist > 5)
                                from.SendLocalizedMessage(1152736); // You must stand between 2 and 5 tiles away from the targeted location to attempt to build this.
                            else if (!from.InLOS(p))
                                from.SendLocalizedMessage(500237); // Target cannot be seen.
                            else if (!ValidateLocation(p, from.Map))
                                from.SendLocalizedMessage(1152735); // The targeted location has at least one impassable tile adjacent to the structure.
                            else
                            {
                                BaseHouse checkHouse = BaseHouse.FindHouseAt(from);

                                if (checkHouse != null)
                                    from.SendLocalizedMessage(500269); // You cannot build that there.
                                else
                                {
                                    IPoint3D point = (IPoint3D)targeted;
                                    Server.Spells.SpellHelper.GetSurfaceTop(ref point);

                                    BaseAddon addon = this.Addon;
                                    addon.MoveToWorld(new Point3D(point), m.Map);

                                    if (addon is TemporaryForge)
                                        ((TemporaryForge)addon).Owner = from;

                                    this.Delete();
                                }
                            }
                        }
                    });
                }
            }
        }

        private bool ValidateLocation(Point3D p, Map map)
        {
            if (!TreasureMap.ValidateLocation(p.X, p.Y, map))
                return false;

            for (int x = p.X - 1; x <= p.X + 1; x++)
            {
                for (int y = p.Y - 1; y <= p.Y + 1; y++)
                {
                    if(TreasureMap.ValidateLocation(x, y, map))
                    {
                        int z = map.GetAverageZ(x, y);
                        IPooledEnumerable eable = map.GetItemsInRange(new Point3D(x, y, z), 0);
                        foreach (Item item in eable)
                        {
                            ItemData id = TileData.ItemTable[item.ItemID & TileData.MaxItemValue];

                            if (item.Z + id.CalcHeight >= z)
                            {
                                eable.Free();
                                return false;
                            }
                        }
                        eable.Free();

                        return true;
                    }
                }
            }

            return false;
        }

        public TemporaryForgeDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(Expires);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Expires = reader.ReadDateTime();

            if (Expires < DateTime.UtcNow)
                Decay();
            else
                m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10), CheckDecay);
        }
    }
}