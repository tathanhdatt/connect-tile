using System;
using Core.AudioService;
using Core.Service;
using DG.Tweening;
using Lean.Touch;
using MatchingPairAlgorithm;
using MessageEvent;
using UnityEngine;
using Random = UnityEngine.Random;

public class CellView : MonoBehaviour
{
   [SerializeField] private GameObject fxSelected;
   [SerializeField] private SpriteRenderer icon;
   [SerializeField] private BoxCollider2D tapArea;
   [SerializeField] private int x;
   [SerializeField] private int y;
   public int X { get => x; set => x = value; }
   public int Y { get => y; set => y = value; }

   public string IconName => icon.sprite.name;

   private void Awake()
   {
      this.transform.localScale = Vector3.zero;
   }

   private void OnEnable()
   {
      this.transform.DOScale(Vector3.one, Random.Range(0.4f, 0.6f))
         .SetEase(Ease.OutBack);
   }

   public void Selected()
   {
      fxSelected.SetActive(true);
      ServiceLocator.GetService<IAudioService>().PlaySfx(AudioName.Click);
      Messenger.Broadcast(EventKey.AddCell, new Point(X, Y));
   }

   public void SelectedScaleEffect()
   {
      this.transform
         .DOScale(new Vector3(0.9f, 0.9f, 0.9f), 0.2f)
         .SetEase(Ease.OutQuint).OnComplete(() =>
         {
            this.transform
               .DOScale(Vector3.one, 0.2f);
         });
   }

   public void SetIconSprite(Sprite sprite)
   {
      this.icon.sprite = sprite;
   }

   public void SetActiveFXSelected(bool active)
   {
      this.fxSelected.SetActive(active);
   }

   public bool EqualsByCoordinates(CellView other)
   {
      return this.x == other.x && this.y == other.y;
   }
}
