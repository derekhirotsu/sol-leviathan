namespace HitDetection {
    public struct HitInfo {
        private Hitbox hitbox;
        public Hitbox Hitbox { get { return hitbox; } }

        private float damageDealt;
        public float DamageDealt { get { return damageDealt; } }

        private EfficacyBehaviour efficacyBehaviour;
        public EfficacyBehaviour EfficacyBehaviour { get { return efficacyBehaviour; } }

        private DamageSource damageSource;
        public DamageSource DamageSource { get { return damageSource; } }

        public HitInfo(Hitbox hit, float damage, EfficacyBehaviour behaviour, DamageSource source) {
            hitbox = hit;
            damageDealt = damage;
            efficacyBehaviour = behaviour;
            damageSource = source;
        }

        public override string ToString() {
            return $"Hit {hitbox.Slug}, Damage dealt: {damageDealt}, Behaviour: {efficacyBehaviour}";
        }
    }
}
