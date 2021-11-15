using UnityEngine;

namespace XNode.Examples.MathNodes {
    [System.Serializable]
    public class MathNode : XNode.Node {
        // Adding [Input] or [Output] is all you need to do to register a field as a valid port on your node 
        [Input] public float a;
        [Input] public float b;
        [Input] public float forceSetValue;
        // The value of an output node field is not used for anything, but could be used for caching output results
        [Output] public float result;

        // Will be displayed as an editable field - just like the normal inspector
        public MathType mathType = MathType.Add;
        public enum MathType { Add, Subtract, Multiply, Divide }

        // GetValue should be overridden to return a value for any specified output port
        public override object GetValue(XNode.NodePort port) {

            // Get new a and b values from input connections. Fallback to field values if input is not connected
            float a = GetInputValue<float>("a", 11);
            float b = GetInputValue<float>("b", 12);
            float c = GetInputValue<float>("a", 13);
            float d = GetInputValue<float>("d", 21);
            float e = GetInputValue<float>("e", this.a);

            Debug.Log($"### a[{a}] b[{b}] c[{c}] d[{d}] e[{e}]");

            // After you've gotten your input values, you can perform your calculations and return a value
            result = 0f;
            if (port.fieldName == "result")
                switch (mathType) {
                    case MathType.Add: default: result = a+b; break;
                    case MathType.Subtract: result = a - b; break;
                    case MathType.Multiply: result = a * b; break;
                    case MathType.Divide: result = a / b; break;
                }
            if (forceSetValue != 0)
            {
                result = forceSetValue;
            }
            return result;
        }
    }
}