using System;
using Server.Items;

namespace Server.Mobiles
{
    public class GargishWanderingHealer : BaseHealer
    {
        [Constructable]
        public GargishWanderingHealer()
        {
            this.Title = "a Gargish wandering healer";
            this.Body = 666;

            this.AddItem(new GnarledStaff());
            //AddItem( new GargishRobe(5) );

            this.SetSkill(SkillName.Camping, 80.0, 100.0);
            this.SetSkill(SkillName.Forensics, 80.0, 100.0);
            this.SetSkill(SkillName.SpiritSpeak, 80.0, 100.0);
        }

        public GargishWanderingHealer(Serial serial)
            : base(serial)
        {
        }

        public override bool CanTeach
        {
            get
            {
                return true;
            }
        }
        public override bool ClickTitle
        {
            get
            {
                return false;
            }
        }// Do not display title in OnSingleClick
        public override bool CheckTeach(SkillName skill, Mobile from)
        {
            if (!base.CheckTeach(skill, from))
                return false;

            return (skill == SkillName.Anatomy) ||
                   (skill == SkillName.Camping) ||
                   (skill == SkillName.Forensics) ||
                   (skill == SkillName.Healing) ||
                   (skill == SkillName.SpiritSpeak) ||
                   (skill == SkillName.Mysticism);
        }

        public override bool CheckResurrect(Mobile m)
        {
            if (m.Criminal)
            {
                this.Say(501222); // Thou art a criminal.  I shall not resurrect thee.
                return false;
            }
            else if (m.Kills >= 5)
            {
                this.Say(501223); // Thou'rt not a decent and good person. I shall not resurrect thee.
                return false;
            }

            return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}