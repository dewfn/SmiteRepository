# SmiteRepository
.NET ORM框架，基于Dapper效率高，包括根据条件Update,Delete有修改字段，各种单表查寻，操作简单。
支持分库，简单分页，暂不支持分表（想办法支持中，有头续的可以联系）。联系:else-love@qq.com

Dao层类，继承BaseRepository，BaseOracleRepository，BaseMysqlRepository  其中一个，根据数据库，
实现基类的构造函数传入数据库连接，在当前类中就可以调用基类中的各种数据库操作方法
public class Class1 : BaseRepository
  {
            public Class1() : base("你的数据库连接") { }
             public List<viewT> Get()
            {
                List<viewT> p = base.Query<viewT>("select a.age,a.keys,z.class as classname from a_testzhu z left join  a_testage a on z.keys=a.keys where a.id>7000" );
                return p;
            }
}
Class1 c=new Class1(); 
IORMRepository<Test> orTest = c.For<Test>();  得到一个ORM操作类，
支持Find,FindAll,Max,Min,Count,Update,Delete,Exists,Sum等等常用方法，使用表达式调用 
List<Test> l= orm.FindAll(t=> t.Id>7000);
Test t=new Test();
t.Id=2;
t.Name="test";
orm.Update(t);   Update与Insert只会持久化有赋值的字段 到数据库

 var k = orm.FindAll( where => where.Sex == 4,(display, F) => display(F.Name)); 取得Test表中所有Sex等于4的，只会查Name一个字段。
 
 
 ----------------------------------自动生成实体 data目录下
 EntityGenerate.tt， Base.ttinclude， MSSQL.ttinclude 把文件放在项目中
 修改EntityGenerate.tt里的命名空间，数据库连接，可自动生成与表一样的实体文件EntityGenerate.cs,文件内包括所有实体
 
 
 [TableName("Test")]
	public partial class Test : BaseEntity
	{
		private int _id;
		/// <summary>
		/// 
		///  int(10) 
		/// </summary>
		[Identity, PrimaryKey(1)] 
		public int Id
		{ get{ return _id; } 	set{ _id = value ;  OnPropertyChanged("Id"); } }
		private string _name;
		/// <summary>
		/// 
		///  nvarchar(50)
		/// </summary>

		public string Name
		{ get{ return _name; } 	set{ _name = value ;  OnPropertyChanged("Name"); } }
	}
