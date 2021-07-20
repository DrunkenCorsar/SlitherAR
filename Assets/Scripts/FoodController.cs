using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class FoodController : MonoBehaviour
{
    [SerializeField] private SettingsController settings;

    [SerializeField] private GameObject[] foodModels;
    [SerializeField] private ARAnchorManager anchorManager;

    [SerializeField] private Text debugText;

    private List<GameObject> _foodPool;
    private List<GameObject> _spawnedFood;

    private ARPlane _detectedPlane;

    private void Start()
    {
        _foodPool = new List<GameObject>();
        _spawnedFood = new List<GameObject>();

        for (var i = 0; i < 64; i++)
        {
            var newFood = Instantiate(foodModels[Random.Range(0, foodModels.Length)], transform);
            newFood.AddComponent<FoodMotion>();
            newFood.SetActive(false);
            _foodPool.Add(newFood);
        }
    }

    public void SetSelectedPlane(ARPlane plane)
    {
        _detectedPlane = plane;
        for (var i = 0; i < 20; i++) SpawnFoodInstance();
    }

    public void StartGame()
    {
        // StartCoroutine(nameof(SpawningCoroutine));
    }

    private IEnumerator SpawningCoroutine()
    {
        while (true)
        {
            // var foodCount = settings.FoodGenerationSpeed;
            SpawnFoodInstance();
            yield return new WaitForSeconds(Random.Range(1, 3));
        }
    }

    private void SpawnFoodInstance()
    {
        var foodInstance = GetFoodFromPool();

        _spawnedFood.Add(foodInstance);

        var edgeVertex = _detectedPlane.boundary[Random.Range(0, _detectedPlane.boundary.Length)];
        var dist = Random.Range(.2f, .8f);
        var position = Vector2.Lerp(edgeVertex, _detectedPlane.centerInPlaneSpace, dist);
        var anchorPosition = _detectedPlane.transform.TransformPoint(position.x, .05f, position.y);

        var anchor = anchorManager.AttachAnchor(_detectedPlane, new Pose(anchorPosition, Quaternion.identity));
        foodInstance.transform.parent = anchor.transform;
        foodInstance.transform.position = anchorPosition;
        foodInstance.transform.localScale = new Vector3(.025f, .025f, .025f);
        foodInstance.gameObject.SetActive(true);
    }

    public void DespawnFood(GameObject food)
    {
        _spawnedFood.Remove(food);
        food.gameObject.SetActive(false);
        food.transform.parent = transform;
        SpawnFoodInstance();
    }

    public List<Vector3> GetFoodPositions()
    {
        return _spawnedFood.Select(t => t.transform.position).ToList();
    }

    private GameObject GetFoodFromPool()
    {
        foreach (var food in _foodPool.Where(food => !food.activeSelf))
            return food;

        var foodInstance = Instantiate(foodModels[Random.Range(0, foodModels.Length)], transform);
        foodInstance.SetActive(false);
        _foodPool.Add(foodInstance);

        return foodInstance;
    }
}