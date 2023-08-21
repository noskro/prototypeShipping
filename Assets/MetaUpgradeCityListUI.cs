using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MetaUpgradeCityListUI : MonoBehaviour
{
    public Transform cityListTransform;
    public Transform cityListItemPrefab;

    private List<Transform> allCitiesList;

    // Start is called before the first frame update
    void Start()
    {
        allCitiesList = new List<Transform>();

        MetaUpgradeUI.OnMetaUpgradeDialogStatusChanged += (state) =>
        {
            if (state)
            {
                ShowNewCityList();
            }
        };
    }

    public void ShowNewCityList()
    {
        for (int i = allCitiesList.Count - 1;i >= 0; i--) 
        {
            Destroy(allCitiesList[i].gameObject);
        }

        allCitiesList = new List<Transform>();

        foreach (PersistentIslandData islandData in DemoController.Instance.worldCreator.AvailableIslands)
        {
            foreach (PersistentCityData cityData in islandData.PersistentCityDataList.Where(c => c.CityDiscovered == true))
            {
                Transform newPrefab = Instantiate(cityListItemPrefab, cityListTransform);
                newPrefab.GetComponent<MetaUpgradeCityUI>().SetCityData(cityData);
                allCitiesList.Add(newPrefab);
            }

            foreach (PersistentCityData cityData in islandData.PersistentCityDataList.Where(c => c.CityDiscovered == false))
            {
                Transform newPrefab = Instantiate(cityListItemPrefab, cityListTransform);
                newPrefab.GetComponent<MetaUpgradeCityUI>().SetCityData(cityData);
                allCitiesList.Add(newPrefab);
            }
        }

        foreach (IslandPrefab islandData in DemoController.Instance.worldCreator.AllExistingIslandPrefabs)
        {
            foreach (CityDataSO cityDataSo in islandData.cityDataList)
            {
                Transform newPrefab = Instantiate(cityListItemPrefab, cityListTransform);
                newPrefab.GetComponent<MetaUpgradeCityUI>().SetCityData(cityDataSo);
                allCitiesList.Add(newPrefab);
            }
        }
    }
}
