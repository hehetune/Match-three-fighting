using UnityEngine;

namespace UI
{
    public class SpriteFillController : MonoBehaviour
    {
        [Range(0, 1)]
        public float fillAmount = 0;
        private Material materialInstance;

        public void Initialize()
        {
            // Get the SpriteRenderer component
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

            // Create a new instance of the material
            materialInstance = new Material(spriteRenderer.material);

            // Assign the new material instance to the SpriteRenderer
            spriteRenderer.material = materialInstance;

            // Set the initial fill value
            SetFill(fillAmount);
        }

        public void SetFill(float value)
        {
            // Ensure the value is clamped between 0 and 1
            value = Mathf.Clamp01(value);

            // Set the fill value on the material instance
            materialInstance.SetFloat("_FillAmount", value);
        }
    }
}