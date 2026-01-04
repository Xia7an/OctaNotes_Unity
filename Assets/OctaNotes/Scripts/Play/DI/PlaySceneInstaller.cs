using OctaNotes.Scripts.Play.Interface;
using OctaNotes.Scripts.Play.Model;
using OctaNotes.Scripts.Play.View;
using UnityEngine;
using Zenject;

public class PlaySceneInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Debug.Log("[PlaySceneInstaller] InstallBindings called");

        Container.BindInterfacesAndSelfTo<ChartRepository>().AsSingle();
        Container.BindInterfacesAndSelfTo<ChartParser>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<InputController>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<TimeManager>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<JudgmentManager>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<GameController>().AsSingle().NonLazy();

        // Effect Presenters - シーン上のGameObjectから参照
        // JudgmentEffectPresenter: 判定テキスト・パーティクル表示用
        // FeedbackEffectPresenter: レーンハイライト表示用（各レーンのGameObjectにアタッチ）
        Container.Bind<IEffectPresenter>().To<JudgmentEffectPresenter>().FromComponentsInHierarchy().AsSingle();
        Container.Bind<IEffectPresenter>().To<FeedbackEffectPresenter>().FromComponentsInHierarchy().AsCached();
    }
}
