function HexBoard(core, center, sideLength, radius, imageSize, expand) {
    this.center = center;
    
    this.gameCore = core;
    this.board = [];
    this.aMin = sideLength - 1; // 0
    this.bMin = sideLength - 1; // 0
    this.cMin = sideLength - 1; // 0 
    this.aMax = 3*sideLength - 2; // 14
    this.bMax = 3 * sideLength - 2; // 11
    this.cMax = 3 * sideLength - 2; // 14
    this.cBoundary = 6*(sideLength-1);
    for (var a = 0; a < this.aMax; a++) {
        this.board[a] = [];
        for (var b = 0; b < this.bMax; b++) {
            this.board[a][b] = [];
        }
    }


    this.tileRadius = radius;
    this.hexRadius = radius / 2.412;
    this.expandFactor = expand;
    this.imageSize = imageSize;

    var totalX = 0;
    var totalY = 0;
    var tileCount = 0;
    for (var a = this.aMin; a < this.aMax; a++) {
        for (var b = this.bMin; b < this.bMax; b++) {
            var c = this.cBoundary - a - b;
            if (c >= this.cMin && c < this.cMax) {
                var tileCenter = hexMath.toScreenCoords(new vMath.vector2(0,0), a, b, this.hexRadius);
                totalX+=tileCenter.x;
                totalY+=tileCenter.y;
                tileCount++;
            }
        }
    }
    var defaultCenter = new vMath.vector2(totalX / tileCount, totalY / tileCount);
    this.origin = new vMath.vector2(center.x - defaultCenter.x, center.y - defaultCenter.y);


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

// Add a tile to the stack at this location
HexBoard.prototype.addTile = function () {
}

// Remove a tile from the stack at this location
HexBoard.prototype.removeTile = function () {
}

HexBoard.prototype.deselect = function () {
    var oldA = this.selectA;
    var oldB = this.selectB;
    if (oldA != null && oldB != null && this.deselectCallback != null) {
        this.deselectCallback(null, this.hoverA, this.hoverB, this.hoverDirection, oldA, oldB);
    }
    this.selectA = null;
    this.selectB = null;
    this.selectDirection = null;
}

HexBoard.prototype.select = function (a, b) {
    this.selectA = a;
    this.selectB = b;
    this.selectDirection = 0;
    if (this.selectCallback != null) {
        this.selectCallback(null, this.selectA, this.selectB, this.selectDirection, a, b);
    }
}

// Update
HexBoard.prototype.update = function () {
    var hexMouseCoords = hexMath.toHexCoords(this.origin, vMath.mousePos.x, vMath.mousePos.y, this.hexRadius);
    var c = this.cBoundary - hexMouseCoords.x - hexMouseCoords.y;
    var hoverA = Math.round(hexMouseCoords.x);
    var hoverB = Math.round(hexMouseCoords.y);
    var hoverC = this.cBoundary - hoverA - hoverB;

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

    if (hoverA < this.aMin|| hoverA >= this.aMax || hoverB < this.bMin || hoverB >= this.bMax || hoverC < this.cMin || hoverC >= this.cMax) {
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
    if (vMath.mouseDown == true && this.gameCore.mouseDown == false) {
        if (this.clickCallback != null) {
            this.clickCallback(null, this.hoverA, this.hoverB, this.hoverDirection);
        }
        else if (this.hoverA == this.selectA && this.hoverB == this.selectB && this.hoverDirection == this.selectDirection) {
            //DESELECT
            var oldA = this.selectA;
            var oldB = this.selectB;

            this.selectA = null;
            this.selectB = null;
            this.selectDirection = null;
            this.gameCore.refreshView = true;
            if (this.deselectCallback != null) {
                this.deselectCallback(null, this.hoverA, this.hoverB, this.hoverDirection, oldA, oldB);
            }
        }
        else {
            // SELECT
            var oldA = this.selectA;
            var oldB = this.selectB;
            if (oldA != null && oldB != null && this.deselectCallback != null) {
                this.deselectCallback(null, this.hoverA, this.hoverB, this.hoverDirection, oldA, oldB);
            }

            this.selectA = this.hoverA;
            this.selectB = this.hoverB;
            this.selectDirection = this.hoverDirection;
            this.gameCore.refreshView = true;
            if (this.selectCallback != null) {
                this.selectCallback(null, this.selectA, this.selectB, this.selectDirection, oldA, oldB);
            }
        }
    }

}

// Render
HexBoard.prototype.draw = function (location, elevation, hover, select, inactive) {
    for (var a = this.aMin; a < this.aMax; a++) {
        for (var b = this.bMin; b < this.bMax; b++) {
            var c = this.cBoundary - a - b;
            if (c >= this.cMin && c < this.cMax) {
                // Draw Hex Tile(s) at screen coordinates + angle
                if ((this.hoverA != a || this.hoverB != b) && (this.selectA != a || this.selectB != b)) {
                    var screenCoords = hexMath.toScreenCoords(this.origin, a, b, this.hexRadius);

                    var tileContext = new HexTile(this, this.tileRadius, this.imageSize, hexMath.theta, screenCoords);
                    if (this.drawCallback != null) {
                        this.drawCallback(tileContext, a, b);
                    }
                }
            }
        }
    }

    // Select Tile
    if (this.drawCallback != null && this.selectA != null && this.selectA < this.aMax && this.selectA >= this.aMin && this.selectB >= this.bMin && this.selectB < this.bMax) {
        var screenCoords = hexMath.toScreenCoords(this.origin, this.selectA, this.selectB, this.hexRadius);
        var tileContext = new HexTile(this, this.tileRadius * this.expandFactor, this.imageSize * this.expandFactor, hexMath.theta, screenCoords);
        this.drawCallback(tileContext, this.selectA, this.selectB);
    }

    // Hover Tile
    if (this.drawCallback != null && this.hoverA != null && this.hoverA < this.aMax && this.hoverA >= this.aMin && this.hoverB >= this.bMin && this.hoverB < this.bMax) {
        var screenCoords = hexMath.toScreenCoords(this.origin, this.hoverA, this.hoverB, this.hexRadius);
        var tileContext = new HexTile(this, this.tileRadius * this.expandFactor, this.imageSize * this.expandFactor, hexMath.theta, screenCoords);
        this.drawCallback(tileContext, this.hoverA, this.hoverB);
    }
};




function HexTile(parentBoard, radius, imageSize, angle, location) {
    this.board = parentBoard;
    this.location = location;
    this.imageSize = imageSize;
    this.radius = radius;
    this.angle = angle;    
};

HexTile.prototype.drawOverlay = function (url, direction, height) {
    this.board.gameCore.spriteContext.draw(new Sprite(url), new vMath.vector2(this.location.x, this.location.y - height), this.radius*1.1, this.radius*1.1, this.angle + direction * Math.PI / 3, 1);
};

HexTile.prototype.drawFrame = function (url, direction, height) {
    this.board.gameCore.spriteContext.draw(new Sprite(url), new vMath.vector2(this.location.x, this.location.y - height), this.radius, this.radius, this.angle + direction * Math.PI / 3, 1);
};

HexTile.prototype.drawPortrait = function (url, direction, height) {
    this.board.gameCore.spriteContext.draw(new Sprite(url), new vMath.vector2(this.location.x, this.location.y - height), this.imageSize, this.imageSize, this.angle + direction * Math.PI / 3, 1);
};



function HexOverlay(url) {
    this.url = url;
    this.grid = [];
    for (var a = 0; a < 14; a++) {
        this.grid[a] = [];
        for (var b = 0; b < 11; b++) {
            this.grid[a][b] = [];
        }
    }
}