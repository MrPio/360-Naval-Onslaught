namespace Interfaces
{
    public interface IDamageable
    {
        public void TakeDamage(int damage,bool critical= false, bool emp = false);
        
        public void Explode(bool reward = true);
    }
}