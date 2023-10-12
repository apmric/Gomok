using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour
{
    private void Awake()
    {
        this.transform.SetParent(GameManager.instance.canvous);
    }
}
