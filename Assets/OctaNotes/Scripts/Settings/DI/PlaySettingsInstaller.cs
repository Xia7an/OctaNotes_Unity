using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "PlaySettingsInstaller", menuName = "Installers/PlaySettingsInstaller")]
public class PlaySettingsInstaller : ScriptableObjectInstaller<PlaySettingsInstaller>
{
    [SerializeField] private OctaNotes.Scripts.Settings.PlaySettingsSO playSettingsSO;
    public override void InstallBindings()
    {
        Container.BindInstance(playSettingsSO).AsSingle();
    }
}
