using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SlotManager : MonoBehaviour
{
   
   [SerializeField] private TMP_Text winTxt;
   [SerializeField] private float spinCoolDownTime;
   
   private Slot[] _slots;
   private bool _isSpinning;
   private float _winAmount;

   private int[] _currentReelIndex; 

   private const string TotalWin = "Total Win: ";

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

      var index = 0;
      
      foreach (var slot in _slots)
      {
         slot.StartSpin(index);
      }
      
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
