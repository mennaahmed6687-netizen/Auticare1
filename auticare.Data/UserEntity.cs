using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using auticare.core;

namespace auticare.Data
{
    public class UserEntity : IdataChild<Childern>, IUserEntity, IUserEntity1
    {
        List<Childern> Listofchild =new List<Childern>() ;
        private Childern? child;

        public void Add(Childern table)
        {
            Listofchild.Add(table);
        }



        public void Delete(int id)
        {
            child = find(id);
            Listofchild.Remove(child);
        }

        public Childern find(int id)
        {
            return Listofchild.Where(x => x.ChildId == id).First();
        }





        public List<Childern> GetData()
        {
            return Listofchild;
        }

        public List<Childern> Search(int ChildId)
        {
            return Listofchild.Where(x => x.ChildId == ChildId).ToList();

        }

        public void Update(int id, Childern table)
        {
            child = find(id);
            child.Name = table.Name;
            child.Age = table.Age;
            child.Gender = table.Gender;
            child.Diagnosis_Level = table.Diagnosis_Level;
        }




    }
}
