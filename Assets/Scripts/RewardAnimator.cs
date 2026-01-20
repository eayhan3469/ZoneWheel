using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UI;

public class RewardAnimator : MonoBehaviour
{
    [SerializeField] private GameObject flyingRewardPrefab;
    [SerializeField] private RectTransform targetStashUI;

    [SerializeField] private float duration = 0.8f;
    [SerializeField] private Ease moveEase = Ease.InBack;

    public void PlayRewardAnimation(Vector3 startPosition, Sprite iconSprite, Action onAnimationComplete)
    {
        GameObject flyingObj = Instantiate(flyingRewardPrefab, transform.root);

        flyingObj.transform.position = startPosition;

        Image img = flyingObj.GetComponent<Image>();
        img.sprite = iconSprite;

        Sequence seq = DOTween.Sequence();

        flyingObj.transform.localScale = Vector3.zero;
        seq.Append(flyingObj.transform.DOScale(2f, 0.3f).SetEase(Ease.OutBack));
        seq.Append(flyingObj.transform.DOMove(targetStashUI.position, duration).SetEase(moveEase)).SetDelay(0.2f);
        seq.Insert(1.2f, flyingObj.transform.DOScale(0f, 0.2f));

        seq.OnComplete(() =>
        {
            Destroy(flyingObj);
            onAnimationComplete?.Invoke();
        });
    }
}
