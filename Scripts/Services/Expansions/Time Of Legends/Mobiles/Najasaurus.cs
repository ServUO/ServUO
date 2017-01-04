using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a najasaurus corpse")]
    public class Najasaurus : BaseCreature
    {
        public override bool AttacksFocus { get { return true; } }

        [Constructable]
        public Najasaurus()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, .2, .4)
		{
			Name = "a najasaurus";
            Body = 1289;
            BaseSoundID = 219;

            SetStr(160, 300);
			SetDex(160, 300);
			SetInt(20, 40);
			
			SetDamage(13, 24);
            SetHits(700, 900);
			
			SetResistance( ResistanceType.Physical, 45, 55 );
			SetResistance( ResistanceType.Fire, 50, 60 );
			SetResistance( ResistanceType.Cold, 45, 55 );
			SetResistance( ResistanceType.Poison, 100 );
			SetResistance( ResistanceType.Energy, 35, 45 );
			
			SetDamageType( ResistanceType.Physical, 50 );
			SetDamageType( ResistanceType.Poison, 50 );
			
			SetSkill( SkillName.MagicResist, 150, 190 );
			SetSkill( SkillName.Tactics, 80, 95 );
			SetSkill( SkillName.Wrestling, 80, 100 );
            SetSkill( SkillName.Poisoning, 90, 100);
			
			Fame = 17000;
			Karma = -17000;
		}

        public override Poison HitPoison { get { return Poison.Lethal; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }

        public override void GenerateLoot()
        {
		    //this.AddLoot(LootPack.NewRandom(600, 800, 3, 300, 750));
            this.AddLoot(LootPack.FilthyRich);
        }

        public Najasaurus(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}