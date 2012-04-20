function SquareBoard(core, boardX, boardY, length, width, tileSize, imageSize, expandFactor) {
    this.gameCore = core;
    this.board = [];
    this.length = length;
    this.width = width;
    this.location = new vMath.vector2(boardX, boardY);

    for (var x = 0; x < length; x++) {
        this.board[x] = [];
        for (var y = 0; y < width; y++) {
            this.board[x][y] = [];
        }
    }
    this.hoverX = null;
    this.hoverY = null;
    this.hoverDirection = null;
    this.hoverEdge = false;

    this.selectX = null;
    this.selectY = null;
    this.selectDirection = null;

    this.tileSize = tileSize;
    this.expandFactor = expandFactor;
    this.imageSize = imageSize;

    this.selectCallback = null;
    this.deselectCallback = null;
    this.hoverCallback = null;
    this.clickCallback = null;
    this.drawTileCallback = null;
};

// Add a tile to the stack at this location
SquareBoard.prototype.addTile = function () {
}

// Remove a tile from the stack at this location
SquareBoard.prototype.removeTile = function () {
}

SquareBoard.prototype.deselect = function () {
    var oldX = this.selectX;
    var oldY = this.selectY;
    if (oldX != null && oldY != null && this.deselectCallback != null) {
        this.deselectCallback(null, this.hoverX, this.hoverY, this.hoverDirection, oldX, oldY);
    }
    this.selectX = null;
    this.selectY = null;
    this.selectDirection = null;
}

SquareBoard.prototype.select = function (x, y) {
    this.selectX = x;
    this.selectY = y;
    this.selectDirection = 0;
    if (this.selectCallback != null) {
        this.selectCallback(null, this.selectX, this.selectY, this.selectDirection, x, y);
    }
}

// Update
SquareBoard.prototype.update = function () {
    var mouseCoords = new vMath.vector2((vMath.mousePos.x - this.location.x + .5 * this.tileSize * this.length - .5 * this.tileSize) / this.tileSize, (vMath.mousePos.y - this.location.y + .5 * this.tileSize * this.width - .5 * this.tileSize) / this.tileSize);
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

    if (hoverX < 0 || hoverX >= this.length || hoverY < 0 || hoverY >= this.width) {
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

    if (vMath.mouseDown == true && this.gameCore.mouseDown == false) {
        if (this.clickCallback != null) {
            this.clickCallback(null, this.hoverX, this.hoverY, this.hoverDirection);
        }
        else if (this.hoverX == this.selectX && this.hoverY == this.selectY && this.hoverDirection == this.selectDirection) {
            //DESELECT
            var oldX = this.selectY;
            var oldX = this.selectY;

            this.selectX = null;
            this.selectY = null;
            this.selectDirection = null;
            this.gameCore.refreshView = true;
            if (this.deselectCallback != null) {
                this.deselectCallback(null, this.hoverX, this.hoverU, this.hoverDirection, oldX, oldY);
            }
        }
        else {
            // SELECT
            var oldX = this.selectX;
            var oldY = this.selectY;
            if (oldX != null && oldY != null && this.deselectCallback != null) {
                this.deselectCallback(null, this.hoverX, this.hoverY, this.hoverDirection, oldX, oldY);
            }

            this.selectX = this.hoverX;
            this.selectY = this.hoverY;
            this.selectDirection = this.hoverDirection;
            this.gameCore.refreshView = true;
            if (this.selectCallback != null) {
                this.selectCallback(null, this.selectX, this.selectY, this.selectDirection, oldX, oldY);
            }
        }
    }

}

// Render
SquareBoard.prototype.draw = function (location, elevation, hover, select, inactive) {
    for (var x = 0; x < this.length; x++) {
        for (var y = 0; y < this.width; y++) {

            // Draw Tile(s) at screen coordinates
            if ((this.hoverX != x || this.hoverY != y) && (this.selectX != x || this.selectY != y)) {
                var screenCoords = new vMath.vector2(this.location.x - .5 * this.length * this.tileSize + x * this.tileSize + .5 * this.tileSize, this.location.y - .5 * this.width * this.tileSize + y * this.tileSize + .5 * this.tileSize);

                var tileContext = new SquareTile(this, this.tileSize, this.imageSize, screenCoords);
                if (this.drawCallback != null) {
                    this.drawCallback(tileContext, x, y);
                }
            }

        }
    }

    // Select Tile
    if (this.drawCallback != null && this.selectX != null && this.selectX < this.length && this.selectX >= 0 && this.selectY >= 0 && this.selectY < this.width) {
        var screenCoords = new vMath.vector2(this.location.x - .5 * this.length * this.tileSize + this.selectX * this.tileSize + .5 * this.tileSize, this.location.y - .5 * this.width * this.tileSize + this.selectY * this.tileSize + .5 * this.tileSize);
        var tileContext = new HexTile(this, this.tileSize * this.expandFactor, this.imageSize * this.expandFactor, screenCoords);
        this.drawCallback(tileContext, this.selectX, this.selectY);
    }

    // Hover Tile
    if (this.drawCallback != null && this.hoverX != null && this.hoverX < this.length && this.hoverX >= 0 && this.hoverY >= 0 && this.hoverY < this.width) {
        var screenCoords = new vMath.vector2(this.location.x - .5 * this.length * this.tileSize + this.hoverX * this.tileSize + .5 * this.tileSize, this.location.y - .5 * this.width * this.tileSize + this.hoverY * this.tileSize + .5 * this.tileSize); var tileContext = new SquareTile(this, this.tileSize * this.expandFactor, this.imageSize * this.expandFactor, screenCoords);
        this.drawCallback(tileContext, this.hoverX, this.hoverY);
    }
};




function SquareTile(parentBoard, tileSize, imageSize, location) {
    this.board = parentBoard;
    this.location = location;
    this.imageSize = imageSize;
    this.tileSize = tileSize;
};

SquareTile.prototype.drawOverlay = function (url, direction, height) {
    this.board.gameCore.spriteContext.draw(new Sprite(url), new vMath.vector2(this.location.x, this.location.y - height), this.tileSize * 1.1, this.tileSize * 1.1, direction * Math.PI / 2, 1);
};

SquareTile.prototype.drawFrame = function (url, direction, height) {
    this.board.gameCore.spriteContext.draw(new Sprite(url), new vMath.vector2(this.location.x, this.location.y - height), this.tileSize, this.tileSize, direction * Math.PI / 2, 1);
};

SquareTile.prototype.drawPortrait = function (url, direction, height) {
    this.board.gameCore.spriteContext.draw(new Sprite(url), new vMath.vector2(this.location.x, this.location.y - height), this.imageSize, this.imageSize, direction * Math.PI / 2, 1);
};



function SquareOverlay(url, length, width) {
    this.url = url;
    this.grid = [];
    this.width = width;
    this.length = length;
    for (var x = 0; x < this.length; x++) {
        this.grid[x] = [];
        for (var y = 0; y < this.width; y++) {
            this.grid[x][y] = [];
        }
    }
}