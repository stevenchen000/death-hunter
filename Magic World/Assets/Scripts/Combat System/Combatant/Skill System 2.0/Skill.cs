﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NewSkillSystem
{
    public class Skill : ScriptableObject
    {
        public string skillName;
        public List<SkillObject> skillObjects;


        public void CastSkill(CastManager caster, float previousFrame, float currentFrame) {

        }


    }
}