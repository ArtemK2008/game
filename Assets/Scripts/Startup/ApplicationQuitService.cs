using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Survivalon.Startup
{
    public sealed class ApplicationQuitService : IApplicationQuitService
    {
        private readonly Action editorQuitRequestedAction;
        private readonly Action playerQuitRequestedAction;

        public ApplicationQuitService(
            Action editorQuitRequestedAction = null,
            Action playerQuitRequestedAction = null)
        {
#if UNITY_EDITOR
            this.editorQuitRequestedAction = editorQuitRequestedAction ?? DefaultEditorQuitRequestedAction;
#else
            this.editorQuitRequestedAction = editorQuitRequestedAction;
#endif
            this.playerQuitRequestedAction = playerQuitRequestedAction ?? DefaultPlayerQuitRequestedAction;
        }

        public void RequestQuit()
        {
#if UNITY_EDITOR
            editorQuitRequestedAction();
#else
            playerQuitRequestedAction();
#endif
        }

#if UNITY_EDITOR
        private static void DefaultEditorQuitRequestedAction()
        {
            if (Application.isPlaying)
            {
                EditorApplication.isPlaying = false;
                return;
            }

            Debug.Log("Quit requested while running in the Unity Editor.");
        }
#endif

        private static void DefaultPlayerQuitRequestedAction()
        {
            Application.Quit();
        }
    }
}
