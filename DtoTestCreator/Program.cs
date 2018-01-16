namespace DtoTestCreator
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    internal class Program
    {
        private const string TestFixture = "[TestFixture]";
        private const string Test = "[Test]";
        private const string PublicVoid = "public void ";
        private const string PublicSealedClass = "public sealed class ";
        private const string NUnitFramework = "using NUnit.Framework;";
        private const string Namespace = "namespace DtoUnitTests";
        private const string OpenBrace = "{";
        private const string CloseBrace = "}";
        private const string SemiColon = ";";
        private const string AreEqual = "Assert.AreEqual(";
        private const string IsTrue = "Assert.IsTrue(";
        private const string System = "using System;";

        private static void Main()
        {
            var path = Environment.CurrentDirectory + @"\Tests\";
            if (Directory.Exists(path))
                Directory.Delete(path, true);

            Directory.CreateDirectory(path);

            var asm = Assembly.LoadFile(@"C:\projects\EcoTrunk\NRG-Copy\Core\Dto\bin\Debug\NRGCopy.Core.Dto.dll");
            var classes = asm.GetTypes().OrderBy(z => z.Name).ToList();
            foreach (var a in classes)
            {
                var builder = new StringBuilder();
                builder.AppendLine(Namespace);
                builder.AppendLine(OpenBrace);
                builder.AppendLine(Tab(1) + $"using {a.Namespace}{SemiColon}");
                builder.AppendLine(Tab(1) + NUnitFramework);
                if(a.GetProperties().Any(z => z.PropertyType == typeof(DateTime) || z.PropertyType == typeof(DateTime?)))
                    builder.AppendLine(Tab(1) + System);

                builder.AppendLine();
                builder.AppendLine(Tab(1) + TestFixture);
                builder.AppendLine(Tab(1) + PublicSealedClass + $"{a.Name}UnitTests");
                builder.AppendLine(Tab(1) + OpenBrace);
                builder.AppendLine(Tab(2) + Test);

                var props = a.GetProperties().ToList();
                foreach (var prop in props)
                {
                    if (classes.Contains(prop.PropertyType))
                    {
                        builder.AppendLine(Tab(2) + PublicVoid + $"{prop.Name}_returns_set_value()");
                        builder.AppendLine(Tab(2) + OpenBrace);
                        builder.AppendLine(Tab(3) + $"var t = new {prop.PropertyType.Name}(){SemiColon}");
                        builder.AppendLine(Tab(3) + $"var dto = new {a.Name}");
                        builder.AppendLine(Tab(3) + OpenBrace);
                        builder.AppendLine(Tab(4) + $"{prop.Name} = t");
                        builder.AppendLine(Tab(3) + CloseBrace + SemiColon);
                        builder.AppendLine();
                        builder.AppendLine(Tab(3) + AreEqual + $"t, dto.{prop.Name}){SemiColon}");
                        builder.AppendLine(Tab(2) + CloseBrace);
                    }
                    else
                    {
                        builder.AppendLine(Tab(2) + PublicVoid + $"{prop.Name}_returns_set_value()");
                        builder.AppendLine(Tab(2) + OpenBrace);
                        builder.AppendLine(Tab(3) + $"var dto = new {a.Name}");
                        builder.AppendLine(Tab(3) + OpenBrace);

                        if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(long) || prop.PropertyType == typeof(decimal) || prop.PropertyType == typeof(double))
                        {
                            builder.AppendLine(Tab(4) + $"{prop.Name} = 10");
                            builder.AppendLine(Tab(3) + CloseBrace + SemiColon);
                            builder.AppendLine();
                            builder.AppendLine(Tab(3) + AreEqual + $"10, dto.{prop.Name}){SemiColon}");
                        }
                        else if (prop.PropertyType == typeof(string))
                        {
                            builder.AppendLine(Tab(4) + $"{prop.Name} = \"Test\"");
                            builder.AppendLine(Tab(3) + CloseBrace + SemiColon);
                            builder.AppendLine();
                            builder.AppendLine(Tab(3) + AreEqual + $"\"Test\", dto.{prop.Name}){SemiColon}");
                        }
                        else if (prop.PropertyType == typeof(int?))
                        {
                            builder.AppendLine(Tab(4) + $"{prop.Name} = 10");
                            builder.AppendLine(Tab(3) + CloseBrace + SemiColon);
                            builder.AppendLine();
                            builder.AppendLine(Tab(3) + AreEqual + $"10, dto.{prop.Name}){SemiColon}");
                        }
                        else if (prop.PropertyType == typeof(DateTime?))
                        {
                            builder.AppendLine(Tab(4) + $"{prop.Name} = new DateTime(2018, 1, 1)");
                            builder.AppendLine(Tab(3) + CloseBrace + SemiColon);
                            builder.AppendLine();
                            builder.AppendLine(Tab(3) + AreEqual + $"new DateTime(2018, 1, 1), dto.{prop.Name}){SemiColon}");
                        }
                        else if (prop.PropertyType == typeof(bool))
                        {
                            builder.AppendLine(Tab(4) + $"{prop.Name} = true");
                            builder.AppendLine(Tab(3) + CloseBrace + SemiColon);
                            builder.AppendLine();
                            builder.AppendLine(Tab(3) + IsTrue + $"dto.{prop.Name}){SemiColon}");
                        }

                        builder.AppendLine(Tab(2) + CloseBrace);
                    }

                    if (props.IndexOf(prop) != props.Count - 1)
                        builder.AppendLine();
                }

                builder.AppendLine(Tab(1) + CloseBrace);
                builder.AppendLine(Tab(0) + CloseBrace);

                var c = builder.ToString().Trim();
                using (var writer = new StreamWriter(path + $"{a.Name}UnitTests.cs", false))
                    writer.Write(c);
            }
        }

        private static string Tab(int interations)
        {
            return new string('\t', interations);
        }
    }
}