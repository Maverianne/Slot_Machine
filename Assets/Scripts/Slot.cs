using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    [SerializeField] private Transform slotMovement;
    [SerializeField] private int layoutMovementGap;
    [SerializeField] private int middleIndex = 3;
    
    private Vector3 _slotMovementStarPos;
    
    private Image[] _images;
   
    private int _destinationIndex;
    private int _symbolSpinIndex;

    private bool _listeningForNotifications; 
    private bool _isSpinning; 
    private bool _canStop;
    
    private float _spinSpeed;
    private float _spinSpeedMultiplier = 1f;
    
    private List<string> _reelStrip;
    private LinkedList<Image> _imgList;


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
       _destinationIndex = index;
       _canStop = false;
       _isSpinning = true;
       _spinSpeedMultiplier = 1;
       StartCoroutine(PerformSpin(delay));
       // Debug.Log("Winner must be: " + _reelStrip[_destinationIndex]);
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
       // Debug.Log("Middle Index is: " + _reelStrip[middleIndex]);
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
