using TestCases.LibOne.Folder1_1.Folder.Folder1_3;
using TestCases.LibOne.Folder1_1.Folder.Folder1_3;
using TestClass = TestCases.LibOne.Folder1_1.Folder.Folder1_3.Class1_3_1;

namespace TestCases.LibOne.Folder1_1
{
    public class CommonTestCases
    {
        public Class1_3_1 Field1;

        public TestCases.LibOne.Folder1_1.Folder.Folder1_3.Class1_3_1 Field2;

        public Folder.Folder1_3.Class1_3_1 Field3;

        public Class1_3_1 Prop1 { get; set; }

        public TestCases.LibOne.Folder1_1.Folder.Folder1_3.Class1_3_1 Prop2 { get; set; }

        public Folder.Folder1_3.Class1_3_1 Prop3 { get; set; }

        public void Method1(Class1_3_1 obj)
        {
            Class1_3_1 cls = new Class1_3_1();
            cls.Method(new Class1_3_1());

            var res = Class1_3_1.StaticField;
            Class1_3_1.StaticMethod(new Class1_3_1());
        }

        public void Method2(TestCases.LibOne.Folder1_1.Folder.Folder1_3.Class1_3_1 obj)
        {
            TestCases.LibOne.Folder1_1.Folder.Folder1_3.Class1_3_1 cls = new TestCases.LibOne.Folder1_1.Folder.Folder1_3.Class1_3_1();
            cls.Method(new TestCases.LibOne.Folder1_1.Folder.Folder1_3.Class1_3_1());

            var res = TestCases.LibOne.Folder1_1.Folder.Folder1_3.Class1_3_1.StaticField;
            TestCases.LibOne.Folder1_1.Folder.Folder1_3.Class1_3_1.StaticMethod(new TestCases.LibOne.Folder1_1.Folder.Folder1_3.Class1_3_1());
        }

        public void Method3(Folder.Folder1_3.Class1_3_1 obj)
        {
            Folder.Folder1_3.Class1_3_1 cls = new Folder.Folder1_3.Class1_3_1();
            cls.Method(new Folder.Folder1_3.Class1_3_1());

            var res = Folder.Folder1_3.Class1_3_1.StaticField;
            Folder.Folder1_3.Class1_3_1.StaticMethod(new Folder.Folder1_3.Class1_3_1());
        }
    }
}
