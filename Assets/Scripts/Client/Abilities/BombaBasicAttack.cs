using Client.Stats;
using Global;
using UnityEngine;
using World;

namespace Client.Abilities
{
    public class BombaBasicAttack : AbilityImpl
    {
        
        public BombaBasicAttack() : base("bomba")
        {
        }

        public override void Cast(GameObject owner, Vector3 target)
        {
            base.Cast(owner, target);
            var spawn = NetworkObjectPool.Singleton.GetNetworkObject(Registry.GetPrefab("projectile"), owner.transform.position, Quaternion.identity);
            if (spawn.TryGetComponent(out Projectile projectile))
            {
                projectile.ownerClient = owner.GetComponent<ClientStatHandler>().OwnerClientId;
                projectile.direction = (target - owner.transform.position).normalized;
                projectile.damage = Registry.GetBaseStat(Character.TestSquare, Stat.Damage);
            }
        }

        public override void Update(GameObject owner, float delta)
        {
            base.Update(owner, delta);
        }
    }
}