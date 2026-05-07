using auticare.core;

namespace auticare.Data
{
    public interface IUserEntity
    {
        void Add(Childern table);
        void Delete(int id);
        Childern find(int id);
        List<Childern> GetData();
        List<Childern> Search(int ChildId);
        void Update(int id, Childern table);
    }
}