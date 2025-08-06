using UnityEngine;
using AK.Wwise;

public class SubmarineSoundController : MonoBehaviour
{
    public AK.Wwise.Event playUnderwater;
    public AK.Wwise.Event playSurface;
    public AK.Wwise.Event Alarm;
    public AK.Wwise.Event Submerge;
    public AK.Wwise.Event Sonar;
    public AK.Wwise.Event Flash;

    bool AlarmOn = false;

    public void SwitchState(SoundState estado)
    {
        switch (estado)
        {
            case SoundState.OnSurface:
                playUnderwater.Stop(this.gameObject);
                Sonar.Stop(this.gameObject);
                playSurface.Post(gameObject);
                break;
            case SoundState.Underwater:
                playSurface.Stop(this.gameObject);
                playUnderwater.Post(gameObject);
                Sonar.Post(gameObject);
                break;
        }
            
    }

    public void SubmergeSound()
    {
        Submerge.Post(gameObject);
    }

    public void ToggleAlarm(bool state)
    {
        if(state && state != AlarmOn)
        {
            AlarmOn = state;
            Sonar.Stop(this.gameObject);
            Alarm.Post(gameObject);
        }            
        else if (!state && state != AlarmOn)
        {
            AlarmOn = state;
            Alarm.Stop(gameObject);
        }            
    }

    public void TurnSoundOff()
    {
        playSurface.Stop(this.gameObject);
        playUnderwater.Stop(this.gameObject);
        Alarm.Stop(this.gameObject);
        Submerge.Stop(this.gameObject);
        Sonar.Stop(this.gameObject);
    }

    public void FlashSound()
    {
        Flash.Post(gameObject);
    }
}

public enum SoundState
{
    OnSurface,
    Underwater,
}
