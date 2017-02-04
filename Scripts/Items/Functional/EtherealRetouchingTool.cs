using System;
using Server;
using Server.Targeting;
using Server.Engines.VeteranRewards;
using Server.Mobiles;
using System.Collections.Generic;

namespace Server.Items
{	
	public class EtherealRetouchingTool : Item, IRewardItem
	{
        public override int LabelNumber { get { return 1113814; } } // Retouching Tool

        public bool IsRewardItem
        {
            get;
            set;
        }

		[Constructable]
		public EtherealRetouchingTool() : base( 0x42C6 )
		{
            LootType = LootType.Blessed;
		}

		public EtherealRetouchingTool( Serial serial ) : base( serial )
		{
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

            if(IsRewardItem)
                list.Add(1080458); // 11th Year Veteran Reward
		}
		
		public override void OnDoubleClick( Mobile from )
		{
            if (IsChildOf(from.Backpack))
            {
                from.Target = new InternalTarget(this);
                from.SendLocalizedMessage(1113815); // Target the ethereal mount you wish to retouch.
            }
		}

        private class InternalTarget : Target
        {
            private EtherealRetouchingTool m_Tool;

            public InternalTarget(EtherealRetouchingTool tool)
                : base(-1, false, TargetFlags.None)
            {
                m_Tool = tool;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is EtherealMount)
                {
                    EtherealMount mount = targeted as EtherealMount;

                    if (mount is GMEthereal)
                        from.SendMessage("You cannot use it on this!");
                    else if (mount.IsChildOf(from.Backpack) && RewardSystem.CheckIsUsableBy(from, m_Tool, null))
                    {
                        if (m_Table.ContainsKey(mount.GetType()))
                        {
                            EtherealEntry entry = m_Table[mount.GetType()];

                            if (mount.MountedID == entry.NormalID)
                            {
                                mount.MountedID = entry.TransparentID;
                                from.SendLocalizedMessage(1113817); // Your ethereal mount's transparency has been restored.
                            }
                            else
                            {
                                mount.MountedID = entry.NormalID;
                                from.SendLocalizedMessage(1113816); // Your ethereal mount's body has been solidified.
                            }

                            if (mount.EtherealHue != 0 && mount.Hue == 0)
                                mount.EtherealHue = 0;
                        }
                        else
                        {
                            if (mount.EtherealHue != EtherealMount.DefaultEtherealHue)
                            {
                                mount.EtherealHue = EtherealMount.DefaultEtherealHue;
                                from.SendLocalizedMessage(1113817); // Your ethereal mount's transparency has been restored.
                            }
                            else
                            {
                                mount.EtherealHue = mount.OriginalHue;
                                from.SendLocalizedMessage(1113816); // Your ethereal mount's body has been solidified.
                            }
                        }

                        mount.InvalidateProperties();
                        from.PlaySound(0x242);
                    }
                }
            }
        }

        private static Dictionary<Type, EtherealEntry> m_Table;

        static EtherealRetouchingTool()
        {
            m_Table = new Dictionary<Type, EtherealEntry>();

            m_Table[typeof(EtherealHorse)]      = new EtherealEntry(0x3EA0, 0x3EAA);
            m_Table[typeof(EtherealLlama)]      = new EtherealEntry(0x3EA6, 0x3EAB);
            m_Table[typeof(EtherealOstard)]     = new EtherealEntry(0x3EA5, 0x3EAC);
            m_Table[typeof(EtherealRidgeback)]  = new EtherealEntry(0x3EBA, 0x3E9A);
            m_Table[typeof(EtherealUnicorn)]    = new EtherealEntry(0x3EB4, 0x3E9B);
            m_Table[typeof(EtherealBeetle)]     = new EtherealEntry(0x3EBC, 0x3E97);
            m_Table[typeof(EtherealKirin)]      = new EtherealEntry(0x3EAD, 0x3E9C);
            m_Table[typeof(EtherealSwampDragon)] = new EtherealEntry(0x3EBD, 0x3E98);
        }

        private class EtherealEntry
        {
            private int m_NormalID;
            private int m_TransparentID;

            public int NormalID { get { return m_NormalID; } }
            public int TransparentID { get { return m_TransparentID; } }

            public EtherealEntry(int normal, int trans)
            {
                m_NormalID = normal;
                m_TransparentID = trans;
            }
        }

        public static void AddProperty(EtherealMount mount, ObjectPropertyList list)
        {
            Type t = mount.GetType();
            string cliloc = "";

            if (m_Table.ContainsKey(t))
                cliloc = mount.MountedID == mount.DefaultMountedID ? "#1078520" : "#1153298";
            else
                cliloc = mount.EtherealHue == EtherealMount.DefaultEtherealHue ? "#1078520" : "#1153298";

            list.Add(1113818, cliloc);
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 1 ); // version

            writer.Write(IsRewardItem);
		}
			
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();

            if (version == 0)
                IsRewardItem = true;
            else
                IsRewardItem = reader.ReadBool();

            if(LootType != LootType.Blessed)
                LootType = LootType.Blessed;
		}
	}	
}
