/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

namespace NoZ.RuneHaze
{
    internal struct PlayerSpawned : ISignal
    {
        public Player Player;
    }

    internal struct PlayerDeSpawned : ISignal
    {
        public Player Player;
    }

    internal struct ActorSpawnEvent : ISignal
    {
        public Actor Actor;
    }
    
    internal struct ActorDiedEvent : ISignal
    {
        public Actor Actor;
    }

    internal struct ActorDespawnEvent : ISignal
    {
        public Actor Actor;
    }
}
