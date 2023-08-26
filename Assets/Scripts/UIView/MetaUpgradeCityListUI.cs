using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class MetaUpgradeCityListUI : MonoBehaviour
{
    public Transform cityListTransform;
    public Transform cityListItemPrefab;

    public TextMeshProUGUI textUpgradeDescription;

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

        // home city
        Transform newPrefab = Instantiate(cityListItemPrefab, cityListTransform);
        newPrefab.GetComponent<MetaUpgradeCityUI>().SetCityData(DemoController.Instance.worldCreator.HomeIsland.PersistentCityDataList[0], DemoController.Instance.worldCreator.HomeIsland);
        newPrefab.GetComponent<MetaUpgradeCityUI>().SetUpgradeText(textUpgradeDescription);
        allCitiesList.Add(newPrefab);

        // all discovered islands first
        foreach (PersistentIslandData islandData in DemoController.Instance.worldCreator.AvailableIslands.Where(i => i.IslandDiscovered == true))
        {
            foreach (PersistentCityData cityData in islandData.PersistentCityDataList.Where(c => c.CityDiscovered == true))
            {
                newPrefab = Instantiate(cityListItemPrefab, cityListTransform);
                newPrefab.GetComponent<MetaUpgradeCityUI>().SetCityData(cityData, islandData);
                newPrefab.GetComponent<MetaUpgradeCityUI>().SetUpgradeText(textUpgradeDescription);
                allCitiesList.Add(newPrefab);
            }

            foreach (PersistentCityData cityData in islandData.PersistentCityDataList.Where(c => c.CityDiscovered == false))
            {
                newPrefab = Instantiate(cityListItemPrefab, cityListTransform);
                newPrefab.GetComponent<MetaUpgradeCityUI>().SetCityData(cityData, islandData);
                newPrefab.GetComponent<MetaUpgradeCityUI>().SetUpgradeText(textUpgradeDescription);
                allCitiesList.Add(newPrefab);
            }
        }

        // then undiscovered islands
        foreach (PersistentIslandData islandData in DemoController.Instance.worldCreator.AvailableIslands.Where(i => i.IslandDiscovered == false))
        {
            foreach (PersistentCityData cityData in islandData.PersistentCityDataList.Where(c => c.CityDiscovered == true))
            {
                newPrefab = Instantiate(cityListItemPrefab, cityListTransform);
                newPrefab.GetComponent<MetaUpgradeCityUI>().SetCityData(cityData, islandData);
                newPrefab.GetComponent<MetaUpgradeCityUI>().SetUpgradeText(textUpgradeDescription);
                allCitiesList.Add(newPrefab);
            }

            foreach (PersistentCityData cityData in islandData.PersistentCityDataList.Where(c => c.CityDiscovered == false))
            {
                newPrefab = Instantiate(cityListItemPrefab, cityListTransform);
                newPrefab.GetComponent<MetaUpgradeCityUI>().SetCityData(cityData, islandData);
                newPrefab.GetComponent<MetaUpgradeCityUI>().SetUpgradeText(textUpgradeDescription);
                allCitiesList.Add(newPrefab);
            }
        }

        // then locked islands
        foreach (IslandPrefab islandData in DemoController.Instance.worldCreator.AllExistingIslandPrefabs)
        {
            foreach (CityDataSO cityDataSo in islandData.cityDataList)
            {
                newPrefab = Instantiate(cityListItemPrefab, cityListTransform);
                newPrefab.GetComponent<MetaUpgradeCityUI>().SetCityData(cityDataSo, islandData);
                newPrefab.GetComponent<MetaUpgradeCityUI>().SetUpgradeText(textUpgradeDescription);
                allCitiesList.Add(newPrefab);
            }
        }
    }
}
