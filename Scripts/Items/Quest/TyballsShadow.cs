using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles 
{
    [CorpseName("a Tyball Shadow corpse")]
    public class TyballsShadow : BaseCreature
    {
        [Constructable]
        public TyballsShadow()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.15, 0.4)
        {
            this.Body = 0x190;
            this.Hue = 0x4001;
            this.Female = false;
            this.Name = "Tyball's Shadow";
           
            this.AddItem(new Robe(2406));
                                
            this.SetStr(400, 450);
            this.SetDex(210, 250);
            this.SetInt(310, 330);

            this.SetHits(2800, 3000);

            this.SetDamage(20, 25);

            this.SetDamageType(ResistanceType.Physical, 100);
            this.SetDamageType(ResistanceType.Energy, 25);
            this.SetDamageType(ResistanceType.Poison, 20);
            this.SetDamageType(ResistanceType.Energy, 20);

            this.SetResistance(ResistanceType.Physical, 100);
            this.SetResistance(ResistanceType.Fire, 70);
            this.SetResistance(ResistanceType.Cold, 70);
            this.SetResistance(ResistanceType.Poison, 70);
            this.SetResistance(ResistanceType.Energy, 70);

            this.SetSkill(SkillName.Magery, 100.0);
            this.SetSkill(SkillName.MagicResist, 120.0);
            this.SetSkill(SkillName.Tactics, 100.0);
            this.SetSkill(SkillName.Wrestling, 100.0);

            this.Fame = 20000; 
            this.Karma = -20000;
            this.VirtualArmor = 65;
        }

        public TyballsShadow(Serial serial)
            : base(serial)
        {
        }

        public override bool BardImmune
        {
            get
            {
                return true;
            }
        }
        public override bool Unprovokable
        {
            get
            {
                return true;
            }
        }
        public override bool Uncalmable
        {
            get
            {
                return true;
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
            this.AddLoot(LootPack.FilthyRich, 3);
        }

        public override void OnDeath(Container c)
        {
            if (this.Map == Map.TerMur)
            {
                List<DamageStore> rights = GetLootingRights();
                List<Mobile> toGive = new List<Mobile>();

                for (int i = rights.Count - 1; i >= 0; --i)
                {
                    DamageStore ds = rights[i];
                    if (ds.m_HasRight)
                        toGive.Add(ds.m_Mobile);
                }

                if (toGive.Count > 0)
                    toGive[Utility.Random(toGive.Count)].AddToBackpack(new YellowKey1());

                /*else
                c.DropItem(new YellowKey1());*/

                if (Utility.RandomDouble() < 0.10)
                    c.DropItem(new ShroudOfTheCondemned());
            }
            base.OnDeath(c);
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