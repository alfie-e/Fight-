using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shark : MonoBehaviour {

    void Update()
    {
        transform.Rotate(0f, .4f, 0f, Space.Self);
    }
}

