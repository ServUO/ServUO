Lotto Scratchers

All scratchers derive from BaseLottoTicket class.  You can adjust the odds in the ReturnOdds(...)
function of each ticket type.  Player luck effects Golden Ticket and Skies the Limit.  Player
Crafting skill (highest skill) effects Crazed Crafting.  Luck/craft skill bumps do not increase
the chace of a win, but increased the odds of a higher prize per scratch.

First, place LottoScratcher, this is the item that controls the stats, progressive jackpots, etc.
You can then place LottoScratcherSatellite that acts as the LottoScratcher item, like the PowerBallSatellite.

ScratcherLotto.cs -
DeleteTicketOnLoss defaulted to true.  When true, and you purchase a ticket as a quick scratch,
the ticket will auto delete if it's not a winner.

Golden Ticket
- Traditional lotto scratch ticket
- Prizes range from 1000 to 1,000,000
- 1200+ luck and a 50% chance odds will go in favor of higher prize
- 1800+ odds will go in favor of a higher price

Crazed Crafting
- This has a lower standard payout, however, wild cards can increase the chances for a win
- 2 of the same wild cards give a 2x reward multiplier
- 3 of the same wild cards, which always will result in a jackpot, give a 10x multiplier
- prizes range from 2,500 to 250,000, or 2,500,000 with a wildcard multiplier
- 8% flat chance to get a wildcard

Skies the Limit
- Progressive lotto scrather
- Progressive gold amount is saved in the ScratherLotto item
- Resets to 500,000 with a win 
- 3 Treasure chests win the progressive
- Normal payout is from 1,000 to 500,000

Stats reset every 90 days

