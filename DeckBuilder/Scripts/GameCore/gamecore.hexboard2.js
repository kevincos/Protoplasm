function HexBoard2(core, srcBoard) {
    this.type = "HexBoard";
    this.name = srcBoard.name;
    this.gameCore = core;

    this.center  = new vMath.vector2(0,0)
    this.center.x = srcBoard.x;
    this.center.y = srcBoard.y;

    this.board = srcBoard.grid;

    this.boundary = 2 * srcBoard.sideLength - 1;
    this.maxCoord = 3 * srcBoard.sideLength - 3;

    this.tileRadius = srcBoard.radius;
    this.hexRadius = srcBoard.radius / 2.412;
    this.expandFactor = 1.2;
    this.imageSize = srcBoard.imageSize;

    var totalX = 0;
    var totalY = 0;
    var tileCount = 0;
    for (var a = 0; a < this.boundary; a++) {
        for (var b = 0; b < this.boundary; b++) {
            var c = this.maxCoord - a - b;
            if (c >= 0 && c < this.boundary) {
                var tileCenter = hexMath.toScreenCoords(new vMath.vector2(0,0), a, b, this.hexRadius);
                totalX+=tileCenter.x;
                totalY+=tileCenter.y;
                tileCount++;
            }
        }
    }
    var defaultCenter = new vMath.vector2(totalX / tileCount, totalY / tileCount);
    this.origin = new vMath.vector2(this.center.x - defaultCenter.x, this.center.y - defaultCenter.y);


    this.hoverA = null;
    this.hoverB = null;
    this.hoverC = null;
    this.hoverDirection = null;
    this.hoverEdge = false;

    this.selectA = null;
    this.selectB = null;    
    this.selectDirection = null;

    this.selectCallback = null;
    this.deselectCallback = null;
    this.hoverCallback = null;
    this.clickCallback = null;
    this.drawTileCallback = null;
};

// Update
HexBoard2.prototype.update = function () {
    var hexMouseCoords = hexMath.toHexCoords(this.origin, vMath.mousePos.x, vMath.mousePos.y, this.hexRadius);
    var c = this.maxCoord - hexMouseCoords.x - hexMouseCoords.y;
    var hoverA = Math.round(hexMouseCoords.x);
    var hoverB = Math.round(hexMouseCoords.y);
    var hoverC = this.maxCoord - hoverA - hoverB;

    var hoverDirection = 0;
    var relA = hoverA - hexMouseCoords.x;
    var relB = hoverB - hexMouseCoords.y;
    var relC = hoverC - c;
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
    var hoverTileCenter = hexMath.toScreenCoords(this.origin, hoverA, hoverB, this.hexRadius);
    var hoverRel = new vMath.vector2(hoverTileCenter.x - vMath.mousePos.x, hoverTileCenter.y - vMath.mousePos.y);
    var hoverEdge = false;
    var hoverRelDist = Math.sqrt(hoverRel.x * hoverRel.x + hoverRel.y * hoverRel.y);
    if (hoverRelDist > 20)
        hoverEdge = true;
    else
        hoverEdge = false;

    if (hoverA < 0 || hoverA >= this.boundary || hoverB < 0 || hoverB >= this.boundary || hoverC < 0 || hoverC >= this.boundary) {
        if (this.hoverA != null)
            this.gameCore.refreshView = true;
        this.hoverA = null;
        this.hoverB = null;
        this.hoverC = null;
        this.hoverDirection = null;
        this.hoverEdge = false;
        return;
    }

    if (hoverA != this.hoverA || hoverB != this.hoverB || hoverC != this.hoverC || hoverDirection != this.hoverDirection || hoverEdge != this.hoverEdge) {
        this.gameCore.refreshView = true;
    }

    this.hoverA = hoverA;
    this.hoverB = hoverB;
    this.hoverC = hoverC;
    this.hoverDirection = hoverDirection;
    this.hoverEdge = hoverEdge;
    if (vMath.mouseDown == true && this.gameCore.mouseDown == false && this.board[this.hoverA][this.hoverB].selectable == true) {        
        var update = {};
        update.selectX = this.hoverA;
        update.selectY = this.hoverB;
        update.selectObjectName = this.name;
        update.playerId = currentPlayerId;
        var updateUrl = '/table/updateMain/' + gameState.tableId;
        $.ajax({
            url: updateUrl,
            type: 'POST',
            dataType: 'json',
            data: JSON.stringify(update),
            contentType: 'application/json; charset=utf-8'
        });
    }

}

// Render
HexBoard2.prototype.draw = function (location, elevation, hover, select, inactive) {
    for (var a = 0; a < this.boundary; a++) {
        for (var b = 0; b < this.boundary; b++) {
            var c = this.maxCoord - a - b;
            if (c >= 0 && c < this.boundary) {
                // Draw Hex Tile(s) at screen coordinates + angle
                if ((this.hoverA != a || this.hoverB != b) && (this.selectA != a || this.selectB != b)) {
                    var tile = this.board[a][b];
                    var height = 0;
                    var screenCoords = hexMath.toScreenCoords(this.origin, a, b, this.hexRadius);
                    this.gameCore.spriteContext.draw(new Sprite(tile.url), new vMath.vector2(screenCoords.x, screenCoords.y - height), this.tileRadius, this.tileRadius, hexMath.theta, 1);

                    for (var i = 0; i < tile.pieces.length; i++) {
                        height = i * 10;
                        direction = 0;
                        if (tile.pieces[i].direction != undefined) {
                            direction = tile.pieces[i].direction;
                        }
                        if (tile.pieces[i].baseUrls != undefined) {
                            for (var j = 0; j < tile.pieces[i].baseUrls.length; j++) {
                                this.gameCore.spriteContext.draw(new Sprite(tile.pieces[i].baseUrls[j]), new vMath.vector2(screenCoords.x, screenCoords.y - height), this.tileRadius, this.tileRadius, hexMath.theta, 1);
                            }
                        }
                        this.gameCore.spriteContext.draw(new Sprite(tile.pieces[i].url), new vMath.vector2(screenCoords.x, screenCoords.y - height), this.tileRadius, this.tileRadius, hexMath.theta, 1);
                        if (tile.pieces[i].tokens != undefined) {
                            for (var j = 0; j < tile.pieces[i].tokens.length; j++) {
                                this.gameCore.spriteContext.draw(new Sprite(tile.pieces[i].tokens[j].url), new vMath.vector2(screenCoords.x, screenCoords.y - height), this.tileRadius, this.tileRadius, hexMath.theta, 1);
                            }
                        }
                    }

                    if (tile.selectable == true && tile.highlightUrl != "")
                        this.gameCore.spriteContext.draw(new Sprite(tile.highlightUrl), new vMath.vector2(screenCoords.x, screenCoords.y - height), this.tileRadius, this.tileRadius, hexMath.theta, 1);
                }
            }
        }
    }

    // Select Tile
    if (this.selectA != null && this.selectA < this.boundary && this.selectA >= 0 && this.selectB >= 0 && this.selectB < this.boundary) {
        var screenCoords = hexMath.toScreenCoords(this.origin, this.selectA, this.selectB, this.hexRadius);
        var tile = this.board[this.selectA][this.selectB];
        var height = 0;

        this.gameCore.spriteContext.draw(new Sprite(tile.url), new vMath.vector2(screenCoords.x, screenCoords.y - height), this.tileRadius * this.expandFactor, this.tileRadius * this.expandFactor, hexMath.theta, 1);

        if (tile.selectable == true && tile.highlightUrl != "")
            this.gameCore.spriteContext.draw(new Sprite(tile.highlightUrl), new vMath.vector2(screenCoords.x, screenCoords.y - height), this.tileRadius, this.tileRadius, hexMath.theta, 1);
    }

    // Hover Tile
    if (this.hoverA != null && this.hoverA < this.boundary && this.hoverA >= 0 && this.hoverB >= 0 && this.hoverB < this.boundary) {
        var screenCoords = hexMath.toScreenCoords(this.origin, this.hoverA, this.hoverB, this.hexRadius);
        var tile = this.board[this.hoverA][this.hoverB];
        var height = 0;

        if (tile.hoverWindow != undefined) {
            gameCore.UpdateInfoWindow(tile.hoverWindow, tile.hoverText);
        }
        this.gameCore.spriteContext.draw(new Sprite(tile.url), new vMath.vector2(screenCoords.x, screenCoords.y - height), this.tileRadius * this.expandFactor, this.tileRadius * this.expandFactor, hexMath.theta, 1);

        for (var i = 0; i < tile.pieces.length; i++) {
            height = i * 10;
            direction = 0;
            if (tile.pieces[i].direction != undefined) {
                direction = tile.pieces[i].direction;
            }
            if (tile.pieces[i].baseUrls != undefined) {
                for (var j = 0; j < tile.pieces[i].baseUrls.length; j++) {
                    this.gameCore.spriteContext.draw(new Sprite(tile.pieces[i].baseUrls[j]), new vMath.vector2(screenCoords.x, screenCoords.y - height), this.tileRadius * this.expandFactor, this.tileRadius * this.expandFactor, hexMath.theta, 1);
                }
            }            
            if (tile.pieces[i].hoverWindow != undefined) {
                gameCore.UpdateInfoWindow(tile.pieces[i].hoverWindow, tile.pieces[i].hoverText);
            }
            this.gameCore.spriteContext.draw(new Sprite(tile.pieces[i].url), new vMath.vector2(screenCoords.x, screenCoords.y - height), this.tileRadius * this.expandFactor, this.tileRadius * this.expandFactor, hexMath.theta, 1);
            if (tile.pieces[i].tokens != undefined) {
                for (var j = 0; j < tile.pieces[i].tokens.length; j++) {
                    this.gameCore.spriteContext.draw(new Sprite(tile.pieces[i].tokens[j].url), new vMath.vector2(screenCoords.x, screenCoords.y - height), this.tileRadius * this.expandFactor, this.tileRadius * this.expandFactor, hexMath.theta, 1);
                }
            }
        }

        if (tile.selectable == true && tile.highlightUrl != "")
            this.gameCore.spriteContext.draw(new Sprite(tile.highlightUrl), new vMath.vector2(screenCoords.x, screenCoords.y - height), this.tileRadius * this.expandFactor, this.tileRadius * this.expandFactor, hexMath.theta, 1);
    }
};