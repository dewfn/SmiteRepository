using SmiteRepository;
using SmiteRepository.ORMapping;
using SmiteRepository.Sqlserver; 
using SmiteRepository.Page;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Data;


namespace ConsoleApplication3
{
    [DataContract]
    public class abc{

        private string test;

        [DataMember(Name = "s")]
        public string Test
        {
            get { return test; }
            set { test = value; }
        }
     
    }
    public class viewT{
        private Nullable<int> age;

public Nullable<int> Age
{
  get { return age; }
  set { age = value; }
}
        private string keys;

public string Keys
{
  get { return keys; }
  set { keys = value; }
}
private string classname;

public string Classname
{
    get { return classname; }
    set { classname = value; }
}
    }
        public class Class1 : BaseRepository
        {
            public Class1(string con) : base(con) { }

            public PagedList<viewT> Get(PageView v)
            {
                string a="";
                PagedList<viewT> p = base.PageGet<viewT>(v, "  select a.age,a.keys,z.class as classname from a_testzhu z left join  a_testage a on z.keys=a.keys where a.age=@Age",new { Age= 8995 });
                return p;
            }
            public List<viewT> Get()
            {
                List<viewT> p = base.Query<viewT>("select a.age,a.keys,z.class as classname from a_testzhu z left join  a_testage a on z.keys=a.keys where a.id>7000" );
                return p;
            }
            public List<viewT> Get2()
            {
                List<viewT> l = new List<viewT>();
                viewT v = null;
                using (IDataReader r = base.GetReader("select a.age,a.keys,z.class as classname from a_testzhu z left join  a_testage a on z.keys=a.keys where a.id>7000"))
                {
                    while (r.Read())
                    {
                        v = new viewT();
                        if(!r.IsDBNull(0))
                        v.Age = r.GetInt32(0);
                        if (!r.IsDBNull(1))
                        v.Keys = r.GetString(1);
                        if (!r.IsDBNull(2))
                        v.Classname = r.GetString(2);
                        l.Add(v);
                    }
                }
                return l;
            }
            public int Update(string sql,object parm)
            {
                return base.ExecuteCommand(sql,parm);
            }
        }
    
}
