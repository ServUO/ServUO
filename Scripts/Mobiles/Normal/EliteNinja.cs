using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;

namespace Server.Mobiles
{
	public class EliteNinja : BaseCreature
	{
		public override bool ClickTitle{ get{ return false; } }
        public override bool CanStealth { get { return true; } }

        private DateTime m_NextWeaponChange;

		[Constructable]
		public EliteNinja() : base( AIType.AI_Ninja, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			SpeechHue = Utility.RandomDyedHue();
			Hue = Utility.RandomSkinHue();
			Name = "an elite ninja";

			Body = ( this.Female = Utility.RandomBool() ) ? 0x191 : 0x190;

			SetHits( 251, 350 );

			SetStr( 126, 225 );
			SetDex( 81, 95 );
			SetInt( 151, 165 );

			SetDamage( 12, 20 );

			SetDamageType( ResistanceType.Physical, 65 );
			SetDamageType( ResistanceType.Fire, 15 );
			SetDamageType( ResistanceType.Poison, 15 );
			SetDamageType( ResistanceType.Energy, 5 );

			SetResistance( ResistanceType.Physical, 35, 65 );
			SetResistance( ResistanceType.Fire, 40, 60 );
			SetResistance( ResistanceType.Cold, 25, 45 );
			SetResistance( ResistanceType.Poison, 40, 60 );
			SetResistance( ResistanceType.Energy, 35, 55 );

			SetSkill( SkillName.Anatomy, 105.0, 120.0 );
			SetSkill( SkillName.MagicResist, 80.0, 100.0 );
			SetSkill( SkillName.Tactics, 115.0, 130.0 );
			SetSkill( SkillName.Wrestling, 95.0, 120.0 );
			SetSkill( SkillName.Fencing, 95.0, 120.0 );
			SetSkill( SkillName.Macing, 95.0, 120.0 );
			SetSkill( SkillName.Swords, 95.0, 120.0 );

			SetSkill( SkillName.Ninjitsu, 95.0, 120.0 );
            SetSkill( SkillName.Hiding, 100.0);
            SetSkill( SkillName.Stealth, 120.0 );

			Fame = 8500;
			Karma = -8500;

            LeatherNinjaBelt belt = new LeatherNinjaBelt();
            belt.UsesRemaining = 20;
            belt.Poison = Poison.Greater;
            belt.PoisonCharges = 20;
            belt.Movable = false;
            AddItem(belt);

            int amount = Skills[SkillName.Ninjitsu].Value >= 100 ? 2 : 1;

            for (int i = 0; i < amount; i++)
            {
                Fukiya f = new Fukiya();
                f.UsesRemaining = 10;
                f.Poison = amount == 1 ? Poison.Regular : Poison.Greater;
                f.PoisonCharges = 10;
                f.Movable = false;
                PackItem(f);
            }

			AddItem( new NinjaTabi() );
			AddItem( new LeatherNinjaJacket());
			AddItem( new LeatherNinjaHood());
			AddItem( new LeatherNinjaPants());
			AddItem( new LeatherNinjaMitts());
			
			if( Utility.RandomDouble() < 0.33 )
				PackItem( new SmokeBomb() );

            if (Utility.RandomBool())
                PackItem(new Tessen());
            else
                PackItem(new Wakizashi());

            if (Utility.RandomBool())
                PackItem(new Nunchaku());
            else
                PackItem(new Daisho());

            if (Utility.RandomBool())
                PackItem(new Sai());
            else
                PackItem(new Tekagi());

            if (Utility.RandomBool())
                PackItem(new Kama());
            else
                PackItem(new Katana());

			Utility.AssignRandomHair( this );
            ChangeWeapon();
		}

		public override void OnDeath( Container c )
		{
			base.OnDeath( c );
			c.DropItem( new BookOfNinjitsu() );
		}

		public override bool BardImmune{ get{ return true; } }

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich );
			AddLoot( LootPack.Rich );
			AddLoot( LootPack.Gems, 2 );
		}
		
		public override bool AlwaysMurderer{ get{ return true; } }

        private void ChangeWeapon()
        {
            if (Backpack == null)
                return;

            Item item = FindItemOnLayer(Layer.OneHanded);

            if (item == null)
                item = FindItemOnLayer(Layer.TwoHanded);

            System.Collections.Generic.List<BaseWeapon> weapons = new System.Collections.Generic.List<BaseWeapon>();

            foreach (Item i in Backpack.Items)
            {
                if (i is BaseWeapon && i != item)
                    weapons.Add((BaseWeapon)i);
            }

            if (weapons.Count > 0)
            {
                if (item != null)
                    Backpack.DropItem(item);

                AddItem(weapons[Utility.Random(weapons.Count)]);

                m_NextWeaponChange = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(30, 60));
            }
        }

        public override void OnThink()
        {
            base.OnThink();

            if (Combatant != null && m_NextWeaponChange < DateTime.UtcNow)
                ChangeWeapon();
        }

		public EliteNinja( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

            m_NextWeaponChange = DateTime.UtcNow;
		}
	}
}