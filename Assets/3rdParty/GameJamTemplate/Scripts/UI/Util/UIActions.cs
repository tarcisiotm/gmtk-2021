using UnityEngine;

namespace TG.GameJamTemplate
{
    /// <summary>
    /// A shortcut component for adding functionality to UI buttons
    /// </summary>
    public class UIActions : MonoBehaviour
    {
        public void QuitApplication()
        {
            Application.Quit();
        }
    }
}