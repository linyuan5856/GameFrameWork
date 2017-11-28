using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Tool_RayCast : MaskableGraphic {

    protected Tool_RayCast()
    {
        useLegacyMeshGeneration = false;
    }

    protected override void OnPopulateMesh(VertexHelper toFill)
    {
        toFill.Clear();
    }
}
