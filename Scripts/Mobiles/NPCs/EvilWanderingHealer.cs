using Server.Items;

namespace Server.Mobiles
{
    public class EvilWanderingHealer : BaseHealer
    {
        [Constructable]
        public EvilWanderingHealer()
        {
            Title = "the Priest Of Mondain";
            Karma = -10000;

            AddItem(new GnarledStaff());

            SetSkill(SkillName.Camping, 80.0, 100.0);
            SetSkill(SkillName.Forensics, 80.0, 100.0);
            SetSkill(SkillName.SpiritSpeak, 80.0, 100.0);
        }

        public EvilWanderingHealer(Serial serial)
            : base(serial)
        {
        }

        public override bool CanTeach => true;
        public override bool AlwaysMurderer => true;
        public override bool ClickTitle => false;// Do not display title in OnSingleClick
        public override bool CheckTeach(SkillName skill, Mobile from)
        {
            if (!base.CheckTeach(skill, from))
                return false;

            return (skill == SkillName.Anatomy) ||
                   (skill == SkillName.Camping) ||
                   (skill == SkillName.Forensics) ||
                   (skill == SkillName.Healing) ||
                   (skill == SkillName.SpiritSpeak);
        }

        public override bool CheckResurrect(Mobile m)
        {
            if (m.Criminal)
            {
                Say(501222); // Thou art a criminal.  I shall not resurrect thee.
                return false;
            }

            return true;
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() <= 0.25)
                c.AddItem(Loot.Construct(typeof(MapFragment)));
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}
