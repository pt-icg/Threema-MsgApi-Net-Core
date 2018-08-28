using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IcgSoftware.Threema.CoreMsgApi.Console.Commands.Fields;

namespace IcgSoftware.Threema.CoreMsgApi.Console.Commands
{
	abstract public class Command
	{
		private readonly LinkedList<Field> fields = new LinkedList<Field>();
		private readonly string subject;
		private readonly string description;

		public Command(string subject, string description)
		{
			this.subject = subject;
			this.description = description;
		}

		protected ByteArrayField CreateByteArrayField(string key)
		{
			return this.CreateByteArrayField(key, true);
		}
		protected ByteArrayField CreateByteArrayField(string key, bool required)
		{
			ByteArrayField field = new ByteArrayField(key, required);
			this.AddField(field);
			return field;
		}

		protected APIConnector CreateConnector(string gatewayId, string secret)
		{
			//return new APIConnector(gatewayId, secret, new DefaultPublicKeyStore());
			return new APIConnector(gatewayId, secret, null);
		}

		protected FileField CreateFileField(string key)
		{
			return this.CreateFileField(key, true);
		}
		protected FileField CreateFileField(String key, bool required)
		{
			FileField field = new FileField(key, required);
			this.AddField(field);
			return field;
		}

		protected FolderField CreateFolderField(string key)
		{
			return this.CreateFolderField(key, true);
		}
		protected FolderField CreateFolderField(string key, bool required)
		{
			FolderField field = new FolderField(key, required);
			this.AddField(field);
			return field;
		}

		protected PublicKeyField CreatePublicKeyField(String key)
		{
			return this.CreatePublicKeyField(key, true);
		}
		protected PublicKeyField CreatePublicKeyField(string key, bool required)
		{
			PublicKeyField field = new PublicKeyField(key, required);
			this.AddField(field);
			return field;
		}

		protected PrivateKeyField CreatePrivateKeyField(string key)
		{
			return this.CreatePrivateKeyField(key, true);
		}
		protected PrivateKeyField CreatePrivateKeyField(string key, bool required)
		{
			PrivateKeyField field = new PrivateKeyField(key, required);
			this.AddField(field);
			return field;
		}

		protected ThreemaIDField CreateThreemaId(string key)
		{
			return this.CreateThreemaId(key, true);
		}
		protected ThreemaIDField CreateThreemaId(String key, bool required)
		{
			ThreemaIDField field = new ThreemaIDField(key, required);
			this.AddField(field);
			return field;
		}

		protected TextField CreateTextField(string key)
		{
			return this.CreateTextField(key, true);
		}
		protected TextField CreateTextField(String key, bool required)
		{
			TextField field = new TextField(key, required);
			this.AddField(field);
			return field;
		}

		public string Description { get { return this.description; } }
		public string Subject { get { return this.subject; } }
		
		public string GetUsageArguments()
		{
			StringBuilder usage = new StringBuilder();
			this.fields.ToList().ForEach(f =>
				{
					usage.Append(" ")
						.Append(f.IsRequired ? "<" : "[")
						.Append(f.Key)
						.Append(f.IsRequired ? ">" : "]");
				});
			return usage.ToString().Trim();
		}

		protected string ReadStream(TextReader reader)
		{
			StringBuilder builder = new StringBuilder();

			char[] buffer = new char[8192];
			int read;
			while ((read = reader.Read(buffer, 0, buffer.Length)) > 0)
			{
				builder.Append(buffer, 0, read);
			}

			return builder.ToString();
		}

		public void Run(string[] arguments)
		{
			int pos = 0;
			foreach (Field f in this.fields)
			{
				if(arguments.Length > pos)
				{
					f.Value = arguments[pos];
				}
				pos++;
			}

			//validate
			foreach (Field f in this.fields)
			{
				if(!f.IsValid)
				{
					return;
				}
			}

			this.Execute();
		}

		private void AddField(Field f)
		{
			if (f.IsRequired)
			{
				//add after last required
				var lastNotRequiredField = this.fields.LastOrDefault(fi => !fi.IsRequired);
				if (lastNotRequiredField != null)
				{
					this.fields.AddBefore(this.fields.Find(lastNotRequiredField), f);
				}
				else
				{
					this.fields.AddLast(f);
				}
			}
			else
			{
				this.fields.AddLast(f);
			}
		}

		protected abstract void Execute();
	}
}
