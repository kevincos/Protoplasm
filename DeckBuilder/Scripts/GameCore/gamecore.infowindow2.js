function InfoWindow2(core, srcWindow) {
    this.gameCore = core;
    this.width = srcWindow.xSize;
    this.height = srcWindow.ySize;
    this.x = srcWindow.x;
    this.y = srcWindow.y;
    this.location = new vMath.vector2(this.x, this.y);
    this.borderWidth = 20;
    this.borderHeight = 25;
    this.type = "InfoWindow";
    this.font = srcWindow.font;
    this.color = srcWindow.color;
    this.locationType = srcWindow.locationType;
    this.url = srcWindow.url;
    this.name = srcWindow.name;

    this.textPieces = [];
    this.currentY = this.borderHeight;
    this.mouseTracking = false;
};



InfoWindow2.prototype.draw = function () {
    if (this.locationType == "Dynamic") {
        this.location = new vMath.vector2(vMath.mousePos.x + 150, vMath.mousePos.y + 100);
    }

    this.gameCore.spriteContext.draw(new Sprite(this.url), new vMath.vector2(this.x, this.y), this.width, this.height, 0, 1);
    this.gameCore.spriteContext.setTextType(this.font, this.color);
    for (var i = 0; i < this.textPieces.length; i++) {
        this.gameCore.spriteContext.textWrap(this.textPieces[i].text, new vMath.vector2(this.location.x + this.textPieces[i].relX - this.width / 2 + this.borderWidth, this.location.y + this.textPieces[i].relY - this.height / 2 + this.borderHeight), this.width - this.textPieces[i].relX - 2 * this.borderWidth, 15);
    }
};

InfoWindow2.prototype.clear = function () {
    this.textPieces = [];
    this.currentY = this.borderHeight;
}

InfoWindow2.prototype.addGap = function (pixels) {
    this.currentY += pixels;
}

InfoWindow2.prototype.addText = function (text, relX) {
    var nextIndex = this.textPieces.length;
    this.textPieces[nextIndex] = {};
    this.textPieces[nextIndex].text = text;
    this.textPieces[nextIndex].relX = relX;
    this.textPieces[nextIndex].relY = this.currentY;
    this.currentY += 15;
}