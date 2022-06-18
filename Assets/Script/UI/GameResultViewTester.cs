using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameResultViewTester : MonoBehaviour
{
    [SerializeField] private MegaJumper.UI.GameResultView resultView;
    [SerializeField] private MegaJumper.SettlementSetting[] settings;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            resultView.ShowWith(new List<MegaJumper.SettlementSetting>(settings), 1500, null);//
        }
    }
}
