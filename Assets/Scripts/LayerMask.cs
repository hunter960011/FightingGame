﻿using System;
using UnityEngine;

[Serializable]
public enum LayerMaskEnum { Ground, Hurtbox, Default, UI, Player, Airbox, Groundedbox };

public class LayerMask : MonoBehaviour
{
	[SerializeField] private LayerMaskEnum _layerMaskEnum = default;

	public LayerMaskEnum LayerMaskEnum { get { return _layerMaskEnum; } private set { } }
}
