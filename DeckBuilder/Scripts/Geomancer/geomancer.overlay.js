function Mark(overlay, a, b, val, range) {
    if (gameState.tileList[a] == null || gameState.tileList[a][b] == null)
        return;
    overlay.grid[a][b] = val;
    if(range > 0)
    {
        range--;
        Mark(overlay, a + 1, b, val, range);
        Mark(overlay, a - 1, b, val, range);
        Mark(overlay, a, b + 1, val, range);
        Mark(overlay, a, b - 1, val, range);
        Mark(overlay, a + 1, b - 1, val, range);
        Mark(overlay, a - 1, b + 1, val, range);
    }
}

function MoveMark(overlay, a, b, startA, startB, range, playerId) {
    if (gameState.tileList[a] == null || gameState.tileList[a][b] == null)
        return;

    if (enemyOverlay.grid[a][b] == "R") {
        overlay.grid[a][b] = "R";
        return;
    }
    overlay.grid[a][b] = "B";
    if (!(a==startA && b==startB) && enemyOverlay.grid[a][b] == "Y") {
        return;
    }
    if (range > 0) {
        range--;
        MoveMark(overlay, a + 1, b, startA, startB, range, playerId);
        MoveMark(overlay, a - 1, b, startA, startB, range, playerId);
        MoveMark(overlay, a, b + 1, startA, startB, range, playerId);
        MoveMark(overlay, a, b - 1, startA, startB, range, playerId);
        MoveMark(overlay, a + 1, b - 1, startA, startB, range, playerId);
        MoveMark(overlay, a - 1, b + 1, startA, startB, range, playerId);
    }
}

function ValidCrystalOverlay(castCard, playerId) {
    overlay = new HexOverlay("/content/images/cardused.png");
    for (var a = 0; a < 14; a++) {
        for (var b = 0; b < 11; b++) {
            if(gameState.tileList[a][b] != null)
                overlay.grid[a][b] = "";
        }
    }
    for (var a = 0; a < 14; a++) {
        for (var b = 0; b < 11; b++) {
            if (gameState.tileList[a][b] != null && gameState.tileList[a][b].crystal != null && gameState.tileList[a][b].crystal.playerId == playerId) {
                Mark(overlay, a,b,"G", gameState.tileList[a][b].crystal.range);
            }
            if (gameState.tileList[a][b] != null && gameState.tileList[a][b].unit != null && gameState.tileList[a][b].unit.name == "HomeCrystal" && gameState.tileList[a][b].unit.playerId == playerId) {
                Mark(overlay, a, b, "G", 2);
            }     
        }
    }
    for (var a = 0; a < 14; a++) {
        for (var b = 0; b < 11; b++) {
            if (gameState.tileList[a][b] != null && gameState.tileList[a][b].crystal != null) {
                Mark(overlay, a, b, "R", 1);
            }
            if (gameState.tileList[a][b] != null && gameState.tileList[a][b].unit != null && gameState.tileList[a][b].unit.name == "HomeCrystal") {
                Mark(overlay, a, b, "R", 1);
            }   
        }
    }

    return overlay;
}

function AwarenessMap(a, b, dir) {
    dir = dir % 6;
    var tileCoords = new vMath.vector2(a,b);
    if (dir == 3) {
        tileCoords.x += 1;
    }
    if (dir == 4) {
        tileCoords.y += 1;
    }
    if (dir == 5) {
        tileCoords.x -= 1;
        tileCoords.y += 1;
    }
    if (dir == 0) {
        tileCoords.x -= 1;
    }
    if (dir == 1) {
        tileCoords.y -= 1;
    }
    if (dir == 2) {
        tileCoords.y -= 1;
        tileCoords.x += 1;
    }
    return tileCoords;
}

function EnemyUnitProfile(playerId) {
    enemyOverlay = new HexOverlay("/content/images/cardused.png");
    for (var a = 0; a < 14; a++) {
        for (var b = 0; b < 11; b++) {
            if (gameState.tileList[a][b] != null)
                enemyOverlay.grid[a][b] = "";
        }
    }
    for (var a = 0; a < 14; a++) {
        for (var b = 0; b < 11; b++) {
            if (gameState.tileList[a][b] != null && gameState.tileList[a][b].unit != null && gameState.tileList[a][b].unit.playerId != playerId) {
                enemyOverlay.grid[a][b] = "R";
                for (var i = 0; i < 6; i++) {
                    if (gameState.tileList[a][b].unit.awareness[i] == "a" || gameState.tileList[a][b].unit.awareness[i] == "d") {
                        var awarenessCoords = AwarenessMap(a, b, i + gameState.tileList[a][b].unit.direction);
                        enemyOverlay.grid[awarenessCoords.x][awarenessCoords.y] = "Y";
                    }
                }
            }
        }
    }
    return enemyOverlay;
}

function ValidMoveOptions(unitA, unitB) {
    overlay = new HexOverlay("/content/images/cardused.png");
    var unit = gameState.tileList[unitA][unitB].unit;
    enemyOverlay = EnemyUnitProfile(unit.playerId);

    for (var a = 0; a < 14; a++) {
        for (var b = 0; b < 11; b++) {
            if (gameState.tileList[a][b] != null)
                overlay.grid[a][b] = "";
        }
    }
    MoveMark(overlay, unitA, unitB, unitA, unitB, unit.speed, unit.playerId);
    for (var a = 0; a < 14; a++) {
        for (var b = 0; b < 11; b++) {
            if (gameState.tileList[a][b] != null && gameState.tileList[a][b].unit != null) {
                if(overlay.grid[a][b] != "" && a != unitA && b != unitB)
                    overlay.grid[a][b] = "R";
            }
        }
    }
    return overlay;
}

function SummonRange(castCard, playerId) {
    overlay = new HexOverlay("/content/images/cardused.png");
    for (var a = 0; a < 14; a++) {
        for (var b = 0; b < 11; b++) {
            if (gameState.tileList[a][b] != null && gameState.tileList[a][b].crystal != null && gameState.tileList[a][b].crystal.playerId == playerId) {
                Mark(overlay, a, b, "G", gameState.tileList[a][b].crystal.range)
            }
            if (gameState.tileList[a][b] != null && gameState.tileList[a][b].unit != null && gameState.tileList[a][b].unit.playerId == playerId && gameState.tileList[a][b].unit.name == "HomeCrystal") {
                Mark(overlay, a, b, "G", 2)
            }
        }
    }
    for (var a = 0; a < 14; a++) {
        for (var b = 0; b < 11; b++) {
            if (gameState.tileList[a][b] != null && gameState.tileList[a][b].unit != null) {                
                overlay.grid[a][b] = "R";
            }
            if (gameState.tileList[a][b] != null && gameState.tileList[a][b].crystal != null && gameState.tileList[a][b].crystal.playerId != playerId) {
                overlay.grid[a][b] = "R";
            }
        }
    }
    return overlay;
}

function CastRange(castCard, playerId) {
    overlay = new HexOverlay("/content/images/cardused.png");
    for (var a = 0; a < 14; a++) {
        for (var b = 0; b < 11; b++) {
            if (gameState.tileList[a][b] != null && gameState.tileList[a][b].crystal != null && gameState.tileList[a][b].crystal.playerId == playerId) {
                Mark(overlay, a, b, "G", gameState.tileList[a][b].crystal.range)
            }
            if (gameState.tileList[a][b] != null && gameState.tileList[a][b].unit != null && gameState.tileList[a][b].unit.playerId == playerId && gameState.tileList[a][b].unit.name == "HomeCrystal") {
                Mark(overlay, a, b, "G", 2)
            }
        }
    }
    return overlay;
}