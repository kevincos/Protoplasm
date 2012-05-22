using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DeckBuilder.Models;
using DeckBuilder.Games;

namespace DeckBuilder.Protoplasm
{
    public class GameObject
    {
        public string type { get; set; }
    }

    public class GameUpdate
    {
        public int playerIndex { get; set; }        

        public string selectObjectName { get; set; }
        public int selectX { get; set; }
        public int selectY { get; set; }
        public int selectIndex { get; set; }
        public int selectDirection { get; set; }
    }

    public abstract class BaseGameState
    {
        public static System.Runtime.Serialization.Json.DataContractJsonSerializer GetSerializer(string game)
        {
            return BaseGameState.Create(game).GetSerializer();            
        }

        public abstract System.Runtime.Serialization.Json.DataContractJsonSerializer GetSerializer();

        public abstract GameView GetClientView(int playerId);

        public abstract void Update(GameUpdate update);

        public abstract void InitializeState(List<Seat> seats);

        public static BaseGameState Create(string game)
        {           
            Type GameType = Type.GetType("DeckBuilder.Games."+game+"State, DeckBuilder, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
            return (BaseGameState)Activator.CreateInstance(GameType);
        }
    }

    public abstract class GameState<ContextType> : BaseGameState where ContextType : PlayerContext, new()
    {
        public List<ContextType> playerContexts { get; set; }
                
        public int sourcePlayerId { get; set; }
        public int activePlayerId { get; set; }
        public int activePlayerIndex { get; set; }
        public int tableId { get; set; }
        public bool gameOver { get; set; }

        public string state { get; set; }
        public List<string> logs { get; set; }

        public List<object> drawObjects { get; set; }
        
        public override void InitializeState(List<Seat> seats)
        {
            tableId = seats[0].TableId;
            playerContexts = new List<ContextType>();
            foreach (Seat seat in seats)
            {
                ContextType playerContext = new ContextType();
                playerContext.playerId = seat.PlayerId;
                playerContext.name = seat.Player.Name;
                playerContexts.Add(playerContext);
            }
            activePlayerIndex = 0;
            activePlayerId = playerContexts[activePlayerIndex].playerId;

            logs = new List<string>();
        }

        public void AdvanceActivePlayer()
        {
            activePlayerIndex++;
            activePlayerIndex %= playerContexts.Count;
            activePlayerId = playerContexts[activePlayerIndex].playerId;
        }
    }

    public class GameView
    {
        public int activePlayerId { get; set; }
        public int tableId { get; set; }
        public List<GameObject> drawList { get; set; }
        public List<string> logs { get; set; }

        public GameView()
        {
            drawList = new List<GameObject>();
            logs = new List<string>();
        }
    }
}