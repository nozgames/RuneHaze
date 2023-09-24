/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

namespace NoZ.RuneHaze
{
    /// <summary>
    /// Current high level state of the actor
    /// </summary>
    public enum ActorState
    {
        /// <summary>
        /// Actor has no current state
        /// </summary>
        None,

        /// <summary>
        /// Actor is spawning
        /// </summary>
        Spawn,

        /// <summary>
        /// Actor is playing an intro sequence
        /// </summary>
        Intro,

        /// <summary>
        /// Actor is active and thinking
        /// </summary>
        Active,

        /// <summary>
        /// Actor is dead
        /// </summary>
        Dead
    }
}
