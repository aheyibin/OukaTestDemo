using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using XNodeEditor;

public class SimpleNode : Node
{
    [Input] public float value;
    [Output] public float result;

    public override object GetValue(NodePort port)
    {
        if (port.fieldName == "result")
        {
            return GetInputValue<float>("value", this.value) + 1;
        }
        else return null;
    }
}
