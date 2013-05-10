﻿using System;
using GameEngine.Info;
using GameEngine.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.Drawing
{
    public class DrawableInstance
    {
        public IGameDrawable Drawable { get; set; }
        public bool Visible { get; set; }
        public float Rotation { get; set; }
        public Color Color { get; set; }
        public SpriteEffects SpriteEffects { get; set; }
        public int Layer { get; set; }
        public Vector2 Offset { get; set; }

        public double StartTimeMS { get; set; }

        // Associated state and group when added to a DrawableSet.
        // a DrawableInstance should be associated with one and only one DrawableSet.
        internal string _associatedState = null;
        internal string _associatedGroup = null;

        public DrawableInstance(IGameDrawable drawable)
        {
            this.StartTimeMS = 0;
            this.Drawable = drawable;
            this.Visible = true;
            this.Rotation = 0;
            this.Color = Color.White;
            this.SpriteEffects = SpriteEffects.None;
            this.Layer = 0;
            this.Offset = Vector2.Zero;
        }

        public int GetWidth(GameTime gameTime)
        {
            return Drawable.GetWidth(GetElapsedMS(gameTime));
        }
        
        public int GetHeight(GameTime gameTime)
        {
            return Drawable.GetHeight(GetElapsedMS(gameTime));
        }

        public double GetElapsedMS(GameTime gameTime)
        {
            return gameTime.TotalGameTime.TotalMilliseconds - StartTimeMS;
        }

        public bool IsFinished(GameTime gameTime)
        {
            return Drawable.IsFinished(GetElapsedMS(gameTime));
        }

        public void Reset(GameTime gameTime)
        {
            StartTimeMS = gameTime.TotalGameTime.TotalMilliseconds;
        }

        public Rectangle Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 destination, float layerDepth, float ScaleX, float ScaleY, float opacity, float maxY, ViewPortInfo viewPortInfo, bool showDrawableComponents)
        {
            // The relative position of the object should always be (X,Y) - (globalDispX, globalDispY). globalDispX and globalDispY
            // are based on viewPortInfo.TopLeftX and viewPortInfo.TopLeftY. viewPortInfo.TopLeftX and viewPortInfo.TopLeftY have 
            // already been corrected in terms of the bounds of the WORLD map coordinates. This allows for panning at the edges.

            int currentFrameWidth = GetWidth(gameTime);
            int currentFrameHeight = GetHeight(gameTime);

            int pxObjectWidth = (int)Math.Ceiling(currentFrameWidth * ScaleX * viewPortInfo.ActualZoom);
            int pxObjectHeight = (int)Math.Ceiling(currentFrameHeight * ScaleY * viewPortInfo.ActualZoom);

            // Draw the Object based on the current Frame dimensions and the specified Object Width Height values.
            Rectangle objectDestRect = new Rectangle(
                    (int)Math.Ceiling(destination.X) + (int)Math.Ceiling(Offset.X * viewPortInfo.ActualZoom),
                    (int)Math.Ceiling(destination.Y) + (int)Math.Ceiling(Offset.Y * viewPortInfo.ActualZoom),
                    pxObjectWidth,
                    pxObjectHeight
            );

            Vector2 drawableOrigin = new Vector2(
                (float)Math.Ceiling(Drawable.Origin.X * currentFrameWidth),
                (float)Math.Ceiling(Drawable.Origin.Y * currentFrameHeight)
                );

            Color drawableColor = new Color()
            {
                R = Color.R,
                G = Color.G,
                B = Color.B,
                A = (byte)(Color.A * opacity)
            };

            // Further adjust the layer depth based on this drawables layer value
            layerDepth += Layer / maxY;

            if (layerDepth > 1) layerDepth = 1;
            if (layerDepth < 0) layerDepth = 0;

            Drawable.Draw(spriteBatch, objectDestRect, drawableColor, Rotation, drawableOrigin, SpriteEffects, layerDepth, GetElapsedMS(gameTime));

            return objectDestRect;
        }

        public override string ToString()
        {
            return string.Format("DrawableInstance: Visible={0}, Layer={1}, Color={2}, Rotation={3}",
                Visible, Layer, Color, Rotation);
        }
    }
}