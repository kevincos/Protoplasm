import random

class GameUpdate:
    def __init__(self):
        self.playerId=0
        self.name = ""
        self.x =0
        self.y=0
        self.index=0
        self.direction =0

def basic_text_view(x, y, text):
    return {"type":"TextBox", "x":x, "y":y, "text":text, "font": "16px Verdana", "color":"Black"}

def text_view(x, y, text, font, color):
    return {"type":"TextBox", "x":x, "y":y, "text":text, "font": font, "color":color}

def basic_image_view(x,y,url):
    return {"type":"Image", "x":x, "y":y, "width":50, "height":50, "url":url}

def image_view(x,y,width,height, url):
    return {"type":"Image", "x":x, "y":y, "width":width, "height":height, "url":url}

class InfoWindow():
    def __init__(self,name, width, height, url, font, color):
        self.name = name
        self.width = width
        self.height = height
        self.font = font
        self.color = color
        self.type = "InfoWindow"
        self.location_type = "Dynamic"
        self.url = url
        self.x = 0
        self.y = 0

    def set_location(self,x,y):
        self.x = x
        self.y = y
        self.location_type = "Static"

    def view(self):
        view = {}
        view["name"] = self.name
        view["type"] = self.type
        view["xSize"] = self.width
        view["ySize"] = self.height
        view["font"] = self.font
        view["color"] = self.color
        view["locationType"] = self.location_type
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

    def view(self):
        view = {}
        view["url"] = self.url
        view["name"] = self.name
        view["type"] = self.type
        return view

class GamePiece(GameObject):
    def __init__(self, name, url):
        GameObject.__init__(self)
        self.url = url
        self.base_urls = []
        self.name = name
        self.type = "Piece"

    def __str__(self):
        return self.name + "(" +self.type + ") url:" + self.url

    def hidden_view(self):
        view = {}
        view["type"] = self.type
        view["name"] = "Unknown"
        view["url"] = ""
        return view

    def view(self):
        view = {}
        view["type"] = self.type
        view["name"] = self.name
        view["url"] = self.url
        if len(self.base_urls) > 0:
            view["baseUrls"] = self.base_urls
        try:
            view["selectable"] = self.selectable
        except AttributeError:
            pass

        try:
            view["highlightUrl"] = self.highlight_url
        except AttributeError:
            pass

        try:
            view["hoverText"] = self.hover_text
        except AttributeError:
            pass

        try:
            view["direction"] = self.direction
        except AttributeError:
            pass

        try:
            view["hoverWindow"] = self.hover_window
        except AttributeError:
            pass

        try:
            if len(self.tokens) > 0:
                view["tokens"] = []
                for token in self.tokens:
                    view["tokens"].append(token.view())
        except AttributeError:
            pass

        return view;

class Tile(GameObject):
    def __init__(self):
        GameObject.__init__(self)
        self.pieces = []
        self.url = ""
        self.highlight_url = ""
        self.selectable = False

    def __str__(self):
        if len(self.pieces) == 0:
            return self.name + "(" + self.type + ") EMPTY  SELECT: " + str(self.selectable)
        else:
            return self.name + "(" + self.type + ") <" + self.pieces[0].name + "> SELECT: " + str(self.selectable)

    def top_piece(self):
        if len(self.pieces) == 0:
            return None
        return self.pieces[0]

    def add_piece(self,piece):
        self.pieces.append(piece)

    def is_empty(self):
        return len(self.pieces) == 0

    def pieces(self):
        return self.pieces

    def clear(self):
        self.pieces = []

    def view(self):
        view = {}
        view["type"] = self.type
        view["url"] = self.url
        view["highlightUrl"] = self.highlight_url
        view["selectable"]=self.selectable
        view["name"]=self.name
        view["pieces"] = []
        for i in range(0,len(self.pieces)):
            view["pieces"].append(self.pieces[i].view())


        try:
            view["hoverText"] = self.hover_text
        except AttributeError:
            pass

        try:
            view["direction"] = self.direction
        except AttributeError:
            pass

        try:
            view["hoverWindow"] = self.hover_window
        except AttributeError:
            pass

        return view


class HexBoard(GameObject):
    def __init__(self,name,side_length, radius):
        GameObject.__init__(self)
        self.name = name
        self.x = 400
        self.y = 300
        self.side_length = side_length;
        self.radius = radius
        self.grid = []
        self.type = "HexBoard";

        self.boundary = 2*side_length-1;
        self.max_coordinate = 3*side_length-3;

        for a in range(0,self.boundary):
            self.grid.append([])
            for b in range(0,self.boundary):
                self.grid[a].append(None)

        for a in range(0, self.boundary):
            for b in range(0, self.boundary):
                if self.is_on_board(a,b):
                    self.grid[a][b] = Tile()

    def c_coordinate(self,a,b):
        return self.max_coordinate - a- b

    def is_on_board(self,a,b):
        return a < self.boundary and b < self.boundary and self.c_coordinate(a,b) < self.boundary

    def get_abc_coords(self,a,b):
        return (a,b,self.max_coordinate - a- b)

    def top_piece(self, a,b):
        if len(self.grid[a][b].pieces) == 0:
            return None
        return self.grid[a][b].pieces[0]

    def is_empty(self,a,b):
        return len(self.grid[a][b].pieces) == 0

    def pieces(self,a,b):
        return self.grid[a][b].pieces;

    def clear_pieces(self,a,b):
        self.grid[a][b].clear()

    def tile_at(self, a,b):
        return self.grid[a][b]

    def add_piece(self,a,b,piece):
        self.grid[a][b].pieces.append(piece)

    def clear_selection(self):
        for a in range(0, self.boundary):
            for b in range(0, self.boundary):
                if self.is_on_board(a,b):
                    self.grid[a][b].selectable = False

    def view(self, x, y, radius):
        view = {}
        view["name"]=self.name
        view["type"]=self.type
        view["sideLength"]=self.side_length
        view["x"]=x
        view["y"]=y
        view["radius"]=radius
        view["grid"] = []
        for a in range(self.boundary):
            view["grid"].append([])
            for b in range(self.boundary):
                if self.grid[a][b] is None:
                    view["grid"][a].append({})
                else:
                    view["grid"][a].append(self.grid[a][b].view())
        return view;


class SquareBoard(GameObject):
    def __init__(self, name, width, height):
        GameObject.__init__(self)
        self.name = name;
        self.height = height;
        self.width = width;
        self.grid = [];
        self.type = "SquareBoard"
        self.x = 400
        self.y = 300
        self.xSize = 400
        self.ySize = 400
        for x in range(width):
            self.grid.append([]);
            for y in range(height):
                self.grid[x].append(Tile())

    def top_piece(self, x,y):
        if len(self.grid[x][y].pieces) == 0:
            return None
        return self.grid[x][y].pieces[0]

    def add_piece(self,x,y,piece):
        self.grid[x][y].pieces.append(piece)

    def tile_at(self, x,y):
        return self.grid[x][y]

    def clear_pieces(self,x,y):
        self.grid[x][y].clear()

    def is_empty(self,x,y):
        return len(self.grid[x][y].pieces) == 0

    def clear_selection(self):
        for x in range(self.width):
            for y in range(self.height):
                self.grid[x][y].selectable = False

    def view(self, x, y, tile_width, tile_height):
        view = {}
        view["name"]=self.name
        view["type"]=self.type
        view["height"]=self.height
        view["width"]=self.width
        view["x"]=x
        view["y"]=y
        view["xTileSize"]=tile_width
        view["yTileSize"]=tile_height
        view["grid"] = []
        for x in range(self.width):
            view["grid"].append([])
            for y in range(self.height):
                view["grid"][x].append(self.grid[x][y].view())
        return view;

class Deck:
    def __init__(self,name):
        self.type = "Deck"
        self.name = name
        self.cards = []

    def shuffle(self):
        shuffled_cards = []
        while len(self.cards) > 0:
            shuffled_cards.append(self.cards.pop(random.randint(0,len(self.cards)-1)))
        self.cards = shuffled_cards

    def draw(self):
        if len(self.cards) == 0:
            return None
        return self.cards.pop()

    def draw_with_discard(self, discard):
        if len(self.cards) == 0:
            self.cards = discard.cards
            if len(self.cards) == 0:
                return None
            discard.cards = []
            self.shuffle()
        return self.cards.pop()

    def add(self, piece):
        self.cards.append(piece)

class Hand:
    def __init__(self, name, frame_url):
        self.type = "Hand"
        self.name = name
        self.cards = []
        self.selected_index = -1
        self.frame_url = frame_url

    def add(self, piece):
        self.cards.append(piece)

    def count(self):
        return len(self.cards)

    def card(self,index):
        if index >= self.count():
            return None
        else:
            return self.cards[index]

    def deselect(self):
        self.selected_index = -1

    def clear_selection(self):
        for card in self.cards:
            card.selectable = False
            card.highlight_url = ""

    def selected_card(self):
        if self.selected_index == -1:
            return None
        return self.cards[self.selected_index]

    def select(self, index):
        self.selected_index = index

    def take(self, index):
        return self.cards.pop(index)

    def view(self, x, y, card_x, card_y):
        view = {}
        view["name"]=self.name
        view["type"]=self.type
        view["cardX"]=card_x
        view["cardY"]=card_y
        view["x"]=x
        view["y"]=y
        view["frameUrl"] = self.frame_url
        view["selectedIndex"]=self.selected_index
        view["cards"] = []
        for card in self.cards:
            view["cards"].append(card.view())
        return view

    def hidden_view(self, x, y, cardX, cardY):
        view = {}
        view["name"]=self.name
        view["type"]=self.type
        view["cardX"]=cardX
        view["cardY"]=cardY
        view["x"]=x
        view["y"]=y
        view["frameUrl"] = self.frame_url
        view["selectedIndex"]=self.selected_index
        view["cards"] = []
        for card in self.cards:
            view["cards"].append(card.hidden_view())
        return view


class PlayerContext:
    def __init__(self, player_id, name):
        self.player_id= player_id
        self.name=name
        self.stats_base = {}
        self.result = "None"

    def stat_log(self,key,value):
        self.stats_base[key]=value

class GameState:
    def __init__(self, seats):
        self.player_contexts = [];
        for i in range(len(seats)):
            self.player_contexts.append(PlayerContext(seats[i].PlayerId, seats[i].Player.Name));
        self.active_player_index = 0
        self.game_over = False
        self.logs = []
        self.table_id = seats[0].TableId
        self.stats_base = {}
        seats[self.active_player_index].Waiting = True

    def is_waiting_for_input(player_index):
        if player_index == self.active_player_index:
            return True
        return False

    def set_waiting_status(self, seats):
        for i in range(len(seats)):
            seats[i].Waiting = (i == self.active_player_index)
            seats[i].Result = self.player_contexts[i].result

    def stats(self):
        return generate_stats()

    def stat_log(self,key, value):
        self.stats_base[key] = value

    def stat_log_item(self,item,data):
        try:
            self.stats_base[item].append(data)
        except:
            self.stats_base[item]=[data]

    def generate_stats(self):
        self.stats_base["player"] = []
        for (i,context) in enumerate(self.player_contexts):
            context.stat_log("index",i)
            context.stat_log("result",context.result)
            self.stats_base["player"].append(context.stats_base)
        return self.stats_base


    def log(self, message):
        self.logs.append(message)

    def get_player_context(self, player_index):
        return self.player_contexts[player_index]

    def advance_active_player(self):
        self.active_player_index+=1
        self.active_player_index%=len(self.player_contexts)