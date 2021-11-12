using UnityEngine;
using XNode;


[CreateAssetMenu]
public class SimpleGraph : NodeGraph
{

    public SimpleNode GetRootNode()
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i] is SimpleNode) return nodes[i] as SimpleNode;
        }
        return null;
    }
}

