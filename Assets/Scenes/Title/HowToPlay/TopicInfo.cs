using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "HowToPlay/TopicInfo")]
public class TopicInfo : ScriptableObject {
    [SerializeField]
    protected List<PageInfo> pages;
    public List<PageInfo> Pages { get { return pages; } }

    [SerializeField]
    protected string topicTitle;
    public string TopicTitle { get { return topicTitle; } }
}
