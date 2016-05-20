# SmiteRepository
####.NET ORM框架，基于Dapper效率高，操作实体类就可以实现数据库操作，很简单方便。联系 else-love@qq.com
---
1. 根据条件Update,Delete有修改字段，不会Update全字段。
2. 各种表ORM查寻：Find,FindAll,Max,Min,Count,Scalar,Exists,Sum，操作简单。
3. Find ，FindAll 可指定 输出列，更好节省资源、合理利用索引
3. 支持原始SQL语句操作
3. 支持分库，简单高效分页
4. 分表（非侵入式）。  
5. 可注册拦截所有SQL，记录日志或修改SQL
***
Dao层类，继承BaseRepository，BaseOracleRepository，BaseMysqlRepository  其中一个，根据数据库，
实现基类的构造函数传入数据库连接，在当前类中就可以调用基类中的各种数据库操作方法
```C#
public class TestRepository: BaseRepository
  {
            public TestRepository() : base("你的数据库连接") { }
             public List<viewT> Get()
            {
                List<viewT> p = base.Query<viewT>(
                "select a.age,a.keys,z.class as classname from a_testzhu z left join  a_testage a on z.keys=a.keys where a.id>7000" );
                return p;
            }
}

TestRepository testRp=new TestRepository(); 
IORMRepository<Test> ormTest= testRp.For<Test>();   //得到一个ORM操作类，
``` 

ormTest支持Find,FindAll,Max,Min,Count,Update,Delete,Exists,Sum等等常用方法，使用表达式调用 
```C#
List<Test> l= ormTest.FindAll(t=> t.Id>7000); 查寻所有Id>7000的
Test t=new Test(); 
t.Sex=2
t.Name="test";
ormTest.Update(t,w=> w.Id>7000);  //修改Id>7000的数据Sex为2，Name为test
```
Update与Insert只会持久化有赋值的字段 到数据库
```C#
 var k = ormTest.FindAll( where => where.Sex == 4,(display, F) => display(F.Name));
```
 取得Test表中所有Sex等于4的，只会查Name一个字段。
 
 
 ----------------------------------自动生成实体 data目录下
 EntityGenerate.tt， Base.ttinclude， MSSQL.ttinclude(mssql)把文件放在项目中
 修改EntityGenerate.tt里的命名空间，数据库连接，可自动生成与表一样的实体文件EntityGenerate.cs,文件内包括所有实体
 EntityGenerate.tt文件如下
```C#
<#@ template debug="true" hostSpecific="true" #>
<#@ output extension=".cs" #>
<#@ include file="Base.ttinclude" #>
<#@ include file="MSSql.ttinclude"  #>

<#
 
	ConnectionString = "Data Source=192.168.1.1;Initial Catalog=test;User ID=test;Password=Password;";
    Namespace       = "com.dewfn.Entities"; //生成实体类的命名空间
    DataContextName = "DataContext";
	BaseEntityClass = "BaseEntity";  //基类
	RenderForeignKeys = false;
	RenderBackReferences = false;
	
Usings = new List<string>()
{
	"System",           //引用的命名空间
	"SmiteRepository"	//引用的命名空间
};


    GenerateModel();

#> 
```
配置好，保存一下就会生成 EntityGenerate.cs 文件
```C#
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
```

Mysql,Oracle会有些BUG,（语法方面），用到的朋友请自己修改



>   如有什么问题 或好想法请联系 else-love@qq.com
