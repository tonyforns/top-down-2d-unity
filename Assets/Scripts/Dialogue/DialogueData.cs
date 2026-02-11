using UnityEngine;

namespace Dialogue
{
    /// <summary>
    /// ScriptableObject que contiene una secuencia de líneas de diálogo.
    /// Crear desde el menú: Create > Dialogue > Dialogue Data
    /// </summary>
    [CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/Dialogue Data", order = 0)]
    public class DialogueData : ScriptableObject
    {
        [Tooltip("Líneas del diálogo en orden")]
        public DialogueLine[] Lines;
    }
}
