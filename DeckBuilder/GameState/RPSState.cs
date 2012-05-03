using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DeckBuilder.Models;
using DeckBuilder.Protoplasm;

namespace DeckBuilder.Games
{
    public class RPSPlayerContext : PlayerContext
    {
        public List<string> moveHistory { get; set; }
        public Hand<GamePiece> hand { get; set; }
        public int wins { get; set; }
        public int score { get; set; }
        public bool eliminated { get; set; }
        
        public RPSPlayerContext()
        {
            moveHistory = new List<string>();
            hand = new Hand<GamePiece>("RPSHand");
            wins = 0;
            score = 0;
            eliminated = false;
            
        }

        public void MarkHand()
        {
            // Mark everything but selected item
            hand.ClearSelection();
            for (int i = 0; i < hand.cards.Count; i++)
            {
                if (eliminated == true)
                {
                    hand.cards[i].selectable = false;
                    hand.cards[i].highlightUrl = "/content/images/classic/tile_shade.png";
                }
                else if (i != hand.selectedIndex)
                {
                    hand.cards[i].selectable = true;
                    if (hand.selectedIndex != -1)
                        hand.cards[i].highlightUrl = "/content/images/classic/tile_shade.png";
                 }
            }
        }
    }

    public class RPSState : GameState<RPSPlayerContext>
    {
        public override System.Runtime.Serialization.Json.DataContractJsonSerializer GetSerializer()
        {
            return new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(RPSState), new Type[] { typeof(RPSPlayerContext), typeof(PlayerContext), typeof(GameObject), typeof(GameUpdate), typeof(GameView), typeof(SquareBoard<ConvoyPiece>), typeof(GamePiece), typeof(SquareTile<ConvoyPiece>), typeof(Hand<GamePiece>), typeof(Image), typeof(TextBox) });
        }

        public RPSState()
        {
            playerContexts = new List<RPSPlayerContext>();
        }

        public override void InitializeState(List<Seat> seats)
        {
            base.InitializeState(seats);
            foreach(RPSPlayerContext context in playerContexts)
            {
                // Set up hand of 3 cards
                context.hand.Add("Rock", "/content/images/classic/rock.png");
                context.hand.Add("Paper", "/content/images/classic/paper.png");
                context.hand.Add("Scissors", "/content/images/classic/scissors.png");

                // Mark Hands
                context.MarkHand();
            }
        }

        public int CalculateScore(string move, string opponentMove)
        {
            if (move == "Eliminated" || opponentMove == "Eliminated")
                return 0;
            if (move == "Rock" && opponentMove == "Scissors" || move == "Scissors" && opponentMove == "Paper" || move == "Paper" && opponentMove == "Rock")
            {
                return 1;
            }
            else if (move != opponentMove)
            {
                return -1;
            }
            return 0;
        }

        public override void Update(GameUpdate update)
        {
            bool finished = true;
            
            // Select card from active hand.

            playerContexts.Single(pc => pc.playerId == update.playerId).hand.Select(update.selectIndex);
            playerContexts.Single(pc => pc.playerId == update.playerId).MarkHand();

            foreach (RPSPlayerContext context in playerContexts)
            {
                if (context.hand.SelectedCard == null && context.eliminated == false)
                    finished = false;
            }
            
            if (finished)
            {
                if (this.playerContexts.Count == 2)
                {
                    string p1move = this.playerContexts[0].hand.SelectedCard.name;
                    string p2move = this.playerContexts[1].hand.SelectedCard.name;
                    this.playerContexts[0].moveHistory.Add(p1move);
                    this.playerContexts[1].moveHistory.Add(p2move);
                    logs.Add(this.playerContexts[0].name + " plays " + p1move + ".");
                    logs.Add(this.playerContexts[1].name + " plays " + p2move + ".");

                    if (p1move == p2move)
                    {
                        // Draw
                        logs.Add("Draw!");

                    }
                    else if (p1move == "Rock" && p2move == "Scissors" || p1move == "Scissors" && p2move == "Paper" || p1move == "Paper" && p2move == "Rock")
                    {
                        // P1 wins
                        logs.Add(p1move + " wins! " + this.playerContexts[0].name + " scores a point!");
                        this.playerContexts[0].wins++;
                    }
                    else
                    {
                        // P2 wins
                        logs.Add(p2move + " wins! " + this.playerContexts[1].name + " scores a point!");
                        this.playerContexts[1].wins++;
                    }
                }
                else
                {
                    // Calculate scores
                    int bestScore = 0;
                    foreach (RPSPlayerContext context in playerContexts)
                    {
                        if (context.eliminated == false)
                        {
                            context.score = 0;
                            string move = context.hand.SelectedCard.name;
                            context.moveHistory.Add(move);
                            foreach (RPSPlayerContext opponentContext in playerContexts)
                            {
                                if (opponentContext.eliminated == false)
                                {
                                    string opponentMove = opponentContext.hand.SelectedCard.name;
                                    context.score += CalculateScore(move, opponentMove);                                    
                                }
                            }
                            logs.Add(context.name + " played " + move + " for a score of " + context.score + ".");
                            if (context.score > bestScore)
                                bestScore = context.score;
                        }
                        else
                        {
                            context.moveHistory.Add("Eliminated");
                        }
                    }
                    // Count players remaining
                    int remainingPlayersCount = 0;
                    foreach (RPSPlayerContext context in playerContexts)
                    {
                        if (context.eliminated || context.score < bestScore)
                        {
                            logs.Add(context.name + " is eliminated.");
                            context.eliminated = true;
                        }
                        else
                            remainingPlayersCount++;
                    }
                    // Check for winner and reset elimination
                    if (remainingPlayersCount == 1)
                    {
                        foreach (RPSPlayerContext context in playerContexts)
                        {
                            if (context.eliminated == false)
                            {
                                logs.Add(context.name + " wins!");
                                context.wins++;
                            }
                            context.eliminated = false;
                        }
                    }
                }

                foreach (RPSPlayerContext context in playerContexts)
                {
                    context.hand.ClearSelection();
                    context.hand.Deselect();
                    context.MarkHand();
                }
            }
            foreach (RPSPlayerContext context in playerContexts)
            {
                if (context.wins == 3)
                {
                    gameOver = true;

                    logs.Add(context.name + " wins!");                    
                }
            }
        }

        public override GameView GetClientView(int playerId)
        {
            // View Consists of player hand + scoreboard

            GameView view = new GameView();
            view.activePlayerId = playerId;
            view.tableId = tableId;
            view.logs = logs;

            RPSPlayerContext viewContext = playerContexts.Single(pc => pc.playerId == playerId);
            view.drawList.Add(viewContext.hand.View(100, 100, 100, 100));

            // Scoreboard
            for(int i = 0; i < playerContexts.Count; i++)
            {
                // Column headers (names)                
                view.drawList.Add(new TextBox(600 + i * 80, 50, playerContexts[i].name));
                
                // Current Move
                if (playerContexts[i].eliminated == true)
                {
                    view.drawList.Add(new Image(600 + i * 80, 80, "/content/images/classic/eliminated.png"));                    
                }
                else if (playerContexts[i].hand.SelectedCard == null)
                {
                    view.drawList.Add(new Image(600 + i * 80, 80, "/content/images/classic/dash.png"));
                }
                else if (playerContexts[i].playerId != playerId)
                {
                    view.drawList.Add(new Image(600 + i * 80, 80, "/content/images/classic/mystery.png"));                    
                }
                else
                {
                    view.drawList.Add(new Image(600 + i * 80, 80, "/content/images/classic/" + playerContexts[i].hand.SelectedCard.name + ".png"));
                }

                for (int j = 0; j < playerContexts[i].moveHistory.Count; j++)
                {
                    view.drawList.Add(new Image(600 + i * 80, 140 + 50 * j, "/content/images/classic/" + playerContexts[i].moveHistory[playerContexts[i].moveHistory.Count - j -1] + ".png"));                    
                }
            }

            // Last Round Summary
            if (playerContexts.Count > 2 && playerContexts[0].moveHistory.Count > 0)
            {
                for(int i =0; i < playerContexts.Count; i++)
                {
                    RPSPlayerContext context = playerContexts[i];
                    string move = playerContexts[i].moveHistory.Last();
                    view.drawList.Add(new TextBox(50, 250+50*i, playerContexts[i].name));
                    view.drawList.Add(new Image(100, 250+50*i, "/content/images/classic/" + move + ".png"));
                    view.drawList.Add(new TextBox(150, 250 + 50*i, "vs"));

                    int score = 0;
                    for (int j = 0; j < playerContexts.Count; j++)
                    {
                        RPSPlayerContext opponentContext = playerContexts[j];
                        string opponentMove = playerContexts[j].moveHistory.Last();
                        score += CalculateScore(move, opponentMove);
                        if (i != j)
                            view.drawList.Add(new Image(200 + 50 * j, 250 + 50 * i, "/content/images/classic/" + opponentMove + ".png"));
                        else
                            view.drawList.Add(new Image(200 + 50 * j, 250 + 50 * i, "/content/images/classic/dash.png"));
                    }
                    view.drawList.Add(new TextBox(250 + 50 * playerContexts.Count, 250 + 50 * i, ""+score));
                }
            }

            return view;
        }
    }
}