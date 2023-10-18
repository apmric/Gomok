using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gomok
{
    public class Stone : MonoBehaviour
    {
        private void Awake()
        {
            this.transform.SetParent(GameManager.instance.canvous);
        }
    }
}
