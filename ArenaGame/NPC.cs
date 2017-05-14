﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArenaGame
{
    class NPC
    {
        public Texture2D texture;
        public Vector2 position;
        public Rectangle npcBounds;

        public bool isVisible = true;
        public bool isMoving;

        private Random random;

        private TimeSpan MovementCooldown;// = TimeSpan.FromSeconds(random.Next(3, 8));
        private TimeSpan? LastMovement;
        private TimeSpan? NewMovement;


        Animation walkDown;
        Animation walkUp;
        Animation walkLeft;
        Animation walkRight;

        Animation standDown;
        Animation standUp;
        Animation standLeft;
        Animation standRight;

        Animation currentAnimation;

        private Vector2 velocity;


        private void initAnimations()
        {
            walkDown = new Animation();
            walkDown.AddFrame(new Rectangle(0, 0, 64, 64), TimeSpan.FromSeconds(.25));
            walkDown.AddFrame(new Rectangle(0, 0, 64, 64), TimeSpan.FromSeconds(.25));
            walkDown.AddFrame(new Rectangle(0, 0, 64, 64), TimeSpan.FromSeconds(.25));
            walkDown.AddFrame(new Rectangle(128, 0, 64, 64), TimeSpan.FromSeconds(.25));

            walkUp = new Animation();
            walkUp.AddFrame(new Rectangle(576, 0, 64, 64), TimeSpan.FromSeconds(.25));
            walkUp.AddFrame(new Rectangle(640, 0, 64, 64), TimeSpan.FromSeconds(.25));
            walkUp.AddFrame(new Rectangle(576, 0, 64, 64), TimeSpan.FromSeconds(.25));
            walkUp.AddFrame(new Rectangle(704, 0, 64, 64), TimeSpan.FromSeconds(.25));

            walkLeft = new Animation();
            walkLeft.AddFrame(new Rectangle(192, 0, 64, 64), TimeSpan.FromSeconds(.25));
            walkLeft.AddFrame(new Rectangle(256, 0, 64, 64), TimeSpan.FromSeconds(.25));
            walkLeft.AddFrame(new Rectangle(192, 0, 64, 64), TimeSpan.FromSeconds(.25));
            walkLeft.AddFrame(new Rectangle(320, 0, 64, 64), TimeSpan.FromSeconds(.25));

            walkRight = new Animation();
            walkRight.AddFrame(new Rectangle(384, 0, 64, 64), TimeSpan.FromSeconds(.25));
            walkRight.AddFrame(new Rectangle(448, 0, 64, 64), TimeSpan.FromSeconds(.25));
            walkRight.AddFrame(new Rectangle(384, 0, 64, 64), TimeSpan.FromSeconds(.25));
            walkRight.AddFrame(new Rectangle(512, 0, 64, 64), TimeSpan.FromSeconds(.25));

            // Standing animations only have a single frame of animation:
            standDown = new Animation();
            standDown.AddFrame(new Rectangle(0, 0, 64, 64), TimeSpan.FromSeconds(.25));

            standUp = new Animation();
            standUp.AddFrame(new Rectangle(576, 0, 64, 64), TimeSpan.FromSeconds(.25));

            standLeft = new Animation();
            standLeft.AddFrame(new Rectangle(192, 0, 64, 64), TimeSpan.FromSeconds(.25));

            standRight = new Animation();
            standRight.AddFrame(new Rectangle(384, 0, 64, 64), TimeSpan.FromSeconds(.25));
        }

        public NPC(Texture2D newTexture, Vector2 newPosition, Rectangle newNpcBounds)
        {
            random = new Random();
            velocity = new Vector2(random.Next(-2, 2), random.Next(-2, 2));
            texture = newTexture;
            position = newPosition;
            npcBounds = newNpcBounds;
            MovementCooldown = TimeSpan.FromSeconds(random.Next(3, 8));



            initAnimations();

            currentAnimation = standDown;
            
        }

        public void checkMovement()
        {
            if (isMoving)
            {
                bool isMovingHorizontally = Math.Abs(velocity.X) > Math.Abs(velocity.Y);
                if (isMovingHorizontally)
                {
                    if (velocity.X > 0)
                    {
                        currentAnimation = walkRight;
                    }
                    else
                    {
                        currentAnimation = walkLeft;
                    }
                }
                else
                {
                    if (velocity.Y > 0)
                    {
                        currentAnimation = walkDown;
                    }
                    else
                    {
                        currentAnimation = walkUp;
                    }
                }
            }
            else
            {

                if (currentAnimation == walkRight)
                {
                    currentAnimation = standRight;
                }
                else if (currentAnimation == walkLeft)
                {
                    currentAnimation = standLeft;
                }
                else if (currentAnimation == walkUp)
                {
                    currentAnimation = standUp;
                }
                else if (currentAnimation == walkDown)
                {
                    currentAnimation = standDown;
                }

                else if (currentAnimation == null)
                {
                    currentAnimation = standDown;
                }
            }
        }
        public void Update(GameTime gameTime)
        {

            if (LastMovement == null || gameTime.TotalGameTime - LastMovement >= MovementCooldown)
            {
                isMoving = !isMoving; 
                LastMovement = gameTime.TotalGameTime;
                velocity = new Vector2(random.Next(-2, 2), random.Next(-2, 2));
            }
            
          
            if (isMoving)
            {
                position += velocity;

                currentAnimation.Update(gameTime);

                if (position.Y <= npcBounds.Y || position.Y >= npcBounds.Y + npcBounds.Height || position.X <= npcBounds.X || position.X >= npcBounds.X + npcBounds.Width)
                {
                    velocity.Y = -velocity.Y;
                }

                if (position.X <= npcBounds.X || position.X >= npcBounds.X + npcBounds.Width)
                {
                    velocity.X = -velocity.X;
                }
            }

            checkMovement();
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, currentAnimation.CurrentRectangle, Color.White);
        }
    }
}
