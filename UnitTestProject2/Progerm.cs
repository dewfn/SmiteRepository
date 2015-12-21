using ConsoleApplication3;
using SmiteRepository;
using SmiteRepository.Sqlserver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject2
{


    [TableName("a_testage")]
    public partial class Test : BaseEntity
    {
        private int _id;
        /// <summary>
        /// 
        ///  int(10)
        /// </summary>
        [Identity, PrimaryKey(1)]
        public int Id
        { get { return _id; } set { _id = value; OnPropertyChanged("Id"); } }
        private string _testname;
        /// <summary>
        /// 
        ///  nvarchar(50)
        /// </summary>

        public string Testname
        { get { return _testname; } set { _testname = value; OnPropertyChanged("Testname"); } }
        private string _keys;
        /// <summary>
        /// 
        ///  nvarchar(50)
        /// </summary>

        public string Keys
        { get { return _keys; } set { _keys = value; OnPropertyChanged("Keys"); } }
        private int _age;
        /// <summary>
        /// 
        ///  int(10)
        /// </summary>

        public int Age
        { get { return _age; } set { _age = value; OnPropertyChanged("Age"); } }

        public string GetTableName() {
            if (this.Testname.StartsWith("a"))
            {
                return "a_testage1";
            }
            else if (this.Testname.StartsWith("b")) {
                return "a_testage2";
            }
            return "a_testage1";
        }
    }


    class Progerm
    {
        public static void test<T>(Expression<Predicate<T>> where){
          Predicate<T> d=  where.Compile();
       
        }
        public delegate string CreateTableNameDelegate<T>(T o);

        static void Main() {
         


            string con = "Data Source=192.168.4.185;Initial Catalog=master;Persist Security Info=True;User ID=sa;Password=wulin!111111";
            Class1 c = new Class1(con);


            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("dsf", new Test());
            object ob;
            dic.TryGetValue("dsf", out ob);


            

            Test t = new Test();
            t.Testname = "afdfdf";


            string f="ddd";

            ORMRepository<Test> ot = (ORMRepository<Test>)c.For<Test>();
            ot.Test<object>(x => x.Testname == f, null);

            IORMRepository<Test> ot1 = c.For<Test>("dtestkljlkj");
            
            IORMRepository<A_testage> orm= c.For<A_testage>();


            var ss = orm.FindAll(w => w.Testname=="nihao");
            var ss2 = orm.FindAll(w => w.Id > 7000);


            A_testage y = new A_testage();

            DateTime s1 = DateTime.Now;
            for (int i = 0; i < 10; i++)
            {
                var r = c.Get();
            }
            DateTime e1 = DateTime.Now;

            DateTime s2 = DateTime.Now;
            //y.Keys = "keys8";
            //y.Sex = 9;
            //y.Yy = "润湿夺";
            //y.Id = 33;
            //string sql = "select * from a_testyy  where Id=@Id";
            for (int i = 0; i < 10; i++)
            {

                var r = orm.FindAll(w=> w.Id>7000);
            }
            DateTime e2 = DateTime.Now;

            Console.WriteLine((e1 - s1).TotalMilliseconds + "     "+(e2 - s2).TotalMilliseconds);


           
        }
    }
}
