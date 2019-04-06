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
    public readonly int index;
    public readonly Sprite effectFile;
    public readonly string name;
    public readonly TargetType targetType;
    public readonly ElementType elementType;
    public readonly int power;
    public readonly int priority;

    public SkillBase(int index, Sprite effectFile, string name, TargetType targetType, ElementType elementType, int power, int priority)
    {
        this.index = index;
        this.effectFile = effectFile;
        this.name = name ?? throw new ArgumentNullException(nameof(name));
        this.targetType = targetType;
        this.elementType = elementType;
        this.power = power;
        this.priority = priority;
    }
}
