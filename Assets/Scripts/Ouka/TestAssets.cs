using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAssets : ScriptableObject
{
    public int[] ints = new int[10];


    public List<GameObject> gameObjects;

    [Serializable]
    public class Character
    {
        public int ID;
        public string Name;
        public int Level;
    }
}


