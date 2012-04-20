
function IconInstance(core, width, height) {
    this.gameCore = core;
    this.iconWidth = width;
    this.iconHeight = height;
};

IconInstance.prototype.draw = function (location, imageUrl) {
    this.gameCore.spriteContext.draw(new Sprite(imageUrl), location, this.iconWidth, this.iconHeight, 0, 1);
};



function CardInstance(core, location, width, height, imageSize) {
    this.location = location;
    this.gameCore = core;
    this.cardWidth = width;
    this.cardHeight = height;
    this.portraitSize = imageSize;
    this.borderWidth = 10;
    this.borderHeight = 20;   
};

CardInstance.prototype.drawFrame = function (url, offSet) {
    if (offSet == undefined) offSet = 0;
    gameCore.spriteContext.draw(new Sprite(url), new vMath.vector2(this.location.x, this.location.y - offSet), this.cardWidth, this.cardHeight, 0, 1);
};

CardInstance.prototype.drawPortrait = function (url, offSet) {
    if (offSet == undefined) offSet = 0;
    gameCore.spriteContext.draw(new Sprite(url), new vMath.vector2(this.location.x, this.location.y - offSet), this.portraitSize, this.portraitSize, 0, 1);
};

CardInstance.prototype.drawText = function (text, relX, relY, offSet) {
    if (offSet == undefined) offSet = 0;
    this.gameCore.spriteContext.textWrap(""+text, new vMath.vector2(this.location.x + relX - this.cardWidth / 2 + this.borderWidth, this.location.y + relY - this.cardHeight / 2 + this.borderHeight - offSet), this.width - relX - 2 * this.borderWidth, 15);
};

function HandInstance(core, location, width, height, imageSize, expand) {
    this.gameCore = core;
    this.cardWidth = width;
    this.cardHeight = height;
    this.portraitSize = imageSize;
    this.expandFactor = expand;

    this.handCount = 0;
    this.contents = [];
    
    this.selectCallback = null;
    this.deselectCallback = null;
    this.clickCallback = null;
    this.hoverCallback = null;
    this.location = location;

    this.selectIndex = null;
    this.hoverIndex = null;

    this.drawCardCallback = null;
};

HandInstance.prototype.add = function (newItem, newUrl) {

    this.contents[this.handCount] = newItem;
    this.handCount++;
};

HandInstance.prototype.select = function (index) {
    this.selectIndex = index;
    this.gameCore.refreshView = true;
}

HandInstance.prototype.deselect = function () {
    this.selectIndex = null;
    this.gameCore.refreshView = true;
}

HandInstance.prototype.update = function () {

    var handMousePos = new vMath.vector2(vMath.mousePos.x - this.location.x, vMath.mousePos.y - this.location.y);
    if (handMousePos.x > -.5 * this.cardWidth && handMousePos.x < -.5 * this.cardWidth + this.handCount * this.cardWidth && handMousePos.y > -.5 * this.cardHeight && handMousePos.y < .5 * this.cardHeight) {
        hoverChoiceIndex = Math.round(handMousePos.x / this.cardWidth);
        if (hoverChoiceIndex != this.hoverIndex) {
            this.hoverIndex = hoverChoiceIndex;
            if (this.hoverCallback != null)
                this.hoverCallback(this.hoverIndex);
            this.gameCore.refreshView = true;
        }
        if (vMath.mouseDown == true && this.gameCore.mouseDown == false) {
            if (this.clickCallback != null) {
                this.clickCallback(this.contents[this.hoverIndex], this.hoverIndex);
            }
            else if (hoverChoiceIndex != this.selectIndex) {
                if (this.selectIndex != null && this.deselectCallback != null) {
                    this.deselectCallback(this.contents[this.selectIndex], this.selectIndex);
                }

                this.selectIndex = hoverChoiceIndex;
                this.gameCore.refreshView = true;
                if (this.selectCallback != null) {
                    this.selectCallback(this.contents[this.selectIndex], this.hoverIndex);
                }
            }
            else {
                this.selectIndex = null;
                this.gameCore.refreshView = true;
                if (this.deselectCallback != null) {
                    this.deselectCallback(this.contents[this.selectIndex], this.hoverIndex);
                }
            }
        }
    }
    else {
        if (this.hoverIndex != null)
            this.gameCore.refreshView = true;
        this.hoverIndex = null;
    }
};

HandInstance.prototype.draw = function () {
    for (var i = 0; i < this.handCount; i++) {
        var cardInstance = new CardInstance(this.gameCore, new vMath.vector2(this.location.x + this.cardWidth * i, this.location.y), this.cardWidth, this.cardHeight, this.portraitSize);
        if (this.selectIndex != i && this.hoverIndex != i) {            
            if(this.drawCallback != null)
                this.drawCallback(cardInstance, i);            
        }
    }

    // Hover Card
    if (this.drawCallback != null && this.hoverIndex != null) {
        var cardInstance = new CardInstance(this.gameCore, new vMath.vector2(this.location.x + this.cardWidth * this.hoverIndex, this.location.y), this.cardWidth * this.expandFactor, this.cardHeight * this.expandFactor, this.portraitSize * this.expandFactor);
        this.drawCallback(cardInstance, this.hoverIndex);
    }

    // Select Card
    if (this.drawCallback != null && this.selectIndex != null) {
        var cardInstance = new CardInstance(this.gameCore, new vMath.vector2(this.location.x + this.cardWidth * this.selectIndex, this.location.y), this.cardWidth * this.expandFactor, this.cardHeight * this.expandFactor, this.portraitSize * this.expandFactor);
        this.drawCallback(cardInstance, this.selectIndex);
    }

};




function StackInstance(core, location, width, height, imageSize, expand) {
    this.gameCore = core;
    this.cardWidth = width;
    this.cardHeight = height;
    this.portraitSize = imageSize;
    this.expandFactor = expand;

    this.stackCount = 0;
    this.contents = [];

    this.clickCallback = null;
    this.location = location;

    this.hover = false;

    this.drawCardCallback = null;
    this.hoverCallback = null;

    this.style = "TopOnly";
};

StackInstance.prototype.add = function (newItem) {

    this.contents[this.stackCount] = newItem;
    this.stackCount++;
};

StackInstance.prototype.draw = function () {
    if (this.style == "TopOnly") {
        cardInstance = new CardInstance(this.gameCore, new vMath.vector2(this.location.x, this.location.y), this.cardWidth, this.cardHeight, this.portraitSize);
        if (this.hover == true) {
            cardInstance = new CardInstance(this.gameCore, new vMath.vector2(this.location.x, this.location.y), this.cardWidth * this.expandFactor, this.cardHeight * this.expandFactor, this.portraitSize * this.expandFactor);
        }
        if (this.drawCallback != null)
            this.drawCallback(cardInstance, this.stackCount-1);
    }
    else {
        for (var i = 0; i < this.stackCount; i++) {
            var cardInstance = new CardInstance(this.gameCore, new vMath.vector2(this.location.x + .05 * this.cardWidth * i, this.location.y + .1 * this.cardHeight * i), this.cardWidth, this.cardHeight, this.portraitSize);
            if (i == this.stackCount - 1 && this.hover == true) {
                cardInstance = new CardInstance(this.gameCore, new vMath.vector2(this.location.x + .05 * this.cardWidth * i, this.location.y + .1 * this.cardHeight * i), this.cardWidth * this.expandFactor, this.cardHeight * this.expandFactor, this.portraitSize * this.expandFactor);
            }
            if (this.drawCallback != null)
                this.drawCallback(cardInstance, i);
        }
    }
};

StackInstance.prototype.update = function () {
    var hover = false;
    var handMousePos = new vMath.vector2(vMath.mousePos.x - this.location.x, vMath.mousePos.y - this.location.y);
    if (handMousePos.x > -.5 * this.cardWidth && handMousePos.x < .5 * this.cardWidth && handMousePos.y > -.5 * this.cardHeight && handMousePos.y < .5 * this.cardHeight) {
        hover = true;

        if (vMath.mouseDown == true && this.gameCore.mouseDown == false) {
            if (this.clickCallback != null) {
                this.clickCallback();
            }
        }
    }
    if (hover != this.hover) {
        if (this.hoverCallback != null)
            this.hoverCallback(hover);
        this.hover = hover;
        this.gameCore.refreshView = true;
    }
};