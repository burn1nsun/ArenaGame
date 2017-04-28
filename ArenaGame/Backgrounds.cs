﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArenaGame
{
    public class Backgrounds
    {
        public Texture2D texture;
        public Rectangle rectangle;


        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, rectangle, Color.White);
        }
    }

    public class Scrolling : Backgrounds
    {

        public Scrolling(Texture2D newTexture, Rectangle newRectangle)
        {
            texture = newTexture;
            rectangle = newRectangle;
        }

        public void Update(int xPos, int yPos, int amount)
        {
            rectangle.X = -xPos / amount;
            rectangle.Y = -yPos / amount;
        }
    }
}
