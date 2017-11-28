using UnityEngine;
using System.Collections;
using System;

public class Tool_Destroy : MonoBehaviour
{
    public float delaySeconds = 4;

	void Start () {
		GameObject.Destroy(this.gameObject, this.delaySeconds);
	}

}
