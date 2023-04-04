using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CourseSet")]
public class CourseSet : ScriptableObject {
    public List<CourseData> courses;

    [SerializeField]
    string setName = "Default Set Name";
    public string SetName { get { return setName; } }

    public List<CourseSet> requiredSets;
    
    public bool SetCompleted {
        get { return courses.TrueForAll(course => course._CourseComplete); }
    }

    public bool SetUnlocked {
        get {
            return requiredSets.TrueForAll(set => set.SetCompleted);
        }
    }

    public string lockedMessage = "This set is locked";
}
