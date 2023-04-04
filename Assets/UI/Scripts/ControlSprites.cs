using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Values based on sprite index of TMP_Sprite Asset
public enum ControlSpriteIndex {
    ButtonNorth = 0,
    ButtonEast = 1,
    ButtonSouth = 2,
    ButtonWest = 3,
    ButtonStart = 6,
    DpadNorth = 13,
    DpadEast = 8,
    DpadSouth = 14,
    DpadWest = 7,
    StickLeft = 11,
    StickRight = 12,
    ShoulderLeft = 17,
    ShoulderRight = 18,
    TriggerLeft = 19,
    TriggerRight = 20,
    MouseButtonLeft = 4,
    MouseButtonMiddle = 9,
    MouseButtonRight = 5,
    Mouse = 10,
    KeyA = 15,
    KeyD = 16,
    KeyW = 26,
    KeyS = 27,
    KeyQ = 24,
    KeyE = 25,
    KeySpace = 21,
    KeyShift = 23,
    KeyEsc = 22
}

public static class ControlSprites {
    // name of asset located in Assets/TextMeshPro/Resources/Sprite Assets
    static string spriteAsset = "control_icons 1";

    public static string GetSpriteTag(ControlSpriteIndex control) {
        return string.Format("<sprite=\"{0}\" index={1:D}>", spriteAsset, control);
    }
}
