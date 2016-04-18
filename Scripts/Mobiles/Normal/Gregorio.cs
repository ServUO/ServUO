using System;
using Server.Engines.Quests;
using Server.Items;

namespace Server.Mobiles
{
    public class Gregorio : BaseCreature
    { 
        [Constructable]
        public Gregorio()
            : base(AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        { 
            this.Race = Race.Human;
            this.Name = "Gregorio";
            this.Title = "the brigand";
			
            this.InitBody();
            this.InitOutfit();
			
            this.SetStr(86, 100);
            this.SetDex(81, 95);
            this.SetInt(61, 75);

            this.SetDamage(15, 27);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 10, 15);
            this.SetResistance(ResistanceType.Fire, 10, 15);
            this.SetResistance(ResistanceType.Poison, 10, 15);
            this.SetResistance(ResistanceType.Energy, 10, 15);

            this.SetSkill(SkillName.MagicResist, 25.0, 50.0);
            this.SetSkill(SkillName.Tactics, 80.0, 100.0);
            this.SetSkill(SkillName.Wrestling, 80.0, 100.0);	
			
            this.PackGold(50, 150);
        }

        public Gregorio(Serial serial)
            : base(serial)
        {
        }

        public override bool InitialInnocent
        {
            get
            {
                return true;
            }
        }
        public static bool IsMurderer(Mobile from)
        { 
            if (from != null && from is PlayerMobile)
            {
                BaseQuest quest = QuestHelper.GetQuest((PlayerMobile)from, typeof(GuiltyQuest));

                if (quest != null)
                    return !quest.Completed;
            }
				
            return false;
        }

        public virtual void InitBody()
        {
            this.InitStats(100, 100, 25);
				
            this.Hue = 0x8412;
            this.Female = false;		
			
            this.HairItemID = 0x203C;
            this.HairHue = 0x47A;
            this.FacialHairItemID = 0x204D;
            this.FacialHairHue = 0x47A;
        }

        public virtual void InitOutfit()
        { 
            this.AddItem(new Sandals(0x75E));
            this.AddItem(new Shirt());
            this.AddItem(new ShortPants(0x66C));
            this.AddItem(new SkullCap(0x649));
            this.AddItem(new Pitchfork());
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