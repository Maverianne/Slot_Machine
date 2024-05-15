using System.Collections;
using UnityEngine;

public class Slot : MonoBehaviour
{
    [SerializeField] private Transform slotMovement;
    [SerializeField] private float spinSpeed = 5;
    [SerializeField] private float spinBaseTime = 1f;
    private bool _isMoving; 
    
    private Vector3 _slotMovementStarPos;
    private int _currentIndex = 1;
    private int _destinationIndex; 
    
    

    private void Awake()
    {
        _slotMovementStarPos = slotMovement.position;
    }
    

    public void StartSpin(int index, float delay)
   {
       _destinationIndex = index;
       _isMoving = true;
       StartCoroutine(Spin(delay));
   }

   private IEnumerator Spin(float delay)
   {
       yield return new WaitForSeconds(delay);
       var slotMovementDes = new Vector3(_slotMovementStarPos.x, _slotMovementStarPos.y - 400, _slotMovementStarPos.z);
       while (_isMoving)
       {
           slotMovement.position = Vector3.MoveTowards(slotMovement.position, slotMovementDes,  spinSpeed * 100 * Time.deltaTime);
           if (slotMovement.position.y <= slotMovementDes.y) ResetLastSymbol();
           
           yield return null;
       }
       _currentIndex = _destinationIndex;
   }
   

   private void ResetLastSymbol()
   {
       slotMovement.GetChild(slotMovement.childCount - 1).SetAsFirstSibling();
       slotMovement.position = _slotMovementStarPos;
       if (_currentIndex == _destinationIndex)
       {
           _isMoving = false;
           return;
       }
       _currentIndex++;
       if (_currentIndex > 27)
       {
           _currentIndex = 0; 
       }
   }
   
}
