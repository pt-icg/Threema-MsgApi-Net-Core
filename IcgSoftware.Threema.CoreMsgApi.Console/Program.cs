using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IcgSoftware.Threema.CoreMsgApi.Console.Commands;

namespace IcgSoftware.Threema.CoreMsgApi.Console
{
    //class Program
    //{
    //    static void Main(string[] args)
    //    {
    //        System.Console.WriteLine("Hello World!");
    //        System.Console.ReadLine();
    //    }
    //}
    class Program
    {
        class Commands
        {
            protected readonly List<CommandGroup> commandGroups = new List<CommandGroup>();

            public List<CommandGroup> CommandGroups { get { return this.commandGroups; } }

            public CommandGroup Create(string description)
            {
                CommandGroup g = new CommandGroup(description);
                this.commandGroups.Add(g);
                return g;
            }

            public ArgumentCommand Find(String[] arguments)
            {
                if (arguments.Length > 0)
                {
                    foreach (CommandGroup g in this.commandGroups)
                    {
                        ArgumentCommand c = g.Find(arguments);
                        if (c != null)
                        {
                            return c;
                        }
                    }
                }
                return null;
            }
        }

        class ArgumentCommand
        {
            protected readonly string[] arguments;
            protected readonly Command command;

            public ArgumentCommand(string[] arguments, Command command)
            {
                this.arguments = arguments;
                this.command = command;
            }

            public string[] Arguments { get { return this.arguments; } }
            public Command Command { get { return this.command; } }

            public void run(string[] givenArguments)
            {
                if (givenArguments.Length < this.arguments.Length)
                {
                    throw new Exception("invalid arguments");
                }

                this.command.Run((string[])givenArguments.Skip(this.arguments.Length).ToArray());
            }
        }

        class CommandGroup
        {
            protected readonly string description;
            protected List<ArgumentCommand> argumentCommands = new List<ArgumentCommand>();

            public CommandGroup(String description)
            {
                this.description = description;
            }

            public string Description { get { return this.description; } }
            public List<ArgumentCommand> ArgumentCommands { get { return this.argumentCommands; } }

            public CommandGroup Add(Command command, params string[] arguments)
            {
                this.argumentCommands.Add(new ArgumentCommand(arguments, command));
                return this;
            }

            public ArgumentCommand Find(string[] arguments)
            {
                ArgumentCommand matchedArgumentCommand = null;
                int argMatchedSize = -1;
                this.argumentCommands.ForEach(c =>
                {
                    bool matched = true;
                    int matchedSize = 0;
                    for (int n = 0; n < c.Arguments.Length; n++)
                    {
                        if (n > arguments.Length || !c.Arguments[n].Equals(arguments[n]))
                        {
                            matched = false;
                            break;
                        }
                        else
                        {
                            matchedSize++;
                        }
                    }

                    if (matched && matchedSize > argMatchedSize)
                    {
                        matchedArgumentCommand = c;
                        argMatchedSize = matchedSize;
                    }
                });

                return matchedArgumentCommand;
            }
        }

        private static readonly Commands commands = new Commands();

        static void Main(string[] args)
        {
            commands.Create("Local operations (no network communication)")
                    .Add(new EncryptCommand(), "-e")
                    .Add(new DecryptCommand(), "-d")
                    .Add(new HashEmailCommand(), "-h", "-e")
                    .Add(new HashPhoneCommand(), "-h", "-p")
                    .Add(new GenerateKeyPairCommand(), "-g")
                    .Add(new DerivePublicKeyCommand(), "-p");

            commands.Create("Network operations")
                    .Add(new SendSimpleMessageCommand(), "-s")
                    .Add(new SendE2ETextMessageCommand(), "-S")
                    .Add(new SendE2EImageMessageCommand(), "-S", "-i")
                    .Add(new SendE2EFileMessageCommand(), "-S", "-f")
                    .Add(new IDLookupByEmailCommand(), "-l", "-e")
                    .Add(new IDLookupByPhoneNoCommand(), "-l", "-p")
                    .Add(new FetchPublicKeyCommand(), "-l", "-k")
                    .Add(new CapabilityCommand(), "-c")
                    .Add(new DecryptAndDownloadCommand(), "-D")
                    .Add(new CreditsCommand(), "-C");

            ArgumentCommand argumentCommand = commands.Find(args);
            if (argumentCommand == null)
            {
                usage(args.Length == 1 && args[0].Equals("html"));
            }
            else
            {
                argumentCommand.run(args);
            }
        }

        private static void usage(bool htmlOutput)
        {
            if (!htmlOutput)
            {
                System.Console.WriteLine("version:{0}", "1.0.0.0");

                System.Console.WriteLine("usage:\n");

                System.Console.WriteLine("General information");
                System.Console.WriteLine("-------------------\n");

                System.Console.WriteLine("Where a key needs to be specified, it can either be given directly as");
                System.Console.WriteLine("a command line parameter (in hex with a prefix indicating the type;");
                System.Console.WriteLine("not recommended on shared machines as other users may be able to see");
                System.Console.WriteLine("the arguments), or as the path to a file that it should be read from");
                System.Console.WriteLine("(file contents also in hex with the prefix).\n");
            }

            string groupDescriptionTemplate = htmlOutput ? "<h3>{0}</h3>\n" : "\n{0}\n" + new String('-', 80) + "\n\n";
            string commandTemplate = htmlOutput ? "<pre><code>MsgApi.exe {0}</code></pre>\n" : "{0}\n";

            commands.CommandGroups.ForEach(commandGroup =>
            {
                System.Console.WriteLine(groupDescriptionTemplate, commandGroup.Description);

                commandGroup.ArgumentCommands.ForEach(argumentCommand =>
                {
                    StringBuilder command = new StringBuilder();
                    for (int n = 0; n < argumentCommand.Arguments.Length; n++)
                    {
                        command.Append(argumentCommand.Arguments[n])
                                .Append(" ");
                    }
                    string argumentDescription = argumentCommand.Command.GetUsageArguments();
                    if (htmlOutput)
                    {
                        System.Console.WriteLine("<h4>{0}</h4>\n", argumentCommand.Command.Subject);
                        //argumentDescription = StringEscapeUtils.escapeHtml(argumentDescription);
                    }
                    command.Append(argumentDescription);

                    System.Console.WriteLine(commandTemplate, command.ToString().Trim());

                    string description = argumentCommand.Command.Description;
                    if (htmlOutput)
                    {
                        System.Console.WriteLine("<p>{0}</p>\n\n", description);
                    }
                    else
                    {
                        //System.Console.WriteLine("   " + WordUtils.wrap(description, 76, "\n   ", false));
                        System.Console.WriteLine("   " + description + "\n   ");
                        System.Console.WriteLine("");
                    }
                });
            });
        }
    }
}
