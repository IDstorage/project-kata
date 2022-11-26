#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DynamicTrailRenderer))]
public class DynamicTrailRendererEditor : Editor
{
}
#endif