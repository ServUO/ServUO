using System;
using Server.Mobiles;

namespace Server.Items
{ 
    public class TreatiseonAlchemyTalisman : BaseTalisman
    {
        [Constructable]
        public TreatiseonAlchemyTalisman()
            : base(0x2F58)
        { 
            this.Skill = SkillName.Alchemy;
            this.SuccessBonus = GetRandomSuccessful();
            this.Blessed = GetRandomBlessed();	
			
            this.Attributes.EnhancePotions = 15;			
            this.SkillBonuses.SetValues(0, SkillName.Magery, 5.0);
        }

        public TreatiseonAlchemyTalisman(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073353;
            }
        }// Library Talisman - Treatise on Alchemy
        public override bool ForceShowName
        {
            get
            {
                return true;
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

    public class PrimerOnArmsTalisman : BaseTalisman
    {
        [Constructable]
        public PrimerOnArmsTalisman()
            : base(0x2F59)
        { 
            this.Blessed = GetRandomBlessed();	
			
            this.Attributes.BonusStr = 1;			
            this.Attributes.RegenHits = 2;
            this.Attributes.WeaponDamage = 20;
						
            this.Removal = TalismanRemoval.Damage;
            this.MaxChargeTime = 1200;
        }

        public PrimerOnArmsTalisman(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073354;
            }
        }// Library Talisman - A Primer on Arms
        public override bool ForceShowName
        {
            get
            {
                return true;
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

    public class MyBookTalisman : BaseTalisman
    {
        [Constructable]
        public MyBookTalisman()
            : base(0x2F5A)
        { 
            this.Blessed = GetRandomBlessed();	
			
            this.Skill = SkillName.Inscribe;
            this.SuccessBonus = GetRandomSuccessful();			
            this.ExceptionalBonus = GetRandomExceptional();	
			
            this.Attributes.BonusInt = 5;			
            this.Attributes.BonusMana = 2;
        }

        public MyBookTalisman(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073355;
            }
        }// Library Talisman - My Book
        public override bool ForceShowName
        {
            get
            {
                return true;
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

    public class TalkingtoWispsTalisman : BaseTalisman
    {
        [Constructable]
        public TalkingtoWispsTalisman()
            : base(0x2F5B)
        { 
            this.Blessed = GetRandomBlessed();	
			
            this.SkillBonuses.SetValues(0, SkillName.SpiritSpeak, 3.0);
            this.SkillBonuses.SetValues(1, SkillName.EvalInt, 5.0);
			
            this.Removal = TalismanRemoval.Ward;
            this.MaxChargeTime = 1200;
        }

        public TalkingtoWispsTalisman(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073356;
            }
        }// Library Talisman - Talking to Wisps
        public override bool ForceShowName
        {
            get
            {
                return true;
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

    public class GrammarOfOrchishTalisman : BaseTalisman
    {
        [Constructable]
        public GrammarOfOrchishTalisman()
            : base(0x2F59)
        { 
            this.Blessed = GetRandomBlessed();	
            this.Protection = GetRandomProtection();
            this.Summoner = new TalismanAttribute(typeof(SummonedOrcBrute), 0, 1072414);
			
            this.SkillBonuses.SetValues(0, SkillName.MagicResist, 5.0);
            this.SkillBonuses.SetValues(1, SkillName.Anatomy, 7.0);
			
            this.MaxChargeTime = 1800;
        }

        public GrammarOfOrchishTalisman(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073358;
            }
        }// Library Talisman - a Grammar of Orchish (Summoner)
        public override bool ForceShowName
        {
            get
            {
                return true;
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

    public class BirdsofBritanniaTalisman : BaseTalisman
    {
        [Constructable]
        public BirdsofBritanniaTalisman()
            : base(0x2F5A)
        { 
            this.Blessed = GetRandomBlessed();	
            this.Slayer = TalismanSlayerName.Bird;
			
            this.SkillBonuses.SetValues(0, SkillName.AnimalTaming, 5.0);
            this.SkillBonuses.SetValues(1, SkillName.AnimalLore, 5.0);
			
            this.MaxChargeTime = 1800;
        }

        public BirdsofBritanniaTalisman(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074892;
            }
        }// Library Talisman - Birds of Britannia Random Summoner
        public override bool ForceShowName
        {
            get
            {
                return true;
            }
        }
        public override Type GetSummoner()
        { 
            return GetRandomSummonType();
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

    public class TheLifeOfTravelingMinstrelTalisman : BaseTalisman
    {
        [Constructable]
        public TheLifeOfTravelingMinstrelTalisman()
            : base(0x2F5B)
        { 
            this.Blessed = GetRandomBlessed();	
            this.Protection = GetRandomProtection();
			
            this.SkillBonuses.SetValues(0, SkillName.Provocation, 5.0);
            this.SkillBonuses.SetValues(1, SkillName.Musicianship, 5.0);
			
            this.Removal = TalismanRemoval.Curse;
            this.MaxChargeTime = 1200;		
        }

        public TheLifeOfTravelingMinstrelTalisman(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073360;
            }
        }// Library Talisman - The Life of a Traveling Minstrel
        public override bool ForceShowName
        {
            get
            {
                return true;
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