using auticare.core;

namespace auticare.Data
{
    public interface IUserEntity
    {
        void Add(Child table);
        void Delete(int id);
        Child find(int id);
        List<Child> GetData();
        List<Child> Search(int ChildId);
        void Update(int id, Child table);
    }
}