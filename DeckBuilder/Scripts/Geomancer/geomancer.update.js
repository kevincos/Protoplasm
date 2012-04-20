function geomancer_update() {
    playerHand.clickCallback = function (item, index) {
        if (selectionState == "Normal") {
            // Select card, go to cast mode
            var castCard = playerContext.hand[index];

            if (castCard.used == true) {
                gameState.tileList[castCard.castA][castCard.castB].spell = null;
                currentMana += castCard.cost;
                castCard.used = false;
            }

            if (currentMana >= castCard.cost) {
                selectionState = "Cast";
                currentMana -= castCard.cost;
                if (castCard.type == "Crystal") {
                    overlay = ValidCrystalOverlay(castCard, currentPlayerId);
                }
                else if (castCard.type == "Summon") {
                    overlay = SummonRange(castCard, currentPlayerId);
                }
                else {
                    overlay = CastRange(castCard, currentPlayerId);
                }
                playerHand.select(index);
            }
        }
        else if (selectionState == "Cast") {
            currentMana += playerContext.hand[playerHand.selectIndex].cost;

            var castCard = playerContext.hand[index];
            castCard.used = false;
            selectionState = "Normal";
            if (playerHand.selectIndex == index) {
                // Unselect
                playerHand.deselect();
                overlay = null;
            }
            else if (currentMana >= castCard.cost) {
                currentMana -= castCard.cost;
                selectionState = "Cast";
                if (castCard.type == "Crystal") {
                    overlay = ValidCrystalOverlay(castCard, currentPlayerId);
                }
                else if (castCard.type == "Summon") {
                    overlay = SummonRange(castCard, currentPlayerId);
                }
                else {
                    overlay = CastRange(castCard, currentPlayerId);
                }
                playerHand.select(index);
            }
        }
        else if (selectionState == "Move") {
            if (currentMana >= playerContext.hand[index].cost) {
                overlay = null;
                currentMana -= playerContext.hand[index].cost;
                selectionState = "Normal";
                var castCard = playerContext.hand[index];
                selectionState = "Cast";
                if (castCard.type == "Crystal") {
                    overlay = ValidCrystalOverlay(castCard, currentPlayerId);
                }
                else if (castCard.type == "Summon") {
                    overlay = SummonRange(castCard, currentPlayerId);
                }
                else {
                    overlay = CastRange(castCard, currentPlayerId);
                }
                playerHand.select(index);
                hexBoard.deselect();
            }
        }
    };
    playerHand.update();


    hexBoard.clickCallback = function (item, a, b, direction) {
        if (selectionState == "Normal") {
            if (gameState.tileList[a][b].unit != null && gameState.tileList[a][b].unit.playerId == currentPlayerId) {
                // Select unit and go to move state
                if (gameState.tileList[a][b].unit.used == true) {
                    gameState.tileList[gameState.tileList[a][b].unit.moveA][gameState.tileList[a][b].unit.moveB].moveUnit = null;
                    gameState.tileList[a][b].unit.used = false;
                    gameState.tileList[a][b].unit.moveA = null;
                    gameState.tileList[a][b].unit.moveB = null;
                }
                overlay = ValidMoveOptions(a, b);
                hexBoard.select(a, b);
                selectionState = "Move";
            }
            else if (gameState.tileList[a][b].moveUnit != null && gameState.tileList[a][b].moveUnit.playerId == currentPlayerId) {
                // Select unit and go to move state
                gameState.tileList[gameState.tileList[a][b].moveUnit.moveA][gameState.tileList[a][b].moveUnit.moveB].used = false;
                overlay = ValidMoveOptions(gameState.tileList[a][b].moveUnit.moveA, gameState.tileList[a][b].moveUnit.moveB);
                hexBoard.select(gameState.tileList[a][b].moveUnit.moveA, gameState.tileList[a][b].moveUnit.moveB);
                gameState.tileList[a][b].moveUnit = null;
                selectionState = "Move";
            }
            else if (gameState.tileList[a][b].spell != null && gameState.tileList[a][b].spell.playerId == currentPlayerId) {
                // Select card and go to cast state
                playerContext.hand[gameState.tileList[a][b].spell.sourceCardIndex].used = false;
                playerHand.selectIndex = gameState.tileList[a][b].spell.sourceCardIndex;
                hexBoard.deselect();
                var castCard = playerContext.hand[gameState.tileList[a][b].spell.sourceCardIndex];
                if (castCard.type == "Summon") {
                    overlay = SummonRange(castCard, currentPlayerId);
                }
                if (castCard.type == "Crystal") {
                    overlay = ValidCrystalOverlay(castCard, currentPlayerId);
                }
                if (castCard.type == "Spell") {
                    overlay = CastRange(castCard, currentPlayerId);
                }
                gameState.tileList[a][b].spell = null;
                selectionState = "Cast";
            }
            else if (gameState.tileList[a][b].crystal != null && gameState.tileList[a][b].crystal.playerId == currentPlayerId) {
                // Activate/deactivate crystal and stay in normal state
                if (gameState.tileList[a][b].crystal.used == false) {
                    gameState.tileList[a][b].crystal.used = true;
                    currentMana += gameState.tileList[a][b].crystal.mana;
                }
                else if (gameState.tileList[a][b].crystal.used == true && currentMana >= gameState.tileList[a][b].crystal.mana) {
                    gameState.tileList[a][b].crystal.used = false;
                    currentMana -= gameState.tileList[a][b].crystal.mana;
                }
            }
            else {
                // Do nothing
            }
        }
        else if (selectionState == "Move") {
            // If valid move, then perform move and go to normal state
            if (overlay.grid[a][b] == "B") {
                gameState.tileList[hexBoard.selectA][hexBoard.selectB].unit.used = true;
                gameState.tileList[hexBoard.selectA][hexBoard.selectB].unit.moveA = a;
                gameState.tileList[hexBoard.selectA][hexBoard.selectB].unit.moveB = b;
                var selectedUnit = gameState.tileList[hexBoard.selectA][hexBoard.selectB].unit
                gameState.tileList[a][b].moveUnit = {};
                gameState.tileList[a][b].moveUnit.name = selectedUnit.name;
                gameState.tileList[a][b].moveUnit.awareness = selectedUnit.awareness;
                gameState.tileList[a][b].moveUnit.speed = selectedUnit.speed;
                gameState.tileList[a][b].moveUnit.attack = selectedUnit.attack;
                gameState.tileList[a][b].moveUnit.defense = selectedUnit.defense;
                gameState.tileList[a][b].moveUnit.hp = selectedUnit.hp;
                gameState.tileList[a][b].moveUnit.maxHP = selectedUnit.maxHP;
                gameState.tileList[a][b].moveUnit.direction = direction;
                gameState.tileList[a][b].moveUnit.playerId = selectedUnit.playerId;
                gameState.tileList[a][b].moveUnit.url = selectedUnit.url;
                gameState.tileList[a][b].moveUnit.moveA = hexBoard.selectA;
                gameState.tileList[a][b].moveUnit.moveB = hexBoard.selectB;
                if (hexBoard.hoverEdge == true && GetAttackTarget(a, b, direction) != null) {
                    gameState.tileList[a][b].moveUnit.attacking = true;
                }
                hexBoard.deselect();
                selectionState = "Normal";
                overlay = null;
            }
        }
        else if (selectionState == "Cast") {
            // If valid cast, then create summon and go to normal state
            if (overlay.grid[a][b] == "G") {
                playerContext.hand[playerHand.selectIndex].used = true;
                playerContext.hand[playerHand.selectIndex].castA = a;
                playerContext.hand[playerHand.selectIndex].castB = b;
                gameState.tileList[a][b].spell = {};
                gameState.tileList[a][b].spell.name = playerContext.hand[playerHand.selectIndex].name;
                gameState.tileList[a][b].spell.awareness = "dad___";
                gameState.tileList[a][b].spell.direction = direction;
                gameState.tileList[a][b].spell.url = playerContext.hand[playerHand.selectIndex].url;
                gameState.tileList[a][b].spell.sourceCardIndex = playerHand.selectIndex;
                gameState.tileList[a][b].spell.playerId = activePlayerId;
                playerHand.selectIndex = null;
                hexBoard.deselect();
                overlay = null;
                selectionState = "Normal";
            }
        }

    }
  
    hexBoard.update();
};