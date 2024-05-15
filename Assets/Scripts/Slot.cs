using System.Collections;
using UnityEngine;

public class Slot : MonoBehaviour
{
   
   public void StartSpin()
   {
       StartCoroutine(Spin());
   }

   private IEnumerator Spin()
   {
       yield return new WaitForSeconds(0);
   }
}
