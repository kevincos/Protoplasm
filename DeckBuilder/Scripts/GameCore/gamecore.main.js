var gameState;
var playerContext;

var gameCore;

//var squareBoards = [];
//var hexBoards = [];
//var hands = [];
//var textBoxes = [];
//var images = [];
var viewObjects = [];


var backgroundUrl = null;
var frameUrl = null;

var currentPlayerId = null;

function main_setGameState(view) {
    gameState = view;
    currentPlayerId = view.activePlayerId;

    //squareBoards = [];
    //hands = [];
    //textBoxes = [];
    //images = [];
    viewObjects = [];
    gameCore.infoWindows = [];

    if (view.background != undefined)
        backgroundUrl = view.background;
       
    if (view.frame != undefined)
        frameUrl = view.frame;

    for (var i = 0; i < view.drawList.length; i++) {
        if (view.drawList[i].type == "SquareBoard") {
            var board = new SquareBoard2(gameCore, view.drawList[i]);
            //squareBoards.push(board);
            viewObjects.push(board);
        }
        if (view.drawList[i].type == "HexBoard") {
            var board = new HexBoard2(gameCore, view.drawList[i]);
            //squareBoards.push(board);
            viewObjects.push(board);
        }
        if (view.drawList[i].type == "Hand") {
            var hand = new HandInstance2(gameCore, view.drawList[i]);
            //hands.push(hand);
            viewObjects.push(hand);
        }
        if (view.drawList[i].type == "TextBox") {
            //textBoxes.push(view.drawList[i]);
            viewObjects.push(view.drawList[i]);
        }
        if (view.drawList[i].type == "Image") {
            //images.push(view.drawList[i]);
            viewObjects.push(view.drawList[i]);
        }
        if (view.drawList[i].type == "InfoWindow") {
            var infoWindow = new InfoWindow2(gameCore, view.drawList[i])
            viewObjects.push(infoWindow);
            gameCore.infoWindows.push(infoWindow);
        }
    }
    updateLogs(gameState.logs);

    gameCore.refreshView = true;
}

function main_init(state) {    
    var stateStr = JSON.stringify(state);
    
    gameCore = new GameCore();
    gameCore.draw = main_draw;
    gameCore.update = main_update;
    gameCore.init();

    main_setGameState(state);

}

function main_update() {
    for (var i = 0; i < viewObjects.length; i++) {
        if (viewObjects[i].type == "SquareBoard" || viewObjects[i].type == "Hand" || viewObjects[i].type == "HexBoard")
            viewObjects[i].update();
    }
}

function main_draw() {
    if (backgroundUrl != null && backgroundUrl != "")
        gameCore.spriteContext.draw(new Sprite(backgroundUrl), new vMath.vector2(400, 300), 800, 600);
    if (frameUrl != null && frameUrl != "")
        gameCore.spriteContext.draw(new Sprite(frameUrl), new vMath.vector2(400, 300), 800, 600);

    for (var i = 0; i < viewObjects.length; i++) {
        if (viewObjects[i].type == "SquareBoard" || viewObjects[i].type == "Hand" || viewObjects[i].type == "InfoWindow" || viewObjects[i].type == "HexBoard")
            viewObjects[i].draw();
        if (viewObjects[i].type == "TextBox")
            gameCore.spriteContext.formattedText(viewObjects[i].text, new vMath.vector2(viewObjects[i].x, viewObjects[i].y), viewObjects[i].font, viewObjects[i].color);
        if (viewObjects[i].type == "Image")
            gameCore.spriteContext.draw(new Sprite(viewObjects[i].url), new vMath.vector2(viewObjects[i].x, viewObjects[i].y), viewObjects[i].width, viewObjects[i].height, 0, 1);
    }  
}