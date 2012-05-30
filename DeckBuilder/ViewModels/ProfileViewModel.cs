using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DeckBuilder.Models;

namespace DeckBuilder.ViewModels
{
    public class SeatViewModel
    {
        public String gameName;
        public String formattedOpponentNames;
        public String status;
        public int tableId;
        public String linkText;
        public bool cancellable;

        public SeatViewModel(Seat seat)
        {
            Table t = seat.Table;
            Game g = t.Game;

            tableId = t.TableID;
            gameName = g.Name;
            linkText = "View";
            if (seat.Waiting == true && (TableState)(t.TableState)==TableState.InPlay)
                linkText = "Your Move";
            if (seat.Waiting == true && (TableState)(t.TableState) == TableState.Proposed)
                linkText = "Accept";

            cancellable = false;
            if (seat.Accepted == false && (TableState)(t.TableState) == TableState.Proposed)
                cancellable = true;

            List<String> opponentNames = t.Seats.Where(s => s != seat).Select(s => s.Player.Name).ToList();
            formattedOpponentNames = opponentNames[0];
            if (opponentNames.Count > 1)
            {
                for (int i = 1; i < opponentNames.Count - 1; i++)
                {
                    formattedOpponentNames += ", " + opponentNames[i];
                }

                formattedOpponentNames += " and " + opponentNames.Last();
            }
            status = "(" + (TableState)(t.TableState) + ")";
            
        }
    }

    public class RecordViewModel
    {
        public String Name { get; set; }
        public int TotalGames { get; set; }
        public int Wins { get; set; }
        public int Draws { get; set; }
        public int Losses { get; set; }
        public int WinPercent { get; set; }

        public int RankedTotalGames { get; set; }
        public int RankedWins { get; set; }
        public int RankedDraws { get; set; }
        public int RankedLosses { get; set; }
        public int RankedWinPercent { get; set; }

        public RecordViewModel(Record r)
        {
            Wins = r.Wins;
            Losses = r.Losses;
            Draws = r.Draws;
            TotalGames = r.GamesPlayed;
            if(TotalGames == 0)
                WinPercent = 0;
            else
                WinPercent = 100 * Wins / TotalGames;

            RankedWins = r.RankedWins;
            RankedLosses = r.RankedLosses;
            RankedDraws = r.RankedDraws;
            RankedTotalGames = r.RankedGamesPlayed;
            if (RankedTotalGames == 0)
                RankedWinPercent = 0;
            else
                RankedWinPercent = 100 * RankedWins / RankedTotalGames;

            Name = r.Game.Name;
        }
    }

    public class ProfileViewModel
    {
        public Player player;

        public List<SeatViewModel> ActiveGames;
        public List<SeatViewModel> CompletedGames;
        public List<SeatViewModel> ProposedGames;

        public List<RecordViewModel> GameRecords;

        public ProfileViewModel(Player viewPlayer)
        {
            player = viewPlayer;
            ActiveGames = player.ActiveSeats.Where(s => s.Table.TableState == (int)TableState.InPlay).Select(s => new SeatViewModel(s)).ToList();
            CompletedGames = player.ActiveSeats.Where(s => s.Table.TableState == (int)TableState.Complete).Select(s => new SeatViewModel(s)).ToList();
            ProposedGames = player.ActiveSeats.Where(s => s.Table.TableState == (int)TableState.Proposed).Select(s => new SeatViewModel(s)).ToList();

            GameRecords = player.Records.Select(r => new RecordViewModel(r)).ToList();
        }
    }
}