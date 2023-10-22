using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CC.SoundSystem.Editor
{
    public interface IDomain
    {
        public GraphNode AddNewGraphNode(string nodeName, Node parent = null);
    }
}
