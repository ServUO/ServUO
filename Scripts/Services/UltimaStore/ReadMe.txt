Configuration File: Scripts/Services/UltimaStore/SystemConfig.cs

Recommend you use custom currency when using this system. EA uses sovreigns which come to about 5,000 for 49.95 US dollars. This is a good base on how to value the items. The item cost in the store are per EA, so careful attention should be made when balancing out your currency system, and how that currency will be obtained. Flooding your market with some of these items will not be a good idea.

Also, if you're like me, and hate the flashy hues, you can comment out the store entries in UltimaStore.cs to your liking. 

Currently, the system supports Gold or points system currency. For a list of various points systems, go to PointsSystems.cs where a solid list can be viewed. If you use a custom currency [recommended], you will need to modify the following functions in SystemConfig.cs:

public static double GetCustomCurrency(Mobile m)

- this needs to return the currency for that player. THis is to ensure they have sufficient currency before an item can be purchased.

public static void DeductCustomCurrecy(Mobile m)

- This ensures that the currency points are actually deducted after the sale.  It's a good idea to get this right.

CurrencyType description:

None - disables the system
Sovereigns - Sovereigns, added as a seperate account currency, will be up to the shard owners how to be implemented.
Gold - uses standard gold currency. If you use this, I would suggest your increase the PointMultiplier, significantly so your not flooding your market with these items
PointsSystem - using this, you will have to designate PointsSystemCurrency as to which points system your going to use. Check PointsSystem.cs for a list of all the points systems, ie Despise Crystals, Void Pool points, etc
Custom - see above on how to implement custom currency