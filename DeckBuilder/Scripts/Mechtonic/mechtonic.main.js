var gameState;
var playerContext;

var gameCore;

var board = null;
var hand = null;
var market = null;
var playArea = null;
var resourceArea = null;

var currentPlayerId = null;

function mechtonic_setGameState(state) {
    gameState = state;
    currentPlayerId = state.sourcePlayerId;
    for (var i = 0; i < gameState.playerContexts.length; i++) {
        if (gameState.playerContexts[i].playerId == currentPlayerId) {
            playerContext = gameState.playerContexts[i];
        }
    }

    
    board = new HexBoard(gameCore, new vMath.vector2(250,350), gameState.sideLength, 50, 50, 1.2);

    market = new HandInstance(gameCore, new vMath.vector2(70, 70), 60, 60, 40, 1.3);
    hand = new HandInstance(gameCore, new vMath.vector2(500, 190), 60, 60, 40, 1.3);
    playArea = new HandInstance(gameCore, new vMath.vector2(550, 350), 140, 140, 40, 1.3);
    resourceArea = new vMath.vector2(550, 510);


    for (var i = 0; i < playerContext.hand.length; i++) {
        hand.add(i, playerContext.hand[i].url);
    }

    for (var i = 0; i < gameState.purchaseCards.length; i++) {
        market.add(i, gameState.purchaseCards[i].url);
    }

    if (gameState.playedCard != null)
        playArea.add(0, gameState.playedCard.url);

    updateLogs(gameState.logs);

    gameCore.refreshView = true;
}

function mechtonic_init(state) {    
    gameCore = new GameCore();
    gameCore.draw = mechtonic_draw;
    gameCore.update = mechtonic_update;
    gameCore.init();
    mechtonic_setGameState(state);    
}

function mechtonic_update() {

    board.clickCallback = function (content, a, b) {
        if (gameState.state == "Summon" || gameState.state == "SpecialSummon" || gameState.state == "StartPlace" || gameState.state == "Select" || gameState.state == "Target" || gameState.state == "CardTarget") {
            if (gameState.board[a][b].selectable == true && playerContext.playerId == gameState.activePlayerId) {
                var update = {};
                update.selectedCard = -1;
                update.selectA = a;
                update.selectB = b;
                update.playerId = currentPlayerId;
                var updateUrl = '/table/updateMechtonic/' + gameState.tableId;
                $.ajax({
                    url: updateUrl,
                    type: 'POST',
                    dataType: 'json',
                    data: JSON.stringify(update),
                    contentType: 'application/json; charset=utf-8'
                });
            }
        }
    }

    hand.clickCallback = function (content, index) {
        if (gameState.state == "Select") {
            if (playerContext.hand[index].selectable == true && playerContext.playerId == gameState.activePlayerId) {

                var update = {};
                update.selectedCard = index;
                update.playerId = currentPlayerId;
                var updateUrl = '/table/updateMechtonic/' + gameState.tableId;
                $.ajax({
                    url: updateUrl,
                    type: 'POST',
                    dataType: 'json',
                    data: JSON.stringify(update),
                    contentType: 'application/json; charset=utf-8'
                });
            }
        }
    }

    market.clickCallback = function (content, index) {

        if (gameState.state == "Buy" || gameState.state == "SpecialBuy") {            
            if (gameState.purchaseCards[index].selectable == true && playerContext.playerId == gameState.activePlayerId) {
                
                var update = {};
                update.selectedCard = index;
                update.playerId = currentPlayerId;
                var updateUrl = '/table/updateMechtonic/' + gameState.tableId;
                $.ajax({
                    url: updateUrl,
                    type: 'POST',
                    dataType: 'json',
                    data: JSON.stringify(update),
                    contentType: 'application/json; charset=utf-8'
                });
            }
        }
    }

    board.update();
    hand.update();
    market.update();
}

function mechtonic_draw() {
    
    gameCore.spriteContext.draw(new Sprite("/content/images/canyon/convoybg.png"), new vMath.vector2(400, 300), 800, 600);

    board.drawCallback = function (tileContext, a, b) {
        if (gameState.board[a][b] != null) {
            tileContext.drawFrame(gameState.board[a][b].url, 0, 0);
            if (gameState.board[a][b].unit != null) {
                if (gameState.playerContexts[gameState.board[a][b].unit.ownerIndex].color == "Red")
                    tileContext.drawFrame("/content/images/mechtonic/unit_frame_red.png", 0, 0);
                if (gameState.playerContexts[gameState.board[a][b].unit.ownerIndex].color == "Blue")
                    tileContext.drawFrame("/content/images/mechtonic/unit_frame_blue.png", 0, 0);
                if (gameState.playerContexts[gameState.board[a][b].unit.ownerIndex].color == "Green")
                    tileContext.drawFrame("/content/images/mechtonic/unit_frame_green.png", 0, 0);
                if (gameState.playerContexts[gameState.board[a][b].unit.ownerIndex].color == "Yellow")
                    tileContext.drawFrame("/content/images/mechtonic/unit_frame_yellow.png", 0, 0);

                if (gameState.board[a][b].unit.strongSide) {
                    tileContext.drawFrame("/content/images/mechtonic/unit_frame_strong.png", 0, 0);
                }
                tileContext.drawFrame(gameState.board[a][b].unit.url, 0, 0);
            }

            if (gameState.board[a][b].selectable == true && gameState.activePlayerId == playerContext.playerId)
                tileContext.drawFrame("/content/images/mechtonic/highlight.png", 0, 0);

        }
    }

    

    hand.drawCallback = function (cardContext, index) {
        cardContext.drawFrame("/content/images/mechtonic/card_frame.png", 0, 0);
        cardContext.drawFrame(playerContext.hand[index].url);
        if (gameState.activePlayerId != playerContext.playerId) {
            cardContext.drawFrame("/content/images/mechtonic/card_shade.png");
        }
    }
    

    playArea.drawCallback = function (cardContext, index) {
        cardContext.drawFrame("/content/images/mechtonic/card_frame.png", 0, 0);
        cardContext.drawFrame(gameState.playedCard.url);
        if (gameState.activePlayerId != playerContext.playerId) {
            cardContext.drawFrame("/content/images/mechtonic/card_shade.png");
        }
    }



    market.drawCallback = function (cardContext, index) {
        cardContext.drawFrame("/content/images/mechtonic/buy_frame.png", 0, 0);
        cardContext.drawFrame(gameState.purchaseCards[index].url);
        if (gameState.state == "Buy" || gameState.state == "SpecialBuy") {
            if (playerContext.money < gameState.purchaseCards[index].cost) {
                cardContext.drawFrame("/content/images/mechtonic/card_shade.png");
            }
            if (gameState.purchaseCards[index].selectable == true && gameState.activePlayerId == playerContext.playerId) {
                cardContext.drawFrame("/content/images/mechtonic/card_highlight.png");
            }
        }
        else {
            cardContext.drawFrame("/content/images/mechtonic/card_shade.png");
        }

    }

    board.draw();

    hand.draw();

    market.draw();

    playArea.draw();

    gameCore.spriteContext.formattedText(playerContext.money, new vMath.vector2(resourceArea.x - 40, resourceArea.y + 15), '40px Verdana', 'black');
}