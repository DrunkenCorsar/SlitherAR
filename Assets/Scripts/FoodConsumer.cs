using UnityEngine;

public class FoodConsumer : MonoBehaviour
{
    [SerializeField] private Slithering slithering;
    public GameController gameController;
    public ScoreboardController scoreboardController;
    public FoodController foodController;

    private float _size;
    private Vector3 _originalScale;
    private Vector3 _destinationScale;
    private Transform _headTransform;

    private void Start()
    {
        _headTransform = GetComponent<Transform>();
        _originalScale = _headTransform.localScale;
        _destinationScale = new Vector3(0.1f, 0.1f, 0.1f);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Food"))
        {
            if (CompareTag("AI"))
            {
                _size += 0.02f;
                _headTransform.localScale = Vector3.Lerp(_originalScale, _destinationScale, _size);
            }

            foodController.DespawnFood(other.gameObject);
            slithering.AddBodyPart(gameObject.CompareTag("AI"));
        }
        else if (gameObject.CompareTag("Player") && other.gameObject.CompareTag("AI"))
        {
            scoreboardController.GameOver(slithering.GetLength());
        }
        else if (gameObject.CompareTag("AI") && other.gameObject.CompareTag("Player"))
        {
            // Destroy(this);
        }
    }
}