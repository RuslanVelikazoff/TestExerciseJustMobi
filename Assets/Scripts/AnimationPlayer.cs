using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class AnimationPlayer : MonoBehaviour
{
    private Animator _Animator;
    private int _Delay = 10;
    private bool _IsWaiting = false;
    private bool _IsDestroying = false;

    private void Start()
    {     
        _Animator = GetComponent<Animator>();
    }

    private void OnDestroy()
    {
        _IsWaiting = false;
        _IsDestroying = true;
    }

    public async void Play(string name, CancellationToken token = default) => await PlayAwaitable(name, token);

    public async Task PlayAwaitable(string name, CancellationToken token = default)
    {
        SetAnimatorEnabled(true);
        _Animator?.Play(name);
        
        await WaitForNextFrame();
        
        if (!IsAnimationPlaying(name))
        {
            SetAnimatorEnabled(false);
            return;
        }
        
        while (!_IsDestroying && IsAnimationPlaying(name) && _Animator.enabled)
        {
            if (token.IsCancellationRequested)
                break;

            try
            {
                await Task.Delay(_Delay);
            }
            catch
            {
                break;
            }
        }
        SetAnimatorEnabled(false);
    }

    public void SetAnimatorEnabled(bool enabled)
    {
        if (_Animator == null)
            return;
        _Animator.enabled = enabled;
    }

    public void StopAllAnimations() => SetAnimatorEnabled(false);

    private bool IsAnimationPlaying(string name)
    {
        if (_Animator == null)
            return false;

        return _Animator.GetCurrentAnimatorStateInfo(0).IsName(name);
    }


    private async Task WaitForNextFrame()
    {
        StartCoroutine(WaitForNextFrameRoutine());

        while (_IsWaiting)
        {      
            await Task.Delay(_Delay);
        }
    }

    private IEnumerator WaitForNextFrameRoutine()
    {
        _IsWaiting = true;
        yield return null;
        _IsWaiting = false;
    }
}
