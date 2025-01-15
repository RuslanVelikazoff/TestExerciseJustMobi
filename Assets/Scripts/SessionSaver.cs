using UnityEngine;
using Zenject;

public class SessionSaver : MonoBehaviour
{
    private ISessionData _SessionData;
    private string _Path => Application.streamingAssetsPath + "\\session.json";

    [Inject]
    private void Initialzie(ISessionData sessionData)
    {
        _SessionData = sessionData;
        Application.quitting += () => _SessionData.Save(_Path);
    }
}
