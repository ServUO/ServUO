using System;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Engines.Quests.Doom
{
    public class BellOfTheDead : Item
    {
        private Chyloth m_Chyloth;
        private SkeletalDragon m_Dragon;
        private bool m_Summoning;
        [Constructable]
        public BellOfTheDead()
            : base(0x91A)
        {
            this.Hue = 0x835;
            this.Movable = false;
        }

        public BellOfTheDead(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1050018;
            }
        }// bell of the dead
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.Administrator)]
        public Chyloth Chyloth
        {
            get
            {
                return this.m_Chyloth;
            }
            set
            {
                this.m_Chyloth = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.Administrator)]
        public SkeletalDragon Dragon
        {
            get
            {
                return this.m_Dragon;
            }
            set
            {
                this.m_Dragon = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.Administrator)]
        public bool Summoning
        {
            get
            {
                return this.m_Summoning;
            }
            set
            {
                this.m_Summoning = value;
            }
        }
        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(this.GetWorldLocation(), 2))
                this.BeginSummon(from);
            else
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
        }

        public virtual void BeginSummon(Mobile from)
        {
            if (this.m_Chyloth != null && !this.m_Chyloth.Deleted)
            {
                from.SendLocalizedMessage(1050010); // The ferry man has already been summoned.  There is no need to ring for him again.
            }
            else if (this.m_Dragon != null && !this.m_Dragon.Deleted)
            {
                from.SendLocalizedMessage(1050017); // The ferryman has recently been summoned already.  You decide against ringing the bell again so soon.
            }
            else if (!this.m_Summoning)
            {
                this.m_Summoning = true;

                Effects.PlaySound(this.GetWorldLocation(), this.Map, 0x100);

                Timer.DelayCall(TimeSpan.FromSeconds(8.0), new TimerStateCallback(EndSummon), from);
            }
        }

        public virtual void EndSummon(object state)
        {
            Mobile from = (Mobile)state;

            if (this.m_Chyloth != null && !this.m_Chyloth.Deleted)
            {
                from.SendLocalizedMessage(1050010); // The ferry man has already been summoned.  There is no need to ring for him again.
            }
            else if (this.m_Dragon != null && !this.m_Dragon.Deleted)
            {
                from.SendLocalizedMessage(1050017); // The ferryman has recently been summoned already.  You decide against ringing the bell again so soon.
            }
            else if (this.m_Summoning)
            {
                this.m_Summoning = false;

                Point3D loc = this.GetWorldLocation();

                loc.Z -= 16;

                Effects.SendLocationParticles(EffectItem.Create(loc, this.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 0, 0, 2023, 0);
                Effects.PlaySound(loc, this.Map, 0x1FE);

                this.m_Chyloth = new Chyloth();

                this.m_Chyloth.Direction = (Direction)(7 & (4 + (int)from.GetDirectionTo(loc)));
                this.m_Chyloth.MoveToWorld(loc, this.Map);

                this.m_Chyloth.Bell = this;
                this.m_Chyloth.AngryAt = from;
                this.m_Chyloth.BeginGiveWarning();
                this.m_Chyloth.BeginRemove(TimeSpan.FromSeconds(40.0));
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write((Mobile)this.m_Chyloth);
            writer.Write((Mobile)this.m_Dragon);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            this.m_Chyloth = reader.ReadMobile() as Chyloth;
            this.m_Dragon = reader.ReadMobile() as SkeletalDragon;

            if (this.m_Chyloth != null)
                this.m_Chyloth.Delete();
        }
    }
}