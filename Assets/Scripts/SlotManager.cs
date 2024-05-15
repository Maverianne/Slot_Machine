using System.Collections;
using TMPro;
using UnityEngine;

public class SlotManager : MonoBehaviour
{
   
   [SerializeField] private TMP_Text winTxt;
   [SerializeField] private float spinCoolDownTime;
   
   private Slot[] _slots;
   private bool _isSpinning;

   private const string TotalWin = "Total Win: ";

   private void Awake()
   {
      _slots = GetComponentsInChildren<Slot>();
   }

   public void SpinSlots()
   {
      if(_isSpinning) return;
      _isSpinning = true;
      
      //Reset Win Amount
      winTxt.text = TotalWin;
      
      foreach (var slot in _slots)
      {
         slot.StartSpin();
      }
   }

   private void GetSpinData()
   {
      
   }
   
   
   private void DisplayWinAmount()
   {
      StartCoroutine(SpinCoolDown());
   }

   private IEnumerator SpinCoolDown()
   {
      yield return new WaitForSeconds(spinCoolDownTime);
      _isSpinning = false; 
   }
}
