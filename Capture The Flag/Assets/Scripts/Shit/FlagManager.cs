using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagManager : MonoBehaviour
{

    [Header("Important Variables")]
    [SerializeField] public bool flagAtBase;
    private Vector3 defaultPosition;
    private Quaternion defaultRotation;
    private Vector3 defaultScale;
    private Transform defaultTransform;

    private void Start()
    {
        defaultPosition = gameObject.transform.position;
        defaultRotation = gameObject.transform.rotation;
        defaultScale = gameObject.transform.localScale;

        flagAtBase = true;
    }

    private void Update()
    {
        if (flagAtBase == true)
        {
            resetTransforms();
        }
    }

    public void resetTransforms()
    {
        gameObject.transform.position = defaultPosition;
        gameObject.transform.rotation = defaultRotation;
        gameObject.transform.localScale = defaultScale;
    }
}
