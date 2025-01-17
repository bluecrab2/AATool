﻿using System;
using AATool.Configuration;
using AATool.Graphics;
using AATool.UI.Screens;
using AATool.Utilities;
using Microsoft.Xna.Framework;

namespace AATool.UI.Controls
{
    class UIBlockPopup : UIPanel
    {
        private const float LerpSpeed = 30;
        private const int Gap = 28;

        private UITextBlock label;
        private UIBlockTile source;
        private UIBlockTile preview;
        private UIPanel window;
        private UIControl topArrow;
        private UIControl bottomArrow;

        private Vector2 targetLocation;
        private Vector2 currentLocation;
        private Point previousLocation;

        private readonly Timer showTimer = new (5);

        public UIBlockPopup()
        {
            this.BuildFromTemplate();
            this.Layer = Layer.Fore;
        }

        public void Finalize(Time time)
        {
            this.showTimer.Update(time);
            bool justExpanded = false;
            if (this.showTimer.IsExpired)
            {
                this.Collapse();
                this.source = null;
                
            }
            else if (this.IsCollapsed)
            {
                this.Expand();
                justExpanded = true;
            }

            this.currentLocation = Vector2.Lerp(this.currentLocation, this.targetLocation, (float)(LerpSpeed * time.Delta));
            if (Vector2.Distance(this.currentLocation, this.targetLocation) < 0.1f)
                this.currentLocation = this.targetLocation;

            var nextLocation = this.currentLocation.ToPoint();
            if (nextLocation != this.previousLocation || justExpanded)
            {
                int x = nextLocation.X - (this.Width / 2);
                x = MathHelper.Clamp(x, -32, this.Root().Width - 32);
                this.MoveTo(new Point(x, nextLocation.Y + 8));
                this.previousLocation = nextLocation;
            }
        }

        public override void DrawThis(Canvas canvas)
        {
            canvas.Draw($"popup_block_glow", this.window.Bounds, Config.Main.TextColor, Layer.Fore);
            canvas.Draw($"popup_block_back", this.window.Bounds, this.Root().FrameBackColor(), Layer.Fore);
            canvas.Draw($"popup_block_border", this.window.Bounds, this.Root().FrameBorderColor(), Layer.Fore);

            if (this.VerticalAlign is VerticalAlign.Top)
            {
                canvas.Draw($"popup_block_arrow_bottom_glow",
                    this.bottomArrow.Bounds,
                    Config.Main.TextColor,
                    Layer.Fore);
                canvas.Draw($"popup_block_arrow_bottom_back",
                    this.bottomArrow.Bounds,
                    this.Root().FrameBackColor(),
                    Layer.Fore);
                canvas.Draw($"popup_block_arrow_bottom_border",
                    this.bottomArrow.Bounds,
                    this.Root().FrameBorderColor(),
                    Layer.Fore);
            }
            else
            {
                canvas.Draw($"popup_block_arrow_top_glow",
                    this.topArrow.Bounds,
                    Config.Main.TextColor,
                    Layer.Fore);
                canvas.Draw($"popup_block_arrow_top_back",
                    this.topArrow.Bounds,
                    this.Root().FrameBackColor(),
                    Layer.Fore);
                canvas.Draw($"popup_block_arrow_top_border",
                    this.topArrow.Bounds,
                    this.Root().FrameBorderColor(),
                    Layer.Fore);

            }
        }

        public override void InitializeThis(UIScreen screen)
        {
            this.window = this.First<UIPanel>("window");
            this.label = this.window.First<UITextBlock>();
            this.topArrow = this.First("arrow_top");
            this.bottomArrow = this.First("arrow_bottom");
            this.window.DrawMode = DrawMode.ChildrenOnly;
        }

        public override void MoveTo(Point point)
        {
            if (this.VerticalAlign is VerticalAlign.Top)
            {
                base.MoveTo(new Point(point.X, point.Y - this.Height - 8));
            }
            else
            {
                base.MoveTo(new Point(point.X, point.Y - 8));
            }
        }

        public void SetSource(UIBlockTile block)
        {
            if (block is null)
                return;

            if (block.Block == this.source?.Block)
            {
                this.showTimer.Expire();
                return;
            }

            if (this.source is null)
            {
                this.Arrange(block.Center);
                this.targetLocation = block.Center.ToVector2();
                this.currentLocation = block.Center.ToVector2();
                this.MoveTo(block.Center);
            }
            else
            {
                this.Arrange(block.Center);
            }
            this.showTimer.Reset();

            if (this.source == block)
                return;

            this.source = block;
            this.label.SetText(block.Block.Name);
            this.targetLocation = block.Center.ToVector2();

            this.RemoveControl(this.preview);
            int top = block.Block.DoubleHeight || block.BlockId is "minecraft:kelp" ? 36 : 70;
            this.preview = new UIBlockTile(block.Block.DoubleHeight ? 2 : 3) {
                VerticalAlign = VerticalAlign.Top,
                DrawMode = DrawMode.ChildrenOnly,
                BlockId = block.Block.Id,
                Margin = new Margin(0, 0, top, 0),
                Layer = Layer.Fore,
            };
            this.window.ClearControls();
            this.window.AddControl(this.label);
            this.window.AddControl(this.preview);
            this.preview.InitializeRecursive(this.Root());

            this.ResizeRecursive(this.Parent.Inner);
            UIMainScreen.Invalidate();
        }

        public void Arrange(Point targetCenter)
        {
            //this.X = this.source.Center.X - (this.Width / 2);

            this.HorizontalAlign = HorizontalAlign.Center;
            //if (this.Left < 0)
            //    this.HorizontalAlign = HorizontalAlign.Right;
            //else if (this.Right > this.Parent.Width)
            //    this.HorizontalAlign = HorizontalAlign.Left;

            this.VerticalAlign = targetCenter.Y - this.Height >= 0
                ? VerticalAlign.Top
                : VerticalAlign.Bottom; 

            /*
            this.X = this.HorizontalAlign switch {
                HorizontalAlign.Center => this.source.Center.X - (this.Width / 2),
                HorizontalAlign.Left => this.source.Left - this.Width,
                _ => this.source.Left
            };

            this.Y = this.VerticalAlign switch {
                VerticalAlign.Center => this.source.Center.Y - (this.Height / 2),
                VerticalAlign.Top => this.source.Center.Y - 8 - this.Height,
                _ => this.source.Center.Y + 8
            };
            
            this.Padding.Resize(this.Size);

            //calculate internal rectangle
            this.Inner = new Rectangle(
                this.X + this.Padding.Left,
                this.Y + this.Padding.Top,
                this.Width - this.Padding.Horizontal,
                this.Height - this.Padding.Vertical);
            */

        }
    }
}
