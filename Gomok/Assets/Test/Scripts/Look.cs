using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Look : MonoBehaviour
{
    Transform target;

    Vector3 rot;

    private void Awake()
    {
        target = Camera.main.transform;
    }

    void Update()
    {
        this.transform.LookAt(target);

        rot = this.transform.eulerAngles;
        rot.y = 180;
        this.transform.eulerAngles = rot;
    }
}
