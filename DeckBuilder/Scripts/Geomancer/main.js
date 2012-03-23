
var FPS = 30;
var SECONDS_BETWEEN_FRAMES = 1 / FPS;
var canvas = null;

var spriteContext = null;

var hexFrame = new Sprite("content/images/hexagon.png");
var usedHexagon = new Sprite("content/images/hexagonused.png");
var highlight = new Sprite("content/images/highlight.png");
var homeCrystal = new Sprite("content/images/homecrystalportrait.png");
var manaCrystal = new Sprite("content/images/manacrystalportrait.png");
var barrenHex = new Sprite("content/images/barren.png");
var unitFrame = new Sprite("content/images/unitframe.png");
var blueFrame = new Sprite("content/images/blueframe.png");
var orangeFrame = new Sprite("content/images/orangeframe.png");
var blueSpellFrame = new Sprite("content/images/bluespellframe.png");
var orangeSpellFrame = new Sprite("content/images/orangespellframe.png");
var attackEdge = new Sprite("content/images/attackedge.png");
var awarenessEdge = new Sprite("content/images/awarenessedge.png");
var minotaur = new Sprite("content/images/minotaurportrait.png");
var lightningBolt = new Sprite("content/images/lightningboltportrait.png");
var hydra = new Sprite("content/images/hydraportrait.png");
var raider = new Sprite("content/images/raiderportrait.png");
var cardBorder = new Sprite("content/images/cardborder.png");
var cardFront = new Sprite("content/images/cardfront.png");
var cardUsed = new Sprite("content/images/cardused.png");
var cardBack = new Sprite("content/images/cardback.png");
var infoWindow = new Sprite("content/images/infowindow.png");

var pos = new vMath.vector2(150, 0);
var yVel = 1;

var selectedCardIndex = null;
var selectedTileA = null;
var selectedTileB = null;

var prevCardIndex = null;
var prevTileA = null;
var prevTileB = null;
var prevDirection = null;

var playerCardBase = new vMath.vector2(50, 750);
var opponentCardBase = new vMath.vector2(550, 50);

var hoverInfoWindowPos = new vMath.vector2(700, 700);
var selectInfoWindowPos = new vMath.vector2(700, 500);



var scale = 1;
var currentTime = 0;

var gameState;

var mouseDown = false;



function init(state) {
    gameState = state;

    $('#postheta').click(function () {
        hexMath.theta += .05;
    });
    $('#negtheta').click(function () {
        hexMath.theta -= .05;
    });
    $('#submitMove').click(function () {

        $.ajax({
            url: 'home/submit',
            type: 'POST',
            dataType: 'json',
            data: JSON.stringify(gameState),
            contentType: 'application/json; charset=utf-8',
            success: function (data) {
                gameState = data;
            },
            error: function () { alert("ERROR"); }
        });


    });

    canvas = document.getElementById('canvas');
    canvas.addEventListener("mousemove", vMath.mouseMoveListener, false);
    canvas.addEventListener("mousedown", vMath.mouseDownListener, false);
    canvas.addEventListener("mouseup", vMath.mouseUpListener, false);
    spriteContext = new SpriteContext(canvas.getContext('2d'));
    setInterval(draw, SECONDS_BETWEEN_FRAMES);
}

function draw() {


    var hexMouseCoords = hexMath.toHexCoords(vMath.mousePos.x, vMath.mousePos.y);
    var c = 18 - hexMouseCoords.x - hexMouseCoords.y;
    var roundedA = Math.round(hexMouseCoords.x);
    var roundedB = Math.round(hexMouseCoords.y);
    var roundedC = Math.round(c);




    var hoverDirection = 0;
    var relA = roundedA - hexMouseCoords.x;
    var relB = roundedB - hexMouseCoords.y;
    var relC = roundedC - c;
    if (relC > relA && relA > relB)
        hoverDirection = 3;
    if (relA > relC && relC > relB)
        hoverDirection = 4;
    if (relA > relB && relB > relC)
        hoverDirection = 5;
    if (relB > relC && relC > relA)
        hoverDirection = 1;
    if (relB > relA && relA > relC)
        hoverDirection = 0;
    if (relC > relB && relB > relA)
        hoverDirection = 2;
        
    
     


    var hoverCardIndex = null;
    var handMousePos = new vMath.vector2(vMath.mousePos.x - playerCardBase.x, vMath.mousePos.y - playerCardBase.y);
    if (handMousePos.x > -30 && handMousePos.x < 60 * gameState.hand.length && handMousePos.y > -40 && handMousePos.y < 40) {
        hoverCardIndex = Math.round(handMousePos.x / 60);
    }

    if (vMath.mouseDown == false) {
        mouseDown = false;
    }

    if (hoverDirection == prevDirection && roundedA == prevTileA && roundedB == prevTileB && hoverCardIndex == prevCardIndex && vMath.mouseDown == false) {
        return;
    }


    if (mouseDown == true) {
        return
    }
    if (vMath.mouseDown == true) {
        mouseDown = true;
    }



    prevTileA = roundedA;
    prevTileB = roundedB;
    prevCardIndex = hoverCardIndex;

    currentTime += SECONDS_BETWEEN_FRAMES;
    
    spriteContext.clear();
    


    var showHoverUnitInfo = false;
    var showHoverCardInfo = false;
    var hideSelectedInfo = false;

    var resetSelection = false;

    // Select/Deselect Tile
    if (roundedA >= 0 && roundedA < 14 && roundedB >= 0 && roundedB < 11) {
        var roundedC = 18 - roundedA - roundedB;
        if (roundedC >= 0 && roundedC < 14) {
            showHoverUnitInfo = true;
            if (vMath.mouseDown == true) {
                if (selectedTileA == null && selectedCardIndex == null) {
                    if (gameState.tileList[roundedA][roundedB].moveUnit != null) {
                        selectedTileA = gameState.tileList[roundedA][roundedB].moveUnit.moveA;
                        selectedTileB = gameState.tileList[roundedA][roundedB].moveUnit.moveB;
                        selectedCardIndex = null;
                        resetSelection = true;
                    }
                    else if (gameState.tileList[roundedA][roundedB].spell != null) {
                        selectedTileA = null;
                        selectedTileB = null;
                        selectedCardIndex = gameState.tileList[roundedA][roundedB].spell.sourceCardIndex;
                        resetSelection = true;
                    }
                    else {
                        selectedTileA = roundedA;
                        selectedTileB = roundedB;
                        selectedCardIndex = null;
                    }
                    if (selectedTileA != null && gameState.tileList[selectedTileA][selectedTileB].unit != null && gameState.tileList[selectedTileA][selectedTileB].unit.used == true) {
                        gameState.tileList[selectedTileA][selectedTileB].unit.used = false;
                        gameState.tileList[gameState.tileList[selectedTileA][selectedTileB].unit.moveA][gameState.tileList[selectedTileA][selectedTileB].unit.moveB].moveUnit = null;
                        gameState.tileList[selectedTileA][selectedTileB].unit.moveA = null;
                        gameState.tileList[selectedTileA][selectedTileB].unit.moveB = null;
                        
                    }
                    else if (selectedCardIndex != null) {
                        gameState.tileList[roundedA][roundedB].spell = null;
                        gameState.hand[selectedCardIndex].used = false;
                        gameState.hand[selectedCardIndex].castA = null;
                        gameState.hand[selectedCardIndex].castB = null;
                    }
                    
                }
                else if (selectedTileA == roundedA && selectedTileB == roundedB) {
                    if (gameState.tileList[selectedTileA][selectedTileB].unit != null && gameState.tileList[selectedTileA][selectedTileB].unit.used == true) {
                        gameState.tileList[selectedTileA][selectedTileB].unit.used = false;
                        gameState.tileList[gameState.tileList[selectedTileA][selectedTileB].unit.moveA][gameState.tileList[selectedTileA][selectedTileB].unit.moveB].moveUnit = null;
                        gameState.tileList[selectedTileA][selectedTileB].unit.moveA = null;
                        gameState.tileList[selectedTileA][selectedTileB].unit.moveB = null;
                    }
                    selectedTileA = null;
                    selectedTileB = null;
                    selectedCardIndex = null;
                }
            }
        }

    }
    // Select/Deselect Card
    if (hoverCardIndex != null && hoverCardIndex < gameState.hand.length && hoverCardIndex >= 0) {
        showHoverCardInfo = true;
        if (vMath.mouseDown == true) {
            if (selectedTileA == null && selectedCardIndex == null) {
                selectedCardIndex = hoverCardIndex;
                if (gameState.hand[selectedCardIndex].used == true) {
                    gameState.hand[selectedCardIndex].used = false;
                    gameState.tileList[gameState.hand[selectedCardIndex].castA][gameState.hand[selectedCardIndex].castB].spell = null;
                    gameState.hand[selectedCardIndex].castA = null;
                    gameState.hand[selectedCardIndex].castB = null;
                }
                selectedTileA = null;
                selectedTileB = null;
            }
            else if (selectedCardIndex == hoverCardIndex) {
                if (gameState.hand[selectedCardIndex].used == true) {
                    gameState.hand[selectedCardIndex].used = false;
                    gameState.tileList[gameState.hand[selectedCardIndex].castA][gameState.hand[selectedCardIndex].castB].spell = null;
                    gameState.hand[selectedCardIndex].castA = null;
                    gameState.hand[selectedCardIndex].castB = null;                    
                }
                selectedCardIndex = null;
                selectedTileA = null;
                selectedTileB = null;
            }

        }
    }

    //Card Selected, click space
    if (vMath.mouseDown == true && selectedCardIndex != null && resetSelection == false) {
        if (roundedA >= 0 && roundedA < 14 && roundedB >= 0 && roundedB < 11) {
            var roundedC = 18 - roundedA - roundedB;
            if (roundedC >= 0 && roundedC < 14) {
                if (gameState.hand[selectedCardIndex].used == undefined || gameState.hand[selectedCardIndex].used == false) {
                    gameState.hand[selectedCardIndex].used = true;
                    gameState.hand[selectedCardIndex].castA = roundedA;
                    gameState.hand[selectedCardIndex].castB = roundedB;
                    gameState.tileList[roundedA][roundedB].spell = {};
                    gameState.tileList[roundedA][roundedB].spell.name = gameState.hand[selectedCardIndex].name;
                    gameState.tileList[roundedA][roundedB].spell.awareness = "dad___";
                    gameState.tileList[roundedA][roundedB].spell.direction = hoverDirection;
                    gameState.tileList[roundedA][roundedB].spell.url = gameState.hand[selectedCardIndex].url;
                    gameState.tileList[roundedA][roundedB].spell.sourceCardIndex = selectedCardIndex;
                    selectedCardIndex = null;
                }
            }
        }
    }

    // Unit selected, click space
    if (vMath.mouseDown == true && selectedTileA != null && resetSelection == false) {
        if (roundedA >= 0 && roundedA < 14 && roundedB >= 0 && roundedB < 11) {
            var roundedC = 18 - roundedA - roundedB;
            if (roundedC >= 0 && roundedC < 14) {
                if (gameState.tileList[selectedTileA][selectedTileB].unit != null && (gameState.tileList[selectedTileA][selectedTileB].unit.used == undefined || gameState.tileList[selectedTileA][selectedTileB].unit.used == false)) {

                    if (roundedA != selectedTileA || roundedB != selectedTileB) {
                        gameState.tileList[selectedTileA][selectedTileB].unit.used = true;
                        gameState.tileList[selectedTileA][selectedTileB].unit.moveA = roundedA;
                        gameState.tileList[selectedTileA][selectedTileB].unit.moveB = roundedB;
                        gameState.tileList[roundedA][roundedB].moveUnit = {};
                        gameState.tileList[roundedA][roundedB].moveUnit.name = gameState.tileList[selectedTileA][selectedTileB].unit.name;
                        gameState.tileList[roundedA][roundedB].moveUnit.awareness = gameState.tileList[selectedTileA][selectedTileB].unit.awareness;
                        gameState.tileList[roundedA][roundedB].moveUnit.direction = hoverDirection;
                        gameState.tileList[roundedA][roundedB].moveUnit.playerId = gameState.tileList[selectedTileA][selectedTileB].unit.playerId;
                        gameState.tileList[roundedA][roundedB].moveUnit.url = gameState.tileList[selectedTileA][selectedTileB].unit.url;
                        gameState.tileList[roundedA][roundedB].moveUnit.moveA = selectedTileA;
                        gameState.tileList[roundedA][roundedB].moveUnit.moveB = selectedTileB;
                        selectedTileA = null;
                        selectedTileB = null;
                    }

                    selectedCardIndex = null;
                }
            }
        }
    }

    for(var a = 0; a < 14; a++)
    {
        for (var b = 0; b < 11; b++) {
            var c = 18 - a - b;
            //if (a >= 0 && b >= 0 && a < 14 && b < 11 && c >= 0 && c < 14) {

            if ((a != roundedA || b != roundedB) && (a != selectedTileA || b != selectedTileB))
                drawTile(a, b, false);
        }
    }





    for (var i = 0; i < gameState.hand.length; i++) {
        if (i != hoverCardIndex && i != selectedCardIndex) {
            drawCard(i, false);
        }
    }



    for (var i = 0; i < gameState.opponentHandCount; i++) {
        var cardHeight = 80;
        var cardWidth = 50;
        spriteContext.draw(cardBorder, new vMath.vector2(opponentCardBase.x + i * 60, opponentCardBase.y), cardWidth, cardHeight, 0, 1);
        spriteContext.draw(cardBack, new vMath.vector2(opponentCardBase.x + i * 60, opponentCardBase.y), cardWidth, 80, 0, 1);
    }


    if (vMath.mousePos.x > 550) {
        hoverInfoWindowPos.x = 100;
        hoverInfoWindowPos.y = 500;
        selectInfoWindowPos.x = 100;
        selectInfoWindowPos.y = 300;
    }
    if (vMath.mousePos.x < 250) {
        hoverInfoWindowPos.x = 700;
        hoverInfoWindowPos.y = 700;
        selectInfoWindowPos.x = 700;
        selectInfoWindowPos.y = 500;
    }



    if ((selectedCardIndex != null || selectedTileA != null) && hideSelectedInfo == false) {
        drawSelectInfo();

        if (selectedTileA != null)
            drawTile(selectedTileA, selectedTileB, false, true);
        if (selectedCardIndex != null)
            drawCard(selectedCardIndex, false, true);
    }
    if (showHoverCardInfo || showHoverUnitInfo)
    {
        drawHoverInfo();

        if(showHoverUnitInfo)
            drawTile(roundedA, roundedB, true, false, hoverDirection);
        if (showHoverCardInfo)
            drawCard(hoverCardIndex, true, false);
    }



}

function drawHoverInfo() {
    spriteContext.draw(infoWindow, hoverInfoWindowPos, 200, 200, 0, 1);
}
function drawSelectInfo() {
    spriteContext.draw(infoWindow, selectInfoWindowPos, 200, 200, 0, 1);
}

function drawCard(i, hover, select) {
    var cardHeight = 80;
    var cardWidth = 50;
    var portraitSize = 40;
    if (hover == true || select == true) {
        cardHeight = 120;
        cardWidth = 80;
        portraitSize = 60;
    }

    spriteContext.draw(cardFront, new vMath.vector2(playerCardBase.x + i * 60, playerCardBase.y), cardWidth, cardHeight, 0, 1);
    spriteContext.draw(new Sprite(gameState.hand[i].url), new vMath.vector2(playerCardBase.x + i * 60, playerCardBase.y), portraitSize, portraitSize, 0, 1);
    spriteContext.text(gameState.hand[i].cost, new vMath.vector2(playerCardBase.x + i * 60 - cardWidth / 3, playerCardBase.y - cardHeight / 4));
    if (gameState.hand[i].used == true) {
        spriteContext.draw(cardUsed, new vMath.vector2(playerCardBase.x + i * 60, playerCardBase.y), cardWidth, cardHeight, 0, 1);
    }

    if (hover == true) {
        var textY = hoverInfoWindowPos.y - 60;
        spriteContext.text(gameState.hand[i].name, new vMath.vector2(hoverInfoWindowPos.x - 80, textY));
        textY += 15;        
        spriteContext.text("Cost: " + gameState.hand[i].cost, new vMath.vector2(hoverInfoWindowPos.x - 70, textY));
        textY += 15;

        spriteContext.text("Type: " + gameState.hand[i].type, new vMath.vector2(hoverInfoWindowPos.x - 70, textY));
        textY += 25;
        spriteContext.textWrap(gameState.hand[i].description, new vMath.vector2(hoverInfoWindowPos.x - 80, textY), 160, 15);
    }
    if (select == true) {
        var textY = selectInfoWindowPos.y - 60;
        spriteContext.text(gameState.hand[i].name, new vMath.vector2(selectInfoWindowPos.x - 80, textY));
        textY += 15;
        spriteContext.text("Cost: " + gameState.hand[i].cost, new vMath.vector2(selectInfoWindowPos.x - 70, textY));
        textY += 15;

        spriteContext.text("Type: " + gameState.hand[i].type, new vMath.vector2(selectInfoWindowPos.x - 70, textY));
        textY += 25;
        spriteContext.textWrap(gameState.hand[i].description, new vMath.vector2(selectInfoWindowPos.x - 80, textY), 160, 15);
    }
}

function drawTile(a, b, hover, select, hoverDirection) {
    var tile = gameState.tileList[a][b];
    var tileSize = 70;
    var portraitSize = 35;
    if (hover == true || select == true) {
        tileSize = 110;
        portraitSize = 55;
    }

    var screenCoords = hexMath.toScreenCoords(a, b);
    if (tile != null) {
        var stackLevel = 0;
        if (tile.type == "Barren") {
            spriteContext.draw(barrenHex, screenCoords, tileSize, tileSize, hexMath.theta, 1);
        }
        else {
            spriteContext.draw(hexFrame, screenCoords, tileSize, tileSize, hexMath.theta, 1);
        }

        if (tile.crystal != null) {
            if (tile.crystal.playerId == 0)
                spriteContext.draw(blueFrame, screenCoords, tileSize, tileSize, hexMath.theta, 1);
            else
                spriteContext.draw(orangeFrame, screenCoords, tileSize, tileSize, hexMath.theta, 1);

            spriteContext.draw(new Sprite(tile.crystal.url), screenCoords, portraitSize, portraitSize, 0, 1);
            
            stackLevel++;
            screenCoords.y -= 20;
        }
        if (tile.unit != null) {
            if (tile.unit.playerId == 0)
                spriteContext.draw(blueFrame, screenCoords, tileSize, tileSize, hexMath.theta, 1);
            else
                spriteContext.draw(orangeFrame, screenCoords, tileSize, tileSize, hexMath.theta, 1);
            if (tile.unit.used == true) {
                spriteContext.draw(usedHexagon, screenCoords, tileSize, tileSize, hexMath.theta, 1);
            }
            

            spriteContext.draw(new Sprite(tile.unit.url), screenCoords, portraitSize, portraitSize, 0, 1);

            for (var i = 0; i < 6; i++) {
                if (tile.unit.awareness[i] == 'a')
                    spriteContext.draw(attackEdge, screenCoords, tileSize, tileSize, hexMath.theta + (tile.unit.direction + i - 1) * Math.PI / 3, 1);
                if (tile.unit.awareness[i] == 'd')
                    spriteContext.draw(awarenessEdge, screenCoords, tileSize, tileSize, hexMath.theta + (tile.unit.direction + i - 1) * Math.PI / 3, 1);

            }
            
            stackLevel++;
            screenCoords.y -= 20;
        }
        if (tile.moveUnit != null) {
            if (tile.moveUnit.playerId == 0)
                spriteContext.draw(blueFrame, screenCoords, tileSize, tileSize, hexMath.theta, 1);
            else
                spriteContext.draw(orangeFrame, screenCoords, tileSize, tileSize, hexMath.theta, 1);
            spriteContext.draw(new Sprite(tile.moveUnit.url), screenCoords, portraitSize, portraitSize, 0, 1);

            for (var i = 0; i < 6; i++) {
                if (tile.moveUnit.awareness[i] == 'a')
                    spriteContext.draw(attackEdge, screenCoords, tileSize, tileSize, hexMath.theta + (tile.moveUnit.direction + i - 1) * Math.PI / 3, 1);
                if (tile.moveUnit.awareness[i] == 'd')
                    spriteContext.draw(awarenessEdge, screenCoords, tileSize, tileSize, hexMath.theta + (tile.moveUnit.direction + i - 1) * Math.PI / 3, 1);

            }

            stackLevel++;
            screenCoords.y -= 20;
        }
        if (tile.spell != null) {
            if (tile.spell.playerId == 0)
                spriteContext.draw(blueSpellFrame, screenCoords, tileSize, tileSize, hexMath.theta, 1);
            else
                spriteContext.draw(orangeSpellFrame, screenCoords, tileSize, tileSize, hexMath.theta, 1);

            spriteContext.draw(new Sprite(tile.spell.url), screenCoords, portraitSize, portraitSize, 0, 1);

            for (var i = 0; i < 6; i++) {
                if (tile.spell.awareness[i] == 'a')
                    spriteContext.draw(attackEdge, screenCoords, tileSize, tileSize, hexMath.theta + (tile.spell.direction + i - 1) * Math.PI / 3, 1);
                if (tile.spell.awareness[i] == 'd')
                    spriteContext.draw(awarenessEdge, screenCoords, tileSize, tileSize, hexMath.theta + (tile.spell.direction + i - 1) * Math.PI / 3, 1);

            }

            stackLevel++;
            screenCoords.y += 20;
        }
        if (hover == true || select == true) {
            
            var infoWindowBase = hoverInfoWindowPos;
            if (select == true)
                infoWindowBase = selectInfoWindowPos;

            var textY = infoWindowBase.y - 60;
            if (tile.unit != null) {
                spriteContext.text(tile.unit.name, new vMath.vector2(infoWindowBase.x - 70, textY));
                textY += 15;
                spriteContext.text("HP: " + tile.unit.hp + " / " + tile.unit.maxHP, new vMath.vector2(infoWindowBase.x - 60, textY));
                textY += 15;
                spriteContext.text("Attack: " + tile.unit.attack, new vMath.vector2(infoWindowBase.x - 60, textY));
                textY += 15;
                spriteContext.text("Defense: " + tile.unit.defense, new vMath.vector2(infoWindowBase.x - 60, textY));
                textY += 15;
                spriteContext.text("Speed: " + tile.unit.speed, new vMath.vector2(infoWindowBase.x - 60, textY));
                textY += 15;                
            }
            if (tile.crystal != null) {
                spriteContext.text(tile.crystal.name, new vMath.vector2(infoWindowBase.x - 70, textY));
                textY += 15;
                spriteContext.text("Charged: " + tile.crystal.charged, new vMath.vector2(infoWindowBase.x - 60, textY));
                textY += 15;
                
            }
            if (tile.spell != null) {
                spriteContext.text(tile.spell.name, new vMath.vector2(infoWindowBase.x - 70, textY));
                textY += 15;
            }
        }
        if(hover==true)
            spriteContext.draw(attackEdge, screenCoords, tileSize, tileSize, hexMath.theta + hoverDirection * Math.PI / 3, 1);
        
    }

}