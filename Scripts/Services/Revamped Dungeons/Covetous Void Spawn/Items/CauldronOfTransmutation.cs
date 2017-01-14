using Server;
using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Targeting;
using Server.Multis;

namespace Server.Items
{
    public class CauldronOfTransmutation : BaseAddon
    {
        public const int DecayPeriod = 4;
        private int _Charges;

        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges
        {
            get { return _Charges; }
            set
            {
                _Charges = value;

                if (_Charges <= 0 && this.RootParent is Mobile)
                    ((Mobile)RootParent).SendLocalizedMessage(1152635); // The cauldron's magic is exhausted

                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime Expires { get; set; }

        public Timer DecayTimer { get; private set; }

        public override bool RetainDeedHue { get { return true; } }

        public CauldronOfTransmutation()
        {
            Charges = 5000;

            AddComponent(new InternalComponent(2421), 0, 0, 0);

            Expires = DateTime.UtcNow + TimeSpan.FromHours(DecayPeriod);
            BeginTimer();
        }

        public void BeginTimer()
        {
            EndTimer();

            DecayTimer = Timer.DelayCall(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10), CheckDecay);
            DecayTimer.Start();
        }

        public void EndTimer()
        {
            if (DecayTimer != null)
            {
                DecayTimer.Stop();
                DecayTimer = null;
            }
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

            EndTimer();
        }

        public override void OnComponentUsed(AddonComponent c, Mobile from)
        {
            if (from.InRange(c.Location, 3))
            {
                BaseHouse house = BaseHouse.FindHouseAt(from);

                if (house != null && house.IsCoOwner(from))
                {
                    from.Target = new InternalTarget(this);
                    from.SendLocalizedMessage(1152738); // Target ingots in your backpack to transmute them.
                }
            }
        }

        public bool TryTransmutate(Mobile from, Item dropped)
        {
            BaseHouse house = BaseHouse.FindHouseAt(from);

            if (house != null && house.IsOwner(from))
            {
                CraftResource res = CraftResources.GetFromType(dropped.GetType());

                if (res == Resource)
                {
                    if (dropped.Amount < 3)
                        from.SendLocalizedMessage(1152634); // There is not enough to transmute
                    else if (Charges <= 0)
                        from.SendLocalizedMessage(1152635); // The cauldron's magic is exhausted
                    else
                    {
                        CraftResourceInfo info = CraftResources.GetInfo(Resource + 1);

                        if (info != null && info.ResourceTypes.Length > 0)
                        {
                            int toDrop = Math.Min(Charges * 3, dropped.Amount);
                            CraftResource newRes = (CraftResource)(int)res + 1;

                            while (toDrop % 3 != 0)
                                toDrop--;

                            int newAmount = toDrop / 3;

                            if (toDrop < dropped.Amount)
                                dropped.Amount -= toDrop;
                            else
                                dropped.Delete();

                            Item item = Loot.Construct(info.ResourceTypes[0]);

                            if (item != null)
                            {
                                item.Amount = newAmount;
                                from.AddToBackpack(item);

                                from.SendLocalizedMessage(1152636); // The cauldron transmutes the material

                                from.PlaySound(Utility.RandomList(0x22, 0x23));

                                Charges -= newAmount;
                                Components.ForEach(c => c.InvalidateProperties());

                                return true;
                            }
                        }
                        else
                            from.SendLocalizedMessage(1152633); // The cauldron cannot transmute that
                    }
                }

            }
            else
                from.SendLocalizedMessage(1152632); // That is not yours!

            return false;
        }

        private class InternalTarget : Target
        {
            public CauldronOfTransmutation Addon { get; set; }

            public InternalTarget(CauldronOfTransmutation addon)
                : base(-1, false, TargetFlags.None)
            {
                Addon = addon;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (Addon == null || !from.InRange(Addon.Location, 3))
                    from.SendLocalizedMessage(500295); // You are too far away to do that.

                if (targeted is Item)
                    Addon.TryTransmutate(from, (Item)targeted);
            }
        }

        private class InternalComponent : AddonComponent
        {
            public override bool ForceShowProperties { get { return true; } }

            public InternalComponent(int id)
                : base(id)
            {
            }

            public override void AddNameProperty(ObjectPropertyList list)
            {
                if (Addon != null)
                    list.Add(1152600, String.Format("#{0}", CraftResources.GetLocalizationNumber(Addon.Resource))); // ~1_RES~ Cauldron of Transmutation
                else
                    base.AddNameProperty(list);
            }

            public override void GetProperties(ObjectPropertyList list)
            {
                base.GetProperties(list);

                if (Addon == null || !(Addon is CauldronOfTransmutation))
                    return;

                int left = 0;
                if (DateTime.UtcNow < ((CauldronOfTransmutation)Addon).Expires)
                    left = (int)(((CauldronOfTransmutation)Addon).Expires - DateTime.UtcNow).TotalSeconds;

                list.Add(1072517, left.ToString()); // Lifespan: ~1_val~ seconds

                CraftResource res = Addon.Resource;
                CraftResource res2 = (CraftResource)(int)res + 1;

                list.Add(1152630, String.Format("#{0}\t#{1}", CraftResources.GetLocalizationNumber(Addon.Resource), CraftResources.GetLocalizationNumber(res2))); // transmutes ~1_SOURCE~ to ~2_DEST~
                list.Add(1152631, String.Format("3\t1")); // ratio ~1_INPUT~ to ~2_OUTPUT~
                list.Add(1060584, ((CauldronOfTransmutation)Addon).Charges.ToString()); // uses remaining: ~1_val~

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

        public CauldronOfTransmutation(Serial serial)
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
                BeginTimer();
        }
    }

    public class CauldronOfTransmutationDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new CauldronOfTransmutation(); } }

        public const int DecayPeriod = 24;

        private Timer m_Timer;

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime Expires { get; set; }

        public CauldronOfTransmutationDeed(CraftResource resource)
        {
            Resource = resource;

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
            list.Add(1152600, String.Format("#{0}", CraftResources.GetLocalizationNumber(Resource))); // ~1_RES~ Cauldron of Transmutation
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            int left = 0;
            if (DateTime.UtcNow < Expires)
                left = (int)(Expires - DateTime.UtcNow).TotalSeconds;

            list.Add(1072517, left.ToString()); // Lifespan: ~1_val~ seconds
            list.Add(1152628); // cannot re-deed
            list.Add(1152629, CauldronOfTransmutation.DecayPeriod.ToString()); // lasts ~1_count~ hours
        }

        public CauldronOfTransmutationDeed(Serial serial)
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