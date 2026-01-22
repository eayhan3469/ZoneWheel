using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class RewardAnimator : MonoBehaviour
{
    [SerializeField] private Image flyingRewardImage;
    [SerializeField] private RectTransform targetStashUI;

    [SerializeField] private float duration = 0.8f;
    [SerializeField] private Ease moveEase = Ease.InBack;


    private void Start()
    {
        flyingRewardImage.gameObject.SetActive(false);
    }

    public void PlayRewardAnimation(Vector3 startPosition, Sprite iconSprite, Action onAnimationComplete)
    {
        flyingRewardImage.rectTransform.DOKill();

        flyingRewardImage.rectTransform.position = startPosition;
        flyingRewardImage.sprite = iconSprite;

        flyingRewardImage.rectTransform.localScale = Vector3.zero;

        flyingRewardImage.gameObject.SetActive(true);

        Sequence seq = DOTween.Sequence();

        seq.Append(flyingRewardImage.rectTransform.DOScale(2f, 0.3f).SetEase(Ease.OutBack));
        seq.Append(flyingRewardImage.rectTransform.DOMove(targetStashUI.position, duration).SetEase(moveEase)).SetDelay(0.2f);
        seq.Insert(1.2f, flyingRewardImage.rectTransform.DOScale(0f, 0.2f));

        seq.OnComplete(() =>
        {
            flyingRewardImage.gameObject.SetActive(false);
            onAnimationComplete?.Invoke();
        });
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (targetStashUI == null)
        {
            GameObject stashObj = GameObject.Find("UI_Stash_Target_FlyingReward");

            if (stashObj)
                targetStashUI = stashObj.GetComponent<RectTransform>();

            if (targetStashUI == null)
                Debug.LogError($"[RewardAnimator] Error: RectTransform named 'UI_Stash_Target_Icon' not found in scene. Please assign the target stash UI element.");
        }

        if (flyingRewardImage == null)
        {
            var images = GetComponentsInChildren<Image>(true);

            foreach (var img in images)
            {
                if (img.name == "UI_RewardAnimation_FlyingReward")
                {
                    flyingRewardImage = img;
                    break;
                }
            }

            if (flyingRewardImage == null)
                Debug.LogError($"[RewardAnimator] Error: Image named 'UI_FlyingReward' not found in children. Please create an Image object for animation.");
        }
    }
#endif
}
