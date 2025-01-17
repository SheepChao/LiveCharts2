﻿using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing;
using SkiaSharp.Views.Desktop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiveChartsCore.SkiaSharpView.WinForms
{
    /// <summary>
    /// The motion canvas control for windows forms, <see cref="MotionCanvas{TDrawingContext}"/>.
    /// </summary>
    /// <seealso cref="UserControl" />
    public partial class MotionCanvas : UserControl
    {
        private bool isDrawingLoopRunning = false;
        private HashSet<IDrawableTask<SkiaSharpDrawingContext>> paintTasks = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="MotionCanvas"/> class.
        /// </summary>
        public MotionCanvas()
        {
            InitializeComponent();
            CanvasCore.Invalidated += CanvasCore_Invalidated;
        }

        /// <summary>
        /// Gets or sets the paint tasks.
        /// </summary>
        /// <value>
        /// The paint tasks.
        /// </value>
        public HashSet<IDrawableTask<SkiaSharpDrawingContext>> PaintTasks { get => paintTasks; set { paintTasks = value; OnPaintTasksChanged(); } }

        /// <summary>
        /// Gets or sets the frames per second.
        /// </summary>
        /// <value>
        /// The frames per second.
        /// </value>
        public double FramesPerSecond { get; set; } = 90;

        /// <summary>
        /// Gets the canvas core.
        /// </summary>
        /// <value>
        /// The canvas core.
        /// </value>
        public MotionCanvas<SkiaSharpDrawingContext> CanvasCore { get; } = new();

        private void SkControl_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            CanvasCore.DrawFrame(new SkiaSharpDrawingContext(CanvasCore, e.Info, e.Surface, e.Surface.Canvas));
        }

        private void CanvasCore_Invalidated(MotionCanvas<SkiaSharpDrawingContext> sender)
        {
            RunDrawingLoop();
        }

        private async void RunDrawingLoop()
        {
            if (isDrawingLoopRunning) return;
            isDrawingLoopRunning = true;

            var ts = TimeSpan.FromSeconds(1 / FramesPerSecond);
            while (!CanvasCore.IsValid)
            {
                skControl2.Invalidate();
                await Task.Delay(ts);
            }

            isDrawingLoopRunning = false;
        }

        private void OnPaintTasksChanged()
        {
            CanvasCore.SetPaintTasks(paintTasks);
        }
    }
}
