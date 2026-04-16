using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OctaNotes.Scripts.SongSelect.View
{
    public class GameConfigCard : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI configNameText;
        [SerializeField] private TextMeshProUGUI configValueText;
        [SerializeField] private Image minLimitImage;
        [SerializeField] private Image maxLimitImage;

        public void Apply(string configName, string configValue, bool isMin, bool isMax)
        {
            if (configNameText != null)
            {
                configNameText.text = configName;
            }

            if (configValueText != null)
            {
                configValueText.text = configValue;
            }

            if (minLimitImage != null)
            {
                minLimitImage.enabled = isMin;
            }

            if (maxLimitImage != null)
            {
                maxLimitImage.enabled = isMax;
            }
        }
    }
}
