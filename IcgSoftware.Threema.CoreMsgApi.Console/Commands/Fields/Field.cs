using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IcgSoftware.Threema.CoreMsgApi.Console.Exceptions;

namespace IcgSoftware.Threema.CoreMsgApi.Console.Commands.Fields
{
	public class Field
	{
		private readonly string key;
		private readonly bool required;
		protected string value;

		protected Field(string key, bool required)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentException("key must not be null or empty.");
			}

			this.key = key;
			this.required = required;
		}

		public string Key { get { return this.key; } }

		public string Value
		{
			private get { return this.value; }
			set { this.value = value; }
		}

		public bool IsRequired { get {return this.required; } }

		public bool IsValid
		{
			get
			{
				if (this.IsRequired && this.value == null)
				{
					throw new RequiredCommandFieldMissingException(string.Format("required field {0}  not set", this.key));
				}

				if (!this.validate())
				{
					throw new InvalidCommandFieldValueException(string.Format("field {0} value invalid", this.key));
				}

				return true;
			}
		}

		protected bool validate()
		{
			return true;
		}
	}
}
