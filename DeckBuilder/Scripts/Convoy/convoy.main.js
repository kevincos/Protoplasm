var gameState;
var playerContext;

var gameCore;

var grid = null;
var hand = null;
var draftHand = null;
var scoreArea = null;

var currentPlayerId = null;

function convoy_setGameState(state) {
    gameState = state;
    currentPlayerId = state.sourcePlayerId;
    for (var i = 0; i < gameState.playerContexts.length; i++) {
        if (gameState.playerContexts[i].playerId == currentPlayerId) {
            playerContext = gameState.playerContexts[i];
        }
    }

    if (playerContext.side == "Left") {
        grid = new SquareBoard(gameCore, 520, 300, 11, 12, 40, 40, 1.2);

        draftHand = new HandInstance(gameCore, new vMath.vector2(90, 90), 60, 60, 40, 1.3);
        hand = new HandInstance(gameCore, new vMath.vector2(90, 190), 60, 60, 40, 1.3);
        playArea = new HandInstance(gameCore, new vMath.vector2(150, 350), 140, 140, 40, 1.3);
        scoreArea = new vMath.vector2(150, 510);
    }
    if (playerContext.side == "Right") {
        grid = new SquareBoard(gameCore, 280, 300, 11, 12, 40, 40, 1.2);

        draftHand = new HandInstance(gameCore, new vMath.vector2(590, 90), 60, 60, 40, 1.3);
        hand = new HandInstance(gameCore, new vMath.vector2(590, 190), 60, 60, 40, 1.3);
        playArea = new HandInstance(gameCore, new vMath.vector2(650, 350), 140, 140, 40, 1.3);
        scoreArea = new vMath.vector2(650, 510);
    }
    

    for (var i = 0; i < playerContext.hand.length; i++) {
        hand.add(i, playerContext.hand[i].url);
    }

    for (var i = 0; i < playerContext.draftHand.length; i++) {
        draftHand.add(i, playerContext.draftHand[i].url);
    }

    if (playerContext.draftReady)
        draftHand.select(playerContext.draftIndex);

    if(gameState.playedPiece != null)
        playArea.add(0, gameState.playedPiece);

    updateLogs(gameState.logs);

    gameCore.refreshView = true;
}

function convoy_init(state) {
    gameCore = new GameCore();
    gameCore.draw = convoy_draw;
    gameCore.update = convoy_update;
    gameCore.init();

    convoy_setGameState(state);

}

function convoy_update() {
    grid.clickCallback = function (content, x, y) {
        if(gameState.state == "Select" || gameState.state =="Target")
        {
            if (gameState.board[x][y].selectable == true) {
                var update = {};
                update.selectedTileX = x;
                update.selectedTileY = y;
                update.playerId = currentPlayerId;
                var updateUrl = '/table/updateConvoy/' + gameState.tableId;
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
        if (gameState.state == "PlayCard") {
            var update = {};
            update.selectedCardIndex = index;
            update.playerId = currentPlayerId;
            var updateUrl = '/table/updateConvoy/' + gameState.tableId;
            $.ajax({
                url: updateUrl,
                type: 'POST',
                dataType: 'json',
                data: JSON.stringify(update),
                contentType: 'application/json; charset=utf-8'
            });
        }
    }

    draftHand.selectCallback = function (content, index) {
        if (gameState.state == "Draft") {
            var update = {};
            update.selectedCardIndex = index;
            update.playerId = currentPlayerId;
            var updateUrl = '/table/updateConvoy/' + gameState.tableId;
            $.ajax({
                url: updateUrl,
                type: 'POST',
                dataType: 'json',
                data: JSON.stringify(update),
                contentType: 'application/json; charset=utf-8'
            });
            draftHand.select(index);
        }
    }

    grid.update();
    hand.update();
    draftHand.update();
}

function convoy_draw() {
    gameCore.spriteContext.draw(new Sprite("/content/images/canyon/convoybg.png"), new vMath.vector2(400, 300), 800, 600);
    
    grid.drawCallback = function (tileContext, x, y) {
        tileContext.drawFrame(gameState.board[x][y].url, 0, 0);

        if (gameState.board[x][y].unit != null) {
            tileContext.drawFrame(gameState.board[x][y].unit.url, gameState.board[x][y].unit.direction, 0);
            if (gameState.board[x][y].unit.hits > 0)
                tileContext.drawFrame("/content/images/canyon/frame_damage.png", 0, 0);                
        }


        if (gameState.board[x][y].selectable == true && gameState.activePlayerId == playerContext.playerId)
            tileContext.drawFrame("/content/images/canyon/tile_highlight.png", 0, 0);
    }

    hand.drawCallback = function (cardContext, index) {
        cardContext.drawFrame("/content/images/canyon/frame_empty.png", 0, 0);   
        cardContext.drawFrame(playerContext.hand[index].url);
        if (gameState.activePlayerId != playerContext.playerId) {
            cardContext.drawFrame("/content/images/canyon/tile_shade.png");
        }
    }


    playArea.drawCallback = function (cardContext, index) {
        cardContext.drawFrame("/content/images/canyon/frame_empty.png", 0, 0);   
        cardContext.drawFrame(gameState.playedPiece.url);
        if (gameState.activePlayerId != playerContext.playerId) {
            cardContext.drawFrame("/content/images/canyon/tile_shade.png");
        }
    }

    draftHand.drawCallback = function (cardContext, index) {
        cardContext.drawFrame("/content/images/canyon/frame_empty.png", 0, 0);   
        cardContext.drawFrame(playerContext.draftHand[index].url);
        //if (index != playerContext.draftIndex && playerContext.draftReady == true) {
        if (index != draftHand.selectIndex && draftHand.selectIndex != null) {
            cardContext.drawFrame("/content/images/canyon/tile_shade.png");
        }

    }

    gameCore.spriteContext.draw(new Sprite("/content/images/canyon/convoy_viewframe.png"), grid.location, (grid.length + 1) * grid.tileSize, (grid.width + 1) * grid.tileSize);
    if (playerContext.playerId == gameState.activePlayerId && (gameState.state == "Select" || gameState.state == "Target"))
        gameCore.spriteContext.draw(new Sprite("/content/images/canyon/convoy_viewframe_active.png"), grid.location, (grid.length + 1) * grid.tileSize, (grid.width + 1) * grid.tileSize);

    grid.draw();

    if (playerContext.side == "Left")
        gameCore.spriteContext.draw(new Sprite("/content/images/canyon/convoy_handframe_blue.png"), new vMath.vector2(hand.location.x + hand.cardWidth, hand.location.y), 40 + 3 * hand.cardWidth, 40 + hand.cardHeight);
    if (playerContext.side == "Right")
        gameCore.spriteContext.draw(new Sprite("/content/images/canyon/convoy_handframe_red.png"), new vMath.vector2(hand.location.x + hand.cardWidth, hand.location.y), 40 + 3 * hand.cardWidth, 40 + hand.cardHeight);
    if (playerContext.playerId == gameState.activePlayerId && gameState.state == "PlayCard")
        gameCore.spriteContext.draw(new Sprite("/content/images/canyon/convoy_handframe_active.png"), new vMath.vector2(hand.location.x + hand.cardWidth, hand.location.y), 40 + 3 * hand.cardWidth, 40 + hand.cardHeight);
    hand.draw();

    if(playerContext.side == "Left")
        gameCore.spriteContext.draw(new Sprite("/content/images/canyon/convoy_handframe_blue.png"), new vMath.vector2(draftHand.location.x + draftHand.cardWidth, draftHand.location.y), 40 + 3 * draftHand.cardWidth, 40 + draftHand.cardHeight);
    if(playerContext.side == "Right")
        gameCore.spriteContext.draw(new Sprite("/content/images/canyon/convoy_handframe_red.png"), new vMath.vector2(draftHand.location.x + draftHand.cardWidth, draftHand.location.y), 40 + 3 * draftHand.cardWidth, 40 + draftHand.cardHeight);
    if (gameState.state == "Draft")
        gameCore.spriteContext.draw(new Sprite("/content/images/canyon/convoy_handframe_active.png"), new vMath.vector2(draftHand.location.x + draftHand.cardWidth, draftHand.location.y), 40 + 3 * draftHand.cardWidth, 40 + draftHand.cardHeight);
    draftHand.draw();

    if (playerContext.side == "Left")
        gameCore.spriteContext.draw(new Sprite("/content/images/canyon/convoy_playframe_blue.png"), playArea.location, 220, 220);
    if (playerContext.side == "Right")
        gameCore.spriteContext.draw(new Sprite("/content/images/canyon/convoy_playframe_red.png"), playArea.location, 220, 220);
    if (playerContext.playerId == gameState.activePlayerId && (gameState.state == "Select" || gameState.state == "Target"))
        gameCore.spriteContext.draw(new Sprite("/content/images/canyon/convoy_playframe_active.png"), playArea.location, 220, 220);

    playArea.draw();

    if (playerContext.side == "Left")
        gameCore.spriteContext.draw(new Sprite("/content/images/canyon/convoy_handframe_blue.png"), scoreArea, 40 + 3 * draftHand.cardWidth, 40 + draftHand.cardHeight);
    if (playerContext.side == "Right")
        gameCore.spriteContext.draw(new Sprite("/content/images/canyon/convoy_handframe_red.png"), scoreArea, 40 + 3 * draftHand.cardWidth, 40 + draftHand.cardHeight);

    for (var i = 0; i < gameState.playerContexts.length; i++) {
        if(gameState.playerContexts[i].side == "Left")
            gameCore.spriteContext.formattedText(gameState.playerContexts[i].score, new vMath.vector2(scoreArea.x - 40, scoreArea.y + 15), '40px Verdana', 'blue');
        if (gameState.playerContexts[i].side == "Right")
            gameCore.spriteContext.formattedText(gameState.playerContexts[i].score, new vMath.vector2(scoreArea.x + 40, scoreArea.y + 15), '40px Verdana', 'red');
    }
}