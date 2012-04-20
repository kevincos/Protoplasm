function TokenInstance(core, location, width, height, expandFactor, url) {
    this.gameCore = core;
    this.width = width;
    this.height = height;

    this.expandFactor = expandFactor;
    this.content = null;
    
    this.clickCallback = null;
    this.location = location;

    this.url = url;

    this.hover = false;
};

TokenInstance.prototype.draw = function () {
    gameCore.spriteContext.draw(new Sprite(this.url), new vMath.vector2(this.location.x, this.location.y), this.width, this.height, 0, 1);
};


function TrackInstance(core, location, width, height, length, url) {
    this.gameCore = core;
    this.trackWidth = width;
    this.trackHeight = height;
    this.url = url;

    this.tokenWidth = width;
    this.tokenHeight = width;

    this.length = 10;
    this.positions = [];
    for (var i = 0; i < length; i++)
    {
        this.positions[i] = {};
        this.positions[i].offSet = i*height/length - .5*height;
        this.positions[i].tokens = [];
    }


    this.clickCallback = null;
    this.tokenCallback = null;

    this.location = location;

    this.drawCardCallback = null;

    this.hoverSegment = null;
    this.hoverTokenIndex = null;
};

TrackInstance.prototype.addToken = function (content, index, url) {
    var newToken = new TokenInstance(this.gameCore, new vMath.vector2(this.location.x + this.positions[index].tokens.length * this.tokenWidth - .5 *this.trackWidth+.5*this.tokenWidth, this.location.y +.5*this.tokenHeight + this.positions[index].offSet), this.tokenWidth, this.tokenHeight, 1.2, url);
    newToken.content = content;
    this.positions[index].tokens[this.positions[index].tokens.length] = newToken;
}

TrackInstance.prototype.draw = function () {
    gameCore.spriteContext.draw(new Sprite(this.url), new vMath.vector2(this.location.x, this.location.y), this.trackWidth, this.trackHeight, 0, 1);
    for (var i = 0; i < this.length; i++) {
        for (var j = 0; j < this.positions[i].tokens.length; j++) {
            this.positions[i].tokens[j].draw();
        }
    }
};

TrackInstance.prototype.update = function () {
    var handMousePos = new vMath.vector2(vMath.mousePos.x - this.location.x, vMath.mousePos.y - this.location.y);
    if (handMousePos.x > -.5 * this.trackWidth && handMousePos.x < .5 * this.trackWidth && handMousePos.y > -.5 * this.trackHeight && handMousePos.y < .5 * this.trackHeight) {
        var hoverSegment = -1;
        for (var i = 0; i < this.length; i++) {
            if (this.positions[i].offSet < handMousePos.y) hoverSegment++;
        }
        var hoverTokenIndex = Math.round((handMousePos.x + .5*this.trackWidth - .5*this.tokenWidth) / this.tokenWidth);

        if (hoverTokenIndex != this.hoverTokenIndex || hoverSegment != this.hoverSegment) {
            this.hoverTokenIndex = hoverTokenIndex;
            this.hoverSegment = hoverSegment;
            if (this.hoverCallback != null)
                this.hoverCallback(this.hoverSegment, this.hoverTokenIndex);
            this.gameCore.refreshView = true;
        }
        if (vMath.mouseDown == true && this.gameCore.mouseDown == false) {
            if (this.clickCallback != null) {
                this.clickCallback(this.positions[this.hoverSegment].tokens[this.hoverTokenIndex], this.hoverSegment, this.hoverTokenIndex);
            }
        }
    }
}