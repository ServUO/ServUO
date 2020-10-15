using Server.Engines.Quests;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using System;

namespace Server.Items
{
    public class ClockworkMechanism : BaseDecayingItem
    {
        public override bool HiddenQuestItemHue => true;
        public override int Lifespan => 3600;
        public override bool UseSeconds => false;

        private int m_CreatureDef;

        public ClockworkCreatureDef CreatureDef => ClockworkCreature.Definitions[m_CreatureDef];

        [Constructable]
        public ClockworkMechanism()
            : base(0x1EAE)
        {
            m_CreatureDef = Utility.Random(ClockworkCreature.Definitions.Length);

            Weight = 1.0;
            Hue = 0x450;
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            list.Add(1112858, string.Format("#{0}", ((int)CreatureDef.CreatureType).ToString())); // ~1_TYPE~ clockwork mechanism
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1072351); // Quest Item
        }

        public override bool DropToWorld(Mobile from, Point3D p)
        {
            return false;
        }

        public override bool DropToItem(Mobile from, Item target, Point3D p)
        {
            if (from.Backpack == target)
            {
                base.DropToItem(from, target, p);
                return true;
            }

            return false;
        }

        public override bool DropToMobile(Mobile from, Mobile target, Point3D p)
        {
            if (from == target)
            {
                base.DropToMobile(from, target, p);
                return true;
            }

            return false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (MadScientistQuest.QuestStarted(from))
            {
                MadScientistQuest.BarkIngredient(from);
            }
            else
            {
                if (!from.HasGump(typeof(BeginQuestGump)))
                {
                    from.SendGump(new BeginQuestGump(this));
                }
            }
        }

        public void OnCompleted(Mobile from)
        {
            Mobile creature = new ClockworkCreature(CreatureDef);
            Point3D p = from.Location;

            creature.MoveToWorld(p, from.Map);

            Timer.DelayCall(TimeSpan.FromSeconds(5.0), delegate
            {
                from.PlaySound(0xFA);
                from.PlaySound(0x5BC);
                from.PlaySound(0x5C7);

                Effects.SendLocationEffect(p, from.Map, 0x1FD4, 30, 16, 0x21, 4);

                for (int j = 0; j < 5; j++)
                {
                    Point3D loc = new Point3D(p.X, p.Y, 10 + p.Z + (j * 20));

                    Effects.SendLocationEffect(loc, from.Map, 0x1AA1, 17, 16, 0x481, 4);
                    Effects.SendLocationEffect(loc, from.Map, 0x1A9F, 10, 16, 0x481, 4);
                    Effects.SendLocationEffect(loc, from.Map, 0x1A8, 25, 16, 0x47E, 4);
                }

                from.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1112987, from.NetState); // The training clockwork fails and the creature vanishes.

                Timer.DelayCall(TimeSpan.FromSeconds(1.0), delegate
                {
                    creature.Delete();
                });
            });
        }

        public ClockworkMechanism(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1); // version

            writer.Write(m_CreatureDef);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version > 0)
                m_CreatureDef = reader.ReadInt();
        }

        public class BeginQuestGump : Gump
        {
            private readonly ClockworkMechanism m_Mechanism;

            public BeginQuestGump(ClockworkMechanism mechanism)
                : base(340, 340)
            {
                m_Mechanism = mechanism;

                AddPage(0);

                AddBackground(0, 0, 291, 99, 0x13BE);
                AddImageTiled(5, 6, 280, 20, 0xA40);

                AddHtmlLocalized(9, 8, 280, 20, 1112855, 0x7FFF, false, false); // Begin Mad Scientist Quest

                AddImageTiled(5, 31, 280, 40, 0xA40);

                AddHtmlLocalized(9, 35, 272, 40, 1112856, 0x7FFF, false, false); // You have a limited amount of time to complete the recipe. Start now?

                AddButton(215, 73, 0xFB7, 0xFB8, 1, GumpButtonType.Reply, 0);
                AddHtmlLocalized(250, 75, 65, 20, 1006044, 0x7FFF, false, false); // OK

                AddButton(5, 73, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0);
                AddHtmlLocalized(40, 75, 100, 20, 1060051, 0x7FFF, false, false); // CANCEL
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                Mobile from = sender.Mobile;

                if (info.ButtonID == 1)
                    MadScientistQuest.StartQuest(from, m_Mechanism);
            }
        }
    }
}
