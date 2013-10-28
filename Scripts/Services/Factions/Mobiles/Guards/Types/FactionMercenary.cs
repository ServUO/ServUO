using System;
using Server.Items;

namespace Server.Factions
{
    public class FactionMercenary : BaseFactionGuard
    {
        [Constructable]
        public FactionMercenary()
            : base("the mercenary")
        {
            this.GenerateBody(false, true);

            this.SetStr(116, 125);
            this.SetDex(61, 85);
            this.SetInt(81, 95);

            this.SetResistance(ResistanceType.Physical, 20, 40);
            this.SetResistance(ResistanceType.Fire, 20, 40);
            this.SetResistance(ResistanceType.Cold, 20, 40);
            this.SetResistance(ResistanceType.Energy, 20, 40);
            this.SetResistance(ResistanceType.Poison, 20, 40);

            this.VirtualArmor = 16;

            this.SetSkill(SkillName.Fencing, 90.0, 100.0);
            this.SetSkill(SkillName.Wrestling, 90.0, 100.0);
            this.SetSkill(SkillName.Tactics, 90.0, 100.0);
            this.SetSkill(SkillName.MagicResist, 90.0, 100.0);
            this.SetSkill(SkillName.Healing, 90.0, 100.0);
            this.SetSkill(SkillName.Anatomy, 90.0, 100.0);

            this.AddItem(new ChainChest());
            this.AddItem(new ChainLegs());
            this.AddItem(new RingmailArms());
            this.AddItem(new RingmailGloves());
            this.AddItem(new ChainCoif());
            this.AddItem(new Boots());
            this.AddItem(this.Newbied(new ShortSpear()));

            this.PackItem(new Bandage(Utility.RandomMinMax(20, 30)));
            this.PackStrongPotions(3, 8);
        }

        public FactionMercenary(Serial serial)
            : base(serial)
        {
        }

        public override GuardAI GuardAI
        {
            get
            {
                return GuardAI.Melee | GuardAI.Smart;
            }
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