/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using System.Collections.Generic;

namespace NoZ.RuneHaze
{
    /// <summary>
    /// Maintains a network synchronized list of effects for a given actor
    /// </summary>
    public class EffectList
    {
        private static readonly List<EffectStack> _stackPool = new();

        public enum ChangeEventType : byte
        {
            Add,
            Remove
        }

        private struct ChangeEvent
        {
            /// <summary>
            /// Type of event
            /// </summary>
            public ChangeEventType Type;

            /// <summary>
            /// Tick the event started
            /// </summary>
            public int Tick;

            /// <summary>
            /// Identifier assigned to the context
            /// </summary>
            public uint ContextId;

            /// <summary>
            /// Network Id of the effect 
            /// </summary>
            public ushort EffectId;

            /// <summary>
            /// Network Object Id of the source actor 
            /// </summary>
            public ulong SourceId;
        }

        private readonly Actor _actor;
        private readonly List<ChangeEvent> _events = new(64);

        public LinkedList<EffectStack> Stacks { get; } = new();

        /// <summary>
        /// Bind the effect list to a specific actor
        /// </summary>
        public EffectList (Actor actor)
        {
            _actor = actor;
        }

        /// <inheritdoc />
        public void ResetDirty()
        {
            _events.Clear();
        }

        /// <inheritdoc />
        public bool IsDirty()
        {
            // we call the base class to allow the SetDirty() mechanism to work
            return _events.Count > 0;
        }
        
        public void Add(Actor source, Effect effect)
        {
            if (effect == null || _actor == null)
                return;

            // Add the new effect
            AddEvent(ChangeEventType.Add, effect, sourceId : source.NetworkObjectId);

            // Remove instant effects immediately
            RemoveEffects(EffectLifetime.Instant);
        }

        /// <summary>
        /// Remove all effects that match the given lifetime
        /// </summary>
        public void RemoveEffects(EffectLifetime lifetime)
        {
            var tick = NetworkManager.Singleton.ServerTime.Tick;
            var tickRate = 1.0f / NetworkManager.Singleton.ServerTime.TickRate;

            LinkedListNode<EffectStack> nextStackNode;
            for (var stackNode = Stacks.First; stackNode != null; stackNode = nextStackNode)
            {
                nextStackNode = stackNode.Next;
                var stack = stackNode.Value;

                LinkedListNode<EffectContext> nextContextNode;
                for(var contextNode = stack.Contexts.First; contextNode != null; contextNode = nextContextNode)
                {
                    nextContextNode = contextNode.Next;
                    var context = contextNode.Value;
                    if (lifetime == context.Lifetime && (lifetime != EffectLifetime.Time || (tick - context.Tick) * tickRate >= context.Duration))
                        AddEvent(ChangeEventType.Remove, contextId: context.Id);
                }
            }
        }

        private void AddEvent(ChangeEventType type, Effect effect=null, ulong sourceId = 0, uint contextId=0)
        {
            _events.Add(HandleEvent(new ChangeEvent
            {
                Type = type,
                Tick = NetworkManager.Singleton.ServerTime.Tick,
                EffectId = effect == null ? (ushort)0 : effect.NetworkId,
                SourceId = sourceId,
                ContextId = contextId
            }));

            SetDirty(true);
        }

        private ChangeEvent HandleEvent(ChangeEvent evt)
        {
            switch (evt.Type)
            {
                case ChangeEventType.Add:
                    {
                        var effect = NetworkScriptableObject<Effect>.Get(evt.EffectId);

                        // Convert the sourceId into an Actor
                        var source = Actor.FromNetworkId(evt.SourceId);

                        // Create the effect context
                        var context = EffectContext.Get(effect, source, _actor, evt.ContextId);

                        // Add the effect context to the end of the stack
                        var stack = GetStack(effect);
                        if(null == stack)
                            stack = AddStack(effect);
                        stack.Contexts.AddLast(context.Node);

                        // Handle maximum stacks
                        while(stack.Contexts.Count > effect.MaximumStacks)
                            stack.Contexts.First.Value.Release();

                        for(var componentNode = context.Components.Last; componentNode != null; componentNode = componentNode.Previous)
                        {
                            var component = componentNode.Value;
                            if (component.Tag == null)
                                component.Enabled = true;
                            else
                                UpdateState(component.Tag);
                        }

                        evt.ContextId = context.Id;

                        return evt;
                    }

                case ChangeEventType.Remove:
                    {
                        foreach(var stack in Stacks)
                            foreach(var context in stack.Contexts)
                                if(context.Id == evt.ContextId)
                                {
                                    context.Release(UpdateState);

                                    if (stack.Contexts.Count == 0)
                                        RemoveStack(stack);

                                    return evt;
                                }
                    }
                    return evt;
            }

            return evt;
        }

        public void Dispose()
        {
            Clear();
        }

        public void Clear()
        {
            _events.Clear();

            // Free all stacks
            while (Stacks.Count > 0)
                RemoveStack(Stacks.First.Value);
        }

        private void UpdateState (Tag tag)
        {
            if (tag == null)
                return;

            EffectComponentContext componentToEnable = null;

            for(var stackNode = Stacks.Last; stackNode != null; stackNode = stackNode.Previous)
            {
                var stack = stackNode.Value;
                for (var contextNode = stack.Contexts.Last; contextNode != null; contextNode = contextNode.Previous)
                {
                    var context = contextNode.Value;
                    for (var componentNode = context.Components.Last; componentNode != null; componentNode = componentNode.Previous)
                    {
                        var component = componentNode.Value;

                        if (component.Tag == tag)
                        {
                            component.Enabled = false;
                            if (componentToEnable == null)
                                componentToEnable = component;
                        }
                    }
                }
            }

            if (componentToEnable != null)
                componentToEnable.Enabled = true;
        }

        private EffectStack GetStack (Effect effect)
        {
            for (var stackNode = Stacks.First; stackNode != null; stackNode = stackNode.Next)
                if (stackNode.Value.Effect == effect)
                    return stackNode.Value;

            return null;
        }

        private EffectStack AddStack (Effect effect)
        {
            EffectStack stack;
            if (_stackPool.Count > 0)
            {
                stack = _stackPool[^1];
                _stackPool.RemoveAt(_stackPool.Count - 1);
            }
            else
                stack = new EffectStack();

            stack.Effect = effect;

            for(var stackNode = Stacks.First; stackNode != null; stackNode = stackNode.Next)
            {
                if(effect.Priority < stackNode.Value.Effect.Priority)
                {
                    Stacks.AddBefore(stackNode, stack.Node);
                    break;
                }
            }

            if (stack.Node.List == null)
                Stacks.AddLast(stack.Node);

            return stack;
        }

        private void RemoveStack (EffectStack stack)
        {
            if (stack.Node.List != null)
                stack.Node.List.Remove(stack.Node);

            while (stack.Contexts.Count > 0)
                stack.Contexts.First.Value.Release();
        }
    }
}
