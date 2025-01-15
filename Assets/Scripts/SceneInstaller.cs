using UnityEngine;
using Zenject;

public class SceneInstaller : MonoInstaller
{
    [SerializeField] private DesktopInput _DesktopInput;
    [SerializeField] private MobileInput _MobileInput;
    [SerializeField] private NotificationText _NotificationText;

    public override void InstallBindings()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            Container.Bind<IInput>().FromComponentInNewPrefab(_MobileInput).AsSingle();
        else
            Container.Bind<IInput>().FromComponentInNewPrefab(_DesktopInput).AsSingle();
        
        string path = Application.streamingAssetsPath + "\\session.json";
        JsonSessionData jsonSessionData = JsonSessionData.Load(path);

        Container.Bind<ISessionData>().FromInstance(jsonSessionData).AsSingle();      
        Container.Bind<INotificationSender>().FromInstance(_NotificationText).AsSingle();
        Container.Bind<ILanguageProvider>().To<RussianLanguageProvider>().AsSingle().NonLazy();
    }
}
