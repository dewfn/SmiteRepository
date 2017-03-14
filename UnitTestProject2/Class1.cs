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
                PagedList<viewT> p = base.PageGet<viewT>(v, "select a.age,a.keys,z.class as classname from a_testzhu z left join  a_testage a on z.keys=a.keys where a.age=@Age",new { Age= 8995 });
                return p;
            }
            public PagedList<object> TestGet(PageView v)
            {
                string a = "";
                PagedList<object> p = base.PageGet<object>(v, @"SELECT cpi.Clue_guid ,   --线索guid
       cpi.Clue_No,
        ( CASE WHEN ci.Customer_Name IS NULL
                    OR ci.Customer_Name = '' THEN '-'
               ELSE ci.Customer_Name
          END + ' / ' + CASE WHEN ci.Contact_Person IS NULL
                                  OR ci.Contact_Person = '' THEN '-'
                             ELSE ci.Contact_Person
                        END ) AS Customer_Name , --姓名
        ( CASE WHEN ci.Mobile_Phone IS NULL
                    OR ci.Mobile_Phone = '' THEN '-'
               ELSE ci.Mobile_Phone
          END + ' / ' + CASE WHEN ci.Contact_Person_Phone IS NULL
                                  OR ci.Contact_Person_Phone = '' THEN '-'
                             ELSE ci.Contact_Person_Phone
                        END ) AS Mobile_Phone , --电话
        d1.Name AS Business_Name , --咨询内容
        bd2.Name AS FirstRequireName ,  --客户需求
        d2.Name AS Customer_level , --客服客户标签
        cpid.NoflowDay ,--未跟进天数
        cpi.status ,--处理状态
        --cpi.talk_num  --沟通次数
        dd.couner --销售沟通次数
 FROM   dbo.Tpo_CluePre_Info cpi  WITH ( NOLOCK )
        JOIN ( SELECT   tcpi.Clue_guid ,
                        COUNT(1) AS couner
               FROM     dbo.Tpo_CluePre_Info_detail tcpi  WITH ( NOLOCK )
                        inner JOIN dbo.Tpo_CluePre_person tcpp  WITH ( NOLOCK ) ON tcpp.person_GUID = tcpi.Person_guid
               WHERE    tcpp.person_ID IN (
                        SELECT  tsug.User_Id
                        FROM    dbo.Tpo_Sys_User_Group tsug  WITH ( NOLOCK )
                        WHERE   tsug.group_Id IN (
                        --获取所有销售人员并排除销售9(朱红勇)
                                SELECT  Group_Id
                                FROM    dbo.Tpo_Sys_Group  WITH ( NOLOCK )
                                WHERE   parent_group_id IN (
                                        SELECT  Group_Id
                                        FROM    dbo.Tpo_Sys_Group
                                        WHERE   group_code = '00002'
                                                 ) AND group_code <> '') )
               GROUP BY tcpi.Clue_guid
             ) dd ON cpi.Clue_guid = dd.Clue_guid
        LEFT JOIN Tpo_Customer_Info AS ci WITH ( NOLOCK ) ON cpi.Clue_guid = ci.customer_guid
        LEFT JOIN Tpo_CluePre_person AS cpp2 WITH ( NOLOCK ) ON cpi.last_personguid = cpp2.person_GUID
        LEFT JOIN ( SELECT  next_date ,
							customer_level,
							follow_GUID,
                            CASE Talk_Date
                              WHEN NULL THEN ''
                              ELSE DATEDIFF(dd, Talk_Date, GETDATE())
                            END AS NoflowDay  --未跟进天数
                    FROM    dbo.Tpo_CluePre_Info_detail 
                  ) AS cpid ON cpp2.follow_GUID = cpid.follow_GUID
        LEFT JOIN Tpo_base_Dictionary AS d1 WITH ( NOLOCK ) ON cpi.business_type = d1.Dictionary_id
                                                              AND d1.type = 1
        LEFT JOIN Tpo_base_Dictionary AS d2 WITH ( NOLOCK ) ON cpi.Presell_Tag = d2.Dictionary_id
                                                              AND d2.type = 3
        LEFT JOIN dbo.ClueInfoView AS civ WITH ( NOLOCK ) ON civ.id = cpi.source
        LEFT JOIN Tpo_CluePre_person n WITH ( NOLOCK ) ON n.person_GUID = cpi.person_GUID
        LEFT JOIN Tpo_CluePre_Info_detail AS cpid2 WITH ( NOLOCK ) ON n.follow_GUID = cpid2.follow_GUID
        LEFT JOIN Tpo_base_Dictionary bd2 WITH ( NOLOCK ) ON bd2.Dictionary_id = cpi.first_require
                                                             AND bd2.type = 5
 WHERE  cpi.status IN ( 8, 10 )  --在销售
 AND n.person_ID NOT  IN ( 408, 696, 1216 )
  order by cpi.SalesTrailDate desc
    ");
                return p;
            }
            public List<viewT> Get()
            {
                List<viewT> p = base.Query<viewT>("select a.age,a.keys  from  a_testage a where a.id>7000" );
                return p;
            }
            public void Exec() { 
            
                DataParameter dbp=new DataParameter();

                IDataReader r = base.GetReader("SELECT *  FROM [dbo].[a_testage]");

                string d = "";
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
