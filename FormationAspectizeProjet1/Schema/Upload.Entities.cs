
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.ComponentModel;

using Aspectize.Core;

[assembly:AspectizeDALAssemblyAttribute]

namespace Upload
{
	public static partial class SchemaNames
	{
		public static partial class Entities
		{
			public const string FileUploaded = "FileUploaded";
		}
	}

	[SchemaNamespace]
	public class DomainProvider : INamespace
	{
		public string Name { get { return GetType().Namespace; } }
		public static string DomainName { get { return new DomainProvider().Name; } }
	}


	[DataDefinition(MustPersist = false)]
	public class FileUploaded : Entity, IDataWrapper
	{
		public static partial class Fields
		{
			public const string Id = "Id";
			public const string Name = "Name";
			public const string ContentType = "ContentType";
			public const string Size = "Size";
		}

		void IDataWrapper.InitData(DataRow data, string namePrefix)
		{
			base.InitData(data, null);
		}

		[Data(IsPrimaryKey=true)]
		public Guid Id
		{
			get { return getValue<Guid>("Id"); }
			set { setValue<Guid>("Id", value); }
		}

		[Data]
		public string Name
		{
			get { return getValue<string>("Name"); }
			set { setValue<string>("Name", value); }
		}

		[Data]
		public string ContentType
		{
			get { return getValue<string>("ContentType"); }
			set { setValue<string>("ContentType", value); }
		}

		[Data]
		public int Size
		{
			get { return getValue<int>("Size"); }
			set { setValue<int>("Size", value); }
		}

	}

}


  
