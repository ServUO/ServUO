using Server.Items;
using Server.Mobiles;
using Server.Spells;
using System;

namespace Server.Engines.Quests.RitualQuest
{
    public class DreamSerpentScale : BaseDecayingItem
    {
        public override int LabelNumber => 1151167;  // Dream Serpent Scales
        public override int Lifespan => 86400;
        public override bool HiddenQuestItemHue => true;

        public DreamSerpentScale()
            : base(0x1F13)
        {
            Hue = 2069;
            QuestItem = true;
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

        public DreamSerpentScale(Serial serial) : base(serial)
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
            reader.ReadInt(); // version
        }
    }

    public class DreamSerpentCharm : BaseDecayingItem
    {
        public override int LabelNumber => 1151187;  // Dream Serpent Charm
        public override int Lifespan => 86400;

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

        public override void OnDoubleClick(Mobile m)
        {
            if (_NextUse > DateTime.UtcNow)
            {
                m.SendLocalizedMessage(1072529, string.Format("{0}\t{1}", ((int)(_NextUse - DateTime.UtcNow).TotalSeconds).ToString(), "seconds"));
            }
            else if (m is PlayerMobile)
            {
                CatchMeIfYouCanQuest quest = QuestHelper.GetQuest<CatchMeIfYouCanQuest>((PlayerMobile)m);

                if (quest != null && SpellHelper.CheckCanTravel(m) && _Timer == null && WarpBounds.Contains(m.Location))
                {
                    TeleportTo(m);
                }
            }
        }

        public void TeleportTo(Mobile m)
        {
            BaseCreature.TeleportPets(m, new Point3D(403, 3391, 38), Map.TerMur);
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
            BaseCreature.TeleportPets(m, new Point3D(662, 3819, -43), Map.TerMur);
            m.MoveToWorld(new Point3D(662, 3819, -43), Map.TerMur);
            m.FixedParticles(0x373A, 10, 15, 5018, EffectLayer.Waist);

            if (charm._Timer != null)
            {
                charm._Timer.Stop();
                charm._Timer = null;
            }

            charm._NextUse = DateTime.UtcNow + Cooldown;

            if (!completeQuest && m is PlayerMobile)
            {
                CatchMeIfYouCanQuest quest = QuestHelper.GetQuest<CatchMeIfYouCanQuest>((PlayerMobile)m);

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

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt(); // version
        }
    }

    public class SoulbinderTear : BaseDecayingItem
    {
        public override int LabelNumber => 1151170;  // Soulbinder's Tears
        public override int Lifespan => 86400;
        public override bool HiddenQuestItemHue => true;

        public SoulbinderTear()
            : base(0xE2A)
        {
            Hue = 2076;
            QuestItem = true;
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

        public SoulbinderTear(Serial serial) : base(serial)
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
            reader.ReadInt(); // version
        }
    }

    public class PristineCrystalLotus : BaseDecayingItem
    {
        public override int LabelNumber => 1151169;  // Pristine Crystal Lotus
        public override int Lifespan => 86400;
        public override bool HiddenQuestItemHue => true;

        public PristineCrystalLotus()
            : base(0x283B)
        {
            Hue = 1152;
            QuestItem = true;
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

        public PristineCrystalLotus(Serial serial) : base(serial)
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
            reader.ReadInt(); // version
        }
    }

    public class ChronicleOfTheGargoyleQueen2 : GargishDocumentBook
    {
        public static void Initialize()
        {
            for (int i = 0; i < 42; i++)
            {
                m_Contents[i] = 1150976 + i;
            }
        }

        private static readonly int[] m_Contents = new int[42];

        public override object Title => 1151164;  // Chronicle of the Gargoyle Queen Vol. II
        public override object Author => "Queen Zhah";
        public override int[] Contents => m_Contents;

        [Constructable]
        public ChronicleOfTheGargoyleQueen2()
        {
            Hue = 573;
        }

        public ChronicleOfTheGargoyleQueen2(Serial serial)
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
            int v = reader.ReadInt();
        }
    }

    public class TerMurSnowglobe : Item
    {
        public override int LabelNumber => 1151172;  // Ter Mur Snowglobe

        public TerMurSnowglobe()
            : base(0xE2F)
        {
            Light = LightType.Circle150;
            Hue = 2599;
        }

        public TerMurSnowglobe(Serial serial) : base(serial)
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
            reader.ReadInt(); // version
        }
    }
}
