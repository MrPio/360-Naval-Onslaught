namespace Interfaces
{
    public interface IDamageble
    {
        public void TakeDamage(int damage, bool EMP = false);
        
        public void Explode(bool reward = true);
    }
}