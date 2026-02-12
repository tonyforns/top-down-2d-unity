using UnityEngine;

namespace Dialogue
{
    /// <summary>
    /// Representa una línea de diálogo con texto, retrato del personaje y nombre opcional.
    /// </summary>
    [System.Serializable]
    public class DialogueLine
    {
        [Tooltip("Texto que se mostrará en el diálogo")]
        [TextArea(2, 5)]
        public string Text;

        [Tooltip("Imagen/retrato del personaje que habla")]
        public Sprite CharacterPortrait;

        [Tooltip("Nombre del personaje")]
        public string CharacterName;

        [Tooltip("Velocidad de escritura en caracteres por segundo (0 = mostrar todo de golpe)")]
        [Min(0f)]
        public float TypewriterSpeed = 30f;
    }
}
