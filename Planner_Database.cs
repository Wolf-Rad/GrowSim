using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyBuildSystem.Features.Runtime.Buildings.Part;

public class Planner_Database : MonoBehaviour
{
    public Dictionary<string, List<BuildingPart>> categorizedParts = new Dictionary<string, List<BuildingPart>>();
    
   

    // Start is called before the first frame update
    void Start()
    {
        categorizedParts.Add("Plants", new List<BuildingPart>());
        categorizedParts.Add("Objects", new List<BuildingPart>());
        categorizedParts.Add("Structures", new List<BuildingPart>());
        categorizedParts.Add("Soil Mediums", new List<BuildingPart>());
        categorizedParts.Add("Diseases", new List<BuildingPart>());
        categorizedParts.Add("Pests", new List<BuildingPart>());
        categorizedParts.Add("Other", new List<BuildingPart>());

        CategorizeBuildingParts();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public int GetCategoryCount(string category)
    {
        if (categorizedParts.ContainsKey(category))
        {
            return categorizedParts[category].Count;
        }

        return 0;
    }

    public Dictionary<string, int> GetPlantCounts()
    {
        Dictionary<string, int> plantCounts = new Dictionary<string, int>();

        if (categorizedParts.ContainsKey("Plants"))
        {
            foreach (BuildingPart plantPart in categorizedParts["Plants"])
            {
                PlantData plantData = plantPart.GetComponent<PlantData>();

                if (plantData != null)
                {
                    if (plantCounts.ContainsKey(plantData.plantName))
                    {
                        plantCounts[plantData.plantName]++;
                    }
                    else
                    {
                        plantCounts.Add(plantData.plantName, 1);
                    }
                }
            }
        }

        return plantCounts;
    }


    public void CategorizeBuildingParts()
    {
        // Clear the lists
        foreach (var list in categorizedParts.Values)
        {
            list.Clear();
        }

        // Find and categorize all building parts
        BuildingPart[] allParts = FindObjectsOfType<BuildingPart>();
        foreach (var part in allParts)
        {
            // You'll need to provide your own logic for determining the category of each part
            string category = DetermineCategory(part);
            categorizedParts[category].Add(part);
        }
    }

    string DetermineCategory(BuildingPart part)
    {
        if (part.CompareTag("Plant")) return "Plants";
        if (part.CompareTag("Object")) return "Objects";
        if (part.CompareTag("Structure")) return "Structures";
        if (part.CompareTag("Soil Medium")) return "Soil Mediums";
        if (part.CompareTag("Disease")) return "Diseases";
        if (part.CompareTag("Pest")) return "Pests";

        return "Other";
    }

}
