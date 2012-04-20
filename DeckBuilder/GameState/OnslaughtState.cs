using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DeckBuilder.Models;

namespace DeckBuilder.Games
{
    public class GalaxyCard
    {
        public int crystalCost_Buy { get; set; }
        public int metalCost_Buy { get; set; }
        public int powerCost_Buy { get; set; }
        public int crystalCost_Play { get; set; }
        public int metalCost_Play { get; set; }
        public int powerCost_Play { get; set; }
        public string description { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public string url { get; set; }

        public bool validTarget { get; set; }
    }

    public class SupplyPile
    {
        public GalaxyCard card { get; set; }
        public int quantity { get; set; }

        public bool validTarget { get; set; }
    }

    public class InvasionCard
    {
        public int invasionLevel { get; set; }
        public string description { get; set; }
        public string name { get; set; }
        public string url { get; set; }
    }    

    public class InvaderToken
    {
        public int strength { get; set; }
        public int speed { get; set; }
        public string url { get; set; }
        public string description { get; set; }
        public string name { get; set; }

        public bool validTarget { get; set; }
    }

    public class OnslaughtUpdate
    {
        public int playerId { get; set; }
        public string type { get; set; }
        
        // Choices
        public GalaxyCard card { get; set; } // Single card chosen (hand or supply)

        // Enemy ship on system track
        public int systemTrackSegmentChoice { get; set; }
        public int systemTrackTokenChoice { get; set; }

        // List of cards
        public List<GalaxyCard> cardChoices { get; set; }
        
    }

    public class OnslaughtPlayerContext
    {
        public List<List<InvaderToken>> systemStrip { get; set; }
        public List<GalaxyCard> hand { get; set; }
        public List<GalaxyCard> deck { get; set; }
        public List<GalaxyCard> discard { get; set; }
        public List<GalaxyCard> playArea { get; set; }
        public int metal { get; set; }
        public int crystal { get; set; }
        public int power { get; set; }
        public int playerId { get; set; }
        public string state { get; set; }


        public GalaxyCard DrawCard(Random r)
        {
            if (deck.Count == 0)
            {
                deck = discard;
                discard = new List<GalaxyCard>();
            }
            if (deck.Count == 0)
                return null;
            int randomIndex = r.Next(0, deck.Count);
            GalaxyCard card = deck.ElementAt(randomIndex);
            deck.RemoveAt(randomIndex);
            return card;            
        }
    }

    public class OnslaughtState
    {
        public List<OnslaughtPlayerContext> playerContexts { get; set; }
        public List<GalaxyCard> trashPile { get; set; }
        public List<InvasionCard> invasionDeck { get; set; }
        public List<InvasionCard> invasionDiscard { get; set; }
        public List<SupplyPile> supplyPiles { get; set; }
        public int invasionLevel { get; set; }

        public int sourcePlayerId { get; set; }
        public int activePlayerId { get; set; }
        public int activePlayerIndex { get; set; }
        public int tableId { get; set; }        

        public void InitializeState(List<Seat> seats)
        {
            tableId = seats[0].TableId;

            trashPile = new List<GalaxyCard>();
            invasionDiscard = new List<InvasionCard>();
            invasionDeck = new List<InvasionCard>();
            supplyPiles = new List<SupplyPile>();
            

            playerContexts = new List<OnslaughtPlayerContext>();

            #region galaxy card definitions
            
            GalaxyCard miner = new GalaxyCard();
            miner.crystalCost_Buy = 1;
            miner.metalCost_Buy = 0;
            miner.powerCost_Play = 0;
            miner.name = "Stardust Collector";
            miner.description = "+1 Metal";
            miner.type = "Miner";
            miner.url = "/content/images/onslaught/miner.png";

            GalaxyCard asteroidMiner = new GalaxyCard();
            asteroidMiner.crystalCost_Buy = 1;
            asteroidMiner.metalCost_Buy = 3;
            asteroidMiner.powerCost_Play = 0;
            asteroidMiner.name = "Asteroid Miner";
            asteroidMiner.description = "+2 Metal";
            asteroidMiner.type = "Miner";
            asteroidMiner.url = "/content/images/onslaught/asteroidminer.png";

            GalaxyCard miningColony = new GalaxyCard();
            miningColony.crystalCost_Buy = 1;
            miningColony.metalCost_Buy = 6;
            miningColony.powerCost_Play = 0;
            miningColony.name = "Mining Colony";
            miningColony.description = "+3 Metal";
            miningColony.type = "Miner";
            miningColony.url = "/content/images/onslaught/miningcolony.png";


            GalaxyCard fighter = new GalaxyCard();
            fighter.crystalCost_Buy = 1;
            fighter.metalCost_Buy = 2;
            fighter.powerCost_Play = 0;
            fighter.name = "Fighter Squadron";
            fighter.description = "Defense Rating: 1";
            fighter.type = "Fleet";
            fighter.url = "/content/images/onslaught/fighter.png";

            GalaxyCard frigate = new GalaxyCard();
            frigate.crystalCost_Buy = 1;
            frigate.metalCost_Buy = 5;
            frigate.powerCost_Play = 0;
            frigate.name = "Assault Frigate";
            frigate.description = "Defense Rating: 3";
            frigate.type = "Fleet";
            frigate.url = "/content/images/onslaught/frigate.png";

            GalaxyCard cruiser = new GalaxyCard();
            cruiser.crystalCost_Buy = 1;
            cruiser.metalCost_Buy = 8;
            cruiser.powerCost_Play = 0;
            cruiser.name = "Heavy Cruiser";
            cruiser.description = "Defense Rating: 6";
            cruiser.type = "Fleet";
            cruiser.url = "/content/images/onslaught/heavycruiser.png";

            GalaxyCard fleetBeacon = new GalaxyCard();
            fleetBeacon.crystalCost_Buy = 1;
            fleetBeacon.metalCost_Buy = 4;
            fleetBeacon.powerCost_Play = 1;
            fleetBeacon.name = "Fleet Beacon";
            fleetBeacon.description = "+3 Cards";
            fleetBeacon.type = "Tech";
            fleetBeacon.url = "/content/images/onslaught/fleetBeacon.png";

            GalaxyCard jumpGate = new GalaxyCard();
            jumpGate.crystalCost_Buy = 1;
            jumpGate.metalCost_Buy = 5;
            jumpGate.powerCost_Play = 1;
            jumpGate.name = "Jump Gate";
            jumpGate.description = "+1 Power, +2 Cards";
            jumpGate.type = "Tech";
            jumpGate.url = "/content/images/onslaught/jumpgate.png";

            GalaxyCard crystalExtractor = new GalaxyCard();
            crystalExtractor.crystalCost_Buy = 1;
            crystalExtractor.metalCost_Buy = 3;
            crystalExtractor.powerCost_Play = 1;
            crystalExtractor.name = "Crystal Extractor";
            crystalExtractor.description = "+2 Steel, +1 Crystal";
            crystalExtractor.type = "Tech";
            crystalExtractor.url = "/content/images/onslaught/crystalextractor.png";

            GalaxyCard refinery = new GalaxyCard();
            refinery.crystalCost_Buy = 1;
            refinery.metalCost_Buy = 5;
            refinery.powerCost_Play = 1;
            refinery.name = "Refinery Platform";
            refinery.description = "+2 Power, +2 Steel, +1 Crystal";
            refinery.type = "Tech";
            refinery.url = "/content/images/onslaught/refinery.png";

            GalaxyCard solarArray = new GalaxyCard();
            solarArray.crystalCost_Buy = 2;
            solarArray.metalCost_Buy = 3;
            solarArray.powerCost_Play = 1;
            solarArray.name = "Solar Array";
            solarArray.description = "+2 Power, +1 Card";
            solarArray.type = "Tech";
            solarArray.url = "/content/images/onslaught/solararray.png";


            GalaxyCard planetaryCannon = new GalaxyCard();
            planetaryCannon.crystalCost_Buy = 1;
            planetaryCannon.metalCost_Buy = 4;
            planetaryCannon.powerCost_Play = 0;
            planetaryCannon.name = "Planetary Cannon";
            planetaryCannon.description = "Destroy most powerful invader in homeworld orbit. +1 to Invasion Level";
            planetaryCannon.type = "Tech";
            planetaryCannon.url = "/content/images/onslaught/planetarycannon.png";

            GalaxyCard disrupterBeam = new GalaxyCard();
            disrupterBeam.crystalCost_Buy = 1;
            disrupterBeam.metalCost_Buy = 5;
            disrupterBeam.powerCost_Play = 0;
            disrupterBeam.name = "Disrupter Beam";
            disrupterBeam.description = "+2 Cards, Opponents gain Space Junk";
            disrupterBeam.type = "Tech";
            disrupterBeam.url = "/content/images/onslaught/disrupter.png";

            GalaxyCard spaceJunk = new GalaxyCard();
            spaceJunk.crystalCost_Buy = 1;
            spaceJunk.metalCost_Buy = 0;
            spaceJunk.powerCost_Play = 0;
            spaceJunk.name = "Space Junk";
            spaceJunk.description = "Worthless Space Junk";
            spaceJunk.type = "Junk";
            spaceJunk.url = "/content/images/onslaught/spacejunk.png";
            
            #endregion

            #region invasion card definitions
            InvasionCard scout = new InvasionCard();
            scout.invasionLevel = 0;
            scout.name = "Scout";
            scout.description = "Assault Level: 1, Speed: 3";
            scout.url = "/content/images/onslaught/scout.png";

            InvasionCard marauder = new InvasionCard();
            marauder.invasionLevel = 0;
            marauder.name = "Marauder";
            marauder.description = "Assault Level: 3, Speed: 2";
            marauder.url = "/content/images/onslaught/marauder.png";

            InvasionCard lull = new InvasionCard();
            lull.invasionLevel = 0;
            lull.name = "Lull";
            lull.description = "Nothing Happens";
            lull.url = "/content/images/onslaught/lull.png";

            InvasionCard harbinger = new InvasionCard();
            harbinger.invasionLevel = 0;
            harbinger.name = "Harbinger";
            harbinger.description = "Assault Level: 2, Speed: 2, +1 Invasion Level";
            harbinger.url = "/content/images/onslaught/harbinger.png";

            InvasionCard destroyer = new InvasionCard();
            destroyer.invasionLevel = 3;
            destroyer.name = "Destroyer";
            destroyer.description = "Assult Level: 6, Speed: 1";
            destroyer.url = "/content/images/onslaught/destroyer.png";

            InvasionCard squadron = new InvasionCard();
            squadron.invasionLevel = 3;
            squadron.name = "Fighter Squadron";
            squadron.description = "Assault Level: 1, Speed: 3, Quantity: 3";
            squadron.url = "/content/images/onslaught/scoutsquadron.png";

            InvasionCard marauderSquad = new InvasionCard();
            marauderSquad.invasionLevel = 6;
            marauderSquad.name = "Marauder Squadron";
            marauderSquad.description = "Assault Level: 3, Speed: 2, Quantity: 2";
            marauderSquad.url = "/content/images/onslaught/maraudersquad.png";

            InvasionCard deathRay = new InvasionCard();
            deathRay.invasionLevel = 6;
            deathRay.name = "Death Ray";
            deathRay.description = "Assault Level: 8, Speed: 2, Destroyed by any ship";
            deathRay.url = "/content/images/onslaught/deathray.png";

            InvasionCard destroyerFleet = new InvasionCard();
            destroyerFleet.invasionLevel = 9;
            destroyerFleet.name = "Destroyer Fleet";
            destroyerFleet.description = "Assult Level: 6, Speed: 1, Quantity: 2";
            destroyerFleet.url = "/content/images/onslaught/destroyerfleet.png";

            invasionDeck.Add(scout);
            invasionDeck.Add(scout);
            invasionDeck.Add(marauder);
            invasionDeck.Add(marauder);
            invasionDeck.Add(lull);
            invasionDeck.Add(harbinger);
            invasionDeck.Add(destroyer);
            invasionDeck.Add(destroyer);
            invasionDeck.Add(squadron);
            invasionDeck.Add(marauderSquad);
            invasionDeck.Add(marauderSquad);
            invasionDeck.Add(deathRay);
            invasionDeck.Add(destroyerFleet);
            invasionDeck.Add(destroyerFleet);
            invasionDeck.Add(destroyerFleet);
            #endregion
            
            supplyPiles.Add(new SupplyPile { card = miner, quantity = 60 });
            supplyPiles.Add(new SupplyPile { card = asteroidMiner, quantity = 60 });
            supplyPiles.Add(new SupplyPile { card = miningColony, quantity = 60 });
            supplyPiles.Add(new SupplyPile { card = fighter, quantity = 20 });
            supplyPiles.Add(new SupplyPile { card = frigate, quantity = 20 });
            supplyPiles.Add(new SupplyPile { card = cruiser, quantity = 20 });
            supplyPiles.Add(new SupplyPile { card = crystalExtractor, quantity = 10 });
            supplyPiles.Add(new SupplyPile { card = fleetBeacon, quantity = 10 });
            supplyPiles.Add(new SupplyPile { card = jumpGate, quantity = 10 });
            supplyPiles.Add(new SupplyPile { card = refinery, quantity = 10 });
            supplyPiles.Add(new SupplyPile { card = solarArray, quantity = 10 });
            supplyPiles.Add(new SupplyPile { card = disrupterBeam, quantity = 10 });
            supplyPiles.Add(new SupplyPile { card = planetaryCannon, quantity = 10 });
            supplyPiles.Add(new SupplyPile { card = spaceJunk, quantity = 60 });

            invasionLevel = 0;
            
            

            Random r = new Random();

            activePlayerIndex = r.Next(seats.Count);
            activePlayerId = seats[activePlayerIndex].PlayerId;

            foreach (Seat seat in seats)
            {
                OnslaughtPlayerContext playerContext = new OnslaughtPlayerContext();
                playerContext.playerId = seat.PlayerId;
                playerContext.state = "Normal";
                playerContext.crystal = 0;
                playerContext.power = 0;
                playerContext.metal = 0;
                playerContext.systemStrip = new List<List<InvaderToken>>();
                for (int i = 0; i < 10; i++)
                {
                    playerContext.systemStrip.Add(new List<InvaderToken>());
                }

                playerContext.deck = new List<GalaxyCard>();
                for (int i = 0; i < 7; i++)
                {
                    playerContext.deck.Add(miner);
                }
                for (int i = 0; i < 3; i++)
                {
                    playerContext.deck.Add(fighter);
                }
               
                playerContext.hand = new List<GalaxyCard>();

                for (int i = 0; i < 5; i++)
                {
                    playerContext.hand.Add(playerContext.DrawCard(r));                    
                }

                playerContext.playArea = new List<GalaxyCard>();
                playerContext.discard = new List<GalaxyCard>();
                playerContext.power = 1;
                playerContext.crystal = 1;

                DrawNextInvasionCard(r);

                playerContexts.Add(playerContext);
            }
        }

        public void DrawNextInvasionCard(Random r)
        {
            while(true)
            {
                if (invasionDeck.Count == 0)
                {
                    invasionLevel++;
                    invasionDeck = invasionDiscard;
                    invasionDiscard = new List<InvasionCard>();
                }

                int randomIndex = r.Next(0, invasionDeck.Count);
            
                InvasionCard card = invasionDeck.ElementAt(randomIndex);
                invasionDeck.RemoveAt(randomIndex);
                invasionDiscard.Add(card);
                if (card.invasionLevel <= invasionLevel)
                {
                    foreach(OnslaughtPlayerContext context in playerContexts)
                    {
                        if (card.name == "Scout")
                        {
                            InvaderToken invader = new InvaderToken { strength = 1, speed = 3, url = "/content/images/onslaught/scout.png", name= "Fighter", description = "Assault Rating: 1, Speed: 3" };
                            context.systemStrip[0].Add(invader);
                        }
                        if (card.name == "Fighter Squadron")
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                InvaderToken invader = new InvaderToken { strength = 1, speed = 3, url = "/content/images/onslaught/scout.png", name= "Fighter", description = "Assault Rating: 1, Speed: 3" };
                                context.systemStrip[0].Add(invader);
                            }
                        }
                        if (card.name == "Harbinger")
                        {
                            InvaderToken invader = new InvaderToken { strength = 2, speed = 2, url = "/content/images/onslaught/harbinger.png", name = "Harbinger", description = "Assault Rating: 2, Speed: 2" };
                            context.systemStrip[0].Add(invader);
                            invasionLevel++;
                        }
                        if (card.name == "Marauder")
                        {
                            InvaderToken invader = new InvaderToken { strength = 3, speed = 2, url = "/content/images/onslaught/marauder.png", name = "Marauder", description = "Assault Rating: 3, Speed: 2" };
                            context.systemStrip[0].Add(invader);
                        }
                        if (card.name == "Marauder Squadron")
                        {
                            InvaderToken invader = new InvaderToken { strength = 3, speed = 2, url = "/content/images/onslaught/marauder.png", name = "Marauder", description = "Assault Rating: 3, Speed: 2" };
                            context.systemStrip[0].Add(invader);
                            InvaderToken invader2 = new InvaderToken { strength = 3, speed = 2, url = "/content/images/onslaught/marauder.png", name = "Marauder", description = "Assault Rating: 3, Speed: 2" };
                            context.systemStrip[0].Add(invader2);
                        }
                        if (card.name == "Destroyer")
                        {
                            InvaderToken invader = new InvaderToken { strength = 6, speed = 1, url = "/content/images/onslaught/destroyer.png", name = "Destroyer", description = "Assault Rating: 6, Speed: 1" };
                            context.systemStrip[0].Add(invader);
                        }
                        if (card.name == "Destroyer Fleet")
                        {
                            InvaderToken invader = new InvaderToken { strength = 6, speed = 1, url = "/content/images/onslaught/destroyer.png", name = "Destroyer", description = "Assault Rating: 6, Speed: 1" };
                            context.systemStrip[0].Add(invader);
                            InvaderToken invader2 = new InvaderToken { strength = 6, speed = 1, url = "/content/images/onslaught/destroyer.png", name = "Destroyer", description = "Assault Rating: 6, Speed: 1" };
                            context.systemStrip[0].Add(invader2);
                        }
                        if (card.name == "Death Ray")
                        {
                            InvaderToken invader = new InvaderToken { strength = 6, speed = 1, url = "/content/images/onslaught/deathray.png", name = "Death Ray", description = "Assault Rating: 6, Speed: 1, Loses vs any ship." };
                            context.systemStrip[0].Add(invader);
                        }
                    }

                    return;
                }
            }
        }

        public void SetShipTargets(OnslaughtPlayerContext context)
        {
            context.state = "Choice";
            for (int i = 0; i < context.systemStrip.Count; i++)
            {
                for (int j = 0; j < context.systemStrip[i].Count; j++)
                {
                    context.systemStrip[i][j].validTarget = true;
                }
            }
            for (int i = 0; i < context.hand.Count; i++)
            {
                context.hand[i].validTarget = false;
            }
            for (int i = 0; i < supplyPiles.Count; i++)
            {
                supplyPiles[i].validTarget = false;
            }
        }

        public void SetHandTargets(OnslaughtPlayerContext context)
        {
            context.state = "Choice";
            for (int i = 0; i < context.systemStrip.Count; i++)
            {
                for (int j = 0; j < context.systemStrip[i].Count; j++)
                {
                    context.systemStrip[i][j].validTarget = false;
                }
            }
            for (int i = 0; i < context.hand.Count; i++)
            {
                context.hand[i].validTarget = true;
            }
            for (int i = 0; i < supplyPiles.Count; i++)
            {
                supplyPiles[i].validTarget = false;
            }
        }

        public void SetSupplyTargets(OnslaughtPlayerContext context)
        {
            context.state = "Choice";
            for (int i = 0; i < context.systemStrip.Count; i++)
            {
                for (int j = 0; j < context.systemStrip[i].Count; j++)
                {
                    context.systemStrip[i][j].validTarget = false;
                }
            }
            for (int i = 0; i < context.hand.Count; i++)
            {
                context.hand[i].validTarget = false;
            }
            for (int i = 0; i < supplyPiles.Count; i++)
            {
                supplyPiles[i].validTarget = true;
            }
        }

        public void Update(OnslaughtUpdate update)
        {
            if (activePlayerId != update.playerId)
                return;

            Random r = new Random();
            for (int playerIndex = 0; playerIndex < playerContexts.Count; playerIndex++)
            {
                OnslaughtPlayerContext context = playerContexts[playerIndex];
                if (context.playerId == update.playerId)
                {
                    if (update.type == "Choice")
                    {
                        GalaxyCard lastPlayedCard = context.playArea[context.playArea.Count - 1];
                        if (lastPlayedCard.type == "Fleet")
                        {
                            InvaderToken targetToken = context.systemStrip[update.systemTrackSegmentChoice][update.systemTrackTokenChoice];
                            int fleetStrength = 0;
                            if (lastPlayedCard.name == "Heavy Cruiser")
                                fleetStrength = 6;
                            if (lastPlayedCard.name == "Fighter Squadron")
                                fleetStrength = 1;
                            if (lastPlayedCard.name == "Assault Frigate")
                                fleetStrength = 3;

                            // Remove invader from current position
                            context.systemStrip[update.systemTrackSegmentChoice].RemoveAt(update.systemTrackTokenChoice);
                            if (fleetStrength >= targetToken.strength || targetToken.name == "Death Ray")
                            {
                                // Send invader to opponent track
                                playerContexts[(playerIndex + 1) % playerContexts.Count].systemStrip[0].Add(targetToken);                                
                            }
                            else
                            {
                                // Push invader back 2 turns
                                int destinationSegment = update.systemTrackSegmentChoice - targetToken.speed * 2;
                                if (destinationSegment < 0) destinationSegment = 0;
                                context.systemStrip[destinationSegment].Add(targetToken);
                            }

                            // Destroy fleet card if loss
                            if (fleetStrength <= targetToken.strength && targetToken.name != "Death Ray")
                            {
                                context.playArea.RemoveAt(context.playArea.Count - 1);
                            }
                            context.state = "Normal";
                        }
                    }
                    if (update.type == "Play" && context.power >= update.card.powerCost_Play && context.metal >= update.card.metalCost_Play && context.crystal >= update.card.crystalCost_Play)
                    {
                        #region playcard
                        context.power -= update.card.powerCost_Play;
                        context.metal -= update.card.metalCost_Play;
                        context.crystal -= update.card.crystalCost_Play;
                        if (update.card.name == "Stardust Collector")
                        {
                            context.metal += 1;
                        }
                        if (update.card.name == "Asteroid Miner")
                        {
                            context.metal += 2;
                        }
                        if (update.card.name == "Mining Colony")
                        {
                            context.metal += 3;
                        }
                        if (update.card.name == "Solar Array")
                        {
                            context.power += 2;
                        }
                        if (update.card.name == "Crystal Extractor")
                        {
                            context.metal += 2;
                            context.crystal += 1;
                        }
                        if (update.card.name == "Fleet Beacon")
                        {
                            context.hand.Add(context.DrawCard(r));
                            context.hand.Add(context.DrawCard(r));
                            context.hand.Add(context.DrawCard(r));
                        }
                        if (update.card.name == "Jump Gate")
                        {
                            context.hand.Add(context.DrawCard(r));
                            context.hand.Add(context.DrawCard(r));
                            context.power += 1;
                        }
                        if (update.card.name == "Refinery Platform")
                        {
                            context.power += 2;
                            context.metal += 2;
                            context.crystal += 1;
                        }
                        if (update.card.name == "Planetary Cannon")
                        {
                            context.power += 2;
                            context.metal += 2;
                            context.crystal += 1;
                        }
                        if (update.card.name == "Disrupter Beam")
                        {
                            context.hand.Add(context.DrawCard(r));
                            context.hand.Add(context.DrawCard(r));
                            foreach (OnslaughtPlayerContext opponent in playerContexts)
                            {
                                if (opponent.playerId != context.playerId)
                                {
                                    foreach (SupplyPile pile in supplyPiles)
                                    {
                                        if (pile.card.name == "Space Junk" && pile.quantity>0)
                                        {
                                            pile.quantity--;
                                            context.discard.Add(pile.card);
                                        }
                                    }                                    
                                }
                            }
                        }
                        if (update.card.type == "Fleet")
                        {
                            SetShipTargets(context);
                        }
                        GalaxyCard playedCard = context.hand.First(c => c.name == update.card.name);
                        context.hand.Remove(playedCard);
                        context.playArea.Add(playedCard);
                        #endregion
                    }
                    if (update.type == "Buy")
                    {
                        #region buycard
                        foreach (SupplyPile pile in supplyPiles)
                        {
                            if (pile.card.name == update.card.name && context.metal >= pile.card.metalCost_Buy && context.crystal >= pile.card.crystalCost_Buy && context.power >= pile.card.powerCost_Buy && pile.quantity > 0)
                            {
                                context.metal -= pile.card.metalCost_Buy;
                                context.crystal -= pile.card.crystalCost_Buy;
                                context.power -= pile.card.powerCost_Buy;
                                pile.quantity--;
                                context.discard.Add(pile.card);
                            }
                        }
                        #endregion
                    }

                    #region endofturn
                    bool endOfTurn = false;
  
                    if (update.type == "End")
                        endOfTurn = true;

                    if (endOfTurn == true)
                    {
                        context.metal = 0;
                        context.crystal = 1;
                        context.power = 1;
                        foreach (GalaxyCard playedCard in context.playArea)
                        {
                            context.discard.Add(playedCard);
                        }
                        foreach (GalaxyCard remainingCard in context.hand)
                        {
                            context.discard.Add(remainingCard);
                        }
                        context.playArea.Clear();
                        context.hand.Clear();
                        for (int i = 0; i < 5; i++)
                        {
                            context.hand.Add(context.DrawCard(r));
                        }

                        for (int i = 8; i >= 0; i--)
                        {
                            for (int j = 0; j < context.systemStrip[i].Count; j++)
                            {
                                InvaderToken token = context.systemStrip[i].First();
                                context.systemStrip[i].RemoveAt(0);
                                int destination = i + token.speed;
                                if (destination > 9) destination = 9;
                                context.systemStrip[destination].Add(token);
                            }                            
                        }
                        
                        DrawNextInvasionCard(r);
                        activePlayerIndex++;
                        activePlayerIndex %= playerContexts.Count;
                        activePlayerId = playerContexts[activePlayerIndex].playerId;
                    }
                    #endregion

                }
            }
            
        }

        public OnslaughtState GetClientState(int playerId)
        {
            OnslaughtState clientState = new OnslaughtState();
            clientState.sourcePlayerId = playerId;
            clientState.activePlayerId = activePlayerId;
            clientState.invasionLevel = invasionLevel;
            clientState.supplyPiles = supplyPiles;
            clientState.invasionDiscard = invasionDiscard;
            clientState.tableId = tableId;
            clientState.playerContexts = new List<OnslaughtPlayerContext>();            
            foreach (OnslaughtPlayerContext playerContext in playerContexts)
            {
                OnslaughtPlayerContext clientContext = new OnslaughtPlayerContext();
                clientContext.playerId = playerContext.playerId;
                if (playerContext.playerId == playerId)
                {
                    clientContext.hand = playerContext.hand;
                    clientContext.playArea = playerContext.playArea;
                    clientContext.discard = playerContext.discard;
                    clientContext.crystal = playerContext.crystal;
                    clientContext.metal = playerContext.metal;
                    clientContext.power = playerContext.power;
                    clientContext.state = playerContext.state;
                }
                clientContext.systemStrip = playerContext.systemStrip;
                clientState.playerContexts.Add(clientContext);
            }
            return clientState;
        }
    }


}