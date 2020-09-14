using Server.Engines.Harvest;
using System;

namespace Server.Items
{
    public class WoodsmansTalisman : BaseTalisman, IUsesRemaining
    {
        public const int DecayPeriod = 24;

        private CraftResource _Resource;
        private int _UsesRemaining;
        private Timer m_Timer;

        [CommandProperty(AccessLevel.GameMaster)]
        public CraftResource Resource
        {
            get { return _Resource; }
            set
            {
                if (_Resource != value)
                {
                    _Resource = value;
                    Hue = CraftResources.GetHue(_Resource);
                    InvalidateProperties();
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int UsesRemaining
        {
            get { return _UsesRemaining; }
            set
            {
                _UsesRemaining = value;

                if (_UsesRemaining <= 0 && RootParent is Mobile)
                    ((Mobile)RootParent).SendLocalizedMessage(1152621); // Your talisman's magic is exhausted.

                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool ShowUsesRemaining { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime Expires { get; set; }

        [Constructable]
        public WoodsmansTalisman(CraftResource resource)
            : base(0x2F5A)
        {
            Resource = resource;

            UsesRemaining = 300;
            ShowUsesRemaining = true;

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
                Effects.SendLocationParticles(EffectItem.Create(Location, Map, EffectItem.DefaultDuration), 0x3728, 8, 20, 5042);
                Effects.PlaySound(Location, Map, 0x201);
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

        public static int CheckHarvest(Mobile from, Type type, HarvestSystem system)
        {
            if (system != Lumberjacking.System)
                return 0;

            CraftResource res = CraftResources.GetFromType(type);
            WoodsmansTalisman talisman = from.FindItemOnLayer(Layer.Talisman) as WoodsmansTalisman;

            if (talisman != null && talisman.UsesRemaining > 0 && res == talisman.Resource)
            {
                talisman.UsesRemaining--;
                from.SendLocalizedMessage(1152625); // The magic of your talisman guides your hands as you cut the wood.
                return Utility.RandomMinMax(2, 5);
            }

            return 0;
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            list.Add(1152599, string.Format("#{0}\t#1152605", CraftResources.GetLocalizationNumber(_Resource))); // ~1_RES~ ~2_TYPE~ Talisman
        }

        public override void AddUsesRemainingProperties(ObjectPropertyList list)
        {
            list.Add(1060584, UsesRemaining.ToString()); // uses remaining: ~1_val~
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            int left = 0;
            if (DateTime.UtcNow < Expires)
                left = (int)(Expires - DateTime.UtcNow).TotalSeconds;

            list.Add(1072517, left.ToString()); // Lifespan: ~1_val~ seconds
        }

        public WoodsmansTalisman(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write((int)Resource);
            writer.Write(Expires);
            writer.Write(UsesRemaining);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Resource = (CraftResource)reader.ReadInt();
            Expires = reader.ReadDateTime();
            UsesRemaining = reader.ReadInt();

            if (Expires < DateTime.UtcNow)
                Decay();
            else
                m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10), CheckDecay);
        }
    }
}
