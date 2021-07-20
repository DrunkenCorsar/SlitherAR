using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SettingsController : MonoBehaviour
{
    [SerializeField] private Slider sliderGameSpeed,
        sliderEnemiesCount,
        sliderFoodGenerationSpeed,
        sliderImmobilityPenalty,
        sliderSpeedBoostingPenalty,
        sliderGroathRate,
        sliderKillReward,
        sliderMapSize;

    /// <summary>
    /// Represents speed of enemies (and processes in the game ?..). 1.0 means normal speed, 2.0 - twice boosted.
    /// </summary>
    public float GameSpeed { get; private set; }

    /// <summary>
    /// Number of AI enemies when the game started.
    /// </summary>
    public int EnemiesCount { get; private set; }

    /// <summary>
    /// Number of food instanses generated every second for every player.
    /// </summary>
    public float FoodGenerationSpeed { get; private set; }

    /// <summary>
    /// Penalty for immobility. Segments per second.
    /// </summary>
    public float ImmobilityPenalty { get; private set; }

    /// <summary>
    /// Penalty in boosted mode. Segments per second.
    /// </summary>
    public float SpeedBoostingPenalty { get; private set; }

    /// <summary>
    /// How much does snake growth when eat. Segments per food.
    /// </summary>
    public float GroathRate { get; private set; }

    /// <summary>
    /// How many pieces of food drops out after emenie dies. 1.0 means the same amount enemie had in himself.
    /// </summary>
    public float KillReward { get; private set; }

    /// <summary>
    /// Value from 1.0 and greater. Represents how many times map's radius is greater than playing field. Map has the shape of circle with the same ar  
    /// </summary>
    public float MapSize { get; private set; }
    public Slider SliderGameSpeed { get => sliderGameSpeed; }
    public Slider SliderEnemiesCount { get => sliderEnemiesCount; }
    public Slider SliderFoodGenerationSpeed { get => sliderFoodGenerationSpeed; }
    public Slider SliderImmobilityPenalty { get => sliderImmobilityPenalty; }
    public Slider SliderSpeedBoostingPenalty { get => sliderSpeedBoostingPenalty; }
    public Slider SliderGroathRate { get => sliderGroathRate; }
    public Slider SliderKillReward { get => sliderKillReward; }
    public Slider SliderMapSize { get => sliderMapSize; }


    // Start is called before the first frame update
    private void Start()
    {
        // sliderGameSpeed = GameObject.Find("SliderGameSpeed").GetComponent<Slider>();
        // sliderEnemiesCount = GameObject.Find("SliderEnemiesCount").GetComponent<Slider>();
        // sliderFoodGenerationSpeed = GameObject.Find("SliderFoodGenerationSpeed").GetComponent<Slider>();
        // sliderImmobilityPenalty = GameObject.Find("SliderImmobilityPenalty").GetComponent<Slider>();
        // sliderSpeedBoostingPenalty = GameObject.Find("SliderSpeedBoostingPenalty").GetComponent<Slider>();
        // sliderGroathRate = GameObject.Find("SliderGroathRate").GetComponent<Slider>();
        // sliderKillReward = GameObject.Find("SliderKillReward").GetComponent<Slider>();
        // sliderMapSize = GameObject.Find("SliderMapSize").GetComponent<Slider>();
        LoadSaveFile();
        UpdateSettings();
    }

    public void UpdateSettings()
    {
        int sliderValue;
        float tmp, k;

        // calculating game speed
        sliderValue = (int) sliderGameSpeed.value;
        sliderValue -= 5;
        tmp = 1f;
        k = 1.3f;
        if (sliderValue >= 0)
            for (var i = 0; i < sliderValue; i++)
                tmp *= k;
        else
            for (var i = 0; i < -sliderValue; i++)
                tmp /= k;

        GameSpeed = tmp;

        // calculating food geration speed
        sliderValue = (int)sliderFoodGenerationSpeed.value;
        sliderValue -= 5;
        tmp = 0.3f; k = 1.3f;
        if (sliderValue >= 0)
        {
            for (int i = 0; i < sliderValue; i++)
            {
                tmp *= k;
            }
        }
        else
        {
            for (int i = 0; i < -sliderValue; i++)
            {
                tmp /= k;
            }
        }
        FoodGenerationSpeed = tmp;

        // calculating enemies count
        EnemiesCount = (int)sliderEnemiesCount.value;

        // calculating immobility penalty
        sliderValue = (int)sliderImmobilityPenalty.value;
        sliderValue -= 5;
        tmp = 0.5f; k = 1.5f;
        if (sliderValue >= 0)
        {
            for (int i = 0; i < sliderValue; i++)
            {
                tmp *= k;
            }
        }
        else
        {
            for (int i = 0; i < -sliderValue; i++)
            {
                tmp /= k;
            }
        }
        ImmobilityPenalty = tmp;

        // calculating speed boosting penalty
        sliderValue = (int)sliderSpeedBoostingPenalty.value;
        sliderValue -= 5;
        tmp = 0.5f; k = 1.5f;
        if (sliderValue >= 0)
        {
            for (int i = 0; i < sliderValue; i++)
            {
                tmp *= k;
            }
        }
        else
        {
            for (int i = 0; i < -sliderValue; i++)
            {
                tmp /= k;
            }
        }
        SpeedBoostingPenalty = tmp;

        // calculating groath rate
        sliderValue = (int)sliderGroathRate.value;
        sliderValue -= 5;
        tmp = 1f; k = 1.5f;
        if (sliderValue >= 0)
        {
            for (int i = 0; i < sliderValue; i++)
            {
                tmp *= k;
            }
        }
        else
        {
            for (int i = 0; i < -sliderValue; i++)
            {
                tmp /= k;
            }
        }
        GroathRate = tmp;

        // calculating kill reward
        sliderValue = (int)sliderKillReward.value;
        sliderValue -= 5;
        tmp = 0.5f; k = 1.3f;
        if (sliderValue >= 0)
        {
            for (int i = 0; i < sliderValue; i++)
            {
                tmp *= k;
            }
        }
        else
        {
            for (int i = 0; i < -sliderValue; i++)
            {
                tmp /= k;
            }
        }
        KillReward = tmp;

        // calculating map size
        sliderValue = (int)sliderMapSize.value;
        sliderValue -= 1;
        tmp = 1f; k = 1.2f;
        if (sliderValue > 0)
        {
            for (int i = 0; i < sliderValue; i++)
            {
                tmp *= k;
            }
        }
        MapSize = tmp;
        CreateSaveFile();
    }

    public void Reset()
    {
        sliderGameSpeed.normalizedValue = 0.5f;
        sliderEnemiesCount.normalizedValue = 0.5f;
        sliderFoodGenerationSpeed.normalizedValue = 0.5f;
        sliderImmobilityPenalty.normalizedValue = 0.5f;
        sliderSpeedBoostingPenalty.normalizedValue = 0.5f;
        sliderGroathRate.normalizedValue = 0.5f;
        sliderKillReward.normalizedValue = 0.5f;
        sliderMapSize.normalizedValue = 0.5f;
    }

    public bool LoadSaveFile()
    {
        string destination = Application.persistentDataPath + "/settings.dat";
        FileStream file;

        if (File.Exists(destination))
        {
            Debug.Log("Settings file exist.");
            file = File.OpenRead(destination);
        }
        else
        {
            Debug.Log("Settings file doesn't exist.");
            return false;
        }

        BinaryFormatter bf = new BinaryFormatter();
        SettingsContainer container = (SettingsContainer)bf.Deserialize(file);
        file.Close();

        sliderGameSpeed.value = container.SliderGameSpeedValue;
        sliderEnemiesCount.value = container.SliderEnemiesCountValue;
        sliderFoodGenerationSpeed.value = container.SliderFoodGenerationSpeedValue;
        sliderImmobilityPenalty.value = container.SliderImmobilityPenaltyValue;
        sliderSpeedBoostingPenalty.value = container.SliderSpeedBoostingPenaltyValue;
        sliderGroathRate.value = container.SliderGroathRateValue;
        sliderKillReward.value = container.SliderKillRewardValue;
        SliderMapSize.value = container.SliderMapSizeValue;

        return true;
    }

    public void CreateSaveFile()
    {
        string destination = Application.persistentDataPath + "/settings.dat";
        FileStream file;

        if (File.Exists(destination))
        {
            file = File.OpenWrite(destination);
        }
        else
        {
            file = File.Create(destination);
        }
        BinaryFormatter bf = new BinaryFormatter();
        SettingsContainer container = new SettingsContainer(this);
        bf.Serialize(file, container);
        file.Close();
        Debug.Log("Settings file created.");
    }
}