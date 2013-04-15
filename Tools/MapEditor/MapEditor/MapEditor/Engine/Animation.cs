//Animation.cs
//Copyright Dejitaru Forge 2011

using System;
using System.Collections.Generic;
using System.Text;

namespace MapEditor
{
    /// <summary>
    /// The type of frame time the animation uses
    /// </summary>
    public enum FrameTimeType
    {
        /// <summary>
        /// Time represented in days
        /// </summary>
        Days,
        /// <summary>
        /// Time represented in hours
        /// </summary>
        Hours,
        /// <summary>
        /// Time represented in minutes
        /// </summary>
        Minutes,
        /// <summary>
        /// Time represented in seconds
        /// </summary>
        Seconds,
        /// <summary>
        /// Time represented in milliseconds
        /// </summary>
        Milliseconds,
        /// <summary>
        /// (per) Frame based animation
        /// </summary>
        Frames,
        /// <summary>
        /// An unspecified format
        /// </summary>
        Other
    }

    /// <summary>
    /// A generic animation class
    /// </summary>
    public class Animation
    {
        #region Data

        /// <summary>
        /// The type of frame time for this animation
        /// </summary>
        public FrameTimeType frameTimeType = FrameTimeType.Other; //default

        /// <summary>
        /// How long to wait before starting the animation (ignored in frame based animation)
        /// </summary>
        public float delay = 0.0f;

        /// <summary>
        /// Only used for frame based animation
        /// </summary>
        public int framesPerSecond = 30;

        /// <summary>
        /// The number of frames (optional)
        /// </summary>
        public int frames = 1;

        /// <summary>
        /// Current frame (starting at 0)
        /// </summary>
        public int currentFrame = -1;

        /// <summary>
        /// Starting frame
        /// </summary>
        protected int startFrame = 0;
        
        /// <summary>
        /// The time when the animation started
        /// </summary>
        public long startTime;

        /// <summary>
        /// Is this animation looping?
        /// </summary>
        public bool looping = true;

        /// <summary>
        /// Is the animation updating (frames advancing)
        /// </summary>
        public bool isUpdating = true;

        /// <summary>
        /// Length (based on frame type time) of each frame (modulo for frame based anim)
        /// </summary>
        public int length;

        /// <summary>
        /// THe last time that the animation was updated (irrelevent in frame based animation)
        /// </summary>
        public long lastRefresh { get; protected set; }

        /// <summary>
        /// frame count (updated every call) - used for frame based animation
        /// </summary>
        private int fCount = 0;

        #endregion


        #region Initialization

        /// <summary>
        /// Create a new animation
        /// </summary>
        /// <param name="type">The frame time type (seconds/frames,etc.)</param>
        /// <param name="Delay">Delay before starting animation</param>
        /// <param name="Looping">is the animation looping</param>
        /// <param name="Frames">Number of frames/length</param>
        /// <param name="StartFrame">Frame to start the animation on (starting at 0)</param>
        /// <param name="Length">Length of each frame in specified time marking</param>
        /// <param name="StartTime">Start time</param>
        public Animation(FrameTimeType type, float Delay, bool Looping, int Frames, int StartFrame, int Length)
        {
            frameTimeType = type;
            delay = Delay;
            looping = Looping;
            frames = Frames;
            currentFrame = 0;
            startFrame = StartFrame;
            length = Length;
            startTime = DateTime.UtcNow.Ticks;
        }

        public Animation()
        {
            frameTimeType = FrameTimeType.Other;
            delay = 0;
            looping = true;
            frames = 1;
            currentFrame = -1;
            startFrame = 0;
            startTime = DateTime.UtcNow.Ticks;
        }

        /// <summary>
        /// Check if it is safe to start animating (time is passed the delay point)
        ///
        /// On frame based animation, delay is calculated by framesPerSecond variable (average fps) - defaulting at 30
        /// </summary>
        /// <returns>true on completion</returns>
        public virtual bool IsDelayComplete()
        {
            bool isComplete = false;

            if (frameTimeType == FrameTimeType.Frames) //frames
                isComplete = true;

            else if (frameTimeType == FrameTimeType.Milliseconds && DateTime.UtcNow.Ticks > startTime + delay) //ms
                isComplete = true;
                /*
            else if (frameTimeType == FrameTimeType.Seconds && DateTime.Now.Second > startTime.Second + delay) //sec
                isComplete = true;

            else if (frameTimeType == FrameTimeType.Minutes && DateTime.Now.Minute > startTime.Minute + delay) //min
                isComplete = true;

            else if (frameTimeType == FrameTimeType.Hours && DateTime.Now.Hour > startTime.Hour + delay) //hr
                isComplete = true;

            else if (frameTimeType == FrameTimeType.Days && DateTime.Now.Day > startTime.Day + delay) //day
                isComplete = true;
            else if (frameTimeType == FrameTimeType.Other)
                isComplete = true;
                 */

            return isComplete;
        }

#endregion


        #region Update

        /// <summary>
        /// updates the animation
        /// </summary>
        public virtual void UpdateFrame()
        {
            if (!isUpdating || (!looping && currentFrame >= frames)) //nothing to update yet
                return;

            if (frameTimeType == FrameTimeType.Frames)
                if (fCount % length == 0)
                currentFrame++;

            if (frameTimeType == FrameTimeType.Milliseconds)
            {
                if ((DateTime.UtcNow.Ticks - lastRefresh) / 10000 > length) //convert ticks (in 100ns) to ms and check time
                {
                    currentFrame++;
                    lastRefresh = DateTime.UtcNow.Ticks;
                }
            }

            else if (frameTimeType == FrameTimeType.Seconds)
            {
                if ((DateTime.UtcNow.Ticks - lastRefresh) / 100000 > length) //convert ticks (in 100ns) to s and check time
                {
                    currentFrame++;
                    lastRefresh = DateTime.UtcNow.Ticks;
                }
            }

            else if (frameTimeType == FrameTimeType.Minutes)
            {
                if ((DateTime.UtcNow.Ticks - lastRefresh) / 1000000 > length) //convert ticks (in 100ns) to m and check time
                {
                    currentFrame++;
                    lastRefresh = DateTime.UtcNow.Ticks;
                }
            }

            else if (frameTimeType == FrameTimeType.Hours)
            {
                if ((DateTime.UtcNow.Ticks - lastRefresh) / 10000000 > length) //convert ticks (in 100ns) to hr and check time
                {
                    currentFrame++;
                    lastRefresh = DateTime.UtcNow.Ticks;
                }
            }

            else if (frameTimeType == FrameTimeType.Days)
            {
                if ((DateTime.UtcNow.Ticks - lastRefresh) / 100000000 > length) //convert ticks (in 100ns) to d and check time
                {
                    currentFrame++;
                    lastRefresh = DateTime.UtcNow.Ticks;
                }
            }

            if (looping)
                currentFrame %= frames;

            fCount = (fCount + 1) % 0xffff;
        }

        #endregion


        #region Other

        /// <summary>
        /// Reset the animation
        /// </summary>
        public void ResetFrame()
        {
            currentFrame = 0;

            lastRefresh = DateTime.UtcNow.Ticks;
            startTime = DateTime.UtcNow.Ticks;
        }

        #endregion
    }
}
