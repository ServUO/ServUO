-Random Item Generator: http://www.uoguide.com/Random_Magic_Item_Generation_System

-This is active for ALL loot on EA servers, however is not active by default.  Here is how to activate it. All edits will be in RandomItemGenerator.cs.

-To drop items via random item generator system on specific monsters, such as hard creatures, etc, you will need to create a drop entry for that creature type in the Initialize() Method.

For example:

m_Table[typeof(Server.Engines.Despise.AndrosTheDreadLord)] = new List<DropEntry>();
m_Table[typeof(Server.Engines.Despise.AndrosTheDreadLord)].Add(new DropEntry(null, 50, 15));

This will give a 50% drop rate, rolled 15 times when Adros the Dreadlord dies. This is in addition to his normal lootpack.

-To drop items via random item generator system as regular loot from loot packs, you will need to edit GenerateRandomItem(Item item, Mobile killer, BaseCreature victim) method.

For example:

GenerateRandomItem(Item item, Mobile killer, BaseCreature victim)
{
   if(victim != null && victim.Map == Map.Felucca && .10 > Utility.RandomDouble())
     return RunicReforging.GenerateRandomItem(item, killer, victim);

   if(victim.Region != null && victim.Region.IsPartOf<DespiseRegion>())
      return RunicReforging.GenerateRandomItem(item, killer, victim);
}

this will drop the new named items 10% of the time as regular loot in Felucca, and everytime as loot in DespiseRegion.