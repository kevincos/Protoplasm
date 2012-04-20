var hexFrame = new Sprite("/content/images/hexagon.png");
var usedHexagon = new Sprite("/content/images/hexagonused.png");
var highlight = new Sprite("/content/images/highlight.png");
var homeCrystal = new Sprite("/content/images/homecrystal.png");
var barrenHex = new Sprite("/content/images/barren.png");
var unitFrame = new Sprite("/content/images/unitframe.png");
var blueFrame = new Sprite("/content/images/blueframe.png");
var orangeFrame = new Sprite("/content/images/orangeframe.png");
var blueSpellFrame = new Sprite("/content/images/bluespellframe.png");
var orangeSpellFrame = new Sprite("/content/images/orangespellframe.png");
var attackEdge = new Sprite("/content/images/attackedge.png");
var awarenessEdge = new Sprite("/content/images/awarenessedge.png");
var cardBorder = new Sprite("/content/images/cardborder.png");
var cardFront = new Sprite("/content/images/cardfront.png");
var cardUsed = new Sprite("/content/images/cardused.png");
var cardBack = new Sprite("/content/images/cardback.png");
var infoWindow = new Sprite("/content/images/infowindow.png");


var gameState;
var playerContext;
var activePlayerId; // Not needed
var currentPlayerId; // Not needed 
var currentPlayerIndex; // Not needed

var gameCore;
var hexBoard = null;
var playerHand = null;
var hoverInfo = null;
var selectInfo = null;
var overlay = null;
var enemyOverlay = null;

var currentMana = 1;
var selectionState = "Normal";

function geomancer_setGameState(state) {
    gameState = state;
    currentPlayerId = state.sourcePlayerId;
    activePlayerId = gameState.playerContexts[gameState.activePlayerIndex].playerId;
    for (var i = 0; i < gameState.playerContexts.length; i++) {
        if (gameState.playerContexts[i].playerId == currentPlayerId) {
            currentPlayerIndex = i;
            playerContext = gameState.playerContexts[i];
        }
    }
    // initialize hand/board/core objects
    playerHand = new HandInstance(gameCore, new vMath.vector2(50, 740), 50, 80, 40, 1.2);
    for(var i = 0; i < gameState.playerContexts[currentPlayerIndex].hand.length; i++)
    {
        playerHand.add(i, gameState.playerContexts[currentPlayerIndex].hand[i].url);        
    }    
    gameCore.refreshView = true;    
    currentMana = 1;

    hexBoard = new HexBoard(gameCore, 70, 35, 1.4);

    hoverInfo = new InfoWindow(gameCore, new vMath.vector2(700,700), 200, 200);
    selectInfo = new InfoWindow(gameCore, new vMath.vector2(700,500), 200, 200);
}

function geomancer_init(state) {     
    $('#submitGameMove').click(function () {
        var updateUrl = '/table/update/' + gameState.tableId;
        $.ajax({
            url: updateUrl,
            type: 'POST',
            dataType: 'json',
            data: JSON.stringify(gameState),
            contentType: 'application/json; charset=utf-8',
        });


    });

    gameCore = new GameCore();
    gameCore.draw = geomancer_draw;
    gameCore.update = geomancer_update;
    gameCore.init();

    geomancer_setGameState(state);
}

