var hexMath = {
    hexOrigin: new vMath.vector2(0,0),
    theta: .0,    
    root3: 1.73205,

    hexTile: function (a, b) {
        this.a = a;
        this.b = b;
    },

    toScreenCoords: function (origin, a, b, r) {
        //return new vector2(origin.x + a * r * root3, origin.y + a * r + 2 * b * r);

        return new vector2(origin.x + (a - hexMath.hexOrigin.x) * r * (hexMath.root3 * Math.cos(hexMath.theta) - Math.sin(hexMath.theta)) - 2 * (b - hexMath.hexOrigin.y) * r * Math.sin(hexMath.theta),
            origin.y + (a - hexMath.hexOrigin.x) * r * (hexMath.root3 * Math.sin(hexMath.theta) + Math.cos(hexMath.theta)) + 2 * (b - hexMath.hexOrigin.y) * r * Math.cos(hexMath.theta));
    },

    toHexCoords: function (origin, x, y, r) {
        //return new vector2((x - origin.x) / r / root3, (y - origin.y) / 2 / r - (x - origin.x) / 2 / r / root3);
        var rx = x - origin.x;
        var ry = y - origin.y;
        return new vector2(hexMath.hexOrigin.x + (rx * Math.cos(hexMath.theta) + ry * Math.sin(hexMath.theta)) / hexMath.root3 / r,
            hexMath.hexOrigin.y + 1 / r / 2 * (-rx * Math.sin(hexMath.theta) - rx * Math.cos(hexMath.theta) / hexMath.root3 + ry * Math.cos(hexMath.theta) - ry * Math.sin(hexMath.theta) / hexMath.root3));

    }
};