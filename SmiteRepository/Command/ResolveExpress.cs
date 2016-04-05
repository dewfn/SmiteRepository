using SmiteRepository.ORMapping;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;


namespace SmiteRepository.Command
{
    internal class ResolveExpress
    {
        public Dictionary<string, object> Argument;

        public string SqlWhere;
        private EntityMeta meta;
        string paramName;
        //public ResolveExpress(Expression expression) {
         
        //    ResolveExpression(expression);
        //}
        public ResolveExpress( Expression expression,EntityMeta meta) {
            this.meta = meta;
            ResolveExpression(expression);
        }
     //  public SqlParameter[] Paras;

        /// <summary>
        /// 解析lamdba，生成Sql查询条件
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private void ResolveExpression(Expression expression)
        {
            this.Argument = new Dictionary<string, object>();
            this.SqlWhere = Resolve(expression);
            if(this.SqlWhere=="1")
                this.SqlWhere="1=1";
            //this.Paras = Argument.Select(x => new SqlParameter(x.Key, x.Value)).ToArray();
        }

        private string Resolve(Expression expression)
        {
            if (expression == null)
                return string.Empty;

         
            if (expression is LambdaExpression)
            {
                LambdaExpression lambda = expression as LambdaExpression;
                paramName = lambda.Parameters[0].ToString()+".";
                expression = lambda.Body;
                return Resolve(expression);
            }
             if (expression is BinaryExpression)
            {
                BinaryExpression binary = expression as BinaryExpression;
               return ResolveSql(binary);
                

            }
             if (expression is UnaryExpression)
            {
                UnaryExpression unary = expression as UnaryExpression;
                if (unary.Operand is MethodCallExpression)//解析!x=>x.Name.Contains("xxx")或!array.Contains(x.Name)这类
                    return ResolveLinqToObject(unary.Operand, false);
           
                
                return Resolve(unary.Operand);
            }
             if (expression is MethodCallExpression)//x=>x.Name.Contains("xxx")或array.Contains(x.Name)这类
            {
                MethodCallExpression methodcall = expression as MethodCallExpression;
                return ResolveLinqToObject(methodcall, true);
            }
             if (expression is ConstantExpression)//x=>"abc"=="abc12"
            {
                ConstantExpression constant = expression as ConstantExpression;
              
                object temp_Value;
                GetValueOrName(expression, out temp_Value);
                    return temp_Value.ToString();
                
            }
            if (expression is MemberExpression)
            { 
                object temp_Value;
                GetValueOrName(expression, out temp_Value);
                    return temp_Value.ToString();
                
                     
            }
            
                throw new ORMException("无法解析" + expression);

         
        }

        /// <summary>
        /// 根据条件生成对应的sql查询操作符
        /// </summary>
        /// <param name="expressiontype"></param>
        /// <returns></returns>
        private string GetOperator(ExpressionType expressiontype)
        {
            switch (expressiontype)
            {
                case ExpressionType.And:
                    return "and";
                case ExpressionType.AndAlso:
                    return "and";
                case ExpressionType.Or:
                    return "or";
                case ExpressionType.OrElse:
                    return "or";
                case ExpressionType.Equal:
                    return "=";
                case ExpressionType.NotEqual:
                    return "<>";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
                case ExpressionType.GreaterThan:
                    return ">";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                case ExpressionType.Add:
                    return "+";
                case ExpressionType.Subtract:
                    return "-";
                default:
                    throw new ORMException(string.Format("不支持{0}此种运算符查找！" + expressiontype));
            }
        }


        private string ResolveSql(BinaryExpression expression)
        {
            var Operator = GetOperator(expression.NodeType);

            var Left = Resolve(expression.Left);
            var Right = Resolve(expression.Right);
            if (Operator == "and")
            {
                if (Left == "1")
                    Left = "1=1";
                else if (Left == "0")
                    Left = "1=0";

                if (Right == "1")
                    Right = "1=1";
                else if (Right == "0")
                    Right = "1=0";
            }
            if (Operator == "=" || Operator == "<>")
            {

                
                if (Left == "NULL")
                {
                    Operator = (Operator == "=") ? "IS" : " IS NOT ";
                    Left = Right;
                    Right = "NULL";
                }
                else if(Right=="NULL")
                    Operator = (Operator == "=") ? "IS" : " IS NOT ";
            }
            string Result = string.Format("({0} {1} {2})", Left, Operator, Right);
            return Result;
        }

        private string ResolveLinqToObject(Expression expression, object value, ExpressionType? expressiontype = null)
        { 

            var MethodCall = expression as MethodCallExpression;
            var MethodName = MethodCall.Method.Name;
            switch (MethodName)
            {
                case "StartsWith":
                    return Like(MethodCall, "({0}+'%')");
                case "EndsWith":
                    return Like(MethodCall, "('%'+{0})");
                case "Contains":
                    if (MethodCall.Object != null && MethodCall.Object.Type == typeof(string))
                        return Like(MethodCall, "('%'+{0}+'%')");
                    else
                        return In(MethodCall, value);
                //case "ContainsKey":
                //    if (MethodCall.Object != null && MethodCall.Object.Type == typeof(string))
                //        return Like(MethodCall, "CONCAT('%',{0},'%')");
                //    else
                //        return In(MethodCall, value);                    
                case "Count":
                    return Len(MethodCall, value, expressiontype.Value);
                case "LongCount":
                    return Len(MethodCall, value, expressiontype.Value);
                default:
                    throw new ORMException(string.Format("不支持{0}方法的查找！", MethodName));
            }

            //switch (MethodName)
            //{
            //    case "StartsWith":
            //        return Like(MethodCall, "(CAST({0} AS VARCHAR)+'%')");
            //    case "EndsWith":
            //        return Like(MethodCall, "('%'+CAST({0} AS VARCHAR))");
            //    case "Contains":
            //        if (MethodCall.Object != null && MethodCall.Object.Type == typeof(string))
            //            return Like(MethodCall, "('%'+CAST({0} AS VARCHAR)+'%')");
            //        else
            //            return In(MethodCall, value);
            //    //case "ContainsKey":
            //    //    if (MethodCall.Object != null && MethodCall.Object.Type == typeof(string))
            //    //        return Like(MethodCall, "CONCAT('%',{0},'%')");
            //    //    else
            //    //        return In(MethodCall, value);                    
            //    case "Count":
            //        return Len(MethodCall, value, expressiontype.Value);
            //    case "LongCount":
            //        return Len(MethodCall, value, expressiontype.Value);
            //    default:
            //        throw new ORMException(string.Format("不支持{0}方法的查找！", MethodName));
            //}

            //switch (MethodName)
            //{
            //    case "StartsWith":
            //        return Like(MethodCall, "CONCAT({0},'%')");
            //    case "EndsWith":
            //        return Like(MethodCall, "CONCAT('%',{0})");
            //    case "Contains":
            //        if (MethodCall.Object != null && MethodCall.Object.Type == typeof(string))
            //            return Like(MethodCall, "CONCAT('%',{0},'%')");
            //        else
            //            return In(MethodCall, value);
            //    //case "ContainsKey":
            //    //    if (MethodCall.Object != null && MethodCall.Object.Type == typeof(string))
            //    //        return Like(MethodCall, "CONCAT('%',{0},'%')");
            //    //    else
            //    //        return In(MethodCall, value);                    
            //    case "Count":
            //        return Len(MethodCall, value, expressiontype.Value);
            //    case "LongCount":
            //        return Len(MethodCall, value, expressiontype.Value);
            //    default:
            //        throw new ORMException(string.Format("不支持{0}方法的查找！", MethodName));
            //}
        }

        private string SetArgument(string name, object value)
        {
          //   name = "@param_"+name;
            string temp = "@param_" + Argument.Count;
            while (Argument.ContainsKey(temp))
            {
                int code = Guid.NewGuid().GetHashCode();
                if (code < 0)
                    code *= -1;
                temp = name + code;
            }

            Argument[temp] = value;
            return temp;
        }

        private int ResolveMethodCall(MethodCallExpression expression,out string LeftName,out string RightName)
        {
           
            Expression arguments0;
            Expression arguments1;
            if (expression.Object == null)
            {
                arguments0 = expression.Arguments[0];
                arguments1 = expression.Arguments[1];
            }
            else
            {
                arguments0 = expression.Object; ;
                arguments1 = expression.Arguments[0];
            }
            LeftName = Resolve(arguments0);
            RightName = Resolve(arguments1);
            return 0;
          
        }
        private int GetValueOrName(Expression expression,out object Value) {

            if (expression is MemberExpression)
            {
                MemberExpression nember = (expression as MemberExpression);
             
                if (nember.Member.MemberType == MemberTypes.Property)
                {
                    if(nember.ToString().StartsWith(paramName))
                    {
                        if (nember.Member.Name == "Value")
                        {
                            nember = (nember.Expression as MemberExpression);

                        }
                       
                            
                            Value = getColumnName( nember.Member.Name);
                            return 1;
                        
                    }
                   
                   
                }
              
                    LambdaExpression lambda = Expression.Lambda(expression);
                    Delegate fn = lambda.Compile();
                    ConstantExpression value = Expression.Constant(fn.DynamicInvoke(null), expression.Type);
                    Value = value.Value;
                    if (Value == null)
                    {
                        Value = "NULL";
                        return 2;
                    }
                    if (Value is bool)
                    {
                        Value = Value.Equals(true) ? "1" : "0";
                        return 2;
                    }
                    if (Value.GetType().GetInterface("IList") != null)
                    {
                        List<string> SetInPara = new List<string>();
                        int i = 1;
                        foreach (var item in (Value as IEnumerable))
                        {
                            string Name_para = string.Format("{0}_InParameter{1}", Value.GetHashCode().ToString(), i);
                            string Key = SetArgument(Name_para, item);
                            SetInPara.Add(Key);
                            i++;
                        }
                        Value = string.Join(",", SetInPara.ToArray());
                       
                    }
                    else {
                        Value= SetArgument(expression.GetHashCode().ToString(), Value);
                    }
                    return 2;
                
            }
            else if (expression is ConstantExpression)
            {
                Value = (expression as ConstantExpression).Value;

                if (Value == null)
                {
                    Value = "NULL";
                    return 2;
                }
                if (Value is bool)
                {
                    Value = Value.Equals(true) ? "1" : "0";
                    return 2;
                }
                if (Value.GetType().GetInterface("IList") != null)
                {
                    List<string> SetInPara = new List<string>();
                    int i = 1;
                    foreach (var item in (Value as IEnumerable<object>))
                    {
                        string Name_para = string.Format("{0}_InParameter{1}", Value.GetHashCode().ToString(), i);
                        string Key = SetArgument(Name_para, item);
                        SetInPara.Add(Key);
                        i++;
                    }
                    Value = string.Join(",", SetInPara.ToArray());

                }
                else
                {
                    Value = SetArgument(expression.GetHashCode().ToString(), Value);
                }
                return 2;
            }
            throw new ORMException(string.Format("表达式 {0} 解析错误，获取值不成功", expression.ToString()));
        }


        private string In(MethodCallExpression expression, object isTrue)
        {

            string Name; string Value;
            ResolveMethodCall(expression, out Name, out Value);
            if (Name.Contains("@param_"))
            {
                string tempName = Name;
                Name = Value;
                Value = tempName;
            }
            string Operator = Convert.ToBoolean(isTrue) ? "in" : " not in";
            string Result = string.Format("{0} {1} ({2})",Name, Operator, Value);
            return Result;
        }

        private string Like(MethodCallExpression expression,string likeType)
        {
            string Name;string Value;
           ResolveMethodCall(expression, out Name, out Value);
           return string.Format("{0} like {1}", Name, string.Format(likeType, Value,"'","+"));           
           
        }
       

        private string Len(MethodCallExpression expression, object value, ExpressionType expressiontype)
        {
            string Name; string Value;
            ResolveMethodCall(expression, out Name, out Value);
          
            string Operator = GetOperator(expressiontype);
            string Result = string.Format("len({0}){1}{2}", getColumnName(Name), Operator, Name);
            return Result;
          
        }
        private string getColumnName(string fileName) {
          
            if(meta==null)
                throw new ORMException("映射类没有设置");
            EntityColumnMeta column = meta.Columns.Find(x => x.FieldName == fileName);
            if(column==null)
                throw new ORMException(string.Format("映射列名[{0}]找不到", fileName));
            return column.ColumnName;

        }

    }
}
