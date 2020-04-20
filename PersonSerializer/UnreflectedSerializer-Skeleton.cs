using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace UnreflectedSerializer {

	public class RootDescriptor<T>{	
		private readonly Func<T, string> func;
		public RootDescriptor()
		{

		}
		public RootDescriptor(Func<T, string> f)
		{
			this.func = f;
		}
		public void Serialize(TextWriter writer, T instance) {
			writer.Write(func.Invoke(instance));
		}
	}

	class Address {
		public string Street { get; set; }
		public string City { get; set; }
	}

	class Country {
		public string Name { get; set; }
		public int AreaCode { get; set; }
	}

	class PhoneNumber {
		public Country Country { get; set; }
		public int Number { get; set; }
	}

	class Person {
		public string FirstName { get; set; }
		public string LastName { get; set; }	
		public Address HomeAddress { get; set; }
		public Address WorkAddress { get; set; }
		public Country CitizenOf { get; set; }
		public PhoneNumber MobilePhone { get; set; }
	}

	class Program {
		static void Main(string[] args) {
			RootDescriptor<Person> rootDesc = GetPersonDescriptor();
			
			var czechRepublic = new Country { Name = "Czech Republic", AreaCode = 420 };
			var person = new Person {
				FirstName = "Pavel",
				LastName = "Jezek",
				HomeAddress = new Address { Street = "Patkova", City = "Prague" },
				WorkAddress = new Address { Street = "Malostranske namesti", City = "Prague" },
				CitizenOf = czechRepublic,
				MobilePhone = new PhoneNumber { Country = czechRepublic, Number = 123456789 }
			};

			rootDesc.Serialize(Console.Out, person);
		}

		static RootDescriptor<Person> GetPersonDescriptor() {
			Func<string, string> contanerStartDesc = (name) => "<" + name +">\n";
			Func<string, string> contanerEndDesc = (name) => "</" + name +">\n";

			Func<string, int, string> intDescriptor = (attrName, i) =>
			{
				return "<" + attrName + ">" + i.ToString() + "</" + attrName + ">\n";
			};
			Func<string, string, string> stringDescriptor = (attrName, str) =>
			{
				return "<" + attrName + ">" + str + "</" + attrName + ">\n";
			};

			Func<string, Address, string> addressDescriptor = (attrName, addr) =>
			{
				StringBuilder sb = new StringBuilder();
				sb.Append(contanerStartDesc(attrName));
				sb.Append(stringDescriptor("Street", addr.Street));
				sb.Append(stringDescriptor("City", addr.City));
				sb.Append(contanerEndDesc(attrName));
				return sb.ToString();
			};
			Func<string, Country, string> countryDescriptor = (attrName, country) =>
			{
				StringBuilder sb = new StringBuilder();
				sb.Append(contanerStartDesc(attrName));
				sb.Append(stringDescriptor("Name", country.Name));
				sb.Append(intDescriptor("AreaCode", country.AreaCode));
				sb.Append(contanerEndDesc(attrName));
				return sb.ToString();
			};
			Func<string, PhoneNumber, string> phoneNumberDescriptor = (attrName, phoneNum) =>
			{
				StringBuilder sb = new StringBuilder();
				sb.Append(contanerStartDesc(attrName));
				sb.Append(countryDescriptor("Country", phoneNum.Country));
				sb.Append(intDescriptor("Number", phoneNum.Number));
				sb.Append(contanerEndDesc(attrName));
				return sb.ToString();
			};
			Func<Person, string> personDescriptor = (p) =>
			{
				StringBuilder sb = new StringBuilder();
				sb.Append(contanerStartDesc("Person"));
				sb.Append(stringDescriptor("FirstName", p.FirstName));
				sb.Append(stringDescriptor("LastName", p.LastName));
				sb.Append(addressDescriptor("HomeAddress", p.HomeAddress));
				sb.Append(addressDescriptor("WorkAddress", p.WorkAddress));
				sb.Append(countryDescriptor("CitizenOf", p.CitizenOf));
				sb.Append(phoneNumberDescriptor("MobilePhone", p.MobilePhone));
				sb.Append(contanerEndDesc("Person"));
				return sb.ToString();
			};

			return new RootDescriptor<Person>(personDescriptor);
		}
	}
}
