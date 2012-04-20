var cardBorder = new Sprite("/content/images/connect4grid.png");
var cardFront = new Sprite("/content/images/redchip.png");
var cardUsed = new Sprite("/content/images/blackchip.png");

var gameState;
var playerContext;

var gameCore;

var grid = null;
var resourceLocation = null;

var currentPlayerId = null;

function connect4_setGameState(state) {
    gameState = state;
    currentPlayerId = state.sourcePlayerId;
    for (var i = 0; i < gameState.playerContexts.length; i++) {
        if (gameState.playerContexts[i].playerId == currentPlayerId) {
            playerContext = gameState.playerContexts[i];
        }
    }

    grid = new SquareBoard(gameCore, 300, 300, 7, 7, 80, 80, 1.2);    
    
    gameCore.refreshView = true;
}

function connect4_init(state) {
    gameCore = new GameCore();
    gameCore.draw = connect4_draw;
    gameCore.update = connect4_update;
    gameCore.init();

    connect4_setGameState(state);
    
}

function connect4_update() {
    grid.clickCallback = function (content, x, y) {
        if (gameState.grid[x][y] == "Empty") {
            var update = {};
            update.x = x;
            update.y = y;
            update.playerId = currentPlayerId;
            var updateUrl = '/table/updateConnect4/' + gameState.tableId;
            $.ajax({
                url: updateUrl,
                type: 'POST',
                dataType: 'json',
                data: JSON.stringify(update),
                contentType: 'application/json; charset=utf-8'
            });
        }
    }

    grid.update();
}

function connect4_draw() {
    grid.drawCallback = function (tileContext, x, y) {
        if (gameState.grid[x][y] == "Red")
            tileContext.drawFrame("/content/images/redchip.png", 0, 0);
        if (gameState.grid[x][y] == "Black")
            tileContext.drawFrame("/content/images/blackchip.png", 0, 0);

        if (x == grid.hoverX && y == grid.hoverY) {
            if(playerContext.color == "Red")
                tileContext.drawFrame("/content/images/redchiphover.png", 0, 0);
            if (playerContext.color == "Black")
                tileContext.drawFrame("/content/images/blackchiphover.png", 0, 0);
        }
        tileContext.drawFrame("/content/images/connect4grid.png", 0, 0);
    }

    grid.draw();    
}