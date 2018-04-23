using System;
using Server.Items;
using Server.Gumps;
using Server.Network;
using Server.Targeting;
using Server.Multis;
using Server.ContextMenus;
using System.Collections;
using System.Collections.Generic;
using Server.Targets;
using Server.Engines.VeteranRewards;

namespace Server.Items
{
    public class TransmutationStone : Item, IRewardItem
   {
       private bool m_IsRewardItem;
       [CommandProperty(AccessLevel.GameMaster)]
       public bool IsRewardItem
       {
           get { return m_IsRewardItem; }
           set { m_IsRewardItem = value; InvalidateProperties(); }
       }
      private TransmutationStone m_Item;
      private static Item t_item;
      private static int priceAdjustment = 100;
      private static string price;
      private static CraftResource resource;

      // Base alchemy skill required. Set to zero to get rid of skill check.
      private static double MinAlchemySkill = 9.0;

      // Base skill required to transmute to a certain resource.
      // Multiplied by 'MinAlchemySkill' to get actual skill needed to transmute to that resource.
      // Setting to '0' cause no skill check for that resource.
      private static int IronSkill      = 1;
      private static int DullCopperSkill   = 2;
      private static int ShadowIronSkill   = 3;
      private static int CopperSkill      = 4;
      private static int BronzeSkill      = 5;
      private static int GoldSkill        = 6;
      private static int AgapiteSkill     = 7;
      private static int VeriteSkill      = 8;
      private static int ValoriteSkill   = 10;

      // Base cost of transmuting to a certain resource.
      // Setting to '0' will cause no fee to transmute to that resource
      private static int BaseIron         = 1;
      private static int BaseDullCopper   = 2;
      private static int BaseShadowIron   = 3;
      private static int BaseCopper      = 4;
      private static int BaseBronze       = 5;
      private static int BaseGold         = 6;
      private static int BaseAgapite      = 7;
      private static int BaseVerite       = 8;
      private static int BaseValorite       = 10;

      // Used to vary the cost of transmutation based on original resource
      // of an item.
      // Setting to '0' will cause no fee to transmute from that resource
      private static int IronAdjustment      = 10;
      private static int DullCopperAdjustment   = 8;
      private static int ShadowIronAdjustment   = 7;
      private static int CopperAdjustment      = 6;
      private static int BronzeAdjustment     = 5;
      private static int GoldAdjustment       = 4;
      private static int AgapiteAdjustment    = 3;
      private static int VeriteAdjustment     = 2;
      private static int ValoriteAdjustment   = 1;

      [Constructable]
      public TransmutationStone() : base( 0xED4 )
      {
         Movable = false;
         Hue = 0x65;
         Name = "a transmutation stone";
      }

      public TransmutationStone( Serial serial ) : base( serial )
      {
      }
  
      public override void OnDoubleClick( Mobile from )
      {
          if (m_IsRewardItem && !RewardSystem.CheckIsUsableBy(from, this, null))
          {
              from.SendMessage("This does not belong to you!!");
              return;
          }
         if( from.Skills[SkillName.Alchemy].Value < MinAlchemySkill )
         {
            from.SendMessage( "Your alchemy skill is not high enough to transmute items." );
            return;
         }

         from.SendMessage( "Select the item you wish to transmute." );

         if ( from.InRange( this.GetWorldLocation(), 1 )  )
         {
            from.Target = new TransTarget( this );
         }
         else
         {
            from.SendMessage( "That is too far away to use." );
         }
      }

      private class TransTarget : Target
      {
         public TransmutationStone m_Stone;
         public TransTarget( Item item ) : base( 1, false, TargetFlags.Beneficial )
         {
         }

         protected override void OnTarget( Mobile from, object target )
         {
            bool done = false;
            
            if ( !(target is Item) )
            {
               from.SendMessage( "You can only transmute items." );
               return;
            }

            t_item = (Item)target;

            if( t_item is BaseArmor || t_item is BaseWeapon )
            {
               from.SendMessage( "Select transmutation resource." );
            }
            else if ( t_item is TransmutationStone )
            {
               BaseHouse house = BaseHouse.FindHouseAt( m_Stone );
               if ( house != null && ( house.IsOwner( from ) || from.AccessLevel >= AccessLevel.GameMaster ) )
               {
                      from.CloseGump( typeof( TStoneGump ) );
                      from.SendGump( new TStoneGump( m_Stone ) );
               }
            }
            else
            {
               from.SendMessage( "You can only transmute armor and weapons." );
               return;
            }

            if ( t_item.IsChildOf( from.Backpack, true ) )
            {
               if ( t_item is BaseArmor )
               {
                  BaseArmor armor = (BaseArmor)t_item;

                  switch( armor.Resource )
                  {
                     case CraftResource.Iron :
                     {
                        resource = armor.Resource;
                        priceAdjustment = IronAdjustment;
                        break;
                     }
                     case CraftResource.DullCopper :
                     {
                        resource = armor.Resource;
                        priceAdjustment = DullCopperAdjustment;
                        break;
                     }
                     case CraftResource.ShadowIron :
                     {
                        resource = armor.Resource;
                        priceAdjustment = ShadowIronAdjustment;
                        break;
                     }
                     case CraftResource.Copper :
                     {
                        resource = armor.Resource;
                        priceAdjustment = CopperAdjustment;
                        break;
                     }
                     case CraftResource.Bronze :
                     {
                        resource = armor.Resource;
                        priceAdjustment = BronzeAdjustment;
                        break;
                     }
                     case CraftResource.Gold :
                     {
                        resource = armor.Resource;
                        priceAdjustment = GoldAdjustment;
                        break;
                     }
                     case CraftResource.Agapite :
                     {
                        resource = armor.Resource;
                        priceAdjustment = AgapiteAdjustment;
                        break;
                     }
                     case CraftResource.Verite :
                     {
                        resource = armor.Resource;
                        priceAdjustment = VeriteAdjustment;
                        break;
                     }
                     case CraftResource.Valorite :
                     {
                        resource = armor.Resource;
                        priceAdjustment = ValoriteAdjustment;
                        break;
                     }
                     default :
                     {
                        from.SendMessage( "You can not transmute armor made with that resource." );
                        return;
                     }
                  }
               }
               else
               {
                  BaseWeapon weapon = (BaseWeapon)t_item;

                  switch( weapon.Resource )
                  {
                     case CraftResource.Iron :
                     {
                        resource = weapon.Resource;
                        priceAdjustment = IronAdjustment;
                        break;
                     }
                     case CraftResource.DullCopper :
                     {
                        resource = weapon.Resource;
                        priceAdjustment = DullCopperAdjustment;
                        break;
                     }
                     case CraftResource.ShadowIron :
                     {
                        resource = weapon.Resource;
                        priceAdjustment = ShadowIronAdjustment;
                        break;
                     }
                     case CraftResource.Copper :
                     {
                        resource = weapon.Resource;
                        priceAdjustment = CopperAdjustment;
                        break;
                     }
                     case CraftResource.Bronze :
                     {
                        resource = weapon.Resource;
                        priceAdjustment = BronzeAdjustment;
                        break;
                     }
                     case CraftResource.Gold :
                     {
                        resource = weapon.Resource;
                        priceAdjustment = GoldAdjustment;
                        break;
                     }
                     case CraftResource.Agapite :
                     {
                        resource = weapon.Resource;
                        priceAdjustment = AgapiteAdjustment;
                        break;
                     }
                     case CraftResource.Verite :
                     {
                        resource = weapon.Resource;
                        priceAdjustment = VeriteAdjustment;
                        break;
                     }
                     case CraftResource.Valorite :
                     {
                        resource = weapon.Resource;
                        priceAdjustment = ValoriteAdjustment;
                        break;
                     }
                     default :
                     {
                        from.SendMessage( "You can not transmute weapons made with that resource." );
                        return;
                     }
                  }
               }
               from.SendGump( new TransGump(from) );
            }
            else from.SendMessage( "The item must be in your backpack to transmute." );
         }
      }

      public class TransGump : Gump
      {
         private Mobile m_Owner;
         public Mobile Owner{ get{ return m_Owner; } set{ m_Owner = value; } }

         public TransGump(Mobile owner) : base( 10, 10 )
         {
            owner.CloseGump( typeof( TransGump ) );

            int gumpX = 0;
            int gumpX1 = 0; int gumpX2 = 0;
            int gumpY = 0;

            m_Owner = owner;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = true;

            // Gump
            AddPage( 0 );
            gumpX = 85; gumpY = 60;
            AddImage( gumpX, gumpY, 0xDB0 );

            gumpX = 59; gumpY = 59;
            AddImage( gumpX, gumpY, 0xDAF );

            gumpX = 324; gumpY = 59;
            AddImage( gumpX, gumpY, 0xDB1 );

            gumpX = 58; gumpY = 297;
            AddImage( gumpX, gumpY, 0xDB2 );

            gumpX = 84; gumpY = 298;
            AddImage( gumpX, gumpY, 0xDB3 );

            gumpX = 323; gumpY = 298;
            AddImage( gumpX, gumpY, 0xDB4 );

            gumpX = 322; gumpY = 34;
            AddImage( gumpX, gumpY, 0xDAE );

            gumpX = 80; gumpY = 34;
            AddImage( gumpX, gumpY, 0xDAD );

            gumpX = 59; gumpY = 34;
            AddImage( gumpX, gumpY, 0xDAC );

            // Buttons
            gumpX = 100; gumpY = 85;
            AddButton( gumpX, gumpY, 0xFB7, 0xFB9, 1, GumpButtonType.Reply, 0 );

            gumpY = 110;
            AddButton( gumpX, gumpY, 0xFB7, 0xFB9, 2, GumpButtonType.Reply, 0 );

            gumpY = 135;
            AddButton( gumpX, gumpY, 0xFB7, 0xFB9, 3, GumpButtonType.Reply, 0 );

            gumpY = 160;
            AddButton( gumpX, gumpY, 0xFB7, 0xFB9, 4, GumpButtonType.Reply, 0 );

            gumpY = 185;
            AddButton( gumpX, gumpY, 0xFB7, 0xFB9, 5, GumpButtonType.Reply, 0 );

            gumpY = 210;
            AddButton( gumpX, gumpY, 0xFB7, 0xFB9, 6, GumpButtonType.Reply, 0 );

            gumpY = 235;
            AddButton( gumpX, gumpY, 0xFB7, 0xFB9, 7, GumpButtonType.Reply, 0 );

            gumpY = 260;
            AddButton( gumpX, gumpY, 0xFB7, 0xFB9, 8, GumpButtonType.Reply, 0 );

            gumpY = 285;
            AddButton( gumpX, gumpY, 0xFB7, 0xFB9, 9, GumpButtonType.Reply, 0 );

            // Text
            gumpX = 120; gumpY = 45;
            AddHtml( gumpX, gumpY, 200, 20, "Select the resource you wish", false, false );
            gumpX = 120; gumpY = 60;
            AddHtml( gumpX, gumpY, 200, 20, "to transmute your item into.", false, false );

            if( resource == CraftResource.Iron)
            {
               price = "-";
            }
            else price = (BaseIron * priceAdjustment).ToString();
            gumpX1 = 160; gumpX2 = 280;
            gumpY = 85;
            AddHtml( gumpX1, gumpY, 200, 20, "Iron", false, false );
            AddHtml( gumpX2, gumpY, 200, 20, price, false, false );

            if( resource == CraftResource.DullCopper)
            {
               price = "-";
            }
            else price = (BaseDullCopper * priceAdjustment).ToString();
            gumpY = 110;
            AddHtml( gumpX1, gumpY, 200, 20, "Dull Copper", false, false );
            AddHtml( gumpX2, gumpY, 200, 20, price, false, false );

            if( resource == CraftResource.ShadowIron)
            {
               price = "-";
            }
            else price = (BaseShadowIron * priceAdjustment).ToString();
            gumpY = 135;
            AddHtml( gumpX1, gumpY, 200, 20, "Shadow Iron", false, false );
            AddHtml( gumpX2, gumpY, 200, 20, price, false, false );

            if( resource == CraftResource.Copper)
            {
               price = "-";
            }
            else price = (BaseCopper * priceAdjustment).ToString();
            gumpY = 160;
            AddHtml( gumpX1, gumpY, 200, 20, "Copper", false, false );
            AddHtml( gumpX2, gumpY, 200, 20, price, false, false );

            if( resource == CraftResource.Bronze)
            {
               price = "-";
            }
            else price = (BaseBronze * priceAdjustment).ToString();
            gumpY = 185;
            AddHtml( gumpX1, gumpY, 200, 20, "Bronze", false, false );
            AddHtml( gumpX2, gumpY, 200, 20, price, false, false );

            if( resource == CraftResource.Gold)
            {
               price = "-";
            }
            else price = (BaseGold * priceAdjustment).ToString();
            gumpY = 210;
            AddHtml( gumpX1, gumpY, 200, 20, "Gold", false, false );
            AddHtml( gumpX2, gumpY, 200, 20, price, false, false );

            if( resource == CraftResource.Agapite)
            {
               price = "-";
            }
            else price = (BaseAgapite * priceAdjustment).ToString();
            gumpY = 235;
            AddHtml( gumpX1, gumpY, 200, 20, "Agapite", false, false );
            AddHtml( gumpX2, gumpY, 200, 20, price, false, false );

            if( resource == CraftResource.Verite)
            {
               price = "-";
            }
            else price = (BaseVerite * priceAdjustment).ToString();
            gumpY = 260;
            AddHtml( gumpX1, gumpY, 200, 20, "Verite", false, false );
            AddHtml( gumpX2, gumpY, 200, 20, price, false, false );

            if( resource == CraftResource.Valorite)
            {
               price = "-";
            }
            else price = (BaseValorite * priceAdjustment).ToString();
            gumpY = 285;
            AddHtml( gumpX1, gumpY, 200, 20, "Valorite", false, false );
            AddHtml( gumpX2, gumpY, 200, 20, price, false, false );

         }

         public override void OnResponse( NetState state, RelayInfo info )
         {
            Mobile from = state.Mobile;
            Container bpack = from.Backpack;

            if ( t_item is BaseArmor )
            {
               BaseArmor armor = (BaseArmor)t_item;

               switch( info.ButtonID )
               {
                  case 1:
                  {
                     if( from.Skills[SkillName.Alchemy].Value < MinAlchemySkill * IronSkill  )
                     {
                        from.SendMessage( "Your alchemy skill is not high enough to transmute that item." );
                        return;
                     }

                     if( armor.Resource == CraftResource.Iron)
                     {
                        from.SendMessage( "That item is already made of iron." );
                     }
                     else
                     {
                        if ( bpack.ConsumeTotal( typeof( Gold ), BaseIron * priceAdjustment) )
                        {
                           armor.Resource = CraftResource.Iron;
                           from.SendMessage( "Your armor has been transmuted." );
                        }
                        else from.SendMessage( "You do not have enough gold. \nYou need {0} in gold to transmute that item.",BaseIron * priceAdjustment );
                     }
                     break;
                  }
                  case 2:
                  {
                     if( from.Skills[SkillName.Alchemy].Value < MinAlchemySkill * DullCopperSkill )
                     {
                        from.SendMessage( "Your alchemy skill is not high enough to transmute that item." );
                        return;
                     }

                     if( armor.Resource == CraftResource.DullCopper)
                     {
                        from.SendMessage( "That item is already made of dull copper." );
                     }
                     else
                     {
                        if ( bpack.ConsumeTotal( typeof( Gold ), BaseDullCopper * priceAdjustment) )
                        {
                           armor.Resource = CraftResource.DullCopper;
                           from.SendMessage( "Your armor has been transmuted." );
                        }
                        else from.SendMessage( "You do not have enough gold. \nYou need {0} in gold to transmute that item.",BaseDullCopper * priceAdjustment );
                     }
                     break;
                  }
                  case 3:
                  {
                     if( from.Skills[SkillName.Alchemy].Value < MinAlchemySkill * ShadowIronSkill )
                     {
                        from.SendMessage( "Your alchemy skill is not high enough to transmute that item." );
                        return;
                     }

                     if( armor.Resource == CraftResource.ShadowIron)
                     {
                        from.SendMessage( "That item is already made of shadow iron." );
                     }
                     else
                     {
                        if ( bpack.ConsumeTotal( typeof( Gold ), BaseShadowIron * priceAdjustment) )
                        {
                           armor.Resource = CraftResource.ShadowIron;
                           from.SendMessage( "Your armor has been transmuted." );
                        }
                        else from.SendMessage( "You do not have enough gold. \nYou need {0} in gold to transmute that item.",BaseShadowIron * priceAdjustment);
                     }
                     break;
                  }
                  case 4:
                  {
                     if( from.Skills[SkillName.Alchemy].Value < MinAlchemySkill * CopperSkill )
                     {
                        from.SendMessage( "Your alchemy skill is not high enough to transmute that item." );
                        return;
                     }

                     if( armor.Resource == CraftResource.Copper)
                     {
                        from.SendMessage( "That item is already made of copper." );
                     }
                     else
                     {
                        if ( bpack.ConsumeTotal( typeof( Gold ), BaseCopper * priceAdjustment) )
                        {
                           armor.Resource = CraftResource.Copper;
                           from.SendMessage( "Your armor has been transmuted." );
                        }
                        else from.SendMessage( "You do not have enough gold. \nYou need {0} in gold to transmute that item.",BaseCopper * priceAdjustment );
                     }
                     break;
                  }
                  case 5:
                  {
                     if( from.Skills[SkillName.Alchemy].Value < MinAlchemySkill * BronzeSkill)
                     {
                        from.SendMessage( "Your alchemy skill is not high enough to transmute that item." );
                        return;
                     }

                     if( armor.Resource == CraftResource.Bronze)
                     {
                        from.SendMessage( "That item is already made of bronze." );
                     }
                     else
                     {
                        if ( bpack.ConsumeTotal( typeof( Gold ), BaseBronze * priceAdjustment) )
                        {
                           armor.Resource = CraftResource.Bronze;
                           from.SendMessage( "Your armor has been transmuted." );
                        }
                        else from.SendMessage( "You do not have enough gold. \nYou need {0} in gold to transmute that item.",BaseBronze * priceAdjustment );
                     }
                     break;
                  }
                  case 6:
                  {
                     if( from.Skills[SkillName.Alchemy].Value < MinAlchemySkill * GoldSkill )
                     {
                        from.SendMessage( "Your alchemy skill is not high enough to transmute that item." );
                        return;
                     }

                     if( armor.Resource == CraftResource.Gold)
                     {
                        from.SendMessage( "That item is already made of gold." );
                     }
                     else
                     {
                        if ( bpack.ConsumeTotal( typeof( Gold ), BaseGold * priceAdjustment) )
                        {
                           armor.Resource = CraftResource.Gold;
                           from.SendMessage( "Your armor has been transmuted." );
                        }
                        else from.SendMessage( "You do not have enough gold. \nYou need {0} in gold to transmute that item.",BaseGold * priceAdjustment );
                     }
                     break;
                  }
                  case 7:
                  {
                     if( from.Skills[SkillName.Alchemy].Value < MinAlchemySkill * AgapiteSkill )
                     {
                        from.SendMessage( "Your alchemy skill is not high enough to transmute that item." );
                        return;
                     }

                     if( armor.Resource == CraftResource.Agapite)
                     {
                        from.SendMessage( "That item is already made of agapite." );
                     }
                     else
                     {
                        if ( bpack.ConsumeTotal( typeof( Gold ), BaseAgapite * priceAdjustment) )
                        {
                           armor.Resource = CraftResource.Agapite;
                           from.SendMessage( "Your armor has been transmuted." );
                        }
                        else from.SendMessage( "You do not have enough gold. \nYou need {0} in gold to transmute that item.",BaseAgapite * priceAdjustment );
                     }
                     break;
                  }
                  case 8:
                  {
                     if( from.Skills[SkillName.Alchemy].Value < MinAlchemySkill * VeriteSkill )
                     {
                        from.SendMessage( "Your alchemy skill is not high enough to transmute that item." );
                        return;
                     }

                     if( armor.Resource == CraftResource.Verite)
                     {
                        from.SendMessage( "That item is already made of verite." );
                     }
                     else
                     {
                        if ( bpack.ConsumeTotal( typeof( Gold ), BaseVerite * priceAdjustment) )
                        {
                           armor.Resource = CraftResource.Verite;
                           from.SendMessage( "Your armor has been transmuted." );
                        }
                        else from.SendMessage( "You do not have enough gold. \nYou need {0} in gold to transmute that item.",BaseVerite * priceAdjustment );
                     }
                     break;
                  }
                  case 9:
                  {
                     if( from.Skills[SkillName.Alchemy].Value < MinAlchemySkill * ValoriteSkill )
                     {
                        from.SendMessage( "Your alchemy skill is not high enough to transmute that item." );
                        return;
                     }

                     if( armor.Resource == CraftResource.Valorite)
                     {
                        from.SendMessage( "That item is already made of valorite." );
                     }
                     else
                     {
                        if ( bpack.ConsumeTotal( typeof( Gold ), BaseValorite * priceAdjustment) )
                        {
                           armor.Resource = CraftResource.Valorite;
                           from.SendMessage( "Your armor has been transmuted." );
                        }
                        else from.SendMessage( "You do not have enough gold. \nYou need {0} in gold to transmute that item.",BaseValorite * priceAdjustment );
                     }
                     break;
                  }
                  default:
                  {
                     from.SendMessage( "Transmutation was aborted." );
                     break;
                  }
               }
            }
            else if ( t_item is BaseWeapon )
            {
               BaseWeapon weapon = (BaseWeapon)t_item;

               switch( info.ButtonID )
               {
                  case 1:
                  {
                     if( from.Skills[SkillName.Alchemy].Value < MinAlchemySkill * IronSkill )
                     {
                        from.SendMessage( "Your alchemy skill is not high enough to transmute that item." );
                        return;
                     }

                     if( weapon.Resource == CraftResource.Iron)
                     {
                        from.SendMessage( "That item is already made of iron." );
                     }
                     else
                     {
                        if ( bpack.ConsumeTotal( typeof( Gold ), BaseIron * priceAdjustment) )
                        {
                           weapon.Resource = CraftResource.Iron;
                           from.SendMessage( "Your weapon has been transmuted." );
                        }
                        else from.SendMessage( "You do not have enough gold. \nYou need {0} in gold to transmute that item.",BaseIron * priceAdjustment );
                     }
                     break;
                  }
                  case 2:
                  {
                     if( from.Skills[SkillName.Alchemy].Value < MinAlchemySkill * DullCopperSkill )
                     {
                        from.SendMessage( "Your alchemy skill is not high enough to transmute that item." );
                        return;
                     }

                     if( weapon.Resource == CraftResource.DullCopper)
                     {
                        from.SendMessage( "That item is already made of dull copper." );
                     }
                     else
                     {
                        if ( bpack.ConsumeTotal( typeof( Gold ), BaseDullCopper * priceAdjustment) )
                        {
                           weapon.Resource = CraftResource.DullCopper;
                           from.SendMessage( "Your weapon has been transmuted." );
                        }
                        else from.SendMessage( "You do not have enough gold. \nYou need {0} in gold to transmute that item.",BaseDullCopper * priceAdjustment );
                     }
                     break;
                  }
                  case 3:
                  {
                     if( from.Skills[SkillName.Alchemy].Value < MinAlchemySkill * ShadowIronSkill )
                     {
                        from.SendMessage( "Your alchemy skill is not high enough to transmute that item." );
                        return;
                     }

                     if( weapon.Resource == CraftResource.ShadowIron)
                     {
                        from.SendMessage( "That item is already made of shadow iron." );
                     }
                     else
                     {
                        if ( bpack.ConsumeTotal( typeof( Gold ), BaseShadowIron * priceAdjustment) )
                        {
                           weapon.Resource = CraftResource.ShadowIron;
                           from.SendMessage( "Your weapon has been transmuted." );
                        }
                        else from.SendMessage( "You do not have enough gold. \nYou need {0} in gold to transmute that item.",BaseShadowIron * priceAdjustment);
                     }
                     break;
                  }
                  case 4:
                  {
                     if( from.Skills[SkillName.Alchemy].Value < MinAlchemySkill * CopperSkill )
                     {
                        from.SendMessage( "Your alchemy skill is not high enough to transmute that item." );
                        return;
                     }

                     if( weapon.Resource == CraftResource.Copper)
                     {
                        from.SendMessage( "That item is already made of copper." );
                     }
                     else
                     {
                        if ( bpack.ConsumeTotal( typeof( Gold ), BaseCopper * priceAdjustment) )
                        {
                           weapon.Resource = CraftResource.Copper;
                           from.SendMessage( "Your weapon has been transmuted." );
                        }
                        else from.SendMessage( "You do not have enough gold. \nYou need {0} in gold to transmute that item.",BaseCopper * priceAdjustment );
                     }
                     break;
                  }
                  case 5:
                  {
                     if( from.Skills[SkillName.Alchemy].Value < MinAlchemySkill * BronzeSkill )
                     {
                        from.SendMessage( "Your alchemy skill is not high enough to transmute that item." );
                        return;
                     }

                     if( weapon.Resource == CraftResource.Bronze)
                     {
                        from.SendMessage( "That item is already made of bronze." );
                     }
                     else
                     {
                        if ( bpack.ConsumeTotal( typeof( Gold ), BaseBronze * priceAdjustment) )
                        {
                           weapon.Resource = CraftResource.Bronze;
                           from.SendMessage( "Your weapon has been transmuted." );
                        }
                        else from.SendMessage( "You do not have enough gold. \nYou need {0} in gold to transmute that item.",BaseBronze * priceAdjustment );
                     }
                     break;
                  }
                  case 6:
                  {
                     if( from.Skills[SkillName.Alchemy].Value < MinAlchemySkill * GoldSkill )
                     {
                        from.SendMessage( "Your alchemy skill is not high enough to transmute that item." );
                        return;
                     }

                     if( weapon.Resource == CraftResource.Gold)
                     {
                        from.SendMessage( "That item is already made of gold." );
                     }
                     else
                     {
                        if ( bpack.ConsumeTotal( typeof( Gold ), BaseGold * priceAdjustment) )
                        {
                           weapon.Resource = CraftResource.Gold;
                           from.SendMessage( "Your weapon has been transmuted." );
                        }
                        else from.SendMessage( "You do not have enough gold. \nYou need {0} in gold to transmute that item.",BaseGold * priceAdjustment );
                     }
                     break;
                  }
                  case 7:
                  {
                     if( from.Skills[SkillName.Alchemy].Value < MinAlchemySkill * AgapiteSkill )
                     {
                        from.SendMessage( "Your alchemy skill is not high enough to transmute that item." );
                        return;
                     }

                     if( weapon.Resource == CraftResource.Agapite)
                     {
                        from.SendMessage( "That item is already made of agapite." );
                     }
                     else
                     {
                        if ( bpack.ConsumeTotal( typeof( Gold ), BaseAgapite * priceAdjustment) )
                        {
                           weapon.Resource = CraftResource.Agapite;
                           from.SendMessage( "Your weapon has been transmuted." );
                        }
                        else from.SendMessage( "You do not have enough gold. \nYou need {0} in gold to transmute that item.",BaseAgapite * priceAdjustment );
                     }
                     break;
                  }
                  case 8:
                  {
                     if( from.Skills[SkillName.Alchemy].Value < MinAlchemySkill * VeriteSkill )
                     {
                        from.SendMessage( "Your alchemy skill is not high enough to transmute that item." );
                        return;
                     }

                     if( weapon.Resource == CraftResource.Verite)
                     {
                        from.SendMessage( "That item is already made of verite." );
                     }
                     else
                     {
                        if ( bpack.ConsumeTotal( typeof( Gold ), BaseVerite * priceAdjustment) )
                        {
                           weapon.Resource = CraftResource.Verite;
                           from.SendMessage( "Your weapon has been transmuted." );
                        }
                        else from.SendMessage( "You do not have enough gold. \nYou need {0} in gold to transmute that item.",BaseVerite * priceAdjustment );
                     }
                     break;
                  }
                  case 9:
                  {
                     if( from.Skills[SkillName.Alchemy].Value < MinAlchemySkill * ValoriteSkill )
                     {
                        from.SendMessage( "Your alchemy skill is not high enough to transmute that item." );
                        return;
                     }

                     if( weapon.Resource == CraftResource.Valorite)
                     {
                        from.SendMessage( "That item is already made of valorite." );
                     }
                     else
                     {
                        if ( bpack.ConsumeTotal( typeof( Gold ), BaseValorite * priceAdjustment) )
                        {
                           weapon.Resource = CraftResource.Valorite;
                           from.SendMessage( "Your weapon has been transmuted." );
                        }
                        else from.SendMessage( "You do not have enough gold. \nYou need {0} in gold to transmute that item.",BaseValorite * priceAdjustment );
                     }
                     break;
                  }
                  default:
                  {
                     from.SendMessage( "Transmutation was aborted." );
                     break;
                  }
               }
            }
         }
      }

      public override void Serialize( GenericWriter writer )
      {
         base.Serialize( writer );
         writer.Write( (int) 0 ); // version
         writer.Write((bool)m_IsRewardItem);
      }

      public override void Deserialize( GenericReader reader )
      {
         base.Deserialize( reader );
         int version = reader.ReadInt();
         m_IsRewardItem = reader.ReadBool();
      }
   }
}
namespace Server.Gumps
{
	public class TStoneGump : Gump
	{
		private Item m_Item;

		public void AddButtonLabel( int x, int y, int buttonID, string text )
		{
			AddButton( x, y - 1, 4005, 4007, buttonID, GumpButtonType.Reply, 0 );
			AddLabel( x + 35, y, 0, text );
		}

		public TStoneGump( Item item ) : base(0,0)
		{
			m_Item = item;
			Closable = false;
			Dragable = true;

			AddPage(0);

			AddBackground( 0, 0, 215, 180, 5054);
			AddBackground( 10, 10, 195, 160, 3000);
			AddLabel( 20, 40, 0, "Do you wish to re-deed this");
			AddLabel( 20, 60, 0, "transmutation stone?");
			AddButtonLabel( 20, 110, 1, "CONTINUE" );
			AddButtonLabel( 20, 135, 0, "CANCEL" );
		}

		public override void OnResponse( Server.Network.NetState sender, RelayInfo info )
		{
			if (info.ButtonID == 1)
			{
				m_Item.Delete();
				sender.Mobile.AddToBackpack( new TransmutationStoneDeed() );
			}
		}
	}
}

