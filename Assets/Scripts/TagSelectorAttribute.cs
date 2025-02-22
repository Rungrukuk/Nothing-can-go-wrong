using System;
using UnityEngine;

/// <summary>
/// Custom attribute to allow tag selection in the Inspector.
/// </summary>
[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public class TagSelectorAttribute : PropertyAttribute { }
