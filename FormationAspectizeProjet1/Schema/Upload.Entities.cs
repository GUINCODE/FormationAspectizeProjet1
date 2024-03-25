
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
			public const string DocumentInfo = "DocumentInfo";
			public const string UploadedFichier = "UploadedFichier";
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

	[DataDefinition]
	public class DocumentInfo : Entity, IDataWrapper
	{
		public static partial class Fields
		{
			public const string Id = "Id";
			public const string Name = "Name";
			public const string Type = "Type";
			public const string Taille = "Taille";
			public const string Description = "Description";
			public const string DateAjout = "DateAjout";
			public const string AutreInfos = "AutreInfos";
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
		public string Type
		{
			get { return getValue<string>("Type"); }
			set { setValue<string>("Type", value); }
		}

		[Data]
		public string Taille
		{
			get { return getValue<string>("Taille"); }
			set { setValue<string>("Taille", value); }
		}

		[Data]
		public string Description
		{
			get { return getValue<string>("Description"); }
			set { setValue<string>("Description", value); }
		}

		[Data]
		public DateTime DateAjout
		{
			get { return getValue<DateTime>("DateAjout"); }
			set { setValue<DateTime>("DateAjout", value); }
		}

		[Data]
		public string AutreInfos
		{
			get { return getValue<string>("AutreInfos"); }
			set { setValue<string>("AutreInfos", value); }
		}

	}

	[DataDefinition]
	public class UploadedFichier : Entity, IDataWrapper
	{
		public static partial class Fields
		{
			public const string Id = "Id";
			public const string Stream = "Stream";
			public const string ContentLength = "ContentLength";
			public const string ContentType = "ContentType";
			public const string Name = "Name";
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
		public byte[] Stream
		{
			get { return getValue<byte[]>("Stream"); }
			set { setValue<byte[]>("Stream", value); }
		}

		[Data]
		public decimal ContentLength
		{
			get { return getValue<decimal>("ContentLength"); }
			set { setValue<decimal>("ContentLength", value); }
		}

		[Data]
		public string ContentType
		{
			get { return getValue<string>("ContentType"); }
			set { setValue<string>("ContentType", value); }
		}

		[Data]
		public string Name
		{
			get { return getValue<string>("Name"); }
			set { setValue<string>("Name", value); }
		}

	}

}


  
