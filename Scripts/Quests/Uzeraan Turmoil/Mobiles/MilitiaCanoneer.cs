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
            : base("the Militia Canoneer")
        {
            this.m_Active = true;
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
                return this.m_Active;
            }
            set
            {
                this.m_Active = value;
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 125, 25);

            this.Hue = Utility.RandomSkinHue();

            this.Female = false;
            this.Body = 0x190;
            this.Name = NameList.RandomName("male");
        }

        public override void InitOutfit()
        {
            Utility.AssignRandomHair(this);
            Utility.AssignRandomFacialHair(this, this.HairHue);

            this.AddItem(new PlateChest());
            this.AddItem(new PlateArms());
            this.AddItem(new PlateGloves());
            this.AddItem(new PlateLegs());

            Torch torch = new Torch();
            torch.Movable = false;
            this.AddItem(torch);
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
                    return this.IsEnemy(master);
            }

            return m.Karma < 0;
        }

        public bool WillFire(Cannon cannon, Mobile target)
        {
            if (this.m_Active && this.IsEnemy(target))
            {
                this.Direction = this.GetDirectionTo(target);
                this.Say(Utility.RandomList(500651, 1049098, 1049320, 1043149));
                return true;
            }

            return false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write((bool)this.m_Active);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            this.m_Active = reader.ReadBool();
        }
    }
}