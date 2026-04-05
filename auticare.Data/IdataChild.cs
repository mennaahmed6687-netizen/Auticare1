using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using auticare.core;

namespace auticare.Data
{
    public interface IdataChild<Table>
    {
        List<Table> GetData();    // عرض الكل
        List<Table> Search(int ChildId);         // بحث برقم ID
        void Add(Table table);          // إضافة
        void Update(int id,Table table);       // تعديل
        void Delete(int id);          // حذف
        Table find(int id);
    }
}

