using QuickPulse.Explains.Monastery;
using QuickPulse.Explains.Monastery.Fragments;

namespace QuickPulse.Explains.Tests.Monastery
{
    public class TheCartographerTests
    {
        [Fact]
        public void ChartPath_keeps_a_namespace_prefix_collision_outside_the_root()
        {
            var result = TheCartographer.ChartPath(
                typeof(Doc.RootDocument),
                typeof(Doctor.OutsideDocument));

            Assert.Equal(
                "QuickPulse/Explains/Tests/Monastery/Doctor/OutsideDocument.md",
                result);
        }

        [Fact]
        public void Compose_does_not_treat_a_namespace_prefix_as_a_descendant()
        {
            var book = TheArchivist.Compose<Doc.RootDocument>();

            var page = Assert.Single(book.Pages);
            Assert.Equal(typeof(Doc.RootDocument).Name + ".md", page.Path);
        }

        [Fact]
        public void ChartLinkPath_navigates_between_sibling_namespaces_and_preserves_case()
        {
            var result = TheCartographer.ChartLinkPath(
                typeof(LinkSources.SourceDocument),
                typeof(LinkTargets.TargetDocument));

            Assert.Equal("../LinkTargets/TargetDocument.md", result);
        }

        [Fact]
        public void DocLink_preserves_the_file_path_case_and_formats_only_the_section()
        {
            var attribute = new DocLinkAttribute(
                typeof(LinkTargets.TargetDocument),
                section: "API Details");

            var fragment = Assert.IsType<LinkFragment>(TheArchivist.ToFragment(
                attribute,
                typeof(LinkSources.SourceDocument),
                typeof(LinkSources.SourceDocument)));

            Assert.Equal("../LinkTargets/TargetDocument.md#api-details", fragment.Link);
            Assert.Equal("#api-details", fragment.LocalLink);
        }

        [Theory]
        [InlineData("Fuzzr.Shuffle<T>()", "#fuzzrshufflet")]
        [InlineData("Fuzzr.Shuffle&lt;T&gt;()", "#fuzzrshufflet")]
        public void DocLink_formats_literal_and_encoded_generic_anchors(
            string section,
            string expected)
        {
            var attribute = new DocLinkAttribute(
                typeof(LinkTargets.TargetDocument),
                section: section);

            var fragment = Assert.IsType<LinkFragment>(TheArchivist.ToFragment(
                attribute,
                typeof(LinkSources.SourceDocument),
                typeof(LinkSources.SourceDocument)));

            Assert.Equal(expected, fragment.LocalLink);
        }
    }
}

namespace QuickPulse.Explains.Tests.Monastery.Doc
{
    [DocFile]
    public class RootDocument;
}

namespace QuickPulse.Explains.Tests.Monastery.Doctor
{
    [DocFile]
    public class OutsideDocument;
}

namespace QuickPulse.Explains.Tests.Monastery.LinkSources
{
    public class SourceDocument;
}

namespace QuickPulse.Explains.Tests.Monastery.LinkTargets
{
    public class TargetDocument;
}
