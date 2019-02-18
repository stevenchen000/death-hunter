﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MovementSystem
{
    public abstract class CharacterRotation : ScriptableObject
    {
        public string description;

        public abstract void Rotate(MovementManager character, Vector3 direction);

    }
}