function InfoWindow(core, location, width, height) {
    this.gameCore = core;
    this.width = width;
    this.height = height;
    this.location = location;
    this.borderWidth = 20;
    this.borderHeight = 20;
    this.textPieces = [];
    this.currentY = this.borderHeight;
    this.mouseTracking = false;
};

InfoWindow.prototype.draw = function () {
    if (this.mouseTracking == true) {
        this.location = new vMath.vector2(vMath.mousePos.x + 150, vMath.mousePos.y + 100);
    }

    this.gameCore.spriteContext.draw(new Sprite("/content/images/infowindow.png"), this.location, this.width, this.height, 0, 1);
    for (var i = 0; i < this.textPieces.length; i++) {
        this.gameCore.spriteContext.textWrap(this.textPieces[i].text, new vMath.vector2(this.location.x + this.textPieces[i].relX - this.width / 2 + this.borderWidth, this.location.y + this.textPieces[i].relY - this.height / 2 + this.borderHeight), this.width - this.textPieces[i].relX - 2 * this.borderWidth, 15);
    }
};

InfoWindow.prototype.clear = function () {
    this.textPieces = [];
    this.currentY = this.borderHeight;
}

InfoWindow.prototype.addGap = function (pixels) {
    this.currentY += pixels;
}

InfoWindow.prototype.addText = function (text, relX) {
    var nextIndex = this.textPieces.length;
    this.textPieces[nextIndex] = {};
    this.textPieces[nextIndex].text = text;
    this.textPieces[nextIndex].relX = relX;
    this.textPieces[nextIndex].relY = this.currentY;
    this.currentY += 15;
}