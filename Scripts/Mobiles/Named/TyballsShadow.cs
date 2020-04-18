using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles
{
    [CorpseName("a tyball shadow corpse")]
    public class TyballsShadow : BaseCreature
    {
        [Constructable]
        public TyballsShadow()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.15, 0.4)
        {
            Body = 0x190;
            Hue = 0x4001;
            Female = false;
            Name = "Tyball's Shadow";

            SetStr(400, 450);
            SetDex(210, 250);
            SetInt(310, 330);

            SetHits(2800, 3000);

            SetDamage(20, 25);

            SetDamageType(ResistanceType.Physical, 100);
            SetDamageType(ResistanceType.Energy, 25);
            SetDamageType(ResistanceType.Poison, 20);
            SetDamageType(ResistanceType.Energy, 20);

            SetResistance(ResistanceType.Physical, 70);
            SetResistance(ResistanceType.Fire, 70);
            SetResistance(ResistanceType.Cold, 70);
            SetResistance(ResistanceType.Poison, 70);
            SetResistance(ResistanceType.Energy, 70);

            SetSkill(SkillName.Magery, 100.0);
            SetSkill(SkillName.MagicResist, 120.0);
            SetSkill(SkillName.Tactics, 100.0);
            SetSkill(SkillName.Wrestling, 100.0);

            SetWearable(new ShroudOfTheCondemned(), -1, 0.1);

            Fame = 20000;
            Karma = -20000;
        }

        public TyballsShadow(Serial serial)
            : base(serial)
        {
        }

        public override bool BardImmune => true;

        public override bool Unprovokable => true;

        public override bool Uncalmable => true;

        public override bool AlwaysMurderer => true;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 3);
        }

        public override void OnDeath(Container c)
        {
            if (Map == Map.TerMur)
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

                ColUtility.Free(toGive);
            }
            base.OnDeath(c);
        }

        public override void OnThink()
        {
            base.OnThink();

            if (Map != null && Region.Find(Location, Map).IsPartOf("Underworld"))
            {
                if (Z < 0 && X >= 1177 && X <= 1183 && Y >= 877 && Y <= 886)
                    Z = 0;
            }
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