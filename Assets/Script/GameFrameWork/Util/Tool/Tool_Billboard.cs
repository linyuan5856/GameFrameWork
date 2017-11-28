using UnityEngine;
using System.Collections;

public class Tool_Billboard : MonoBehaviour
{

    public Camera cameraToLookAt = null;
    public bool x = false;
    public bool y = true;
    public bool z = false;

    void Start()
    {
        if (this.cameraToLookAt == null)
            this.cameraToLookAt = Camera.main;
    }

    void Update()
    {
        if (this.cameraToLookAt != null)
        {
            Vector3 v = this.cameraToLookAt.transform.position - transform.position;
            //v.x = v.z = 0.0f;
            v.x = !this.x ? 0.0f : v.x;
            v.y = !this.y ? 0.0f : v.y;
            v.z = !this.z ? 0.0f : v.z;
            v.x = v.y = 0;

            this.transform.LookAt(cameraToLookAt.transform.position - v);
        }
        
    }
}
