function HandInstance2(core, srcHand) {
    this.type = "Hand";
    this.gameCore = core;

    this.name = srcHand.name;

    this.location = new vMath.vector2(srcHand.x, srcHand.y);
    this.cardWidth = srcHand.cardX;
    this.cardHeight = srcHand.cardY;
    this.expandFactor = 1.2;
    this.imageSize = .8 * this.cardWidth;
    this.frameUrl = srcHand.frameUrl;

    this.cards = srcHand.cards;
    
    this.handCount = 0;
    this.contents = [];

    this.selectIndex = srcHand.selectedIndex;
    this.hoverIndex = null;
};

HandInstance2.prototype.update = function () {

    var handMousePos = new vMath.vector2(vMath.mousePos.x - this.location.x, vMath.mousePos.y - this.location.y);
    if (handMousePos.x > -.5 * this.cardWidth && handMousePos.x < -.5 * this.cardWidth + this.cards.length * this.cardWidth && handMousePos.y > -.5 * this.cardHeight && handMousePos.y < .5 * this.cardHeight) {
        hoverChoiceIndex = Math.round(handMousePos.x / this.cardWidth);
        if (hoverChoiceIndex != this.hoverIndex) {
            this.hoverIndex = hoverChoiceIndex;
            this.gameCore.refreshView = true;
        }
        if (vMath.mouseDown == true && this.gameCore.mouseDown == false && this.cards[hoverChoiceIndex].selectable == true) {

            var update = {};
            update.selectIndex = hoverChoiceIndex;
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
    else {
        if (this.hoverIndex != null)
            this.gameCore.refreshView = true;
        this.hoverIndex = null;
    }
};

HandInstance2.prototype.draw = function () {
    for (var i = 0; i < this.cards.length; i++) {
        if (this.selectIndex != i && this.hoverIndex != i) {

            // Draw Card Fame
            // Draw Card Image
            // Draw Card Highlight
            gameCore.spriteContext.draw(new Sprite(this.frameUrl), new vMath.vector2(this.location.x + this.cardWidth * i, this.location.y), this.cardWidth, this.cardHeight, 0, 1);
            if (this.cards[i].url != null && this.cards[i].url != "")
                gameCore.spriteContext.draw(new Sprite(this.cards[i].url), new vMath.vector2(this.location.x + this.cardWidth * i, this.location.y), this.imageSize, this.imageSize, 0, 1);
            if (this.cards[i].highlightUrl != null && this.cards[i].highlightUrl != "")
                gameCore.spriteContext.draw(new Sprite(this.cards[i].highlightUrl), new vMath.vector2(this.location.x + this.cardWidth * i, this.location.y), this.cardWidth, this.cardHeight, 0, 1);

        }
    }

    // Hover Card
    if (this.hoverIndex != null && this.hoverIndex != -1) {
        if (this.cards[this.hoverIndex].hoverWindow != undefined) {
            gameCore.UpdateInfoWindow(this.cards[this.hoverIndex].hoverWindow, this.cards[this.hoverIndex].hoverText);
        }

        gameCore.spriteContext.draw(new Sprite(this.frameUrl), new vMath.vector2(this.location.x + this.cardWidth * this.hoverIndex, this.location.y), this.cardWidth * this.expandFactor, this.cardHeight * this.expandFactor, 0, 1);
        if (this.cards[this.hoverIndex].url != null && this.cards[this.hoverIndex].url != "")
            gameCore.spriteContext.draw(new Sprite(this.cards[this.hoverIndex].url), new vMath.vector2(this.location.x + this.cardWidth * this.hoverIndex, this.location.y), this.imageSize * this.expandFactor, this.imageSize * this.expandFactor, 0, 1);
        if (this.cards[this.hoverIndex].highlightUrl != null && this.cards[this.hoverIndex].highlightUrl != "")
            gameCore.spriteContext.draw(new Sprite(this.cards[this.hoverIndex].highlightUrl), new vMath.vector2(this.location.x + this.cardWidth * this.hoverIndex, this.location.y), this.cardWidth * this.expandFactor, this.cardHeight * this.expandFactor, 0, 1);
    }

    // Select Card
    if (this.selectIndex != -1 && this.selectIndex != null) {
        gameCore.spriteContext.draw(new Sprite(this.frameUrl), new vMath.vector2(this.location.x + this.cardWidth * this.selectIndex, this.location.y), this.cardWidth * this.expandFactor, this.cardHeight * this.expandFactor, 0, 1);
        if (this.cards[this.selectIndex].url != null && this.cards[this.selectIndex].url != "")
            gameCore.spriteContext.draw(new Sprite(this.cards[this.selectIndex].url), new vMath.vector2(this.location.x + this.cardWidth * this.selectIndex, this.location.y), this.imageSize * this.expandFactor, this.imageSize * this.expandFactor, 0, 1);
        if (this.cards[this.selectIndex].highlightUrl != null && this.cards[this.selectIndex].highlightUrl != "")
            gameCore.spriteContext.draw(new Sprite(this.cards[this.selectIndex].highlightUrl), new vMath.vector2(this.location.x + this.cardWidth * this.selectIndex, this.location.y), this.cardWidth * this.expandFactor, this.cardHeight * this.expandFactor, 0, 1);
    }

};