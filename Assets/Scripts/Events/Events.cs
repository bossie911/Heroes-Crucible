using System;
using UnityEngine;
using GameStudio.HunterGatherer.Divisions.Upgrades;
using GameStudio.HunterGatherer.Divisions;

public static class Events
{
    //Example normal event
    public static readonly Evt onUpgradeExample = new Evt();

    //Example event 
    public static readonly Evt<int> onUpgradeParameterExample = new Evt<int>();

    //Upgrade Events
    public static readonly Evt<Division, UpgradeBase> onUpgrade = new Evt<Division, UpgradeBase>();

}

/// <summary>
/// Custom event class for Events with 0 parameters.
/// </summary>
public class Evt
{
    private event Action _action = delegate { };

    public void Invoke()
    {
        _action.Invoke();
    }

    public void AddListener(Action listener)
    {
        _action -= listener;
        _action += listener;
    }

    public void RemoveListener(Action listener)
    {
        _action -= listener;
    }
}

/// <summary>
/// Custom event class for Events with 0 parameters.
/// </summary>
public class Evt<T>
{
    private event Action<T> _action = delegate { };

    public void Invoke(T param)
    {
        _action.Invoke(param);
    }

    public void AddListener(Action<T> listener)
    {
        _action -= listener;
        _action += listener;
    }

    public void RemoveListener(Action<T> listener)
    {
        _action -= listener;
    }
}

/// <summary>
/// Custom event class for Events with 0 parameters.
/// </summary>
public class Evt<T1, T2>
{
    private event Action<T1, T2> _action = delegate { };

    public void Invoke(T1 param1, T2 param2)
    {
        _action.Invoke(param1, param2);
    }

    public void AddListener(Action<T1, T2> listener)
    {
        _action -= listener;
        _action += listener;
    }

    public void RemoveListener(Action<T1, T2> listener)
    {
        _action -= listener;
    }
}

/// <summary>
/// Custom event class for Events with 3 parameters.
/// </summary>
public class Evt<T1, T2, T3>
{
    private event Action<T1, T2, T3> _action = delegate { };

    public void Invoke(T1 param1, T2 param2, T3 param3)
    {
        _action.Invoke(param1, param2, param3);
    }

    public void AddListener(Action<T1, T2, T3> listener)
    {
        _action -= listener;
        _action += listener;
    }

    public void RemoveListener(Action<T1, T2, T3> listener)
    {
        _action -= listener;
    }
}
