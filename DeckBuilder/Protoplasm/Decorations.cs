using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeckBuilder.Protoplasm
{
    public class TextBox : GameObject
    {
        public int x { get; set; }
        public int y { get; set; }
        public string text { get; set; }
        public string font { get; set; }
        public string color { get; set; }

        public TextBox()
        {
        }

        public TextBox(int x, int y, string text)
            :this(x,y,text, "16px Verdana", "Black") {}

        public TextBox(int x, int y, string text, string font)
            : this(x, y, text, font, "Black") {}

        public TextBox(int x, int y, string text, string font, string color)
        {
            this.type = "TextBox";
            this.x = x;
            this.y = y;
            this.text = text;
            this.font = font;
            this.color = color;
        }
    }

    public class Image : GameObject
    {
        public string url { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int width { get; set; }
        public int height { get; set; }

        public Image()
        {
        }

        public Image(int x, int y, string url) : this(x,y,50,50,url) {}

        public Image(int x, int y, int width, int height, string url)
        {
            this.type = "Image";
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.url = url;
        }
    }
}