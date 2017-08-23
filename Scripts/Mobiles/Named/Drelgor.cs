
using System;
using Server;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.Misc;
using Server.Regions;
using Server.Network;
using Server.Targeting;

namespace Server.Mobiles
{
    [CorpseName("A Drelgor The Impaler Corpse")]
    public class Drelgor : BaseCreature
    {
        private bool init = false; //Don't change this.
        private double msgevery = 1.0; //Recurring message. Change to 0 to disable.
        private DateTime m_NextMsgTime;

        [Constructable]
        public Drelgor()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Drelgor the Impaler";
            Body = 147;
            BaseSoundID = 451;

            SetStr(127, 137);
            SetDex(82, 94);
            SetInt(48, 55);

            SetHits(131, 136);

            SetDamage(6, 8);

            SetDamageType(ResistanceType.Physical, 40);
            SetDamageType(ResistanceType.Cold, 60);

            SetResistance(ResistanceType.Physical, 35, 45);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 50, 60);
            SetResistance(ResistanceType.Poison, 20, 30);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.Wrestling, 60);
            SetSkill(SkillName.Tactics, 60);
            SetSkill(SkillName.MagicResist, 60);

            Fame = 3600;
            Karma = -3600;

            VirtualArmor = 40;

            PackItem(new Scimitar());
            PackItem(new WoodenShield());

            switch (Utility.Random(5))
            {
                case 0:
                    this.PackItem(new BoneArms());
                    break;
                case 1:
                    this.PackItem(new BoneChest());
                    break;
                case 2:
                    this.PackItem(new BoneGloves());
                    break;
                case 3:
                    this.PackItem(new BoneLegs());
                    break;
                case 4:
                    this.PackItem(new BoneHelm());
                    break;
            }
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
            AddLoot(LootPack.Meager);
        }

        public override bool BleedImmune { get { return true; } }

        #region Start/Stop
        private void Start()
        {
            m_NextMsgTime = DateTime.UtcNow + TimeSpan.FromMinutes(msgevery);

            BroadcastMessage();
            init = true;
        }
        #endregion
        
        #region Broadcast Message
        public void BroadcastMessage()
        {
            foreach (NetState state in NetState.Instances)
            {
                Mobile m = state.Mobile;

                if (m != null && this.Region.Name == "Old Haven Training" && m.Region.Name == "Old Haven Training")
                {
                    m.SendLocalizedMessage(1077840, "", 34); // // Who dares to defile Haven? I am Drelgor the Impaler! I shall claim your souls as payment for this intrusion!
                    m.PlaySound(0x14);
                }
            }
        }
        #endregion

        #region OnThink
        public override void OnThink()
        {
            if (!init)
                Start();

            if (msgevery != 0 && DateTime.UtcNow >= m_NextMsgTime)
            {
                BroadcastMessage();
                m_NextMsgTime = DateTime.UtcNow + TimeSpan.FromMinutes(msgevery);
            }

            base.OnThink();
        }
        #endregion

        public Drelgor(Serial serial)
            : base(serial)
        {
        }

        public override OppositionGroup OppositionGroup
        {
            get { return OppositionGroup.FeyAndUndead; }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
            writer.Write(init);
            writer.Write(m_NextMsgTime);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            init = reader.ReadBool();
            m_NextMsgTime = reader.ReadDateTime();
        }
    }
}

