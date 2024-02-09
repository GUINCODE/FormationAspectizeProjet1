
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.ComponentModel;

using Aspectize.Core;

[assembly:AspectizeDALAssemblyAttribute]

namespace FormationAspectizeProjet1
{
	public static partial class SchemaNames
	{
		public static partial class Entities
		{
			public const string Customer = "Customer";
		}
	}

	[SchemaNamespace]
	public class DomainProvider : INamespace
	{
		public string Name { get { return GetType().Namespace; } }
		public static string DomainName { get { return new DomainProvider().Name; } }
	}


	[DataDefinition]
	public class Customer : Entity, IDataWrapper
	{
		public static partial class Fields
		{
			public const string Id = "Id";
			public const string lastName = "lastName";
			public const string firstName = "firstName";
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
		public string lastName
		{
			get { return getValue<string>("lastName"); }
			set { setValue<string>("lastName", value); }
		}

		[Data]
		public string firstName
		{
			get { return getValue<string>("firstName"); }
			set { setValue<string>("firstName", value); }
		}

       


    }

}


  
