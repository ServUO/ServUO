using System;
using Server.Items;

namespace Server.Mobiles
{
    public class EscortableMage : BaseEscortable
    {
        [Constructable]
        public EscortableMage()
        {
            this.Title = "the mage";

            this.SetSkill(SkillName.EvalInt, 80.0, 100.0);
            this.SetSkill(SkillName.Inscribe, 80.0, 100.0);
            this.SetSkill(SkillName.Magery, 80.0, 100.0);
            this.SetSkill(SkillName.Meditation, 80.0, 100.0);
            this.SetSkill(SkillName.MagicResist, 80.0, 100.0);
        }

        public EscortableMage(Serial serial)
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
        }// Do not display 'the mage' when single-clicking
        public override void InitOutfit()
        {
            this.AddItem(new Robe(GetRandomHue()));

            int lowHue = GetRandomHue();

            this.AddItem(new ShortPants(lowHue));

            if (this.Female)
                this.AddItem(new ThighBoots(lowHue));
            else
                this.AddItem(new Boots(lowHue));

            Utility.AssignRandomHair(this);

            this.PackGold(200, 250);
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

        private static int GetRandomHue()
        {
            switch ( Utility.Random(5) )
            {
                default:
                case 0:
                    return Utility.RandomBlueHue();
                case 1:
                    return Utility.RandomGreenHue();
                case 2:
                    return Utility.RandomRedHue();
                case 3:
                    return Utility.RandomYellowHue();
                case 4:
                    return Utility.RandomNeutralHue();
            }
        }
    }
}