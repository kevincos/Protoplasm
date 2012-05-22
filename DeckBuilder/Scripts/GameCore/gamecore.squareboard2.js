function SquareBoard2(core, srcBoard) {
    this.type = "SquareBoard";
    this.gameCore = core;
    this.board = srcBoard.grid;
    this.width = srcBoard.width;
    this.height = srcBoard.height;
    this.sizeX = srcBoard.xSize;
    this.sizeY = srcBoard.ySize;
    this.name = srcBoard.name;
    this.location = new vMath.vector2(srcBoard.x, srcBoard.y);

    this.hoverX = null;
    this.hoverY = null;
    this.hoverDirection = null;
    this.hoverEdge = false;

    this.selectX = null;
    this.selectY = null;
    this.selectDirection = null;

    //this.tileSize = this.sizeX / this.width;
    this.tileSize = srcBoard.xTileSize;
    this.expandFactor = 1.2;
    //this.imageSize = this.sizeX / this.width;
    this.imageSize = this.tileSize;
    this.sizeX = this.width * this.tileSize;
    this.sizeY = this.height * this.tileSize;
};

// Update
SquareBoard2.prototype.update = function () {
    var mouseCoords = new vMath.vector2((vMath.mousePos.x - this.location.x + .5 * this.tileSize * this.width - .5 * this.tileSize) / this.tileSize, (vMath.mousePos.y - this.location.y + .5 * this.tileSize * this.height - .5 * this.tileSize) / this.tileSize);
    var hoverX = Math.round(mouseCoords.x);
    var hoverY = Math.round(mouseCoords.y);

    var hoverDirection = 0;
    var relX = hoverX - mouseCoords.x;
    var relY = hoverY - mouseCoords.y;

    if (relY >= Math.abs(relX))
        hoverDirection = 0;
    else if (relY <= -Math.abs(relX))
        hoverDirection = 2;
    else if (relX >= Math.abs(relY))
        hoverDirection = 1;
    else
        hoverDirection = 3;


    var hoverRel = new vMath.vector2(relX, relY);
    var hoverEdge = false;
    var hoverRelDist = Math.sqrt(hoverRel.x * hoverRel.x + hoverRel.y * hoverRel.y);
    if (hoverRelDist > 20)
        hoverEdge = true;
    else
        hoverEdge = false;

    if (hoverX < 0 || hoverX >= this.width || hoverY < 0 || hoverY >= this.height) {
        if (this.hoverX != null)
            this.gameCore.refreshView = true;
        this.hoverX = null;
        this.hoverY = null;
        this.hoverDirection = null;
        this.hoverEdge = false;
        return;
    }

    if (hoverX != this.hoverX || hoverY != this.hoverY || hoverDirection != this.hoverDirection || hoverEdge != this.hoverEdge) {
        this.gameCore.refreshView = true;
    }

    this.hoverX = hoverX;
    this.hoverY = hoverY;
    this.hoverDirection = hoverDirection;
    this.hoverEdge = hoverEdge;


    if (vMath.mouseDown == true && this.gameCore.mouseDown == false && this.board[this.hoverX][this.hoverY].selectable == true) {
        var update = {};
        update.selectX = this.hoverX;
        update.selectY = this.hoverY;
        update.selectObjectName = this.name;
        update.playerIndex = currentPlayerIndex;
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
SquareBoard2.prototype.draw = function (location, elevation, hover, select, inactive) {
    for (var x = 0; x < this.width; x++) {
        for (var y = 0; y < this.height; y++) {

            // Draw Tile(s) at screen coordinates
            if ((this.hoverX != x || this.hoverY != y) && (this.selectX != x || this.selectY != y)) {
                var screenCoords = new vMath.vector2(this.location.x - .5 * this.width * this.tileSize + x * this.tileSize + .5 * this.tileSize, this.location.y - .5 * this.height * this.tileSize + y * this.tileSize + .5 * this.tileSize);
                var tile = this.board[x][y];
                var height = 0;
                this.gameCore.spriteContext.draw(new Sprite(tile.url), new vMath.vector2(screenCoords.x, screenCoords.y - height), this.tileSize, this.tileSize, 0, 1);
                for (var i = 0; i < tile.pieces.length; i++) {
                    height = i * 10;
                    direction = 0;
                    if (tile.pieces[i].direction != undefined) {
                        direction = tile.pieces[i].direction;
                    }
                    this.gameCore.spriteContext.draw(new Sprite(tile.pieces[i].url), new vMath.vector2(screenCoords.x, screenCoords.y - height), this.imageSize, this.imageSize, direction * Math.PI / 2, 1);
                    if (tile.pieces[i].tokens != undefined) {
                        for (var j = 0; j < tile.pieces[i].tokens.length; j++) {
                            this.gameCore.spriteContext.draw(new Sprite(tile.pieces[i].tokens[j].url), new vMath.vector2(screenCoords.x, screenCoords.y - height), this.imageSize, this.imageSize, direction * Math.PI / 2, 1);
                        }
                    }
                }
                if (tile.selectable == true)
                    this.gameCore.spriteContext.draw(new Sprite("/content/images/classic/tile_highlight.png"), new vMath.vector2(screenCoords.x, screenCoords.y - height), this.tileSize, this.tileSize, this.selectDirection * Math.PI / 2, 1);
            }

        }
    }

    // Select Tile
    if (this.selectX != null && this.selectX < this.width && this.selectX >= 0 && this.selectY >= 0 && this.selectY < this.height) {
        var screenCoords = new vMath.vector2(this.location.x - .5 * this.width * this.tileSize + this.selectX * this.tileSize + .5 * this.tileSize, this.location.y - .5 * this.height * this.tileSize + this.selectY * this.tileSize + .5 * this.tileSize);
        var tile = this.board[this.selectX][this.selectY];
        this.gameCore.spriteContext.draw(new Sprite(tile.url), new vMath.vector2(screenCoords.x, screenCoords.y - height), this.tileSize * this.expandFactor, this.tileSize * this.expandFactor, 0, 1);
        for (var i = 0; i < tile.pieces.length; i++) {
            height = i * 10;
            direction = 0;
            if (tile.pieces[i].direction != undefined) {
                direction = tile.pieces[i].direction;
            }
            this.gameCore.spriteContext.draw(new Sprite(tile.pieces[i].url), new vMath.vector2(screenCoords.x, screenCoords.y - height), this.imageSize * this.expandFactor, this.imageSize * this.expandFactor, direction * Math.PI / 2, 1);
        }
        if (tile.selectable == true)
            this.gameCore.spriteContext.draw(new Sprite("/content/images/classic/tile_highlight.png"), new vMath.vector2(screenCoords.x, screenCoords.y - height), this.tileSize * this.expandFactor, this.tileSize * this.expandFactor, 0, 1);
    }

    // Hover Tile
    if (this.hoverX != null && this.hoverX < this.width && this.hoverX >= 0 && this.hoverY >= 0 && this.hoverY < this.height) {
        var screenCoords = new vMath.vector2(this.location.x - .5 * this.width * this.tileSize + this.hoverX * this.tileSize + .5 * this.tileSize, this.location.y - .5 * this.height * this.tileSize + this.hoverY * this.tileSize + .5 * this.tileSize);
        var tile = this.board[this.hoverX][this.hoverY];

        this.gameCore.spriteContext.draw(new Sprite(tile.url), new vMath.vector2(screenCoords.x, screenCoords.y - height), this.tileSize * this.expandFactor, this.tileSize * this.expandFactor, 0, 1);
        for (var i = 0; i < tile.pieces.length; i++) {
            height = i * 10;
            direction = 0;
            if (tile.pieces[i].direction != undefined) {
                direction = tile.pieces[i].direction;
            }
            this.gameCore.spriteContext.draw(new Sprite(tile.pieces[i].url), new vMath.vector2(screenCoords.x, screenCoords.y - height), this.imageSize * this.expandFactor, this.imageSize * this.expandFactor, direction * Math.PI / 2, 1);
            if (tile.pieces[i].hoverWindow != undefined) {
                gameCore.UpdateInfoWindow(tile.pieces[i].hoverWindow, tile.pieces[i].hoverText);
            }
        }
        if (tile.selectable == true)
            this.gameCore.spriteContext.draw(new Sprite("/content/images/classic/tile_highlight.png"), new vMath.vector2(screenCoords.x, screenCoords.y - height), this.tileSize * this.expandFactor, this.tileSize * this.expandFactor, 0, 1);
        if (tile.hoverWindow != undefined) {
            gameCore.UpdateInfoWindow(tile.hoverWindow, tile.hoverText);
        }


    }
};

