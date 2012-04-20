var cardBorder = new Sprite("/content/images/cardborder.png");
var cardFront = new Sprite("/content/images/cardfront.png");
var cardUsed = new Sprite("/content/images/cardused.png");

var gameState;
var playerContext;

var gameCore;

var hand = null;
var playArea = null;
var systemTrack = null;
var opponentTracks = [];
var invasionTrack = null;
var invasionDiscard = null;
var resourceCounters = null;
var infoWindow = null;
var infoCard = null;
var supplyPiles = [];
var resourceLocation = null;

var currentPlayerId = null;

function onslaught_setGameState(state) {
    gameState = state;
    currentPlayerId = state.sourcePlayerId;
    for (var i = 0; i < gameState.playerContexts.length; i++) {
        if (gameState.playerContexts[i].playerId == currentPlayerId) {
            playerContext = gameState.playerContexts[i];
        }
    }

    hand = new HandInstance(gameCore, new vMath.vector2(60, 100), 60, 80, 40, 1.3);
    for (var i = 0; i < playerContext.hand.length; i++) {
        hand.add(i, playerContext.hand[i].url);
    }

    resourceCounters = new IconInstance(gameCore, 50, 50);
    playArea = new HandInstance(gameCore, new vMath.vector2(60, 200), 60, 80, 40, 1.0);
    for (var i = 0; i < playerContext.playArea.length; i++) {
        playArea.add(i, playerContext.playArea[i].url);
    }

    resourceLocation = new vMath.vector2(400, 50);
    var supplyLocationX = 60;
    var supplyLocationY = 400;
    var supplyWidth = 60;
    var supplyHeight = 80;
    var prevType = "Miner";
    for (var i = 0; i < gameState.supplyPiles.length; i++) {
        if (gameState.supplyPiles[i].card.type != prevType) {
            prevType = gameState.supplyPiles[i].card.type;
            supplyLocationY += supplyHeight;
            supplyLocationX = 60;
        }
        supplyPiles[i] = new StackInstance(gameCore, new vMath.vector2(supplyLocationX, supplyLocationY), supplyWidth, supplyHeight, 40, 1.2);
        supplyLocationX += supplyWidth;
                
        for (var j = 0; j < gameState.supplyPiles[i].quantity; j++) {
            supplyPiles[i].add(gameState.supplyPiles[i].card);
        }
    }
    
    systemTrack = new TrackInstance(gameCore, new vMath.vector2(515, 480), 100, 550, 10, "/content/images/onslaught/systemtrack.png");
    systemTrack.tokenWidth = 30;
    systemTrack.tokenHeight = 30;
    for(var i =0; i < 10; i++)
    {
        for(var j =0; j < playerContext.systemStrip[i].length; j++)
        {
            systemTrack.addToken(null, i, playerContext.systemStrip[i][j].url);
        }
    }

    var opponentIndex = 0;
    for(var o = 0; o < gameState.playerContexts.length; o++)
    {
        if(gameState.playerContexts[o].playerId != playerContext.playerId)
        {
            opponentTracks[opponentIndex] = new TrackInstance(gameCore, new vMath.vector2(620, 480), 100, 550, 10, "/content/images/onslaught/systemtrack.png");
            opponentTracks[opponentIndex].tokenWidth = 30;
            opponentTracks[opponentIndex].tokenHeight = 30;
            for(var i =0; i < 10; i++)
            {
                for(var j =0; j < gameState.playerContexts[o].systemStrip[i].length; j++)
                {
                    opponentTracks[opponentIndex].addToken(null, i, gameState.playerContexts[o].systemStrip[i][j].url);
                }
            }
            opponentIndex++;
        }
    }


    invasionDiscard = new StackInstance(gameCore, new vMath.vector2(700, 150), 60, 80, 40, 1.2);
    invasionDiscard.add(gameState.invasionDiscard[gameState.invasionDiscard.length-1]);

    invasionTrack = new TrackInstance(gameCore, new vMath.vector2(700, 480), 50, 550, 10, "/content/images/onslaught/invasiontrack.png");
    invasionTrack.addToken(null, gameState.invasionLevel, "/content/images/onslaught/skull.png")

    infoWindow = new InfoWindow(gameCore, new vMath.vector2(700, 700), 200, 200);
    infoWindow.mouseTracking = true;

    gameCore.refreshView = true;
}

function onslaught_init(state) {
    gameCore = new GameCore();
    gameCore.draw = onslaught_draw;
    gameCore.update = onslaught_update;
    gameCore.init();

    $('#submitGameMove').click(function () {
        var update = {};
        update.playerId = playerContext.playerId;
        update.type = "End";
        var updateUrl = '/table/updateOnslaught/' + gameState.tableId;
        $.ajax({
            url: updateUrl,
            type: 'POST',
            dataType: 'json',
            data: JSON.stringify(update),
            contentType: 'application/json; charset=utf-8',
        });


    });

    onslaught_setGameState(state);
    
}

function submitUpdate(update) {
    var updateUrl = '/table/updateOnslaught/' + gameState.tableId;
    $.ajax({
        url: updateUrl,
        type: 'POST',
        dataType: 'json',
        data: JSON.stringify(update),
        contentType: 'application/json; charset=utf-8'
    });
}

function onslaught_update() {
    infoCard = null;

    hand.selectCallback = function (item, index) {
        if(playerContext.state == "Normal")
        {
            var update = {};
            update.playerId = playerContext.playerId;
            update.type = "Play";
            update.card = playerContext.hand[index];
            submitUpdate(update);
        }
        else if(playerContext.state == "Choice" && playerContext.hand[index].validTarget == true) 
        {
            var update = {};
            update.playerId = playerContext.playerId;
            update.type = "Choice";
            update.card = playerContext.hand[index];
            submitUpdate(update);
        }
    }
    hand.hoverCallback = function (index) {
        infoCard = playerContext.hand[index];
    }
    hand.update();

    playArea.hoverCallback = function (index) {
        infoCard = playerContext.playArea[index];
    }
    playArea.update();

    for (var i = 0; i < supplyPiles.length; i++) {
        supplyPiles[i].clickCallback = function (cardContext, index) {
            if(playerContext.state == "Normal")
            {
                var update = {};
                update.playerId = playerContext.playerId;
                update.type = "Buy";
                update.card = gameState.supplyPiles[i].card;
                submitUpdate(update);
            }
            else if(playerContext.state == "Choice" && gameState.supplyPiles[i].validTarget == true) 
            {
                var update = {};
                update.playerId = playerContext.playerId;
                update.type = "Choice";
                update.card = gameState.supplyPiles[i].card;
                submitUpdate(update);
            }
        };
        supplyPiles[i].hoverCallback = function (hover) {
            if (hover == true)
                infoCard = gameState.supplyPiles[i].card;
        }
        supplyPiles[i].update();
    }

    invasionDiscard.hoverCallback = function (hover) {
        if (hover == true)
            infoCard = gameState.invasionDiscard[gameState.invasionDiscard.length-1];
    }
    invasionDiscard.update();
    
    systemTrack.hoverCallback = function(segmentIndex, tokenIndex)
    {
        infoCard = playerContext.systemStrip[segmentIndex][tokenIndex];
    }
    systemTrack.clickCallback = function(content, segmentIndex, tokenIndex)
    {
        if(playerContext.state == "Choice" && playerContext.systemStrip[segmentIndex][tokenIndex].validTarget == true) 
        {
            var update = {};
            update.playerId = playerContext.playerId;
            update.type = "Choice";
            update.card = null;
            update.systemTrackSegmentChoice = segmentIndex;
            update.systemTrackTokenChoice = tokenIndex;
            submitUpdate(update);
        }
    }

    systemTrack.update();
}

function drawCardFrame(cardContext, type) {
    if (type == "Miner") {
        cardContext.drawFrame("/content/images/onslaught/minerframe.png");
    }
    else if (type == "Fleet") {
        cardContext.drawFrame("/content/images/onslaught/fleetframe.png");
    }
    else if (type == "Tech") {
        cardContext.drawFrame("/content/images/onslaught/techframe.png");
    }
    else {
        cardContext.drawFrame("/content/images/cardfront.png");
    }
}

function onslaught_draw() {    

    hand.drawCallback = function (cardContext, index) {
        var card = playerContext.hand[index];
        drawCardFrame(cardContext, card.type);

        cardContext.drawPortrait(card.url);
        if(playerContext.metal < card.metalCost_Play || playerContext.power < card.powerCost_Play || playerContext.crystal < card.crystalCost_Play)
            cardContext.drawFrame("/content/images/cardUsed.png");

        if(playerContext.state == "Choice" && card.validTarget == false)
            cardContext.drawFrame("/content/images/cardUsed.png");
    }

    playArea.drawCallback = function (cardContext, index) {
        drawCardFrame(cardContext, playerContext.playArea[index].type);
        cardContext.drawPortrait(playerContext.playArea[index].url);
    }


    hand.draw();
    playArea.draw();

    systemTrack.draw();
    for(var i = 0; i < opponentTracks.length; i++)
    {
        opponentTracks[i].draw();
    }
    invasionTrack.draw();

    for (var i = 0; i < supplyPiles.length; i++) {
        supplyPiles[i].drawCallback = function (cardContext, index) {
            var card = supplyPiles[i].contents[index];
            drawCardFrame(cardContext, card.type);
            if (card.url != null)
                cardContext.drawPortrait(card.url);
            if(playerContext.metal < card.metalCost_Buy || playerContext.power < card.powerCost_Buy || playerContext.crystal < card.crystalCost_Buy)
                cardContext.drawFrame("/content/images/cardUsed.png");

            if(playerContext.state == "Choice" && card.validTarget == false)
                cardContext.drawFrame("/content/images/cardUsed.png");
        };        
    }

    invasionDiscard.drawCallback = function (cardContext, index) {
        var card = gameState.invasionDiscard[gameState.invasionDiscard.length-1];
        cardContext.drawFrame("/content/images/cardfront.png");
        if (card.url != null)
                cardContext.drawPortrait(card.url);
    }

    for (var i = 0; i < supplyPiles.length; i++) {
        supplyPiles[i].draw();
    }
    invasionDiscard.draw();

    // Draw Resource Values
    for (var i = 0; i < playerContext.metal; i++) {
        resourceCounters.draw(new vMath.vector2(resourceLocation.x, resourceLocation.y + i * 10), "/content/images/onslaught/metal.png");
    }
    for (var i = 0; i < playerContext.crystal; i++) {
        resourceCounters.draw(new vMath.vector2(resourceLocation.x + 50, resourceLocation.y + i * 10), "/content/images/onslaught/crystal.png");
    }
    for (var i = 0; i < playerContext.power; i++) {
        resourceCounters.draw(new vMath.vector2(resourceLocation.x+100, resourceLocation.y + i * 10), "/content/images/onslaught/power.png");
    }

    if (infoCard != null) {
        infoWindow.clear();
        infoWindow.addText(infoCard.name, 5);
        infoWindow.addGap(5);
        infoWindow.addText(infoCard.description, 5);
        infoWindow.draw();
    }
}