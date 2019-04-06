using System;

using Server.Mobiles;
using Server.Engines.Quests;
using Server.Spells;

namespace Server.Items
{
    public class DreamSerpentScale : BaseDecayingItem
    {
        public override int LabelNumber { get { return 1151167; } } // Dream Serpent Scales
        public override int LifeSpan { get { return 86400; } }

        public DreamSerpentScale()
            : base(0x1F13)
        {
            Hue = 2069;
        }

        public override bool DropToWorld(Mobile from, Point3D p)
        {
            Delete();

            from.SendLocalizedMessage(500461); // You destroy the item.

            return true;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1075269); // Destroyed when dropped
        }

        public DreamSerpentCharm(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenaricReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt(); // version
        }
    }

    public class DreamSerpentCharm : BaseDecayingItem
    {
        public override int LabelNumber { get { return 1151187; } } // Dream Serpent Charm
        public override int LifeSpan { get { return 86400; } }

        public DreamSerpentCharm()
            : base(6463)
        {
            Hue = 91;
        }

        private static Rectangle2D WarpBounds = new Rectangle2D(659, 3815, 6, 8);
        private static TimeSpan WarpTime = TimeSpan.FromSeconds(30);
        private static TimeSpan Cooldown = TimeSpan.FromMinutes(1);

        private DateTime _NextUse;
        private Timer _Timer;
        // move, 30 seconds then get booted out back to quest giver brxit. 2 minute cooldown once used
        // must be close to the dude
        // mopnsters aggroed go in as well
        // when you hit him 6 times, you get booted out. dbl click to end quest.
        public override void OnDoubleClick(Mobile m)
        {
            if (_NextUse > DateTime.UtcNow)
            {
                m.SendLocalizedMessage(1072529, (int)(_NextUse - DateTime.UtcNow).TotalSeconds, "seconds");
            }
            else
            {
                var quest = QuestHelper.GetQuest<CatchMeIfYouCanQuest>();

                if (quest != null && SpellHelper.CheckCanTravel(m) && _Timer != null && WarpBounds.Contains(m.X, m.Y))
                {
                    TeleportTo(m);
                }
            }
        }

        public void TeleportTo(Mobile m)
        {
            m.MoveToWorld(new Point3D(403, 3391, 38), Map.TerMur);
            m.FixedParticles(0x373A, 10, 15, 5018, EffectLayer.Waist);

            _Timer = Timer.DelayCall(WarpTime, () =>
            {
                TeleportFrom(m, this, false);
            });
        }

        public static void CompleteQuest(Mobile m)
        {
            DreamSerpentCharm charm = m.Backpack.FindItemByType<DreamSerpentCharm>();

            TeleportFrom(m, charm, true);
        }

        public static void TeleportFrom(Mobile m, DreamSerpentCharm charm, bool completeQuest)
        {
            m.MoveToWorld(new Point3D(662, 3819, -43), Map.TerMur);
            m.FixedParticles(0x373A, 10, 15, 5018, EffectLayer.Waist);

            if (charm._Timer != null)
            {
                charm._Timer.Stop();
                charm._Timer = null;
            }

            charm._NextUse = DateTime.UtcNow + Cooldown;

            if (!completeQuest)
            {
                var quest = QuestHelper.GetQuest<CatchMeIfYouCanQuest>((PlayerMobile)from);

                quest.Objectives[0].CurProgress = 0;
            }
        }

        public DreamSerpentCharm(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenaricReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt(); // version
        }
    }
}
