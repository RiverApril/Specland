using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Specland {
    public class FpsControl {

        private Stopwatch stopWatch;

        private int TempFrames = 0;
        private long TempTicks = 0;
        private long TempMilliseconds = 0;

        private int FramesPerSecond = 0;
        private long TicksperSecond = 0;
        private float MillisecondsPerTick = 0;

        public FpsControl() {
            stopWatch = Stopwatch.StartNew();
            stopWatch.Start();
        }


        /*internal void update(GameTime gameTime) {
            frameCounter++;
            frameTime += gameTime.ElapsedGameTime.Milliseconds;
            if (frameTime >= 1000) {
                currentFrameRate = frameCounter;
                frameTime = 0;
                frameCounter = 0;
            }
        }*/

        public void update() {
            TempFrames++;
            TempTicks += stopWatch.ElapsedTicks;
            TempMilliseconds += stopWatch.ElapsedMilliseconds;
            if (TempMilliseconds >= 1000) {
                FramesPerSecond = TempFrames;
                TicksperSecond = TempTicks;
                MillisecondsPerTick = (float)TempMilliseconds / (float)FramesPerSecond;
                TempFrames = 0;
                TempTicks = 0;
                TempMilliseconds = 0;
            }
            stopWatch.Restart();
        }

        public int getFps() {
            return FramesPerSecond;
        }

        public long getTps() {
            return TicksperSecond;
        }

        public float getMpt() {
            return MillisecondsPerTick;
        }
    }
}
