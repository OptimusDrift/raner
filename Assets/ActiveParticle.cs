﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveParticle : MonoBehaviour
{
    [SerializeField]
    private GameObject particle;

    private void OnDestroy()
    {
        Instantiate(particle, transform.position, Quaternion.identity);
    }
}
