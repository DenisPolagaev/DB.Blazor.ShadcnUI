using DP.ShadcnBlazor;
using Xunit;

namespace DP.ShadcnBlazor.Tests;

/// <summary>
/// Pins the <c>cn()</c> contract: join (clsx) plus the tailwind-merge conflict rules the shadcn
/// class lists rely on when a consumer overrides a class.
/// </summary>
public class ClassMergeTests
{
    [Theory]
    [InlineData("h-9 h-12", "h-12")]
    [InlineData("px-4 px-8", "px-8")]
    [InlineData("px-4 p-2", "p-2")]
    [InlineData("p-2 px-4", "p-2 px-4")]
    [InlineData("m-2 mx-4", "m-2 mx-4")]
    [InlineData("bg-primary bg-secondary", "bg-secondary")]
    [InlineData("text-sm text-primary", "text-sm text-primary")]
    [InlineData("text-sm text-lg", "text-lg")]
    [InlineData("rounded-md rounded-lg", "rounded-lg")]
    [InlineData("rounded-t-md rounded-lg", "rounded-lg")]
    [InlineData("border border-input", "border border-input")]
    [InlineData("border-2 border", "border")]
    [InlineData("border input", "border input")]
    [InlineData("shadow-sm shadow-none", "shadow-none")]
    [InlineData("w-full w-48", "w-48")]
    [InlineData("size-9 size-6", "size-6")]
    [InlineData("flex flex-col", "flex flex-col")]
    [InlineData("flex-1 flex-auto", "flex-auto")]
    [InlineData("grid grid-cols-2", "grid grid-cols-2")]
    [InlineData("justify-center justify-between", "justify-between")]
    [InlineData("gap-2 gap-4", "gap-4")]
    [InlineData("gap-x-2 gap-y-4", "gap-x-2 gap-y-4")]
    [InlineData("hover:bg-primary hover:bg-secondary", "hover:bg-secondary")]
    [InlineData("hover:bg-primary bg-secondary", "hover:bg-primary bg-secondary")]
    [InlineData("data-[state=open]:animate-in animate-in", "data-[state=open]:animate-in animate-in")]
    [InlineData("bg-primary/90 bg-primary/50", "bg-primary/50")]
    [InlineData("-mx-1 -mx-2", "-mx-2")]
    [InlineData("truncate line-clamp-2", "truncate line-clamp-2")]
    [InlineData("overflow-hidden overflow-y-auto", "overflow-hidden overflow-y-auto")]
    public void Merge_ResolvesConflictsInTheSameGroup(string input, string expected) =>
        Assert.Equal(expected, TwMerge.Merge(input));

    [Fact]
    public void Join_SkipsEmptyValues()
    {
        Assert.Equal("a b", Cn.Join("a", "", null, "  ", "b"));
        Assert.Null(Cn.Join(null, "", "   "));
    }

    [Fact]
    public void Merge_KeepsUnknownUtilities()
    {
        Assert.Equal("foo-bar baz-qux", TwMerge.Merge("foo-bar baz-qux"));
    }

    [Fact]
    public void Merge_ConsumerClassWinsOverComponentDefault()
    {
        var merged = Cn.Merge("h-9 px-4 bg-primary", "h-12 px-8 bg-secondary");
        Assert.Equal("h-12 px-8 bg-secondary", merged);
    }
}
