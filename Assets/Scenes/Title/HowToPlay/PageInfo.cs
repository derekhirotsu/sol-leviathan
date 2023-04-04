using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "HowToPlay/PageInfo")]
public class PageInfo : ScriptableObject {
    [SerializeField]
    protected string pageHeading;
    public string PageHeading { get { return pageHeading; } }

    [SerializeField]
    [Multiline]
    protected string pageKeyboardControls;
    public string PageKeyboardControls { get { return pageKeyboardControls; } }

    [SerializeField]
    [Multiline]
    protected string pageGamepadControls;
    public string PageGamepadControls { get { return pageGamepadControls; } }

    [SerializeField]
    [Multiline]
    protected string pageDescription;
    public string PageDescription { get { return pageDescription; } }
}
