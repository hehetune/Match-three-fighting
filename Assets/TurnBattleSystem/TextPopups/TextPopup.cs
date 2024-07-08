/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using CodeMonkey.Utils;

public enum TextPopupType
{
    Damage = 0,
    CriticalDamage = 1,
    HealHp = 2,
    RegenMana = 3,
    RegenEnergy = 4,
} 
public class TextPopup : MonoBehaviour {
    

    // Create a Damage Popup
    public static TextPopup Create(Vector3 position, string text, TextPopupType type) {
        Transform damagePopupTransform = Instantiate(GameAssets.i.pfDamagePopup, position, Quaternion.identity);

        TextPopup textPopup = damagePopupTransform.GetComponent<TextPopup>();
        textPopup.Setup(text, type);

        return textPopup;
    }

    private static int sortingOrder = 10000;

    private const float DISAPPEAR_TIMER_MAX = 1f;

    private TextMeshPro textMesh;
    private float disappearTimer;
    private Color textColor;
    private Vector3 moveVector;

    private void Awake() {
        textMesh = transform.GetComponent<TextMeshPro>();
    }

    public void Setup(string text, TextPopupType type) {
        textMesh.SetText(text);
        switch (type)
        {
            case TextPopupType.Damage:
                textMesh.fontSize = 7;
                textColor = UtilsClass.GetColorFromString("FFC500");
                moveVector = new Vector3(0, 1) * 10f;
                break;
            case TextPopupType.CriticalDamage:
                textMesh.fontSize = 9;
                textColor = UtilsClass.GetColorFromString("FF2B00");
                moveVector = new Vector3(0, 1) * 10f;
                break;
            case TextPopupType.HealHp:
                textMesh.fontSize = 7;
                textColor = UtilsClass.GetColorFromString("cf4f3e");
                moveVector = new Vector3(0, 1) * 10f;
                break;
            case TextPopupType.RegenMana:
                textMesh.fontSize = 7;
                textColor = UtilsClass.GetColorFromString("48a3d4");
                moveVector = new Vector3(0, 1) * 10f;
                break;
            case TextPopupType.RegenEnergy:
                textMesh.fontSize = 7;
                textColor = UtilsClass.GetColorFromString("69d448");
                moveVector = new Vector3(0, 1) * 10f;
                break;
            default: break;
        }
        textMesh.color = textColor;
        disappearTimer = DISAPPEAR_TIMER_MAX;

        sortingOrder++;
        textMesh.sortingOrder = sortingOrder;
    }

    private void Update() {
        transform.position += moveVector * Time.deltaTime;
        moveVector -= moveVector * 1f * Time.deltaTime;

        if (disappearTimer > DISAPPEAR_TIMER_MAX * .5f) {
            // First half of the popup lifetime
            float increaseScaleAmount = 0.1f;
            transform.localScale += Vector3.one * increaseScaleAmount * Time.deltaTime;
        } else {
            // Second half of the popup lifetime
            float decreaseScaleAmount = 0.1f;
            transform.localScale -= Vector3.one * decreaseScaleAmount * Time.deltaTime;
        }

        disappearTimer -= Time.deltaTime;
        if (disappearTimer < 0) {
            // Start disappearing
            float disappearSpeed = 3f;
            textColor.a -= disappearSpeed * Time.deltaTime;
            textMesh.color = textColor;
            if (textColor.a < 0) {
                Destroy(gameObject);
            }
        }
    }

}
