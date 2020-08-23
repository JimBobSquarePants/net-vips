namespace NetVips.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;
    using Xunit.Abstractions;

    public class IoFuncsTests : IClassFixture<TestsFixture>
    {
        public IoFuncsTests(TestsFixture testsFixture, ITestOutputHelper output)
        {
            testsFixture.SetUpLogging(output);
        }

        /// <summary>
        /// test the vips7 filename splitter ... this is very fragile and annoying
        /// code with lots of cases
        /// </summary>
        [SkippableFact]
        public void TestSplit7()
        {
            Action act = () => NetVips.PathFilename7("");
            var ex = Record.Exception(act);

            Skip.IfNot(ex == null, "vips configured with --disable-deprecated, skipping test");

            string[] Split(string path)
            {
                var filename7 = NetVips.PathFilename7(path);
                var mode7 = NetVips.PathMode7(path);
                return new[] { filename7, mode7 };
            }

            var cases = new Dictionary<string, string[]>
            {
                {
                    "c:\\silly:dir:name\\fr:ed.tif:jpeg:95,,,,c:\\icc\\srgb.icc",
                    new[]
                    {
                        "c:\\silly:dir:name\\fr:ed.tif",
                        "jpeg:95,,,,c:\\icc\\srgb.icc"
                    }
                },
                {
                    "I180:",
                    new[]
                    {
                        "I180",
                        ""
                    }
                },
                {
                    "c:\\silly:",
                    new[]
                    {
                        "c:\\silly",
                        ""
                    }
                },
                {
                    "c:\\program files\\x:hello",
                    new[]
                    {
                        "c:\\program files\\x",
                        "hello"
                    }
                },
                {
                    "C:\\fixtures\\2569067123_aca715a2ee_o.jpg",
                    new[]
                    {
                        "C:\\fixtures\\2569067123_aca715a2ee_o.jpg",
                        ""
                    }
                }
            };

            foreach (var entry in cases)
            {
                Assert.Equal(entry.Value, Split(entry.Key));
            }
        }

        [Fact]
        public void TestNewFromImage()
        {
            var im = Image.MaskIdeal(100, 100, 0.5, reject: true, optical: true);

            var im2 = im.NewFromImage(12);

            Assert.Equal(im.Width, im2.Width);
            Assert.Equal(im.Height, im2.Height);
            Assert.Equal(im.Interpretation, im2.Interpretation);
            Assert.Equal(im.Format, im2.Format);
            Assert.Equal(im.Xres, im2.Xres);
            Assert.Equal(im.Yres, im2.Yres);
            Assert.Equal(im.Xoffset, im2.Xoffset);
            Assert.Equal(im.Yoffset, im2.Yoffset);
            Assert.Equal(1, im2.Bands);
            Assert.Equal(12, im2.Avg());

            im2 = im.NewFromImage(1, 2, 3);
            Assert.Equal(3, im2.Bands);
            Assert.Equal(2, im2.Avg());
        }

        [Fact]
        public void TestNewFromMemory()
        {
            var s = Enumerable.Repeat((byte)0, 200).ToArray();
            var im = Image.NewFromMemory(s, 20, 10, 1, "uchar");
            Assert.Equal(20, im.Width);
            Assert.Equal(10, im.Height);
            Assert.Equal("uchar", im.Format);
            Assert.Equal(1, im.Bands);
            Assert.Equal(0, im.Avg());

            im += 10;
            Assert.Equal(10, im.Avg());
        }

        [SkippableFact]
        public void TestGetFields()
        {
            Skip.IfNot(NetVips.AtLeastLibvips(8, 5), "requires libvips >= 8.5");

            var im = Image.Black(10, 10);
            var fields = im.GetFields();

            // we might add more fields later
            Assert.True(fields.Length > 10);

            Assert.Equal("width", fields[0]);
        }

        [SkippableFact]
        public void TestGetSuffixes()
        {
            Skip.IfNot(NetVips.AtLeastLibvips(8, 8), "requires libvips >= 8.8");

            var suffixes = NetVips.GetSuffixes();

            // vips supports these file types by default
            // (without being dependent on external dependencies):
            // - Native file format (`*.v`, `*.vips`).
            // - PPM images (`*.ppm`, `*.pgm`, `*.pbm`, `*.pfm`).
            // - Analyze images (`*.hdr`).
            Assert.True(suffixes.Length >= 7);
        }

        [Fact]
        public void TestWriteToMemory()
        {
            var s = Enumerable.Repeat((byte)0, 200).ToArray();
            var im = Image.NewFromMemory(s, 20, 10, 1, "uchar");
            var t = im.WriteToMemory();
            Assert.True(s.SequenceEqual(t));
        }

        [SkippableFact]
        public void TestRegion()
        {
            Skip.IfNot(NetVips.AtLeastLibvips(8, 8), "requires libvips >= 8.8");

            var im = Image.Black(100, 100);
            var region = Region.New(im);
            var data = region.Fetch(0, 0, 10, 10);

            Assert.Equal(10, region.Width);
            Assert.Equal(10, region.Height);
            Assert.True(data.Length == 100);
            Assert.True(data.All(p => p == 0));

            data = region.Fetch(0, 0, 20, 10);

            Assert.Equal(20, region.Width);
            Assert.Equal(10, region.Height);
            Assert.True(data.Length == 200);
            Assert.True(data.All(p => p == 0));
        }

        [Fact]
        public void TestSetProgress()
        {
            var im = Image.NewFromFile(Helper.JpegFile, access: Enums.Access.Sequential);

            var lastPercent = 0;

            var progress = new Progress<int>(percent => lastPercent = percent);
            im.SetProgress(progress);

            var buf = im.DzsaveBuffer("image-pyramid");
            Assert.True(buf.Length > 0);
            Assert.True(lastPercent <= 100);
        }
    }
}