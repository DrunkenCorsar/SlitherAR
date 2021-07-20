using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class SettingsContainer
{

    /// <summary>
    /// Represents speed of enemies (and processes in the game ?..). 1.0 means normal speed, 2.0 - twice boosted.
    /// </summary>
    public float SliderGameSpeedValue;

    /// <summary>
    /// Number of AI enemies when the game started.
    /// </summary>
    public float SliderEnemiesCountValue;

    /// <summary>
    /// Number of food instanses generated every second for every player.
    /// </summary>
    public float SliderFoodGenerationSpeedValue;

    /// <summary>
    /// Penalty for immobility. Segments per second.
    /// </summary>
    public float SliderImmobilityPenaltyValue;

    /// <summary>
    /// Penalty in boosted mode. Segments per second.
    /// </summary>
    public float SliderSpeedBoostingPenaltyValue;

    /// <summary>
    /// How much does snake growth when eat. Segments per food.
    /// </summary>
    public float SliderGroathRateValue;

    /// <summary>
    /// How many pieces of food drops out after emenie dies. 1.0 means the same amount enemie had in himself.
    /// </summary>
    public float SliderKillRewardValue;

    /// <summary>
    /// Value from 1.0 and greater. Represents how many times map's radius is greater than playing field. Map has the shape of circle with the same ar  
    /// </summary>
    public float SliderMapSizeValue;

    public SettingsContainer(SettingsController controller)
    {
        SliderGameSpeedValue = controller.SliderGameSpeed.value;
        SliderEnemiesCountValue = controller.SliderEnemiesCount.value;
        SliderFoodGenerationSpeedValue = controller.SliderFoodGenerationSpeed.value;
        SliderImmobilityPenaltyValue = controller.SliderImmobilityPenalty.value;
        SliderSpeedBoostingPenaltyValue = controller.SliderSpeedBoostingPenalty.value;
        SliderGroathRateValue = controller.SliderGroathRate.value;
        SliderKillRewardValue = controller.SliderKillReward.value;
        SliderMapSizeValue = controller.SliderMapSize.value;
    }
}
