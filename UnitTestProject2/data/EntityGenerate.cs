﻿
//---------------------------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated by template for T4.
//    Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
//---------------------------------------------------------------------------------------------------
using System;

using SmiteRepository;

namespace UnitTestProject2
{

	[TableName("a_testyy")]
	public partial class A_testyy : BaseEntity
	{
		private int _Id;
		/// <summary>
		/// 
		///  int(10)
		/// </summary>
		[Identity, PrimaryKey(1)] 
		public int Id
		{ get{ return _Id; } 	set{ _Id = value ; } }
		private string _Yy;
		/// <summary>
		/// 
		///  varchar(50)
		/// </summary>

		public string Yy
		{ get{ return _Yy; } 	set{ _Yy = value ; } }
		private string _Class;
		/// <summary>
		/// 
		///  varchar(50)
		/// </summary>

		public string Class
		{ get{ return _Class; } 	set{ _Class = value ; } }
		private string _Keys;
		/// <summary>
		/// 
		///  varchar(50)
		/// </summary>

		public string Keys
		{ get{ return _Keys; } 	set{ _Keys = value ; } }
		private int? _Sex;
		/// <summary>
		/// 
		///  int(10)
		/// </summary>
		[Nullable  ] 
		public int? Sex
		{ get{ return _Sex; } 	set{ _Sex = value ; } }
	}

	[TableName("a_testyy_two")]
	public partial class A_testyy_two : BaseEntity
	{
		private int _Id;
		/// <summary>
		/// 
		///  int(10)
		/// </summary>
		[Identity, PrimaryKey(1)] 
		public int Id
		{ get{ return _Id; } 	set{ _Id = value ; } }
		private string _Yy;
		/// <summary>
		/// 
		///  varchar(50)
		/// </summary>

		public string Yy
		{ get{ return _Yy; } 	set{ _Yy = value ; } }
		private string _Class;
		/// <summary>
		/// 
		///  varchar(50)
		/// </summary>

		public string Class
		{ get{ return _Class; } 	set{ _Class = value ; } }
		private string _Keys;
		/// <summary>
		/// 
		///  varchar(50)
		/// </summary>

		public string Keys
		{ get{ return _Keys; } 	set{ _Keys = value ; } }
		private int? _Sex;
		/// <summary>
		/// 
		///  int(10)
		/// </summary>
		[Nullable  ] 
		public int? Sex
		{ get{ return _Sex; } 	set{ _Sex = value ; } }
	}
}
