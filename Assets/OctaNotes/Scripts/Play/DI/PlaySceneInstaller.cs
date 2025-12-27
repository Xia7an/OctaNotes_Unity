using UnityEngine;
using Zenject;
using OctaNotes.Scripts.Settings;

public class PlaySceneInstaller : MonoInstaller
{
    [SerializeField] private PlaySettingsSO playSettingsSO;

    public override void InstallBindings()
    {
        if (playSettingsSO != null)
        {
            Container.BindInstance(playSettingsSO).AsSingle();
        }

        Container.BindInterfacesAndSelfTo<OctaNotes.Scripts.Play.Model.ChartRepository>().AsSingle();
        Container.BindInterfacesAndSelfTo<OctaNotes.Scripts.Play.Model.ChartParser>().AsSingle().NonLazy();
    }
}
