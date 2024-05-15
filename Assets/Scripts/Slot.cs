using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    private Image[] _images;
    private int _currentIndex;
    
   public void StartSpin(int index)
   {
       _currentIndex = index;
       StartCoroutine(Spin());
   }

   private IEnumerator Spin()
   {
       yield return new WaitForSeconds(0);
   }
}
