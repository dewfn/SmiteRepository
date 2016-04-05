using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;
using SmiteRepository;
using SmiteRepository.Sqlserver;
using ConsoleApplication3;
using SmiteRepository.Page;
using System.Linq.Expressions;
using System.Reflection.Emit;
using ServiceStack.Text;
using System.Data;
using SmiteRepository.Extansions;
namespace UnitTestProject2
{
    [TestClass]
    public class UnitTest1_Ext:BaseRepository
    {

        public UnitTest1_Ext()
            : base("Data Source=.;Initial Catalog=Test;Persist Security Info=True;User ID=sa;Password=123456;pooling=true;min pool size=5;max pool size=5")
        {
           
            RegisterORM.Register_CustomTableNameToDelete<A_testyy>(where => where.Id > 3,
                delegate(object[] SqlParams, A_testyy Entity)
                {
                    int Id = (int)SqlParams[0];
                    if(Id==3)
                        return new string[]{"a_testyy_two","a_testyy"};
                    else
                        return new string[] {  "a_testyy" };
                });
            RegisterORM.Register_CustomTableNameToInsert<A_testyy>(
               delegate(A_testyy Entity)
               {
                   if (Entity.Yy.StartsWith("two"))
                       return "a_testyy_two";
                   else
                       return  "a_testyy" ;
               });
            RegisterORM.Register_CustomTableNameToUpdate<A_testyy>( w => w.Yy == "two99777",
                delegate(object[] SqlParams, A_testyy Entity)
                {
                    string yy = SqlParams[0].ToString();
                    if (yy.StartsWith("two"))
                      return new string[] { "a_testyy_two" };
                    else
                         return new string[] { "a_testyy" }; 
                });
            RegisterORM.Register_CustomTableNameToSelect<A_testyy>(where => where.Yy == "two3232" && where.Sex > 1,
               delegate(object[] SqlParams, A_testyy Entity)
               {
                   string yy = SqlParams[0].ToString();
                   if (yy.StartsWith("two"))
                       return new string[] {  "a_testyy_two" };
                   else
                       return new string[] { "a_testyy_two","a_testyy" };
               });
            RegisterORM.Register_CustomTableNameToSelect<A_testyy>(where => where.Yy.EndsWith("two") && where.Sex > 0,
               delegate(object[] SqlParams, A_testyy Entity)
               {
                   string yy = SqlParams[0].ToString();
                   if (yy.StartsWith("two"))
                       return new string[] {  "a_testyy_two" };
                   else
                       return new string[] { "a_testyy_two","a_testyy" };
               });
            RegisterORM.Register_CustomTableNameToSelect<A_testyy>(where => where.Keys.Contains("keys") && where.Sex > 1,
              delegate(object[] SqlParams, A_testyy Entity)
              {
                  string yy = SqlParams[0].ToString();
                  //    return new string[] { "a_testyy_two" };
                      return new string[] { "a_testyy_two", "a_testyy" };
              });
            RegisterORM.Register_CustomTableNameToSelect<A_testyy>(where => where.Id > 3,
                delegate(object[] SqlParams, A_testyy Entity)
                {
                    int Id = (int)SqlParams[0];
                    if (Id == 1)
                        return new string[] { "a_testyy_two", "a_testyy" };
                    else
                        return new string[] { "a_testyy" };
                });
            ExecSqlHandle.RegisterExecHandle(delegate(string sql,object @params){
                return null;
            },true);

            orm = base.For<A_testyy>();
        }

        IORMRepository<A_testyy> orm;
       
      
        [TestMethod]
        public void TestORM_Find_Order()
        { 
            A_testyy y=new A_testyy();
            var k = orm.Find(where => where.Id>1, (Display, F) => Display(F.Keys, F.Class), (Order, F) => Order(F.Id.Desc(), F.Yy.Asc()));
            Assert.IsNotNull(k);
        } 
        [TestMethod]
        public void TestORM_FindAll()
        {
            List<string> lf = new List<string>();
            lf.Add("keys1");
            lf.Add("keys3");
            string[] klj = {"e","f"};
            
           // var k =orm.FindAll(x => klj.Contains(x.Classname) && "" == "");
           // var kk= orm.FindAll(x => "ffffff".Contains(x.Classname) && "" == "");
            var kkk = orm.FindAll(x => x.Keys.Contains("fdfdf"));
            Assert.IsNotNull(kkk);
        }
        [TestMethod]
        public void TestORM_Find()
        {
            A_testyy y = new A_testyy();
            var k = orm.Find(where => where.Id > 1, (Display, F) => Display(F.Keys, F.Class));
            Assert.IsNotNull(k);
        }
        [TestMethod]
        public void TestORM_Avg()
        {

            var k = orm.Avg<int>((Display, F) => Display(F.Sex), where => where.Yy == "two99777" && where.Sex > 0);
            Assert.AreEqual(k, 1);
        }
        [TestMethod]
        public void TestORM_Max()
        {

            var k = orm.Max<int?>((Display, F) => Display(F.Sex), where => where.Sex > 1);
            Assert.IsTrue(k > 1);
        }
        [TestMethod]
        public void TestORM_Min()
        {

            var k = orm.Min<int?>((Display, F) => Display(F.Sex), where => where.Sex > 1);
            Assert.IsTrue(k > 1);
        }
        [TestMethod]
        public void TestORM_Exists()
        {

            var k = orm.Exists(w => w.Sex == 4);
            Assert.IsTrue(k );
        }
        [TestMethod]
        public void TestORM_Count()
        {
            var k = orm.Count(where => where.Yy.EndsWith("lkjl") && where.Sex > 0);
            Assert.IsTrue(k==1);
        }
        [TestMethod]
        public void TestORM_Scalar()
        {
            var k = orm.Scalar<string>((display, F) => display(F.Class), where => where.Yy == "lkjl" && where.Sex > 1);
            Assert.AreEqual(k, "你好");
        }
        [TestMethod]
        public void TestORM_Sum()
        {
            var k = orm.Sum<int>((Display, F) => Display(F.Sex), where => where.Sex > 1);
            Assert.IsTrue(k > 1);
        }
        [TestMethod]
        public void TestQuery()
        {
            List<viewT> p = base.Query<viewT>("select a.age,a.keys,z.class as classname from a_testyy z left join  a_testage a on z.keys=a.keys where a.keys=@Key and z.class=@Class", new { Key = "abc", Class ="2"});
            Assert.IsNotNull(p);
            
        }
       
        [TestMethod]
        public void TestUpdate( )
        {
            A_testyy y = new A_testyy();
            
            y.Keys = "keys8";            
            y.Sex = 8;
            y.Yy = "润湿夺";
            y.Id =1;
            int r = orm.Update(y);

            Assert.AreEqual(r,1);
        }
        [TestMethod]
        public void TestInsert()
        {
            A_testyy yy = new A_testyy();
            yy.Yy = "two新的";
            yy.Keys = "keys99";
            //yy.Sex = 3;
            yy.Class = "fdk";
            long r = orm.Add(yy);
            Assert.IsTrue(r>0);
        }
        [TestMethod]
        public void TestDelete()
        {
            int id = 3;
            int r = orm.Delete(where => where.Id >id);
            Assert.IsTrue(r>-1);
        }
        [TestMethod]
        public void TestUpdate222()
        {
            A_testyy z = new A_testyy();
            z.Sex =1;
            z.Class = "testupdate1";
            var r = orm.Update(z, w => w.Yy == "two99777");
            Assert.AreEqual(r, 1);
        }

        [TestMethod]
        public void Test()
        {



            PageView v = new PageView();
            v.PageIndex = 0;
            v.PageSize = 3;
            v.SortName = "Id";
            v.SortOrder = "desc";
            A_testyy filter = new A_testyy();
            filter.Keys = "12";
            filter.Sex = 9;
            System.Linq.Expressions.Expression<Predicate<A_testyy>> where = w => 1 == 1;
            if (filter.Sex > 0)
                where = where.And<A_testyy>(w => w.Sex == filter.Sex);
            if (!string.IsNullOrEmpty(filter.Keys))
                where = where.And<A_testyy>(w => w.Keys.StartsWith(filter.Keys));

            var r = orm.GetPage(v,where); 
            Assert.IsNotNull(r);
        }

        [TestMethod]
        public void TestPageEntity()
        {
             

            PageView  v=new PageView();
            v.PageIndex=0;
            v.PageSize=3;
            v.SortName="Id";
            v.SortOrder="desc";

            var r = orm.GetPage(v,w=> w.Keys.Contains("keys")&&w.Sex>1,null);
            Assert.IsTrue(r .Total==4&&r.DataList.Count==3);
        }
        //[TestMethod]
        //public void Tes1111()
        //{

        //   var d= this.For<Tpo_Presell_Shunt>().Find(w => w.Id == 3 && w.Ischecked.Value==true);
        //    Assert.IsTrue(d==null);
        //}
        [TestMethod]
        public void TestDataList() {
            PageView v = new PageView();
            v.PageIndex = 0;
            v.PageSize = 2;
            v.SortName = "Id";
            v.SortOrder = "desc";
            var d= this.PageGet<object>(v,"select  * from a_testyy") ;

           
            var fdfd = d.ToJson();

            Assert.IsFalse(string.IsNullOrEmpty(fdfd));

        
        }
    }
}
