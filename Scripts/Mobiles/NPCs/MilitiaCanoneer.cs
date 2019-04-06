using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests.Haven
{
    public class MilitiaCanoneer : BaseQuester
    {
        private bool m_Active;
        [Constructable]
        public MilitiaCanoneer()
            : base("the Militia Cannoneer")
        {
            m_Active = true;
        }

        public MilitiaCanoneer(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Active
        {
            get
            {
                return m_Active;
            }
            set
            {
                m_Active = value;
            }
        }
        public override void InitBody()
        {
            InitStats(100, 125, 25);

            Hue = Utility.RandomSkinHue();

            Female = false;
            Body = 0x190;
            Name = NameList.RandomName("male");
        }

        public override void InitOutfit()
        {
            Utility.AssignRandomHair(this);
            Utility.AssignRandomFacialHair(this, HairHue);

            AddItem(new PlateChest());
            AddItem(new PlateArms());
            AddItem(new PlateGloves());
            AddItem(new PlateLegs());

            Torch torch = new Torch();
            torch.Movable = false;
            AddItem(torch);
            torch.Ignite();
        }

        public override bool CanTalkTo(PlayerMobile to)
        {
            return false;
        }

        public override void OnTalk(PlayerMobile player, bool contextMenu)
        {
        }

        public override bool IsEnemy(Mobile m)
        {
            if (m.Player || m is BaseVendor)
                return false;

            if (m is BaseCreature)
            {
                BaseCreature bc = (BaseCreature)m;

                Mobile master = bc.GetMaster();
                if (master != null)
                    return IsEnemy(master);
            }

            return m.Karma < 0;
        }

        public bool WillFire(Cannon cannon, Mobile target)
        {
            if (m_Active && IsEnemy(target))
            {
                Direction = GetDirectionTo(target);
                Say(Utility.RandomList(500651, 1049098, 1049320, 1043149));
                return true;
            }

            return false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write((bool)m_Active);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_Active = reader.ReadBool();
        }
    }
}
