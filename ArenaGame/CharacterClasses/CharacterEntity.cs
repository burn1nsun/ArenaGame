﻿using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace ArenaGame
{
    public class CharacterEntity
    {
        public List<CharacterEntityShootableProjectile> Projectiles = new List<CharacterEntityShootableProjectile>();
        List<CharacterEntityShootableProjectile> projectilesToRemove = new List<CharacterEntityShootableProjectile>();

        static TimeSpan shootingCooldown = TimeSpan.FromMilliseconds(150);
        private TimeSpan? lastBulletShot;

        public static Texture2D characterSheetTexture;
        static private Texture2D characterBorder;

        private const float desiredSpeed = 350;

        Animation walkDown;
        Animation walkUp;
        Animation walkLeft;
        Animation walkRight;

        Animation standDown;
        Animation standUp;
        Animation standLeft;
        Animation standRight;

        Animation currentAnimation;

        MouseState oldState;
        KeyboardState previousState;

        public Stats charStats;

        SharedVariables sharedVariables = SharedVariables.Instance;

        public float X{ get; set; }
        public float Y { get; set; }
        private Vector2 velocity { get; set; }
        public static Texture2D ProjectileTexture { get; set; }

        public CharacterEntity()
        {  
            X = 1460;
            Y = 960;

            if (characterSheetTexture == null)
            {
                characterSheetTexture = sharedVariables.Content.Load<Texture2D>("charactersheet64");
            }
            if (characterBorder == null)
            {
                characterBorder = new Texture2D(sharedVariables.Graphics, 64, 64);
                characterBorder.CreateBorder(1, Color.Red);
                
            }
            
            initAnimations();
            ProjectileTexture = sharedVariables.Content.Load<Texture2D>("Projectiles/Projectile1");

            charStats = new Stats(1, 100, 5, 1, 0, 1);
        }
        private void initAnimations()
        {
            walkDown = new Animation();
            walkDown.AddFrame(new Rectangle(0, 324, 64, 64), TimeSpan.FromSeconds(.25));
            walkDown.AddFrame(new Rectangle(64, 324, 64, 64), TimeSpan.FromSeconds(.25));
            walkDown.AddFrame(new Rectangle(0, 324, 64, 64), TimeSpan.FromSeconds(.25));
            walkDown.AddFrame(new Rectangle(128, 324, 64, 64), TimeSpan.FromSeconds(.25));

            walkUp = new Animation();
            walkUp.AddFrame(new Rectangle(576, 324, 64, 64), TimeSpan.FromSeconds(.25));
            walkUp.AddFrame(new Rectangle(640, 324, 64, 64), TimeSpan.FromSeconds(.25));
            walkUp.AddFrame(new Rectangle(576, 324, 64, 64), TimeSpan.FromSeconds(.25));
            walkUp.AddFrame(new Rectangle(704, 324, 64, 64), TimeSpan.FromSeconds(.25));

            walkLeft = new Animation();
            walkLeft.AddFrame(new Rectangle(192, 324, 64, 64), TimeSpan.FromSeconds(.25));
            walkLeft.AddFrame(new Rectangle(256, 324, 64, 64), TimeSpan.FromSeconds(.25));
            walkLeft.AddFrame(new Rectangle(192, 324, 64, 64), TimeSpan.FromSeconds(.25));
            walkLeft.AddFrame(new Rectangle(320, 324, 64, 64), TimeSpan.FromSeconds(.25));

            walkRight = new Animation();
            walkRight.AddFrame(new Rectangle(384, 324, 64, 64), TimeSpan.FromSeconds(.25));
            walkRight.AddFrame(new Rectangle(448, 324, 64, 64), TimeSpan.FromSeconds(.25));
            walkRight.AddFrame(new Rectangle(384, 324, 64, 64), TimeSpan.FromSeconds(.25));
            walkRight.AddFrame(new Rectangle(512, 324, 64, 64), TimeSpan.FromSeconds(.25));

            // Standing animations only have a single frame of animation:
            standDown = new Animation();
            standDown.AddFrame(new Rectangle(0, 324, 64, 64), TimeSpan.FromSeconds(.25));

            standUp = new Animation();
            standUp.AddFrame(new Rectangle(576, 324, 64, 64), TimeSpan.FromSeconds(.25));

            standLeft = new Animation();
            standLeft.AddFrame(new Rectangle(192, 324, 64, 64), TimeSpan.FromSeconds(.25));

            standRight = new Animation();
            standRight.AddFrame(new Rectangle(384, 324, 64, 64), TimeSpan.FromSeconds(.25));
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 topLeftOfSprite = new Vector2(X, Y);
            
            spriteBatch.Draw(characterSheetTexture, topLeftOfSprite, currentAnimation.CurrentRectangle, Color.White);
            foreach (CharacterEntityShootableProjectile proj in Projectiles)
            {
                proj.Draw(spriteBatch);

            }
            //spriteBatch.Draw(characterBorder, topLeftOfSprite, Color.White);
        }

        public void Update(GameTime gameTime)
        {

            checkKeyInputs(gameTime);
            checkMouseInputShooting(gameTime);
            currentAnimation.Update(gameTime);
            updateProjectileTravel(gameTime);

            removeProjectiles();

        }
        void updateProjectileTravel(GameTime gameTime)
        {
            foreach (CharacterEntityShootableProjectile proj in Projectiles)
            {
                if (!proj.IsProjectileDead)
                {
                    proj.Update(gameTime);
                }
                else
                {
                    projectilesToRemove.Add(proj);
                }
            }
        }
        void removeProjectiles()
        {
            foreach (CharacterEntityShootableProjectile proj in projectilesToRemove)
            {
                Projectiles.Remove(proj);
            }
            projectilesToRemove.Clear();
        }
        void checkMouseInputShooting(GameTime gameTime)
        {
            MouseState newState = Mouse.GetState();
            
            //&& oldState.LeftButton == ButtonState.Released
            if (newState.LeftButton == ButtonState.Pressed  )
            {
                if (lastBulletShot == null || gameTime.TotalGameTime - lastBulletShot >= shootingCooldown)
                { 
                    CharacterEntityShootableProjectile projectile = new CharacterEntityShootableProjectile(new Vector2(X,Y),
                    new Vector2(newState.X,newState.Y),
                    ProjectileTexture, sharedVariables.Graphics);
                    Projectiles.Add(projectile);
                    lastBulletShot = gameTime.TotalGameTime;
                }
                
            }
            oldState = newState;
        }
        void checkKeyInputs(GameTime gameTime)
        {
            velocity = GetDesiredVelocityFromInput();

            X += velocity.X * (float)gameTime.ElapsedGameTime.TotalSeconds;
            Y += velocity.Y * (float)gameTime.ElapsedGameTime.TotalSeconds;

            

            // We can use the velocity variable to determine if the 
            // character is moving or standing still
            bool isMoving = velocity != Vector2.Zero;
            if (isMoving)
            {
                // If the absolute value of the X component
                // is larger than the absolute value of the Y
                // component, then that means the character is
                // moving horizontally:
                bool isMovingHorizontally = Math.Abs(velocity.X) > Math.Abs(velocity.Y);
                if (isMovingHorizontally)
                {
                    // No that we know the character is moving horizontally 
                    // we can check if the velocity is positive (moving right)
                    // or negative (moving left)
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
                    // If the character is not moving horizontally
                    // then it must be moving vertically. The SpriteBatch
                    // class treats positive Y as down, so this defines the
                    // coordinate system for our game. Therefore if
                    // Y is positive then the character is moving down.
                    // Otherwise, the character is moving up.
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

                // This else statement contains logic for if the
                // character is standing still.
                // First we are going to check if the character
                // is currently playing any walking animations.
                // If so, then we want to switch to a standing animation.
                // We want to preserve the direction that the character
                // is facing so we'll set the corresponding standing
                // animation according to the walking animation being played.
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
                // If the character is standing still but is not showing
                // any animation at all then we'll default to facing down.
                else if (currentAnimation == null)
                {
                    currentAnimation = standDown;
                }
            }
        }
        Vector2 GetDesiredVelocityFromInput()
        {
            Vector2 velocity = new Vector2();

            KeyboardState keyBoardState = Keyboard.GetState();


            if (keyBoardState.IsKeyDown(Keys.W) && !previousState.IsKeyDown(Keys.S))
            {
                velocity.Y = -3;
                velocity.Normalize();
            }
            if (keyBoardState.IsKeyDown(Keys.S) && !previousState.IsKeyDown(Keys.W))
            {
                velocity.Y = 3;
                velocity.Normalize();
            }
            if (keyBoardState.IsKeyDown(Keys.A) && !previousState.IsKeyDown(Keys.D))
            {
                velocity.X = -3;
                velocity.Normalize();
            }
            if (keyBoardState.IsKeyDown(Keys.D) && !previousState.IsKeyDown(Keys.A))
            {
                velocity.X = 3;
                velocity.Normalize();
            }
            if (keyBoardState.IsKeyDown(Keys.D) && keyBoardState.IsKeyDown(Keys.W))
            {
                velocity.X = 3;
                velocity.Y = -3;
                velocity.Normalize();
            }

            if (keyBoardState.IsKeyDown(Keys.A) && keyBoardState.IsKeyDown(Keys.S))
            {
                velocity.X = -3;
                velocity.Y = 3;
                velocity.Normalize();
            }

            if (keyBoardState.IsKeyDown(Keys.D) && keyBoardState.IsKeyDown(Keys.S))
            {
                velocity.X = 3;
                velocity.Y = 3;
                velocity.Normalize();
            }
            if (keyBoardState.IsKeyDown(Keys.A) && keyBoardState.IsKeyDown(Keys.W))
            {
                velocity.X = -3;
                velocity.Y = -3;
                velocity.Normalize();
            }
            if (keyBoardState.IsKeyDown(Keys.A) && keyBoardState.IsKeyDown(Keys.W) && keyBoardState.IsKeyDown(Keys.D))
            {
                velocity.Y = -3;
                velocity.X = 0;
                velocity.Normalize();
            }
            if (keyBoardState.IsKeyDown(Keys.S) && keyBoardState.IsKeyDown(Keys.A) && keyBoardState.IsKeyDown(Keys.D))
            {
                velocity.Y = 3;
                velocity.X = 0;
                velocity.Normalize();
            }
            if (keyBoardState.IsKeyDown(Keys.S) && keyBoardState.IsKeyDown(Keys.W) && keyBoardState.IsKeyDown(Keys.A))
            {
                velocity.Y = 0;
                velocity.X = -3;
                velocity.Normalize();
            }
            if (keyBoardState.IsKeyDown(Keys.S) && keyBoardState.IsKeyDown(Keys.W) && keyBoardState.IsKeyDown(Keys.D))
            {
                velocity.Y = 0;
                velocity.X = 3;
                velocity.Normalize();
            }
            if (keyBoardState.IsKeyDown(Keys.S) && keyBoardState.IsKeyDown(Keys.W) && keyBoardState.IsKeyDown(Keys.A) && keyBoardState.IsKeyDown(Keys.D))
            {
                velocity.Y = 0;
                velocity.X = 0;
            }
            velocity *= desiredSpeed;
            previousState = keyBoardState;
            return velocity;
        }
        public void Collision(Rectangle newRectangle)
        {

            Rectangle rect = new Rectangle((int)X, (int)Y, 66, 66);

            if (rect.TouchTopOf(newRectangle))
            {
                Y = newRectangle.Top - rect.Height;
            }

            if (rect.TouchBottomOf(newRectangle))
            {
                Y = newRectangle.Bottom;
            }

            if (rect.TouchRightOf(newRectangle))
            {
                X = newRectangle.Right;
            }
            if (rect.TouchLeftOf(newRectangle))
            {
                X = newRectangle.Left - rect.Width;
            }

        }
        public void isOutOfBounds(Rectangle boundRect)
        {
            Rectangle rect = new Rectangle((int)X, (int)Y, 66, 66);

            if(rect.X < boundRect.X)
            {
                X = boundRect.X + 1;
            }
            if (rect.X > boundRect.Width)
            {
                X = boundRect.Width - 1;
            }
            if (rect.Y < boundRect.Y)
            {
                Y = boundRect.Y + 1;
            }
            if (rect.Y > boundRect.Height)
            {
                Y = boundRect.Height - 1;
            }



        }

    }
    static class Utilities
    {
        public static void CreateBorder(this Texture2D texture, int borderWidth, Color borderColor)
        {
            
            Color[] colors = new Color[texture.Width * texture.Height];

            for (int x = 0; x < texture.Width; x++)
            {
                for (int y = 0; y < texture.Height; y++)
                {
                    bool colored = false;
                    for (int i = 0; i <= borderWidth; i++)
                    {
                        if (x == i || y == i || x == texture.Width - 1 - i || y == texture.Height - 1 - i)
                        {
                            colors[x + y * texture.Width] = borderColor;
                            colored = true;
                            break;
                        }
                    }

                    if (colored == false)
                        colors[x + y * texture.Width] = Color.Transparent;
                }
            }

            texture.SetData(colors);
        }
    }
}