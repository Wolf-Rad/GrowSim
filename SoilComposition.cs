using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;


public class SoilComposition : MonoBehaviour
{
    

    


    //For SoilComposition
    [System.Serializable]
    public class OrganicMatterContent
    {
        public float DecomposedPlantMatterPercentage = 3f;
        public float DecomposedAnimalMatterPercentage = 2f;
    }

    [System.Serializable]
    public class MineralContent
    {
        public float SandPercentage = 40f;
        public float SiltPercentage = 40f;
        public float ClayPercentage = 20f;
    }

    [System.Serializable]
    public class WaterHoldingCapacity
    {
        public float MaxWaterHoldingCapacity = 25f;
        public float CurrentWaterBars = 15f;
    }

    [System.Serializable]
    public class Aeration
    {
        public float AerationValue = 0.75f;
    }

    [System.Serializable]
    public class MicroorganismPopulation
    {
        public int ProtozoaPopulation = 5000;
        public int BacteriaPopulation = 1000000;
        public int FungiPopulation = 25000;
    }

    [System.Serializable]
    public class SoilPH
    {
        public float SoilPHValue = 6.5f;
    }

    [System.Serializable]
    public class NutrientLevels
    {
        public float NitrogenLevel = 10f;
        public float PhosphorusLevel = 8f;
        public float PotassiumLevel = 6f;
    }

    [System.Serializable]
    public class SoilStructure
    {
        public string SoilStructureValue = "Loam";
    }

    [System.Serializable]
    public class BiologicalDiversity
    {
        public int PlantSpeciesCount = 25;
        public int AnimalSpeciesCount = 10;
        public int MicroorganismSpeciesCount = 50;
    }

    // ... Add the rest of the property categories here ...
    public OrganicMatterContent organicMatterContent;
    public MineralContent mineralContent;
    public WaterHoldingCapacity waterHoldingCapacity;
    public Aeration aeration;
    public MicroorganismPopulation microorganismPopulation;
    public SoilPH soilPH;
    public NutrientLevels nutrientLevels;
    public SoilStructure soilStructure;
    public BiologicalDiversity biologicalDiversity;


    private int soil_ID;

    // Methods ========================
    public void UpdateSoilComposition()
    {
        UpdateMaxWaterHoldingCapacity();
        UpdateAerationValue();
        UpdateSoilStructure();
    }

    public void UpdateMaxWaterHoldingCapacity()
    {
        waterHoldingCapacity.MaxWaterHoldingCapacity = mineralContent.ClayPercentage * 0.5f;
    }

    public void AddOrganicMatter(float plantMatter, float animalMatter)
    {
        organicMatterContent.DecomposedPlantMatterPercentage += plantMatter;
        organicMatterContent.DecomposedAnimalMatterPercentage += animalMatter;
    }

    public void AddMinerals(float sand, float silt, float clay)
    {
        mineralContent.SandPercentage += sand;
        mineralContent.SiltPercentage += silt;
        mineralContent.ClayPercentage += clay;
    }

    public void AddMicroorganisms(int protozoa, int bacteria, int fungi)
    {
        microorganismPopulation.ProtozoaPopulation += protozoa;
        microorganismPopulation.BacteriaPopulation += bacteria;
        microorganismPopulation.FungiPopulation += fungi;
    }

    public void UpdateAerationValue()
    {
        aeration.AerationValue = 1 - (mineralContent.ClayPercentage / 100);
    }

    public void AdjustSoilPH(float pHAdjustment)
    {
        soilPH.SoilPHValue += pHAdjustment;
    }

    public void AddNutrients(float nitrogen, float phosphorus, float potassium)
    {
        nutrientLevels.NitrogenLevel += nitrogen;
        nutrientLevels.PhosphorusLevel += phosphorus;
        nutrientLevels.PotassiumLevel += potassium;
    }

    public void UpdateSoilStructure()
    {
        if (mineralContent.ClayPercentage > 40)
            soilStructure.SoilStructureValue = "Clay";
        else if (mineralContent.SandPercentage > 40)
            soilStructure.SoilStructureValue = "Sandy";
        else
            soilStructure.SoilStructureValue = "Loam";
    }

    public void UpdateBiologicalDiversity(int plantSpeciesChange, int animalSpeciesChange, int microorganismSpeciesChange)
    {
        biologicalDiversity.PlantSpeciesCount += plantSpeciesChange;
        biologicalDiversity.AnimalSpeciesCount += animalSpeciesChange;
        biologicalDiversity.MicroorganismSpeciesCount += microorganismSpeciesChange;
    }
    // Start is called before the first frame update
    void Start()
    {
        soil_ID = Random.Range(0, 10000);
        Debug.Log("SoilComposition script is running.Its in soil id #" + soil_ID);
       
    }

    
}
