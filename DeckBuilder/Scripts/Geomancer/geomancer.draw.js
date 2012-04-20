function geomancer_draw() {

    gameCore.spriteContext.text("CURRENT MANA: " + currentMana, new vMath.vector2(20,20));

    hexBoard.drawCallback = function (hexTile, a, b) {
        var tile = gameState.tileList[a][b];

        if (tile != null) {
            var stackLevel = 0;


            if (tile.type == "Barren") {
                hexTile.drawFrame("/content/images/barren.png", 0, stackLevel * 20);
            }
            else {
                hexTile.drawFrame("/content/images/hexagon.png", 0, stackLevel * 20);
            }
            if (tile.crystal != null) {
                if (tile.crystal.playerId == currentPlayerId) {
                    hexTile.drawFrame("/content/images/blueframe.png", 0, stackLevel * 20);
                    if (activePlayerId != currentPlayerId)
                        hexTile.drawFrame("/content/images/hexagonused.png", 0, stackLevel * 20);
                }
                else
                    hexTile.drawFrame("/content/images/orangeFrame.png", 0, stackLevel * 20);
                hexTile.drawPortrait(tile.crystal.url, 0, stackLevel * 20);
                if (tile.crystal.used == true) {
                    hexTile.drawFrame("/content/images/hexagonused.png", 0, stackLevel * 20);
                }
                stackLevel++;
            }
            if (tile.unit != null) {
                if (tile.unit.playerId == currentPlayerId) {
                    hexTile.drawFrame("/content/images/blueframe.png", 0, stackLevel * 20);
                    if (activePlayerId != currentPlayerId)
                        hexTile.drawFrame("/content/images/hexagonused.png", 0, stackLevel * 20);
                }
                else
                    hexTile.drawFrame("/content/images/orangeFrame.png", 0, stackLevel * 20);
                if (tile.unit.used == true) {
                    hexTile.drawFrame("/content/images/hexagonused.png", 0, stackLevel * 20);
                }

                hexTile.drawPortrait(tile.unit.url, 0, stackLevel * 20);

                for (var i = 0; i < 6; i++) {
                    if (tile.unit.awareness[i] == 'a')
                        hexTile.drawFrame("/content/images/attackedge.png", (tile.unit.direction + i - 1), stackLevel * 20);
                    if (tile.unit.awareness[i] == 'd')
                        hexTile.drawFrame("/content/images/awarenessedge.png", (tile.unit.direction + i - 1), stackLevel * 20);
                }

                stackLevel++;
            }
            if (tile.moveUnit != null) {
                if (tile.moveUnit.playerId == currentPlayerId)
                    hexTile.drawFrame("/content/images/blueframe.png", 0, stackLevel * 20);
                else
                    hexTile.drawFrame("/content/images/orangeFrame.png", 0, stackLevel * 20);
                hexTile.drawPortrait(tile.moveUnit.url, 0, stackLevel * 20);
                for (var i = 0; i < 6; i++) {
                    if (tile.moveUnit.awareness[i] == 'a')
                        hexTile.drawFrame("/content/images/attackedge.png", (tile.moveUnit.direction + i - 1), stackLevel * 20);
                    if (tile.moveUnit.awareness[i] == 'd')
                        hexTile.drawFrame("/content/images/awarenessedge.png", (tile.moveUnit.direction + i - 1), stackLevel * 20);
                }
                if (tile.moveUnit.attacking == true) {
                    hexTile.drawFrame("/content/images/attackarrow.png", tile.moveUnit.direction, stackLevel * 20);
                }
                stackLevel++;
            }
            if (tile.spell != null) {
                if (tile.spell.playerId == currentPlayerId)
                    hexTile.drawFrame("/content/images/blueframe.png", 0, stackLevel * 20);
                else
                    hexTile.drawFrame("/content/images/orangeFrame.png", 0, stackLevel * 20);

                hexTile.drawPortrait(tile.spell.url, 0, stackLevel * 20);
                for (var i = 0; i < 6; i++) {
                    if (tile.spell.awareness[i] == 'a')
                        hexTile.drawFrame("/content/images/attackedge.png", (tile.spell.direction + i - 1), stackLevel * 20);
                    if (tile.spell.awareness[i] == 'd')
                        hexTile.drawFrame("/content/images/awarenessedge.png", (tile.spell.direction + i - 1), stackLevel * 20);
                }

                stackLevel++;
            }
            if (overlay != null) {
                if (overlay.grid[a][b] == "R") {
                    hexTile.drawOverlay("/content/images/redoverlay.png", 0, 0);
                }
                if (overlay.grid[a][b] == "G") {
                    hexTile.drawOverlay("/content/images/greenoverlay.png", 0, 0);
                }
                if (overlay.grid[a][b] == "B") {
                    hexTile.drawOverlay("/content/images/blueoverlay.png", 0, 0);
                }
                if (overlay.grid[a][b] == "Y") {
                    hexTile.drawOverlay("/content/images/yellowoverlay.png", 0, 0);
                }
            }
            if (a == hexBoard.hoverA && b == hexBoard.hoverB) {
                if (selectionState == "Move") {
                    if (hexBoard.hoverEdge == true && GetAttackTarget(a, b, hexBoard.hoverDirection)) {
                        hexTile.drawFrame("/content/images/attackarrow.png", hexBoard.hoverDirection, 0);
                    }
                    else {
                        hexTile.drawFrame("/content/images/movearrow.png", hexBoard.hoverDirection, 0);
                    }
                }
                else {
                    hexTile.drawFrame("/content/images/attackedge.png", hexBoard.hoverDirection, 0);
                }
            }


        }
    }

    hexBoard.draw();

    playerHand.drawCallback = function (cardInstance, index) {
        var offSet = 0;
        if (playerContext.hand[index].used == true)
            offSet = 10;
        cardInstance.drawFrame("/content/images/cardfront.png", offSet);
        cardInstance.drawPortrait(playerContext.hand[index].url, offSet);
        cardInstance.drawText(playerContext.hand[index].cost, 20, 0, offSet);
        if (playerContext.hand[index].used == true)
            cardInstance.drawFrame("/content/images/cardused.png", offSet);
    }

    playerHand.draw();


    if (vMath.mousePos.x > 550) {
        hoverInfo.location.x = 100;
        hoverInfo.location.y = 500;
        selectInfo.location.x = 100;
        selectInfo.location.y = 300;
    }
    if (vMath.mousePos.x < 250) {
        hoverInfo.location.x = 700;
        hoverInfo.location.y = 700;
        selectInfo.location.x = 700;
        selectInfo.location.y = 500;
    }



    if (playerHand.selectIndex != null || hexBoard.selectA != null) {
        if (playerHand.selectIndex != null) {
            selectInfo.clear();
            selectInfo.addText(playerContext.hand[playerHand.selectIndex].name, 0);
            selectInfo.addText("Cost: " + playerContext.hand[playerHand.selectIndex].cost, 0);
            selectInfo.addText("Type: " + playerContext.hand[playerHand.selectIndex].type, 0);
            selectInfo.addText(playerContext.hand[playerHand.selectIndex].description, 10);
        }
        if (hexBoard.selectA != null) {
            selectInfo.clear();
            var selectTile = gameState.tileList[hexBoard.selectA][hexBoard.selectB];
            if (selectTile != null) {
                if (selectTile.spell != null) {
                    selectInfo.addText(selectTile.spell.name, 0);
                    selectInfo.addGap(5);
                }
                if (selectTile.crystal != null) {
                    selectInfo.addText(selectTile.crystal.name, 0);
                    selectInfo.addText("Mana: " + selectTile.crystal.mana, 10);
                    selectInfo.addText("Range: " + selectTile.crystal.range, 10);
                    selectInfo.addGap(5);
                }
                if (selectTile.unit != null) {
                    selectInfo.addText(selectTile.unit.name, 0);
                    selectInfo.addText("HP: " + selectTile.unit.hp + "/" + selectTile.unit.maxHP, 10);
                    selectInfo.addText("Attack: " + selectTile.unit.attack, 10);
                    selectInfo.addText("Defense: " + selectTile.unit.defense, 10);
                    selectInfo.addText("Speed: " + selectTile.unit.speed, 10);
                    selectInfo.addGap(5);
                }
            }
        }
        selectInfo.draw();
    }
    if (playerHand.hoverIndex != null || hexBoard.hoverA != null) {
        if (playerHand.hoverIndex != null) {
            hoverInfo.clear();
            hoverInfo.addText(playerContext.hand[playerHand.hoverIndex].name, 0);
            hoverInfo.addText("Cost: " + playerContext.hand[playerHand.hoverIndex].cost, 10);
            hoverInfo.addText("Type: " + playerContext.hand[playerHand.hoverIndex].type, 10);
            hoverInfo.addText(playerContext.hand[playerHand.hoverIndex].description, 10);
        }
        if (hexBoard.hoverA != null) {
            hoverInfo.clear();
            var hoverTile = gameState.tileList[hexBoard.hoverA][hexBoard.hoverB];
            if (hoverTile != null) {
                if (hoverTile.spell != null) {
                    hoverInfo.addText(hoverTile.spell.name, 0);
                    hoverInfo.addGap(5);
                }
                if (hoverTile.crystal != null) {
                    hoverInfo.addText(hoverTile.crystal.name, 0);
                    hoverInfo.addText("Mana: " + hoverTile.crystal.mana, 10);
                    hoverInfo.addText("Range: " + hoverTile.crystal.range, 10);
                    hoverInfo.addGap(5);
                }
                if (hoverTile.unit != null) {
                    hoverInfo.addText(hoverTile.unit.name, 0);
                    hoverInfo.addText("HP: " + hoverTile.unit.hp + "/" + hoverTile.unit.maxHP, 10);
                    hoverInfo.addText("Attack: " + hoverTile.unit.attack, 10);
                    hoverInfo.addText("Defense: " + hoverTile.unit.defense, 10);
                    hoverInfo.addText("Speed: " + hoverTile.unit.speed, 10);
                    hoverInfo.addGap(5);
                }
            }
        }
        hoverInfo.draw();
    }
}

function GetAttackTarget(a, b, direction) {
    var attackTile = AwarenessMap(a, b, direction + 1);
    if (gameState.tileList[attackTile.x] != null && gameState.tileList[attackTile.x][attackTile.y] != null && gameState.tileList[attackTile.x][attackTile.y].unit != null && gameState.tileList[attackTile.x][attackTile.y].unit.playerId != currentPlayerId) {
        return gameState.tileList[attackTile.x][attackTile.y].unit;
    }
    return null;
}