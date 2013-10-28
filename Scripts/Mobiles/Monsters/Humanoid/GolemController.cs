using System;
using Server.Items;

namespace Server.Mobiles 
{ 
    [CorpseName("a golem controller corpse")] 
    public class GolemController : BaseCreature 
    { 
        [Constructable] 
        public GolemController()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        { 
            this.Name = NameList.RandomName("golem controller");
            this.Title = "the controller";

            this.Body = 400;
            this.Hue = 0x455;

            this.AddArcane(new Robe());
            this.AddArcane(new ThighBoots());
            this.AddArcane(new LeatherGloves());
            this.AddArcane(new Cloak());

            this.SetStr(126, 150);
            this.SetDex(96, 120);
            this.SetInt(151, 175);

            this.SetHits(76, 90);

            this.SetDamage(6, 12);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 30, 40);
            this.SetResistance(ResistanceType.Fire, 25, 35);
            this.SetResistance(ResistanceType.Cold, 35, 45);
            this.SetResistance(ResistanceType.Poison, 5, 15);
            this.SetResistance(ResistanceType.Energy, 15, 25);

            this.SetSkill(SkillName.EvalInt, 95.1, 100.0);
            this.SetSkill(SkillName.Magery, 95.1, 100.0);
            this.SetSkill(SkillName.Meditation, 95.1, 100.0);
            this.SetSkill(SkillName.MagicResist, 102.5, 125.0);
            this.SetSkill(SkillName.Tactics, 65.0, 87.5);
            this.SetSkill(SkillName.Wrestling, 65.0, 87.5);

            this.Fame = 4000;
            this.Karma = -4000;

            this.VirtualArmor = 16;

            if (0.7 > Utility.RandomDouble())
                this.PackItem(new ArcaneGem());
        }

        public GolemController(Serial serial)
            : base(serial)
        { 
        }

        public override bool ClickTitle
        {
            get
            {
                return false;
            }
        }
        public override bool ShowFameTitle
        {
            get
            {
                return false;
            }
        }
        public override bool AlwaysMurderer
        {
            get
            {
                return true;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Rich);
        }

        public void AddArcane(Item item)
        {
            if (item is IArcaneEquip)
            {
                IArcaneEquip eq = (IArcaneEquip)item;
                eq.CurArcaneCharges = eq.MaxArcaneCharges = 20;
            }

            item.Hue = ArcaneGem.DefaultArcaneHue;
            item.LootType = LootType.Newbied;

            this.AddItem(item);
        }

        public override void Serialize(GenericWriter writer) 
        { 
            base.Serialize(writer); 
            writer.Write((int)0); 
        }

        public override void Deserialize(GenericReader reader) 
        { 
            base.Deserialize(reader); 
            int version = reader.ReadInt(); 
        }
    }
}