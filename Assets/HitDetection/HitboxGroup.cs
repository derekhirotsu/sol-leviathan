using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HitDetection {
    [CreateAssetMenu (menuName = "HitDetection/HitboxGroup")]
    public class HitboxGroup : ScriptableObject {
        [SerializeField]
        protected string groupName = "";
        public string GroupName { get { return groupName; } }

        public static implicit operator string(HitboxGroup group) {
            return group.GroupName;
        }
    }
}
