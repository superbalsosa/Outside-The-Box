using DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities.Error;

public class ErrorManager : MonoBehaviour, IErrorManager
{
    #region VARIABLES

    [SerializeField] private bool isAllAllowed;
    [SerializeField] private List<ErrorUtility> errorCases = new List<ErrorUtility>();

    #endregion

    #region SINGLETON
    public static ErrorManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            InterfaceDependencyInjector.Instance.Register<IErrorManager>(() => this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    #region PRIVATE_METHODS
    private bool CheckErrorPermission(ErrorType errorType)
    {
        if (isAllAllowed) return true;

        bool isPermitted = default;

        if (errorCases.FirstOrDefault(x => x.errorType == errorType).isAllowed)
        {
            isPermitted = true;
        }

        return isPermitted;
    }
    #endregion

    #region PUBLIC_METHODS
    public void WriteError(ErrorType errorType, string errorMessage, string ErrorStackTrace = "")
    {
        if (CheckErrorPermission(errorType))
        {
            Debug.LogError($"Error on {errorType}: {errorMessage}.\nStackTrace: {ErrorStackTrace}.");
        }
    }
    public void WriteMessage(ErrorType errorType, string message = "Ok")
    {
        if (CheckErrorPermission(errorType))
        {
            Debug.Log($"Debug on {errorType}: {message}.");
        }
    }
    #endregion
}

public interface IErrorManager
{
    void WriteError(ErrorType errorType, string errorMessage, string ErrorStackTrace = "");
    void WriteMessage(ErrorType errorType, string message = "Ok");
}