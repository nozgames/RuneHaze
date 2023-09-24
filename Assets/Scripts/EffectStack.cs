/*

    Copyright (c) 2023 NoZ Games, LLC. All rights reserved.

*/

using System.Collections.Generic;

namespace NoZ.RuneHaze
{
    public class EffectStack
    {
        public Effect Effect;
        public LinkedList<EffectContext> Contexts = new LinkedList<EffectContext>();
        public LinkedListNode<EffectStack> Node;

        public EffectStack()
        {
            Node = new LinkedListNode<EffectStack>(this);
        }
    }
}