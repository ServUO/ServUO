using System;
using Server.Mobiles;

namespace Server.Items
{
    public class StatCapScroll : SpecialScroll
    {
        public StatCapScroll()
            : this(105)
        {
        }

        [Constructable]
        public StatCapScroll(int value)
            : base(SkillName.Alchemy, value)
        {
            this.Hue = 0x481;
        }

        public StatCapScroll(Serial serial)
            : base(serial)
        {
        }

        public override int Message
        {
            get
            {
                return 1049469;
            }
        }/* Using a scroll increases the maximum amount of a specific skill or your maximum statistics.
        * When used, the effect is not immediately seen without a gain of points with that skill or statistics.
        * You can view your maximum skill values in your skills window.
        * You can view your maximum statistic value in your statistics window. */
        public override int Title 
        {
            get
            {
                int level = ((int)this.Value - 230) / 5;

                if (level >= 0 && level <= 4 && this.Value % 5 == 0)
                    return 1049458 + level;	/* Wonderous Scroll (+5 Maximum Stats): OR
                * Exalted Scroll (+10 Maximum Stats): OR
                * Mythical Scroll (+15 Maximum Stats): OR
                * Legendary Scroll (+20 Maximum Stats): OR
                * Ultimate Scroll (+25 Maximum Stats): */
				
                return 0;
            }
        }
        public override string DefaultTitle
        {
            get
            {
                return String.Format("<basefont color=#FFFFFF>Power Scroll ({0}{1} Maximum Stats):</basefont>", ((int)this.Value - 225) >= 0 ? "+" : "", (int)this.Value - 225);
            }
        }
        public override void AddNameProperty(ObjectPropertyList list)
        {
            int level = ((int)this.Value - 230) / 5;
			
            if (level >= 0 && level <= 4 && (int)this.Value % 5 == 0)
                list.Add(1049463 + level, "#1049476");	/* a wonderous scroll of ~1_type~ (+5 Maximum Stats) OR
            * an exalted scroll of ~1_type~ (+10 Maximum Stats) OR
            * a mythical scroll of ~1_type~ (+15 Maximum Stats) OR
            * a legendary scroll of ~1_type~ (+20 Maximum Stats) OR
            * an ultimate scroll of ~1_type~ (+25 Maximum Stats) */
            else
                list.Add("a scroll of power ({0}{1} Maximum Stats)", (this.Value - 225) >= 0 ? "+" : "", this.Value - 225);
        }

        public override void OnSingleClick(Mobile from)
        {
            int level = ((int)this.Value - 230) / 5;
			
            if (level >= 0 && level <= 4 && (int)this.Value % 5 == 0)
                base.LabelTo(from, 1049463 + level, "#1049476");
            else
                base.LabelTo(from, "a scroll of power ({0}{1} Maximum Stats)", (this.Value - 225) >= 0 ? "+" : "", this.Value - 225);
        }

        public override bool CanUse(Mobile from)
        {
            if (!base.CanUse(from))
                return false;
			
            int newValue = (int)this.Value;
			
            if (from is PlayerMobile && ((PlayerMobile)from).HasStatReward)
                newValue += 5;
			
            if (from.StatCap >= newValue)
            {
                from.SendLocalizedMessage(1049510); // Your stats are too high for this power scroll.
                return false;
            }
			
            return true;
        }

        public override void Use(Mobile from)
        {
            if (!this.CanUse(from))
                return;

            from.SendLocalizedMessage(1049512); // You feel a surge of magic as the scroll enhances your powers!

            if (from is PlayerMobile && ((PlayerMobile)from).HasStatReward)
                from.StatCap = (int)this.Value + 5;
            else
                from.StatCap = (int)this.Value;

            Effects.SendLocationParticles(EffectItem.Create(from.Location, from.Map, EffectItem.DefaultDuration), 0, 0, 0, 0, 0, 5060, 0);
            Effects.PlaySound(from.Location, from.Map, 0x243);

            Effects.SendMovingParticles(new Entity(Serial.Zero, new Point3D(from.X - 6, from.Y - 6, from.Z + 15), from.Map), from, 0x36D4, 7, 0, false, true, 0x497, 0, 9502, 1, 0, (EffectLayer)255, 0x100);
            Effects.SendMovingParticles(new Entity(Serial.Zero, new Point3D(from.X - 4, from.Y - 6, from.Z + 15), from.Map), from, 0x36D4, 7, 0, false, true, 0x497, 0, 9502, 1, 0, (EffectLayer)255, 0x100);
            Effects.SendMovingParticles(new Entity(Serial.Zero, new Point3D(from.X - 6, from.Y - 4, from.Z + 15), from.Map), from, 0x36D4, 7, 0, false, true, 0x497, 0, 9502, 1, 0, (EffectLayer)255, 0x100);

            Effects.SendTargetParticles(from, 0x375A, 35, 90, 0x00, 0x00, 9502, (EffectLayer)255, 0x100);

            this.Delete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = (this.InheritsItem ? 0 : reader.ReadInt()); //Required for SpecialScroll insertion

            this.LootType = LootType.Cursed;
            this.Insured = false;
        }
    }
}