using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using auticare.core;

namespace auticare.Data
{
    public class UserEntity : IdataChild<Child>, IUserEntity, IUserEntity1
    {
        List<Child> Listofchild =new List<Child>() ;
        private Child? child;

        public void Add(Child table)
        {
            Listofchild.Add(table);
        }



        public void Delete(int id)
        {
            child = find(id);
            Listofchild.Remove(child);
        }

        public Child find(int id)
        {
            return Listofchild.Where(x => x.ChildId == id).First();
        }





        public List<Child> GetData()
        {
            return Listofchild;
        }

        public List<Child> Search(int ChildId)
        {
            return Listofchild.Where(x => x.ChildId == ChildId).ToList();

        }

        public void Update(int id, Child table)
        {
            child = find(id);
            child.Name = table.Name;
            child.Age = table.Age;
            child.Gender = table.Gender;
            child.Diagnosis_Level = table.Diagnosis_Level;
        }




    }
}
