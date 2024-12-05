namespace Client.Abilities
{
    public class BaseBasicAttack : AbilityImpl
    {

        public float rateOfFire = 1;
        public string spawnedPrefabReference;
        
        public BaseBasicAttack(string identifier, float rateOfFire, string spawnedPrefabReference) : base(identifier)
        {
            this.rateOfFire = rateOfFire;
            this.spawnedPrefabReference = spawnedPrefabReference;
        }

    }
}