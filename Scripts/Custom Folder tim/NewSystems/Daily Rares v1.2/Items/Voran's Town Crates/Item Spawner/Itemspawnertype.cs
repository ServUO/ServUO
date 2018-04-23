using System; 
using System.Collections; 
using Server; 

namespace Server.Items 
{ 
   public class ItemSpawnerType 
   { 
      public static Type GetType( string name ) 
      { 
         return ScriptCompiler.FindTypeByName( name ); 
      } 
   } 
} 
