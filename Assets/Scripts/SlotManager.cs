using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class SlotManager : MonoBehaviour
{
   
   [SerializeField] private TMP_Text winTxt;
   [SerializeField] private float spinCoolDownTime;
   [SerializeField] private float spinSlotDelay = 0.05f; 
   [SerializeField] private Slot[] _slots;
   
   private bool _isSpinning;
   private float _winAmount;

   private const string TotalWin = "TOTAL WIN: ";

   private List<Spins> _spinsList = new List<Spins>();

   private void Awake()
   {
      _slots = GetComponentsInChildren<Slot>();
   }

   private void Start()
   {
      _spinsList = FileHandler.ReadFromJson<Spins>("spins.json");
   }

   public void SpinSlots()
   {
      if(_isSpinning) return;
      _isSpinning = true;
      
      //Reset Win Amount
      winTxt.text = TotalWin;
      var spinReel = Random.Range(0, _spinsList.Count);
      
      _winAmount = _spinsList[spinReel].WinAmount;

      for (var i = 0; i < _slots.Length; i++)
      {
         _slots[i].StartSpin(_spinsList[spinReel].ReelIndex[i], spinSlotDelay);
         Debug.Log(_spinsList[spinReel].ReelIndex[i]);
      }
      DisplayWinAmount();

   }

   private void DisplayWinAmount()
   {
      winTxt.text = TotalWin + _winAmount;
      StartCoroutine(SpinCoolDown());
   }

   private IEnumerator SpinCoolDown()
   {
      yield return new WaitForSeconds(spinCoolDownTime);
      _isSpinning = false; 
   }


   [Serializable]
   private class Spins
   {
      public int[] ReelIndex;
      public int ActiveReelCount;
      public int WinAmount;
   }
}
