import random

class GameUpdate:
    def __init__(self):
        self.playerId=0
        self.name = ""
        self.x =0
        self.y=0
        self.index=0
        self.direction =0

def BasicTextView(x, y, text):
    return {"type":"TextBox", "x":x, "y":y, "text":text, "font": "16px Verdana", "color":"Black"}

def TextView(x, y, text, font, color):
    return {"type":"TextBox", "x":x, "y":y, "text":text, "font": font, "color":color}

def BasicImageView(x,y,url):
    return {"type":"Image", "x":x, "y":y, "width":50, "height":50, "url":url}

def ImageView(x,y,width,height, url):
    return {"type":"Image", "x":x, "y":y, "width":width, "height":height, "url":url}

class InfoWindow():
    def __init__(self,name, xSize, ySize, url, font, color):
        self.name = name
        self.xSize = xSize
        self.ySize = ySize
        self.font = font
        self.color = color
        self.type = "InfoWindow"
        self.locationType = "Dynamic"
        self.url = url
        self.x = 0
        self.y = 0

    def SetLocation(self,x,y):
        self.x = x
        self.y = y
        self.locationType = "Static"

    def View(self):
        view = {}
        view["name"] = self.name
        view["type"] = self.type
        view["xSize"] = self.xSize
        view["ySize"] = self.ySize
        view["font"] = self.font
        view["color"] = self.color
        view["locationType"] = self.locationType
        view["url"]=self.url
        view["x"] = self.x
        view["y"] = self.y
        return view


class GameObject:
    def __init__(self):
        self.name = ""
        self.type = ""

    def __str__(self):
        return self.name + "(" + self.type +")"

class Token(GameObject):
    def __init__(self, name, url):
        self.url = url
        self.name = name
        self.type = "Token"

    def View(self):
        view = {}
        view["url"] = self.url
        view["name"] = self.name
        view["type"] = self.type
        return view

class GamePiece(GameObject):
    def __init__(self, name, url):
        GameObject.__init__(self)
        self.url = url
        self.baseUrls = []
        self.name = name
        self.type = "Piece"

    def __str__(self):
        return self.name + "(" +self.type + ") url:" + self.url

    def HiddenView(self):
        view = {}
        view["type"] = self.type
        view["name"] = "Unknown"
        view["url"] = ""
        return view

    def View(self):
        view = {}
        view["type"] = self.type
        view["name"] = self.name
        view["url"] = self.url
        if len(self.baseUrls) > 0:
            view["baseUrls"] = self.baseUrls
        try:
            view["selectable"] = self.selectable
        except AttributeError:
            pass

        try:
            view["highlightUrl"] = self.highlightUrl
        except AttributeError:
            pass

        try:
            view["hoverText"] = self.hoverText
        except AttributeError:
            pass

        try:
            view["direction"] = self.direction
        except AttributeError:
            pass

        try:
            view["hoverWindow"] = self.hoverWindow
        except AttributeError:
            pass

        try:
            if len(self.tokens) > 0:
                view["tokens"] = []
                for token in self.tokens:
                    view["tokens"].append(token.View())
        except AttributeError:
            pass

        return view;

class SquareTile(GameObject):
    def __init__(self):
        GameObject.__init__(self)
        self.pieces = []
        self.url = ""
        self.highlightUrl = ""
        self.selectable = False

    def __str__(self):
        if len(self.pieces) == 0:
            return self.name + "(" + self.type + ") EMPTY  SELECT: " + str(self.selectable)
        else:
            return self.name + "(" + self.type + ") <" + self.pieces[0].name + "> SELECT: " + str(self.selectable)

    def TopPiece(self):
        if len(self.pieces) == 0:
            return None
        return self.pieces[0]

    def View(self):
        view = {}
        view["type"] = self.type
        view["url"] = self.url
        view["highlightUrl"] = self.highlightUrl
        view["selectable"]=self.selectable
        view["name"]=self.name
        view["pieces"] = []
        for i in range(0,len(self.pieces)):
            view["pieces"].append(self.pieces[i].View())

        return view;

class Tile(GameObject):
    def __init__(self):
        GameObject.__init__(self)
        self.pieces = []
        self.url = ""
        self.highlightUrl = ""
        self.selectable = False

    def __str__(self):
        if len(self.pieces) == 0:
            return self.name + "(" + self.type + ") EMPTY  SELECT: " + str(self.selectable)
        else:
            return self.name + "(" + self.type + ") <" + self.pieces[0].name + "> SELECT: " + str(self.selectable)

    def TopPiece(self):
        if len(self.pieces) == 0:
            return None
        return self.pieces[0]

    def View(self):
        view = {}
        view["type"] = self.type
        view["url"] = self.url
        view["highlightUrl"] = self.highlightUrl
        view["selectable"]=self.selectable
        view["name"]=self.name
        view["pieces"] = []
        for i in range(0,len(self.pieces)):
            view["pieces"].append(self.pieces[i].View())
        return view


class HexBoard(GameObject):
    def __init__(self,name,sideLength, radius):
        GameObject.__init__(self)
        self.name = name
        self.x = 400
        self.y = 300
        self.sideLength = sideLength;
        self.radius = radius
        self.imageSize = .8*radius
        self.grid = []
        self.type = "HexBoard";

        self.aMin = sideLength - 1;
        self.bMin = sideLength - 1;
        self.cMin = sideLength - 1;
        self.aMax = 3*sideLength - 2;
        self.bMax = 3 * sideLength - 2;
        self.cMax = 3 * sideLength - 2;
        self.cBoundary = 6*(sideLength-1);
        for a in range(0,self.aMax):
            self.grid.append([])
            for b in range(0,self.bMax):
                self.grid[a].append(None)

        for a in range(self.aMin, self.aMax):
            for b in range(self.bMin, self.bMax):
                c = self.cBoundary - a - b;
                if c >= self.cMin and c < self.cMax:
                    self.grid[a][b] = Tile()

    def TopPiece(self, a,b):
        if len(self.grid[a][b].pieces) == 0:
            return None
        return self.grid[a][b].pieces[0]

    def Tile(self, a,b):
        return self.grid[a][b]

    def ClearSelection(self):
        for a in range(self.aMin, self.aMax):
            for b in range(self.bMin, self.bMax):
                c = self.cBoundary - a - b;
                if c >= self.cMin and c < self.cMax:
                    self.grid[a][b].selectable = False

    def View(self, x, y, radius):
        view = {}
        view["name"]=self.name
        view["type"]=self.type
        view["sideLength"]=self.sideLength
        view["x"]=x
        view["y"]=y
        view["radius"]=radius
        view["grid"] = []
        for a in range(self.aMax):
            view["grid"].append([])
            for b in range(self.bMax):
                if self.grid[a][b] is None:
                    view["grid"][a].append({})
                else:
                    view["grid"][a].append(self.grid[a][b].View())
        return view;


class SquareBoard(GameObject):
    def __init__(self, name, length, width):
        GameObject.__init__(self)
        self.name = name;
        self.length = length;
        self.width = width;
        self.grid = [];
        self.type = "SquareBoard"
        self.x = 400
        self.y = 300
        self.xSize = 400
        self.ySize = 400
        for x in range(length):
            self.grid.append([]);
            for y in range(width):
                self.grid[x].append(SquareTile())

    def TopPiece(self, x,y):
        if len(self.grid[x][y].pieces) == 0:
            return None
        return self.grid[x][y].pieces[0]

    def Tile(self, x,y):
        return self.grid[x][y]

    def ClearSelection(self):
        for x in range(self.length):
            for y in range(self.width):
                self.grid[x][y].selectable = False

    def View(self, x, y, xTileSize, yTileSize):
        view = {}
        view["name"]=self.name
        view["type"]=self.type
        view["length"]=self.length
        view["width"]=self.width
        view["x"]=x
        view["y"]=y
        view["xTileSize"]=xTileSize
        view["yTileSize"]=yTileSize
        view["grid"] = []
        for x in range(self.length):
            view["grid"].append([])
            for y in range(self.width):
                view["grid"][x].append(self.grid[x][y].View())
        return view;

class Deck:
    def __init__(self,name):
        self.type = "Deck"
        self.name = name
        self.cards = []

    def Shuffle(self):
        shuffledCards = []
        while len(self.cards) > 0:
            shuffledCards.append(self.cards.pop(random.randint(0,len(self.cards)-1)))
        self.cards = shuffledCards

    def Draw(self):
        if len(self.cards) == 0:
            return None
        return self.cards.pop()

    def DrawWithDiscard(self, discard):
        if len(self.cards) == 0:
            self.cards = discard.cards
            if len(self.cards) == 0:
                return None
            discard.cards = []
            self.Shuffle()
        return self.cards.pop()

    def Add(self, piece):
        self.cards.append(piece)

class Pile:
    def __init__(self,name):
        self.type = "Pile"
        self.name = name
        self.cards = []

    def Add(self,piece):
        self.cards.append(piece)

class Hand:
    def __init__(self, name, frameUrl):
        self.type = "Hand"
        self.name = name
        self.cards = []
        self.selectedIndex = -1
        self.frameUrl = frameUrl

    def Add(self, piece):
        self.cards.append(piece)

    def Deselect(self):
        self.selectedIndex = -1

    def ClearSelection(self):
        for card in self.cards:
            card.selectable = False
            card.highlightUrl = ""

    def SelectedCard(self):
        if self.selectedIndex == -1:
            return None
        return self.cards[self.selectedIndex]

    def Select(self, index):
        self.selectedIndex = index

    def Take(self, index):
        return self.cards.pop(index)

    def View(self, x, y, cardX, cardY):
        view = {}
        view["name"]=self.name
        view["type"]=self.type
        view["cardX"]=cardX
        view["cardY"]=cardY
        view["x"]=x
        view["y"]=y
        view["frameUrl"] = self.frameUrl
        view["selectedIndex"]=self.selectedIndex
        view["cards"] = []
        for card in self.cards:
            view["cards"].append(card.View())
        return view

    def HiddenView(self, x, y, cardX, cardY):
        view = {}
        view["name"]=self.name
        view["type"]=self.type
        view["cardX"]=cardX
        view["cardY"]=cardY
        view["x"]=x
        view["y"]=y
        view["frameUrl"] = self.frameUrl
        view["selectedIndex"]=self.selectedIndex
        view["cards"] = []
        for card in self.cards:
            view["cards"].append(card.HiddenView())
        return view

class GameView:
    def __init__(self):
        self.tableId = 0
        self.logs = []
        self.drawList = []



class PlayerContext:
    def __init__(self, playerId, name):
        self.playerId= playerId
        self.name=name

class GameState:
    def __init__(self, seats):
        self.playerContexts = [];
        for i in range(len(seats)):
            self.playerContexts.append(PlayerContext(seats[i].PlayerId, seats[i].Player.Name));
        self.activePlayerIndex = 0
        self.activePlayerId = self.playerContexts[0].playerId
        self.gameOver = False
        self.logs = []
        self.tableId = seats[0].TableId


    def GetPlayerContext(self, playerId):
        return filter(lambda pc: pc.playerId == playerId,self.playerContexts)[0]

    def AdvanceActivePlayer(self):
        self.activePlayerIndex+=1
        self.activePlayerIndex%=len(self.playerContexts)
        self.activePlayerId = self.playerContexts[self.activePlayerIndex].playerId