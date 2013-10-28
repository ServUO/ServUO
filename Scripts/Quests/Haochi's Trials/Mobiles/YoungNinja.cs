using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests.Samurai
{
    [CorpseName("a young ninja's corpse")]
    public class YoungNinja : BaseCreature
    {
        [Constructable]
        public YoungNinja()
            : base(AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            this.InitStats(45, 30, 5);
            this.SetHits(20, 30);

            this.Hue = Utility.RandomSkinHue();
            this.Body = 0x190;
            this.Name = "a young ninja";

            Utility.AssignRandomHair(this);
            Utility.AssignRandomFacialHair(this);

            this.AddItem(new NinjaTabi());
            this.AddItem(new LeatherNinjaPants());
            this.AddItem(new LeatherNinjaJacket());
            this.AddItem(new LeatherNinjaBelt());

            this.AddItem(new Bandana(Utility.RandomNondyedHue()));

            switch ( Utility.Random(3) )
            {
                case 0:
                    this.AddItem(new Tessen());
                    break;
                case 1:
                    this.AddItem(new Kama());
                    break;
                default:
                    this.AddItem(new Lajatang());
                    break;
            }

            this.SetSkill(SkillName.Swords, 50.0);
            this.SetSkill(SkillName.Tactics, 50.0);
        }

        public YoungNinja(Serial serial)
            : base(serial)
        {
        }

        public override bool AlwaysMurderer
        {
            get
            {
                return true;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}