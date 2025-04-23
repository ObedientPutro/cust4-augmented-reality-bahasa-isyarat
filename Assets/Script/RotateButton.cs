using UnityEngine;
using UnityEngine.EventSystems;

public class RotateButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    
    public enum Direction { Up, Down, Left, Right }
    public enum GameManager { Word, Alphabet, Sentence }

    [SerializeField] private Direction rotateDirection;
    [SerializeField] private GameManager gameManager;

    private Vector3 GetRotationVector() {
        return rotateDirection switch {
            Direction.Up => Vector3.left,
            Direction.Down => Vector3.right,
            Direction.Left => Vector3.down,
            Direction.Right => Vector3.up,
            _ => Vector3.zero
        };
    }

    private ModelTarget GetModelTarget() {
        return gameManager switch {
            GameManager.Word => WordManager.CurrentModelTarget,
            GameManager.Alphabet => AlphabetManager.CurrentModelTarget,
            GameManager.Sentence => SentenceManager.CurrentModelTarget,
            _ => null
        };
    }

    public void OnPointerDown(PointerEventData eventData) {
        ModelTarget target = GetModelTarget();
        if (target != null) {
            target.StartContinuousRotate(GetRotationVector());
        }
    }

    public void OnPointerUp(PointerEventData eventData) {
        ModelTarget target = GetModelTarget();
        if (target != null) {
            target.StopRotation();
        }
    }
}
