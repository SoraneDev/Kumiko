using Kumiko;
using UnityEngine;


public class LogButtonAction : MonoBehaviour
{
    [SerializeField] private KumikoButton _button;
    
    
    private void OnEnable()
    {
        _button.Confirmed += ConfirmedLog;
        _button.ConfirmRejected += ConfirmRejectedLog;
    }
    
    
    private void OnDisable()
    {
        _button.Confirmed -= ConfirmedLog;
        _button.ConfirmRejected -= ConfirmRejectedLog;
    }
    
    
    private void ConfirmedLog()
    {
        Debug.Log("Confirmed");
    }
    
    
    private void ConfirmRejectedLog()
    {
        Debug.Log("ConfirmRejected");
    }
}
