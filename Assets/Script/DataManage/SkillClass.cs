using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TargetType
{
    SingleEnemy,
    AllEnemy,
    SingleAlli,
    AllAlli,
    All,
    None = 999
}

public enum ElementType
{
    Fire,
    Ice,
    Elec,
    None = 999
}


public class SkillBase
{
    readonly int index;
    readonly string effectFile;
    readonly string name;
    readonly TargetType targetType;
    readonly ElementType elementType;
    readonly int power;
    readonly int priority;

    public SkillBase(int index, string effectFile, string name, TargetType targetType, ElementType elementType, int power, int priority)
    {
        this.index = index;
        this.effectFile = effectFile ?? throw new ArgumentNullException(nameof(effectFile));
        this.name = name ?? throw new ArgumentNullException(nameof(name));
        this.targetType = targetType;
        this.elementType = elementType;
        this.power = power;
        this.priority = priority;
    }
}
