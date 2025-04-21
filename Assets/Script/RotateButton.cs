using UnityEngine;
using UnityEngine.EventSystems;

public class RotateButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    
    public enum Direction { Up, Down, Left, Right }
    [SerializeField] private Direction rotateDirection;

    private Vector3 GetRotationVector() {
        return rotateDirection switch {
            Direction.Up => Vector3.left,
            Direction.Down => Vector3.right,
            Direction.Left => Vector3.down,
            Direction.Right => Vector3.up,
            _ => Vector3.zero
        };
    }

    public void OnPointerDown(PointerEventData eventData) {
        var target = AlphabetManager.CurrentModelTarget;
        if (target != null) {
            target.StartContinuousRotate(GetRotationVector());
        }
    }

    public void OnPointerUp(PointerEventData eventData) {
        var target = AlphabetManager.CurrentModelTarget;
        if (target != null) {
            target.StopRotation();
        }
    }
}
