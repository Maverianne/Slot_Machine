using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

public class Slot : MonoBehaviour
{
    [SerializeField] private Transform slotMovement;
    [SerializeField] private int layoutMovementGap;
    [SerializeField] private int middleIndex = 3;
    [SerializeField] private int animLoopDuration = 5; 
    [SerializeField] private float blinkDuration = 0.3f; 
    
    private Vector3 _slotMovementStarPos;
    
    private Image[] _images;
   
    private int _destinationIndex;
    private int _symbolSpinIndex;

    private bool _listeningForNotifications; 
    private bool _isSpinning; 
    private bool _canStop;
    private bool _winnerIsVisible;
    
    private float _spinSpeed;
    private float _spinSpeedMultiplier = 1f;
    private float _startAnimTimeStamp;
    
    private List<string> _reelStrip;
    private LinkedList<Image> _imgList;

    private bool IsAnimationRunning => Time.unscaledTime < _startAnimTimeStamp + animLoopDuration;


    private void Awake()
    {
        _slotMovementStarPos = slotMovement.position;
        RegisterNotifications(true);
        _images = slotMovement.GetComponentsInChildren<Image>();
    }

    public void SetUpSlot(float spinSpeed, List<string> reelStrip)
    {
        _spinSpeed = spinSpeed;
        _reelStrip = reelStrip;

        for (var i = 0; i < _images.Length; i++)
        {
            _images[i].sprite = MainManager.Instance.SlotManager.SpriteLibrary.GetSprite(_reelStrip[i]);
        }

        _symbolSpinIndex = _images.Length;
        _imgList = new LinkedList<Image>(_images);
    }

    public void StartSpin(int index, float delay)
    {
        //TODO: check results with prints (weird cases)
       _destinationIndex = index;
       _canStop = false;
       _isSpinning = true;
       _spinSpeedMultiplier = 1;
       StartCoroutine(PerformSpin(delay));
   }

   private IEnumerator PerformSpin(float delay)
   {
       yield return new WaitForSeconds(delay);
       var slotMovementDes = new Vector3(_slotMovementStarPos.x, _slotMovementStarPos.y - layoutMovementGap, _slotMovementStarPos.z);
       while (_isSpinning)
       {
           slotMovement.position = Vector3.MoveTowards(slotMovement.position, slotMovementDes,  (_spinSpeed *_spinSpeedMultiplier) * 100 * Time.deltaTime);
           if (slotMovement.position.y <= slotMovementDes.y) AssignLastSymbol();
           
           yield return null;
       }
       
       EventListener.TriggerEvent(ConstantsManager.Notifications.StoppedSpinning);
   }
   

   private void AssignLastSymbol()
   {
       slotMovement.GetChild(slotMovement.childCount - 1).SetAsFirstSibling();
       _imgList.Last.Value.sprite = MainManager.Instance.SlotManager.SpriteLibrary.GetSprite(_reelStrip[_symbolSpinIndex]);
        PopList();

        slotMovement.position = _slotMovementStarPos;
       
       if (middleIndex == _destinationIndex && _canStop)
       {
           _isSpinning = false;
           return;
       }
       
       _symbolSpinIndex++;
       middleIndex++;
       if (_symbolSpinIndex > _reelStrip.Count - 1) _symbolSpinIndex = 0;
       if (middleIndex > _reelStrip.Count - 1) middleIndex = 0;
   }
   
   private void PopList()
   {
       var lastValue = _imgList.Last.Value;
       _imgList.RemoveLast();
       _imgList.AddFirst(lastValue);
   }

   public void ReachedMinSpinTime()
   {
       _canStop = true;
   }

   private void AccelerateSpin()
   {
       _spinSpeedMultiplier += 0.5f;
   }

   public void AnimateReel()
   {
       _startAnimTimeStamp = Time.unscaledTime;
       _winnerIsVisible = false;
       StartCoroutine(PerformFade(false, blinkDuration));
   }

   private void CheckAnimReelPhase()
   {
       if (!IsAnimationRunning)
       {
           if (!_winnerIsVisible)
           {
               //In case the loop ended but the image didn't end as visible
               StartCoroutine(PerformFade(!_winnerIsVisible, blinkDuration));
               _winnerIsVisible = !_winnerIsVisible;
               return;
           }
           EventListener.TriggerEvent(ConstantsManager.Notifications.WinAnimationFinished);
           return;
       }

       StartCoroutine(PerformFade(!_winnerIsVisible, blinkDuration));
       _winnerIsVisible = !_winnerIsVisible;
   }
   
   private IEnumerator PerformFade(bool fadeIn, float duration) 
   {
       var timer = 0f;
       var winningImage = _imgList.ElementAt(2);
       var startingColor = winningImage.color;
       var targetAlpha = fadeIn ? 1 : 0;

       while (timer < duration) {
           timer += Time.deltaTime;
           var progress = Mathf.Clamp01(timer / duration);
           progress = Easing.InOutQuad(progress);

           var targetColorAlphaOnly = startingColor;
           targetColorAlphaOnly.a = Mathf.Lerp(startingColor.a, targetAlpha, progress);
           winningImage.color = targetColorAlphaOnly;
           yield return null;
       }
       CheckAnimReelPhase();
   }
   
   private void RegisterNotifications(bool register)
   {
       if (register)
       {
           if(_listeningForNotifications) return;
           _listeningForNotifications = true;
           EventListener.StartListening(ConstantsManager.Notifications.StoppedSpinning, AccelerateSpin);
       }
       else
       {
           if(!_listeningForNotifications) return;
           _listeningForNotifications = false;
           EventListener.StopListening(ConstantsManager.Notifications.StoppedSpinning, AccelerateSpin);
       }
   }
}
