using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameStudio.HunterGatherer.GodFavor.UI;
using DG.Tweening;

public class GodFavorUIParticle : MonoBehaviour
{
    [SerializeField]
    GameObject particlePrefab;
    [SerializeField]
    float duration = 2.5f;
    [SerializeField]
    float minDistanceFromTarget;

    Vector3 position;
    Vector3 destiation;

    Transform particle;
    RectTransform particleRect;
    bool UI = false;


    void Awake()
    {
        particle = Instantiate(particlePrefab, Vector3.zero, Quaternion.identity, GodFavorUI.Instance.transform).transform;

        particleRect = particle.GetComponent<RectTransform>();
        UI = particleRect;

        particle.gameObject.SetActive(false);
    }

    public void Play()
    {
        position = Camera.main.WorldToScreenPoint(transform.position);

        if (UI)
        {
            particleRect.position = position;
            particleRect.DOMove(destiation, duration).OnComplete(() => particle.gameObject.SetActive(false));
        }
        else
        {
            particle.position = position;
            particle.DOMove(destiation, duration).OnComplete(() => particle.gameObject.SetActive(false));
        }



        particle.gameObject.SetActive(true);
    }
}
