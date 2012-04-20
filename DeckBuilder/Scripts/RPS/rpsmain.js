var rock = new Sprite("/content/images/rock.png");
var paper = new Sprite("/content/images/paper.png");
var scissors = new Sprite("/content/images/scissors.png");
var unknown = new Sprite("/content/images/mystery.png");
var cardBorder = new Sprite("/content/images/cardborder.png");
var cardFront = new Sprite("/content/images/cardfront.png");
var cardUsed = new Sprite("/content/images/cardused.png");

var gameState;
var playerContext;

var gameCore;
var scoreIcons = null;
var rpsHand = null;

function rps_setGameState(state) {
    gameState = state;
    currentPlayerId = state.sourcePlayerId;
    for (var i = 0; i < gameState.playerContexts.length; i++) {
        if (gameState.playerContexts[i].playerId == currentPlayerId) {
            playerContext = gameState.playerContexts[i];
        }
    }
    gameCore.refreshView = true;
}

function rps_init(state) {
    gameCore = new GameCore();
    gameCore.draw = rps_draw;
    gameCore.update = rps_update;    
    gameCore.init();

    rps_setGameState(state);
    scoreIcons = new IconInstance(gameCore, 50, 50);    
    rpsHand = new HandInstance(gameCore, new vMath.vector2(100, 100), 100, 100, 70, 1.2);
    rpsHand.add("Rock", "/content/images/rock.png");
    rpsHand.add("Paper", "/content/images/paper.png");
    rpsHand.add("Scissors", "/content/images/scissors.png");
}

function rps_update() {
    rpsHand.update();
}

function rps_draw() {
    // Draw and interact with hand
    if (playerContext.currentMove == "None") {        
        rpsHand.deselect();
    }
 
    
    rpsHand.selectCallback = function (item) {
        playerContext.currentMove = item;
        var updateUrl = '/table/updateRPS/' + gameState.tableId;
        $.ajax({
            url: updateUrl,
            type: 'POST',
            dataType: 'json',
            data: JSON.stringify(gameState),
            contentType: 'application/json; charset=utf-8'
        });
    }

    rpsHand.deselectCallback = function (item) {
        playerContext.currentMove = "None";
        var updateUrl = '/table/updateRPS/' + gameState.tableId;
        $.ajax({
            url: updateUrl,
            type: 'POST',
            dataType: 'json',
            data: JSON.stringify(gameState),
            contentType: 'application/json; charset=utf-8'
        });
    }


    rpsHand.drawCallback = function (cardContext, index) {
        cardContext.drawFrame("/content/images/cardfront.png");
        cardContext.drawPortrait("/content/images/" + rpsHand.contents[index] + ".png");
        if(rpsHand.selectIndex != null && rpsHand.selectIndex != index)
            cardContext.drawFrame("/content/images/cardUsed.png");
    }



    rpsHand.draw();
    
    // Draw Score Column
    gameCore.spriteContext.text(gameState.playerContexts[0].name, new vMath.vector2(500, 15));
    gameCore.spriteContext.text("Wins:" + gameState.playerContexts[0].wins, new vMath.vector2(500, 30));
    if (gameState.playerContexts[0].currentMove != "None" && gameState.playerContexts[0].currentMove != "Unknown")    
        scoreIcons.draw(new vMath.vector2(550, 60), "/content/images/"+gameState.playerContexts[0].currentMove+".png");
    else
        scoreIcons.draw(new vMath.vector2(550, 60), "/content/images/mystery.png");

    gameCore.spriteContext.text(gameState.playerContexts[1].name, new vMath.vector2(600, 15));
    gameCore.spriteContext.text("Wins:" + gameState.playerContexts[1].wins, new vMath.vector2(600, 30));
    if(gameState.playerContexts[1].currentMove != "None" && gameState.playerContexts[1].currentMove != "Unknown")
        scoreIcons.draw(new vMath.vector2(650, 60), "/content/images/" + gameState.playerContexts[1].currentMove + ".png");
    else
        scoreIcons.draw(new vMath.vector2(650, 60), "/content/images/mystery.png");
    
    
        
    for(var i = 0; i < gameState.currentRound; i++)
    {
        scoreIcons.draw(new vMath.vector2(550, 110 + 50 * i), "/content/images/" + gameState.playerContexts[0].moveHistory[gameState.currentRound - 1 - i] + ".png");
        scoreIcons.draw(new vMath.vector2(650, 110 + 50 * i), "/content/images/" + gameState.playerContexts[1].moveHistory[gameState.currentRound - 1 - i] + ".png");
    }
}