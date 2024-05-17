using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Task = System.Threading.Tasks.Task;

namespace Managers
{
   public class SlotManager : MonoBehaviour
   {
   
      [SerializeField] private TMP_Text winTxt;
      [SerializeField] private SpriteLibrary spriteLibrary;

      [SerializeField] private float spinCoolDownTime;
      [SerializeField] private float spinSpeed = 70f;
      [SerializeField] private float spinBaseTime = 1f;
      [SerializeField] private float spinStartIndividualDelay = 0.05f;
      [SerializeField] private float spinStopIndividualDelay = 0.3f;
   
      private bool _listeningForNotifications; 
      private bool _isSpinning;

      private int _currentSlot;
      private int _currenReelCount; 
      private float _winAmount;

      private Button _spinBtn;
      private Slot[] _slots;
      private List<Spins> _spinsList = new ();
      private List<List<string>> _reelStripList = new ();

      public SpriteLibrary SpriteLibrary => spriteLibrary;


      private void Awake()
      {
         _slots = GetComponentsInChildren<Slot>();
         _spinBtn = GetComponentInChildren<Button>();
      }

      private async void Start()
      {
         await LoadJsonData();
         RegisterNotifications(true);
         for (var i = 0; i < _slots.Length; i++)
         {
            _slots[i].SetUpSlot(spinSpeed, _reelStripList[i]);
         }
      }

      private Task LoadJsonData()
      {
         _spinsList = FileHandler.ReadFromJsonList<Spins>(ConstantsManager.Files.Spins);
         _reelStripList =  FileHandler.ReadFromJson<string>(ConstantsManager.Files.ReelStrips);
         return Task.CompletedTask;
      }

      public void SpinSlots()
      {
         if(_isSpinning) return;
         _isSpinning = true;
         _spinBtn.interactable = false;
         
         //Reset Win Amount
         winTxt.text = ConstantsManager.TextUI.TotalWin;
         
         //Get/set spin info and start spin
         var spinReel = Random.Range(0, _spinsList.Count);
         _winAmount = _spinsList[spinReel].WinAmount;
         _currenReelCount = _spinsList[spinReel].ActiveReelCount;

         for (var i = 0; i < _slots.Length; i++)
         {
            _slots[i].StartSpin(_spinsList[spinReel].ReelIndex[i], spinStartIndividualDelay);
         }
         
         
         StartCoroutine(PerformSpinStop(spinBaseTime));
      }
      
      
      private void CheckStopNextSlot()
      {
         if (_currentSlot < _slots.Length) StartCoroutine(PerformSpinStop(spinStopIndividualDelay));
         else DisplayWinAmount();
      }
      
      private void DisplayWinAmount()
      {
         winTxt.text = ConstantsManager.TextUI.TotalWin + _winAmount;
         
         // If there's animation, animate symbols. If not, go directly to cool down and reset spin
         if (_currenReelCount == 0)
         {
            CoolDownSpin();
         }
         else
         {
            for (var i = 0; i < _currenReelCount; i++)
            {
               _slots[i].AnimateReel();
            }
         }
      }

      private void CoolDownSpin()
      {
         StartCoroutine(PerformCoolDown());
      }
   
      public void RegisterNotifications(bool register)
      {
         if (register)
         {
            if(_listeningForNotifications) return;
            _listeningForNotifications = true;
            EventListener.StartListening(ConstantsManager.Notifications.StoppedSpinning, CheckStopNextSlot);
            EventListener.StartListening(ConstantsManager.Notifications.WinAnimationFinished, CoolDownSpin);
         }
         else
         {
            if(!_listeningForNotifications) return;
            _listeningForNotifications = false;
            EventListener.StopListening(ConstantsManager.Notifications.StoppedSpinning, CheckStopNextSlot);
            EventListener.StopListening(ConstantsManager.Notifications.WinAnimationFinished, CoolDownSpin);
         }
      }

      private IEnumerator PerformSpinStop(float stopDelay)
      {
         yield return new WaitForSeconds(stopDelay);
         _slots[_currentSlot].ReachedMinSpinTime();
         _currentSlot++;
      }

      private IEnumerator PerformCoolDown()
      {
         yield return new WaitForSeconds(spinCoolDownTime);
         _spinBtn.interactable = true;
         _isSpinning = false;
         _currentSlot = 0;
      }



      [Serializable]
      private class Spins
      {
         public int[] ReelIndex;
         public int ActiveReelCount;
         public int WinAmount;
      }
      
   }
}
