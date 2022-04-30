using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Path Cable Materials", menuName = "Patch material list", order = 1)]
public class PatchCableMaterials : ScriptableObject
{
    public Material normal;
    public Material locked;

}
