﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbilitySystem;

namespace AbilitySystem
{
    [CreateAssetMenu(menuName = "Abilities/Combos/Combo Lists/Basic Combo")]
    public class ComboList : ScriptableObject
    {
        public Texture image;
        public List<Combo> combos;
    }
}