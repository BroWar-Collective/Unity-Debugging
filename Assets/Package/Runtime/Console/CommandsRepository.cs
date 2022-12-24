using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BroWar.Debugging.Console
{
    using BroWar.Common.Utilities;

    /// <summary>
    /// Scriptable container for <see cref="ConsoleCommand"/>.
    /// </summary>
    [CreateAssetMenu(fileName = "Commands Repository", menuName = "BroWar/Debugging/Commands Repository")]
    public class CommandsRepository : ScriptableObject
    {
        [EditorButton(nameof(CollectAll), "Collect All")]
        public List<ConsoleCommand> commands;

        private void CollectAll()
        {
            commands = AssetUtility.CollectAllAssets<ConsoleCommand>();
            ForceDirty();
        }

        private void ForceDirty()
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                return;
            }

            EditorUtility.SetDirty(this);
#endif
        }
    }
}