public interface IWeapon
{
    void SetOwner(string tag);
    void Attack();
    bool CanAttack();
    int GetDamage();
}
