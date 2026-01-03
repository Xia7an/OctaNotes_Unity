using OctaNotes.Scripts.Play.Model;
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

        Container.BindInterfacesAndSelfTo<ChartRepository>().AsSingle();
        Container.BindInterfacesAndSelfTo<ChartParser>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<PlayInputLayer>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<JudgeManager>().AsSingle().NonLazy();
    }
}
