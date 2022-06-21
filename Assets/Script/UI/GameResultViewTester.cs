using MegaJumper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameResultViewTester : MonoBehaviour
{
    [SerializeField] private MegaJumper.UI.GameResultView resultView;
    [SerializeField] private DataInstaller settingsContainer;

    private LocalSaveManager m_localSaveManager;

    [Inject]
    public void Constructor(LocalSaveManager localSaveManager)
    {
        m_localSaveManager = localSaveManager;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            resultView.ShowWith(new List<SettlementSetting>(settingsContainer.settlementSettingContainer.settlementSettings), 1500, null);//
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            m_localSaveManager.SaveDataInstance.AddCoin(5000);
        }
    }
}
