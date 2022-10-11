using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class ScreenShake : MonoBehaviour
{
    public static ScreenShake Instance;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple Screen Shake scripts have been found in the scene destroying self", gameObject);
            Destroy(this);
        }
    }

    [SerializeField]
    private float distanceShakeModifier = 60f;

    [HideInInspector]
    public bool IsShaking = false;

    public IEnumerator Shake(float duration, float magnitude, Vector2 sourcePosition)
    {
        IsShaking = true;
        Vector3 originalPosition = transform.localPosition;
        float elapsedTime = 0.0f;

        while (elapsedTime < duration)
        {
            float dist = 1;
            if (sourcePosition != Vector2.zero)
            {
                dist = RelateDistanceToMagnitude(magnitude, sourcePosition);
            }
            float x = Random.Range(-1f, 1f) * dist;
            float y = Random.Range(-1f, 1f) * dist;

            transform.localPosition = new Vector3(transform.localPosition.x + x, transform.localPosition.y + y, transform.localPosition.z);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;
        IsShaking = false;
    }

    private float RelateDistanceToMagnitude(float magnitude, Vector2 sourcePosition)
    {
        Vector2 ownPosition = new Vector2(transform.position.x, transform.position.z);
        Vector2 positionOffset = ownPosition - sourcePosition;
        float absoluteOffset = Mathf.Abs(positionOffset.x) + Mathf.Abs(positionOffset.y);
        magnitude = magnitude / (absoluteOffset / distanceShakeModifier);
        return magnitude;
    }
}
